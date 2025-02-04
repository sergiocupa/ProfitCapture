

namespace ProfitCapture.UI.Template
{

    public partial class LogConsole : Form
    {

        private void LogConsole_Shown(object sender, EventArgs e)
        {
            if (AppendConsole)
            {
                Console.SetOut(new RichTextBoxWriter(Content, ViewQueue, false));
                Console.SetError(new RichTextBoxWriter(Content, ViewQueue, true));
            }
        }

        private void LogConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AppendConsole)
            {
                ViewQueue.Stop();
            }

            if (ClosingEvent != null) ClosingEvent();
        }


        DispatcherQueue ViewQueue;
        bool AppendConsole;
        Action ClosingEvent;


        public LogConsole(Action closing, bool append_console = true)
        {
            InitializeComponent();

            ClosingEvent = closing;

            AppendConsole = append_console;

            if(AppendConsole)
            {
                ViewQueue = new DispatcherQueue();
            }
        }


    }
}
