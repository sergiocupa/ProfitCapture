using Newtonsoft.Json;
using ProfitCapture.Models;
using System.Globalization;


namespace ProfitCapture.Parsers
{

    internal class AssetQuoteParser
    {


        public static List<AssetQuote> ReadAssembleAssets(string local)
        {
            var result = new List<AssetQuote>();

            if (Directory.Exists(local))
            {
                var dirs = Directory.GetDirectories(local);
                foreach (var dir in dirs)
                {
                    var quote = new AssetQuote();
                    quote.Metadata.Name  = Path.GetFileName(dir);
                    quote.Metadata.Local = Path.GetDirectoryName(dir);

                    var files = Directory.GetFiles(dir);
                    foreach (var file in files)
                    {
                        var timel   = new AssetQuoteTimeline();
                        var fn      = Path.GetFileName(file).Replace(".dat","");
                        timel.Date  = DateTime.Parse(fn);
                        timel.Local = file;
                        quote.Timelines.Add(timel);
                    }
                    result.Add(quote);
                }
            }
            return result;
        }


        public static List<AssetQuoteTimelinePoint> ReadAssembleTimelinePoints(AssetQuoteTimeline timeline, ulong index)
        {
            var result = new List<AssetQuoteTimelinePoint>();
            var text  = File.ReadAllText(timeline.Local);
            var lines = text.Split('\n',StringSplitOptions.RemoveEmptyEntries);

            var im = index+1;

            int ix = 0; 
            while(ix <  lines.Length)
            {
                var va = lines[ix].Trim();
                var ob = JsonConvert.DeserializeObject<AssetPoint>(va);

                if(ob != null && !string.IsNullOrEmpty(ob.Name) && ob.Name.EndsWith(".ULT"))
                {
                    var aqtp = new AssetQuoteTimelinePoint()
                    {
                        Index = im,
                        Time  = new DateTime(timeline.Date.Year, timeline.Date.Month, timeline.Date.Day, ob.Time.Hours, ob.Time.Minutes, ob.Time.Seconds, ob.Time.Milliseconds),
                        Last  = decimal.Parse(ob.Value, CultureInfo.CurrentCulture)
                    };
                    result.Add(aqtp);
                    im++;
                }
                ix++;
            }
            return result;
        }

    }

    public class AssetPoint
    {
        public TimeSpan Time { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
