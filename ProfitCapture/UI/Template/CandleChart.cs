using ProfitCapture.Utils;
using System.Windows.Forms.DataVisualization.Charting;


namespace ProfitCapture.UI.Template
{

    public partial class CandleChart : UserControl
    {


        // Pegar a posicao atual
        //  Primeira interacao, plotar posicao 0 do eixo X
        //    Antes, definir Min o valor X, o valor Max = Min + SizeX
        //    Se valor atual estourar Max, entao deslocar Max = Atual, deslocar Min = Max - SizeX
        ...


        public void Append(DateTime x, double open, double close, double min, double max, bool is_update = false, int serie = 0, Color? color = null)
        {
            try
            {
                if (serie >= 0 && serie < Chart.Series.Count)
                {
                    UpdateY(min,max);

                    if(is_update)
                    {

                    }
                    else
                    {
                        var dp = new CandlePoint() { InputX = x, XValue = x.ToOADate(), YValues = new double[] { open, max, min, close } };
                        if (color.HasValue)
                        {
                            dp.Color = color.Value;
                        }

                        Chart.Series[serie].Points.Add(dp);
                    }

                   

                    if(Chart.Series[serie].Points.Count >= 2)
                    {
                        var first = (CandlePoint)Chart.Series[serie].Points.FirstOrDefault();
                        var last  = (CandlePoint)Chart.Series[serie].Points.LastOrDefault();
                        var dif   = last.InputX.Subtract(first.InputX);

                        if(dif > SizeX)
                        {

                        }
                    }






                    var s = x;
                    s.Subtract(SizeX);

                    Chart.Series[serie].Points.Remove(Chart.Series[serie].Points[0]);
                    Area.AxisX.Maximum = x.ToOADate();
                    Area.AxisX.Minimum = s.ToOADate();

                    //if (Ix > SizeX)
                    //{
                    //    Chart.Series[serie].Points.Remove(Chart.Series[serie].Points[0]);
                    //    Area.AxisX.Maximum = x.ToOADate();
                    //    Area.AxisX.Minimum = (Ix - SizeX);
                    //}
                    Ix++;
                }
            }
            catch (Exception) { }
        }


        public void Append(DateTime x, double y, int serie = 0, Color? color = null)
        {
            try
            {
                if (serie >= 0 && serie < Chart.Series.Count)
                {
                    UpdateY(y);

                    var point = new DataPoint(x.ToOADate(), y);

                    if (color.HasValue)
                    {
                        point.Color = color.Value;
                    }

                    Chart.Series[serie].Points.Add(point);

                    if (Ix > SizeX)
                    {
                        Chart.Series[serie].Points.Remove(Chart.Series[serie].Points[0]);

                        Area.AxisX.Maximum = x.ToOADate();
                        Area.AxisX.Minimum = (Ix - SizeX);
                    }
                    Ix++;
                }
            }
            catch (Exception) { }
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
            var dados = new (int Data, double Abertura, double Maxima, double Minima, double Fechamento)[]
            {
                (1,  100, 110, 90,  105),
                (5,  105, 120, 100, 115),
                (10, 115, 125, 110, 120),
                (15, 120, 130, 115, 125),
                (20, 135, 140, 120, 125),
            };

            foreach (var dado in dados)
            {
                DataPoint dp = new DataPoint
                {
                    XValue  = dado.Data,//.ToOADate(),
                    YValues = new double[] { dado.Maxima, dado.Minima, dado.Abertura, dado.Fechamento }
                };

                Append(dp);

                Serie.Points.Add(dp);
            }
        }


        //public void SetSizeX(double x, int serie)
        //{
        //    SizeX = x;
        //    Ix    = 0;

        //    if(serie >= 0 && serie < Chart.Series.Count)
        //    {
        //        Chart.Series[serie].Points.Clear();
        //    }

        //    Chart.ChartAreas[0].AxisX.Maximum = SizeX;
        //    Chart.ChartAreas[0].AxisX.Minimum = Ix;
        //}

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
                s["PointWidth"]     = "0.1"; // Largura das velas

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
            SizeX = new TimeSpan(3, 0, 0);

            Chart = new Chart() { BackColor = Color.FromArgb(70,70,80), Dock = DockStyle.Fill };
            Controls.Add(Chart);

            Area = new ChartArea() { BackColor = Color.FromArgb(70, 70, 80) };
            Area.Name = "CandleArea";
            Chart.ChartAreas.Add(Area);

            Area.Position = new ElementPosition(0, 1, 100, 100);

            // Configuração do eixo X
            Area.AxisX.Interval                 = 1;
            Area.AxisX.MajorGrid.LineColor      = System.Drawing.Color.LightGray;
            Area.AxisX.Maximum                  = SizeX;
            Area.AxisX.LineWidth                = 0;
            Area.AxisX.MajorGrid.Enabled        = false; // Remove linhas principais
            Area.AxisX.MinorGrid.Enabled        = false; // Remove linhas secundárias
            Area.AxisX2.LineWidth               = 0;
            Area.AxisX.LabelStyle.Format        = "HH:mm:ss";

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
        private long Ix;
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
