using AgOpenGPS.Controls;
using AgOpenGPS.Core.Translations;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AgOpenGPS
{
    public partial class FormTreePlant : Form
    {
        private readonly FormGPS mf = null;

        // Working copies of visible AB/Curve tracks
        private List<CTrk> gTemp = new List<CTrk>();
        private int indx = -1;

        public FormTreePlant(Form callingForm)
        {
            mf = callingForm as FormGPS;
            InitializeComponent();

            nudGridSpacing.Controls[0].Enabled = false;
            nudNumLines.Controls[0].Enabled = false;

            // Translations
            Text = gStr.gsTreePlanting;
            groupBoxRefLine.Text = gStr.gsTreePlantRefLine;
            groupBoxSettings.Text = gStr.gsTreePlantSettings;
            lblGridSpacing.Text = gStr.gsTreePlantGridSpacing;
            lblNumLines.Text = gStr.gsTreePlantLinesPerSide;
            btnBuild.Text = gStr.gsTreePlantBuildLines;
            groupBoxShift.Text = gStr.gsTreePlantShift;

            // Same pictures as the side shift (nudge) buttons, turned by 90 degrees
            btnShiftFwd.Image = RotatedImage(Properties.Resources.SnapLeft);
            btnShiftBack.Image = RotatedImage(Properties.Resources.SnapRight);
            btnShiftSnap.Image = RotatedImage(Properties.Resources.SnapToPivot);
        }

        private static System.Drawing.Image RotatedImage(System.Drawing.Image source)
        {
            var bmp = new System.Drawing.Bitmap(source);
            bmp.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
            return bmp;
        }

        private void FormTreePlant_Load(object sender, EventArgs e)
        {
            // Populate gTemp with AB and Curve tracks
            gTemp.Clear();
            foreach (var item in mf.trk.gArr)
            {
                if (item.mode == TrackMode.AB || item.mode == TrackMode.Curve)
                {
                    gTemp.Add(new CTrk(item));
                }
            }

            // Restore saved reference index or default to 0
            indx = -1;
            int savedRefIdx = Properties.ToolSettings.Default.setTool_treePlantTramRefIndex;
            if (savedRefIdx >= 0 && gTemp.Count > 0)
            {
                // Find matching track in gTemp by name and mode
                for (int i = 0; i < gTemp.Count; i++)
                {
                    if (mf.trk.gArr.Count > savedRefIdx
                        && gTemp[i].name == mf.trk.gArr[savedRefIdx].name
                        && gTemp[i].mode == mf.trk.gArr[savedRefIdx].mode)
                    {
                        indx = i;
                        break;
                    }
                }
            }
            if (indx < 0 && gTemp.Count > 0) indx = 0;

            // Load NUD values
            nudGridSpacing.ValueChanged -= nud_ValueChanged;
            nudNumLines.ValueChanged -= nud_ValueChanged;

            nudGridSpacing.Value = (decimal)Properties.ToolSettings.Default.setTool_treePlantGridSpacing;
            nudNumLines.Value = Properties.ToolSettings.Default.setTool_treePlantNumLines;

            nudGridSpacing.ValueChanged += nud_ValueChanged;
            nudNumLines.ValueChanged += nud_ValueChanged;

            // Enable/disable controls based on track availability
            bool hasTracks = gTemp.Count > 0;
            btnSelectCurve.Enabled = hasTracks;
            btnSelectCurveBk.Enabled = hasTracks;
            btnBuild.Enabled = hasTracks;

            UpdateTrackLabel();
            UpdateToggleButtons();
            UpdateShiftLabel();
        }

        // Shift step is the same as the side shift (nudge) step, stored in cm
        private double ShiftStep => Properties.ToolSettings.Default.setAS_snapDistance * 0.01;

        private void btnShiftFwd_Click(object sender, EventArgs e)
        {
            if (!CheckLinesBuilt()) return;
            mf.ABLine.ShiftTreePlantLines(ShiftStep);
            UpdateShiftLabel();
        }

        private void btnShiftBack_Click(object sender, EventArgs e)
        {
            if (!CheckLinesBuilt()) return;
            mf.ABLine.ShiftTreePlantLines(-ShiftStep);
            UpdateShiftLabel();
        }

        private void btnShiftSnap_Click(object sender, EventArgs e)
        {
            if (!CheckLinesBuilt()) return;
            mf.ABLine.SnapTreePlantToTool();
            UpdateShiftLabel();
        }

        private void btnShiftZero_Click(object sender, EventArgs e)
        {
            if (!CheckLinesBuilt()) return;
            mf.ABLine.ResetTreePlantShift();
            UpdateShiftLabel();
        }

        private bool CheckLinesBuilt()
        {
            if (mf.ABLine.treePlantLines.Count > 0) return true;
            mf.TimedMessageBox(2000, gStr.gsTreePlanting, gStr.gsTreePlantBuildFirst);
            return false;
        }

        private void UpdateShiftLabel()
        {
            lblShift.Text = ((int)Math.Round(mf.ABLine.TreePlantShift * mf.m2InchOrCm)).ToString() + mf.unitsInCm;
        }

        private void btnSelectCurve_Click(object sender, EventArgs e)
        {
            if (gTemp.Count == 0) return;
            indx++;
            if (indx >= gTemp.Count) indx = 0;
            UpdateTrackLabel();
        }

        private void btnSelectCurveBk_Click(object sender, EventArgs e)
        {
            if (gTemp.Count == 0) return;
            indx--;
            if (indx < 0) indx = gTemp.Count - 1;
            UpdateTrackLabel();
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (indx < 0 || indx >= gTemp.Count)
            {
                mf.TimedMessageBox(2000, gStr.gsTreePlantBuildLines, gStr.gsTreePlantNoRefLine);
                return;
            }

            if (gTemp[indx].mode != TrackMode.AB)
            {
                mf.TimedMessageBox(2000, gStr.gsTreePlantBuildLines, gStr.gsTreePlantOnlyAB);
                return;
            }

            double gridSpacing = (double)nudGridSpacing.Value;
            int numLines = (int)nudNumLines.Value;

            mf.ABLine.BuildTreePlantLines(indx, gridSpacing, numLines, gTemp);

            // Save the reference index
            Properties.ToolSettings.Default.setTool_treePlantTramRefIndex = mf.ABLine.treePlantRefIndex;
            Properties.ToolSettings.Default.setTool_treePlantNumLines = numLines;
            Properties.ToolSettings.Default.Save();

            int lineCount = mf.ABLine.treePlantLines.Count;
            mf.TimedMessageBox(1500, gStr.gsTreePlantBuildLines, string.Format(gStr.gsTreePlantLinesBuilt, lineCount));
            UpdateShiftLabel();
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (mf.isTreePlantModeOn)
            {
                mf.isTreePlantModeOn = false;
                mf.isTreePlantAngleOutputOn = false;
            }
            else
            {
                if (mf.ABLine.treePlantLines.Count == 0)
                {
                    mf.TimedMessageBox(2000, gStr.gsTreePlanting, gStr.gsTreePlantBuildFirst);
                    return;
                }
                mf.isTreePlantModeOn = true;
            }

            Properties.ToolSettings.Default.setTool_isTreePlantMode = mf.isTreePlantModeOn;
            Properties.ToolSettings.Default.setTool_treePlantAngleOutput = mf.isTreePlantAngleOutputOn;
            Properties.ToolSettings.Default.Save();

            UpdateToggleButtons();
        }

        private void btnAngleOutput_Click(object sender, EventArgs e)
        {
            if (!mf.isTreePlantModeOn)
            {
                mf.TimedMessageBox(2000, gStr.gsTreePlantAngleOutput, gStr.gsTreePlantEnableFirst);
                return;
            }

            mf.isTreePlantAngleOutputOn = !mf.isTreePlantAngleOutputOn;

            Properties.ToolSettings.Default.setTool_treePlantAngleOutput = mf.isTreePlantAngleOutputOn;
            Properties.ToolSettings.Default.Save();

            UpdateToggleButtons();
        }

        private void UpdateToggleButtons()
        {
            // Eye = tree planting shown, crossed out eye = hidden
            btnToggle.Image = mf.isTreePlantModeOn
                ? Properties.Resources.TrackVisible
                : Properties.Resources.TracksInvisible;

            if (mf.isTreePlantAngleOutputOn)
            {
                btnAngleOutput.BackColor = System.Drawing.Color.LightGreen;
                btnAngleOutput.Text = gStr.gsTreePlantAngleOutput + " " + gStr.gsOn;
            }
            else
            {
                btnAngleOutput.BackColor = System.Drawing.Color.LightGray;
                btnAngleOutput.Text = gStr.gsTreePlantAngleOutput + " " + gStr.gsOff;
            }
        }

        private void UpdateTrackLabel()
        {
            if (indx > -1 && gTemp.Count > 0)
            {
                lblCurveSelected.Text = (indx + 1).ToString() + " / " + gTemp.Count.ToString()
                    + "\r\n" + gTemp[indx].name;
            }
            else
            {
                lblCurveSelected.Text = "*";
            }
        }

        private void nud_ValueChanged(object sender, EventArgs e)
        {
            Properties.ToolSettings.Default.setTool_treePlantGridSpacing = (double)nudGridSpacing.Value;
            Properties.ToolSettings.Default.setTool_treePlantNumLines = (int)nudNumLines.Value;
            Properties.ToolSettings.Default.Save();
        }

        private void bntOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void nud_Click(object sender, EventArgs e)
        {
            ((NudlessNumericUpDown)sender).ShowKeypad(this);
        }
    }
}
