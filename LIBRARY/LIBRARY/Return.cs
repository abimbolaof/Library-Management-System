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
    public partial class Return : Form
    {
        public Return()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string statement = "INSERT INTO returned VALUES ('" + borrowIDtxt.Text + "','" + id + "','" + no +
                                "','" + title + "','" + author + "'," + int.Parse(accnotxt.Text) + ",'" + loandatetxt.Text +
                                "','" + returndatetxt.Text + "');" +
                                "UPDATE books SET loaned=loaned-1 , available=available+1 WHERE markid='" + id + "' AND markno='" + no + "';" +
                                "DELETE FROM overdue WHERE markid='" + id + "' AND markno='" + no + "' AND accno=" + int.Parse(accnotxt.Text) +
                                ";UPDATE loaned SET status='returned' WHERE markid='" + id + "' AND markno='" + no + "' AND accno=" + int.Parse(accnotxt.Text);
            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                int affectedRows = com.ExecuteNonQuery();

                this.Owner.Text += ".";
                MessageBox.Show("Item Returned!!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                con.Close();
                this.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Return_Load(object sender, EventArgs e)
        {
        }
    }
}
