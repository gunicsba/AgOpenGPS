using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace AgOpenGPS.Classes.Oeny
{
    // Downloads Hungarian cadastral parcel shapefiles from the public OENY (oeny.hu) WFS endpoint.
    // The endpoint requires a Referer header matching the HRSZ search page or it redirects to /error/403.
    public class OenyClient
    {
        private const string WfsUrl = "https://www.oeny.hu/hk-geoserver/hrsz/wfs";
        private const string RefererUrl = "https://www.oeny.hu/oeny/hrsz-kereso/";

        // bbox values are EOV (EPSG:23700) meters: minEasting, minNorthing, maxEasting, maxNorthing
        public async Task<byte[]> DownloadShapeZipAsync(double minEasting, double minNorthing, double maxEasting, double maxNorthing)
        {
            string bbox = string.Join(",",
                Inv(minEasting), Inv(minNorthing), Inv(maxEasting), Inv(maxNorthing), "EPSG:23700");

            string url = $"{WfsUrl}?SERVICE=WFS&VERSION=1.1.0&REQUEST=GetFeature&TYPENAME=hrsz:foldreszlet" +
                         $"&OUTPUTFORMAT=shape-zip&SRSNAME=EPSG:23700&BBOX={bbox}";

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Referrer = new Uri(RefererUrl);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (AgOpenGPS)");

                using (var response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
        }

        private static string Inv(double value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
