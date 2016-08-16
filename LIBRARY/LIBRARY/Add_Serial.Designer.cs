namespace LIBRARY
{
    partial class Add_Serial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Add_Serial));
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.titletxt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.paddtxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.aaddtxt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.freqtxt = new System.Windows.Forms.ComboBox();
            this.markidtxt = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.marknotxt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(401, 222);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 37);
            this.button1.TabIndex = 12;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Mark ID";
            // 
            // titletxt
            // 
            this.titletxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.titletxt.Location = new System.Drawing.Point(143, 74);
            this.titletxt.MaxLength = 150;
            this.titletxt.Name = "titletxt";
            this.titletxt.Size = new System.Drawing.Size(333, 20);
            this.titletxt.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Title";
            // 
            // paddtxt
            // 
            this.paddtxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.paddtxt.Location = new System.Drawing.Point(143, 97);
            this.paddtxt.MaxLength = 150;
            this.paddtxt.Multiline = true;
            this.paddtxt.Name = "paddtxt";
            this.paddtxt.Size = new System.Drawing.Size(333, 38);
            this.paddtxt.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Publisher Address";
            // 
            // aaddtxt
            // 
            this.aaddtxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.aaddtxt.Location = new System.Drawing.Point(143, 141);
            this.aaddtxt.MaxLength = 150;
            this.aaddtxt.Multiline = true;
            this.aaddtxt.Name = "aaddtxt";
            this.aaddtxt.Size = new System.Drawing.Size(333, 38);
            this.aaddtxt.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Agent Adress";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 185);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Frequency";
            // 
            // freqtxt
            // 
            this.freqtxt.FormattingEnabled = true;
            this.freqtxt.Items.AddRange(new object[] {
            "DAILY",
            "WEEKLY",
            "MONTHLY",
            "YEARLY",
            "3-MONTH",
            "6-MONTH",
            "IRREGULAR"});
            this.freqtxt.Location = new System.Drawing.Point(143, 185);
            this.freqtxt.Name = "freqtxt";
            this.freqtxt.Size = new System.Drawing.Size(178, 21);
            this.freqtxt.TabIndex = 10;
            // 
            // markidtxt
            // 
            this.markidtxt.FormattingEnabled = true;
            this.markidtxt.Location = new System.Drawing.Point(143, 21);
            this.markidtxt.Name = "markidtxt";
            this.markidtxt.Size = new System.Drawing.Size(135, 21);
            this.markidtxt.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(320, 222);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 37);
            this.button2.TabIndex = 13;
            this.button2.Text = "CANCEL";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // marknotxt
            // 
            this.marknotxt.BackColor = System.Drawing.Color.LightGray;
            this.marknotxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.marknotxt.ForeColor = System.Drawing.Color.Red;
            this.marknotxt.Location = new System.Drawing.Point(142, 48);
            this.marknotxt.MaxLength = 10;
            this.marknotxt.Name = "marknotxt";
            this.marknotxt.ReadOnly = true;
            this.marknotxt.Size = new System.Drawing.Size(136, 21);
            this.marknotxt.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(11, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "Mark No.";
            // 
            // Add_Serial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 271);
            this.Controls.Add(this.marknotxt);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.markidtxt);
            this.Controls.Add(this.freqtxt);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.aaddtxt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.paddtxt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.titletxt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Add_Serial";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add_Serial";
            this.Load += new System.EventHandler(this.Add_Serial_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox titletxt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox paddtxt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox aaddtxt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox freqtxt;
        private System.Windows.Forms.ComboBox markidtxt;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox marknotxt;
        private System.Windows.Forms.Label label6;
        public bool modify = false;
        public string ConnectionString = "", modifyVal="";
        int marknum = 0;
    }
}