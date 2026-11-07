namespace GreenNet.Client
{
    partial class frmDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblRtt = new System.Windows.Forms.Label();
            this.lblEnergy = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.nudThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblAlert = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblAdaptive = new System.Windows.Forms.Label();
            this.chartThroughput = new GreenNet.Core.MiniChart();
            this.chartCpu = new GreenNet.Core.MiniChart();
            this.chartEnergy = new GreenNet.Core.MiniChart();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // lblRtt
            // 
            this.lblRtt.Location = new System.Drawing.Point(16, 14);
            this.lblRtt.Name = "lblRtt";
            this.lblRtt.Size = new System.Drawing.Size(200, 18);
            this.lblRtt.TabIndex = 0;
            this.lblRtt.Text = "RTT: --";
            // 
            // lblEnergy
            // 
            this.lblEnergy.Location = new System.Drawing.Point(230, 14);
            this.lblEnergy.Name = "lblEnergy";
            this.lblEnergy.Size = new System.Drawing.Size(280, 18);
            this.lblEnergy.TabIndex = 1;
            this.lblEnergy.Text = "Năng lượng ước tính: --";
            // 
            // lblScore
            // 
            this.lblScore.Location = new System.Drawing.Point(530, 14);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(200, 18);
            this.lblScore.TabIndex = 2;
            this.lblScore.Text = "Green Score: --";
            // 
            // lblThreshold
            // 
            this.lblThreshold.AutoSize = true;
            this.lblThreshold.Location = new System.Drawing.Point(16, 48);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.TabIndex = 3;
            this.lblThreshold.Text = "Ngưỡng cảnh báo (J)";
            // 
            // nudThreshold
            // 
            this.nudThreshold.Location = new System.Drawing.Point(150, 44);
            this.nudThreshold.Name = "nudThreshold";
            this.nudThreshold.Size = new System.Drawing.Size(80, 23);
            this.nudThreshold.TabIndex = 4;
            this.nudThreshold.Maximum = new decimal(new int[] {100000, 0, 0, 0});
            this.nudThreshold.Value = new decimal(new int[] {50, 0, 0, 0});
            // 
            // lblAlert
            // 
            this.lblAlert.Location = new System.Drawing.Point(250, 48);
            this.lblAlert.Name = "lblAlert";
            this.lblAlert.Size = new System.Drawing.Size(440, 20);
            this.lblAlert.TabIndex = 5;
            this.lblAlert.Text = "Bình thường";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnClear.Location = new System.Drawing.Point(740, 40);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(104, 30);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Xóa biểu đồ";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblAdaptive
            // 
            this.lblAdaptive.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.lblAdaptive.Location = new System.Drawing.Point(16, 76);
            this.lblAdaptive.Name = "lblAdaptive";
            this.lblAdaptive.Size = new System.Drawing.Size(828, 18);
            this.lblAdaptive.TabIndex = 7;
            this.lblAdaptive.Text = "Chưa có phiên truyền nào.";
            // 
            // chartThroughput
            // 
            this.chartThroughput.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.chartThroughput.Location = new System.Drawing.Point(16, 102);
            this.chartThroughput.Name = "chartThroughput";
            this.chartThroughput.Size = new System.Drawing.Size(828, 148);
            this.chartThroughput.TabIndex = 8;
            this.chartThroughput.Title = "Tốc độ truyền";
            this.chartThroughput.Unit = "KB/s";
            // 
            // chartCpu
            // 
            this.chartCpu.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.chartCpu.Location = new System.Drawing.Point(16, 258);
            this.chartCpu.Name = "chartCpu";
            this.chartCpu.Size = new System.Drawing.Size(828, 148);
            this.chartCpu.TabIndex = 9;
            this.chartCpu.Title = "CPU của tiến trình";
            this.chartCpu.Unit = "%";
            // 
            // chartEnergy
            // 
            this.chartEnergy.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.chartEnergy.Location = new System.Drawing.Point(16, 414);
            this.chartEnergy.Name = "chartEnergy";
            this.chartEnergy.Size = new System.Drawing.Size(828, 190);
            this.chartEnergy.TabIndex = 10;
            this.chartEnergy.Title = "Năng lượng tích lũy ước tính";
            this.chartEnergy.Unit = "J";
            // 
            // frmDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 620);
            this.Controls.Add(this.lblRtt);
            this.Controls.Add(this.lblEnergy);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.lblThreshold);
            this.Controls.Add(this.nudThreshold);
            this.Controls.Add(this.lblAlert);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblAdaptive);
            this.Controls.Add(this.chartThroughput);
            this.Controls.Add(this.chartCpu);
            this.Controls.Add(this.chartEnergy);
            this.Name = "frmDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Green Networking – Dashboard realtime";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmDashboard_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRtt;
        private System.Windows.Forms.Label lblEnergy;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.NumericUpDown nudThreshold;
        private System.Windows.Forms.Label lblAlert;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblAdaptive;
        private GreenNet.Core.MiniChart chartThroughput;
        private GreenNet.Core.MiniChart chartCpu;
        private GreenNet.Core.MiniChart chartEnergy;
    }
}
