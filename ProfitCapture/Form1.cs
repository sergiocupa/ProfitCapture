using Microsoft.VisualBasic.Logging;
using ProfitCapture.UI;
using ProfitCapture.UI.Template;



namespace ProfitCapture
{
    public partial class Form1 : Form
    {

        private void OpenButton_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


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

        Capture     Capture;
        AssetViewer AssetView;

        public Form1()
        {
            InitializeComponent();

            Capture = new Capture(panel2,label1);

            FormClosing += Form1_FormClosing;

            AssetView = new AssetViewer(this, panel3, panel4, panel5);

            AssetView.ViewAssetList();
        }

        
    }


    public class DdeItem
    {
        public string Name;
        public string Value;
        public string Field;
        public string Origin;
        public DateTime Time;

        public DdeItem()
        {

        }
    }

    


    


    
}
