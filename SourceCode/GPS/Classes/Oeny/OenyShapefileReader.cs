using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace AgOpenGPS.Classes.Oeny
{
    // Minimal reader for the Esri shapefile (.shp/.dbf) triplet returned by the OENY
    // "shape-zip" WFS output. Only the Polygon shape type (5) is supported, which is
    // what the hrsz:foldreszlet layer uses.
    public static class OenyShapefileReader
    {
        public class RawParcel
        {
            public string Hrsz;
            public string KshKod;
            // Rings in raw EOV meters: (Easting, Northing) per point, first ring is the outer boundary.
            public List<List<(double Easting, double Northing)>> Rings = new List<List<(double Easting, double Northing)>>();
        }

        public static List<RawParcel> ParseShapeZip(byte[] zipBytes)
        {
            byte[] shpBytes = null;
            byte[] dbfBytes = null;

            using (var zipStream = new MemoryStream(zipBytes))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string lower = entry.Name.ToLowerInvariant();
                    if (lower.EndsWith(".shp")) shpBytes = ReadAllBytes(entry);
                    else if (lower.EndsWith(".dbf")) dbfBytes = ReadAllBytes(entry);
                }
            }

            if (shpBytes == null || dbfBytes == null)
                throw new InvalidDataException("OENY shape-zip did not contain the expected .shp/.dbf files");

            List<Dictionary<string, string>> records = ReadDbfFields(dbfBytes, "hrsz", "ksh_kod");
            List<List<List<(double Easting, double Northing)>>> geometries = ReadShpPolygons(shpBytes);

            var result = new List<RawParcel>(geometries.Count);
            for (int i = 0; i < geometries.Count; i++)
            {
                string hrsz = i < records.Count && records[i].TryGetValue("hrsz", out string h) && !string.IsNullOrWhiteSpace(h) ? h : $"#{i + 1}";
                string kshKod = i < records.Count && records[i].TryGetValue("ksh_kod", out string k) ? k : string.Empty;

                result.Add(new RawParcel
                {
                    Hrsz = hrsz,
                    KshKod = kshKod,
                    Rings = geometries[i]
                });
            }
            return result;
        }

        private static byte[] ReadAllBytes(ZipArchiveEntry entry)
        {
            using (Stream stream = entry.Open())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        // Reads the requested text fields for every record, in on-disk record order.
        private static List<Dictionary<string, string>> ReadDbfFields(byte[] dbf, params string[] fieldNames)
        {
            var result = new List<Dictionary<string, string>>();

            Encoding encoding;
            try { encoding = Encoding.GetEncoding(1250); }
            catch { encoding = Encoding.UTF8; }

            int recordCount = BitConverter.ToInt32(dbf, 4);
            short headerLen = BitConverter.ToInt16(dbf, 8);
            short recordLen = BitConverter.ToInt16(dbf, 10);

            var fields = new List<(string Name, int Offset, int Length)>();
            int pos = 32;
            int fieldOffset = 1; // first byte of every record is a deletion flag
            while (dbf[pos] != 0x0D)
            {
                string name = encoding.GetString(dbf, pos, 11).TrimEnd('\0', ' ');
                int len = dbf[pos + 16];
                fields.Add((name, fieldOffset, len));
                fieldOffset += len;
                pos += 32;
            }

            var wantedIndices = fieldNames
                .Select(name => (name, index: fields.FindIndex(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase))))
                .ToList();

            for (int r = 0; r < recordCount; r++)
            {
                int recordStart = headerLen + r * recordLen;
                var record = new Dictionary<string, string>();

                foreach (var (name, index) in wantedIndices)
                {
                    if (index < 0) continue;
                    var f = fields[index];
                    record[name] = encoding.GetString(dbf, recordStart + f.Offset, f.Length).Trim();
                }

                result.Add(record);
            }

            return result;
        }

        // Reads all Polygon records; returns per-feature list of rings, each ring a list of (easting, northing) points.
        private static List<List<List<(double Easting, double Northing)>>> ReadShpPolygons(byte[] shp)
        {
            var features = new List<List<List<(double Easting, double Northing)>>>();
            int pos = 100; // past the fixed 100-byte main file header

            while (pos + 8 <= shp.Length)
            {
                int contentLengthWords = ReadInt32BE(shp, pos + 4);
                int contentStart = pos + 8;
                int contentLengthBytes = contentLengthWords * 2;

                int shapeType = BitConverter.ToInt32(shp, contentStart);
                var rings = new List<List<(double Easting, double Northing)>>();

                if (shapeType == 5) // Polygon
                {
                    int numParts = BitConverter.ToInt32(shp, contentStart + 36);
                    int numPoints = BitConverter.ToInt32(shp, contentStart + 40);

                    int partsStart = contentStart + 44;
                    var partStartIndices = new int[numParts];
                    for (int i = 0; i < numParts; i++)
                        partStartIndices[i] = BitConverter.ToInt32(shp, partsStart + i * 4);

                    int pointsStart = partsStart + numParts * 4;

                    for (int p = 0; p < numParts; p++)
                    {
                        int start = partStartIndices[p];
                        int end = (p + 1 < numParts) ? partStartIndices[p + 1] : numPoints;

                        var ring = new List<(double, double)>(end - start);
                        for (int i = start; i < end; i++)
                        {
                            int pointOffset = pointsStart + i * 16;
                            double easting = BitConverter.ToDouble(shp, pointOffset);
                            double northing = BitConverter.ToDouble(shp, pointOffset + 8);
                            ring.Add((easting, northing));
                        }
                        rings.Add(ring);
                    }
                }
                // shapeType 0 (Null Shape) or anything unsupported simply yields no rings for that record.

                features.Add(rings);
                pos = contentStart + contentLengthBytes;
            }

            return features;
        }

        private static int ReadInt32BE(byte[] data, int offset)
        {
            return (data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3];
        }
    }
}
