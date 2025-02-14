using ProfitCapture.Models;
using ProfitCapture.Parsers;
using ProfitCapture.UI.Template;
using System.ComponentModel;
using System.Windows.Forms.DataVisualization.Charting;


namespace ProfitCapture.UI
{

    internal class AssetViewer
    {

        public void ViewAssetList()
        {
            // Implementar carregamente de markups, se existe para a data correspondente ao arquivo de captura
            

            var setts = CaptureSetting.Load();
            var at    = AssetQuoteParser.ReadAssembleAssets(setts.GetCompleteCaptureLocation());

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
                AssetChart.SetStep(per.Period);
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
                    AssetChart.SetStep(per.Period);
                }

                if (Running)
                {
                    Stop();
                    Start();
                }
            }
        }


        private void ProtCandles(AssetQuoteTimelinePeriod candle, bool newc)
        {
            PlotCandle(candle, newc);

            var media9  = MediaMovel(9, candle.Index, SelectedTimeline.Periods);
            var media21 = MediaMovel(21, candle.Index, SelectedTimeline.Periods);

            PlotLine01(candle.Time, media9, newc);
            PlotLine02(candle.Time, media21, newc);
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

                    ProcessEntireTimeline = true;

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

                    AssetQuoteTimelinePoint point = null;

                    while (Running && SelectedPoint < SelectedTimeline.Points.Count)
                    {
                        point = SelectedTimeline.Points[SelectedPoint];
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

                        if (!ProcessEntireTimeline)
                        {
                            Principal.label6.Invoke(() =>
                            {
                                Principal.label6.Text = "Count: " + SelectedPoint + "/" + SelectedTimeline.Points.Count + " | Current: " + point.Last + " | Min: " + candle.Min + " | Max: " + candle.Max;
                            });

                            ProtCandles(candle, newc);

                            TimelineMre.Reset();
                            TimelineMre.Wait(1);
                        }

                        if (stop_prev == null)
                        {
                            stop_prev = point;
                        }
                        previus = point;
                        SelectedPoint++;
                    }

                    Principal.label6.Invoke(() =>
                    {
                        Principal.label6.Text = "Count: " + SelectedPoint + "/" + SelectedTimeline.Points.Count + " | Current: " + point.Last + " | Min: " + candle.Min + " | Max: " + candle.Max;
                    });

                    if (ProcessEntireTimeline)
                    {
                        int ix = 0;
                        while (ix < SelectedTimeline.Periods.Count)
                        {
                            var cand = SelectedTimeline.Periods[ix];
                            ProtCandles(cand, true);
                            ix++;
                        }
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



        private void AssetChart_SelectedCandle(CandlePoint obj)
        {
            //DateTime time = DateTime.FromOADate(obj.XValue);
            //MessageBox.Show($"{obj.Name}:\nHora: {time:HH:mm:ss}\nMáxima: {obj.YValues[0]:F2}\nMínima: {obj.YValues[1]:F2}");

            if (SelectedTimeline != null)
            {
                if (Principal.SalvarMarcacoes.Checked)
                {
                    AssetChart.AddSerie("Marking", SeriesChartType.Point);
                    SelectedTimeline.PrepareMarking();

                    var data = (AssetQuoteTimelinePeriod)obj.Data;

                    var posit = !SelectedTimeline.IsLongTrade ? data.Open : data.Close;
                    AssetChart.Append(data.Time, (double)posit, "Marking");

                    var pos = SelectedTimeline.Markings.Where(w => w.Index == data.Index).FirstOrDefault();

                    if(pos != null)
                    {
                        if(pos.Note == null) pos.Note = new Note();

                        if(!SelectedTimeline.IsLongTrade)
                        {
                            pos.Note.Transact = TransactOption.Buy;
                        }
                        else
                        {
                            pos.Note.Transact = TransactOption.Sell;

                            // Linkar marcaçoes
                            AssetChart.AddSerie(pos.Note.UID, SeriesChartType.Line);
                            AssetChart.Append(SelectedTimeline.PreviusSelectedCandle.Time, (double)SelectedTimeline.PreviusSelectedCandle.Open, pos.Note.UID);
                            AssetChart.Append(data.Time, (double)data.Close, pos.Note.UID);

                            // Apresentar faixa
                        }

                        SelectedTimeline.SaveMarkings();
                    }

                    SelectedTimeline.IsLongTrade = !SelectedTimeline.IsLongTrade;
                    SelectedTimeline.PreviusSelectedCandle = data;
                }
            }
        }




        decimal MediaMovel(int periodos, ulong index, List<AssetQuoteTimelinePeriod> candles)
        {
            if(periodos <= 0)
            {
                periodos = 1;
            }

            if(index <= 0)
            {
                return (candles.Count > 0) ? candles[0].Close : 0;
            }

            var mi = 0.0m;
            int sm = 0;
            int ix = (int)index;
            while(ix > 0 && sm < periodos)
            {
                var cand = candles[ix];
                var me = cand.Close;
                mi += me;
                ix--;
                sm++;
            }
            var average = mi / (decimal)sm;
            return average;
        }


        public void Stop()
        {
            Running = false;
            TimelineMre.Set();
        }

        public void PlotLine01(DateTime x, decimal y, bool add)
        {
            AssetChart.Append(x, (double)y, _serie_id: 1, is_update: !add);
        }
        public void PlotLine02(DateTime x, decimal y, bool add)
        {
            AssetChart.Append(x, (double)y, _serie_id: 2, is_update: !add);
        }

        public void PlotCandle(AssetQuoteTimelinePeriod candle, bool add)
        {
            AssetChart.Append(candle.Time, (double)candle.Open, (double)candle.Close, (double)candle.Min, (double)candle.Max, candle, !add);
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

            AssetChart.AddSerie("Media9");
            AssetChart.AddSerie("Media21");

            AssetChart.SelectedCandle += AssetChart_SelectedCandle;

            DataGridViewTemplate.EsquemaBrancoLinhaAlternada(AssetGrid, true);
            DataGridViewTemplate.EsquemaBrancoLinhaAlternada(DatetimeGrid, true);
            //AssetGrid.BackgroundColor = Color.FromArgb(60, 60, 60);
            //AssetGrid.RowsDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);
            //DatetimeGrid.BackgroundColor = Color.FromArgb(60, 60, 60);
            //DatetimeGrid.RowsDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);



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

            AssetGrid.CellClick += (object? sender, DataGridViewCellEventArgs e) =>
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

            DatetimeGrid.CellClick += (object? sender, DataGridViewCellEventArgs e) =>
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
