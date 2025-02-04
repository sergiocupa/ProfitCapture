using System.Text;


namespace ProfitCapture.UI.Template
{
    public class RichTextBoxWriter : TextWriter
    {
        private readonly RichTextBox _richTextBox;
        private DispatcherQueue ViewQueue;
        private bool IsError;
        public RichTextBoxWriter(RichTextBox richTexttbox, DispatcherQueue view_queue, bool is_error)
        {
            _richTextBox = richTexttbox;
            ViewQueue    = view_queue;
            IsError      = is_error;

            _richTextBox.TextChanged += (object? sender, EventArgs e) =>
            {
                _richTextBox.SelectionStart = _richTextBox.Text.Length;
                _richTextBox.ScrollToCaret();
            };
        }

        public override void Write(char value)
        {
            AppendText(value.ToString());
        }

        public override void Write(string value)
        {
            AppendText(value);
        }

        public override void WriteLine(char value)
        {
            AppendText(value + Environment.NewLine);
        }

        public override void WriteLine(string value)
        {
            AppendText(value + Environment.NewLine);
        }

        public override Encoding Encoding => Encoding.ASCII;
        public static Color TEXT_ERROR = Color.Salmon;
        public static Color TEXT_INFO  = Color.FromArgb(120, 255, 200);


        private void AppendText(string text)
        {
            var act = (string s) =>
            {
                if (_richTextBox.InvokeRequired)
                {
                    var d = new StringArgReturningVoidDelegate(AppendText);
                    _richTextBox.Invoke(() =>
                    {
                        _richTextBox.SelectionColor = IsError ? TEXT_ERROR : TEXT_INFO;

                        _richTextBox.AppendText(s);
                    });
                }
                else
                {
                    _richTextBox.SelectionColor = IsError ? TEXT_ERROR : TEXT_INFO;

                    _richTextBox.AppendText(s);
                }
            };

            if(ViewQueue != null)
            {
                ViewQueue.Enqueue(act, text);
            }
            else
            {
                act(text);
            }
        }
    }

    public delegate void StringArgReturningVoidDelegate(string text);
}
