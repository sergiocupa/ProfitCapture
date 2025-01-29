using System.Windows.Forms.DataVisualization.Charting;


namespace ProfitCapture.UI.Template
{

    public partial class CandleChart : UserControl
    {

        public void Append(double value, Color? color = null)
        {
            try
            {
                if (value > Chart.ChartAreas[0].AxisY.Maximum)
                {
                    Chart.ChartAreas[0].AxisY.Maximum = value;
                }
                if (value < Chart.ChartAreas[0].AxisY.Minimum)
                {
                    Chart.ChartAreas[0].AxisY.Minimum = value;
                }

                var point = color.HasValue ? new DataPoint(Ix, value) { Color = color.Value } : new DataPoint(Ix, value);

                Serie.Points.Add(point);

                if (Ix > SizeX)
                {
                    Serie.Points.Remove(Serie.Points[0]);
                    Area.AxisX.Maximum = Ix;
                    Area.AxisX.Minimum = (Ix - SizeX);
                }
                Ix++;
            }
            catch (Exception) { }
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
                Serie.Points.Add(dp);
            }
        }


        public void SetSizeX(double x)
        {
            SizeX = x;
            Ix = 0;
            Serie.Points.Clear();
            Chart.ChartAreas[0].AxisX.Maximum = SizeX;
            Chart.ChartAreas[0].AxisX.Minimum = Ix;
        }


        public void Init()
        {
            Chart = new Chart() { BackColor = Color.FromArgb(70,70,80) };
            Area = new ChartArea() { BackColor = Color.FromArgb(70, 70, 80) };
            Area.Name = "CandleArea";
            Chart.ChartAreas.Add(Area);

            // Configuração do eixo X
            //Area.AxisX.LabelStyle.Format = "HH:mm:ss";
            Area.AxisX.Interval = 1;
            Area.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;

            
            Serie = new Series();
            Serie.Name = "Candles";
            Serie.ChartType = SeriesChartType.Candlestick;
            Serie.BorderWidth = 1;
        

           // Configuração do estilo de vela
            Serie["OpenCloseStyle"] = "Triangle"; // Forma dos marcadores de abertura/fechamento
            Serie["ShowOpenClose"]  = "Both"; // Mostra abertura e fechamento
            Serie["PointWidth"]     = "0.1"; // Largura das velas

            // Cores de alta e baixa
            Serie["PriceUpColor"]   = "#60FFBB"; // Alta
            Serie["PriceDownColor"] = "#FFBB60"; // Baixa

            Chart.Series.Add(Serie);
            Chart.Dock = DockStyle.Fill;
            Controls.Add(Chart);

            Area.AxisY.Enabled = AxisEnabled.False;
            Area.AxisY.Maximum           = 200;
            Area.AxisY.Minimum           = 0;
            Area.AxisX.Maximum           = SizeX;
            Area.AxisY.LabelStyle.Format = "F2";
            Area.AxisY.LineWidth         = 0;
            Area.AxisX.LineWidth         = 0;

           
            Area.Position = new ElementPosition(0, 1, 100, 100);


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

            Area.AxisX.MajorGrid.Enabled = false; // Remove linhas principais
            Area.AxisX.MinorGrid.Enabled = false; // Remove linhas secundárias

            Area.AxisX2.LineWidth = 0;

        }


        private ChartArea Area;
        private Series Serie;
        private double SizeX = 100;
        private long Ix;
        private Chart Chart;


        public CandleChart()
        {
            InitializeComponent();
            Init();
        }
    }
}
