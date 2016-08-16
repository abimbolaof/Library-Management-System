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
using System.Collections.Specialized;
using System.Collections;

namespace LIBRARY
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        ListDictionary Priv = new ListDictionary();
        ListDictionary loadedPriv = new ListDictionary();
        ListDictionary menuPriv = new ListDictionary();
        ListDictionary loadedMenuPriv = new ListDictionary();

        private void button3_Click(object sender, EventArgs e)
        {
            if (idtxt.Text != "" && nametxt.Text != "")
            {
                string statement = "INSERT INTO subject VALUES ('" + idtxt.Text + "','" + nametxt.Text + "');" +
                                   "INSERT INTO lastmark VALUES ('" + idtxt.Text + "','" + "0" + "');";

                try
                {
                    MySqlConnection con = new MySqlConnection(ConnectionString);
                    con.Open();

                    MySqlCommand com = new MySqlCommand(statement, con);
                    int affectedRows = com.ExecuteNonQuery();
                    LoadSettings();
                    MessageBox.Show("New Subject Created!!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    con.Close();
                }
                catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (sublist.SelectedItems.Count > 0)
            {
                string DItem = sublist.SelectedItem.ToString();

                char[] sep = new char[2];
                sep[0] = ' ';
                string[] str = new string[2];
                str = DItem.Split(sep);

                string id = "", name = "";
                id = str[0];
                name = str[1];

                string statement = "DELETE FROM subject WHERE id='" + id + "'; DELETE FROM lastmark WHERE id='" + id + "';";

                DialogResult dra = MessageBox.Show("Are you sure you want to delete subject '" + DItem + "' ?", "LMIs", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dra == DialogResult.Yes)
                {
                    try
                    {
                        MySqlConnection con = new MySqlConnection(ConnectionString);
                        con.Open();

                        MySqlCommand com = new MySqlCommand(statement, con);
                        int affectedRows = com.ExecuteNonQuery();
                        sublist.Items.Remove(DItem);
                        LoadSettings();
                        con.Close();
                    }
                    catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }


                    DialogResult dr = MessageBox.Show("Do you wish to delete all library items under this Subject?", "LMIs", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        statement = "DELETE FROM books WHERE markid='" + id + "'; DELETE FROM serials WHERE markid='" + id + "'";

                        try
                        {
                            MySqlConnection con = new MySqlConnection(ConnectionString);
                            con.Open();

                            MySqlCommand com = new MySqlCommand(statement, con);
                            int affectedRows = com.ExecuteNonQuery();
                            this.Owner.Text += ".";
                            con.Close();
                        }
                        catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                }
            }
        }

        void LoadSettings()
        {
            sublist.Items.Clear();
            userList.Items.Clear();

            //load subjects
            string statement = "SELECT * FROM subject";

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                MySqlDataReader reader = com.ExecuteReader();

                object[] obj = new object[5];

                while (reader.Read())
                {
                    reader.GetValues(obj);
                    sublist.Items.Add(obj[0].ToString() + " (" + obj[1].ToString() + ")");
                }
                reader.Close();
                con.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }


            //load users
            statement = "SELECT username FROM users";

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                MySqlDataReader reader = com.ExecuteReader();

                object[] obj = new object[1];

                while (reader.Read())
                {
                    reader.GetValues(obj);
                    userList.Items.Add(obj[0].ToString());
                }
                reader.Close();
                con.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }


            //load No. Of Days Brfore Due Date
            statement = "SELECT DueDays FROM settings";

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);

                MySqlDataReader reader = com.ExecuteReader();

                object[] obj = new object[1];

                while (reader.Read())
                {
                    reader.GetValues(obj);
                    DueDaystxt.Text = obj[0].ToString();
                }
                
                reader.Close();
                con.Close();

            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }


            //load auto_refresh time
            statement = "SELECT auto_refresh FROM settings";

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);

                MySqlDataReader reader = com.ExecuteReader();

                object[] obj = new object[1];
                string[] str = new string[2];
                char[] sep = new char[1];
                sep[0] = ';';

                while (reader.Read())
                {
                    reader.GetValues(obj);
                }

                str = obj[0].ToString().Split(sep);

                if (str[0] == "YES")
                {
                    checkBox1.Checked = true;
                    auto_time.Text = str[1];
                    auto_time.Enabled = true;
                }
                else
                {
                    checkBox1.Checked = false;
                    auto_time.Enabled = false;
                }

                reader.Close();
                con.Close();

            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 
        }


        private void Settings_Load(object sender, EventArgs e)
        {
            LoadSettings();

            if (sel == "sub")
                mainTab.SelectedTab = subTab;
            else if (sel == "user")
                mainTab.SelectedTab = userTab;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string password="", privString="", menuPrivString="";
            int ct = 0;

            if (passwordtxt1.Text == passwordtxt2.Text)
            {
                password = passwordtxt2.Text;

                string statement = "CREATE USER '" + usernametxt.Text + "'@'%' IDENTIFIED BY '" + password + "'; " +
                               "GRANT ";

                foreach (DictionaryEntry val in Priv)
                {
                    ct++;
                    statement += val.Value.ToString();
                    privString += val.Value.ToString() + ";";
                    if (ct < Priv.Count)
                        statement += ", ";
                }

                foreach (DictionaryEntry val in menuPriv)
                {
                    menuPrivString += val.Value.ToString() + ";";
                }


                if (superPriv.Checked)
                {
                    statement += " ON *.* TO '" + usernametxt.Text + "'@'%' IDENTIFIED BY '" + password + "'";
                    statement += "; GRANT ALL PRIVILEGES ON *.* TO '" + usernametxt.Text + "'@'%' IDENTIFIED BY '" + password + "' WITH GRANT OPTION";
                    statement += "; GRANT ALL PRIVILEGES ON `acu_lmis`.* TO '" + usernametxt.Text + "'@'%' IDENTIFIED BY '" + password + "'";
                    menuPrivString += "SUPER;";
                }
                else
                {
                    statement += " ON `acu_lmis`.* TO '" + usernametxt.Text + "'@'%' IDENTIFIED BY '" + password + "'";
                }


                statement += "; GRANT SELECT ON `acu_lmis`.`loaned` TO '" + usernametxt.Text + "'@'%'; " +
                                 "GRANT INSERT, DELETE, SELECT ON `acu_lmis`.`overdue` TO '" + usernametxt.Text + "'@'%';";

                string st = "INSERT INTO users VALUES ('" + usernametxt.Text + "','" + privString + "','" + menuPrivString + "')";
                //MessageBox.Show(statement);
                try
                {
                    MySqlConnection con = new MySqlConnection(ConnectionString);
                    con.Open();

                    MySqlCommand com = new MySqlCommand(st, con);
                    int affectedRows = com.ExecuteNonQuery();


                    com.CommandText = statement;
                    com.ExecuteNonQuery();

                    LoadSettings();

                    Priv.Clear();
                    menuPriv.Clear();

                    MessageBox.Show("User '" + usernametxt.Text + "' Created Successfully!!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    con.Close();
                }
                catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            }
            else
                MessageBox.Show("Passwords do not match!!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void selectPriv_CheckedChanged(object sender, EventArgs e)
        {
            if (selectPriv.Checked)
                Priv.Add("select", "SELECT");
            else
                Priv.Remove("select");
        }

        private void deletePriv_CheckedChanged(object sender, EventArgs e)
        {
            if (deletePriv.Checked)
                Priv.Add("delete", "DELETE");
            else
                Priv.Remove("delete");
        }

        private void updatePriv_CheckedChanged(object sender, EventArgs e)
        {
            if (updatePriv.Checked)
                Priv.Add("update", "UPDATE");
            else
                Priv.Remove("update");
        }

        private void createPriv_CheckedChanged(object sender, EventArgs e)
        {
            if (createPriv.Checked)
                Priv.Add("create", "CREATE");
            else
                Priv.Remove("create");
        }

        private void dropPriv_CheckedChanged(object sender, EventArgs e)
        {
            if (dropPriv.Checked)
                Priv.Add("drop", "DROP");
            else
                Priv.Remove("drop");
        }

        private void insertPriv_CheckedChanged(object sender, EventArgs e)
        {
            if (insertPriv.Checked)
                Priv.Add("insert", "INSERT");
            else
                Priv.Remove("insert");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (userList.SelectedItems.Count > 0)
            {
                selectedUser = userList.SelectedItem.ToString();

                Priv.Clear();
                menuPriv.Clear();
                loadedPriv.Clear();
                loadedMenuPriv.Clear();

                selectP.CheckState = CheckState.Unchecked;
                deleteP.CheckState = CheckState.Unchecked;
                createP.CheckState = CheckState.Unchecked;
                insertP.CheckState = CheckState.Unchecked;
                dropP.CheckState = CheckState.Unchecked;
                updateP.CheckState = CheckState.Unchecked;
                superP.CheckState = CheckState.Unchecked;


                booksP.CheckState = CheckState.Unchecked;
                serialsP.CheckState = CheckState.Unchecked;
                borrowP.CheckState = CheckState.Unchecked;
                returnP.CheckState = CheckState.Unchecked;
                dueP.CheckState = CheckState.Unchecked;
                studentP.CheckState = CheckState.Unchecked;
                staffP.CheckState = CheckState.Unchecked;


                //load privileges
                string statement = "SELECT privileges FROM users WHERE username='" + selectedUser + "'";

                try
                {
                    MySqlConnection con = new MySqlConnection(ConnectionString);
                    con.Open();

                    MySqlCommand com = new MySqlCommand(statement, con);
                    MySqlDataReader reader = com.ExecuteReader();

                    object[] obj = new object[1];
                    char[] sep = new char[1];
                    sep[0] = ';';
                    string[] str = new string[12];

                    while (reader.Read())
                    {
                        reader.GetValues(obj);
                        str = obj[0].ToString().Split(sep);
                    }
                    reader.Close();
                    con.Close();

                    for (int i = 0; i <= str.Length - 1; i++)
                    {
                        loadedPriv.Add(str[i], str[i]);
                    }
                }
                catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                foreach (DictionaryEntry val in loadedPriv)
                {
                    if (val.Value.ToString() == "SELECT")
                        selectP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "DELETE")
                        deleteP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "UPDATE")
                        updateP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "CREATE")
                        createP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "DROP")
                        dropP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "INSERT")
                        insertP.CheckState = CheckState.Checked;
                }



                //load MENU privileges
                statement = "SELECT menuprivileges FROM users WHERE username='" + selectedUser + "'";

                try
                {
                    MySqlConnection con = new MySqlConnection(ConnectionString);
                    con.Open();

                    MySqlCommand com = new MySqlCommand(statement, con);
                    MySqlDataReader reader = com.ExecuteReader();

                    object[] obj = new object[1];
                    char[] sep = new char[1];
                    sep[0] = ';';
                    string[] str = new string[12];

                    while (reader.Read())
                    {
                        reader.GetValues(obj);
                        str = obj[0].ToString().Split(sep);
                    }
                    reader.Close();
                    con.Close();

                    for (int i = 0; i <= str.Length - 1; i++)
                    {
                        loadedMenuPriv.Add(str[i], str[i]);
                    }
                }
                catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                foreach (DictionaryEntry val in loadedMenuPriv)
                {
                    //MessageBox.Show(val.Value.ToString());
                    if (val.Value.ToString() == "BOOKS")
                        booksP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "SERIALS")
                        serialsP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "LOANED")
                        borrowP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "RETURNED")
                        returnP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "OVERDUE")
                        dueP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "STUDENTS")
                        studentP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "STAFF")
                        staffP.CheckState = CheckState.Checked;
                    else if (val.Value.ToString() == "SUPER")
                        superP.CheckState = CheckState.Checked;
                }

                privBox.Enabled = true;
                menuPrivBox.Enabled = true;
                superbox.Enabled = true;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int ct=0;
            string privString = "", menuPrivString="";
            string statement = "REVOKE ALL PRIVILEGES ON *.* FROM '" + selectedUser + "'@'%';" +
                               "REVOKE GRANT OPTION ON *.* FROM '" + selectedUser + "'@'%';" +
                               "GRANT ";

            Priv.Clear();
            menuPriv.Clear();

            if (selectP.CheckState == CheckState.Checked)
                Priv.Add("select", "SELECT");
            if (deleteP.CheckState == CheckState.Checked)
                Priv.Add("delete", "DELETE");
            if (updateP.CheckState == CheckState.Checked)
                Priv.Add("update", "UPDATE");
            if (createP.CheckState == CheckState.Checked)
                Priv.Add("create", "CREATE");
            if (dropP.CheckState == CheckState.Checked)
                Priv.Add("drop", "DROP");
            if (insertP.CheckState == CheckState.Checked)
                Priv.Add("insert", "INSERT");
            

            if (booksP.CheckState == CheckState.Checked)
                menuPriv.Add("books", "BOOKS");
            if (serialsP.CheckState == CheckState.Checked)
                menuPriv.Add("serials", "SERIALS");
            if (borrowP.CheckState == CheckState.Checked)
                menuPriv.Add("loaned", "LOANED");
            if (returnP.CheckState == CheckState.Checked)
                menuPriv.Add("returned", "RETURNED");
            if (dueP.CheckState == CheckState.Checked)
                menuPriv.Add("overdue", "OVERDUE");
            if (studentP.CheckState == CheckState.Checked)
                menuPriv.Add("students", "STUDENTS");
            if (staffP.CheckState == CheckState.Checked)
                menuPriv.Add("staff", "STAFF");


            foreach (DictionaryEntry val in Priv)
            {
                //MessageBox.Show("Priv: " + val.Value.ToString());
                ct++;
                statement += val.Value.ToString();
                privString += val.Value.ToString() + ";";
                if (ct < Priv.Count)
                    statement += ", ";
            }

            foreach (DictionaryEntry val in menuPriv)
            {
                //MessageBox.Show("menuPriv: " + val.Value.ToString());
                menuPrivString += val.Value.ToString() + ";";
            }



            if (superP.Checked)
            {
                statement += " ON *.* TO '" + selectedUser + "'@'%'";
                statement += "; GRANT ALL PRIVILEGES ON *.* TO '" + selectedUser + "'@'%' WITH GRANT OPTION";
                statement += "; GRANT ALL PRIVILEGES ON `acu_lmis`.* TO '" + selectedUser + "'@'%'";
                menuPrivString += "SUPER";
            }
            else
            {
                statement += " ON `acu_lmis`.* TO '" + selectedUser + "'@'%'";
            }

            statement += "; GRANT SELECT ON `acu_lmis`.`loaned` TO '" + selectedUser + "'@'%'; " +
                         "GRANT INSERT, DELETE, SELECT ON `acu_lmis`.`overdue` TO '" + selectedUser + "'@'%';" +
                         "UPDATE users SET privileges = '" + privString + "', menuprivileges='" + menuPrivString + "' WHERE username = '" + selectedUser + "'";


            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                int affectedRows = com.ExecuteNonQuery();

                LoadSettings();

                Priv.Clear();
                loadedPriv.Clear();
                menuPriv.Clear();
                loadedMenuPriv.Clear();
             
                privBox.Enabled = false;
                menuPrivBox.Enabled = false;
                superbox.Enabled = false;

                MessageBox.Show("User '" + selectedUser + "' Updated Successfully!!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                con.Close();
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); } 

        }

        private void selectP_CheckedChanged(object sender, EventArgs e)
        {
            if (selectP.Checked)
                Priv.Add("select", "SELECT");
            else
                Priv.Remove("select");
        }

        private void deleteP_CheckedChanged(object sender, EventArgs e)
        {
            if (deleteP.Checked)
                Priv.Add("delete", "DELETE");
            else
                Priv.Remove("delete");
        }

        private void updateP_CheckedChanged(object sender, EventArgs e)
        {
            if (updateP.Checked)
                Priv.Add("update", "UPDATE");
            else
                Priv.Remove("update");
        }

        private void createP_CheckedChanged(object sender, EventArgs e)
        {
            if (createP.Checked)
                Priv.Add("create", "CREATE");
            else
                Priv.Remove("CREATE");
        }

        private void dropP_CheckedChanged(object sender, EventArgs e)
        {
            if (dropP.Checked)
                Priv.Add("drop", "DROP");
            else
                Priv.Remove("drop");
        }

        private void insertP_CheckedChanged(object sender, EventArgs e)
        {
            if (insertP.Checked)
                Priv.Add("insert", "INSERT");
            else
                Priv.Remove("insert");
        }


        private void button9_Click(object sender, EventArgs e)
        {

            if (userList.SelectedItems.Count > 0)
            {
                selectedUser = userList.SelectedItem.ToString();

                DialogResult r =  MessageBox.Show("Are you sure you want to delete user '" + selectedUser + "'?", "LMIS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r == DialogResult.Yes)
                {

                    string statement = "DROP USER '" + selectedUser + "'@'%'; DELETE FROM users WHERE username='" + selectedUser + "';";

                    try
                    {
                        MySqlConnection con = new MySqlConnection(ConnectionString);
                        con.Open();

                        MySqlCommand com = new MySqlCommand(statement, con);
                        int affectedRows = com.ExecuteNonQuery();

                        userList.Items.Remove(selectedUser);
                        Priv.Clear();
                        loadedPriv.Clear();
                        menuPriv.Clear();
                        loadedMenuPriv.Clear();
                        privBox.Enabled = false;
                        menuPrivBox.Enabled = false;
                        superbox.Enabled = false;

                        //MessageBox.Show("User '" + usernametxt.Text + "' Deleted Successfully!!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        con.Close();
                    }
                    catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }   
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //save due days
            string statement = "UPDATE settings SET DueDays='" + DueDaystxt.Text + "'";

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                com.ExecuteNonQuery();

                con.Close();
                MessageBox.Show("SETTING SAVED!!");
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void booksPriv_CheckedChanged(object sender, EventArgs e)
        {
            if (booksPriv.Checked)
                menuPriv.Add("BOOKS", "BOOKS");
            else
                menuPriv.Remove("BOOKS");
        }

        private void serialsPriv_CheckedChanged(object sender, EventArgs e)
        {
            if (serialsPriv.Checked)
                menuPriv.Add("SERIALS", "SERIALS");
            else
                menuPriv.Remove("SERIALS");
        }

        private void borrowPriv_CheckedChanged(object sender, EventArgs e)
        {
            if (borrowPriv.Checked)
                menuPriv.Add("LOANED", "LOANED");
            else
                menuPriv.Remove("LOANED");
        }

        private void returnPriv_CheckedChanged(object sender, EventArgs e)
        {
            if (returnPriv.Checked)
                menuPriv.Add("RETURNED", "RETURNED");
            else
                menuPriv.Remove("RETURNED");
        }

        private void duePriv_CheckedChanged(object sender, EventArgs e)
        {
            if (duePriv.Checked)
                menuPriv.Add("OVERDUE", "OVERDUE");
            else
                menuPriv.Remove("OVERDUE");
        }

        private void studentPriv_CheckedChanged(object sender, EventArgs e)
        {
            if (studentPriv.Checked)
                menuPriv.Add("STUDENTS", "STUDENTS");
            else
                menuPriv.Remove("STUDENTS");
        }

        private void staffPriv_CheckedChanged(object sender, EventArgs e)
        {
            if (staffPriv.Checked)
                menuPriv.Add("STAFF", "STAFF");
            else
                menuPriv.Remove("STAFF");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //save auto_refresh setting
            string statement = "UPDATE settings SET auto_refresh='" + auto_refresh_string + auto_time.Text + "'";

            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);
                com.ExecuteNonQuery();

                con.Close();
                MessageBox.Show("SETTING SAVED!!");
            }
            catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                auto_refresh_string = "YES;";
                auto_time.Enabled = true;
            }
            else
            {
                auto_refresh_string = "NO;";
                auto_time.Enabled = false;
            }
        }

    }
}
