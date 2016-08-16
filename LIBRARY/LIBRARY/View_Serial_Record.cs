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
    public partial class View_Serial_Record : Form
    {
        public View_Serial_Record()
        {
            InitializeComponent();
        }

        private void View_Serial_Record_Load(object sender, EventArgs e)
        {
            list.Items.Clear();

            string[] str = new string[2];
            char[] sep = new char[1];
            sep[0] = ' ';
            str = marklabel.Text.Split(sep);

            string statement = "SELECT volume, date FROM serials_record WHERE markid='" + str[0] + "' AND markno='" + str[1] + "'";
  
            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                object[] obj = new object[2];

                MySqlDataReader reader = com.ExecuteReader();
                int sn = 0;

                while (reader.Read())
                {
                    reader.GetValues(obj);
                    list.Items.Add((sn + 1).ToString());

                    list.Items[sn].SubItems.Add(obj[0].ToString());
                    list.Items[sn].SubItems.Add(obj[1].ToString());
                    sn++;  
                }
                reader.Close();
                con.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 
        }
    }
}
