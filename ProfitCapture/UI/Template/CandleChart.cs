using ProfitCapture.Utils;
using System.Windows.Forms.DataVisualization.Charting;


namespace ProfitCapture.UI.Template
{

    public partial class CandleChart : UserControl
    {

        public void Append(DateTime x, double open, double close, double low, double high, TimeSpan? step = null, bool is_update = false, int serie = 0, Color? color = null)
        {
            try
            {
                if (serie >= 0 && serie < Chart.Series.Count)
                {
                    UpdateY(low,high);

                    if(is_update)
                    {
                        var pt = Chart.Series[serie].Points.LastOrDefault();
                        if(pt != null)
                        {
                            var pp = (CandlePoint)pt;
                            pp.XValue  = x.ToOADate();
                            pp.InputX  = x;
                            pp.YValues = new double[] { low, high, open, close };
                        }
                    }
                    else
                    {
                        var dp = new CandlePoint() { InputX = x, XValue = x.ToOADate(), YValues = new double[] { low, high, open, close } };
                        if (color.HasValue)
                        {
                            dp.Color = color.Value;
                        }

                        Chart.Series[serie].Points.Add(dp);
                    }

                    UpdateRangeX(x, serie, step);
                }
            }
            catch (Exception) { }
        }


        public void Append(DateTime x, double y, int serie = 0, TimeSpan? step = null, bool is_update = false, Color? color = null)
        {
            try
            {
                if (serie >= 0 && serie < Chart.Series.Count)
                {
                    UpdateY(y);

                    if (is_update)
                    {
                        var pt = Chart.Series[serie].Points.LastOrDefault();
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

                        Chart.Series[serie].Points.Add(dp);
                    }

                    UpdateRangeX(x, serie, step);
                }
            }
            catch (Exception) { }
        }


        public void UpdateRangeX(DateTime x, int serie, TimeSpan? step)
        {
            if (Chart.Series[serie].Points.Count >= 2)
            {
                var first = (CandlePoint)Chart.Series[serie].Points.FirstOrDefault();
                var last  = (CandlePoint)Chart.Series[serie].Points.LastOrDefault();
                var dif   = last.InputX.Subtract(first.InputX);

                if (dif > SizeX)
                {
                    // Plota deslocando
                    var a = x.Subtract(SizeX);

                    if(step.HasValue)
                    {
                        a = a.Subtract(step.Value);
                    }

                    Area.AxisX.Minimum = a.ToOADate();
                    Area.AxisX.Maximum = x.ToOADate();
                }
                else
                {
                    // Plota ate fim da tela
                    var a = first.InputX;
                    if (step.HasValue)
                    {
                        a = a.Subtract(step.Value);
                    }
                    Area.AxisX.Minimum = a.ToOADate();

                    var m = first.InputX.Add(SizeX);
                    Area.AxisX.Maximum = m.ToOADate();
                }
            }
            else
            {
                var first = (CandlePoint)Chart.Series[serie].Points.FirstOrDefault();
                if (first != null)
                {
                    // Plota ate fim da tela
                    var a = first.InputX;
                    if (step.HasValue)
                    {
                        a = a.Subtract(step.Value);
                    }
                    Area.AxisX.Minimum = a.ToOADate();

                    var m = first.InputX.Add(SizeX);
                    Area.AxisX.Maximum = m.ToOADate();
                }
            }
        }


        private void UpdateY(double value)
        {
            if (value > Chart.ChartAreas[0].AxisY.Maximum)
            {
                Chart.ChartAreas[0].AxisY.Maximum = value;
            }
            if (value < Chart.ChartAreas[0].AxisY.Minimum)
            {
                Chart.ChartAreas[0].AxisY.Minimum = value;
            }
        }

        private void UpdateY(double min, double max)
        {
            if (max > Chart.ChartAreas[0].AxisY.Maximum)
            {
                Chart.ChartAreas[0].AxisY.Maximum = max;
            }
            if (min < Chart.ChartAreas[0].AxisY.Minimum)
            {
                Chart.ChartAreas[0].AxisY.Minimum = min;
            }
        }


        public void TestData()
        {
            var dt = DateTime.Now;
            var b = dt.AddMinutes(1);

            var step = new TimeSpan(0, 1, 0);

            Append(dt.AddMinutes(1), 82,  94,  80,  100, step);
            Append(dt.AddMinutes(2), 105, 106, 100, 110, step);
            Append(dt.AddMinutes(3), 106, 119, 100, 120, step);
            Append(dt.AddMinutes(4), 120, 121, 119, 125, step);
            Append(dt.AddMinutes(5), 125, 128, 120, 130, step);
            Append(dt.AddMinutes(6), 116, 114, 110, 120, step);
        }


        public void AddSerie(string name, SeriesChartType type, Color? color = null)
        {
            var s = new Series();

            s.Name        = name;
            s.ChartType   = type;
            s.BorderWidth = 1;

            if (type == SeriesChartType.Candlestick)
            {
                // Configuração do estilo de vela
                s["OpenCloseStyle"] = "Triangle"; // Forma dos marcadores de abertura/fechamento
                s["ShowOpenClose"]  = "Both"; // Mostra abertura e fechamento
                s["PointWidth"]     = "0.8"; // Largura das velas

                // Cores de alta e baixa
                s["PriceUpColor"]   = "#60FFBB"; // Alta
                s["PriceDownColor"] = "#FFBB60"; // Baixa
            }
            else
            {
                s.Color = color.HasValue ? color.Value : ColorGenerator.Create();
            }

            Chart.Series.Add(s);
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
            SizeX = new TimeSpan(1, 0, 0);

            Chart = new Chart() { BackColor = Color.FromArgb(70,70,80), Dock = DockStyle.Fill };
            Controls.Add(Chart);

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

            AddSerie("Candles", SeriesChartType.Candlestick);

            InitAxisX();
        }


        private ChartArea Area;
        private TimeSpan SizeX;
        private Chart Chart;


        public CandleChart()
        {
            InitializeComponent();
            Init();
        }
    }

    public class CandlePoint : DataPoint
    {
        public DateTime InputX;
    }
}
