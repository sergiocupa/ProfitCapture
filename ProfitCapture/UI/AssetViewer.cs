﻿using ProfitCapture.Models;
using ProfitCapture.Parsers;
using ProfitCapture.UI.Template;
using ProfitCapture.Utils;
using System.ComponentModel;


namespace ProfitCapture.UI
{

    internal class AssetViewer
    {

        public void ViewAssetList()
        {
            var setts = CaptureSetting.Load();
            var at    = AssetQuoteParser.ReadAssembleAssets(setts.CaptureLocation);

            var act = (List<AssetQuote> a) =>
            {
                AssetGrid.SuspendLayout();
                Assets.Clear();
                a.ForEach(b => Assets.Add(b));
                AssetGrid.ResumeLayout();
                AssetGrid.Refresh();
            };

            if(AssetGrid.InvokeRequired)
            {
                AssetGrid.Invoke(() => { act(at); });
            }
            else
            {
                act(at);
            }
        }


        public void ViewDays(AssetQuote asset)
        {
            SelectedAsset = asset;

            var act = (List<AssetQuoteTimeline> tm) =>
            {
                DatetimeGrid.SuspendLayout();
                Timelines.Clear();
                tm.ForEach(a => Timelines.Add(a));
                DatetimeGrid.ResumeLayout();
                DatetimeGrid.Refresh();
            };

            if (AssetGrid.InvokeRequired)
            {
                AssetGrid.Invoke(() => { act(asset.Timelines); });
            }
            else
            {
                act(asset.Timelines);
            }
        }

        public void ViewTimeline(AssetQuoteTimeline timeline)
        {
            SelectedTimeline = timeline;

            if(Principal.CandlePeriods.SelectedIndex >= 0)
            {
                var per = (CandlePeriod)Principal.CandlePeriods.Items[Principal.CandlePeriods.SelectedIndex];
                SelectedTimeline.SelectedDuration = per.Period;
            }

            TimelineMre.Set();
            Running = false;

            Start();
        }

        private void CandlePeriods_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if(SelectedTimeline != null)
            {
                if (Principal.CandlePeriods.SelectedIndex >= 0)
                {
                    var per = (CandlePeriod)Principal.CandlePeriods.Items[Principal.CandlePeriods.SelectedIndex];
                    SelectedTimeline.SelectedDuration = per.Period;
                }

                if (Running)
                {
                    Stop();
                    Start();
                }
            }
        }


