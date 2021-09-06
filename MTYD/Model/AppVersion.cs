using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MTYD.Model
{
    public class AppInfo
    {
        public string version_uid { get; set; }
        public string program { get; set; }
        public string version { get; set; }
        public string build { get; set; }
    }

    public class Version
    {
        public string message { get; set; }
        public int code { get; set; }
        public IList<AppInfo> result { get; set; }
        public string sql { get; set; }
    }

    public class AppVersion
    {
        public AppVersion()
        {
        }

        public async Task<string> isRunningLatestVersion(string currentVersion)
        {
            string result = "";
            try
            {
                var version = await GetAppVersion();
                Debug.WriteLine("Version from endpoint: " + version);
                Debug.WriteLine("currentVersion: " + currentVersion);

                double currentVersionDouble = Double.Parse(currentVersion);
                double lastestVersionDouble = Double.Parse(version);

                if (currentVersionDouble >= lastestVersionDouble)
                {
                    result = "TRUE";
                }
                else
                {
                    result = "FALSE";
                }

            }
            catch
            {
                result = "Error";
            }

            return result;
        }

        public async Task<string> GetAppVersion()
        {
            string result = "";
            var client = new System.Net.Http.HttpClient();
            var endpointCall = await client.GetAsync("https://tsx3rnuidi.execute-api.us-west-1.amazonaws.com/dev/api/v2/version_details");

            if (endpointCall.IsSuccessStatusCode)
            {
                var content = await endpointCall.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Version>(content);
                if (data.result.Count != 0)
                {
                    result = data.result[2].version;
                }
                else
                {
                    result = "Error";
                }
            }
            else
            {
                result = "Error";
            }

            return result;
        }
    }
}
