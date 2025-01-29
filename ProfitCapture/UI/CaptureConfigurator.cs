

using ProfitCapture.Parsers;

namespace ProfitCapture
{

    public partial class CaptureConfigurator : Form
    {
        public CaptureConfigurator()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();
            var res = fd.ShowDialog();
            if (res == DialogResult.OK)
            {
                textBox1.Text = fd.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(richTextBox1.Text))
                {
                    MessageBox.Show("Informe conteudo DDE"); return;
                }
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Informe local para salvar as capturas"); return;
                }

                var par = DdeToroParser.Parse(richTextBox1.Text);
                if(par == null)
                {
                    MessageBox.Show("Não foi possível converter texto informado para formar a integração entre app Nelogica"); return;
                }

                var setts = CaptureSetting.Load();
                setts.CaptureLocation    = textBox1.Text;
                setts.ChannelsRawContent = richTextBox1.Text;
                setts.Save();

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
