

using Newtonsoft.Json;

namespace ProfitCapture.Models
{

    public class AssetQuote
    {

        public string Rubric { get { return Metadata != null ? Metadata.Name : ""; } }

        public AssetGrid Metadata;
        public List<AssetQuoteTimeline> Timelines;

        public AssetQuote()
        {
            Metadata  = new AssetGrid();
            Timelines = new List<AssetQuoteTimeline>();
        }
    }

    public class AssetQuoteTimeline
    {
        public AssetGrid Metadata;
        public DateTime Date { get; set; }
        public TimeSpan SelectedDuration { get; set; }
        public string Local { get; set; }

        public List<AssetQuoteTimelinePoint>  Points;
        public List<AssetQuoteTimelinePeriod> Periods;
        public List<AssetQuoteTimelinePeriod> Markings;

        public void SaveMarkings()
        {
            var json = JsonConvert.SerializeObject(Markings, Formatting.Indented);

            var path = Metadata.RootPath + "/" + CaptureSetting.DEFAULT_MARKING + "/" + Metadata.Name;

            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var local = (path + "/" + Date.ToString("yyyy-MM-dd") + ".json").Replace("\\","/");
            File.WriteAllText(local, json);
        }

        public void PrepareMarking()
        {
            if (IsPreparedMarkings) return;

            if(Periods.Count > 0)
            {
                Markings.Clear();
                foreach (var period in Periods)
                {
                    Markings.Add(new AssetQuoteTimelinePeriod(period));
                }
            }
            IsPreparedMarkings = true;
        }


        public bool IsPreparedMarkings { get; set; }
        public bool IsLongTrade { get; set; }
        public AssetQuoteTimelinePeriod PreviusSelectedCandle { get; set; }


        public AssetQuoteTimeline()
        {
            Points           = new List<AssetQuoteTimelinePoint>();
            Periods          = new List<AssetQuoteTimelinePeriod>();
            SelectedDuration = new TimeSpan(0, 1, 0);
            Markings         = new List<AssetQuoteTimelinePeriod>();
        }
    }


    public class AssetQuoteTimelinePoint
    {
        public ulong Index;
        public DateTime Time;
        public decimal Last;
        public decimal Negotiations;
        public decimal Volumes;
        public Note Note;
        public Note Prediction;


        public AssetQuoteTimelinePoint(AssetQuoteTimelinePoint a)
        {
            if(a != null)
            {
                Index = a.Index;
                Time = a.Time;
                Last = a.Last;
                Negotiations = a.Negotiations;
                Volumes = a.Volumes;
                Note = a.Note != null ? new Note(a.Note) : null;
                Prediction = a.Prediction != null ? new Note(a.Prediction) : null;
            }
        }

        public AssetQuoteTimelinePoint() { }
    }

    public enum TransactOption
    {
        None       = 0,
        Buy        = 1,
        Sell       = 2,
        SellShort  = 3,
        BuyToCover = 4
    }

    public class AssetQuoteTimelinePeriod
    {  
        public ulong    Index;
        public DateTime Time;
        public decimal  Open;
        public decimal  Current;
        public decimal  Close;
        public decimal  Min;
        public decimal  Max;
        public TimeSpan Duration;
        public List<AssetQuoteTimelinePoint> Points;

        public Note Note;
        public Note Prediction;

        public AssetQuoteTimelinePeriod(AssetQuoteTimelinePeriod a)
        {
            if(a != null)
            {
                Index = a.Index;
                Time = a.Time;
                Open = a.Open;
                Current = a.Current;
                Close = a.Close;
                Min = a.Min;
                Max = a.Max;
                Duration = a.Duration;
                Points = a.Points != null ? a.Points.Select(s => new AssetQuoteTimelinePoint(s)).ToList() : new List<AssetQuoteTimelinePoint>(0);
                Note = a.Note != null ? new Note(a.Note) : null;
                Prediction = a.Prediction != null ? new Note(a.Prediction) : null;
            }
            else
            {
                Points = new List<AssetQuoteTimelinePoint>();
            }
        }

        public AssetQuoteTimelinePeriod()
        {
            Points = new List<AssetQuoteTimelinePoint>();
        }
    }

    public class Note
    {
        public string UID;
        public TransactOption Transact;


        public Note(Note a)
        {
            if(a != null)
            {
                Transact = a.Transact;
                UID = a.UID;
            }
        }

        public Note()
        {
            Transact = TransactOption.None;
            UID = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
        }
    }

}
