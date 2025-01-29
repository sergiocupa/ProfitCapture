using ProfitCapture.Models;
using ProfitCapture.Parsers;
using ProfitCapture.UI.Template;
using NDde.Client;
using System.ComponentModel;


namespace ProfitCapture
{

    internal class Capture
    {

        private void LineReceived(List<AssetGrid> items)
        {
            EventCounter++;

            if (items.Count > 0)
            {
                var dir       = Setting.CaptureLocation + "/" + items[0].Asset;
                var file_name = dir + "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".dat";

                var content = string.Join("|", items.Select(s => s.Name + "=" + s.Value)) + "\n";
                
                File.AppendAllText(file_name, content);
            }

            Grid.Invoke(() => 
            {
                Grid.SuspendLayout();

                foreach (var i in Assets)
                {
                    var a = items.Where(w => w.Item == i.Item).FirstOrDefault();
                    if (a != null)
                    {
                        i.Value = a.Value;
                        i.Count++;
                    }
                }

                Grid.ResumeLayout();
                Grid.Refresh();
            });
        }


        private void AdviseReceived(object o, DdeAdviseEventArgs args)
        {
            Queue.Enqueue((a) =>
            {
                Counter++;

                var target = Assets.Where(w => w.Item == a.Name).FirstOrDefault();
                if (target != null)
                {
                    target.Value = a.Value.Trim();

                    var all = Assets.Where(w => w.Asset == target.Asset).ToList();
                    var av = all.Where(w => !string.IsNullOrEmpty(w.Value)).Count();

                    if(all.Count == av)
                    {
                        var nv = all.Select(s => new AssetGrid() { Asset = s.Asset, Name = s.Name, Item = s.Item, Value = s.Value }).ToList();
                        all.ForEach(s => s.Value = "");
                        LineQueue.Enqueue(LineReceived, nv);
                    }
                }            },
            new DdeItem() { Name = args.Item, Value = args.Text.Trim('\0') });
        }


        public void Start()
        {
            if (Client != null) return;

            Grid = new DataGridView() { Dock = DockStyle.Fill };
            Body.Controls.Add(Grid);
            DataGridViewTemplate.EsquemaBrancoLinhaAlternada(Grid);

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

            if (!Directory.Exists(Setting.CaptureLocation))
            {
                Directory.CreateDirectory(Setting.CaptureLocation);
            }

            var items = new List<string>();
            Assets.Clear();
            foreach (var f in Info.Assets)
            {
                var dir = Setting.CaptureLocation + "/" + f.Name;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                foreach (var a in f.Fields)
                {
                    var grid = new AssetGrid() { Asset = f.Name, Name = a.Name, Item = a.Item };
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
        CaptureSetting Setting;

        public Capture(Control body)
        {
            Body = body;
            Assets = new BindingList<AssetGrid>();
            Queue = new TypedActionQueue();
            LineQueue = new TypedActionQueue();
        }

    }
}
