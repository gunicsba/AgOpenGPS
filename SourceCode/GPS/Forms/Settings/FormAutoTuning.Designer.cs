namespace AgOpenGPS
{
    partial class FormAutoTuning
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
            this.groupBoxStatus = new System.Windows.Forms.GroupBox();
            this.lblAutoTuningStatus = new System.Windows.Forms.Label();
            this.lblLearningStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkAutoTuningEnabled = new System.Windows.Forms.CheckBox();
            this.groupBoxPerformance = new System.Windows.Forms.GroupBox();
            this.lblCrossTrackError = new System.Windows.Forms.Label();
            this.lblOscillation = new System.Windows.Forms.Label();
            this.lblOvershoot = new System.Windows.Forms.Label();
            this.lblPerformanceScore = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.progressPerformance = new System.Windows.Forms.ProgressBar();
            this.groupBoxCurrentParams = new System.Windows.Forms.GroupBox();
            this.lblCurrentProportional = new System.Windows.Forms.Label();
            this.lblCurrentMaxLimit = new System.Windows.Forms.Label();
            this.lblCurrentMinMove = new System.Windows.Forms.Label();
            this.lblCurrentIntegral = new System.Windows.Forms.Label();
            this.lblCurrentSpeedFactor = new System.Windows.Forms.Label();
            this.lblCurrentAcquireFactor = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.nudLearningRate = new System.Windows.Forms.NumericUpDown();
            this.nudAdaptationSpeed = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.btnStartLearning = new System.Windows.Forms.Button();
            this.btnResetToDefaults = new System.Windows.Forms.Button();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.btnLoadConfig = new System.Windows.Forms.Button();
            this.groupBoxStatus.SuspendLayout();
            this.groupBoxPerformance.SuspendLayout();
            this.groupBoxCurrentParams.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLearningRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAdaptationSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxStatus
            // 
            this.groupBoxStatus.Controls.Add(this.lblAutoTuningStatus);
            this.groupBoxStatus.Controls.Add(this.lblLearningStatus);
            this.groupBoxStatus.Controls.Add(this.label1);
            this.groupBoxStatus.Controls.Add(this.label2);
            this.groupBoxStatus.Controls.Add(this.chkAutoTuningEnabled);
            this.groupBoxStatus.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBoxStatus.Location = new System.Drawing.Point(12, 12);
            this.groupBoxStatus.Name = "groupBoxStatus";
            this.groupBoxStatus.Size = new System.Drawing.Size(460, 100);
            this.groupBoxStatus.TabIndex = 0;
            this.groupBoxStatus.TabStop = false;
            this.groupBoxStatus.Text = "Status";
            // 
            // lblAutoTuningStatus
            // 
            this.lblAutoTuningStatus.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblAutoTuningStatus.Location = new System.Drawing.Point(120, 45);
            this.lblAutoTuningStatus.Name = "lblAutoTuningStatus";
            this.lblAutoTuningStatus.Size = new System.Drawing.Size(100, 15);
            this.lblAutoTuningStatus.TabIndex = 1;
            this.lblAutoTuningStatus.Text = "DISABLED";
            // 
            // lblLearningStatus
            // 
            this.lblLearningStatus.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblLearningStatus.Location = new System.Drawing.Point(120, 70);
            this.lblLearningStatus.Name = "lblLearningStatus";
            this.lblLearningStatus.Size = new System.Drawing.Size(100, 15);
            this.lblLearningStatus.TabIndex = 3;
            this.lblLearningStatus.Text = "WAITING";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label1.Location = new System.Drawing.Point(15, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Auto-Tuning:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label2.Location = new System.Drawing.Point(15, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Learning:";
            // 
            // chkAutoTuningEnabled
            // 
            this.chkAutoTuningEnabled.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.chkAutoTuningEnabled.Location = new System.Drawing.Point(15, 20);
            this.chkAutoTuningEnabled.Name = "chkAutoTuningEnabled";
            this.chkAutoTuningEnabled.Size = new System.Drawing.Size(200, 20);
            this.chkAutoTuningEnabled.TabIndex = 4;
            this.chkAutoTuningEnabled.Text = "Enable Auto-Tuning";
            this.chkAutoTuningEnabled.UseVisualStyleBackColor = true;
            this.chkAutoTuningEnabled.CheckedChanged += new System.EventHandler(this.chkAutoTuningEnabled_CheckedChanged);
            // 
            // groupBoxPerformance
            // 
            this.groupBoxPerformance.Controls.Add(this.lblCrossTrackError);
            this.groupBoxPerformance.Controls.Add(this.lblOscillation);
            this.groupBoxPerformance.Controls.Add(this.lblOvershoot);
            this.groupBoxPerformance.Controls.Add(this.lblPerformanceScore);
            this.groupBoxPerformance.Controls.Add(this.label3);
            this.groupBoxPerformance.Controls.Add(this.label4);
            this.groupBoxPerformance.Controls.Add(this.label5);
            this.groupBoxPerformance.Controls.Add(this.label6);
            this.groupBoxPerformance.Controls.Add(this.progressPerformance);
            this.groupBoxPerformance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBoxPerformance.Location = new System.Drawing.Point(12, 120);
            this.groupBoxPerformance.Name = "groupBoxPerformance";
            this.groupBoxPerformance.Size = new System.Drawing.Size(460, 140);
            this.groupBoxPerformance.TabIndex = 1;
            this.groupBoxPerformance.TabStop = false;
            this.groupBoxPerformance.Text = "Performance Metrics";
            // 
            // lblCrossTrackError
            // 
            this.lblCrossTrackError.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCrossTrackError.Location = new System.Drawing.Point(150, 25);
            this.lblCrossTrackError.Name = "lblCrossTrackError";
            this.lblCrossTrackError.Size = new System.Drawing.Size(80, 15);
            this.lblCrossTrackError.TabIndex = 1;
            this.lblCrossTrackError.Text = "N/A";
            // 
            // lblOscillation
            // 
            this.lblOscillation.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblOscillation.Location = new System.Drawing.Point(150, 50);
            this.lblOscillation.Name = "lblOscillation";
            this.lblOscillation.Size = new System.Drawing.Size(80, 15);
            this.lblOscillation.TabIndex = 3;
            this.lblOscillation.Text = "N/A";
            // 
            // lblOvershoot
            // 
            this.lblOvershoot.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblOvershoot.Location = new System.Drawing.Point(350, 25);
            this.lblOvershoot.Name = "lblOvershoot";
            this.lblOvershoot.Size = new System.Drawing.Size(80, 15);
            this.lblOvershoot.TabIndex = 5;
            this.lblOvershoot.Text = "N/A";
            // 
            // lblPerformanceScore
            // 
            this.lblPerformanceScore.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPerformanceScore.Location = new System.Drawing.Point(350, 50);
            this.lblPerformanceScore.Name = "lblPerformanceScore";
            this.lblPerformanceScore.Size = new System.Drawing.Size(80, 15);
            this.lblPerformanceScore.TabIndex = 7;
            this.lblPerformanceScore.Text = "N/A";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label3.Location = new System.Drawing.Point(15, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Cross-track Error:";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label4.Location = new System.Drawing.Point(15, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(130, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Oscillation:";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label5.Location = new System.Drawing.Point(250, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Overshoot:";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label6.Location = new System.Drawing.Point(250, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "Score:";
            // 
            // progressPerformance
            // 
            this.progressPerformance.Location = new System.Drawing.Point(15, 80);
            this.progressPerformance.Name = "progressPerformance";
            this.progressPerformance.Size = new System.Drawing.Size(430, 23);
            this.progressPerformance.TabIndex = 8;
            // 
            // groupBoxCurrentParams
            // 
            this.groupBoxCurrentParams.Controls.Add(this.lblCurrentProportional);
            this.groupBoxCurrentParams.Controls.Add(this.lblCurrentMaxLimit);
            this.groupBoxCurrentParams.Controls.Add(this.lblCurrentMinMove);
            this.groupBoxCurrentParams.Controls.Add(this.lblCurrentIntegral);
            this.groupBoxCurrentParams.Controls.Add(this.lblCurrentSpeedFactor);
            this.groupBoxCurrentParams.Controls.Add(this.lblCurrentAcquireFactor);
            this.groupBoxCurrentParams.Controls.Add(this.label7);
            this.groupBoxCurrentParams.Controls.Add(this.label8);
            this.groupBoxCurrentParams.Controls.Add(this.label9);
            this.groupBoxCurrentParams.Controls.Add(this.label10);
            this.groupBoxCurrentParams.Controls.Add(this.label11);
            this.groupBoxCurrentParams.Controls.Add(this.label12);
            this.groupBoxCurrentParams.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBoxCurrentParams.Location = new System.Drawing.Point(12, 270);
            this.groupBoxCurrentParams.Name = "groupBoxCurrentParams";
            this.groupBoxCurrentParams.Size = new System.Drawing.Size(460, 120);
            this.groupBoxCurrentParams.TabIndex = 2;
            this.groupBoxCurrentParams.TabStop = false;
            this.groupBoxCurrentParams.Text = "Current Parameters";
            // 
            // lblCurrentProportional
            // 
            this.lblCurrentProportional.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCurrentProportional.Location = new System.Drawing.Point(150, 25);
            this.lblCurrentProportional.Name = "lblCurrentProportional";
            this.lblCurrentProportional.Size = new System.Drawing.Size(80, 15);
            this.lblCurrentProportional.TabIndex = 1;
            this.lblCurrentProportional.Text = "0.0";
            // 
            // lblCurrentMaxLimit
            // 
            this.lblCurrentMaxLimit.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCurrentMaxLimit.Location = new System.Drawing.Point(150, 50);
            this.lblCurrentMaxLimit.Name = "lblCurrentMaxLimit";
            this.lblCurrentMaxLimit.Size = new System.Drawing.Size(80, 15);
            this.lblCurrentMaxLimit.TabIndex = 3;
            this.lblCurrentMaxLimit.Text = "0";
            // 
            // lblCurrentMinMove
            // 
            this.lblCurrentMinMove.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCurrentMinMove.Location = new System.Drawing.Point(150, 75);
            this.lblCurrentMinMove.Name = "lblCurrentMinMove";
            this.lblCurrentMinMove.Size = new System.Drawing.Size(80, 15);
            this.lblCurrentMinMove.TabIndex = 5;
            this.lblCurrentMinMove.Text = "0";
            // 
            // lblCurrentIntegral
            // 
            this.lblCurrentIntegral.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCurrentIntegral.Location = new System.Drawing.Point(370, 25);
            this.lblCurrentIntegral.Name = "lblCurrentIntegral";
            this.lblCurrentIntegral.Size = new System.Drawing.Size(80, 15);
            this.lblCurrentIntegral.TabIndex = 7;
            this.lblCurrentIntegral.Text = "0";
            // 
            // lblCurrentSpeedFactor
            // 
            this.lblCurrentSpeedFactor.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCurrentSpeedFactor.Location = new System.Drawing.Point(370, 50);
            this.lblCurrentSpeedFactor.Name = "lblCurrentSpeedFactor";
            this.lblCurrentSpeedFactor.Size = new System.Drawing.Size(80, 15);
            this.lblCurrentSpeedFactor.TabIndex = 9;
            this.lblCurrentSpeedFactor.Text = "0.00";
            // 
            // lblCurrentAcquireFactor
            // 
            this.lblCurrentAcquireFactor.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCurrentAcquireFactor.Location = new System.Drawing.Point(370, 75);
            this.lblCurrentAcquireFactor.Name = "lblCurrentAcquireFactor";
            this.lblCurrentAcquireFactor.Size = new System.Drawing.Size(80, 15);
            this.lblCurrentAcquireFactor.TabIndex = 11;
            this.lblCurrentAcquireFactor.Text = "0.00";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label7.Location = new System.Drawing.Point(15, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(130, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "Proportional:";
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label8.Location = new System.Drawing.Point(15, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(130, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "Max Limit:";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label9.Location = new System.Drawing.Point(15, 75);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(130, 15);
            this.label9.TabIndex = 4;
            this.label9.Text = "Steer Response:";
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label10.Location = new System.Drawing.Point(250, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(115, 15);
            this.label10.TabIndex = 6;
            this.label10.Text = "Integral:";
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label11.Location = new System.Drawing.Point(250, 50);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(115, 15);
            this.label11.TabIndex = 8;
            this.label11.Text = "Speed Factor:";
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label12.Location = new System.Drawing.Point(250, 75);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(115, 15);
            this.label12.TabIndex = 10;
            this.label12.Text = "Acquire Factor:";
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.nudLearningRate);
            this.groupBoxSettings.Controls.Add(this.nudAdaptationSpeed);
            this.groupBoxSettings.Controls.Add(this.label13);
            this.groupBoxSettings.Controls.Add(this.label14);
            this.groupBoxSettings.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 400);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(460, 80);
            this.groupBoxSettings.TabIndex = 3;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Learning Settings";
            // 
            // nudLearningRate
            // 
            this.nudLearningRate.DecimalPlaces = 1;
            this.nudLearningRate.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.nudLearningRate.Location = new System.Drawing.Point(150, 25);
            this.nudLearningRate.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudLearningRate.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudLearningRate.Name = "nudLearningRate";
            this.nudLearningRate.Size = new System.Drawing.Size(80, 21);
            this.nudLearningRate.TabIndex = 1;
            this.nudLearningRate.Value = new decimal(new int[] { 10, 0, 0, 0 });
            this.nudLearningRate.ValueChanged += new System.EventHandler(this.nudLearningRate_ValueChanged);
            // 
            // nudAdaptationSpeed
            // 
            this.nudAdaptationSpeed.DecimalPlaces = 1;
            this.nudAdaptationSpeed.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.nudAdaptationSpeed.Location = new System.Drawing.Point(370, 25);
            this.nudAdaptationSpeed.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudAdaptationSpeed.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudAdaptationSpeed.Name = "nudAdaptationSpeed";
            this.nudAdaptationSpeed.Size = new System.Drawing.Size(80, 21);
            this.nudAdaptationSpeed.TabIndex = 3;
            this.nudAdaptationSpeed.Value = new decimal(new int[] { 20, 0, 0, 0 });
            this.nudAdaptationSpeed.ValueChanged += new System.EventHandler(this.nudAdaptationSpeed_ValueChanged);
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label13.Location = new System.Drawing.Point(15, 27);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(130, 15);
            this.label13.TabIndex = 0;
            this.label13.Text = "Learning Rate (%):";
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label14.Location = new System.Drawing.Point(250, 27);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(115, 15);
            this.label14.TabIndex = 2;
            this.label14.Text = "Adaptation Speed (%):";
            // 
            // btnStartLearning
            // 
            this.btnStartLearning.BackColor = System.Drawing.Color.LightGreen;
            this.btnStartLearning.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnStartLearning.Location = new System.Drawing.Point(12, 490);
            this.btnStartLearning.Name = "btnStartLearning";
            this.btnStartLearning.Size = new System.Drawing.Size(110, 30);
            this.btnStartLearning.TabIndex = 4;
            this.btnStartLearning.Text = "Start Learning";
            this.btnStartLearning.UseVisualStyleBackColor = false;
            this.btnStartLearning.Click += new System.EventHandler(this.btnStartLearning_Click);
            // 
            // btnResetToDefaults
            // 
            this.btnResetToDefaults.BackColor = System.Drawing.Color.Orange;
            this.btnResetToDefaults.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnResetToDefaults.Location = new System.Drawing.Point(130, 490);
            this.btnResetToDefaults.Name = "btnResetToDefaults";
            this.btnResetToDefaults.Size = new System.Drawing.Size(110, 30);
            this.btnResetToDefaults.TabIndex = 5;
            this.btnResetToDefaults.Text = "Reset Defaults";
            this.btnResetToDefaults.UseVisualStyleBackColor = false;
            this.btnResetToDefaults.Click += new System.EventHandler(this.btnResetToDefaults_Click);
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.BackColor = System.Drawing.Color.LightBlue;
            this.btnSaveConfig.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnSaveConfig.Location = new System.Drawing.Point(248, 490);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(110, 30);
            this.btnSaveConfig.TabIndex = 6;
            this.btnSaveConfig.Text = "Save";
            this.btnSaveConfig.UseVisualStyleBackColor = false;
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.BackColor = System.Drawing.Color.LightYellow;
            this.btnLoadConfig.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnLoadConfig.Location = new System.Drawing.Point(366, 490);
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(110, 30);
            this.btnLoadConfig.TabIndex = 7;
            this.btnLoadConfig.Text = "Load";
            this.btnLoadConfig.UseVisualStyleBackColor = false;
            this.btnLoadConfig.Click += new System.EventHandler(this.btnLoadConfig_Click);
            // 
            // FormAutoTuning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 535);
            this.Controls.Add(this.btnLoadConfig);
            this.Controls.Add(this.btnSaveConfig);
            this.Controls.Add(this.btnResetToDefaults);
            this.Controls.Add(this.btnStartLearning);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.groupBoxCurrentParams);
            this.Controls.Add(this.groupBoxPerformance);
            this.Controls.Add(this.groupBoxStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAutoTuning";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Auto-Tuning Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAutoTuning_FormClosing);
            this.Load += new System.EventHandler(this.FormAutoTuning_Load);
            this.groupBoxStatus.ResumeLayout(false);
            this.groupBoxPerformance.ResumeLayout(false);
            this.groupBoxCurrentParams.ResumeLayout(false);
            this.groupBoxSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudLearningRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAdaptationSpeed)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxStatus;
        private System.Windows.Forms.Label lblAutoTuningStatus;
        private System.Windows.Forms.Label lblLearningStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkAutoTuningEnabled;
        private System.Windows.Forms.GroupBox groupBoxPerformance;
        private System.Windows.Forms.Label lblCrossTrackError;
        private System.Windows.Forms.Label lblOscillation;
        private System.Windows.Forms.Label lblOvershoot;
        private System.Windows.Forms.Label lblPerformanceScore;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ProgressBar progressPerformance;
        private System.Windows.Forms.GroupBox groupBoxCurrentParams;
        private System.Windows.Forms.Label lblCurrentProportional;
        private System.Windows.Forms.Label lblCurrentMaxLimit;
        private System.Windows.Forms.Label lblCurrentMinMove;
        private System.Windows.Forms.Label lblCurrentIntegral;
        private System.Windows.Forms.Label lblCurrentSpeedFactor;
        private System.Windows.Forms.Label lblCurrentAcquireFactor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.NumericUpDown nudLearningRate;
        private System.Windows.Forms.NumericUpDown nudAdaptationSpeed;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnStartLearning;
        private System.Windows.Forms.Button btnResetToDefaults;
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.Button btnLoadConfig;
    }
}