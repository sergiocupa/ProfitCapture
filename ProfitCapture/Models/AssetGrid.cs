namespace ProfitCapture.Models
{
    public class AssetGrid
    {
        public string Item { get; set; }
        public string Asset { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public ulong Count { get; set; }


        public override string ToString()
        {
            return "Asset:" + Asset + " | Name: " + Name + " | Value: " + Value + " | Count: " + Count; 
        }


    }
}
