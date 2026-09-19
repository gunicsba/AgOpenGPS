using System;
using System.Collections.Generic;

namespace AgOpenGPS
{
    /// <summary>
    /// A grid of cells that remembers which spots of the field are painted (worked).
    /// The painted triangles are filled in, then any line can be measured for how much of it is painted.
    /// </summary>
    public class CoverageGrid
    {
        private readonly byte[] cells;
        private readonly int cols, rows;
        private readonly double minEasting, minNorthing;

        public readonly double cellSize;

        public CoverageGrid(double minE, double minN, double maxE, double maxN, double preferredCellSize = 1.0, int maxCells = 16000000)
        {
            double size = preferredCellSize;
            double width = Math.Max(maxE - minE, 1.0), height = Math.Max(maxN - minN, 1.0);

            //a huge field gets bigger cells so the grid stays a sensible size
            while ((width / size + 3) * (height / size + 3) > maxCells) size *= 1.25;

            cellSize = size;
            minEasting = minE - size;
            minNorthing = minN - size;
            cols = (int)(width / size) + 3;
            rows = (int)(height / size) + 3;
            cells = new byte[cols * rows];
        }

        /// <summary> Paints every cell of which the center is inside the triangle </summary>
        public void FillTriangle(double e1, double n1, double e2, double n2, double e3, double n3)
        {
            double x1 = (e1 - minEasting) / cellSize, y1 = (n1 - minNorthing) / cellSize;
            double x2 = (e2 - minEasting) / cellSize, y2 = (n2 - minNorthing) / cellSize;
            double x3 = (e3 - minEasting) / cellSize, y3 = (n3 - minNorthing) / cellSize;

            double denom = ((y2 - y3) * (x1 - x3)) + ((x3 - x2) * (y1 - y3));
            if (Math.Abs(denom) < 1e-12) return;

            int xStart = Math.Max(0, (int)Math.Floor(Math.Min(x1, Math.Min(x2, x3))));
            int xEnd = Math.Min(cols - 1, (int)Math.Floor(Math.Max(x1, Math.Max(x2, x3))));
            int yStart = Math.Max(0, (int)Math.Floor(Math.Min(y1, Math.Min(y2, y3))));
            int yEnd = Math.Min(rows - 1, (int)Math.Floor(Math.Max(y1, Math.Max(y2, y3))));

            for (int y = yStart; y <= yEnd; y++)
            {
                double py = y + 0.5;
                for (int x = xStart; x <= xEnd; x++)
                {
                    double px = x + 0.5;
                    double a = (((y2 - y3) * (px - x3)) + ((x3 - x2) * (py - y3))) / denom;
                    double b = (((y3 - y1) * (px - x3)) + ((x1 - x3) * (py - y3))) / denom;
                    if (a >= 0 && b >= 0 && (1.0 - a - b) >= 0) cells[(y * cols) + x] = 1;
                }
            }
        }

        public bool IsCovered(double easting, double northing)
        {
            int x = (int)Math.Floor((easting - minEasting) / cellSize);
            int y = (int)Math.Floor((northing - minNorthing) / cellSize);
            if (x < 0 || y < 0 || x >= cols || y >= rows) return false;
            return cells[(y * cols) + x] != 0;
        }

        /// <summary>
        /// Walks along the line and counts the spots that count (isInside, for example inside the boundary)
        /// and how many of those are painted.
        /// </summary>
        public void MeasureLine(IList<double[]> line, Func<double, double, bool> isInside, out int inside, out int covered)
        {
            inside = 0;
            covered = 0;

            for (int i = 0; i < line.Count - 1; i++)
            {
                double e1 = line[i][0], n1 = line[i][1];
                double dE = line[i + 1][0] - e1, dN = line[i + 1][1] - n1;
                int steps = (int)Math.Ceiling(Math.Sqrt((dE * dE) + (dN * dN)) / cellSize);

                for (int s = 0; s < steps; s++)
                {
                    double t = (double)s / steps;
                    double e = e1 + (dE * t), n = n1 + (dN * t);

                    if (!isInside(e, n)) continue;
                    inside++;
                    if (IsCovered(e, n)) covered++;
                }
            }
        }
    }
}
