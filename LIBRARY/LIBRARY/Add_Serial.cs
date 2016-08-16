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
    public partial class Add_Serial : Form
    {
        public Add_Serial()
        {
            InitializeComponent();
        }

        private void Add_Serial_Load(object sender, EventArgs e)
        {
            try
            {
                if (!LoadSub())
                {
                    MessageBox.Show("NO SUBJECTS LOADED ON THE SYSTEM\nPLEASE GO TO SETTINGS TO ADD SUBJECTS.", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }

                if ((modify) && (modifyVal != ""))
                {
                    char[] sep = new char[1];
                    sep[0] = ' ';

                    string[] spl = new string[2];
                    spl = modifyVal.Split(sep);

                    markidtxt.Text = spl[0];
                    marknotxt.Text = spl[1];

                    LoadItems();
                }
                else     
                    markidtxt.SelectedIndex = 0;
            }
            catch { }
        }

        string GenMarkNo()
        {
            string[] str = new string[2];
            char[] sep = new char[2];
            sep[0] = ' ';
            str = markidtxt.Text.Split(sep);

            //MessageBox.Show(str[0]);

            string statement = "SELECT no FROM lastmark WHERE id='" + str[0] + "'";
            string no = "";
            int mkno = 0;

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                object[] obj = new object[2];

                MySqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    reader.GetValues(obj);
                    no = obj[0].ToString();
                }
                reader.Close();
                con.Close();

                //MessageBox.Show(no);

                mkno = int.Parse(no) + 1;

            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            return mkno.ToString();
        }


        bool LoadSub()
        {
            //load subjects
            string statement = "SELECT * FROM subject";
            string id = "";
            bool loaded = false;

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                object[] obj = new object[2];

                MySqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    reader.GetValues(obj);
                    id = obj[0].ToString() + " (" + obj[1].ToString() + ")";
                    markidtxt.Items.Add(id);
                    loaded = true;
                }
                reader.Close();
                con.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message + "load sub", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return loaded;
        }



        bool LoadItems()
        {
            bool res = false;
            string statement = "";

            statement = "SELECT * FROM serials WHERE markid='" + markidtxt.Text +
                                   "' AND markno='" + marknotxt.Text + "'";

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                object[] obj = new object[7];

                MySqlDataReader reader = com.ExecuteReader();


                while (reader.Read())
                {
                    reader.GetValues(obj);

                    markidtxt.Text = obj[0].ToString();
                    marknotxt.Text = obj[1].ToString();
                    titletxt.Text = obj[2].ToString();
                    paddtxt.Text = obj[3].ToString();
                    aaddtxt.Text = obj[4].ToString();
                    freqtxt.Text = obj[5].ToString();
                    res = true;
                }
                reader.Close();
                con.Close();

            }
            catch (Exception f) { MessageBox.Show(f.Message + "load items", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return res;

        }



        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (modify)
                {
                    char[] sep = new char[2];
                    string[] str = new string[2];
                    sep[0] = ' ';
                    str = markidtxt.Text.Split(sep);

                    marknum = int.Parse(GenMarkNo());
                    marknotxt.Text = marknum.ToString();
                    
                    string statement = "UPDATE serials SET markid='" + str[0] + "', markno='" + marknotxt.Text +
                                       "', title='" + titletxt.Text + "', addressOfpublisher='" + paddtxt.Text +
                                       "', addressOfagent='" + aaddtxt.Text + "', frequency='" + freqtxt.Text + "'";

                    try
                    {
                        MySqlConnection con = new MySqlConnection(ConnectionString);
                        con.Open();

                        MySqlCommand com = new MySqlCommand(statement, con);
                        int affectedRows = com.ExecuteNonQuery();
                        con.Close();
                        this.Owner.Text += ".";

                        MessageBox.Show(affectedRows.ToString() + " Record(s) Updated Successfully!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }


                }
                else
                {
                    if (button1.Text == "NEW")
                    {
                        button1.Text = "OK";
                        marknotxt.Enabled = true;
                        markidtxt.Enabled = true;
                        titletxt.Enabled = true;
                        paddtxt.Enabled = true;
                        aaddtxt.Enabled = true;
                        freqtxt.Enabled = true;
                    }
                    else
                    {
                        char[] sep = new char[2];
                        string[] str = new string[2];
                        sep[0] = ' ';
                        str = markidtxt.Text.Split(sep);

                        marknum = int.Parse(GenMarkNo());
                        marknotxt.Text = marknum.ToString();

                        string statement = "INSERT INTO serials VALUES ('" + str[0] + "','" + marknotxt.Text + "','" +
                                           titletxt.Text + "','" + paddtxt.Text + "','" + aaddtxt.Text + "','" + freqtxt.Text + "');" +
                                           "UPDATE lastmark SET no='" + marknum.ToString() + "' WHERE id ='" + str[0] + "';";

                        try
                        {
                            MySqlConnection con = new MySqlConnection(ConnectionString);
                            con.Open();

                            MySqlCommand com = new MySqlCommand(statement, con);
                            int affectedRows = com.ExecuteNonQuery();
                            con.Close();
                            this.Owner.Text += ".";

                            MessageBox.Show("Record Inserted Successfully!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //this.Close();

                            button1.Text = "NEW";
                            marknotxt.Enabled = false;
                            markidtxt.Enabled = false;
                            titletxt.Enabled = false;
                            paddtxt.Enabled = false;
                            aaddtxt.Enabled = false;
                            freqtxt.Enabled = false;

                        }
                        catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                }
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }        
    }
}
