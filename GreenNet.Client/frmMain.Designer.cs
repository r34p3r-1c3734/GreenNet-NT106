namespace GreenNet.Client
{
    partial class frmMain
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
            this.lblWelcome = new System.Windows.Forms.Label();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.btnHistory = new System.Windows.Forms.Button();
            this.btnCompare = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.gbSend = new System.Windows.Forms.GroupBox();
            this.lblFile = new System.Windows.Forms.Label();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblProto = new System.Windows.Forms.Label();
            this.cboProto = new System.Windows.Forms.ComboBox();
            this.lblAlgo = new System.Windows.Forms.Label();
            this.cboAlgo = new System.Windows.Forms.ComboBox();
            this.chkEncrypt = new System.Windows.Forms.CheckBox();
            this.chkAuto = new System.Windows.Forms.CheckBox();
            this.lblPower = new System.Windows.Forms.Label();
            this.nudPower = new System.Windows.Forms.NumericUpDown();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.gbResult = new System.Windows.Forms.GroupBox();
            this.lblRtt = new System.Windows.Forms.Label();
            this.lblEnergy = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.lblRatio = new System.Windows.Forms.Label();
            this.lblServer = new System.Windows.Forms.Label();
            this.lblIntegrity = new System.Windows.Forms.Label();
            this.lblAdaptive = new System.Windows.Forms.Label();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.nudPower)).BeginInit();
            this.gbSend.SuspendLayout();
            this.gbResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Location = new System.Drawing.Point(16, 16);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Xin chào";
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            // 
            // btnDashboard
            // 
            this.btnDashboard.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnDashboard.Location = new System.Drawing.Point(486, 10);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(100, 30);
            this.btnDashboard.TabIndex = 1;
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.UseVisualStyleBackColor = true;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // btnHistory
            // 
            this.btnHistory.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnHistory.Location = new System.Drawing.Point(592, 10);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(90, 30);
            this.btnHistory.TabIndex = 2;
            this.btnHistory.Text = "Lịch sử";
            this.btnHistory.UseVisualStyleBackColor = true;
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // btnCompare
            // 
            this.btnCompare.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnCompare.Location = new System.Drawing.Point(688, 10);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(76, 30);
            this.btnCompare.TabIndex = 3;
            this.btnCompare.Text = "So sánh";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnLogout.Location = new System.Drawing.Point(770, 10);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(74, 30);
            this.btnLogout.TabIndex = 4;
            this.btnLogout.Text = "Đăng xuất";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // gbSend
            // 
            this.gbSend.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.gbSend.Location = new System.Drawing.Point(16, 50);
            this.gbSend.Name = "gbSend";
            this.gbSend.Size = new System.Drawing.Size(828, 192);
            this.gbSend.TabIndex = 5;
            this.gbSend.Text = "Cấu hình gửi file";
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(16, 32);
            this.lblFile.Name = "lblFile";
            this.lblFile.TabIndex = 0;
            this.lblFile.Text = "File";
            // 
            // txtFile
            // 
            this.txtFile.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.txtFile.Location = new System.Drawing.Point(110, 28);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(520, 23);
            this.txtFile.TabIndex = 1;
            this.txtFile.ReadOnly = true;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnBrowse.Location = new System.Drawing.Point(642, 26);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(110, 28);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Chọn file…";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblProto
            // 
            this.lblProto.AutoSize = true;
            this.lblProto.Location = new System.Drawing.Point(16, 74);
            this.lblProto.Name = "lblProto";
            this.lblProto.TabIndex = 3;
            this.lblProto.Text = "Giao thức";
            // 
            // cboProto
            // 
            this.cboProto.Location = new System.Drawing.Point(90, 70);
            this.cboProto.Name = "cboProto";
            this.cboProto.Size = new System.Drawing.Size(80, 23);
            this.cboProto.TabIndex = 4;
            this.cboProto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProto.Items.AddRange(new object[] {"TCP","UDP"});
            this.cboProto.SelectedIndexChanged += new System.EventHandler(this.cboProto_SelectedIndexChanged);
            // 
            // lblAlgo
            // 
            this.lblAlgo.AutoSize = true;
            this.lblAlgo.Location = new System.Drawing.Point(190, 74);
            this.lblAlgo.Name = "lblAlgo";
            this.lblAlgo.TabIndex = 5;
            this.lblAlgo.Text = "Thuật toán nén";
            // 
            // cboAlgo
            // 
            this.cboAlgo.Location = new System.Drawing.Point(290, 70);
            this.cboAlgo.Name = "cboAlgo";
            this.cboAlgo.Size = new System.Drawing.Size(120, 23);
            this.cboAlgo.TabIndex = 6;
            this.cboAlgo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlgo.Items.AddRange(new object[] {"None","Gzip","Brotli","Zstd"});
            // 
            // chkEncrypt
            // 
            this.chkEncrypt.Location = new System.Drawing.Point(430, 72);
            this.chkEncrypt.Name = "chkEncrypt";
            this.chkEncrypt.Size = new System.Drawing.Size(200, 19);
            this.chkEncrypt.TabIndex = 7;
            this.chkEncrypt.Text = "Mã hóa AES-256-GCM";
            this.chkEncrypt.UseVisualStyleBackColor = true;
            // 
            // chkAuto
            // 
            this.chkAuto.Location = new System.Drawing.Point(16, 112);
            this.chkAuto.Name = "chkAuto";
            this.chkAuto.Size = new System.Drawing.Size(330, 19);
            this.chkAuto.TabIndex = 8;
            this.chkAuto.Text = "Tự tối ưu (đổi thuật toán nén, giảm tốc)";
            this.chkAuto.UseVisualStyleBackColor = true;
            this.chkAuto.CheckedChanged += new System.EventHandler(this.chkAuto_CheckedChanged);
            // 
            // lblPower
            // 
            this.lblPower.AutoSize = true;
            this.lblPower.Location = new System.Drawing.Point(360, 114);
            this.lblPower.Name = "lblPower";
            this.lblPower.TabIndex = 9;
            this.lblPower.Text = "Ngưỡng công suất (W, 0 = tắt)";
            // 
            // nudPower
            // 
            this.nudPower.Location = new System.Drawing.Point(560, 110);
            this.nudPower.Name = "nudPower";
            this.nudPower.Size = new System.Drawing.Size(70, 23);
            this.nudPower.TabIndex = 10;
            this.nudPower.Enabled = false;
            this.nudPower.Maximum = new decimal(new int[] {500, 0, 0, 0});
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(16, 148);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(120, 32);
            this.btnSend.TabIndex = 11;
            this.btnSend.Text = "Gửi file";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(146, 148);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 32);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Enabled = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.progressBar.Location = new System.Drawing.Point(240, 152);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(512, 24);
            this.progressBar.TabIndex = 13;
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.lblProgress.Location = new System.Drawing.Point(16, 250);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(828, 20);
            this.lblProgress.TabIndex = 6;
            this.lblProgress.Text = "Sẵn sàng";
            // 
            // gbResult
            // 
            this.gbResult.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.gbResult.Location = new System.Drawing.Point(16, 278);
            this.gbResult.Name = "gbResult";
            this.gbResult.Size = new System.Drawing.Size(828, 106);
            this.gbResult.TabIndex = 7;
            this.gbResult.Text = "Kết quả phiên gần nhất";
            // 
            // lblRtt
            // 
            this.lblRtt.Location = new System.Drawing.Point(16, 28);
            this.lblRtt.Name = "lblRtt";
            this.lblRtt.Size = new System.Drawing.Size(200, 18);
            this.lblRtt.TabIndex = 0;
            this.lblRtt.Text = "RTT: --";
            // 
            // lblEnergy
            // 
            this.lblEnergy.Location = new System.Drawing.Point(230, 28);
            this.lblEnergy.Name = "lblEnergy";
            this.lblEnergy.Size = new System.Drawing.Size(240, 18);
            this.lblEnergy.TabIndex = 1;
            this.lblEnergy.Text = "Năng lượng ước tính: --";
            // 
            // lblScore
            // 
            this.lblScore.Location = new System.Drawing.Point(490, 28);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(200, 18);
            this.lblScore.TabIndex = 2;
            this.lblScore.Text = "Green Score: --";
            // 
            // lblRatio
            // 
            this.lblRatio.Location = new System.Drawing.Point(16, 54);
            this.lblRatio.Name = "lblRatio";
            this.lblRatio.Size = new System.Drawing.Size(200, 18);
            this.lblRatio.TabIndex = 3;
            this.lblRatio.Text = "Tỷ lệ byte truyền: --";
            // 
            // lblServer
            // 
            this.lblServer.Location = new System.Drawing.Point(230, 54);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(240, 18);
            this.lblServer.TabIndex = 4;
            this.lblServer.Text = "Server: --";
            // 
            // lblIntegrity
            // 
            this.lblIntegrity.Location = new System.Drawing.Point(490, 54);
            this.lblIntegrity.Name = "lblIntegrity";
            this.lblIntegrity.Size = new System.Drawing.Size(300, 18);
            this.lblIntegrity.TabIndex = 5;
            this.lblIntegrity.Text = "Toàn vẹn (SHA-256): --";
            // 
            // lblAdaptive
            // 
            this.lblAdaptive.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.lblAdaptive.Location = new System.Drawing.Point(16, 80);
            this.lblAdaptive.Name = "lblAdaptive";
            this.lblAdaptive.Size = new System.Drawing.Size(790, 18);
            this.lblAdaptive.TabIndex = 6;
            this.lblAdaptive.Text = "Tự tối ưu: --";
            // 
            // rtbLog
            // 
            this.rtbLog.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom);
            this.rtbLog.Location = new System.Drawing.Point(16, 396);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(828, 188);
            this.rtbLog.TabIndex = 8;
            this.rtbLog.ReadOnly = true;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Title = "Chọn file cần gửi";
            this.gbSend.Controls.Add(this.lblFile);
            this.gbSend.Controls.Add(this.txtFile);
            this.gbSend.Controls.Add(this.btnBrowse);
            this.gbSend.Controls.Add(this.lblProto);
            this.gbSend.Controls.Add(this.cboProto);
            this.gbSend.Controls.Add(this.lblAlgo);
            this.gbSend.Controls.Add(this.cboAlgo);
            this.gbSend.Controls.Add(this.chkEncrypt);
            this.gbSend.Controls.Add(this.chkAuto);
            this.gbSend.Controls.Add(this.lblPower);
            this.gbSend.Controls.Add(this.nudPower);
            this.gbSend.Controls.Add(this.btnSend);
            this.gbSend.Controls.Add(this.btnCancel);
            this.gbSend.Controls.Add(this.progressBar);
            this.gbResult.Controls.Add(this.lblRtt);
            this.gbResult.Controls.Add(this.lblEnergy);
            this.gbResult.Controls.Add(this.lblScore);
            this.gbResult.Controls.Add(this.lblRatio);
            this.gbResult.Controls.Add(this.lblServer);
            this.gbResult.Controls.Add(this.lblIntegrity);
            this.gbResult.Controls.Add(this.lblAdaptive);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 600);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.btnDashboard);
            this.Controls.Add(this.btnHistory);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.gbSend);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.gbResult);
            this.Controls.Add(this.rtbLog);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Green Networking – Gửi file";
            this.MinimumSize = new System.Drawing.Size(820, 520);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.nudPower)).EndInit();
            this.gbSend.ResumeLayout(false);
            this.gbSend.PerformLayout();
            this.gbResult.ResumeLayout(false);
            this.gbResult.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.GroupBox gbSend;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblProto;
        private System.Windows.Forms.ComboBox cboProto;
        private System.Windows.Forms.Label lblAlgo;
        private System.Windows.Forms.ComboBox cboAlgo;
        private System.Windows.Forms.CheckBox chkEncrypt;
        private System.Windows.Forms.CheckBox chkAuto;
        private System.Windows.Forms.Label lblPower;
        private System.Windows.Forms.NumericUpDown nudPower;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.GroupBox gbResult;
        private System.Windows.Forms.Label lblRtt;
        private System.Windows.Forms.Label lblEnergy;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Label lblRatio;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Label lblIntegrity;
        private System.Windows.Forms.Label lblAdaptive;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}
