using ProfitCapture.Models;
using ProfitCapture.Parsers;
using ProfitCapture.UI.Template;
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

            Start();
        }

        public void Start()
        {
            if (Running) return;
            if(SelectedTimeline == null) return;

            TimelineThr = new Thread(() => 
            {
                Running = true;

                SelectedTimeline.Periods.Clear();

                AssetQuoteTimelinePoint  previus   = null;
                AssetQuoteTimelinePeriod candle    = null;
                AssetQuoteTimelinePoint  stop_prev = null;
                var stop_time_dur = new DateTime();

                while (Running && SelectedPoint < SelectedTimeline.Points.Count)
                {
                    var point = SelectedTimeline.Points[SelectedPoint];

                    if(previus != null)
                    {
                        if(point.Time >= stop_time_dur)
                        {
                            if(candle != null)
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
                                Time = point.Time,
                            };
                            SelectedTimeline.Periods.Add(candle);
                            PlotCandle(candle, true);
                            stop_prev = null;
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
                                Time = point.Time,
                            };
                            SelectedTimeline.Periods.Add(candle);
                            PlotCandle(candle, true);
                        }
                    }


                    // atualizar vela
                    candle.Points.Add(point);
                    candle.Current = point.Last;

                    if(candle.Current > candle.Max)
                    {
                        candle.Max = candle.Current;
                    }
                    if (candle.Current < candle.Min)
                    {
                        candle.Min = candle.Current;
                    }

                    PlotCandle(candle, false);

                    if (!ProcessEntireTimeline)
                    {
                        TimelineMre.Reset();
                        TimelineMre.Wait(1000);
                    }

                    if(stop_prev == null)
                    {
                        stop_prev     = point;
                        stop_time_dur = point.Time;
                        stop_time_dur.Add(SelectedTimeline.SelectedDuration);
                    }
                    previus = point;
                    SelectedPoint++;
                }
            });
            TimelineThr.Start();
        }

        public void Stop()
        {

        }

        public void PlotCandle(AssetQuoteTimelinePeriod candle, bool add)
        {
            // usar chart para plotar
            // se ADD == true, entao inserir point na serie, senao atualizar indicadores, min, max, value, etc...
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

        internal AssetViewer(Panel asset_panel, Panel timeline_panel, Panel chart_panel)
        {
            TimelineMre = new ManualResetEventSlim();

            AssetGrid    = new DataGridView() { Dock = DockStyle.Fill };
            DatetimeGrid = new DataGridView() { Dock = DockStyle.Fill };
            AssetChart   = new CandleChart()  { Dock = DockStyle.Fill };

            DataGridViewTemplate.EsquemaBrancoLinhaInferior(AssetGrid);
            DataGridViewTemplate.EsquemaBrancoLinhaInferior(DatetimeGrid);

            AssetGrid.Columns.AddRange(DataGridViewTemplate.CriarColunaTexto(nameof(AssetQuote.Rubric), nameof(AssetQuote.Rubric)));
            DatetimeGrid.Columns.AddRange(DataGridViewTemplate.CriarColunaTexto(nameof(AssetQuoteTimeline.Date), nameof(AssetQuoteTimeline.Date)));

            Assets = new BindingList<AssetQuote>();
            AssetGrid.DataSource = Assets;

            Timelines = new BindingList<AssetQuoteTimeline>();
            DatetimeGrid.DataSource = Timelines;

            asset_panel.Controls.Add(AssetGrid);
            timeline_panel.Controls.Add(DatetimeGrid);
            chart_panel.Controls.Add(AssetChart);

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
