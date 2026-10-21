using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AgOpenGPS.Classes.Oeny
{
    // Fetches cadastral parcels from the geoshop.hu web map's coverage preview endpoint.
    // For a query polygon (EOV) it returns every intersecting parcel with its HRSZ, town and outline.
    // The outlines are generalized (rounded to 0.1 m), and the query area is limited per product.
    public class GeoshopClient
    {
        private const string CoverageUrl = "https://map.geoshop.hu/api/coverage/polygon";
        private const string OriginUrl = "https://geoshop.hu";
        private const string RefererUrl = "https://geoshop.hu/";
        // Public key the geoshop.hu web map sends with every request.
        private const string ApiKey = "riNqFyhQiaNal4CuXgrxm202dnI3qX";
        // "Földrészlet" products, both queried in one request: 62 = belterület (inner town, max 1.0 km²),
        // 63 = külterület (outer area, max 1.5 km²).
        private static readonly int[] ProductIds = { 62, 63 };

        // Largest search square (km) whose area stays within the smallest limit (belterület, 1.0 km²).
        public const decimal MaxSearchSizeKm = 1.0M;

        private static readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        // bbox values are EOV (EPSG:23700) meters
        public async Task<List<OenyShapefileReader.RawParcel>> GetParcelsAsync(double minEasting, double minNorthing, double maxEasting, double maxNorthing)
        {
            var ring = new JArray(
                new JArray(minEasting, minNorthing),
                new JArray(maxEasting, minNorthing),
                new JArray(maxEasting, maxNorthing),
                new JArray(minEasting, maxNorthing),
                new JArray(minEasting, minNorthing));

            var addFeeParams = new JArray();
            var productChoices = new JArray();
            foreach (int productId in ProductIds)
            {
                addFeeParams.Add(new JObject { ["productID"] = productId, ["addFeeUsers"] = 0, ["addFeeYears"] = 0 });
                productChoices.Add(new JObject
                {
                    ["productID"] = productId,
                    ["choices"] = new JArray(
                        Choice("csomag", "földrészlet és épület+alrészlet+minőségi osztály"),
                        Choice("fileformat", "dxf"))
                });
            }

            var body = new JObject
            {
                ["addFeeParams"] = addFeeParams,
                ["productIDs"] = JArray.FromObject(ProductIds),
                ["detailLevel"] = 1,
                ["queryPolygon"] = new JObject
                {
                    ["type"] = "MultiPolygon",
                    // MultiPolygon nesting: [polygon][ring][point]. Collection initializers are used on purpose,
                    // new JArray(someJArray) is the copy constructor and would not add a nesting level.
                    ["coordinates"] = new JArray { new JArray { ring } }
                },
                ["productChoices"] = productChoices
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, CoverageUrl))
            {
                request.Headers.Add("X-Api-Key", ApiKey);
                request.Headers.Add("Origin", OriginUrl);
                request.Headers.Referrer = new Uri(RefererUrl);
                request.Headers.Accept.ParseAdd("application/json");
                request.Content = new StringContent(body.ToString(Newtonsoft.Json.Formatting.None), Encoding.UTF8, "application/json");

                using (var response = await _client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    return ParseCoverage(json);
                }
            }
        }

        private static JObject Choice(string type, string value)
        {
            return new JObject
            {
                ["productChoiceType"] = type,
                ["productChoiceValues"] = new JArray(new JObject { ["optionChoiceValue"] = value })
            };
        }

        private static List<OenyShapefileReader.RawParcel> ParseCoverage(string json)
        {
            var result = new List<OenyShapefileReader.RawParcel>();

            foreach (JToken product in JArray.Parse(json))
            {
                // productStatus 0 = ok, 1 = area over the product's limit, others (e.g. no coverage) just have no parcels.
                int status = product.Value<int?>("productStatus") ?? 0;
                if (status == 1)
                {
                    double maxArea = product.Value<double?>("maximumQueryableArea") ?? 0;
                    throw new InvalidOperationException($"GeoShop refused the query: the search area is over {maxArea:0.0} km². Use a smaller search size.");
                }

                if (!(product["discreteProducts"] is JArray items)) continue;

                foreach (JToken item in items)
                {
                    JToken feature = item["coveredItem"];
                    JToken description = feature?["properties"]?["description"];
                    JToken geometry = feature?["geometry"];
                    if (geometry == null) continue;

                    var parcel = new OenyShapefileReader.RawParcel
                    {
                        Hrsz = description?.Value<string>("itemDescription") ?? $"#{result.Count + 1}",
                        TownName = description?.Value<string>("itemGroupValue")
                    };

                    // Polygon: [ring][point][xy], MultiPolygon: [polygon][ring][point][xy]
                    string type = geometry.Value<string>("type");
                    JArray coordinates = (JArray)geometry["coordinates"];
                    IEnumerable<JToken> polygons = type == "Polygon" ? new JToken[] { coordinates } : (IEnumerable<JToken>)coordinates;

                    foreach (JToken polygon in polygons)
                    {
                        foreach (JToken ringToken in polygon)
                        {
                            var points = new List<(double Easting, double Northing)>();
                            foreach (JToken pt in ringToken)
                                points.Add(((double)pt[0], (double)pt[1]));
                            parcel.Rings.Add(points);
                        }
                    }

                    if (parcel.Rings.Count > 0)
                        result.Add(parcel);
                }
            }

            return result;
        }
    }
}
