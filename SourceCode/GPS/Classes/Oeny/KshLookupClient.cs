using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AgOpenGPS.Classes.Oeny
{
    // Looks up Hungarian settlement (town) names from KSH codes, using the same public
    // dataset the shape_kml.html converter tool uses.
    public static class KshLookupClient
    {
        private const string SourceUrl = "https://raw.githubusercontent.com/ferenci-tamas/IrszHnk/refs/heads/master/IrszHnk.json";
        private static readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        private static Dictionary<string, string> _kshToTown;
        private static Task _loadTask;

        public static Task EnsureLoadedAsync()
        {
            if (_kshToTown != null) return Task.CompletedTask;
            if (_loadTask == null) _loadTask = LoadAsync();
            return _loadTask;
        }

        private static async Task LoadAsync()
        {
            try
            {
                string json = await _client.GetStringAsync(SourceUrl);
                JArray arr = JArray.Parse(json);
                var map = new Dictionary<string, string>();

                foreach (JToken row in arr)
                {
                    string ksh = (string)row["Helység.KSH.kódja"];
                    string town = (string)row["Helység.megnevezése"];
                    ksh = ksh?.Trim();
                    town = town?.Trim();

                    if (!string.IsNullOrEmpty(ksh) && !string.IsNullOrEmpty(town) && !map.ContainsKey(ksh))
                        map[ksh] = town;
                }

                _kshToTown = map;
            }
            catch
            {
                // Town names are a nice-to-have; fail quietly and just show HRSZ without a town prefix.
                _kshToTown = new Dictionary<string, string>();
            }
        }

        public static string GetTownName(string kshCode)
        {
            if (string.IsNullOrWhiteSpace(kshCode) || _kshToTown == null) return null;
            return _kshToTown.TryGetValue(kshCode.Trim(), out string town) ? town : null;
        }
    }
}
