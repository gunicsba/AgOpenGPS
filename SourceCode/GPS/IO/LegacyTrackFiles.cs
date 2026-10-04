// LegacyTrackFiles.cs
// Purpose: Convert v5 era ABLines.txt + CurveLines.txt into TrackLines.txt.
// Up to 6.4 this happened in FileLoadTracks; it was lost when field loading was refactored.
// The old files are left untouched, so the field still opens in an old version too.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AgLibrary.Logging;

namespace AgOpenGPS.IO
{
    public static class LegacyTrackFiles
    {
        private const string ABLinesFile = "ABLines.txt";
        private const string CurveLinesFile = "CurveLines.txt";

        /// <summary>
        /// True when the field has old style AB/curve files but no TrackLines.txt yet.
        /// </summary>
        public static bool NeedsConversion(string fieldDirectory)
        {
            return !File.Exists(Path.Combine(fieldDirectory, "TrackLines.txt")) &&
                   (File.Exists(Path.Combine(fieldDirectory, ABLinesFile)) ||
                    File.Exists(Path.Combine(fieldDirectory, CurveLinesFile)));
        }

        /// <summary>
        /// Writes TrackLines.txt from the old files. Throws on malformed content, in which case
        /// nothing is written.
        /// </summary>
        public static int Convert(string fieldDirectory)
        {
            var tracks = new List<CTrk>();
            tracks.AddRange(LoadABLines(Path.Combine(fieldDirectory, ABLinesFile)));
            tracks.AddRange(LoadCurveLines(Path.Combine(fieldDirectory, CurveLinesFile)));

            TrackFiles.Save(fieldDirectory, tracks);
            Log.EventWriter($"Converted {tracks.Count} old AB/curve line(s) to TrackLines.txt in {fieldDirectory}");
            return tracks.Count;
        }

        /// <summary>
        /// Converts every field under the Fields directory that still has only old style lines.
        /// A broken field is logged and skipped. Returns the number of fields converted.
        /// </summary>
        public static int ConvertAllFields(string fieldsDirectory)
        {
            if (string.IsNullOrEmpty(fieldsDirectory) || !Directory.Exists(fieldsDirectory)) return 0;

            int converted = 0;
            foreach (var dir in Directory.GetDirectories(fieldsDirectory))
            {
                try
                {
                    if (!File.Exists(Path.Combine(dir, "Field.txt")) || !NeedsConversion(dir)) continue;

                    Convert(dir);
                    converted++;
                }
                catch (Exception ex)
                {
                    Log.EventWriter($"Converting old AB/curve lines failed in {dir}: {ex.Message}");
                }
            }
            return converted;
        }

        // One line per AB line: name,heading in degrees,easting,northing
        private static List<CTrk> LoadABLines(string path)
        {
            var result = new List<CTrk>();
            if (!File.Exists(path)) return result;

            foreach (var raw in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;

                var words = raw.Split(',');
                if (words.Length < 4) throw new InvalidDataException("Bad line in ABLines.txt: " + raw);

                // The numbers are the last three fields, so a comma in the name does not break the line
                int n = words.Length;
                string name = string.Join(",", words, 0, n - 3).Trim();
                double heading = glm.toRadians(double.Parse(words[n - 3], CultureInfo.InvariantCulture));
                var ptA = new vec2(double.Parse(words[n - 2], CultureInfo.InvariantCulture),
                                   double.Parse(words[n - 1], CultureInfo.InvariantCulture));

                result.Add(new CTrk
                {
                    name = name.StartsWith("AB", StringComparison.Ordinal) ? name : "AB " + name,
                    mode = TrackMode.AB,
                    heading = heading,
                    ptA = ptA,
                    ptB = new vec2(ptA.easting + Math.Sin(heading) * 100, ptA.northing + Math.Cos(heading) * 100)
                });
            }
            return result;
        }

        // $CurveLines header, then per curve: name, average heading (radians), point count, points
        private static List<CTrk> LoadCurveLines(string path)
        {
            var result = new List<CTrk>();
            if (!File.Exists(path)) return result;

            using (var reader = new StreamReader(path))
            {
                reader.ReadLine(); // $CurveLines

                string name;
                while ((name = reader.ReadLine()) != null)
                {
                    name = name.Trim();
                    if (name.Length == 0) continue;

                    double heading = double.Parse(ReadRequired(reader), CultureInfo.InvariantCulture);
                    int count = int.Parse(ReadRequired(reader).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture);

                    var pts = new List<vec3>(count);
                    for (int i = 0; i < count; i++)
                    {
                        var parts = ReadRequired(reader).Split(',');
                        pts.Add(new vec3(double.Parse(parts[0], CultureInfo.InvariantCulture),
                                         double.Parse(parts[1], CultureInfo.InvariantCulture),
                                         double.Parse(parts[2], CultureInfo.InvariantCulture)));
                    }

                    // A curve needs at least two points to be usable
                    if (pts.Count < 2) continue;

                    bool isBoundaryCurve = name.StartsWith("Bound", StringComparison.Ordinal);
                    result.Add(new CTrk
                    {
                        name = isBoundaryCurve || name.StartsWith("Cu", StringComparison.Ordinal) ? name : "Cu " + name,
                        mode = isBoundaryCurve ? TrackMode.bndCurve : TrackMode.Curve,
                        heading = heading,
                        curvePts = pts,
                        ptA = new vec2(pts[0].easting, pts[0].northing),
                        ptB = new vec2(pts[pts.Count - 1].easting, pts[pts.Count - 1].northing)
                    });
                }
            }
            return result;
        }

        private static string ReadRequired(StreamReader reader)
        {
            var line = reader.ReadLine();
            if (line == null) throw new InvalidDataException("Unexpected end of CurveLines.txt");
            return line;
        }
    }
}
