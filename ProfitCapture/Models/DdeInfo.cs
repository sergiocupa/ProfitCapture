namespace ProfitCapture.Models
{
    public class DdeInfo
    {
        public string Service { get; set; }
        public string Topic { get; set; }
        public List<Asset> Assets { get; set; }

        public DdeInfo()
        {
            Assets = new List<Asset>();
        }
    }

    public class Asset
    {
        public string Name;
        public List<FieldAsset> Fields;

        public Asset()
        {
            Fields = new List<FieldAsset>();
        }
    }

    public class FieldAsset
    {
        public string Item;
        public string Name;
        public string Value;
        public Asset Asset;

    }
}
