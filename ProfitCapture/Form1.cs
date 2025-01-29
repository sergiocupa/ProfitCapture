using ProfitCapture.UI.Template;



namespace ProfitCapture
{
    public partial class Form1 : Form
    {


        private void Open_Click(object sender, EventArgs e)
        {
            try
            {
                Capture.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            try
            {
                Capture.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var config = new CaptureConfigurator();
                config.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            try
            {
                Capture.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            try
            {
                Environment.Exit(0);
            }
            catch (Exception)
            {
            }
        }

        Capture Capture;
        CandleChart Chart;

        public Form1()
        {
            InitializeComponent();

            Capture = new Capture(panel2);

            FormClosing += Form1_FormClosing;

            Chart = new CandleChart() { Dock = DockStyle.Fill };
            panel5.Controls.Add(Chart);


            Chart.TestData();
        }

        
    }


    public class DdeItem
    {
        public string Name;
        public string Value;
        public string Field;
        public string Origin;

        public DdeItem()
        {

        }
    }

    


    


    
}
