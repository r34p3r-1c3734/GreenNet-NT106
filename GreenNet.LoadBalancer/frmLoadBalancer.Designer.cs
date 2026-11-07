namespace GreenNet.LoadBalancer
{
    partial class frmLoadBalancer
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
            this.lblPort = new System.Windows.Forms.Label();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.lblAlgo = new System.Windows.Forms.Label();
            this.cboAlgo = new System.Windows.Forms.ComboBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.gbAdd = new System.Windows.Forms.GroupBox();
            this.lblSrvName = new System.Windows.Forms.Label();
            this.txtSrvName = new System.Windows.Forms.TextBox();
            this.lblSrvHost = new System.Windows.Forms.Label();
            this.txtSrvHost = new System.Windows.Forms.TextBox();
            this.lblSrvPort = new System.Windows.Forms.Label();
            this.nudSrvPort = new System.Windows.Forms.NumericUpDown();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnForget = new System.Windows.Forms.Button();
            this.dgvServers = new System.Windows.Forms.DataGridView();
            this.chartAssigned = new GreenNet.Core.MiniChart();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSrvPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.gbAdd.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(16, 18);
            this.lblPort.Name = "lblPort";
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "Cổng LB";
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(80, 14);
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(80, 23);
            this.nudPort.TabIndex = 1;
            this.nudPort.Minimum = new decimal(new int[] {1024, 0, 0, 0});
            this.nudPort.Maximum = new decimal(new int[] {65535, 0, 0, 0});
            this.nudPort.Value = new decimal(new int[] {9000, 0, 0, 0});
            // 
            // lblAlgo
            // 
            this.lblAlgo.AutoSize = true;
            this.lblAlgo.Location = new System.Drawing.Point(180, 18);
            this.lblAlgo.Name = "lblAlgo";
            this.lblAlgo.TabIndex = 2;
            this.lblAlgo.Text = "Thuật toán";
            // 
            // cboAlgo
            // 
            this.cboAlgo.Location = new System.Drawing.Point(250, 14);
            this.cboAlgo.Name = "cboAlgo";
            this.cboAlgo.Size = new System.Drawing.Size(180, 23);
            this.cboAlgo.TabIndex = 3;
            this.cboAlgo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlgo.Items.AddRange(new object[] {"Round-robin","Least-connections","Energy-aware"});
            this.cboAlgo.SelectedIndexChanged += new System.EventHandler(this.cboAlgo_SelectedIndexChanged);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(450, 10);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(90, 30);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(548, 10);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(90, 30);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Enabled = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(650, 18);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(190, 20);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Đã dừng";
            // 
            // gbAdd
            // 
            this.gbAdd.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.gbAdd.Location = new System.Drawing.Point(16, 52);
            this.gbAdd.Name = "gbAdd";
            this.gbAdd.Size = new System.Drawing.Size(828, 64);
            this.gbAdd.TabIndex = 7;
            this.gbAdd.Text = "Thêm server";
            // 
            // lblSrvName
            // 
            this.lblSrvName.AutoSize = true;
            this.lblSrvName.Location = new System.Drawing.Point(12, 29);
            this.lblSrvName.Name = "lblSrvName";
            this.lblSrvName.TabIndex = 0;
            this.lblSrvName.Text = "Tên";
            // 
            // txtSrvName
            // 
            this.txtSrvName.Location = new System.Drawing.Point(44, 25);
            this.txtSrvName.Name = "txtSrvName";
            this.txtSrvName.Size = new System.Drawing.Size(110, 23);
            this.txtSrvName.TabIndex = 1;
            this.txtSrvName.Text = "Server A";
            // 
            // lblSrvHost
            // 
            this.lblSrvHost.AutoSize = true;
            this.lblSrvHost.Location = new System.Drawing.Point(170, 29);
            this.lblSrvHost.Name = "lblSrvHost";
            this.lblSrvHost.TabIndex = 2;
            this.lblSrvHost.Text = "IP";
            // 
            // txtSrvHost
            // 
            this.txtSrvHost.Location = new System.Drawing.Point(192, 25);
            this.txtSrvHost.Name = "txtSrvHost";
            this.txtSrvHost.Size = new System.Drawing.Size(140, 23);
            this.txtSrvHost.TabIndex = 3;
            this.txtSrvHost.Text = "127.0.0.1";
            // 
            // lblSrvPort
            // 
            this.lblSrvPort.AutoSize = true;
            this.lblSrvPort.Location = new System.Drawing.Point(348, 29);
            this.lblSrvPort.Name = "lblSrvPort";
            this.lblSrvPort.TabIndex = 4;
            this.lblSrvPort.Text = "Cổng";
            // 
            // nudSrvPort
            // 
            this.nudSrvPort.Location = new System.Drawing.Point(388, 25);
            this.nudSrvPort.Name = "nudSrvPort";
            this.nudSrvPort.Size = new System.Drawing.Size(80, 23);
            this.nudSrvPort.TabIndex = 5;
            this.nudSrvPort.Minimum = new decimal(new int[] {1, 0, 0, 0});
            this.nudSrvPort.Maximum = new decimal(new int[] {65535, 0, 0, 0});
            this.nudSrvPort.Value = new decimal(new int[] {9001, 0, 0, 0});
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(486, 23);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(80, 28);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Thêm";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(576, 23);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(130, 28);
            this.btnRemove.TabIndex = 7;
            this.btnRemove.Text = "Xóa dòng chọn";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnForget
            // 
            this.btnForget.Location = new System.Drawing.Point(714, 23);
            this.btnForget.Name = "btnForget";
            this.btnForget.Size = new System.Drawing.Size(104, 28);
            this.btnForget.TabIndex = 8;
            this.btnForget.Text = "Quên khóa";
            this.btnForget.UseVisualStyleBackColor = true;
            this.btnForget.Click += new System.EventHandler(this.btnForget_Click);
            // 
            // dgvServers
            // 
            this.dgvServers.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.dgvServers.Location = new System.Drawing.Point(16, 126);
            this.dgvServers.Name = "dgvServers";
            this.dgvServers.Size = new System.Drawing.Size(828, 180);
            this.dgvServers.TabIndex = 8;
            this.dgvServers.AllowUserToAddRows = false;
            this.dgvServers.AllowUserToDeleteRows = false;
            this.dgvServers.ReadOnly = true;
            this.dgvServers.RowHeadersVisible = false;
            this.dgvServers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvServers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvServers.BackgroundColor = System.Drawing.SystemColors.Window;
            // 
            // chartAssigned
            // 
            this.chartAssigned.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.chartAssigned.Location = new System.Drawing.Point(16, 316);
            this.chartAssigned.Name = "chartAssigned";
            this.chartAssigned.Size = new System.Drawing.Size(828, 150);
            this.chartAssigned.TabIndex = 9;
            this.chartAssigned.Mode = GreenNet.Core.ChartMode.Bar;
            this.chartAssigned.Title = "Số lần phân phối theo server";
            // 
            // rtbLog
            // 
            this.rtbLog.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom);
            this.rtbLog.Location = new System.Drawing.Point(16, 476);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(828, 150);
            this.rtbLog.TabIndex = 10;
            this.rtbLog.ReadOnly = true;
            // 
            // bindingSource1
            // 
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            this.gbAdd.Controls.Add(this.lblSrvName);
            this.gbAdd.Controls.Add(this.txtSrvName);
            this.gbAdd.Controls.Add(this.lblSrvHost);
            this.gbAdd.Controls.Add(this.txtSrvHost);
            this.gbAdd.Controls.Add(this.lblSrvPort);
            this.gbAdd.Controls.Add(this.nudSrvPort);
            this.gbAdd.Controls.Add(this.btnAdd);
            this.gbAdd.Controls.Add(this.btnRemove);
            this.gbAdd.Controls.Add(this.btnForget);
            // 
            // frmLoadBalancer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 640);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.nudPort);
            this.Controls.Add(this.lblAlgo);
            this.Controls.Add(this.cboAlgo);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.gbAdd);
            this.Controls.Add(this.dgvServers);
            this.Controls.Add(this.chartAssigned);
            this.Controls.Add(this.rtbLog);
            this.Name = "frmLoadBalancer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Green Networking – Load Balancer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLoadBalancer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSrvPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.gbAdd.ResumeLayout(false);
            this.gbAdd.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Label lblAlgo;
        private System.Windows.Forms.ComboBox cboAlgo;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox gbAdd;
        private System.Windows.Forms.Label lblSrvName;
        private System.Windows.Forms.TextBox txtSrvName;
        private System.Windows.Forms.Label lblSrvHost;
        private System.Windows.Forms.TextBox txtSrvHost;
        private System.Windows.Forms.Label lblSrvPort;
        private System.Windows.Forms.NumericUpDown nudSrvPort;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnForget;
        private System.Windows.Forms.DataGridView dgvServers;
        private GreenNet.Core.MiniChart chartAssigned;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Timer timer1;
    }
}
