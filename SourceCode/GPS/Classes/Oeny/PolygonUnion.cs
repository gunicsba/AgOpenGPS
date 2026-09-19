using System;
using System.Collections.Generic;
using System.Linq;

namespace AgOpenGPS.Classes.Oeny
{
    // Merges adjacent simple polygons (e.g. neighbouring cadastral parcels from the same
    // survey) by cancelling out edges that are shared, exactly, between two of them.
    // This only works when shared borders have coincident vertices, which is the normal
    // case for parcels coming from the same shapefile export.
    public static class PolygonUnion
    {
        private const double Epsilon = 0.01; // 1 cm, matches expected shapefile vertex precision

        // Returns one ring per disjoint (non-adjacent) region, largest area first.
        // Falls back to returning the input rings unchanged if the edges don't cancel out cleanly.
        public static List<List<vec2>> Union(List<List<vec2>> polygons)
        {
            if (polygons == null || polygons.Count == 0) return new List<List<vec2>>();
            if (polygons.Count == 1) return new List<List<vec2>> { polygons[0] };

            var edges = new List<(vec2 A, vec2 B)>();
            foreach (var ring in polygons)
            {
                int n = ring.Count;
                if (n < 3) continue;
                for (int i = 0; i < n; i++)
                {
                    vec2 a = ring[i];
                    vec2 b = ring[(i + 1) % n];
                    if (NearlyEqual(a, b)) continue; // skip zero-length / duplicate closing point
                    edges.Add((a, b));
                }
            }

            var remaining = new List<(vec2 A, vec2 B)>(edges);
            for (int i = 0; i < remaining.Count; i++)
            {
                for (int j = i + 1; j < remaining.Count; j++)
                {
                    if (NearlyEqual(remaining[i].A, remaining[j].B) && NearlyEqual(remaining[i].B, remaining[j].A))
                    {
                        remaining.RemoveAt(j);
                        remaining.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            if (remaining.Count < 3) return polygons; // nothing cancelled, or union failed - keep originals

            var loops = ChainIntoLoops(remaining);
            return loops.Count > 0 ? loops.OrderByDescending(PolygonAreaAbs).ToList() : polygons;
        }

        private static List<List<vec2>> ChainIntoLoops(List<(vec2 A, vec2 B)> remaining)
        {
            var loops = new List<List<vec2>>();
            var used = new bool[remaining.Count];

            for (int start = 0; start < remaining.Count; start++)
            {
                if (used[start]) continue;

                var loop = new List<vec2> { remaining[start].A };
                vec2 origin = remaining[start].A;
                vec2 current = remaining[start].B;
                used[start] = true;
                loop.Add(current);

                int guard = 0;
                while (!NearlyEqual(current, origin) && guard++ < remaining.Count * 2)
                {
                    int nextIndex = -1;
                    for (int k = 0; k < remaining.Count; k++)
                    {
                        if (used[k]) continue;
                        if (NearlyEqual(remaining[k].A, current))
                        {
                            nextIndex = k;
                            break;
                        }
                    }
                    if (nextIndex == -1) break; // broken chain, e.g. a T-junction corner

                    used[nextIndex] = true;
                    current = remaining[nextIndex].B;
                    loop.Add(current);
                }

                if (loop.Count >= 4 && NearlyEqual(loop[0], loop[loop.Count - 1]))
                {
                    loop.RemoveAt(loop.Count - 1); // drop duplicate closing point
                    loops.Add(loop);
                }
            }

            return loops;
        }

        private static bool NearlyEqual(vec2 a, vec2 b)
        {
            return Math.Abs(a.easting - b.easting) < Epsilon && Math.Abs(a.northing - b.northing) < Epsilon;
        }

        private static double PolygonAreaAbs(List<vec2> ring)
        {
            double area = 0;
            int j = ring.Count - 1;
            for (int i = 0; i < ring.Count; j = i++)
                area += (ring[j].easting + ring[i].easting) * (ring[j].northing - ring[i].northing);
            return Math.Abs(area / 2.0);
        }
    }
}
