

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
        public DateTime Date { get; set; }
        public TimeSpan SelectedDuration { get; set; }
        public string Local { get; set; }

        public List<AssetQuoteTimelinePoint>  Points;
        public List<AssetQuoteTimelinePeriod> Periods;
       

        public AssetQuoteTimeline()
        {
            Points           = new List<AssetQuoteTimelinePoint>();
            Periods          = new List<AssetQuoteTimelinePeriod>();
            SelectedDuration = new TimeSpan(0, 1, 0);
        }
    }


    public class AssetQuoteTimelinePoint
    {
        public ulong Index;
        public DateTime Time;
        public decimal Last;
        public decimal Negotiations;
        public decimal Volumes;
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

        public AssetQuoteTimelinePeriod()
        {
            Points = new List<AssetQuoteTimelinePoint>();
        }
    }

}
