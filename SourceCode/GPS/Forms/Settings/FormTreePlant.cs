using AgOpenGPS.Controls;
using AgOpenGPS.Core.Translations;
using System;
using System.Windows.Forms;

namespace AgOpenGPS
{
    public partial class FormTreePlant : Form
    {
        private readonly FormGPS mf = null;

        public FormTreePlant(Form callingForm)
        {
            mf = callingForm as FormGPS;
            InitializeComponent();

            nudAngle.Controls[0].Enabled = false;
            nudSideExtension.Controls[0].Enabled = false;
            nudGridSpacing.Controls[0].Enabled = false;
            nudAngleScale.Controls[0].Enabled = false;
        }

        private void FormTreePlant_Load(object sender, EventArgs e)
        {
            nudAngle.ValueChanged -= nud_ValueChanged;
            nudSideExtension.ValueChanged -= nud_ValueChanged;
            nudGridSpacing.ValueChanged -= nud_ValueChanged;
            nudAngleScale.ValueChanged -= nud_ValueChanged;

            nudAngle.Value = (decimal)Properties.ToolSettings.Default.setTool_treePlantAngle;
            nudSideExtension.Value = (decimal)Properties.ToolSettings.Default.setTool_treePlantSideExtension;
            nudGridSpacing.Value = (decimal)Properties.ToolSettings.Default.setTool_treePlantGridSpacing;
            nudAngleScale.Value = (decimal)Properties.ToolSettings.Default.setTool_treePlantAngleScale;

            nudAngle.ValueChanged += nud_ValueChanged;
            nudSideExtension.ValueChanged += nud_ValueChanged;
            nudGridSpacing.ValueChanged += nud_ValueChanged;
            nudAngleScale.ValueChanged += nud_ValueChanged;

            UpdateToggleButtons();
        }

        private void nud_ValueChanged(object sender, EventArgs e)
        {
            Properties.ToolSettings.Default.setTool_treePlantAngle = (double)nudAngle.Value;
            Properties.ToolSettings.Default.setTool_treePlantSideExtension = (double)nudSideExtension.Value;
            Properties.ToolSettings.Default.setTool_treePlantGridSpacing = (double)nudGridSpacing.Value;
            Properties.ToolSettings.Default.setTool_treePlantAngleScale = (double)nudAngleScale.Value;
            Properties.ToolSettings.Default.Save();
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
                if (mf.trk.idx < 0 || mf.trk.gArr[mf.trk.idx].mode != TrackMode.AB)
                {
                    mf.TimedMessageBox(2000, "Tree Planting", "Requires an active AB Line");
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
                mf.TimedMessageBox(2000, "Angle Output", "Enable Tree Plant first");
                return;
            }

            mf.isTreePlantAngleOutputOn = !mf.isTreePlantAngleOutputOn;

            Properties.ToolSettings.Default.setTool_treePlantAngleOutput = mf.isTreePlantAngleOutputOn;
            Properties.ToolSettings.Default.Save();

            UpdateToggleButtons();
        }

        private void UpdateToggleButtons()
        {
            if (mf.isTreePlantModeOn)
            {
                btnToggle.BackColor = System.Drawing.Color.DarkGreen;
                btnToggle.ForeColor = System.Drawing.Color.White;
                btnToggle.Text = "Tree Plant\r\nON";
            }
            else
            {
                btnToggle.BackColor = System.Drawing.Color.WhiteSmoke;
                btnToggle.ForeColor = System.Drawing.Color.Black;
                btnToggle.Text = "Tree Plant\r\nOFF";
            }

            if (mf.isTreePlantAngleOutputOn)
            {
                btnAngleOutput.BackColor = System.Drawing.Color.LightGreen;
                btnAngleOutput.Text = "Angle Output ON";
            }
            else
            {
                btnAngleOutput.BackColor = System.Drawing.Color.LightGray;
                btnAngleOutput.Text = "Angle Output OFF";
            }
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
