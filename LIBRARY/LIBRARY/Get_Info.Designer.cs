namespace LIBRARY
{
    partial class Get_Info
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Get_Info));
            this.button1 = new System.Windows.Forms.Button();
            this.picbox = new System.Windows.Forms.Panel();
            this.infotxt = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 20);
            this.button1.TabIndex = 1;
            this.button1.Text = "Show Photo";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // picbox
            // 
            this.picbox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.picbox.Location = new System.Drawing.Point(160, 4);
            this.picbox.Name = "picbox";
            this.picbox.Size = new System.Drawing.Size(170, 170);
            this.picbox.TabIndex = 2;
            this.picbox.Paint += new System.Windows.Forms.PaintEventHandler(this.picbox_Paint);
            // 
            // infotxt
            // 
            this.infotxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infotxt.FormattingEnabled = true;
            this.infotxt.Location = new System.Drawing.Point(12, 26);
            this.infotxt.Name = "infotxt";
            this.infotxt.Size = new System.Drawing.Size(495, 368);
            this.infotxt.TabIndex = 3;
            // 
            // Get_Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 405);
            this.Controls.Add(this.picbox);
            this.Controls.Add(this.infotxt);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Get_Info";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Details";
            this.Load += new System.EventHandler(this.Get_Info_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public string ConnectionString = "", infoStatement="";
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel picbox;
        public bool ssinfo = false;
        bool ShowPicClick = false;
        private System.Windows.Forms.ListBox infotxt;
    }
}