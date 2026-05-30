namespace AgOpenGPS
{
    partial class FormTreePlant
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.groupBoxRefLine = new System.Windows.Forms.GroupBox();
            this.btnSelectCurveBk = new System.Windows.Forms.Button();
            this.lblCurveSelected = new System.Windows.Forms.Label();
            this.btnSelectCurve = new System.Windows.Forms.Button();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.lblGridSpacing = new System.Windows.Forms.Label();
            this.nudGridSpacing = new AgOpenGPS.NudlessNumericUpDown();
            this.lblNumLines = new System.Windows.Forms.Label();
            this.nudNumLines = new AgOpenGPS.NudlessNumericUpDown();
            this.btnBuild = new System.Windows.Forms.Button();
            this.btnToggle = new System.Windows.Forms.Button();
            this.btnAngleOutput = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.groupBoxRefLine.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGridSpacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumLines)).BeginInit();
            this.SuspendLayout();
            //
            // groupBoxRefLine
            //
            this.groupBoxRefLine.Controls.Add(this.btnSelectCurveBk);
            this.groupBoxRefLine.Controls.Add(this.lblCurveSelected);
            this.groupBoxRefLine.Controls.Add(this.btnSelectCurve);
            this.groupBoxRefLine.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.groupBoxRefLine.Location = new System.Drawing.Point(12, 8);
            this.groupBoxRefLine.Name = "groupBoxRefLine";
            this.groupBoxRefLine.Size = new System.Drawing.Size(430, 80);
            this.groupBoxRefLine.TabIndex = 0;
            this.groupBoxRefLine.TabStop = false;
            this.groupBoxRefLine.Text = "Reference AB Line";
            //
            // btnSelectCurveBk
            //
            this.btnSelectCurveBk.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.btnSelectCurveBk.Location = new System.Drawing.Point(15, 28);
            this.btnSelectCurveBk.Name = "btnSelectCurveBk";
            this.btnSelectCurveBk.Size = new System.Drawing.Size(70, 40);
            this.btnSelectCurveBk.TabIndex = 0;
            this.btnSelectCurveBk.Text = "\u25C4";
            this.btnSelectCurveBk.UseVisualStyleBackColor = true;
            this.btnSelectCurveBk.Click += new System.EventHandler(this.btnSelectCurveBk_Click);
            //
            // lblCurveSelected
            //
            this.lblCurveSelected.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblCurveSelected.Location = new System.Drawing.Point(100, 28);
            this.lblCurveSelected.Name = "lblCurveSelected";
            this.lblCurveSelected.Size = new System.Drawing.Size(220, 40);
            this.lblCurveSelected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCurveSelected.Text = "*";
            //
            // btnSelectCurve
            //
            this.btnSelectCurve.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.btnSelectCurve.Location = new System.Drawing.Point(335, 28);
            this.btnSelectCurve.Name = "btnSelectCurve";
            this.btnSelectCurve.Size = new System.Drawing.Size(70, 40);
            this.btnSelectCurve.TabIndex = 1;
            this.btnSelectCurve.Text = "\u25BA";
            this.btnSelectCurve.UseVisualStyleBackColor = true;
            this.btnSelectCurve.Click += new System.EventHandler(this.btnSelectCurve_Click);
            //
            // groupBoxSettings
            //
            this.groupBoxSettings.Controls.Add(this.lblGridSpacing);
            this.groupBoxSettings.Controls.Add(this.nudGridSpacing);
            this.groupBoxSettings.Controls.Add(this.lblNumLines);
            this.groupBoxSettings.Controls.Add(this.nudNumLines);
            this.groupBoxSettings.Controls.Add(this.btnBuild);
            this.groupBoxSettings.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 94);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(430, 155);
            this.groupBoxSettings.TabIndex = 1;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            //
            // lblGridSpacing
            //
            this.lblGridSpacing.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblGridSpacing.Location = new System.Drawing.Point(10, 28);
            this.lblGridSpacing.Name = "lblGridSpacing";
            this.lblGridSpacing.Size = new System.Drawing.Size(200, 30);
            this.lblGridSpacing.Text = "Grid Spacing (m)";
            //
            // nudGridSpacing
            //
            this.nudGridSpacing.BackColor = System.Drawing.Color.White;
            this.nudGridSpacing.DecimalPlaces = 1;
            this.nudGridSpacing.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.nudGridSpacing.Increment = new decimal(new int[] { 1, 0, 0, 65536 }); // 0.1
            this.nudGridSpacing.Location = new System.Drawing.Point(260, 28);
            this.nudGridSpacing.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudGridSpacing.Minimum = new decimal(new int[] { 1, 0, 0, 65536 }); // 0.1
            this.nudGridSpacing.Name = "nudGridSpacing";
            this.nudGridSpacing.ReadOnly = true;
            this.nudGridSpacing.Size = new System.Drawing.Size(150, 30);
            this.nudGridSpacing.TabIndex = 0;
            this.nudGridSpacing.Value = new decimal(new int[] { 48, 0, 0, 65536 }); // 4.8
            this.nudGridSpacing.Click += new System.EventHandler(this.nud_Click);
            //
            // lblNumLines
            //
            this.lblNumLines.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblNumLines.Location = new System.Drawing.Point(10, 68);
            this.lblNumLines.Name = "lblNumLines";
            this.lblNumLines.Size = new System.Drawing.Size(200, 30);
            this.lblNumLines.Text = "# Lines / Side";
            //
            // nudNumLines
            //
            this.nudNumLines.BackColor = System.Drawing.Color.White;
            this.nudNumLines.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.nudNumLines.Location = new System.Drawing.Point(260, 68);
            this.nudNumLines.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            this.nudNumLines.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudNumLines.Name = "nudNumLines";
            this.nudNumLines.ReadOnly = true;
            this.nudNumLines.Size = new System.Drawing.Size(150, 30);
            this.nudNumLines.TabIndex = 1;
            this.nudNumLines.Value = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudNumLines.Click += new System.EventHandler(this.nud_Click);
            //
            // btnBuild
            //
            this.btnBuild.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.btnBuild.Location = new System.Drawing.Point(120, 110);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Size = new System.Drawing.Size(180, 36);
            this.btnBuild.TabIndex = 2;
            this.btnBuild.Text = "Build Lines";
            this.btnBuild.UseVisualStyleBackColor = true;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
            //
            // btnToggle
            //
            this.btnToggle.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggle.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnToggle.ForeColor = System.Drawing.Color.Black;
            this.btnToggle.Location = new System.Drawing.Point(12, 260);
            this.btnToggle.Name = "btnToggle";
            this.btnToggle.Size = new System.Drawing.Size(100, 80);
            this.btnToggle.TabIndex = 2;
            this.btnToggle.Text = "Tree Plant\r\nOFF";
            this.btnToggle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnToggle.UseVisualStyleBackColor = false;
            this.btnToggle.Click += new System.EventHandler(this.btnToggle_Click);
            //
            // btnAngleOutput
            //
            this.btnAngleOutput.BackColor = System.Drawing.Color.LightGray;
            this.btnAngleOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAngleOutput.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.btnAngleOutput.Location = new System.Drawing.Point(120, 260);
            this.btnAngleOutput.Name = "btnAngleOutput";
            this.btnAngleOutput.Size = new System.Drawing.Size(160, 80);
            this.btnAngleOutput.TabIndex = 3;
            this.btnAngleOutput.Text = "Angle Output OFF";
            this.btnAngleOutput.UseVisualStyleBackColor = false;
            this.btnAngleOutput.Click += new System.EventHandler(this.btnAngleOutput_Click);
            //
            // bntOK
            //
            this.bntOK.BackColor = System.Drawing.Color.Transparent;
            this.bntOK.FlatAppearance.BorderSize = 0;
            this.bntOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.bntOK.Image = global::AgOpenGPS.Properties.Resources.OK64;
            this.bntOK.Location = new System.Drawing.Point(340, 260);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(100, 80);
            this.bntOK.TabIndex = 4;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            //
            // FormTreePlant
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(454, 352);
            this.ControlBox = false;
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.btnAngleOutput);
            this.Controls.Add(this.btnToggle);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.groupBoxRefLine);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormTreePlant";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tree Planting";
            this.Load += new System.EventHandler(this.FormTreePlant_Load);
            this.groupBoxRefLine.ResumeLayout(false);
            this.groupBoxSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudGridSpacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumLines)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxRefLine;
        private System.Windows.Forms.Button btnSelectCurveBk;
        private System.Windows.Forms.Label lblCurveSelected;
        private System.Windows.Forms.Button btnSelectCurve;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.Label lblGridSpacing;
        private AgOpenGPS.NudlessNumericUpDown nudGridSpacing;
        private System.Windows.Forms.Label lblNumLines;
        private AgOpenGPS.NudlessNumericUpDown nudNumLines;
        private System.Windows.Forms.Button btnBuild;
        private System.Windows.Forms.Button btnToggle;
        private System.Windows.Forms.Button btnAngleOutput;
        private System.Windows.Forms.Button bntOK;
    }
}
