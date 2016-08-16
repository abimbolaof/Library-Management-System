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
using System.Collections;
using System.Collections.Specialized;

namespace LIBRARY
{
    public partial class Form1 : Form
    {
        bool logoutClick = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.Top = Screen.PrimaryScreen.WorkingArea.Top;
            this.Left = Screen.PrimaryScreen.WorkingArea.Left;
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;

            //MessageBox.Show(ConnectionString);

            char[] sep = new char[3];
            sep[0] = ';';
            sep[1] = '=';
            string[] str = new string[10];
            str = ConnectionString.Split(sep);
            useridlabel.Text = str[5];

            //remove all unauthorized menu
            this.Controls.Remove(ManageBooksPanel); ManageBooksButton.Enabled = false; addBookToolStripMenuItem1.Enabled = false;
            this.Controls.Remove(ManageSerialsPanel); ManageSerialsButton.Enabled = false; addSerialToolStripMenuItem1.Enabled = false;
            this.Controls.Remove(LoanedItemsPanel); LoanedItemsButton.Enabled = false;
            this.Controls.Remove(ReturnedItemsPanel); ReturnedItemsButton.Enabled = false;
            this.Controls.Remove(OverdueItemsPanel); OverdueItemsButton.Enabled = false;
            this.Controls.Remove(StaffRegistrationPanel); StaffButton.Enabled = false; toolStripButton28.Enabled = false;
            this.Controls.Remove(StudentRegistrationPanel); StudentsButton.Enabled = false; toolStripButton17.Enabled = false;
            
            toolStripDropDownButton5.Enabled = false;
            settings.Enabled = false;



            //load menu privileges
            LoadMenuPrivileges();

            option4.CheckState = CheckState.Checked;
            bOption8.CheckState = CheckState.Checked;
            rOption8.CheckState = CheckState.Checked;
            soption1.CheckState = CheckState.Checked;
            dOption8.CheckState = CheckState.Checked;

            SearchBooks();
            SearchSerials();
            SearchRegister("student");
            SearchRegister("staff");
            SearchLoaned();
            SearchReturned();
            CheckDue();
            SearchDue();

            timer1.Enabled = true;

            if (!Directory.Exists(Application.StartupPath + "/cache"))
            {
                Directory.CreateDirectory(Application.StartupPath + "/cache");
            }
        }


        void RefreshSystem()
        {
            SearchBooks();
            SearchSerials();
            SearchRegister("student");
            SearchRegister("staff");
            SearchLoaned();
            SearchReturned();
            CheckDue();
            SearchDue();
        }


        void LoadMenuPrivileges()
        {
            string statement = "SELECT menuprivileges FROM users WHERE username='" + useridlabel.Text + "'";

            object[] obj = new object[14];

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                MySqlDataReader reader = com.ExecuteReader();

                string[] str = new string[12];
                char[] sep = new char[2];
                sep[0] = ';';
                int h = 0;

                while (reader.Read())
                {
                    h++;
                    reader.GetValues(obj);
                    str = obj[0].ToString().Split(sep);
                }
                
                reader.Close();
                con.Close();

                if (h < 1)
                {
                    toolStripDropDownButton5.Enabled = true;
                    settings.Enabled = true;
                }


                ListDictionary menuItems = new ListDictionary();

                try
                {

                    for (int i = 0; i <= str.Length - 1; i++)
                    {
                        menuItems.Add(str[i], str[i]);
                    }
                }
                catch { }


                int ct = 0;
                //add menu buttons
                foreach (DictionaryEntry val in menuItems)
                {
                    if (val.Value.ToString() == "BOOKS")
                    {
                        ManageBooksButton.Enabled = true;
                        this.Controls.Add(ManageBooksPanel);
                        addBookToolStripMenuItem1.Enabled = true;
                        ct++;
                        if (ct < 2)
                            selectedMenu = "ManageBooks";
                    }
                    else if (val.Value.ToString() == "SERIALS")
                    {
                        ManageSerialsButton.Enabled = true;
                        this.Controls.Add(ManageSerialsPanel);
                        addSerialToolStripMenuItem1.Enabled = true;
                        ct++;
                        if (ct < 2)
                            selectedMenu = "ManageSerials";
                    }
                    else if (val.Value.ToString() == "LOANED")
                    {
                        LoanedItemsButton.Enabled = true;
                        this.Controls.Add(LoanedItemsPanel);
                        ct++;
                        if (ct < 2)
                            selectedMenu = "LoanedItems";
                    }
                    else if (val.Value.ToString() == "RETURNED")
                    {
                        ReturnedItemsButton.Enabled = true;
                        this.Controls.Add(ReturnedItemsPanel);
                        ct++;
                        if (ct < 2)
                            selectedMenu = "ReturnedItems";
                    }
                    else if (val.Value.ToString() == "OVERDUE")
                    {
                        OverdueItemsButton.Enabled = true;
                        this.Controls.Add(OverdueItemsPanel);
                        ct++;
                        if (ct < 2)
                            selectedMenu = "OverdueItems";
                    }
                    else if (val.Value.ToString() == "STUDENTS")
                    {
                        StudentsButton.Enabled = true;
                        this.Controls.Add(StudentRegistrationPanel);
                        toolStripButton17.Enabled = true;
                        ct++;
                        if (ct < 2)
                            selectedMenu = "StudentRegistration";
                    }
                    else if (val.Value.ToString() == "STAFF")
                    {
                        StaffButton.Enabled = true;
                        this.Controls.Add(StaffRegistrationPanel);
                        toolStripButton28.Enabled = true;
                        ct++;
                        if (ct < 2)
                            selectedMenu = "StaffRegistration";
                    }
                    else if (val.Value.ToString() == "SUPER")
                    {
                        toolStripDropDownButton5.Enabled = true;
                        settings.Enabled = true;
                    }
                }

            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }


        void CheckDue()
        {
            dueList.Items.Clear();
            string today = "", day="", month="", year="";

            day = DateTime.Now.Day.ToString();
            if (int.Parse(day) < 10)
                day = "0" + day;

            month = DateTime.Now.Month.ToString();
            if (int.Parse(month) < 10)
                month = "0" + month;

            year = DateTime.Now.Year.ToString();

            today = day + "/" + month + "/" + year;
            //MessageBox.Show(DateTime.Now.ToShortDateString());

            string statement = "DELETE FROM overdue; INSERT INTO overdue SELECT * FROM Loaned WHERE duedate < '" + today + "' AND status='loaned';";

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                com.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }


        void SearchDue()
        {
            dueList.Items.Clear();

            string statement = "";

            if (DueSearchType == "All")
            {
                statement = "SELECT * FROM overdue WHERE borrowerid LIKE '%" + dueSearchtxt.Text + "%' OR " +
                            "title LIKE '%" + dueSearchtxt.Text + "%' OR " +
                            "markid LIKE '%" + dueSearchtxt.Text + "%' OR " +
                            "markno LIKE '%" + dueSearchtxt.Text + "%' OR " +
                            "loandate LIKE '%" + dueSearchtxt.Text + "%' OR " +
                            "duedate LIKE '%" + dueSearchtxt.Text + "%' OR " +
                            "accno LIKE '%" + dueSearchtxt.Text + "%'";
            }
            else if (DueSearchType == "Author")
            {
                statement = "SELECT * FROM overdue WHERE author LIKE '%" + dueSearchtxt.Text + "%'";
            }
            else if (DueSearchType == "Title")
            {
                statement = "SELECT * FROM overdue WHERE title LIKE '%" + dueSearchtxt.Text + "%'";
            }
            else if (DueSearchType == "BorrowerID")
            {
                statement = "SELECT * FROM overdue WHERE borrowerid LIKE '%" + dueSearchtxt.Text + "%'";
            }
            else if (DueSearchType == "LoanDate")
            {
                statement = "SELECT * FROM overdue WHERE loandate LIKE '%" + dueSearchtxt.Text + "%'";
            }
            else if (DueSearchType == "DueDate")
            {
                statement = "SELECT * FROM overdue WHERE duedate LIKE '%" + dueSearchtxt.Text + "%'";
            }
            else if (DueSearchType == "AccessionNo")
            {
                statement = "SELECT * FROM overdue WHERE accno LIKE '%" + dueSearchtxt.Text + "%'";
            }
            else if (DueSearchType == "Subject")
            {
                statement = "SELECT * FROM overdue WHERE markid LIKE '%" + dueSearchtxt.Text + "%'";
            }

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                MySqlDataReader reader = com.ExecuteReader();

                object[] obj = new object[14];
                int sn = 0;

                while (reader.Read())
                {
                    reader.GetValues(obj);
                    dueList.Items.Add((sn + 1).ToString());

                    if (sn % 2 == 0)
                        dueList.Items[sn].BackColor = Color.Beige;
                    else
                        dueList.Items[sn].BackColor = Color.White;

                    dueList.Items[sn].SubItems.Add(obj[3].ToString());
                    dueList.Items[sn].SubItems.Add(obj[4].ToString());

                    string mark = obj[1].ToString() + " " + obj[2].ToString();

                    dueList.Items[sn].SubItems.Add(mark);

                    dueList.Items[sn].SubItems.Add(obj[5].ToString());

                    dueList.Items[sn].SubItems.Add(obj[0].ToString());
                    dueList.Items[sn].SubItems.Add(obj[6].ToString());
                    dueList.Items[sn].SubItems.Add(obj[7].ToString());

                    sn++;
                }
                reader.Close();
                con.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }


