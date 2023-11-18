namespace ZK_RFID
{
    partial class Form1
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
            this.gpb_tcp = new System.Windows.Forms.GroupBox();
            this.ipServerIP = new System.Windows.Forms.TextBox();
            this.tb_Port = new System.Windows.Forms.TextBox();
            this.btDisConnectTcp = new System.Windows.Forms.Button();
            this.btConnectTcp = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btStartMactive = new System.Windows.Forms.Button();
            this.timer_RealTime = new System.Windows.Forms.Timer(this.components);
            this.ListBox = new System.Windows.Forms.ListBox();
            this.gpb_tcp.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpb_tcp
            // 
            this.gpb_tcp.Controls.Add(this.ipServerIP);
            this.gpb_tcp.Controls.Add(this.tb_Port);
            this.gpb_tcp.Controls.Add(this.btDisConnectTcp);
            this.gpb_tcp.Controls.Add(this.btConnectTcp);
            this.gpb_tcp.Controls.Add(this.label2);
            this.gpb_tcp.Controls.Add(this.label4);
            this.gpb_tcp.Location = new System.Drawing.Point(12, 12);
            this.gpb_tcp.Name = "gpb_tcp";
            this.gpb_tcp.Size = new System.Drawing.Size(343, 77);
            this.gpb_tcp.TabIndex = 5;
            this.gpb_tcp.TabStop = false;
            this.gpb_tcp.Text = "TCP/IP";
            // 
            // ipServerIP
            // 
            this.ipServerIP.Location = new System.Drawing.Point(96, 19);
            this.ipServerIP.Name = "ipServerIP";
            this.ipServerIP.Size = new System.Drawing.Size(121, 20);
            this.ipServerIP.TabIndex = 23;
            this.ipServerIP.Text = "192.168.0.250";
            // 
            // tb_Port
            // 
            this.tb_Port.Location = new System.Drawing.Point(96, 43);
            this.tb_Port.Name = "tb_Port";
            this.tb_Port.Size = new System.Drawing.Size(121, 20);
            this.tb_Port.TabIndex = 22;
            this.tb_Port.Text = "27011";
            // 
            // btDisConnectTcp
            // 
            this.btDisConnectTcp.Enabled = false;
            this.btDisConnectTcp.Location = new System.Drawing.Point(234, 43);
            this.btDisConnectTcp.Name = "btDisConnectTcp";
            this.btDisConnectTcp.Size = new System.Drawing.Size(90, 25);
            this.btDisConnectTcp.TabIndex = 20;
            this.btDisConnectTcp.Text = "Disconnect";
            this.btDisConnectTcp.UseVisualStyleBackColor = true;
            this.btDisConnectTcp.Click += new System.EventHandler(this.btDisConnectTcp_Click);
            // 
            // btConnectTcp
            // 
            this.btConnectTcp.Location = new System.Drawing.Point(234, 14);
            this.btConnectTcp.Name = "btConnectTcp";
            this.btConnectTcp.Size = new System.Drawing.Size(90, 25);
            this.btConnectTcp.TabIndex = 19;
            this.btConnectTcp.Text = "Connect";
            this.btConnectTcp.UseVisualStyleBackColor = true;
            this.btConnectTcp.Click += new System.EventHandler(this.btConnectTcp_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(51, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "IP:";
            // 
            // btStartMactive
            // 
            this.btStartMactive.Enabled = false;
            this.btStartMactive.Font = new System.Drawing.Font("SimSun", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btStartMactive.ForeColor = System.Drawing.Color.DarkBlue;
            this.btStartMactive.Location = new System.Drawing.Point(12, 95);
            this.btStartMactive.Name = "btStartMactive";
            this.btStartMactive.Size = new System.Drawing.Size(343, 43);
            this.btStartMactive.TabIndex = 6;
            this.btStartMactive.Text = "Start";
            this.btStartMactive.UseVisualStyleBackColor = true;
            this.btStartMactive.Click += new System.EventHandler(this.btStartMactive_Click);
            // 
            // timer_RealTime
            // 
            this.timer_RealTime.Interval = 50;
            this.timer_RealTime.Tick += new System.EventHandler(this.timer_RealTime_Tick);
            // 
            // ListBox
            // 
            this.ListBox.FormattingEnabled = true;
            this.ListBox.Location = new System.Drawing.Point(12, 144);
            this.ListBox.Name = "ListBox";
            this.ListBox.Size = new System.Drawing.Size(671, 472);
            this.ListBox.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 626);
            this.Controls.Add(this.ListBox);
            this.Controls.Add(this.btStartMactive);
            this.Controls.Add(this.gpb_tcp);
            this.Name = "Form1";
            this.Text = "Form1";
            this.gpb_tcp.ResumeLayout(false);
            this.gpb_tcp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpb_tcp;
        private System.Windows.Forms.TextBox tb_Port;
        private System.Windows.Forms.Button btDisConnectTcp;
        private System.Windows.Forms.Button btConnectTcp;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ipServerIP;
        private System.Windows.Forms.Button btStartMactive;
        private System.Windows.Forms.Timer timer_RealTime;
        private System.Windows.Forms.ListBox ListBox;
    }
}

