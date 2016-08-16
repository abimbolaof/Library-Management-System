using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;

namespace LIBRARY
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectionString = serverString + ";database=acu_lmis;username=" + usernametxt.Text + ";password=" + passwordtxt.Text + ";";
                MySqlConnection con = new MySqlConnection(ConnectionString);

                con.Open();
                con.Close();

                loginClose = true;
                this.Close();

                Form1 f = new Form1();
                f.ConnectionString = this.ConnectionString;
                f.Show();
            }
            catch (Exception f) { MessageBox.Show("Could not Connect : " + f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); passwordtxt.Text = ""; passwordtxt.Focus(); }
        }


        private void Login_Load(object sender, EventArgs e)
        {
            try
            {
                FileStream f = new FileStream("acu_lmis_config.cfg", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(f);
                serverString = sr.ReadLine();
                sr.Close();
                f.Close();
            }
            catch { }
        }
        

        void Login_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (!loginClose)
                Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            loginClose = true;
            this.Close();
            conSetting cs = new conSetting();
            cs.Show();
        }
    }
}