        void SearchReturned()
        {
            returnedList.Items.Clear();

            string statement = "";

            if (ReturnSearchType == "All")
            {
                statement = "SELECT * FROM returned WHERE borrowerid LIKE '%" + returnSearchtxt.Text + "%' OR " +
                            "title LIKE '%" + returnSearchtxt.Text + "%' OR " +
                            "markid LIKE '%" + returnSearchtxt.Text + "%' OR " +
                            "markno LIKE '%" + returnSearchtxt.Text + "%' OR " +
                            "loandate LIKE '%" + returnSearchtxt.Text + "%' OR " +
                            "returndate LIKE '%" + returnSearchtxt.Text + "%' OR " +
                            "accno LIKE '%" + returnSearchtxt.Text + "%'";
            }
            else if (ReturnSearchType == "Author")
            {
                statement = "SELECT * FROM returned WHERE author LIKE '%" + returnSearchtxt.Text + "%'";
            }
            else if (ReturnSearchType == "Title")
            {
                statement = "SELECT * FROM returned WHERE title LIKE '%" + returnSearchtxt.Text + "%'";
            }
            else if (ReturnSearchType == "BorrowerID")
            {
                statement = "SELECT * FROM returned WHERE borrowerid LIKE '%" + returnSearchtxt.Text + "%'";
            }
            else if (ReturnSearchType == "LoanDate")
            {
                statement = "SELECT * FROM returned WHERE loandate LIKE '%" + returnSearchtxt.Text + "%'";
            }
            else if (ReturnSearchType == "ReturnDate")
            {
                statement = "SELECT * FROM returned WHERE returndate LIKE '%" + returnSearchtxt.Text + "%'";
            }
            else if (ReturnSearchType == "AccessionNo")
            {
                statement = "SELECT * FROM returned WHERE accno LIKE '%" + returnSearchtxt.Text + "%'";
            }
            else if (ReturnSearchType == "Subject")
            {
                statement = "SELECT * FROM returned WHERE markid LIKE '%" + returnSearchtxt.Text + "%'";
            }

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                MySqlDataReader reader = com.ExecuteReader();

                object[] obj = new object[14];
                int sn = 0;

                while (reader.Read())
                {
                    reader.GetValues(obj);
                    returnedList.Items.Add((sn + 1).ToString());

                    if (sn % 2 == 0)
                        returnedList.Items[sn].BackColor = Color.Beige;
                    else
                        returnedList.Items[sn].BackColor = Color.White;

                    returnedList.Items[sn].SubItems.Add(obj[3].ToString());
                    returnedList.Items[sn].SubItems.Add(obj[4].ToString());

                    string mark = obj[1].ToString() + " " + obj[2].ToString();

                    returnedList.Items[sn].SubItems.Add(mark);

                    returnedList.Items[sn].SubItems.Add(obj[5].ToString());

                    returnedList.Items[sn].SubItems.Add(obj[0].ToString());
                    returnedList.Items[sn].SubItems.Add(obj[6].ToString());
                    returnedList.Items[sn].SubItems.Add(obj[7].ToString());

                    sn++;
                }
                reader.Close();
                con.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }


