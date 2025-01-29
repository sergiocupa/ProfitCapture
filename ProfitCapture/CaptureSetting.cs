using Newtonsoft.Json;
using System.Reflection;


namespace ProfitCapture
{

    public class CaptureSetting
    {

        public string SettingsLocation { get; set; }
        public string CaptureLocation { get; set; }
        public string ChannelsRawContent { get; set; }


        public void Save()
        {
            SettingsLocation = LOCAL;
            var fn = SettingsLocation + "/" + nameof(CaptureSetting) + ".json";
            var ser = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(fn,ser);
        }


        public static CaptureSetting Load()
        {
            var fn = LOCAL + "/" + nameof(CaptureSetting) + ".json";

            if (File.Exists(fn))
            {
                var json = File.ReadAllText(fn);
                return JsonConvert.DeserializeObject<CaptureSetting>(json);
            }
            else return new CaptureSetting() { SettingsLocation = LOCAL};
        }



        private static string LOCAL;

        private static string GetAbsLocalExecuting()
        {
            if(string.IsNullOrEmpty(LOCAL))
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                LOCAL = Path.GetDirectoryName(ass.ManifestModule.FullyQualifiedName).Replace("\\", "/");
            }
            return LOCAL;
        }

        private CaptureSetting()
        {

        }

        static CaptureSetting()
        {
            GetAbsLocalExecuting();
        }
    }
}