        public void Start()
        {
            if (Running) return;
            if(SelectedTimeline == null) return;

            TimelineThr = new Thread(() => 
            {
                try
                {
                    Running = true;

                    // carregar points

                    SelectedTimeline.Points = AssetQuoteParser.ReadAssembleTimelinePoints(SelectedTimeline, 0);
                    var om = SelectedTimeline.Points.OrderBy(o => o.Time).ToList();
                    var first = om.FirstOrDefault();
                    var last = om.LastOrDefault();

                    Principal.label5.Invoke(() =>
                    {
                        Principal.label5.Text = first != null ? first.Time.ToString("dd/MM/yyyy HH:mm:ss") : "###" + " < > " + last != null ? last.Time.ToString("dd/MM/yyyy HH:mm:ss") : "###";
                    });


                    SelectedTimeline.Periods.Clear();

                    AssetQuoteTimelinePoint previus = null;
                    AssetQuoteTimelinePeriod candle = null;
                    AssetQuoteTimelinePoint stop_prev = null;
                    var stop_time_dur = new DateTime();

                    var ord = SelectedTimeline.Points.OrderBy(o => o.Last);
                    var min = ord.FirstOrDefault();
                    var max = ord.LastOrDefault();
                    var vmin = 0.0;
                    var vmax = 0.0;
                    if (ord.Count() > 1)
                    {
                        vmin = (double)min.Last;
                        vmax = (double)max.Last;
                    }
                    else if (ord.Count() == 1)
                    {
                        vmin = (double)max.Last;
                        vmax = (double)max.Last;
                    }
                    AssetChart.ResetY(vmin, vmax);

                    while (Running && SelectedPoint < SelectedTimeline.Points.Count)
                    {
                        var point = SelectedTimeline.Points[SelectedPoint];
                        var ti = CandlePeriod.RoundToNearestInterval(point.Time, SelectedTimeline.SelectedDuration);

                        // Usar exemplo do chatgpt
                        bool newc = false;
                        if (previus != null)
                        {
                            if (point.Time >= stop_time_dur)
                            {
                                if (candle != null)
                                {
                                    candle.Close = point.Last;
                                }

                                // Criar vela
                                candle = new AssetQuoteTimelinePeriod()
                                {
                                    Open = point.Last,
                                    Current = point.Last,
                                    Duration = SelectedTimeline.SelectedDuration,
                                    Index = (ulong)SelectedTimeline.Periods.Count,
                                    Time = ti,
                                    Min = point.Last,
                                    Max = point.Last,
                                    Close = point.Last
                                };

                                stop_time_dur = CandlePeriod.AddInterval(point.Time, SelectedTimeline.SelectedDuration);

                                SelectedTimeline.Periods.Add(candle);
                                PlotCandle(candle, true);


                                //var media1 = MediaMovel(9, SelectedTimeline.Periods);
                                //PlotLine(ti, media1, true);
                                stop_prev = null;
                                newc = true;
                            }
                        }
                        else
                        {
                            if (stop_prev == null)
                            {
                                // Criar vela
                                candle = new AssetQuoteTimelinePeriod()
                                {
                                    Open = point.Last,
                                    Current = point.Last,
                                    Duration = SelectedTimeline.SelectedDuration,
                                    Index = (ulong)SelectedTimeline.Periods.Count,
                                    Time = ti,
                                    Min = point.Last,
                                    Max = point.Last,
                                    Close = point.Last
                                };

                                stop_time_dur = CandlePeriod.AddInterval(point.Time, SelectedTimeline.SelectedDuration);

                                SelectedTimeline.Periods.Add(candle);
                                PlotCandle(candle, true);


                                //var media1 = MediaMovel(9, SelectedTimeline.Periods);
                                //PlotLine(ti, media1, true);
                                newc = true;
                            }
                        }

                        // atualizar vela
                        candle.Points.Add(point);
                        candle.Close = point.Last;
                        candle.Current = point.Last;

                        if (candle.Current > candle.Max)
                        {
                            candle.Max = candle.Current;
                        }
                        if (candle.Current < candle.Min)
                        {
                            candle.Min = candle.Current;
                        }

                        Principal.label6.Invoke(() =>
                        {
                            Principal.label6.Text = "Count: " + SelectedPoint + " | Current: " + point.Last + " | Min: " + candle.Min + " | Max: " + candle.Max;
                        });

                        PlotCandle(candle, false);



                        var media = MediaMovel(9, SelectedTimeline.Periods);

                        PlotLine(ti, media, newc);



                        if (!ProcessEntireTimeline)
                        {
                            TimelineMre.Reset();
                            TimelineMre.Wait(100);
                        }

                        if (stop_prev == null)
                        {
                            stop_prev = point;
                        }
                        previus = point;
                        SelectedPoint++;
                    }
                }
                catch (Exception ex)
                {
                    Principal.Invoke(() =>
                    {
                        MessageBox.Show(ex.Message);
                    });
                }
            });
            TimelineThr.Start();
        }


        decimal MediaMovel(int periodos, List<AssetQuoteTimelinePeriod> candles)
        {
            var mi = 0.0m;
            int iz = candles.Count;
            int ix = 0;
            while(ix < periodos && iz > 0)
            {
                iz--;
                var cand = candles[iz];
                var me = cand.Points.Sum(s => s.Last) / (decimal)cand.Points.Count;
                mi += me;
                ix++;
            }
            var average = mi / (decimal)ix;
            return average;
        }


        public void Stop()
        {
            Running = false;
            TimelineMre.Set();
        }

        public void PlotLine(DateTime x, decimal y, bool add)
        {
            if (!AcumuladorNext)
            {
                AcumuladorNext = true;
                Acumulador = y;
            }

            Acumulador = MathFunc.Integrate(y, Acumulador, 200, 1);
            AssetChart.Append(x, (double)Acumulador, 1, !add);
        }

