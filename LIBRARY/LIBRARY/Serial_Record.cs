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

namespace LIBRARY
{
    public partial class Serial_Record : Form
    {
        public Serial_Record()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                char[] sep = new char[2];
                string[] str = new string[2];
                sep[0] = ' ';
                str = marktxt.Text.Split(sep);

                string statement = "INSERT INTO serials_record VALUES ('" + str[0] + "','" + str[1] + "','" +
                                           volumetxt.Text + "','" + datetxt.Text + "')";

                try
                {
                    MySqlConnection con = new MySqlConnection(ConnectionString);
                    con.Open();

                    MySqlCommand com = new MySqlCommand(statement, con);
                    int affectedRows = com.ExecuteNonQuery();
                    con.Close();
                    this.Owner.Text += ".";

                    //MessageBox.Show(affectedRows.ToString() + " Record(s) Inserted Successfully!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();

                }
                catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            catch { }
        }

        private void Serial_Record_Load(object sender, EventArgs e)
        {
            marktxt.Text = mark;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
