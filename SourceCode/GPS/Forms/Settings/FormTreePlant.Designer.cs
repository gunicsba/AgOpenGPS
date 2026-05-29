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
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.lblAngleScale = new System.Windows.Forms.Label();
            this.nudAngleScale = new AgOpenGPS.NudlessNumericUpDown();
            this.lblGridSpacing = new System.Windows.Forms.Label();
            this.nudGridSpacing = new AgOpenGPS.NudlessNumericUpDown();
            this.lblSideExtension = new System.Windows.Forms.Label();
            this.nudSideExtension = new AgOpenGPS.NudlessNumericUpDown();
            this.lblAngle = new System.Windows.Forms.Label();
            this.nudAngle = new AgOpenGPS.NudlessNumericUpDown();
            this.btnToggle = new System.Windows.Forms.Button();
            this.btnAngleOutput = new System.Windows.Forms.Button();
            this.bntOK = new System.Windows.Forms.Button();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAngleScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGridSpacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSideExtension)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAngle)).BeginInit();
            this.SuspendLayout();
            //
            // groupBoxSettings
            //
            this.groupBoxSettings.Controls.Add(this.lblAngleScale);
            this.groupBoxSettings.Controls.Add(this.nudAngleScale);
            this.groupBoxSettings.Controls.Add(this.lblGridSpacing);
            this.groupBoxSettings.Controls.Add(this.nudGridSpacing);
            this.groupBoxSettings.Controls.Add(this.lblSideExtension);
            this.groupBoxSettings.Controls.Add(this.nudSideExtension);
            this.groupBoxSettings.Controls.Add(this.lblAngle);
            this.groupBoxSettings.Controls.Add(this.nudAngle);
            this.groupBoxSettings.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 12);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(430, 240);
            this.groupBoxSettings.TabIndex = 0;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Tree Planting Settings";
            //
            // lblAngleScale
            //
            this.lblAngleScale.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblAngleScale.Location = new System.Drawing.Point(10, 185);
            this.lblAngleScale.Name = "lblAngleScale";
            this.lblAngleScale.Size = new System.Drawing.Size(200, 30);
            this.lblAngleScale.Text = "Angle Scale (deg/m)";
            //
            // nudAngleScale
            //
            this.nudAngleScale.BackColor = System.Drawing.Color.White;
            this.nudAngleScale.DecimalPlaces = 1;
            this.nudAngleScale.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.nudAngleScale.Location = new System.Drawing.Point(260, 185);
            this.nudAngleScale.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudAngleScale.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudAngleScale.Name = "nudAngleScale";
            this.nudAngleScale.ReadOnly = true;
            this.nudAngleScale.Size = new System.Drawing.Size(150, 30);
            this.nudAngleScale.TabIndex = 7;
            this.nudAngleScale.Value = new decimal(new int[] { 20, 0, 0, 0 });
            this.nudAngleScale.Click += new System.EventHandler(this.nud_Click);
            //
            // lblGridSpacing
            //
            this.lblGridSpacing.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblGridSpacing.Location = new System.Drawing.Point(10, 130);
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
            this.nudGridSpacing.Location = new System.Drawing.Point(260, 130);
            this.nudGridSpacing.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudGridSpacing.Minimum = new decimal(new int[] { 1, 0, 0, 65536 }); // 0.1
            this.nudGridSpacing.Name = "nudGridSpacing";
            this.nudGridSpacing.ReadOnly = true;
            this.nudGridSpacing.Size = new System.Drawing.Size(150, 30);
            this.nudGridSpacing.TabIndex = 5;
            this.nudGridSpacing.Value = new decimal(new int[] { 48, 0, 0, 65536 }); // 4.8
            this.nudGridSpacing.Click += new System.EventHandler(this.nud_Click);
            //
            // lblSideExtension
            //
            this.lblSideExtension.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblSideExtension.Location = new System.Drawing.Point(10, 75);
            this.lblSideExtension.Name = "lblSideExtension";
            this.lblSideExtension.Size = new System.Drawing.Size(200, 30);
            this.lblSideExtension.Text = "Side Extension (m)";
            //
            // nudSideExtension
            //
            this.nudSideExtension.BackColor = System.Drawing.Color.White;
            this.nudSideExtension.DecimalPlaces = 1;
            this.nudSideExtension.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.nudSideExtension.Location = new System.Drawing.Point(260, 75);
            this.nudSideExtension.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            this.nudSideExtension.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudSideExtension.Name = "nudSideExtension";
            this.nudSideExtension.ReadOnly = true;
            this.nudSideExtension.Size = new System.Drawing.Size(150, 30);
            this.nudSideExtension.TabIndex = 3;
            this.nudSideExtension.Value = new decimal(new int[] { 50, 0, 0, 0 });
            this.nudSideExtension.Click += new System.EventHandler(this.nud_Click);
            //
            // lblAngle
            //
            this.lblAngle.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblAngle.Location = new System.Drawing.Point(10, 20);
            this.lblAngle.Name = "lblAngle";
            this.lblAngle.Size = new System.Drawing.Size(200, 30);
            this.lblAngle.Text = "Angle from AB (\u00B0)";
            //
            // nudAngle
            //
            this.nudAngle.BackColor = System.Drawing.Color.White;
            this.nudAngle.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.nudAngle.Location = new System.Drawing.Point(260, 20);
            this.nudAngle.Maximum = new decimal(new int[] { 179, 0, 0, 0 });
            this.nudAngle.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudAngle.Name = "nudAngle";
            this.nudAngle.ReadOnly = true;
            this.nudAngle.Size = new System.Drawing.Size(150, 30);
            this.nudAngle.TabIndex = 1;
            this.nudAngle.Value = new decimal(new int[] { 90, 0, 0, 0 });
            this.nudAngle.Click += new System.EventHandler(this.nud_Click);
            //
            // btnToggle
            //
            this.btnToggle.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggle.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnToggle.ForeColor = System.Drawing.Color.Black;
            this.btnToggle.Location = new System.Drawing.Point(12, 270);
            this.btnToggle.Name = "btnToggle";
            this.btnToggle.Size = new System.Drawing.Size(100, 80);
            this.btnToggle.TabIndex = 10;
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
            this.btnAngleOutput.Location = new System.Drawing.Point(120, 270);
            this.btnAngleOutput.Name = "btnAngleOutput";
            this.btnAngleOutput.Size = new System.Drawing.Size(160, 80);
            this.btnAngleOutput.TabIndex = 11;
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
            this.bntOK.Location = new System.Drawing.Point(340, 270);
            this.bntOK.Name = "bntOK";
            this.bntOK.Size = new System.Drawing.Size(100, 80);
            this.bntOK.TabIndex = 12;
            this.bntOK.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bntOK.UseVisualStyleBackColor = false;
            this.bntOK.Click += new System.EventHandler(this.bntOK_Click);
            //
            // FormTreePlant
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(454, 365);
            this.ControlBox = false;
            this.Controls.Add(this.bntOK);
            this.Controls.Add(this.btnAngleOutput);
            this.Controls.Add(this.btnToggle);
            this.Controls.Add(this.groupBoxSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormTreePlant";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tree Planting";
            this.Load += new System.EventHandler(this.FormTreePlant_Load);
            this.groupBoxSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudAngleScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGridSpacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSideExtension)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAngle)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.Label lblAngle;
        private AgOpenGPS.NudlessNumericUpDown nudAngle;
        private System.Windows.Forms.Label lblSideExtension;
        private AgOpenGPS.NudlessNumericUpDown nudSideExtension;
        private System.Windows.Forms.Label lblGridSpacing;
        private AgOpenGPS.NudlessNumericUpDown nudGridSpacing;
        private System.Windows.Forms.Label lblAngleScale;
        private AgOpenGPS.NudlessNumericUpDown nudAngleScale;
        private System.Windows.Forms.Button btnToggle;
        private System.Windows.Forms.Button btnAngleOutput;
        private System.Windows.Forms.Button bntOK;
    }
}
