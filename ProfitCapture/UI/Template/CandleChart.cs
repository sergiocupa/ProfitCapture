using ProfitCapture.Utils;
using System.Windows.Forms.DataVisualization.Charting;


namespace ProfitCapture.UI.Template
{

    public partial class CandleChart : UserControl
    {


        public void Append(DateTime x, double open, double close, double low, double high, object data, bool is_update = false, string serie_name = null, int serie_id = 0, Color? color = null)
        {
            if(InvokeRequired)
            {
                Invoke(() => { AppendInvoke(x, open, close, low, high, data, is_update, serie_id, serie_name, color); });
            }
            else
            {
                AppendInvoke(x, open, close, low, high, data, is_update, serie_id, serie_name, color);
            }
        }

        private void AppendInvoke(DateTime x, double open, double close, double low, double high, object data, bool is_update, int _serie_id, string serie_name, Color? color)
        {
            try
            {
                Series serie = null;

                if(!string.IsNullOrEmpty(serie_name))
                {
                    serie = Chart.Series.Where(w => w.Name == serie_name).FirstOrDefault();
                }
                if(serie == null && _serie_id >= 0 && _serie_id < Chart.Series.Count)
                {
                    serie = Chart.Series[_serie_id];
                }

                if (serie != null)
                {
                    UpdateY(low,high);

                    if(is_update)
                    {
                        var pt = serie.Points.LastOrDefault();
                        if(pt != null)
                        {
                            var pp = (CandlePoint)pt;
                            pp.XValue = x.ToOADate();
                            pp.InputX = x;
                            pp.YValues = new double[] { low, high, open, close };
                        }
                    }
                    else
                    {
                        var dp = new CandlePoint() { InputX = x, XValue = x.ToOADate(), YValues = new double[] { low, high, open, close }, Serie = serie, Data = data };
                        if (color.HasValue)
                        {
                            dp.Color = color.Value;
                        }

                        serie.Points.Add(dp);
                    }

                    UpdateRangeX(x, serie);
                }
            }
            catch (Exception ex) 
            { 
                Console.Error.WriteLine(ex.ToString());
            }
        }