        void SearchLoaned()
        {
            loanedList.Items.Clear();

            string statement = "";

            if (LoanSearchType == "All")
            {
                statement = "SELECT * FROM loaned WHERE borrowerid LIKE '%" + loanSearchtxt.Text + "%' OR " +
                            "title LIKE '%" + loanSearchtxt.Text + "%' OR " +
                            "markid LIKE '%" + loanSearchtxt.Text + "%' OR " +
                            "markno LIKE '%" + loanSearchtxt.Text + "%' OR " +
                            "loandate LIKE '%" + loanSearchtxt.Text + "%' OR " +
                            "duedate LIKE '%" + loanSearchtxt.Text + "%' OR " +
                            "accno LIKE '%" + loanSearchtxt.Text + "%'";
            }
            else if (LoanSearchType == "Author")
            {
                statement = "SELECT * FROM loaned WHERE author LIKE '%" + loanSearchtxt.Text + "%'";
            }
            else if (LoanSearchType == "Title")
            {
                statement = "SELECT * FROM loaned WHERE title LIKE '%" + loanSearchtxt.Text + "%'";
            }
            else if (LoanSearchType == "BorrowerID")
            {
                statement = "SELECT * FROM loaned WHERE borrowerid LIKE '%" + loanSearchtxt.Text + "%'";
            }
            else if (LoanSearchType == "LoanDate")
            {
                statement = "SELECT * FROM loaned WHERE loandate LIKE '%" + loanSearchtxt.Text + "%'";
            }
            else if (LoanSearchType == "ReturnDate")
            {
                statement = "SELECT * FROM loaned WHERE duedate LIKE '%" + loanSearchtxt.Text + "%'";
            }
            else if (LoanSearchType == "AccessionNo")
            {
                statement = "SELECT * FROM loaned WHERE accno LIKE '%" + loanSearchtxt.Text + "%'";
            }
            else if (LoanSearchType == "Subject")
            {
                statement = "SELECT * FROM loaned WHERE markid LIKE '%" + loanSearchtxt.Text + "%'";
            }

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                MySqlDataReader reader = com.ExecuteReader();

                object[] obj = new object[14];
                int sn = 0;

                while (reader.Read())
                {
                    reader.GetValues(obj);
                    loanedList.Items.Add((sn + 1).ToString());

                    if (sn % 2 == 0)
                        loanedList.Items[sn].BackColor = Color.Beige;
                    else
                        loanedList.Items[sn].BackColor = Color.White;

                    if (obj[8].ToString() == "returned")
                        loanedList.Items[sn].ForeColor = Color.Red;

                    loanedList.Items[sn].SubItems.Add(obj[3].ToString());
                    loanedList.Items[sn].SubItems.Add(obj[4].ToString());

                    string mark = obj[1].ToString() + " " + obj[2].ToString();

                    loanedList.Items[sn].SubItems.Add(mark);

                    loanedList.Items[sn].SubItems.Add(obj[5].ToString());

                    loanedList.Items[sn].SubItems.Add(obj[0].ToString());
                    loanedList.Items[sn].SubItems.Add(obj[6].ToString());
                    loanedList.Items[sn].SubItems.Add(obj[7].ToString());

                    sn++;
                }
                reader.Close();
                con.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        void SearchRegister(string type)
        {
            try
            {
                string statement = "SELECT id, name, department, faculty, level FROM registration WHERE type='" + type + "'";

                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                MySqlDataReader reader = com.ExecuteReader();

                object[] obj = new object[5];
                int sn = 0;

                if (type == "student")
                {
                    studentList.Items.Clear();

                    while (reader.Read())
                    {
                        reader.GetValues(obj);
                        studentList.Items.Add((sn + 1).ToString());

                        if (sn % 2 == 0)
                            studentList.Items[sn].BackColor = Color.Beige;
                        else
                            studentList.Items[sn].BackColor = Color.White;

                        studentList.Items[sn].SubItems.Add(obj[0].ToString());

                        studentList.Items[sn].SubItems.Add(obj[1].ToString());

                        studentList.Items[sn].SubItems.Add(obj[4].ToString());

                        studentList.Items[sn].SubItems.Add(obj[2].ToString());

                        studentList.Items[sn].SubItems.Add(obj[3].ToString());

                        sn++;
                    }
                }

                else if (type == "staff")
                {
                    staffList.Items.Clear();

                    while (reader.Read())
                    {
                        reader.GetValues(obj);
                        staffList.Items.Add((sn + 1).ToString());

                        if (sn % 2 == 0)
                            staffList.Items[sn].BackColor = Color.Beige;
                        else
                            staffList.Items[sn].BackColor = Color.White;

                        staffList.Items[sn].SubItems.Add(obj[0].ToString());

                        staffList.Items[sn].SubItems.Add(obj[1].ToString());

                        staffList.Items[sn].SubItems.Add(obj[2].ToString());

                        staffList.Items[sn].SubItems.Add(obj[3].ToString());


                        sn++;
                    }
                }
                
                reader.Close();
                con.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }


        void SearchSerials()
        {
            serialList.Items.Clear();

            string statement = "";

            if (SerialSearchType == "All")
            {
                statement = "SELECT title, addressOfpublisher, markid, markno, addressOfagent, frequency FROM serials WHERE title LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "markid LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "markno LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "addressOfpublisher LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "addressOfagent LIKE '%" + bookSearchbox.Text + "%'";
            }
            else if (SerialSearchType == "Publisher")
            {
                statement = "SELECT  title, addressOfpublisher, markid, markno, addressOfagent, frequency FROM serials WHERE addressOfpublisher LIKE '%" + bookSearchbox.Text + "%'";
            }
            else if (SerialSearchType == "Title")
            {
                statement = "SELECT title, addressOfpublisher, markid, markno, addressOfagent, frequency FROM serials WHERE title LIKE '%" + bookSearchbox.Text + "%'";
            }
            else if (SerialSearchType == "Subject")
            {
                statement = "SELECT title, addressOfpublisher, markid, markno, addressOfagent, frequency FROM serials WHERE markid=(SELECT id FROM subject WHERE name LIKE '%" + serialSearchbox.Text + "%');";
            }
            else if (SerialSearchType == "MarkID")
            {
                try
                {
                    string[] str = new string[2];
                    char[] sep = new char[1];
                    sep[0] = ' ';
                    str = serialSearchbox.Text.Split(sep);

                    string mid = "", mno = "";
                    mid = str[0];
                    mno = str[1];

                    statement = "SELECT title, addressOfpublisher, markid, markno, addressOfagent, frequency FROM serials WHERE markid='" + mid + "' " +
                                "AND markno='" + mno + "'";
                }
                catch { }
            }


            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                MySqlDataReader reader = com.ExecuteReader();

                object[] obj = new object[6];
                int sn = 0;

                while (reader.Read())
                {
                    reader.GetValues(obj);

                    serialList.Items.Add((sn + 1).ToString());

                    if (sn % 2 == 0)
                        serialList.Items[sn].BackColor = Color.Beige;
                    else
                        serialList.Items[sn].BackColor = Color.White;

                    serialList.Items[sn].SubItems.Add(obj[1].ToString());

                    serialList.Items[sn].SubItems.Add(obj[0].ToString());

                    string mark = obj[2].ToString() + " " + obj[3].ToString();
                    serialList.Items[sn].SubItems.Add(mark);

                    serialList.Items[sn].SubItems.Add(obj[4].ToString());
                    serialList.Items[sn].SubItems.Add(obj[5].ToString());

                    sn++;
                }
                reader.Close();
                con.Close();

            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        void SearchBooks()
        {
            bookList.Items.Clear();
            string statement = "";

            if (BookSearchType == "All")
            {
                statement = "SELECT author, title, markid, markno, available FROM books WHERE author LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "title LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "markid LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "markno LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "isbn LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "accfrom LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "accto LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "date LIKE '%" + bookSearchbox.Text + "%' OR " +
                            "source LIKE '%" + bookSearchbox.Text + "%'";
            }
            else if (BookSearchType == "Author")
            {
                statement = "SELECT author, title, markid, markno, available FROM books WHERE author LIKE '%" + bookSearchbox.Text + "%'";
            }
            else if (BookSearchType == "Title")
            {
                statement = "SELECT author, title, markid, markno, available FROM books WHERE title LIKE '%" + bookSearchbox.Text + "%'";
            }
            else if (BookSearchType == "Subject")
            {
                statement = "SELECT author, title, markid, markno, available FROM books WHERE markid=(SELECT id FROM subject WHERE name LIKE '%" + bookSearchbox.Text + "%');";
            }
            else if (BookSearchType == "MarkID")
            {
                try
                {
                    string[] str = new string[2];
                    char[] sep = new char[1];
                    sep[0] = ' ';
                    str = bookSearchbox.Text.Split(sep);

                    string mid = "", mno = "";
                    mid = str[0];
                    mno = str[1];

                    statement = "SELECT author, title, markid, markno, available FROM books WHERE markid='" + mid + "' " +
                                "AND markno='" + mno + "'";
                }
                catch { }
            }


            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                MySqlDataReader reader = com.ExecuteReader();

                object[] obj = new object[7];
                int sn = 0;

                while (reader.Read())
                {
                    reader.GetValues(obj);

                    bookList.Items.Add((sn + 1).ToString());

                    if (sn % 2 == 0)
                        bookList.Items[sn].BackColor = Color.Beige;
                    else
                        bookList.Items[sn].BackColor = Color.White;

                    bookList.Items[sn].SubItems.Add(obj[1].ToString());

                    bookList.Items[sn].SubItems.Add(obj[0].ToString());

                    string mark = obj[2].ToString() + " " + obj[3].ToString();
                    bookList.Items[sn].SubItems.Add(mark);

                    bookList.Items[sn].SubItems.Add(obj[4].ToString());
                    //bookList.Items[sn].SubItems.Add(obj[5].ToString());

                    sn++;
                }
                reader.Close();
                con.Close();

            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }


        void searchbox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == Keys.Enter.ToString())
                SearchBooks();           
        }      
        
        private void option1_Click(object sender, EventArgs e)
        {
            option1.CheckState = CheckState.Checked;
            option3.CheckState = CheckState.Unchecked;
            option4.CheckState = CheckState.Unchecked;
            option5.CheckState = CheckState.Unchecked;
            option6.CheckState = CheckState.Unchecked;
            BookSearchType = "Author";
        }


        private void option3_Click(object sender, EventArgs e)
        {
            option3.CheckState = CheckState.Checked;
            option1.CheckState = CheckState.Unchecked;
            option4.CheckState = CheckState.Unchecked;
            option5.CheckState = CheckState.Unchecked;
            option6.CheckState = CheckState.Unchecked;
            BookSearchType = "Title";
        }

        private void option4_Click(object sender, EventArgs e)
        {
            option4.CheckState = CheckState.Checked;
            option1.CheckState = CheckState.Unchecked;
            option3.CheckState = CheckState.Unchecked;
            option5.CheckState = CheckState.Unchecked;
            option6.CheckState = CheckState.Unchecked;
            BookSearchType = "All";
        }

        private void option5_Click(object sender, EventArgs e)
        {
            option5.CheckState = CheckState.Checked;
            option1.CheckState = CheckState.Unchecked;
            option3.CheckState = CheckState.Unchecked;
            option4.CheckState = CheckState.Unchecked;
            option6.CheckState = CheckState.Unchecked;
            BookSearchType = "Subject";
        }

        private void option6_Click(object sender, EventArgs e)
        {
            option5.CheckState = CheckState.Unchecked;
            option1.CheckState = CheckState.Unchecked;
            option3.CheckState = CheckState.Unchecked;
            option4.CheckState = CheckState.Unchecked;
            option6.CheckState = CheckState.Checked;
            BookSearchType = "MarkID";
        }


        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            /*Add_Record ad = new Add_Record();
            ad.Owner = this;
            ad.ConnectionString = ConnectionString;
            ad.Show();*/
        }

