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
    public partial class Loan : Form
    {
        public Loan()
        {
            InitializeComponent();
        }

        bool VerifyBorrowerID()
        {
            int i = 0;
            string statement = "SELECT name FROM registration WHERE id='" + borrowIDtxt.Text + "'";
            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                MySqlDataReader reader = com.ExecuteReader();      

                while (reader.Read())
                {
                    //reader.GetValue(0);
                    i++;
                }
                reader.Close();
                con.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            if (i < 1)
                return false;
            else
                return true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //check if accession number entered is valid
                object[] obj = new object[2];

                string statement = "SELECT accfrom, accto FROM books WHERE markid='" + id + "' AND markno='" + no + "'";
                try
                {
                    MySqlConnection con = new MySqlConnection(ConnectionString);
                    con.Open();

                    MySqlCommand com = new MySqlCommand(statement, con);
                    MySqlDataReader reader = com.ExecuteReader();

                    while (reader.Read())
                    {
                        reader.GetValues(obj);
                    }
                    reader.Close();
                    con.Close();
                }
                catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }


                //if borrowerid valid
                if (VerifyBorrowerID())
                {
                    //if accno valid
                    if ((int.Parse(accnotxt.Text) >= int.Parse(obj[0].ToString())) && (int.Parse(accnotxt.Text) <= int.Parse(obj[1].ToString())))
                    {
                        int count = 0;
                        //check if user has some due items at hand
                        statement = "SELECT markid, markno, loandate, duedate FROM overdue WHERE borrowerid='" + borrowIDtxt.Text + "'";
                        try
                        {
                            MySqlConnection con = new MySqlConnection(ConnectionString);
                            con.Open();

                            MySqlCommand com = new MySqlCommand(statement, con);
                            MySqlDataReader reader = com.ExecuteReader();

                            while (reader.Read())
                            {
                                count++;
                            }
                            reader.Close();
                            con.Close();
                        }
                        catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                        //if true
                        if (count > 0)
                        {
                            MessageBox.Show("User '" + borrowIDtxt.Text + "' has " + count.ToString() + " overdue Item(s). \nCannot Borrow Any more Items until they have been returned!",
                                            "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        //else loan book
                        else
                        {

                            statement = "INSERT INTO loaned VALUES ('" + borrowIDtxt.Text + "','" + id + "','" + no +
                                                "','" + title + "','" + author + "'," + int.Parse(accnotxt.Text) + ",'" + loandatetxt.Text +
                                                "','" + duedatetxt.Text + "', 'loaned');" +
                                                "UPDATE books SET loaned=loaned+1 , available=available-1 WHERE markid='" + id + "' AND markno='" + no + "';";
                            try
                            {
                                MySqlConnection con = new MySqlConnection(ConnectionString);
                                con.Open();

                                MySqlCommand com = new MySqlCommand(statement, con);
                                int affectedRows = com.ExecuteNonQuery();

                                this.Owner.Text += ".";
                                MessageBox.Show("Item Loaned!!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                con.Close();
                                this.Close();
                            }
                            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Accession Number Invalid!!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                    MessageBox.Show("Borrower ID Invalid!!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch { }

       }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        void Loan_Load(object sender, System.EventArgs e)
        {
            //Load No. Of days before due date
            string statement = "SELECT DueDays FROM settings";

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);

                MySqlDataReader reader = com.ExecuteReader();

                reader.Read();
                DueDays = Double.Parse(reader.GetValue(0).ToString());

                reader.Close();
                con.Close();

            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }


            //set dates
            string today = "", day = "", month = "", year = "", duedate="";

            //loan date
            day = DateTime.Now.Day.ToString();
            if (int.Parse(day) < 10)
                day = "0" + day;

            month = DateTime.Now.Month.ToString();
            if (int.Parse(month) < 10)
                month = "0" + month;

            year = DateTime.Now.Year.ToString();

            today = day + "/" + month + "/" + year;
            loandatetxt.Text = today;

            //due date
            day = DateTime.Now.AddDays(DueDays).Day.ToString();
            if (int.Parse(day) < 10)
                day = "0" + day;

            month = DateTime.Now.AddDays(DueDays).Month.ToString();
            if (int.Parse(month) < 10)
                month = "0" + month;

            year = DateTime.Now.AddDays(DueDays).Year.ToString();

            duedate = day + "/" + month + "/" + year;
            duedatetxt.Text = duedate;
        }

    }
}
