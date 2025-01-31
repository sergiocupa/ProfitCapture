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
            var lines = text.Split('\n');

            var im = index+1;

            int ix = 0; 
            while(ix <  lines.Length)
            {
                var fields = lines[ix].Split('|');

                if(fields.Length >= 4)
                {
                    var p1 = fields[0].Split("=");
                    var p2 = fields[1].Split("=");
                    var p3 = fields[2].Split("=");
                    var p4 = fields[3].Split("=");

                    if(p1.Length >= 2 && p2.Length >= 2 && p3.Length >= 2 && p4.Length >= 2)
                    {
                        var v1 = TimeSpan.Parse(p1[1]);
                        var v2 = decimal.Parse(p2[1], CultureInfo.CurrentCulture);
                        var v3 = decimal.Parse(p3[1], CultureInfo.CurrentCulture);
                        var v4 = decimal.Parse(p4[1], CultureInfo.CurrentCulture);

                        var aqtp = new AssetQuoteTimelinePoint()
                        { 
                            Index = im,
                            Time = new DateTime(timeline.Date.Year,timeline.Date.Month, timeline.Date.Day, v1.Hours, v1.Minutes, v1.Seconds), 
                            Last = v2,  
                            Negotiations = v3,
                            Volumes = v4
                        };
                        result.Add(aqtp);
                        im++;
                    }
                }
                ix++;
            }
            return result;
        }

    }
}
