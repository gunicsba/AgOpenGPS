using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgLibrary.Logging;
using AgOpenGPS.Classes.Oeny;
using AgOpenGPS.Core.Models;

namespace AgOpenGPS.Forms.Field
{
    // Lets the user search Hungarian OENY cadastral parcels (HRSZ) near the current position,
    // preview them on a pan/zoomable map, and pick one or more to use as the field boundary.
    public class FormOenyImport : Form
    {
        private class ParcelEntry
        {
            public string Hrsz;
            public string TownName;
            public List<List<vec2>> LocalRings; // local plane easting/northing, ring 0 = outer boundary
            public double AreaHa;

            public string DisplayName => string.IsNullOrEmpty(TownName) ? Hrsz : $"{TownName} - {Hrsz}";
        }

        private readonly FormGPS mf;
        private readonly OenyClient _oenyClient = new OenyClient();
        private readonly EhtTransformationClient _ehtClient = new EhtTransformationClient();

        private List<ParcelEntry> _parcels = new List<ParcelEntry>();

        // View state for the preview panel (pan/zoom around the auto-fit view)
        private double _zoom = 1.0;
        private double _panEasting, _panNorthing;
        private double _fitScale = 1.0, _centerEasting, _centerNorthing;
        private bool _isDragging;
        private bool _didDrag;
        private Point _dragStartMouse;
        private double _dragStartPanEasting, _dragStartPanNorthing;

        private readonly Timer _vehicleTimer = new Timer { Interval = 500 };

        private NumericUpDown nudRadiusKm;
        private Button btnSearch;
        private Button btnAddSelected;
        private Button btnCancel;
        private CheckedListBox clbParcels;
        private Panel pnlPreview;
        private Label lblStatus;
        private Button btnZoomIn, btnZoomOut;
        private Button btnPanUp, btnPanDown, btnPanLeft, btnPanRight;

