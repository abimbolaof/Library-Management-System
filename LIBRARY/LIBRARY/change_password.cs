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
    public partial class change_password : Form
    {
        public change_password()
        {
            InitializeComponent();
        }

        private void change_password_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(server + ":" + database + ":" + username);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pass="";
            string statement="";

            if (pass1.Text == pass2.Text)
            {
                pass = pass2.Text;

                ConnectionString = "server=" + server + ";database=" + database + ";username=" + username + ";password=" + oldpass.Text + ";";
                //MessageBox.Show(ConnectionString);

                try
                {
                    MySqlConnection con = new MySqlConnection(ConnectionString);
                    con.Open();

                    statement = "SET PASSWORD FOR '" + username + "'@'%' = PASSWORD('" + pass + "')";
                    //MessageBox.Show(statement);

                    MySqlCommand com = new MySqlCommand(statement, con);
                    int affectedRows = com.ExecuteNonQuery();

                    MessageBox.Show("Password Changed Successfully!\nCHANGES WILL REFLECT THE NEXT TIME YOU START THE APPLICATION!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    con.Close();
                    this.Close();
                }
                catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else
                MessageBox.Show("Passwords Do Not Match!!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
}
