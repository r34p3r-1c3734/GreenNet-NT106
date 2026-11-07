namespace GreenNet.Client
{
    partial class frmCompare
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
            this.lblFile = new System.Windows.Forms.Label();
            this.cboFile = new System.Windows.Forms.ComboBox();
            this.lblMetric = new System.Windows.Forms.Label();
            this.cboMetric = new System.Windows.Forms.ComboBox();
            this.btnReload = new System.Windows.Forms.Button();
            this.lblBaseline = new System.Windows.Forms.Label();
            this.chart = new GreenNet.Core.MiniChart();
            this.dgv = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(16, 18);
            this.lblFile.Name = "lblFile";
            this.lblFile.TabIndex = 0;
            this.lblFile.Text = "File";
            // 
            // cboFile
            // 
            this.cboFile.Location = new System.Drawing.Point(50, 14);
            this.cboFile.Name = "cboFile";
            this.cboFile.Size = new System.Drawing.Size(250, 23);
            this.cboFile.TabIndex = 1;
            this.cboFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFile.SelectedIndexChanged += new System.EventHandler(this.Filter_Changed);
            // 
            // lblMetric
            // 
            this.lblMetric.AutoSize = true;
            this.lblMetric.Location = new System.Drawing.Point(320, 18);
            this.lblMetric.Name = "lblMetric";
            this.lblMetric.TabIndex = 2;
            this.lblMetric.Text = "Chỉ số";
            // 
            // cboMetric
            // 
            this.cboMetric.Location = new System.Drawing.Point(370, 14);
            this.cboMetric.Name = "cboMetric";
            this.cboMetric.Size = new System.Drawing.Size(220, 23);
            this.cboMetric.TabIndex = 3;
            this.cboMetric.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMetric.Items.AddRange(new object[] {"Năng lượng / MB (J)","Thời gian (ms)","Tỷ lệ byte truyền","Green Score"});
            this.cboMetric.SelectedIndexChanged += new System.EventHandler(this.Filter_Changed);
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(610, 12);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(90, 28);
            this.btnReload.TabIndex = 4;
            this.btnReload.Text = "Tải lại";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // lblBaseline
            // 
            this.lblBaseline.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.lblBaseline.Location = new System.Drawing.Point(16, 50);
            this.lblBaseline.Name = "lblBaseline";
            this.lblBaseline.Size = new System.Drawing.Size(828, 20);
            this.lblBaseline.TabIndex = 5;
            this.lblBaseline.Text = "So sánh các phiên cùng một file để kết quả công bằng.";
            this.lblBaseline.ForeColor = System.Drawing.SystemColors.GrayText;
            // 
            // chart
            // 
            this.chart.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.chart.Location = new System.Drawing.Point(16, 78);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(828, 240);
            this.chart.TabIndex = 6;
            this.chart.Mode = GreenNet.Core.ChartMode.Bar;
            this.chart.Title = "So sánh theo cấu hình";
            // 
            // dgv
            // 
            this.dgv.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom);
            this.dgv.Location = new System.Drawing.Point(16, 328);
            this.dgv.Name = "dgv";
            this.dgv.Size = new System.Drawing.Size(828, 300);
            this.dgv.TabIndex = 7;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.BackgroundColor = System.Drawing.SystemColors.Window;
            // 
            // frmCompare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 640);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.cboFile);
            this.Controls.Add(this.lblMetric);
            this.Controls.Add(this.cboMetric);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.lblBaseline);
            this.Controls.Add(this.chart);
            this.Controls.Add(this.dgv);
            this.Name = "frmCompare";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Green Networking – So sánh cấu hình";
            this.Load += new System.EventHandler(this.frmCompare_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.ComboBox cboFile;
        private System.Windows.Forms.Label lblMetric;
        private System.Windows.Forms.ComboBox cboMetric;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Label lblBaseline;
        private GreenNet.Core.MiniChart chart;
        private System.Windows.Forms.DataGridView dgv;
    }
}