        public FormOenyImport(Form callingForm)
        {
            mf = callingForm as FormGPS;
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Import Boundary from OENY (HRSZ)";
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(960, 680);
            MinimumSize = new Size(700, 480);

            var lblRadius = new Label { Text = "Search size (km):", Left = 12, Top = 18, Width = 110, AutoSize = true };
            nudRadiusKm = new NumericUpDown
            {
                Left = 130,
                Top = 14,
                Width = 60,
                Minimum = 0.1M,
                Maximum = 10M,
                DecimalPlaces = 1,
                Increment = 0.1M,
                Value = 0.3M
            };

            btnSearch = new Button { Text = "Search Here", Left = 200, Top = 11, Width = 130, Height = 30 };
            btnSearch.Click += async (s, e) => await SearchAsync();

            lblStatus = new Label { Left = 340, Top = 18, Width = 600, AutoSize = false, Height = 20, Text = string.Empty, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            clbParcels = new CheckedListBox
            {
                Left = 12,
                Top = 50,
                Width = 260,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
                CheckOnClick = true,
                IntegralHeight = false
            };
            clbParcels.ItemCheck += (s, e) => BeginInvoke((Action)(() => pnlPreview.Invalidate()));

            pnlPreview = new Panel
            {
                Left = 284,
                Top = 50,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlPreview.Paint += PnlPreview_Paint;
            pnlPreview.MouseDown += PnlPreview_MouseDown;
            pnlPreview.MouseMove += PnlPreview_MouseMove;
            pnlPreview.MouseUp += PnlPreview_MouseUp;
            pnlPreview.MouseWheel += PnlPreview_MouseWheel;
            pnlPreview.Resize += (s, e) => pnlPreview.Invalidate();

            // Touch-friendly zoom buttons, top-right of the map
            btnZoomIn = new Button { Text = "+", Font = new Font("Tahoma", 20F, FontStyle.Bold), Width = 72, Height = 72, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnZoomOut = new Button { Text = "-", Font = new Font("Tahoma", 20F, FontStyle.Bold), Width = 72, Height = 72, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnZoomIn.Click += (s, e) => ZoomBy(1.25);
            btnZoomOut.Click += (s, e) => ZoomBy(0.8);

            // Touch-friendly pan (D-pad), top-left of the map
            btnPanUp = new Button { Text = "\u25B2", Width = 60, Height = 60, Anchor = AnchorStyles.Top | AnchorStyles.Left };
            btnPanDown = new Button { Text = "\u25BC", Width = 60, Height = 60, Anchor = AnchorStyles.Top | AnchorStyles.Left };
            btnPanLeft = new Button { Text = "\u25C4", Width = 60, Height = 60, Anchor = AnchorStyles.Top | AnchorStyles.Left };
            btnPanRight = new Button { Text = "\u25BA", Width = 60, Height = 60, Anchor = AnchorStyles.Top | AnchorStyles.Left };
            btnPanUp.Click += (s, e) => PanBy(0, 1);
            btnPanDown.Click += (s, e) => PanBy(0, -1);
            // Left/right are inverted relative to dx because the view center moves opposite the arrow's intent.
            btnPanLeft.Click += (s, e) => PanBy(1, 0);
            btnPanRight.Click += (s, e) => PanBy(-1, 0);

            pnlPreview.Controls.Add(btnZoomIn);
            pnlPreview.Controls.Add(btnZoomOut);
            pnlPreview.Controls.Add(btnPanUp);
            pnlPreview.Controls.Add(btnPanDown);
            pnlPreview.Controls.Add(btnPanLeft);
            pnlPreview.Controls.Add(btnPanRight);

            btnAddSelected = new Button
            {
                Text = "Add Selected as Boundary",
                Left = 12,
                Width = 240,
                Height = 40,
                Enabled = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnAddSelected.Click += BtnAddSelected_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Width = 172,
                Height = 40,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnCancel.Click += (s, e) => Close();

            Controls.Add(lblRadius);
            Controls.Add(nudRadiusKm);
            Controls.Add(btnSearch);
            Controls.Add(lblStatus);
            Controls.Add(clbParcels);
            Controls.Add(pnlPreview);
            Controls.Add(btnAddSelected);
            Controls.Add(btnCancel);

            LayoutFixedControls();
            Resize += (s, e) => LayoutFixedControls();

            _vehicleTimer.Tick += (s, e) => pnlPreview.Invalidate();
            _vehicleTimer.Start();
            FormClosed += (s, e) => _vehicleTimer.Stop();
        }

        // Positions everything that depends on the current client size (bottom buttons, map overlay buttons).
        private void LayoutFixedControls()
        {
            int bottomButtonsTop = ClientSize.Height - 48;
            clbParcels.Height = bottomButtonsTop - 10 - clbParcels.Top;
            pnlPreview.Width = ClientSize.Width - pnlPreview.Left - 12;
            pnlPreview.Height = bottomButtonsTop - 10 - pnlPreview.Top;

            btnAddSelected.Top = bottomButtonsTop;
            btnCancel.Top = bottomButtonsTop;
            btnCancel.Left = ClientSize.Width - btnCancel.Width - 12;

            btnZoomIn.Left = pnlPreview.Width - btnZoomIn.Width - 10;
            btnZoomIn.Top = 10;
            btnZoomOut.Left = pnlPreview.Width - btnZoomOut.Width - 10;
            btnZoomOut.Top = btnZoomIn.Bottom + 6;

            btnPanUp.Left = 10 + btnPanLeft.Width;
            btnPanUp.Top = 10;
            btnPanLeft.Left = 10;
            btnPanLeft.Top = 10 + btnPanUp.Height;
            btnPanRight.Left = btnPanUp.Left + btnPanUp.Width;
            btnPanRight.Top = btnPanLeft.Top;
            btnPanDown.Left = btnPanUp.Left;
            btnPanDown.Top = btnPanLeft.Top + btnPanLeft.Height;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await SearchAsync();
        }

        private async Task SearchAsync()
        {
            if (mf == null) return;

            btnSearch.Enabled = false;
            btnAddSelected.Enabled = false;
            lblStatus.Text = "Searching...";
            clbParcels.Items.Clear();
            _parcels = new List<ParcelEntry>();
            pnlPreview.Invalidate();

            try
            {
                Task townsTask = KshLookupClient.EnsureLoadedAsync();

                Wgs84 center = mf.AppModel.CurrentLatLon;
                var (centerEasting, centerNorthing) = await _ehtClient.Wgs84ToEovAsync(center);

                double halfSizeM = (double)nudRadiusKm.Value * 1000.0 / 2.0;

                byte[] zip = await _oenyClient.DownloadShapeZipAsync(
                    centerEasting - halfSizeM, centerNorthing - halfSizeM,
                    centerEasting + halfSizeM, centerNorthing + halfSizeM);

                List<OenyShapefileReader.RawParcel> rawParcels = OenyShapefileReader.ParseShapeZip(zip);

                if (rawParcels.Count == 0)
                {
                    lblStatus.Text = "No parcels found in this area.";
                    return;
                }

                // Flatten every ring point across every parcel into one batch conversion call.
                var allPoints = new List<(double Easting, double Northing)>();
                var ringPointCounts = new List<List<int>>();

                foreach (var raw in rawParcels)
                {
                    var counts = new List<int>();
                    foreach (var ring in raw.Rings)
                    {
                        counts.Add(ring.Count);
                        allPoints.AddRange(ring);
                    }
                    ringPointCounts.Add(counts);
                }

                List<Wgs84> wgsPoints = await _ehtClient.EovToWgs84BulkAsync(allPoints);
                await townsTask;

                int cursor = 0;
                var newParcels = new List<ParcelEntry>();
                for (int i = 0; i < rawParcels.Count; i++)
                {
                    var localRings = new List<List<vec2>>();
                    foreach (int count in ringPointCounts[i])
                    {
                        var localRing = new List<vec2>(count);
                        for (int p = 0; p < count; p++)
                        {
                            GeoCoord geo = mf.AppModel.LocalPlane.ConvertWgs84ToGeoCoord(wgsPoints[cursor]);
                            localRing.Add(new vec2(geo.Easting, geo.Northing));
                            cursor++;
                        }
                        localRings.Add(localRing);
                    }

                    newParcels.Add(new ParcelEntry
                    {
                        Hrsz = rawParcels[i].Hrsz,
                        TownName = KshLookupClient.GetTownName(rawParcels[i].KshKod),
                        LocalRings = localRings,
                        AreaHa = CalculatePolygonAreaHa(localRings.FirstOrDefault())
                    });
                }

                // Natural sort: town name, then HRSZ with numeric groups padded so "2" sorts before "10".
                _parcels = newParcels
                    .OrderBy(p => p.TownName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(p => NaturalKey(p.Hrsz), StringComparer.Ordinal)
                    .ToList();

                foreach (var parcel in _parcels)
                {
                    clbParcels.Items.Add($"{parcel.DisplayName}   ({parcel.AreaHa:0.00} ha)");
                }

                ResetView();

                lblStatus.Text = $"Found {_parcels.Count} parcel(s). Check the ones to import (or tap them on the map).";
                btnAddSelected.Enabled = _parcels.Count > 0;
                pnlPreview.Invalidate();
            }
            catch (Exception ex)
            {
                Log.EventWriter("OENY import error: " + ex);
                lblStatus.Text = "Error: " + ex.Message;
                FormDialog.Show("OENY Import", "Failed to fetch parcels: " + ex.Message, DialogSeverity.Error);
            }
            finally
            {
                btnSearch.Enabled = true;
            }
        }

        private static string NaturalKey(string s)
        {
            return System.Text.RegularExpressions.Regex.Replace(s ?? string.Empty, "\\d+", m => m.Value.PadLeft(10, '0'));
        }

        private static double CalculatePolygonAreaHa(List<vec2> ring)
        {
            if (ring == null || ring.Count < 3) return 0;

            double area = 0;
            int j = ring.Count - 1;
            for (int i = 0; i < ring.Count; j = i++)
            {
                area += (ring[j].easting + ring[i].easting) * (ring[j].northing - ring[i].northing);
            }
            return Math.Abs(area / 2.0) * 0.0001;
        }

        #region Pan / zoom / hit-test

        // Recomputes the auto-fit scale/center for the current parcel set and resets pan/zoom.
        private void ResetView()
        {
            _zoom = 1.0;
            _panEasting = 0;
            _panNorthing = 0;

            if (_parcels.Count == 0) return;

            double minE = double.MaxValue, maxE = double.MinValue, minN = double.MaxValue, maxN = double.MinValue;
            foreach (var parcel in _parcels)
            {
                foreach (var ring in parcel.LocalRings)
                {
                    foreach (var pt in ring)
                    {
                        if (pt.easting < minE) minE = pt.easting;
                        if (pt.easting > maxE) maxE = pt.easting;
                        if (pt.northing < minN) minN = pt.northing;
                        if (pt.northing > maxN) maxN = pt.northing;
                    }
                }
            }

            _centerEasting = (minE + maxE) / 2.0;
            _centerNorthing = (minN + maxN) / 2.0;

            double spanE = Math.Max(1, maxE - minE);
            double spanN = Math.Max(1, maxN - minN);
            const double margin = 30;
            _fitScale = Math.Min((pnlPreview.Width - 2 * margin) / spanE, (pnlPreview.Height - 2 * margin) / spanN);
            if (_fitScale <= 0 || double.IsInfinity(_fitScale)) _fitScale = 1.0;
        }

        private double CurrentScale => _fitScale * _zoom;

        private PointF ToScreen(vec2 pt)
        {
            double scale = CurrentScale;
            double worldE = pt.easting - _centerEasting - _panEasting;
            double worldN = pt.northing - _centerNorthing - _panNorthing;
            float x = (float)(pnlPreview.Width / 2.0 + worldE * scale);
            float y = (float)(pnlPreview.Height / 2.0 - worldN * scale); // flip so north is up
            return new PointF(x, y);
        }

        private vec2 ToWorld(Point screenPt)
        {
            double scale = CurrentScale;
            double worldE = (screenPt.X - pnlPreview.Width / 2.0) / scale + _centerEasting + _panEasting;
            double worldN = -(screenPt.Y - pnlPreview.Height / 2.0) / scale + _centerNorthing + _panNorthing;
            return new vec2(worldE, worldN);
        }

        private void ZoomBy(double factor)
        {
            _zoom = Math.Max(0.2, Math.Min(30.0, _zoom * factor));
            pnlPreview.Invalidate();
        }

        // dx/dy are in "screen-fraction" units: -1/0/1 for D-pad taps, or pixels/scale for dragging.
        private void PanBy(double dx, double dy)
        {
            double stepWorld = (pnlPreview.Width / CurrentScale) * 0.2;
            _panEasting -= dx * stepWorld;
            _panNorthing += dy * stepWorld;
            pnlPreview.Invalidate();
        }

        private void PnlPreview_MouseWheel(object sender, MouseEventArgs e)
        {
            ZoomBy(e.Delta > 0 ? 1.15 : 1.0 / 1.15);
        }

        private void PnlPreview_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _isDragging = true;
            _didDrag = false;
            _dragStartMouse = e.Location;
            _dragStartPanEasting = _panEasting;
            _dragStartPanNorthing = _panNorthing;
        }

        private void PnlPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            int dxPixels = e.Location.X - _dragStartMouse.X;
            int dyPixels = e.Location.Y - _dragStartMouse.Y;

            if (!_didDrag && (Math.Abs(dxPixels) > 4 || Math.Abs(dyPixels) > 4))
                _didDrag = true;

            if (!_didDrag) return;

            double scale = CurrentScale;
            _panEasting = _dragStartPanEasting - dxPixels / scale;
            _panNorthing = _dragStartPanNorthing + dyPixels / scale;
            pnlPreview.Invalidate();
        }

        private void PnlPreview_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            bool wasDrag = _didDrag;
            _isDragging = false;
            _didDrag = false;

            if (wasDrag || _parcels.Count == 0) return;

            // Treat as a tap/click: toggle whichever parcel is under the cursor.
            vec2 worldPt = ToWorld(e.Location);
            for (int i = _parcels.Count - 1; i >= 0; i--)
            {
                var outer = _parcels[i].LocalRings.FirstOrDefault();
                if (outer != null && PointInPolygon(worldPt, outer))
                {
                    clbParcels.SetItemChecked(i, !clbParcels.GetItemChecked(i));
                    break;
                }
            }
        }

        private static bool PointInPolygon(vec2 pt, List<vec2> ring)
        {
            bool inside = false;
            int j = ring.Count - 1;
            for (int i = 0; i < ring.Count; j = i++)
            {
                if (((ring[i].northing > pt.northing) != (ring[j].northing > pt.northing)) &&
                    (pt.easting < (ring[j].easting - ring[i].easting) * (pt.northing - ring[i].northing) / (ring[j].northing - ring[i].northing) + ring[i].easting))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        #endregion

        private void PnlPreview_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = 0; i < _parcels.Count; i++)
            {
                bool isChecked = i < clbParcels.Items.Count && clbParcels.GetItemChecked(i);
                Color color = isChecked ? Color.OrangeRed : Color.SteelBlue;

                using (var pen = new Pen(color, isChecked ? 2.5f : 1.5f))
                using (var brush = new SolidBrush(Color.FromArgb(isChecked ? 90 : 40, color)))
                {
                    foreach (var ring in _parcels[i].LocalRings)
                    {
                        if (ring.Count < 3) continue;
                        PointF[] points = ring.Select(ToScreen).ToArray();
                        g.FillPolygon(brush, points);
                        g.DrawPolygon(pen, points);
                    }
                }
            }

            DrawVehicleMarker(g);
        }

        // Draws the tractor's live position + heading as a yellow arrow on top of the parcels.
        private void DrawVehicleMarker(Graphics g)
        {
            if (mf == null) return;

            GeoCoord geo = mf.AppModel.LocalPlane.ConvertWgs84ToGeoCoord(mf.AppModel.CurrentLatLon);
            PointF center = ToScreen(new vec2(geo.Easting, geo.Northing));
            double headingRad = mf.AppModel.FixHeading.AngleInRadians;

            const float size = 12f;
            PointF tip = new PointF(
                center.X + (float)(Math.Sin(headingRad) * size * 1.6),
                center.Y - (float)(Math.Cos(headingRad) * size * 1.6));
            PointF left = new PointF(
                center.X + (float)(Math.Sin(headingRad + 2.5) * size),
                center.Y - (float)(Math.Cos(headingRad + 2.5) * size));
            PointF right = new PointF(
                center.X + (float)(Math.Sin(headingRad - 2.5) * size),
                center.Y - (float)(Math.Cos(headingRad - 2.5) * size));

            using (var brush = new SolidBrush(Color.Gold))
            using (var pen = new Pen(Color.Black, 1.5f))
            {
                PointF[] arrow = { tip, left, right };
                g.FillPolygon(brush, arrow);
                g.DrawPolygon(pen, arrow);
            }
        }

        private void BtnAddSelected_Click(object sender, EventArgs e)
        {
            if (mf == null) return;

            List<int> selectedIndices = clbParcels.CheckedIndices.Cast<int>().ToList();
            if (selectedIndices.Count == 0)
            {
                FormDialog.Show("OENY Import", "Select at least one parcel first.", DialogSeverity.Error);
                return;
            }

            if (mf.bnd.bndList.Count > 0)
            {
                DialogResult result = FormDialog.ShowQuestion("Boundary Exists", "A boundary already exists. Replace it with the selected parcel(s)?");
                if (result == DialogResult.OK)
                {
                    mf.bnd.bndList.Clear();
                }
                // Cancel: keep the existing outer boundary and add the new parcel(s) as extra inner boundary/boundaries.
            }

            var selectedRings = selectedIndices
                .Select(index => _parcels[index].LocalRings.FirstOrDefault())
                .Where(ring => ring != null && ring.Count >= 3)
                .ToList();

            // Adjacent parcels sharing an exact border get merged into one outline instead of overlapping boundaries.
            List<List<vec2>> mergedRings = PolygonUnion.Union(selectedRings);

            foreach (var ring in mergedRings)
            {
                var boundary = new CBoundaryList();
                foreach (vec2 pt in ring)
                {
                    boundary.fenceLine.Add(new vec3(pt.easting, pt.northing, 0));
                }

                boundary.CalculateFenceArea(mf.bnd.bndList.Count);
                boundary.FixFenceLine(mf.bnd.bndList.Count);
                mf.bnd.bndList.Add(boundary);
            }

            mf.fd.UpdateFieldBoundaryGUIAreas();
            mf.CalculateMinMax();
            mf.FileSaveBoundary();
            mf.bnd.BuildTurnLines();
            mf.btnABDraw.Visible = true;

            Log.EventWriter($"OENY Import: added {selectedIndices.Count} parcel(s), merged into {mergedRings.Count} boundary/boundaries");

            Close();
        }
    }
}
