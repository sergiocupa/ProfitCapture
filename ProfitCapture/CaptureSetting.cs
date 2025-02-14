using Newtonsoft.Json;
using System.IO;
using System.Reflection;


namespace ProfitCapture
{

    public class CaptureSetting
    {

        public string SettingsLocation { get; set; }
        public string CaptureLocation { get; set; }
        public string ChannelsRawContent { get; set; }



        public static bool IsValidPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                string root = Path.GetPathRoot(path);
                string sub  = path.Substring(root.Length).Trim('\\', '/');
                return !string.IsNullOrEmpty(sub);
            }
            else return false;
        }

        public void PrepareDirectories()
        {
            if(IsValidPath(CaptureLocation))
            {
                if (!Directory.Exists(CaptureLocation))
                {
                    Directory.CreateDirectory(CaptureLocation);
                }

                var tg = GetCompleteCaptureLocation();
                if (!Directory.Exists(tg))
                {
                    Directory.CreateDirectory(tg);
                }

                var ta = GetCompleteMarkingLocation();
                if (!Directory.Exists(ta))
                {
                    Directory.CreateDirectory(ta);
                }
            }
        }

        public string GetCompleteCaptureLocation()
        {
            return !string.IsNullOrEmpty(CaptureLocation) ? (CaptureLocation.Replace("\\", "/") + "/" + DEFAULT_CAPTURE) : DEFAULT_CAPTURE;
        }

        public string GetCompleteMarkingLocation()
        {
            return !string.IsNullOrEmpty(CaptureLocation) ? (CaptureLocation.Replace("\\", "/") + "/" + DEFAULT_MARKING) : DEFAULT_MARKING;
        }


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
                var obj  = JsonConvert.DeserializeObject<CaptureSetting>(json);
                obj.PrepareDirectories();
                return obj;
            }
            else return new CaptureSetting() { SettingsLocation = LOCAL};
        }


        public static string DEFAULT_CAPTURE = "Capture";
        public static string DEFAULT_MARKING = "Marking";

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
