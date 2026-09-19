using AgLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AgOpenGPS
{
    public enum CoverageState { Unavailable, Building, Ready }

    /// <summary>
    /// Keeps a coverage grid of the field up to date with the painted patches, so we can ask how much of a
    /// track is already worked, no matter how it got worked.
    /// The grid is built on a background thread, the guidance and autosteer never wait for it.
    /// </summary>
    public class CCoverageMap
    {
        private readonly FormGPS mf;
        private readonly object sync = new object();

        private CoverageGrid grid;
        private bool isBuilding, hasFailed;
        private int generation;

        //per patch the index of the next triangle that isn't in the grid yet, patches only get points added
        private Dictionary<List<vec3>, int> done = new Dictionary<List<vec3>, int>();

        public CCoverageMap(FormGPS _f)
        {
            mf = _f;
        }

        /// <summary> Forget everything, the next question builds the grid again (field closed, opened or coverage deleted) </summary>
        public void Reset()
        {
            lock (sync)
            {
                grid = null;
                done = new Dictionary<List<vec3>, int>();
                isBuilding = false;
                hasFailed = false;
                generation++;
            }
        }

        /// <summary>
        /// Ready when the grid can be used. The first call after a reset starts building it in the background
        /// and returns Building until it is done, after that only what was painted since is added, that is quick.
        /// </summary>
        public CoverageState GetState()
        {
            if (mf.bnd.bndList.Count == 0 || mf.bnd.bndList[0].fenceLine.Count < 3) return CoverageState.Unavailable;

            lock (sync)
            {
                if (hasFailed) return CoverageState.Unavailable;
                if (isBuilding) return CoverageState.Building;
                if (grid == null)
                {
                    StartBuild();
                    return CoverageState.Building;
                }
            }

            CatchUp();
            return CoverageState.Ready;
        }

        //called inside the lock, on the main thread. Looks at the patches now and lets a thread do the painting
        private void StartBuild()
        {
            double minE = double.MaxValue, minN = double.MaxValue, maxE = double.MinValue, maxN = double.MinValue;
            foreach (vec3 pt in mf.bnd.bndList[0].fenceLine)
            {
                minE = Math.Min(minE, pt.easting);
                maxE = Math.Max(maxE, pt.easting);
                minN = Math.Min(minN, pt.northing);
                maxN = Math.Max(maxN, pt.northing);
            }

            //the thread only reads what is in the patches right now, they only grow so that is safe
            var snapshot = new List<KeyValuePair<List<vec3>, int>>();
            foreach (CPatches section in mf.triStrip)
            {
                foreach (List<vec3> strip in section.patchList)
                {
                    snapshot.Add(new KeyValuePair<List<vec3>, int>(strip, strip.Count));
                }
            }

            int startedGeneration = generation;
            isBuilding = true;

            var worker = new Thread(() =>
            {
                CoverageGrid newGrid = null;
                var newDone = new Dictionary<List<vec3>, int>();
                bool failed = false;

                try
                {
                    newGrid = new CoverageGrid(minE, minN, maxE, maxN);
                    foreach (var item in snapshot)
                    {
                        newDone[item.Key] = Paint(newGrid, item.Key, 1, item.Value);
                    }
                }
                catch (Exception ex)
                {
                    failed = true;
                    Log.EventWriter("Coverage grid failed: " + ex);
                }

                lock (sync)
                {
                    //a reset while we were busy means this result is of no use
                    if (startedGeneration == generation)
                    {
                        if (failed) hasFailed = true;
                        else
                        {
                            grid = newGrid;
                            done = newDone;
                        }
                        isBuilding = false;
                    }
                }
            });

            worker.IsBackground = true;
            worker.Priority = ThreadPriority.BelowNormal;
            worker.Start();
        }

        //first vertex is the color, the triangles are made of 3 following vertices. Returns the next triangle to do.
        private static int Paint(CoverageGrid target, List<vec3> strip, int next, int count)
        {
            if (next < 1) next = 1;
            for (; next + 2 < count; next++)
            {
                target.FillTriangle(strip[next].easting, strip[next].northing,
                    strip[next + 1].easting, strip[next + 1].northing,
                    strip[next + 2].easting, strip[next + 2].northing);
            }
            return next;
        }

        //main thread, adds what got painted after the grid was built
        private void CatchUp()
        {
            foreach (CPatches section in mf.triStrip)
            {
                foreach (List<vec3> strip in section.patchList)
                {
                    done.TryGetValue(strip, out int next);

                    //a patch that got shorter was emptied, build it all again
                    if (next > strip.Count)
                    {
                        Reset();
                        return;
                    }

                    done[strip] = Paint(grid, strip, next, strip.Count);
                }
            }
        }

        /// <summary>
        /// Percent (0-100) of the part of the line inside the field boundary (and outside the inner ones) that is painted.
        /// Returns -1 when hardly any of the line is inside the boundary. Only call this when GetState gave Ready.
        /// </summary>
        public double GetLineCoverage(List<vec3> line)
        {
            if (grid == null || line == null || line.Count < 2) return -1;

            var points = new List<double[]>(line.Count);
            foreach (vec3 pt in line) points.Add(new[] { pt.easting, pt.northing });

            grid.MeasureLine(points, (e, n) => mf.bnd.IsPointInsideFenceArea(new vec2(e, n)), out int inside, out int covered);

            //a track that just clips a corner is not a track we can judge
            if (inside < 5) return -1;
            return covered * 100.0 / inside;
        }
    }
}
