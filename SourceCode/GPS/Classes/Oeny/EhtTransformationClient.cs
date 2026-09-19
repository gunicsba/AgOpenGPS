using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AgOpenGPS.Core.Models;
using Newtonsoft.Json.Linq;

namespace AgOpenGPS.Classes.Oeny
{
    // Wraps the public EHT2 (eht2.gnssnet.hu) EOV <-> ETRS89(WGS84) coordinate transformation API.
    // Note the API's own "x"/"y" naming is swapped relative to what most people expect:
    // "y" is the EOV easting-like value (larger, ~400000-900000), "x" is the northing-like value (~25000-350000).
    public class EhtTransformationClient
    {
        private const string BaseUrl = "https://eht2.gnssnet.hu/api/transformation";
        private const int BatchSize = 400;

        private static readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        // Converts a single WGS84 point to EOV (easting, northing) meters, used to center a search bbox.
        public async Task<(double Easting, double Northing)> Wgs84ToEovAsync(Wgs84 point)
        {
            string url = $"{BaseUrl}/etrs89-to-eov?pointNumber=1" +
                         $"&lon={Inv(point.Longitude)}&lat={Inv(point.Latitude)}&h=0";

            string json = await _client.GetStringAsync(url);
            JObject obj = JObject.Parse(json);

            double easting = (double)obj["y"];
            double northing = (double)obj["x"];
            return (easting, northing);
        }

        // Converts a batch of EOV (easting, northing) points to WGS84, preserving input order.
        public async Task<List<Wgs84>> EovToWgs84BulkAsync(IReadOnlyList<(double Easting, double Northing)> points)
        {
            var result = new List<Wgs84>(new Wgs84[points.Count]);

            for (int offset = 0; offset < points.Count; offset += BatchSize)
            {
                int count = Math.Min(BatchSize, points.Count - offset);

                var payload = new JArray();
                for (int i = 0; i < count; i++)
                {
                    var p = points[offset + i];
                    payload.Add(new JObject
                    {
                        ["pointNumber"] = (offset + i).ToString(CultureInfo.InvariantCulture),
                        ["y"] = p.Easting,
                        ["x"] = p.Northing,
                        ["h"] = 0,
                        ["remark"] = ""
                    });
                }

                var content = new StringContent(payload.ToString(Newtonsoft.Json.Formatting.None), Encoding.UTF8, "application/json");

                using (var response = await _client.PostAsync($"{BaseUrl}/eov-to-etrs89", content))
                {
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    JArray arr = JArray.Parse(json);

                    foreach (JToken item in arr)
                    {
                        int idx = (int)item["pointNumber"];
                        double lon = (double)item["lon"];
                        double lat = (double)item["lat"];
                        if (idx >= 0 && idx < result.Count)
                            result[idx] = new Wgs84(lat, lon);
                    }
                }
            }

            return result;
        }

        private static string Inv(double value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