        void Form1_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (!logoutClick)
                Application.Exit();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to logout user: `" + useridlabel.Text + "` ?", "LMIS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                logoutClick = true;
                ConnectionString = "";
                this.Close();
                Login lf = new Login();
                lf.ConnectionString = "";
                lf.Show();
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            Student_Register sr = new Student_Register();
            sr.Owner = this;
            sr.ConnectionString = ConnectionString;
            sr.Show();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (bookList.SelectedItems.Count > 0)
            {
                string id = "", no = "", si = "";
                char[] sep = new char[1];
                string[] val = new string[2];
                string statement = "";
                

                DialogResult r = MessageBox.Show("Are you sure you want to delete the selected item?", "LMIS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    try
                    {
                        si = bookList.SelectedItems[0].SubItems[3].Text;

                        sep[0] = ' ';

                        val = si.Split(sep);
                        id = val[0];
                        no = val[1];

                        statement = "DELETE FROM books WHERE markid='" + id +
                               "' AND markno='" + no + "'";

                        MySqlConnection con = new MySqlConnection(ConnectionString);
                        con.Open();

                        MySqlCommand com = new MySqlCommand(statement, con);
                        int affectedRows = com.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                    SearchBooks();
                }
            }                            
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (bookList.SelectedItems.Count > 0)
            {
                Add_Books ar = new Add_Books();
                ar.Owner = this;
                ar.ConnectionString = ConnectionString;
                ar.Text = "Modify Record";
                ar.modify = true;
                ar.modifyVal = bookList.SelectedItems[0].SubItems[3].Text;
                ar.Show();
            }
        }

