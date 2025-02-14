using ProfitCapture.Models;
using ProfitCapture.Parsers;
using ProfitCapture.UI.Template;
using NDde.Client;
using System.ComponentModel;


namespace ProfitCapture
{

    internal class Capture
    {

        private void AdviseReceived(object o, DdeAdviseEventArgs args)
        {
            Queue.Enqueue((a) =>
            {
                if (Counter == ulong.MaxValue) Counter = 0;
                Counter++;

                var target = Assets.Where(w => w.Item == a.Name).FirstOrDefault();
                if (target != null)
                {
                    var dir     = Setting.GetCompleteCaptureLocation() + "/" + target.Asset;
                    var fname   = dir + "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".dat";
                    var content = "{Time:\"" + a.Time.ToString("HH:mm:ss.fffffff") + "\",Name:\"" + a.Name + "\",Value:\"" + a.Value.Replace(",",".") + "\"}\n";

                    File.AppendAllText(fname, content);

                    Grid.Invoke(() =>
                    {
                        Grid.SuspendLayout();
                        target.Value = a.Value;
                        target.Count++;
                        Grid.ResumeLayout();
                        Grid.Refresh();
                    });

                    CounterLabel.Invoke(() => { CounterLabel.Text = Counter.ToString(); });
                }
            },
            new DdeItem() { Name = args.Item, Value = args.Text.Trim('\0'), Time = DateTime.Now });
        }


        public void Start()
        {
            if (Client != null) return;

            Grid = new DataGridView() { Dock = DockStyle.Fill };
            Body.Controls.Add(Grid);
            DataGridViewTemplate.EsquemaBrancoLinhaAlternada(Grid, true);

            var tp = typeof(AssetGrid);
            var fd = tp.GetProperties();
            var gl = new List<DataGridViewColumn>();
            foreach (var f in fd)
            {
                gl.Add(DataGridViewTemplate.CriarColunaTexto(f.Name, f.Name));
            }
            Grid.Columns.AddRange(gl.ToArray());

            Setting = CaptureSetting.Load();

            Info = DdeToroParser.Parse(Setting.ChannelsRawContent);
            if (Info == null)
            {
                MessageBox.Show("Não foi possível converter texto informado para formar a integração entre app Nelogica"); return;
            }

            var items = new List<string>();
            Assets.Clear();
            foreach (var f in Info.Assets)
            {
                var dir = Setting.GetCompleteCaptureLocation() + "/" + f.Name;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                foreach (var a in f.Fields)
                {
                    var grid = new AssetGrid() { Asset = f.Name, Name = a.Name, Item = a.Item, RootPath = Setting.CaptureLocation };
                    Assets.Add(grid);
                    items.Add(a.Item);
                }
            }
            Grid.DataSource = Assets;

            Client = new DdeClient(Info.Service, Info.Topic);
            Client.Connect();
            Client.Advise += AdviseReceived;

            int ix = 0;
            while (ix < items.Count)
            {
                Client.StartAdvise(items[ix], 1, true, 120000);
                ix++;
            }
        }

        public void Stop()
        {
            if (Client == null) return;
            Client.Disconnect();
            Client = null;
        }

        public void Shutdown()
        {
            Stop();
            Queue.Stop();
            LineQueue.Stop();

        }


        ulong Counter;
        ulong EventCounter;
        TypedActionQueue Queue;
        TypedActionQueue LineQueue;
        DdeClient Client;
        BindingList<AssetGrid> Assets;
        DdeInfo Info;
        DataGridView Grid;
        Control Body;
        Label CounterLabel;
        CaptureSetting Setting;

        public Capture(Control body, Label counter_label)
        {
            Body = body;
            CounterLabel = counter_label;
            Assets = new BindingList<AssetGrid>();
            Queue = new TypedActionQueue();
            LineQueue = new TypedActionQueue();
        }

    }
}
