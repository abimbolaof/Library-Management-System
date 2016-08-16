using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LIBRARY
{
    public partial class conSetting : Form
    {
        public conSetting()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileStream f = new FileStream("acu_lmis_config.cfg", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(f);
            sw.WriteLine("server=" + textBox1.Text);
            sw.Close();
            f.Close();

            this.Close();
            Login l = new Login();
            l.Show();
           
        }

        private void conSetting_Load(object sender, EventArgs e)
        {
            try
            {
                FileStream f = new FileStream("acu_lmis_config.cfg", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(f);
                string str = sr.ReadLine();

                char[] sep = new char[2];
                string[] str2 = new string[2];
                sep[0] = '=';
                str2 = str.Split(sep);

                textBox1.Text = str2[1];
                sr.Close();
                f.Close();
            }
            catch { }
        }
    }
}
