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
    public partial class Add_Books : Form
    {
        public Add_Books()
        {
            InitializeComponent();
        }

        private void Add_Record_Load(object sender, EventArgs e)
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
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return loaded;
        }


        bool LoadItems()
        {
            bool res = false;
            string statement = "";
            
            statement = "SELECT author, title, category, isbn, accfrom, accto, date, source, quantity, publicationYear FROM books WHERE markid='" + markidtxt.Text +
                                   "' AND markno='" + marknotxt.Text + "'";
            
            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                object[] obj = new object[24];

                MySqlDataReader reader = com.ExecuteReader();

                
                while (reader.Read())
                {
                    reader.GetValues(obj);

                    authortxt.Text = obj[0].ToString();
                    titletxt.Text = obj[1].ToString();
                    isbntxt.Text = obj[3].ToString();
                    //accfromtxt.Text = obj[4].ToString();
                    //acctotxt.Text = obj[5].ToString();

                    datetxt.Text = obj[6].ToString();

                    sourcetxt.Text = obj[7].ToString();
                    quantitytxt.Text = obj[8].ToString();
                    PubYear.Text = obj[9].ToString();

                    res = true;

                }
                reader.Close();
                con.Close();

            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return res;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string category = "BOOKS";
                int marknum = 0;
                if (modify)
                {
                    int qty = int.Parse(quantitytxt.Text);

                    accfrom = 1;
                    accto = qty;

                    char[] sep = new char[2];
                    string[] str = new string[2];
                    sep[0] = ' ';
                    str = markidtxt.Text.Split(sep);

                    string statement = "UPDATE books SET author='" + authortxt.Text + "', title='" + titletxt.Text +
                                       "', isbn='" + isbntxt.Text + "', publicationYear='" + PubYear.Text + "', accfrom=" + accfrom +
                                       ", accto=" + accto + ", date='" + datetxt.Text +
                                       "', source='" + sourcetxt.Text + "', quantity=" + int.Parse(quantitytxt.Text) +
                                       ", available=" + int.Parse(quantitytxt.Text) + ", borrowed=" + 0 + ", lost_damaged=" + 0 +
                                       " WHERE markid='" + str[0] + "' AND markno='" + marknotxt.Text + "'";

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
                        authortxt.Enabled = true;
                        titletxt.Enabled = true;
                        markidtxt.Enabled = true;
                        marknotxt.Enabled = true;
                        isbntxt.Enabled = true;
                        datetxt.Enabled = true;
                        sourcetxt.Enabled = true;
                        quantitytxt.Enabled = true;
                        button1.Text = "OK";
                        markidtxt.Focus();
                    }
                    else
                    {
                        int qty = int.Parse(quantitytxt.Text);

                        accfrom = 1;
                        accto = qty;

                        marknum = int.Parse(GenMarkNo());
                        marknotxt.Text = marknum.ToString();

                        char[] sep = new char[2];
                        string[] str = new string[2];
                        sep[0] = ' ';
                        str = markidtxt.Text.Split(sep);

                        string statement = "INSERT INTO books VALUES ('" + authortxt.Text + "','" + titletxt.Text + "','" +
                               str[0] + "','" + marknotxt.Text + "','" + category + "','" + isbntxt.Text + "','" + PubYear.Text + "'," +
                               accfrom + "," + accto + ",'" + datetxt.Text + "','" +
                               sourcetxt.Text + "'," + int.Parse(quantitytxt.Text) + "," + int.Parse(quantitytxt.Text) + "," + 0 + "," + 0 + ");" +
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
                            //MessageBox.Show("accfrom: " + accfrom.ToString() + " __ accto: " + accto.ToString());

                            authortxt.Enabled = false;
                            titletxt.Enabled = false;
                            markidtxt.Enabled = false;
                            marknotxt.Enabled = false;
                            isbntxt.Enabled = false;
                            datetxt.Enabled = false;
                            sourcetxt.Enabled = false;
                            quantitytxt.Enabled = false;
                            button1.Text = "NEW";
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