        private void getInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedMenu == "ManageBooks")
                toolStripButton1.PerformClick();
            else if (selectedMenu == "StudentRegistration")
                toolStripButton9.PerformClick();
            else if (selectedMenu == "StaffRegistration")
                toolStripButton14.PerformClick();
            else if (selectedMenu == "ManageSerials")
                toolStripButton31.PerformClick();
            else if (selectedMenu == "LoanedItems")
                toolStripButton18.PerformClick();
            else if (selectedMenu == "ReturnedItems")
                toolStripButton23.PerformClick();
            else if (selectedMenu == "OverdueItems")
                toolStripButton24.PerformClick();
        }

        private void modifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedMenu == "ManageBooks")
                toolStripButton3.PerformClick();
            else if (selectedMenu == "StudentRegistration")
                toolStripButton7.PerformClick();
            else if (selectedMenu == "StaffRegistration")
                toolStripButton12.PerformClick();
            else if (selectedMenu == "ManageSerials")
                toolStripButton16.PerformClick();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedMenu == "ManageBooks")
                toolStripButton4.PerformClick();
            else if (selectedMenu == "StudentRegistration")
                toolStripButton8.PerformClick();
            else if (selectedMenu == "StaffRegistration")
                toolStripButton13.PerformClick();
            else if (selectedMenu == "ManageSerials")
                toolStripButton26.PerformClick();
        }

        private void addNewRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedMenu == "ManageBooks")
                toolStripButton34.PerformClick();
            if (selectedMenu == "StudentRegistration")
                toolStripButton6.PerformClick();
            else if (selectedMenu == "StaffRegistration")
                toolStripButton11.PerformClick();
            else if (selectedMenu == "ManageSerials")
                toolStripButton33.PerformClick();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Text == "ACU Library Management Information System.")
            {
                SearchBooks();
                SearchSerials();
                SearchRegister("student");
                SearchRegister("staff");
                SearchLoaned();
                SearchReturned();
                CheckDue();
                SearchDue();
                this.Text = "ACU Library Management Information System";
            }

        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (studentList.SelectedItems.Count > 0)
            {
                Student_Register mr = new Student_Register();
                mr.Owner = this;
                mr.ConnectionString = ConnectionString;
                mr.modify = true;
                mr.Text = "Update Registration";
                mr.modifyval = studentList.SelectedItems[0].SubItems[1].Text;
                mr.Show();
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (studentList.SelectedItems.Count > 0)
            {
                string si = "";

                si = studentList.SelectedItems[0].SubItems[1].Text;

                DialogResult r = MessageBox.Show("Are you sure you want to delete the selected item?\n" + si, "LMIS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    string statement = "DELETE FROM registration WHERE id='" + si + "'";
                    try
                    {
                        MySqlConnection con = new MySqlConnection(ConnectionString);
                        con.Open();

                        MySqlCommand com = new MySqlCommand(statement, con);
                        int affectedRows = com.ExecuteNonQuery();
                        con.Close();

                        SearchRegister("student");
                    }
                    catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }                 
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            if (staffList.SelectedItems.Count > 0)
            {
                Staff_Register sr = new Staff_Register();
                sr.Owner = this;
                sr.ConnectionString = ConnectionString;
                sr.modify = true;
                sr.Text = "Update Registration";
                sr.modifyval = staffList.SelectedItems[0].SubItems[1].Text;
                sr.Show();
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            Staff_Register sr = new Staff_Register();
            sr.Owner = this;
            sr.ConnectionString = ConnectionString;
            sr.Show();
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            if (staffList.SelectedItems.Count > 0)
            {
                string si = "";

                si = staffList.SelectedItems[0].SubItems[1].Text;

                DialogResult r = MessageBox.Show("Are you sure you want to delete the selected item?\n" + si, "LMIS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    string statement = "DELETE FROM registration WHERE id='" + si + "'";
                    try
                    {
                        MySqlConnection con = new MySqlConnection(ConnectionString);
                        con.Open();

                        MySqlCommand com = new MySqlCommand(statement, con);
                        int affectedRows = com.ExecuteNonQuery();
                        con.Close();

                        SearchRegister("staff");
                    }
                    catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }                 
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            //toolStripButton2.PerformClick();
        }

        private void toolStripButton28_Click(object sender, EventArgs e)
        {
            toolStripButton11.PerformClick();
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            toolStripButton6.PerformClick();
        }

        private void context_Opening(object sender, CancelEventArgs e)
        {
            viewVolumesToolStripMenuItem.Visible = false;
            newVolumeToolStripMenuItem.Visible = false;
            loanItemToolStripMenuItem.Visible = false;
            returnItemToolStripMenuItem.Visible = false;
            addNewRecordToolStripMenuItem.Visible = false;
            modifyToolStripMenuItem.Visible = false;
            deleteToolStripMenuItem.Visible = false;
            getInfoToolStripMenuItem.Visible = true;
            getInfoToolStripMenuItem.Enabled = false;

            if (selectedMenu == "LoanedItems")
            {
                if (loanedList.SelectedItems.Count > 0)
                {
                    if (loanedList.SelectedItems[0].ForeColor != Color.Red)
                        returnItemToolStripMenuItem.Visible = true;
                    else
                        returnItemToolStripMenuItem.Visible = false;
                    getInfoToolStripMenuItem.Enabled = true;
                }
                else
                {
                    returnItemToolStripMenuItem.Visible = false;
                    getInfoToolStripMenuItem.Enabled = false;
                }
            }
            else if (selectedMenu == "ManageBooks")
            {
                if (bookList.SelectedItems.Count > 0)
                {
                    loanItemToolStripMenuItem.Visible = true;
                    modifyToolStripMenuItem.Visible = true;
                    deleteToolStripMenuItem.Visible = true;
                    getInfoToolStripMenuItem.Visible = true;
                    getInfoToolStripMenuItem.Enabled = true;
                }
                else
                {
                    loanItemToolStripMenuItem.Visible = false;
                    modifyToolStripMenuItem.Visible = false;
                    deleteToolStripMenuItem.Visible = false;
                    getInfoToolStripMenuItem.Visible = false;
                }
                addNewRecordToolStripMenuItem.Visible = true;
            }
            else if (selectedMenu == "ManageSerials")
            {
                if (serialList.SelectedItems.Count > 0)
                {
                    viewVolumesToolStripMenuItem.Visible = true;
                    newVolumeToolStripMenuItem.Visible = true;
                    modifyToolStripMenuItem.Visible = true;
                    deleteToolStripMenuItem.Visible = true;
                    getInfoToolStripMenuItem.Visible = true;
                    getInfoToolStripMenuItem.Enabled = true;
                }
                else
                {
                    viewVolumesToolStripMenuItem.Visible = false;
                    newVolumeToolStripMenuItem.Visible = false;
                    modifyToolStripMenuItem.Visible = false;
                    deleteToolStripMenuItem.Visible = false;
                    getInfoToolStripMenuItem.Visible = false;
                    getInfoToolStripMenuItem.Enabled = false;
                }

                addNewRecordToolStripMenuItem.Visible = true;
            }
            else if (selectedMenu == "ReturnedItems" || selectedMenu == "OverdueItems")
            {
                getInfoToolStripMenuItem.Visible = true;

                if (returnedList.SelectedItems.Count > 0 || dueList.SelectedItems.Count > 0)
                {
                    getInfoToolStripMenuItem.Enabled = true;
                }
                else
                    getInfoToolStripMenuItem.Enabled = false;

            }
            else if (selectedMenu == "StudentRegistration")
            {
                if (studentList.SelectedItems.Count > 0)
                {
                    getInfoToolStripMenuItem.Visible = true;
                    getInfoToolStripMenuItem.Enabled = true;
                    modifyToolStripMenuItem.Visible = true;
                    deleteToolStripMenuItem.Visible = true;
                }
                addNewRecordToolStripMenuItem.Visible = true;
            }
            else if (selectedMenu == "StaffRegistration")
            {
                if (staffList.SelectedItems.Count > 0)
                {
                    getInfoToolStripMenuItem.Visible = true;
                    getInfoToolStripMenuItem.Enabled = true;
                    modifyToolStripMenuItem.Visible = true;
                    deleteToolStripMenuItem.Visible = true;
                }
                else
                    getInfoToolStripMenuItem.Enabled = false;

                addNewRecordToolStripMenuItem.Visible = true;
            }      
        }


        private void returnItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton19.PerformClick();
        }

        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            if (bookList.SelectedItems.Count > 0)
            {
                //get available count
                int avail = int.Parse(bookList.SelectedItems[0].SubItems[4].Text);

                if (avail > 1)
                {

                    string BItem = bookList.SelectedItems[0].SubItems[3].Text;

                    char[] sep = new char[2];
                    sep[0] = ' ';

                    string id = "", no = "";

                    string[] str = new string[2];
                    str = BItem.Split(sep);

                    id = str[0];
                    no = str[1];

                    Loan br = new Loan();
                    br.id = id;
                    br.no = no;
                    br.ConnectionString = ConnectionString;
                    br.Owner = this;
                    br.itemtxt.Text = id + " " + no;
                    br.title = bookList.SelectedItems[0].SubItems[1].Text;
                    br.author = bookList.SelectedItems[0].SubItems[2].Text;
                    br.Show();
                }
                else
                    MessageBox.Show("The Selected Item is not available for loan!!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            if (loanedList.SelectedItems.Count > 0)
            {
                string BItem = loanedList.SelectedItems[0].SubItems[3].Text;

                char[] sep = new char[2];
                sep[0] = ' ';

                string id = "", no = "";

                string[] str = new string[2];
                str = BItem.Split(sep);

                id = str[0];
                no = str[1];

                Return rt = new Return();
                rt.id = id;
                rt.no = no;
                rt.ConnectionString = ConnectionString;
                rt.Owner = this;
                rt.itemtxt.Text = id + " " + no;
                rt.accnotxt.Text = loanedList.SelectedItems[0].SubItems[4].Text;
                rt.borrowIDtxt.Text = loanedList.SelectedItems[0].SubItems[5].Text;
                rt.loandatetxt.Text = loanedList.SelectedItems[0].SubItems[6].Text;

                //set dates
                string today = "", day = "", month = "", year = "";
                //return date
                day = DateTime.Now.Day.ToString();
                if (int.Parse(day) < 10)
                    day = "0" + day;
                month = DateTime.Now.Month.ToString();
                if (int.Parse(month) < 10)
                    month = "0" + month;
                year = DateTime.Now.Year.ToString();
                today = day + "/" + month + "/" + year;

                rt.returndatetxt.Text = today;

                rt.title = loanedList.SelectedItems[0].SubItems[1].Text;
                rt.author = loanedList.SelectedItems[0].SubItems[2].Text;
                rt.Show();
            }
        }

        private void settings_Click(object sender, EventArgs e)
        {
            Settings st = new Settings();
            st.Owner = this;
            st.ConnectionString = ConnectionString;
            st.Show();
        }

        private void toolStripButton29_Click(object sender, EventArgs e)
        {
            Settings st = new Settings();
            st.sel = "cat";
            st.Owner = this;
            st.ConnectionString = ConnectionString;
            st.Show();
        }

        private void toolStripButton30_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton21_Click(object sender, EventArgs e)
        {
            
        }


        void loanSearchtxt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == Keys.Enter.ToString())
            {
                SearchLoaned();
            }
        }

        void returnSearchtxt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == Keys.Enter.ToString())
            {
                SearchReturned();
            }
        }

        void dueSearchtxt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == Keys.Enter.ToString())
            {
                SearchDue();
            }
        }

        private void bOption1_Click(object sender, EventArgs e)
        {
            bOption1.CheckState = CheckState.Checked;
            bOption2.CheckState = CheckState.Unchecked;
            bOption3.CheckState = CheckState.Unchecked;
            bOption4.CheckState = CheckState.Unchecked;
            bOption5.CheckState = CheckState.Unchecked;
            bOption6.CheckState = CheckState.Unchecked;
            bOption7.CheckState = CheckState.Unchecked;
            bOption8.CheckState = CheckState.Unchecked;
            LoanSearchType = "BorrowerID";
        }

        private void bOption2_Click(object sender, EventArgs e)
        {
            bOption1.CheckState = CheckState.Unchecked;
            bOption2.CheckState = CheckState.Checked;
            bOption3.CheckState = CheckState.Unchecked;
            bOption4.CheckState = CheckState.Unchecked;
            bOption5.CheckState = CheckState.Unchecked;
            bOption6.CheckState = CheckState.Unchecked;
            bOption7.CheckState = CheckState.Unchecked;
            bOption8.CheckState = CheckState.Unchecked;
            LoanSearchType = "LoanDate";
        }

        private void bOption3_Click(object sender, EventArgs e)
        {
            bOption1.CheckState = CheckState.Unchecked;
            bOption2.CheckState = CheckState.Unchecked;
            bOption3.CheckState = CheckState.Checked;
            bOption4.CheckState = CheckState.Unchecked;
            bOption5.CheckState = CheckState.Unchecked;
            bOption6.CheckState = CheckState.Unchecked;
            bOption7.CheckState = CheckState.Unchecked;
            bOption8.CheckState = CheckState.Unchecked;
            LoanSearchType = "ReturnDate";
        }

        private void bOption4_Click(object sender, EventArgs e)
        {
            bOption1.CheckState = CheckState.Unchecked;
            bOption2.CheckState = CheckState.Unchecked;
            bOption3.CheckState = CheckState.Unchecked;
            bOption4.CheckState = CheckState.Checked;
            bOption5.CheckState = CheckState.Unchecked;
            bOption6.CheckState = CheckState.Unchecked;
            bOption7.CheckState = CheckState.Unchecked;
            bOption8.CheckState = CheckState.Unchecked;
            LoanSearchType = "Title";
        }

        private void bOption5_Click(object sender, EventArgs e)
        {
            bOption1.CheckState = CheckState.Unchecked;
            bOption2.CheckState = CheckState.Unchecked;
            bOption3.CheckState = CheckState.Unchecked;
            bOption4.CheckState = CheckState.Unchecked;
            bOption5.CheckState = CheckState.Checked;
            bOption6.CheckState = CheckState.Unchecked;
            bOption7.CheckState = CheckState.Unchecked;
            bOption8.CheckState = CheckState.Unchecked;
            LoanSearchType = "Author";
        }

        private void bOption6_Click(object sender, EventArgs e)
        {
            bOption1.CheckState = CheckState.Unchecked;
            bOption2.CheckState = CheckState.Unchecked;
            bOption3.CheckState = CheckState.Unchecked;
            bOption4.CheckState = CheckState.Unchecked;
            bOption5.CheckState = CheckState.Unchecked;
            bOption6.CheckState = CheckState.Checked;
            bOption7.CheckState = CheckState.Unchecked;
            bOption8.CheckState = CheckState.Unchecked;
            LoanSearchType = "Subject";
        }

        private void bOption7_Click(object sender, EventArgs e)
        {
            bOption1.CheckState = CheckState.Unchecked;
            bOption2.CheckState = CheckState.Unchecked;
            bOption3.CheckState = CheckState.Unchecked;
            bOption4.CheckState = CheckState.Unchecked;
            bOption5.CheckState = CheckState.Unchecked;
            bOption6.CheckState = CheckState.Unchecked;
            bOption7.CheckState = CheckState.Checked;
            bOption8.CheckState = CheckState.Unchecked;
            LoanSearchType = "AccessionNo";
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bOption1.CheckState = CheckState.Unchecked;
            bOption2.CheckState = CheckState.Unchecked;
            bOption3.CheckState = CheckState.Unchecked;
            bOption4.CheckState = CheckState.Unchecked;
            bOption5.CheckState = CheckState.Unchecked;
            bOption6.CheckState = CheckState.Unchecked;
            bOption7.CheckState = CheckState.Unchecked;
            bOption8.CheckState = CheckState.Checked;
            LoanSearchType = "All";
        }

        private void rOption1_Click(object sender, EventArgs e)
        {
            rOption1.CheckState = CheckState.Checked;
            rOption2.CheckState = CheckState.Unchecked;
            rOption3.CheckState = CheckState.Unchecked;
            rOption4.CheckState = CheckState.Unchecked;
            rOption5.CheckState = CheckState.Unchecked;
            rOption6.CheckState = CheckState.Unchecked;
            rOption7.CheckState = CheckState.Unchecked;
            rOption8.CheckState = CheckState.Unchecked;
            ReturnSearchType = "BorrowerID";
        }

        private void rOption2_Click(object sender, EventArgs e)
        {
            rOption1.CheckState = CheckState.Unchecked;
            rOption2.CheckState = CheckState.Checked;
            rOption3.CheckState = CheckState.Unchecked;
            rOption4.CheckState = CheckState.Unchecked;
            rOption5.CheckState = CheckState.Unchecked;
            rOption6.CheckState = CheckState.Unchecked;
            rOption7.CheckState = CheckState.Unchecked;
            rOption8.CheckState = CheckState.Unchecked;
            ReturnSearchType = "LoanDate";
        }

        private void rOption3_Click(object sender, EventArgs e)
        {
            rOption1.CheckState = CheckState.Unchecked;
            rOption2.CheckState = CheckState.Unchecked;
            rOption3.CheckState = CheckState.Checked;
            rOption4.CheckState = CheckState.Unchecked;
            rOption5.CheckState = CheckState.Unchecked;
            rOption6.CheckState = CheckState.Unchecked;
            rOption7.CheckState = CheckState.Unchecked;
            rOption8.CheckState = CheckState.Unchecked;
            ReturnSearchType = "ReturnDate";
        }

        private void rOption4_Click(object sender, EventArgs e)
        {
            rOption1.CheckState = CheckState.Unchecked;
            rOption2.CheckState = CheckState.Unchecked;
            rOption3.CheckState = CheckState.Unchecked;
            rOption4.CheckState = CheckState.Checked;
            rOption5.CheckState = CheckState.Unchecked;
            rOption6.CheckState = CheckState.Unchecked;
            rOption7.CheckState = CheckState.Unchecked;
            rOption8.CheckState = CheckState.Unchecked;
            ReturnSearchType = "Title";
        }

        private void rOption5_Click(object sender, EventArgs e)
        {
            rOption1.CheckState = CheckState.Unchecked;
            rOption2.CheckState = CheckState.Unchecked;
            rOption3.CheckState = CheckState.Unchecked;
            rOption4.CheckState = CheckState.Unchecked;
            rOption5.CheckState = CheckState.Checked;
            rOption6.CheckState = CheckState.Unchecked;
            rOption7.CheckState = CheckState.Unchecked;
            rOption8.CheckState = CheckState.Unchecked;
            ReturnSearchType = "Author";
        }

        private void rOption6_Click(object sender, EventArgs e)
        {
            rOption1.CheckState = CheckState.Unchecked;
            rOption2.CheckState = CheckState.Unchecked;
            rOption3.CheckState = CheckState.Unchecked;
            rOption4.CheckState = CheckState.Unchecked;
            rOption5.CheckState = CheckState.Unchecked;
            rOption6.CheckState = CheckState.Checked;
            rOption7.CheckState = CheckState.Unchecked;
            rOption8.CheckState = CheckState.Unchecked;
            ReturnSearchType = "Subject";
        }

        private void rOption7_Click(object sender, EventArgs e)
        {
            rOption1.CheckState = CheckState.Unchecked;
            rOption2.CheckState = CheckState.Unchecked;
            rOption3.CheckState = CheckState.Unchecked;
            rOption4.CheckState = CheckState.Unchecked;
            rOption5.CheckState = CheckState.Unchecked;
            rOption6.CheckState = CheckState.Unchecked;
            rOption7.CheckState = CheckState.Checked;
            rOption8.CheckState = CheckState.Unchecked;
            ReturnSearchType = "AccessionNo";
        }

        private void rOption8_Click(object sender, EventArgs e)
        {
            rOption1.CheckState = CheckState.Unchecked;
            rOption2.CheckState = CheckState.Unchecked;
            rOption3.CheckState = CheckState.Unchecked;
            rOption4.CheckState = CheckState.Unchecked;
            rOption5.CheckState = CheckState.Unchecked;
            rOption6.CheckState = CheckState.Unchecked;
            rOption7.CheckState = CheckState.Unchecked;
            rOption8.CheckState = CheckState.Checked;
            ReturnSearchType = "All";
        }


        private void dOption1_Click(object sender, EventArgs e)
        {
            dOption1.CheckState = CheckState.Checked;
            dOption2.CheckState = CheckState.Unchecked;
            dOption3.CheckState = CheckState.Unchecked;
            dOption4.CheckState = CheckState.Unchecked;
            dOption5.CheckState = CheckState.Unchecked;
            dOption6.CheckState = CheckState.Unchecked;
            dOption7.CheckState = CheckState.Unchecked;
            dOption8.CheckState = CheckState.Unchecked;
            DueSearchType = "BorrowerID";
        }

        private void dOption2_Click(object sender, EventArgs e)
        {
            dOption1.CheckState = CheckState.Unchecked;
            dOption2.CheckState = CheckState.Checked;
            dOption3.CheckState = CheckState.Unchecked;
            dOption4.CheckState = CheckState.Unchecked;
            dOption5.CheckState = CheckState.Unchecked;
            dOption6.CheckState = CheckState.Unchecked;
            dOption7.CheckState = CheckState.Unchecked;
            dOption8.CheckState = CheckState.Unchecked;
            DueSearchType = "LoanDate";
        }

        private void dOption3_Click(object sender, EventArgs e)
        {
            dOption1.CheckState = CheckState.Unchecked;
            dOption2.CheckState = CheckState.Unchecked;
            dOption3.CheckState = CheckState.Checked;
            dOption4.CheckState = CheckState.Unchecked;
            dOption5.CheckState = CheckState.Unchecked;
            dOption6.CheckState = CheckState.Unchecked;
            dOption7.CheckState = CheckState.Unchecked;
            dOption8.CheckState = CheckState.Unchecked;
            DueSearchType = "DueDate";
        }

        private void dOption4_Click(object sender, EventArgs e)
        {
            dOption1.CheckState = CheckState.Unchecked;
            dOption2.CheckState = CheckState.Unchecked;
            dOption3.CheckState = CheckState.Unchecked;
            dOption4.CheckState = CheckState.Checked;
            dOption5.CheckState = CheckState.Unchecked;
            dOption6.CheckState = CheckState.Unchecked;
            dOption7.CheckState = CheckState.Unchecked;
            dOption8.CheckState = CheckState.Unchecked;
            DueSearchType = "Title";
        }

        private void dOption5_Click(object sender, EventArgs e)
        {
            dOption1.CheckState = CheckState.Unchecked;
            dOption2.CheckState = CheckState.Unchecked;
            dOption3.CheckState = CheckState.Unchecked;
            dOption4.CheckState = CheckState.Unchecked;
            dOption5.CheckState = CheckState.Checked;
            dOption6.CheckState = CheckState.Unchecked;
            dOption7.CheckState = CheckState.Unchecked;
            dOption8.CheckState = CheckState.Unchecked;
            DueSearchType = "Author";
        }

        private void dOption6_Click(object sender, EventArgs e)
        {
            dOption1.CheckState = CheckState.Unchecked;
            dOption2.CheckState = CheckState.Unchecked;
            dOption3.CheckState = CheckState.Unchecked;
            dOption4.CheckState = CheckState.Unchecked;
            dOption5.CheckState = CheckState.Unchecked;
            dOption6.CheckState = CheckState.Checked;
            dOption7.CheckState = CheckState.Unchecked;
            dOption8.CheckState = CheckState.Unchecked;
            DueSearchType = "Subject";
        }

        private void dOption7_Click(object sender, EventArgs e)
        {
            dOption1.CheckState = CheckState.Unchecked;
            dOption2.CheckState = CheckState.Unchecked;
            dOption3.CheckState = CheckState.Unchecked;
            dOption4.CheckState = CheckState.Unchecked;
            dOption5.CheckState = CheckState.Unchecked;
            dOption6.CheckState = CheckState.Unchecked;
            dOption7.CheckState = CheckState.Checked;
            dOption8.CheckState = CheckState.Unchecked;
            DueSearchType = "AccessionNo";
        }

        private void dOption8_Click(object sender, EventArgs e)
        {
            dOption1.CheckState = CheckState.Unchecked;
            dOption2.CheckState = CheckState.Unchecked;
            dOption3.CheckState = CheckState.Unchecked;
            dOption4.CheckState = CheckState.Unchecked;
            dOption5.CheckState = CheckState.Unchecked;
            dOption6.CheckState = CheckState.Unchecked;
            dOption7.CheckState = CheckState.Unchecked;
            dOption8.CheckState = CheckState.Checked;
            DueSearchType = "All";
        }

        
        private void addBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add_Books ad = new Add_Books();
            ad.Owner = this;
            ad.ConnectionString = ConnectionString;
            ad.Show();
        }

        private void bookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //addBookToolStripMenuItem.PerformClick();
        }

        private void addSerialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add_Serial asr = new Add_Serial();
            asr.Owner = this;
            asr.ConnectionString = ConnectionString;
            asr.Show();
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            //toolStripButton19.PerformClick();

            if (dueList.SelectedItems.Count > 0)
            {
                string DItem = dueList.SelectedItems[0].SubItems[3].Text;

                char[] sep = new char[2];
                sep[0] = ' ';

                string id = "", no = "";

                string[] str = new string[2];
                str = DItem.Split(sep);

                id = str[0];
                no = str[1];

                Return rt = new Return();
                rt.id = id;
                rt.no = no;
                rt.ConnectionString = ConnectionString;
                rt.Owner = this;
                rt.itemtxt.Text = id + " " + no;
                rt.accnotxt.Text = dueList.SelectedItems[0].SubItems[4].Text;
                rt.borrowIDtxt.Text = dueList.SelectedItems[0].SubItems[5].Text;
                rt.loandatetxt.Text = dueList.SelectedItems[0].SubItems[6].Text;

                //set dates
                string today = "", day = "", month = "", year = "";
                //return date
                day = DateTime.Now.Day.ToString();
                if (int.Parse(day) < 10)
                    day = "0" + day;
                month = DateTime.Now.Month.ToString();
                if (int.Parse(month) < 10)
                    month = "0" + month;
                year = DateTime.Now.Year.ToString();
                today = day + "/" + month + "/" + year;

                rt.returndatetxt.Text = today;

                rt.title = dueList.SelectedItems[0].SubItems[1].Text;
                rt.author = dueList.SelectedItems[0].SubItems[2].Text;
                rt.Show();
            }
        }

        private void addBookToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripButton34.PerformClick();
        }

        private void addSerialToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripButton33.PerformClick();
        }

        private void serialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //addSerialToolStripMenuItem.PerformClick();
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton34_Click(object sender, EventArgs e)
        {
            Add_Books ad = new Add_Books();
            ad.Owner = this;
            ad.ConnectionString = ConnectionString;
            ad.Show();
        }

        private void toolStripButton33_Click(object sender, EventArgs e)
        {
            Add_Serial asr = new Add_Serial();
            asr.Owner = this;
            asr.ConnectionString = ConnectionString;
            asr.Show();
        }

        private void toolStripButton16_Click_1(object sender, EventArgs e)
        {
            if (serialList.SelectedItems.Count > 0)
            {
                Add_Serial asr = new Add_Serial();
                asr.Owner = this;
                asr.ConnectionString = ConnectionString;
                asr.Text = "Modify Record";
                asr.modify = true;
                asr.modifyVal = serialList.SelectedItems[0].SubItems[3].Text;
                asr.Show();
            }
        }


        private void soption1_Click(object sender, EventArgs e)
        {
            soption1.CheckState = CheckState.Checked;
            soption2.CheckState = CheckState.Unchecked;
            soption3.CheckState = CheckState.Unchecked;
            soption4.CheckState = CheckState.Unchecked;
            soption5.CheckState = CheckState.Unchecked;
            SerialSearchType = "All";
        }

        private void soption2_Click(object sender, EventArgs e)
        {
            soption1.CheckState = CheckState.Unchecked;
            soption2.CheckState = CheckState.Checked;
            soption3.CheckState = CheckState.Unchecked;
            soption4.CheckState = CheckState.Unchecked;
            soption5.CheckState = CheckState.Unchecked;
            SerialSearchType = "Publisher";

        }

        private void soption3_Click(object sender, EventArgs e)
        {
            soption1.CheckState = CheckState.Unchecked;
            soption2.CheckState = CheckState.Unchecked;
            soption3.CheckState = CheckState.Checked;
            soption4.CheckState = CheckState.Unchecked;
            soption5.CheckState = CheckState.Unchecked;
            SerialSearchType = "Title";
        }

        private void soption4_Click(object sender, EventArgs e)
        {
            soption1.CheckState = CheckState.Unchecked;
            soption2.CheckState = CheckState.Unchecked;
            soption3.CheckState = CheckState.Unchecked;
            soption4.CheckState = CheckState.Checked;
            soption5.CheckState = CheckState.Unchecked;
            SerialSearchType = "Subject";
        }

        private void soption5_Click(object sender, EventArgs e)
        {
            soption1.CheckState = CheckState.Unchecked;
            soption2.CheckState = CheckState.Unchecked;
            soption3.CheckState = CheckState.Unchecked;
            soption4.CheckState = CheckState.Unchecked;
            soption5.CheckState = CheckState.Checked;
            SerialSearchType = "MarkID";
        }

        private void toolStripButton26_Click(object sender, EventArgs e)
        {
            if (serialList.SelectedItems.Count > 0)
            {
                string id = "", no = "", si = "";
                char[] sep = new char[1];
                string[] val = new string[2];
                string statement = "";


                DialogResult r = MessageBox.Show("Are you sure you want to delete the selected item?", "LMIS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    try
                    {
                        si = serialList.SelectedItems[0].SubItems[3].Text;

                        sep[0] = ' ';

                        val = si.Split(sep);
                        id = val[0];
                        no = val[1];

                        statement = "DELETE FROM serials WHERE markid='" + id +
                               "' AND markno='" + no + "'; DELETE FROM serials_record WHERE markid='" + id +
                               "' AND markno='" + no + "'";

                        MySqlConnection con = new MySqlConnection(ConnectionString);
                        con.Open();

                        MySqlCommand com = new MySqlCommand(statement, con);
                        int affectedRows = com.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                   
                    SearchSerials();
                }
            }            
        }

        private void newVolumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton29.PerformClick();
        }

        private void toolStripButton29_Click_1(object sender, EventArgs e)
        {
            if (serialList.SelectedItems.Count > 0)
            {
                Serial_Record sr = new Serial_Record();
                sr.ConnectionString = ConnectionString;
                sr.Owner = this;
                sr.mark = serialList.SelectedItems[0].SubItems[3].Text;
                sr.Show();
            }
        }

        private void toolStripButton35_Click(object sender, EventArgs e)
        {
            if (serialList.SelectedItems.Count > 0)
            {
                View_Serial_Record vsr = new View_Serial_Record();
                vsr.Owner = this;
                vsr.ConnectionString = ConnectionString;
                vsr.marklabel.Text = serialList.SelectedItems[0].SubItems[3].Text;
                vsr.Show();
            }
        }

        private void viewVolumesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton35.PerformClick();
        }

        private void toolStripButton31_Click(object sender, EventArgs e)
        {
            if (serialList.SelectedItems.Count > 0)
            {
                infomark = SepMark(serialList.SelectedItems[0].SubItems[3].Text);
                infoStatement = "SELECT * FROM serials WHERE markid='" + infomark[0] + "' AND markno='" + infomark[1] + "'";
                Get_Info gi = new Get_Info();
                gi.Owner = this;
                gi.ConnectionString = ConnectionString;
                gi.infoStatement = infoStatement;
                gi.Show();
            }
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            if (staffList.SelectedItems.Count > 0)
            {
                infoStatement = "SELECT * FROM registration WHERE id='" + staffList.SelectedItems[0].SubItems[1].Text + "'";
                Get_Info gi = new Get_Info();
                gi.Owner = this;
                gi.ConnectionString = ConnectionString;
                gi.infoStatement = infoStatement;
                gi.ssinfo = true;
                gi.Show();
            }
        }

        private void toolStripButton24_Click(object sender, EventArgs e)
        {
            if (dueList.SelectedItems.Count > 0)
            {
                infomark = SepMark(dueList.SelectedItems[0].SubItems[3].Text);
                infoStatement = "SELECT * FROM overdue WHERE markid='" + infomark[0] + "' AND markno='" + infomark[1] + "' AND accno='" + dueList.SelectedItems[0].SubItems[4].Text + "'";
                Get_Info gi = new Get_Info();
                gi.Owner = this;
                gi.ConnectionString = ConnectionString;
                gi.infoStatement = infoStatement;
                gi.Show();
            }
        }

        private void toolStripButton23_Click(object sender, EventArgs e)
        {
            if (returnedList.SelectedItems.Count > 0)
            {
                infomark = SepMark(returnedList.SelectedItems[0].SubItems[3].Text);
                infoStatement = "SELECT * FROM returned WHERE markid='" + infomark[0] + "' AND markno='" + infomark[1] + "' AND accno='" + returnedList.SelectedItems[0].SubItems[4].Text + "'";
                Get_Info gi = new Get_Info();
                gi.Owner = this;
                gi.ConnectionString = ConnectionString;
                gi.infoStatement = infoStatement;
                gi.Show();
            }
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            if (loanedList.SelectedItems.Count > 0)
            {
                infomark = SepMark(loanedList.SelectedItems[0].SubItems[3].Text);
                infoStatement = "SELECT * FROM loaned WHERE markid='" + infomark[0] + "' AND markno='" + infomark[1] + "' AND accno='" + loanedList.SelectedItems[0].SubItems[4].Text + "'";
                Get_Info gi = new Get_Info();
                gi.Owner = this;
                gi.ConnectionString = ConnectionString;
                gi.infoStatement = infoStatement;
                gi.Show();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (bookList.SelectedItems.Count > 0)
            {
                infomark = SepMark(bookList.SelectedItems[0].SubItems[3].Text);
                infoStatement = "SELECT * FROM books WHERE markid='" + infomark[0] + "' AND markno='" + infomark[1] + "'";
                Get_Info gi = new Get_Info();
                gi.Owner = this;
                gi.ConnectionString = ConnectionString;
                gi.infoStatement = infoStatement;
                gi.Show();
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (studentList.SelectedItems.Count > 0)
            {
                infoStatement = "SELECT * FROM registration WHERE id='" + studentList.SelectedItems[0].SubItems[1].Text + "'";
                Get_Info gi = new Get_Info();
                gi.Owner = this;
                gi.ConnectionString = ConnectionString;
                gi.infoStatement = infoStatement;
                gi.ssinfo = true;
                gi.Show();
            }
        }

        string[] SepMark(string mark)
        {
            char[] sep = new char[2];
            string[] str = new string[2];

            sep[0] = ' ';
            str = mark.Split(sep);
            return str;
        }

        private void banner2_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                string temp = Application.StartupPath + "/icons/banner2.jpg";
                Image im = Image.FromFile(temp);
                Graphics g = banner2.CreateGraphics();
                g.Clear(Color.Black);
                g.DrawImage(im, 0, 0, banner2.Width, banner2.Height);
                g.Dispose();
                im.Dispose();
            }
            catch { }
        }

        private void banner1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                string temp = Application.StartupPath + "/icons/banner1.jpg";
                Image im = Image.FromFile(temp);
                Graphics g = banner1.CreateGraphics();
                g.Clear(Color.Black);
                g.DrawImage(im, 0, 0, banner1.Width, banner1.Height);
                g.Dispose();
                im.Dispose();
            }
            catch { }
        }

        private void subjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings st = new Settings();
            st.sel = "sub";
            st.Owner = this;
            st.ConnectionString = ConnectionString;
            st.Show();
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings st = new Settings();
            st.sel = "user";
            st.Owner = this;
            st.ConnectionString = ConnectionString;
            st.Show();
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Report rep = new Report();
            rep.Owner = this;
            rep.ConnectionString = ConnectionString;
            rep.Show();
        }

        private void ManageBooksButton_Click(object sender, EventArgs e)
        {
            ManageBooksPanel.BringToFront();
            selectedMenu = "ManageBooks";
        }

        private void ManageSerialsButton_Click(object sender, EventArgs e)
        {
            ManageSerialsPanel.BringToFront();
            selectedMenu = "ManageSerials";
        }

        private void LoanedItemsButton_Click(object sender, EventArgs e)
        {
            LoanedItemsPanel.BringToFront();
            selectedMenu = "LoanedItems";
        }

        private void ReturnedItemsButton_Click(object sender, EventArgs e)
        {
            ReturnedItemsPanel.BringToFront();
            selectedMenu = "ReturnedItems";
        }

        private void OverdueItemsButton_Click(object sender, EventArgs e)
        {
            OverdueItemsPanel.BringToFront();
            selectedMenu = "OverdueItems";
        }

        private void StudentsButton_Click(object sender, EventArgs e)
        {
            StudentRegistrationPanel.BringToFront();
            selectedMenu = "StudentRegistration";
        }

        private void StaffButton_Click(object sender, EventArgs e)
        {
            StaffRegistrationPanel.BringToFront();
            selectedMenu = "StaffRegistration";
        }

        private void toolStripButton21_Click_1(object sender, EventArgs e)
        {
            change_password cp = new change_password();

            string[] str = new string[4];
            char[] sep = new char[2];
            sep[0] = '=';
            sep[1] = ';';

            str = ConnectionString.Split(sep);
            cp.server = str[1];
            cp.database = str[3];
            cp.username = str[5];

            cp.Owner = this;
            cp.Show();
        }

        private void toolStripButton30_Click_1(object sender, EventArgs e)
        {
            RefreshSystem();
        }

        private void toolStripButton36_Click(object sender, EventArgs e)
        {
            RefreshSystem();
        }


        private void toolStripButton37_Click(object sender, EventArgs e)
        {
            RefreshSystem();
        }

        private void toolStripButton38_Click(object sender, EventArgs e)
        {
            RefreshSystem();
        }

        private void toolStripButton39_Click(object sender, EventArgs e)
        {
            RefreshSystem();
        }

        private void toolStripButton40_Click(object sender, EventArgs e)
        {
            RefreshSystem();
        }

        private void toolStripButton41_Click(object sender, EventArgs e)
        {
            RefreshSystem();
        }

        private void loanItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton20.PerformClick();
        }

    }
}
