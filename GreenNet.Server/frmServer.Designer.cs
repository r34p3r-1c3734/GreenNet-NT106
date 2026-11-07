namespace GreenNet.Server
{
    partial class frmServer
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
            this.components = new System.ComponentModel.Container();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.lblConn = new System.Windows.Forms.Label();
            this.txtConn = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblFp = new System.Windows.Forms.Label();
            this.txtFingerprint = new System.Windows.Forms.TextBox();
            this.lblFpHint = new System.Windows.Forms.Label();
            this.gbStats = new System.Windows.Forms.GroupBox();
            this.lblClients = new System.Windows.Forms.Label();
            this.lblEnergy = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.gbStats.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(16, 18);
            this.lblName.Name = "lblName";
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Tên server";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(100, 14);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(140, 23);
            this.txtName.TabIndex = 1;
            this.txtName.Text = "Server A";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(260, 18);
            this.lblPort.Name = "lblPort";
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Cổng (TCP+UDP)";
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(370, 14);
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(80, 23);
            this.nudPort.TabIndex = 3;
            this.nudPort.Minimum = new decimal(new int[] {1024, 0, 0, 0});
            this.nudPort.Maximum = new decimal(new int[] {65535, 0, 0, 0});
            this.nudPort.Value = new decimal(new int[] {9001, 0, 0, 0});
            // 
            // lblConn
            // 
            this.lblConn.AutoSize = true;
            this.lblConn.Location = new System.Drawing.Point(16, 52);
            this.lblConn.Name = "lblConn";
            this.lblConn.TabIndex = 4;
            this.lblConn.Text = "Chuỗi kết nối SQL";
            // 
            // txtConn
            // 
            this.txtConn.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.txtConn.Location = new System.Drawing.Point(140, 48);
            this.txtConn.Name = "txtConn";
            this.txtConn.Size = new System.Drawing.Size(604, 23);
            this.txtConn.TabIndex = 5;
            this.txtConn.Text = "Server=localhost;Database=GreenNet;Integrated Security=true;TrustServerCertificate=true;Connect Timeout=5";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(16, 86);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 32);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(126, 86);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 32);
            this.btnStop.TabIndex = 7;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Enabled = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.lblStatus.Location = new System.Drawing.Point(240, 94);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(500, 20);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Đã dừng";
            // 
            // lblFp
            // 
            this.lblFp.AutoSize = true;
            this.lblFp.Location = new System.Drawing.Point(16, 134);
            this.lblFp.Name = "lblFp";
            this.lblFp.TabIndex = 9;
            this.lblFp.Text = "Vân tay khóa";
            // 
            // txtFingerprint
            // 
            this.txtFingerprint.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.txtFingerprint.Location = new System.Drawing.Point(110, 130);
            this.txtFingerprint.Name = "txtFingerprint";
            this.txtFingerprint.Size = new System.Drawing.Size(634, 23);
            this.txtFingerprint.TabIndex = 10;
            this.txtFingerprint.ReadOnly = true;
            // 
            // lblFpHint
            // 
            this.lblFpHint.AutoSize = true;
            this.lblFpHint.Location = new System.Drawing.Point(110, 156);
            this.lblFpHint.Name = "lblFpHint";
            this.lblFpHint.TabIndex = 11;
            this.lblFpHint.Text = "Đưa chuỗi này cho người dùng/Load Balancer để đối chiếu khi kết nối lần đầu.";
            this.lblFpHint.ForeColor = System.Drawing.SystemColors.GrayText;
            // 
            // gbStats
            // 
            this.gbStats.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.gbStats.Location = new System.Drawing.Point(16, 180);
            this.gbStats.Name = "gbStats";
            this.gbStats.Size = new System.Drawing.Size(728, 60);
            this.gbStats.TabIndex = 12;
            this.gbStats.Text = "Trạng thái";
            // 
            // lblClients
            // 
            this.lblClients.Location = new System.Drawing.Point(16, 28);
            this.lblClients.Name = "lblClients";
            this.lblClients.Size = new System.Drawing.Size(150, 18);
            this.lblClients.TabIndex = 0;
            this.lblClients.Text = "Client đang kết nối: 0";
            // 
            // lblEnergy
            // 
            this.lblEnergy.Location = new System.Drawing.Point(200, 28);
            this.lblEnergy.Name = "lblEnergy";
            this.lblEnergy.Size = new System.Drawing.Size(240, 18);
            this.lblEnergy.TabIndex = 1;
            this.lblEnergy.Text = "Năng lượng đã ghi nhận: 0 J";
            // 
            // lblScore
            // 
            this.lblScore.Location = new System.Drawing.Point(470, 28);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(240, 18);
            this.lblScore.TabIndex = 2;
            this.lblScore.Text = "Green Score: 100";
            // 
            // rtbLog
            // 
            this.rtbLog.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom);
            this.rtbLog.Location = new System.Drawing.Point(16, 252);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(728, 292);
            this.rtbLog.TabIndex = 13;
            this.rtbLog.ReadOnly = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            this.gbStats.Controls.Add(this.lblClients);
            this.gbStats.Controls.Add(this.lblEnergy);
            this.gbStats.Controls.Add(this.lblScore);
            // 
            // frmServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 560);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.nudPort);
            this.Controls.Add(this.lblConn);
            this.Controls.Add(this.txtConn);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblFp);
            this.Controls.Add(this.txtFingerprint);
            this.Controls.Add(this.lblFpHint);
            this.Controls.Add(this.gbStats);
            this.Controls.Add(this.rtbLog);
            this.Name = "frmServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Green Networking – Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmServer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.gbStats.ResumeLayout(false);
            this.gbStats.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Label lblConn;
        private System.Windows.Forms.TextBox txtConn;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblFp;
        private System.Windows.Forms.TextBox txtFingerprint;
        private System.Windows.Forms.Label lblFpHint;
        private System.Windows.Forms.GroupBox gbStats;
        private System.Windows.Forms.Label lblClients;
        private System.Windows.Forms.Label lblEnergy;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Timer timer1;
    }
}