        public void Append(DateTime x, double y, string? serie_name = null, int _serie_id = 0, bool is_update = false, Color? color = null)
        {
            try
            {
                Series serie = null;

                if (!string.IsNullOrEmpty(serie_name))
                {
                    serie = Chart.Series.Where(w => w.Name == serie_name).FirstOrDefault();
                }
                if (serie == null && _serie_id >= 0 && _serie_id < Chart.Series.Count)
                {
                    serie = Chart.Series[_serie_id];
                }

                if (serie != null)
                {
                    //UpdateY(y);

                    if (is_update)
                    {
                        var pt = serie.Points.LastOrDefault();
                        if (pt != null)
                        {
                            var pp = (CandlePoint)pt;
                            pp.XValue = x.ToOADate();
                            pp.InputX = x;
                            pp.YValues = new double[] { y };
                        }
                    }
                    else
                    {
                        var dp = new CandlePoint() { InputX = x, XValue = x.ToOADate(), YValues = new double[] { y } };
                        if (color.HasValue)
                        {
                            dp.Color = color.Value;
                        }

                        serie.Points.Add(dp);
                    }

                    //UpdateRangeX(x, serie);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }


        public void UpdateRangeX(DateTime x, Series serie)
        {
            if (serie.Points.Count >= 2)
            {
                var first = (CandlePoint)serie.Points.FirstOrDefault();
                var last  = (CandlePoint)serie.Points.LastOrDefault();
                var dif   = last.InputX.Subtract(first.InputX);

                if (dif > SizeX)
                {
                    // Plota deslocando
                    var a = x.Subtract(SizeX);

                    if(Step.HasValue)
                    {
                        a = a.Subtract(Step.Value);
                    }

                    Area.AxisX.Minimum = a.Add(OffsetX).ToOADate();
                    Area.AxisX.Maximum = x.Add(OffsetX).ToOADate();
                }
                else
                {
                    // Plota ate fim da tela
                    var a = first.InputX;
                    if (Step.HasValue)
                    {
                        a = a.Subtract(Step.Value);
                    }
                    Area.AxisX.Minimum = a.Add(OffsetX).ToOADate();

                    var m = first.InputX.Add(SizeX);
                    Area.AxisX.Maximum = m.Add(OffsetX).ToOADate();
                }
            }
            else
            {
                var first = (CandlePoint)serie.Points.FirstOrDefault();
                if (first != null)
                {
                    // Plota ate fim da tela
                    var a = first.InputX;
                    if (Step.HasValue)
                    {
                        a = a.Subtract(Step.Value);
                    }
                    Area.AxisX.Minimum = a.Add(OffsetX).ToOADate();

                    var m = first.InputX.Add(SizeX).Add(OffsetX);
                    Area.AxisX.Maximum = m.Add(OffsetX).ToOADate();
                }
            }
        }


        public void SetStep(TimeSpan? step)
        {
            Step = step;
        }


        public void ResetY(double min, double max)
        {
            Chart.ChartAreas[0].AxisY2.Minimum = min;
            Chart.ChartAreas[0].AxisY2.Maximum = max;
        }


        private void UpdateY(double value)
        {
            if (value > Chart.ChartAreas[0].AxisY2.Maximum)
            {
                Chart.ChartAreas[0].AxisY2.Maximum = value;
            }
            if (value < Chart.ChartAreas[0].AxisY2.Minimum)
            {
                Chart.ChartAreas[0].AxisY2.Minimum = value;
            }
        }

        private void UpdateY(double min, double max)
        {
            if (max > Chart.ChartAreas[0].AxisY2.Maximum)
            {
                Chart.ChartAreas[0].AxisY2.Maximum = max;
            }
            if (min < Chart.ChartAreas[0].AxisY2.Minimum)
            {
                Chart.ChartAreas[0].AxisY2.Minimum = min;
            }
        }


        public void Demo()
        {
            var dt = DateTime.Now;
            var b = dt.AddMinutes(1);

            SetStep(new TimeSpan(0, 1, 0));

            Append(dt.AddMinutes(1), 82,  94,  80,  100, null);
            Append(dt.AddMinutes(2), 105, 106, 100, 110, null);
            Append(dt.AddMinutes(3), 106, 119, 100, 120, null);
            Append(dt.AddMinutes(4), 120, 121, 119, 125, null);
            Append(dt.AddMinutes(5), 125, 128, 120, 130, null);
            Append(dt.AddMinutes(6), 116, 114, 110, 120, null);
        }


        public void AddSerie(string name, SeriesChartType type = SeriesChartType.Line, Color? color = null)
        {
            var ex = Chart.Series.Where(w => w.Name == name).FirstOrDefault();
            if (ex == null)
            {
                var s = new Series();

                s.Name = name;
                s.ChartType = type;
                s.BorderWidth = 1;
                s.YAxisType = AxisType.Secondary;

                s.IsXValueIndexed = false;

                if (type == SeriesChartType.Candlestick)
                {
                    // Configuração do estilo de vela
                    //s["OpenCloseStyle"] = "Triangle"; // Forma dos marcadores de abertura/fechamento
                    s["OpenCloseStyle"] = "Candlestick"; // Forma dos marcadores de abertura/fechamento
                    s["ShowOpenClose"]  = "Both"; // Mostra abertura e fechamento
                    s["PointWidth"]     = "0.6"; // Largura das velas

                    // Cores de alta e baixa
                    s["PriceUpColor"]   = "#60FFBB"; // Alta
                    s["PriceDownColor"] = "#FFBB60"; // Baixa
                }
                else
                {
                    s.Color = color.HasValue ? color.Value : ColorGenerator.Create();

                    if (type == SeriesChartType.Point)
                    {
                        s.MarkerStyle = MarkerStyle.Circle;
                        s.MarkerSize  = 6;
                    }
                }
                Chart.Series.Add(s);
            }
        }


        private void Chart_MouseWheel(object? sender, MouseEventArgs e)
        {
            try
            {
                Area.RecalculateAxesScale();


                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)// Eixo X
                {
                    DateTime minX = DateTime.FromOADate(Area.AxisX.Minimum);
                    DateTime maxX = DateTime.FromOADate(Area.AxisX.Maximum);

                    TimeSpan difX = maxX - minX; // Calcula o intervalo de tempo atual

                    if (e.Delta > 0) // Zoom in no eixo X (reduz intervalo)
                    {
                        DateTime novoMinX = minX.AddMilliseconds(difX.TotalMilliseconds / 6.0);
                        DateTime novoMaxX = maxX.AddMilliseconds(-difX.TotalMilliseconds / 6.0);

                        if (novoMinX < novoMaxX)
                        {
                            SizeX = novoMaxX.Subtract(novoMinX);

                            // Converte de volta para OADate antes de atribuir
                            Area.AxisX.Minimum = novoMinX.ToOADate();
                            Area.AxisX.Maximum = novoMaxX.ToOADate();
                        }
                    }
                    else if (e.Delta < 0)
                    {
                        DateTime novoMinX = minX.AddMilliseconds(-difX.TotalMilliseconds / 2.0);
                        DateTime novoMaxX = maxX.AddMilliseconds(difX.TotalMilliseconds / 2.0);

                        SizeX = novoMaxX.Subtract(novoMinX);

                        // Converte de volta para OADate antes de atribuir
                        Area.AxisX.Minimum = novoMinX.ToOADate();
                        Area.AxisX.Maximum = novoMaxX.ToOADate();
                    }
                }
                else// Eixo Y
                {
                    if (e.Delta > 0) // Zoom in (reduz intervalo)
                    {
                        var dif = (Area.AxisY2.Maximum - Area.AxisY2.Minimum) / 6.0;

                        double novoMin = Area.AxisY2.Minimum + dif;
                        double novoMax = Area.AxisY2.Maximum - dif;

                        if (novoMin < novoMax)
                        {
                            Area.AxisY2.Minimum = novoMin;
                            Area.AxisY2.Maximum = novoMax;
                        }
                    }
                    else if (e.Delta < 0) // Zoom out (aumenta intervalo)
                    {
                        var dif = (Area.AxisY2.Maximum - Area.AxisY2.Minimum) / 2.0;

                        Area.AxisY2.Maximum += dif;
                        Area.AxisY2.Minimum -= dif;
                    }
                }

                Area.RecalculateAxesScale();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        // Testar todas as series para ver qual mais proximo
        // Ignorar se: mouse movimentando
        private DataPoint? GetNearestCandle(Chart chart, int mouseX, int mouseY, int serie_index, ref double min_distance)
        {
            DataPoint? nearestPoint = null;

            double xValue = Area.AxisX.PixelPositionToValue(mouseX);
            double yValue = Area.AxisY2.PixelPositionToValue(mouseY);

            Series series = chart.Series[serie_index];
            double distance = double.MaxValue;
            bool setted = false;

            if (series.ChartType == SeriesChartType.Candlestick)
            {
                foreach (DataPoint candle in series.Points)
                {
                    double xDistance = Math.Abs(candle.XValue - xValue);

                    double high = candle.YValues[1];
                    double low = candle.YValues[0];

                    if (yValue >= low && yValue <= high && xDistance < distance)
                    {
                        distance     = xDistance;
                        nearestPoint = candle;
                        setted       = true;
                    }
                }
            }
            else
            {
                foreach (DataPoint point in series.Points)
                {
                    // Calcula a distância entre o ponto clicado e cada ponto da série
                    double d = Math.Sqrt(Math.Pow(point.XValue - xValue, 2) + Math.Pow(point.YValues[0] - yValue, 2));

                    if (d < distance)
                    {
                        distance     = d;
                        nearestPoint = point;
                        setted       = true;
                    }
                }
            }

            if (setted) min_distance = distance;

            return nearestPoint;
        }


        private void ChartClick(int x, int y)
        {
            var list = new List<DataPointDistance>();
            double distance = 0;

            int ix = 0;
            while (ix < Chart.Series.Count)
            {
                DataPoint? dp = GetNearestCandle(Chart, x, y, ix, ref distance);
                if (dp != null)
                {
                    var ob = new DataPointDistance() { Point = dp, Distance = distance, SerieIndex = ix };
                    list.Add(ob);
                }
                ix++;
            }

            var ol = list.OrderBy(s => s.Distance).FirstOrDefault();
            if (ol != null)
            {
                var se = Chart.Series[ol.SerieIndex];

                if (se.ChartType == SeriesChartType.Candlestick)
                {
                    if (SelectedCandle != null) SelectedCandle((CandlePoint)ol.Point);
                }
                else
                {
                    //MessageBox.Show($"{se.Name}:\nHora: {time:HH:mm:ss}\nValor: {ol.Point.YValues[0]:F2}");
                }
            }
        }


        private void Chart_MouseMove(object? sender, MouseEventArgs e)
        {
            DateTime novoMinX = new DateTime(0);
            DateTime novoMaxX;

            try
            {
                if (IsDragging)
                {
                    Area.RecalculateAxesScale();

                    // Y
                    double rangeY = Area.AxisY2.Maximum - Area.AxisY2.Minimum;
                    double moveFactorY = (e.Y - LastMouseY) * (rangeY / 500.0);
                    Area.AxisY2.Minimum += moveFactorY;
                    Area.AxisY2.Maximum += moveFactorY;
                    LastMouseY = e.Y;


                    // X
                    DateTime minX = DateTime.FromOADate(Area.AxisX.Minimum);

                    if (!Step.HasValue) Step = new TimeSpan(0, 1, 0);

                    var PixelsPerStep = (double)Width / ((double)SizeX.Ticks / (double)Step.Value.Ticks);
                    var Displacement  = e.X - LastMouseX;

                    if (Displacement >= PixelsPerStep)
                    {
                        novoMinX = minX.Subtract(Step.Value);
                        LastMouseX = e.X;

                        Area.AxisX.Minimum = novoMinX.ToOADate();
                        novoMaxX = novoMinX.Add(SizeX);
                        Area.AxisX.Maximum = novoMaxX.ToOADate();
                    }
                    else if(Displacement <= (-PixelsPerStep))
                    {
                        novoMinX = minX.Add(Step.Value);
                        LastMouseX = e.X;

                        Area.AxisX.Minimum = novoMinX.ToOADate();
                        novoMaxX = novoMinX.Add(SizeX);
                        Area.AxisX.Maximum = novoMaxX.ToOADate();
                    }

                    Area.RecalculateAxesScale();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private void Chart_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsDragging  = true;
                LastMouseY  = e.Y;
                LastMouseX  = e.X;
                StartMouseX = e.X;
                StartMouseY = e.Y;
            }
        }

        private void Chart_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsDragging = false;

                var md = (Math.Abs(StartMouseX - e.X) + Math.Abs(StartMouseY - e.Y)) / 2.0;
                if(md < 2)
                {
                    ChartClick(e.X,e.Y);
                }
            }
        }



        public void InitAxisX()
        {
            var d1 = DateTime.Now;
            var d2   = d1;
            d2.Add(SizeX);

            Area.AxisX.Maximum = d1.ToOADate();
            Area.AxisX.Minimum = d2.ToOADate();
        }

        public void Init()
        {
            SizeX   = new TimeSpan(1, 0, 0);
            OffsetX = new TimeSpan(0);

            Chart = new Chart() { BackColor = Color.FromArgb(70,70,80), Dock = DockStyle.Fill };
            Controls.Add(Chart);

            Chart.MouseWheel += Chart_MouseWheel;
            Chart.MouseUp    += Chart_MouseUp;
            Chart.MouseDown  += Chart_MouseDown;
            Chart.MouseMove  += Chart_MouseMove;

            Area = new ChartArea() { BackColor = Color.FromArgb(70, 70, 80) };
            Area.Name = "CandleArea";
            Chart.ChartAreas.Add(Area);

            Area.Position = new ElementPosition(0, 1, 101, 100);

            // Configuração do eixo X
            Area.AxisX.Interval                 = 1;
            Area.AxisX.IntervalType             = DateTimeIntervalType.Minutes;  
            Area.AxisX.MajorGrid.LineColor      = System.Drawing.Color.LightGray;
            Area.AxisX.LineWidth                = 0;
            Area.AxisX.MajorGrid.Enabled        = false; // Remove linhas principais
            Area.AxisX.MinorGrid.Enabled        = false; // Remove linhas secundárias
            //Area.AxisX2.LineWidth               = 0;
            Area.AxisX.LabelStyle.Format        = "HH:mm";
            Area.AxisX.LabelStyle.ForeColor     = Color.FromArgb(160, 160, 160);

            Area.AxisX.IsMarginVisible = false;

            // Alinha os valores do eixo Y à esquerda
            Area.AxisY.Enabled                  = AxisEnabled.False;
            Area.AxisY.Maximum                  = 50;
            Area.AxisY.Minimum                  = 0;
            Area.AxisY.LabelStyle.Format        = "F2";
            Area.AxisY.LineWidth                = 0;
           
            // Alinha os valores do eixo Y à direita
            Area.AxisY2.Enabled                 = AxisEnabled.True; // Habilita o segundo eixo Y
            Area.AxisY2.LabelStyle.Enabled      = true; // Exibe os rótulos no eixo Y2
            Area.AxisY2.LabelStyle.Format       = "F2"; // Formato dos rótulos
            Area.AxisY2.LineWidth               = 0; // Remove a linha do eixo Y2, se desejado
            Area.AxisY2.MajorGrid.LineColor     = Color.FromArgb(120,120,120);
            Area.AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            Area.AxisY2.LabelStyle.ForeColor    = Color.FromArgb(160, 160, 160);
            Area.AxisY2.LabelStyle.Font         = new Font("Segoe UI", 8);
            Area.AxisY2.MajorGrid.Enabled       = true; // Remove as linhas de grade do eixo Y2
            Area.AxisY2.MajorTickMark.Enabled   = false; // Habilita as marcas de tique
            Area.AxisY2.IsLogarithmic           = false;
            Area.AxisY2.IsStartedFromZero       = false;
            Area.AxisY2.IsMarginVisible         = false;

            AddSerie("Candles", SeriesChartType.Candlestick);

            InitAxisX();
        }



        public event Action<CandlePoint> SelectedCandle;
        private TimeSpan? Step;
        private ChartArea Area;
        private TimeSpan SizeX;
        private TimeSpan OffsetX;
        private Chart Chart;
        private double LastMouseY;
        private double LastMouseX;
        private double StartMouseX;
        private double StartMouseY;
        private bool IsDragging = false;


        public CandleChart()
        {
            InitializeComponent();
            Init();
        }
    }

    public class CandlePoint : DataPoint
    {
        public DateTime InputX;
        public Series Serie;
        public object Data;
    }


    public class DataPointDistance
    {
        public DataPoint Point { get; set; }
        public double Distance { get; set; }
        public int SerieIndex { get; set; }
    }


}