        public void PlotCandle(AssetQuoteTimelinePeriod candle, bool add)
        {
            AssetChart.Append(candle.Time, (double)candle.Open, (double)candle.Close, (double)candle.Min, (double)candle.Max, !add);

            //Console.WriteLine("Time: " + candle.Time.ToString("HH:mm:ss") + " | " +
            //                  "Open: " + candle.Open + " | " +
            //                  "Close: " + candle.Close + " | " +
            //                  "Min: " + candle.Min + " | " +
            //                  "Max: " + candle.Max);
        }


        public bool ProcessEntireTimeline;
        public bool Speed;

        int SelectedPoint;
        AssetQuote                      SelectedAsset;
        AssetQuoteTimeline              SelectedTimeline;
        ManualResetEventSlim            TimelineMre;
        Thread                          TimelineThr;
        bool                            Running;
        BindingList<AssetQuote>         Assets;
        BindingList<AssetQuoteTimeline> Timelines;
        DataGridView                    AssetGrid;
        DataGridView                    DatetimeGrid;
        CandleChart                     AssetChart;
        Form1                           Principal;
        LogConsole                      Log;

        decimal Acumulador;
        bool AcumuladorNext;

        internal AssetViewer(Form1 principal, Panel asset_panel, Panel timeline_panel, Panel chart_panel)
        {
            Principal = principal;

            Principal.CandlePeriods.DataSource    = CandlePeriod.GetDefaultPeriods();
            Principal.CandlePeriods.DisplayMember = nameof(CandlePeriod.Name);
            Principal.CandlePeriods.SelectedIndex = 0;
            Principal.CandlePeriods.SelectedIndexChanged += CandlePeriods_SelectedIndexChanged;

            TimelineMre = new ManualResetEventSlim();

            AssetGrid    = new DataGridView() { Dock = DockStyle.Fill };
            DatetimeGrid = new DataGridView() { Dock = DockStyle.Fill };
            AssetChart   = new CandleChart()  { Dock = DockStyle.Fill };

            AssetChart.AddSerie("Media");

            DataGridViewTemplate.EsquemaBrancoLinhaInferior(AssetGrid);
            DataGridViewTemplate.EsquemaBrancoLinhaInferior(DatetimeGrid);
            AssetGrid.BackgroundColor = Color.FromArgb(60, 60, 60);
            AssetGrid.RowsDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);
            DatetimeGrid.BackgroundColor = Color.FromArgb(60, 60, 60);
            DatetimeGrid.RowsDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);



            AssetGrid.Columns.AddRange(DataGridViewTemplate.CriarColunaTexto(nameof(AssetQuote.Rubric), nameof(AssetQuote.Rubric)));
            DatetimeGrid.Columns.AddRange(DataGridViewTemplate.CriarColunaTexto(nameof(AssetQuoteTimeline.Date), nameof(AssetQuoteTimeline.Date)));

            Assets = new BindingList<AssetQuote>();
            AssetGrid.DataSource = Assets;

            Timelines = new BindingList<AssetQuoteTimeline>();
            DatetimeGrid.DataSource = Timelines;

            asset_panel.Controls.Add(AssetGrid);
            timeline_panel.Controls.Add(DatetimeGrid);
            chart_panel.Controls.Add(AssetChart);

            //AssetChart.Demo();

            AssetGrid.CellContentClick += (object? sender, DataGridViewCellEventArgs e) =>
            {
                if(AssetGrid.SelectedRows.Count > 0)
                {
                    var sa = AssetGrid.SelectedRows[0].DataBoundItem as AssetQuote;
                    if(sa != null)
                    {
                        ViewDays(sa);
                    }
                }
            };
            DatetimeGrid.CellContentClick += (object? sender, DataGridViewCellEventArgs e) =>
            {
                if (DatetimeGrid.SelectedRows.Count > 0)
                {
                    var sa = DatetimeGrid.SelectedRows[0].DataBoundItem as AssetQuoteTimeline;
                    if (sa != null)
                    {
                        ViewTimeline(sa);
                    }
                }
            };
        }

       
    }




}
