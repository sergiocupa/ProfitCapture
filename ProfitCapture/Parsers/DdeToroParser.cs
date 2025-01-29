using ProfitCapture.Models;

namespace ProfitCapture.Parsers
{

    public class DdeToroParser
    {

        public static DdeInfo Parse(string content)
        {
            var info = PreParse(content);

            if (info != null && info.Lines.Count > 0)
            {
                var meta = info.Lines[0];
                var dde = new DdeInfo() { Service = meta.Service, Topic = meta.Topic };

                foreach (var f in info.Lines)
                {
                    if (f.Fields.Count > 0)
                    {
                        var fmeta = f.Fields[0];
                        var an = fmeta.Split('.');
                        var at = new Asset() { Name = an[0] };

                        foreach (var v in f.Fields)
                        {
                            var ax = v.Split('.');

                            if (ax.Length > 1)
                            {
                                at.Fields.Add(new FieldAsset() { Name = ax[1], Asset = at, Item = v });
                            }
                        }

                        if (at.Fields.Count > 0)
                        {
                            dde.Assets.Add(at);
                        }
                    }
                }

                return dde;
            }
            return null;
        }

        private static PreParseInfo PreParse(string content)
        {
            var info = new PreParseInfo();
            if (string.IsNullOrEmpty(content)) return null;

            var lin = new List<Line>();
            var lines = content.Split('\n');
            foreach (var line in lines)
            {
                var ln = new Line() { Columns = line.Split('\t').ToList() };
                lin.Add(ln);
            }

            if (lin.Count > 1)
            {
                info.Header = lin[0].Columns;

                int ix = 1;
                while (ix < lin.Count)
                {
                    var v = lin[ix];

                    if (info.Header.Count > 1 && info.Header.Count == v.Columns.Count)
                    {
                        var line = new RowPreParseInfo() { Index = info.Lines.Count };

                        int iz = 1;
                        while (iz < v.Columns.Count)
                        {
                            var c = v.Columns[iz];
                            var part = c.Split('|');

                            if (part.Length >= 2)
                            {
                                var field = part[1].Split("!");
                                line.Service = part[0].Replace("=", "");
                                line.Topic = field[0];

                                line.Fields.Add(field[1].Replace("'", ""));
                            }
                            iz++;
                        }
                        info.Lines.Add(line);
                    }
                    ix++;
                }
            }
            return info;
        }


    }


    internal class PreParseInfo
    {
        internal List<string> Header;
        internal List<RowPreParseInfo> Lines;

        internal PreParseInfo()
        {
            Header = new List<string>();
            Lines = new List<RowPreParseInfo>();
        }
    }
    internal class RowPreParseInfo
    {
        internal int Index;
        internal string Service;
        internal string Topic;
        internal List<string> Fields;

        internal RowPreParseInfo()
        {
            Fields = new List<string>();
        }
    }


    public class DdeReceiverBuffer
    {
        public string Name;
        public string Field;
        public string Origin;
        public string Value;

        public DdeReceiverBuffer()
        {

        }
    }

    internal class Line
    {
        internal List<string> Columns;

        internal Line()
        {
            Columns = new List<string>();
        }
    }
}
