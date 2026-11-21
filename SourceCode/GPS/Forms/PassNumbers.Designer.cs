using AgOpenGPS.Core.Models;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace AgOpenGPS
{
    public partial class FormGPS
    {
        // Clickable pass numbers 0..9 drawn on the parallel passes on the turn side.
        // Number N is the U-turn skip value that brings the turn to that pass
        // (same numbering as cboxpRowWidth: 0 is the adjacent pass).

        public bool isPassNumbersOn;

        // spacing of the number rows along the pass, in meters
        private const double passNumberSpacing = 10.0;

        // tap tolerance around a number, in screen pixels
        private const double passNumberHitRadius = 40.0;

        private struct PassNumberHit
        {
            public double X, Y; // screen pixels, 0 at top-left of oglMain
            public int Number;
            public bool IsLeft; // pass is left of the travel direction
        }

        // screen positions of the numbers drawn in the last frame, used for hit testing
        private readonly List<PassNumberHit> passNumberHits = new List<PassNumberHit>();

        private bool IsPassNumbersActive =>
            isPassNumbersOn && isJobStarted && trk.idx > -1 && !ct.isContourBtnOn && !yt.IsSpiralTurn
            && camera.camSetDistance > -600;

        /// <summary>
        /// Draw the pass numbers in world space. Must be called while the world modelview is active.
        /// </summary>
        private void DrawPassNumbers()
        {
            passNumberHits.Clear();
            if (!IsPassNumbersActive) return;

            double widthMinusOverlap = tool.width - tool.overlap;
            if (widthMinusOverlap < 0.1) return;

            // stations (point + travel heading) along the current pass, ground fixed every passNumberSpacing
            List<vec3> stations = GetPassNumberStations();
            if (stations.Count == 0) return;

            // matrices for world -> screen projection (hit testing)
            double[] modl = new double[16];
            double[] proj = new double[16];
            int[] vp = new int[4];
            GL.GetDouble(GetPName.ModelviewMatrix, modl);
            GL.GetDouble(GetPName.ProjectionMatrix, proj);
            GL.GetInteger(GetPName.Viewport, vp);

            // label size in world units, same growth with zoom as the flag text,
            // capped so neighbouring passes don't overlap
            double size = Math.Pow(-camera.camSetDistance, 0.85) / 1000.0 * 1.4;
            double maxSize = widthMinusOverlap * 0.6 / Font32;
            if (size > maxSize) size = maxSize;

            int selected = yt.rowSkipsWidth - 1;

            bool hasBoundary = bnd.bndList.Count > 0 && bnd.bndList[0].fenceLine.Count > 2;

            GL.Enable(EnableCap.Blend);

            foreach (vec3 st in stations)
            {
                // right of travel direction
                double rightE = Math.Cos(st.heading);
                double rightN = -Math.Sin(st.heading);

                // both sides, the turn side is the active one
                for (int s = 0; s < 2; s++)
                {
                    bool isLeft = s == 0;
                    bool isTurnSide = isLeft == yt.isTurnLeft;
                    double side = isLeft ? -1.0 : 1.0;
                    double toolOffset2 = isLeft ? -tool.offset * 2.0 : tool.offset * 2.0;

                    for (int n = 0; n < 10; n++)
                    {
                        // same lateral offset as the U-turn uses for its target line
                        double dist = widthMinusOverlap * (n + 1) + toolOffset2;
                        double e = st.easting + side * rightE * dist;
                        double nn = st.northing + side * rightN * dist;

                        if (!IsPointInFrustum(e, nn)) continue;
                        if (hasBoundary && !IsPointInsideField(new vec2(e, nn))) continue;

                        bool isSel = isTurnSide && n == selected;
                        DrawPassNumberLabel(e, nn, n.ToString(), isSel ? size * 1.3 : size, isSel, isTurnSide);

                        if (ProjectToScreen(e, nn, modl, proj, vp, out double sx, out double sy))
                            passNumberHits.Add(new PassNumberHit { X = sx, Y = sy, Number = n, IsLeft = isLeft });
                    }
                }
            }
        }

        private const double Font32 = 32.0; // glyph height of the font in units of 'size'

        /// <summary>
        /// Points on the current guidance line every passNumberSpacing meters near the vehicle.
        /// Heading of each point is the travel direction.
        /// </summary>
        private List<vec3> GetPassNumberStations()
        {
            var list = new List<vec3>();

            // only draw rows within this distance forward and backward of the vehicle
            const double range = 60.0;
            vec2 piv = new vec2(pivotAxlePos.easting, pivotAxlePos.northing);

            if (trk.gArr[trk.idx].mode == TrackMode.AB)
            {
                double h = ABLine.abHeading;
                double dirE = Math.Sin(h), dirN = Math.Cos(h);
                vec3 a = ABLine.currentLinePtA;

                double travel = ABLine.isHeadingSameWay ? h : h + Math.PI;

                // project pivot onto the line, stations are multiples of spacing from ptA (ground fixed)
                double sPiv = (piv.easting - a.easting) * dirE + (piv.northing - a.northing) * dirN;
                double sStart = Math.Ceiling((sPiv - range) / passNumberSpacing) * passNumberSpacing;

                for (double s = sStart; s <= sPiv + range; s += passNumberSpacing)
                {
                    list.Add(new vec3(a.easting + dirE * s, a.northing + dirN * s, travel));
                }
            }
            else
            {
                List<vec3> cur = curve.curList;
                if (cur == null || cur.Count < 2) return list;

                double rangeSq = range * range;
                double acc = 0; // distance along the curve from its first point
                double next = 0; // next station distance

                for (int i = 0; i < cur.Count - 1; i++)
                {
                    double segE = cur[i + 1].easting - cur[i].easting;
                    double segN = cur[i + 1].northing - cur[i].northing;
                    double segLen = Math.Sqrt(segE * segE + segN * segN);
                    if (segLen < 1e-6) continue;

                    while (next <= acc + segLen)
                    {
                        double t = (next - acc) / segLen;
                        double e = cur[i].easting + segE * t;
                        double nn = cur[i].northing + segN * t;

                        double dE = e - piv.easting, dN = nn - piv.northing;
                        if (dE * dE + dN * dN < rangeSq)
                        {
                            double h = Math.Atan2(segE, segN);
                            double travel = curve.isHeadingSameWay ? h : h + Math.PI;
                            list.Add(new vec3(e, nn, travel));
                        }
                        next += passNumberSpacing;
                    }
                    acc += segLen;
                }
            }

            return list;
        }

        private bool IsPointInFrustum(double e, double n)
        {
            if (frustum[0] * e + frustum[1] * n + frustum[3] <= 0) return false;//right
            if (frustum[4] * e + frustum[5] * n + frustum[7] <= 0) return false;//left
            if (frustum[16] * e + frustum[17] * n + frustum[19] <= 0) return false;//bottom
            if (frustum[20] * e + frustum[21] * n + frustum[23] <= 0) return false;//top
            if (frustum[8] * e + frustum[9] * n + frustum[11] <= 0) return false;//far
            if (frustum[12] * e + frustum[13] * n + frustum[15] <= 0) return false;//near
            return true;
        }

        private bool IsPointInsideField(vec2 pt)
        {
            if (!bnd.bndList[0].fenceLine.IsPointInPolygon(pt)) return false;
            for (int i = 1; i < bnd.bndList.Count; i++)
            {
                if (bnd.bndList[i].fenceLine.IsPointInPolygon(pt)) return false;
            }
            return true;
        }

        /// <summary>
        /// Draw a number centered on (e, n), flat on the ground and turned to the camera heading.
        /// </summary>
        private void DrawPassNumberLabel(double e, double n, string text, double size, bool isSelected, bool isTurnSide)
        {
            double w = text.Length * 16.0 * size;
            double h = Font32 * size;
            double pad = 4.0 * size;

            GL.PushMatrix();
            GL.Translate(e, n, 0);
            if (camera.FollowDirectionHint) GL.Rotate(-camHeading, 0, 0, 1);

            // dark background box
            GL.Color4(0.0, 0.0, 0.0, isSelected ? 0.75 : 0.5);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex3(-w / 2 - pad, -h / 2, 0);
            GL.Vertex3(w / 2 + pad, -h / 2, 0);
            GL.Vertex3(w / 2 + pad, h / 2, 0);
            GL.Vertex3(-w / 2 - pad, h / 2, 0);
            GL.End();

            if (isSelected) GL.Color3(1.0f, 1.0f, 0.0f);
            else if (isTurnSide) GL.Color3(0.95f, 0.95f, 0.95f);
            else GL.Color3(0.65f, 0.65f, 0.7f); // other side is dimmer


            // font.DrawText has y going down (screen space), flip it for the world
            GL.Scale(1, -1, 1);
            font.DrawText(-w / 2, -h / 2, text, size);

            GL.PopMatrix();
        }

        private bool ProjectToScreen(double e, double n, double[] m, double[] p, int[] vp, out double sx, out double sy)
        {
            sx = sy = 0;

            // eye = modelview * (e, n, 0, 1)   (column major)
            double ex = m[0] * e + m[4] * n + m[12];
            double ey = m[1] * e + m[5] * n + m[13];
            double ez = m[2] * e + m[6] * n + m[14];
            double ew = m[3] * e + m[7] * n + m[15];

            // clip = projection * eye
            double cx = p[0] * ex + p[4] * ey + p[8] * ez + p[12] * ew;
            double cy = p[1] * ex + p[5] * ey + p[9] * ez + p[13] * ew;
            double cw = p[3] * ex + p[7] * ey + p[11] * ez + p[15] * ew;
            if (cw <= 1e-9) return false;

            double ndcX = cx / cw;
            double ndcY = cy / cw;

            sx = vp[0] + (ndcX + 1.0) * 0.5 * vp[2];
            sy = oglMain.Height - (vp[1] + (ndcY + 1.0) * 0.5 * vp[3]);
            return true;
        }

        /// <summary>
        /// Called from oglMain_MouseDown. Returns true if a pass number was tapped.
        /// </summary>
        private bool TryPickPassNumber(int x, int y)
        {
            if (!IsPassNumbersActive || passNumberHits.Count == 0) return false;

            int best = -1;
            bool bestIsLeft = false;
            double bestDist = passNumberHitRadius * passNumberHitRadius;
            foreach (var hit in passNumberHits)
            {
                double dx = hit.X - x, dy = hit.Y - y;
                double d = dx * dx + dy * dy;
                if (d < bestDist)
                {
                    bestDist = d;
                    best = hit.Number;
                    bestIsLeft = hit.IsLeft;
                }
            }
            if (best < 0) return false;

            SetSkipFromPassNumber(best, bestIsLeft);
            return true;
        }

        private void SetSkipFromPassNumber(int n, bool isLeft)
        {
            if (n < 0 || n >= cboxpRowWidth.Items.Count) return;

            // number on the other side: turn to that side as well (not while a turn is running)
            if (isLeft != yt.isTurnLeft)
            {
                if (yt.isYouTurnTriggered) return;
                yt.isTurnLeft = isLeft;
            }

            // Alternative and IgnoreWorkedTracks need a skip of at least 1,
            // so tapping the adjacent pass (0) goes back to normal skipping
            if (n == 0 && yt.skipMode != SkipMode.Normal)
            {
                yt.skipMode = SkipMode.Normal;
            }

            if (cboxpRowWidth.SelectedIndex != n)
            {
                // the combo handler sets rowSkipsWidth, resets the planned turn and saves
                cboxpRowWidth.SelectedIndex = n;
            }
            else
            {
                // same value (side change, or Alternative mid-alternation): restart from the picked value
                yt.rowSkipsWidth = n + 1;
                yt.Set_Alternate_skips();
                UpdateSkipButton();
                if (!yt.isYouTurnTriggered) yt.ResetCreatedYouTurn();
            }
        }
    }
}
