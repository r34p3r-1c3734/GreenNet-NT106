namespace GreenNet.Client
{
    partial class frmLogin
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.gbServer = new System.Windows.Forms.GroupBox();
            this.lblLbHost = new System.Windows.Forms.Label();
            this.txtLbHost = new System.Windows.Forms.TextBox();
            this.lblLbPort = new System.Windows.Forms.Label();
            this.txtLbPort = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.gbServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(24, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "GREEN NETWORKING";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Location = new System.Drawing.Point(26, 50);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Đăng nhập để bắt đầu";
            // 
            // gbServer
            // 
            this.gbServer.Location = new System.Drawing.Point(24, 80);
            this.gbServer.Name = "gbServer";
            this.gbServer.Size = new System.Drawing.Size(372, 70);
            this.gbServer.TabIndex = 2;
            this.gbServer.Text = "Load Balancer";
            // 
            // lblLbHost
            // 
            this.lblLbHost.AutoSize = true;
            this.lblLbHost.Location = new System.Drawing.Point(12, 31);
            this.lblLbHost.Name = "lblLbHost";
            this.lblLbHost.TabIndex = 0;
            this.lblLbHost.Text = "Địa chỉ";
            // 
            // txtLbHost
            // 
            this.txtLbHost.Location = new System.Drawing.Point(70, 27);
            this.txtLbHost.Name = "txtLbHost";
            this.txtLbHost.Size = new System.Drawing.Size(140, 23);
            this.txtLbHost.TabIndex = 1;
            this.txtLbHost.Text = "127.0.0.1";
            // 
            // lblLbPort
            // 
            this.lblLbPort.AutoSize = true;
            this.lblLbPort.Location = new System.Drawing.Point(222, 31);
            this.lblLbPort.Name = "lblLbPort";
            this.lblLbPort.TabIndex = 2;
            this.lblLbPort.Text = "Cổng";
            // 
            // txtLbPort
            // 
            this.txtLbPort.Location = new System.Drawing.Point(262, 27);
            this.txtLbPort.Name = "txtLbPort";
            this.txtLbPort.Size = new System.Drawing.Size(90, 23);
            this.txtLbPort.TabIndex = 3;
            this.txtLbPort.Text = "9000";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(24, 172);
            this.lblUser.Name = "lblUser";
            this.lblUser.TabIndex = 3;
            this.lblUser.Text = "Tên đăng nhập";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(140, 168);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(256, 23);
            this.txtUser.TabIndex = 4;
            this.txtUser.MaxLength = 30;
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Location = new System.Drawing.Point(24, 208);
            this.lblPass.Name = "lblPass";
            this.lblPass.TabIndex = 5;
            this.lblPass.Text = "Mật khẩu";
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(140, 204);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(256, 23);
            this.txtPass.TabIndex = 6;
            this.txtPass.UseSystemPasswordChar = true;
            this.txtPass.MaxLength = 40;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(140, 246);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(120, 32);
            this.btnLogin.TabIndex = 7;
            this.btnLogin.Text = "Đăng nhập";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(276, 246);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(120, 32);
            this.btnRegister.TabIndex = 8;
            this.btnRegister.Text = "Đăng ký";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(24, 292);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(372, 24);
            this.lblStatus.TabIndex = 9;
            this.lblStatus.Text = "";
            this.gbServer.Controls.Add(this.lblLbHost);
            this.gbServer.Controls.Add(this.txtLbHost);
            this.gbServer.Controls.Add(this.lblLbPort);
            this.gbServer.Controls.Add(this.txtLbPort);
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 330);
            this.AcceptButton = this.btnLogin;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.gbServer);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.lblPass);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Green Networking – Đăng nhập";
            this.gbServer.ResumeLayout(false);
            this.gbServer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.GroupBox gbServer;
        private System.Windows.Forms.Label lblLbHost;
        private System.Windows.Forms.TextBox txtLbHost;
        private System.Windows.Forms.Label lblLbPort;
        private System.Windows.Forms.TextBox txtLbPort;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Label lblStatus;
    }
}
