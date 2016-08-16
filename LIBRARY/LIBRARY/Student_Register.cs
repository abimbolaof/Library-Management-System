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
using System.Net;
using System.Collections;
using System.Collections.Specialized;

namespace LIBRARY
{
    public partial class Student_Register : Form
    {
        public Student_Register()
        {
            InitializeComponent();
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            if ((modify) && (modifyval != ""))
            {
                button1.Text = "Change Photo...";
                idtxt.Text = modifyval;
                Search();
            }
        }

        bool Search()
        {
            bool res = false;
            string statement = "SELECT * FROM registration WHERE id='" + idtxt.Text + "'";
            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(statement, con);

                object[] obj = new object[15];
                char[] sep = new char[1];
                sep[0] = ' ';
                string[] str = new string[3];
                //byte[] data;
                //int fsize=0;


                MySqlDataReader reader = com.ExecuteReader();
                reader.Read();


                reader.GetValues(obj);

                idtxt.Text = obj[0].ToString();

                str = obj[1].ToString().Split(sep);

                surnametxt.Text = str[0];

                firstnametxt.Text = str[1];

                othernamestxt.Text = str[2];

                departmenttxt.Text = obj[2].ToString();

                facultytxt.Text = obj[3].ToString();

                programmetxt.Text = obj[4].ToString();

                leveltxt.Text = obj[5].ToString();

                roomtxt.Text = obj[6].ToString();

                halltxt.Text = obj[7].ToString();

                addresstxt.Text = obj[8].ToString();

                phone1txt.Text = obj[9].ToString();

                phone2txt.Text = obj[10].ToString();

                emailtxt.Text = obj[11].ToString();

                temp = Application.StartupPath + "/cache/userphoto.jpg";

                FileSize = reader.GetInt32(reader.GetOrdinal("photo_size"));

                image = new byte[FileSize];

                reader.GetBytes(reader.GetOrdinal("photo"), 0, image, 0, FileSize);

                reader.Close();
                con.Close();

                FileStream fs = new FileStream(temp, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(image, 0, FileSize);
                fs.Close();

            }
            catch (Exception f) { MessageBox.Show(f.Message + ":search:" + f.Source, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return res;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void idtxt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == Keys.Enter.ToString())
            {
                if (idtxt.Text != "")
                {
                    if (Search())
                    {
                        surnametxt.Enabled = true;
                        firstnametxt.Enabled = true;
                        othernamestxt.Enabled = true;
                        departmenttxt.Enabled = true;
                        facultytxt.Enabled = true;
                        programmetxt.Enabled = true;
                        leveltxt.Enabled = true;
                        roomtxt.Enabled = true;
                        halltxt.Enabled = true;
                        addresstxt.Enabled = true;
                        phone1txt.Enabled = true;
                        phone2txt.Enabled = true;
                        emailtxt.Enabled = true;
                        button1.Enabled = true;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (modify)
                {
                    string name = surnametxt.Text + " " + firstnametxt.Text + " " + othernamestxt.Text;

                    FileInfo inf = new FileInfo(temp);
                    ext = inf.Extension;
                    //MessageBox.Show(ext + ":" + temp + ":" + image.Length);

                    string statement = "DELETE FROM registration WHERE id='" + idtxt.Text + "'; INSERT INTO registration VALUES (@id, @name, @department, " +
                                           "@faculty, @programme, @level, @roomNo, @hall, @address, @phone1, " +
                                           "@phone2, @email, @photo, @photo_ext, @photo_size, @type);";


                    try
                    {
                        MySqlConnection con = new MySqlConnection(ConnectionString);
                        con.Open();

                        MySqlCommand com = new MySqlCommand(statement, con);

                        com.Parameters.AddWithValue("@id", idtxt.Text);
                        com.Parameters.AddWithValue("@name", name);
                        com.Parameters.AddWithValue("@department", departmenttxt.Text);
                        com.Parameters.AddWithValue("@faculty", facultytxt.Text);
                        com.Parameters.AddWithValue("@programme", programmetxt.Text);
                        com.Parameters.AddWithValue("@level", leveltxt.Text);
                        com.Parameters.AddWithValue("@roomNo", roomtxt.Text);
                        com.Parameters.AddWithValue("@hall", halltxt.Text);
                        com.Parameters.AddWithValue("@address", addresstxt.Text);
                        com.Parameters.AddWithValue("@phone1", phone1txt.Text);
                        com.Parameters.AddWithValue("@phone2", phone2txt.Text);
                        com.Parameters.AddWithValue("@email", emailtxt.Text);
                        com.Parameters.AddWithValue("@photo", image);
                        com.Parameters.AddWithValue("@photo_ext", ext);
                        com.Parameters.AddWithValue("@photo_size", FileSize);
                        com.Parameters.AddWithValue("@type", "student");

                        int affectedRows = com.ExecuteNonQuery();
                        con.Close();

                        this.Owner.Text += ".";
                        MessageBox.Show("Record Updated Successfully!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Close();

                    }
                    catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }

                else
                {
                    if (button2.Text == "NEW")
                    {
                        idtxt.Enabled = true;
                        idtxt.Focus();
                        surnametxt.Enabled = true;
                        firstnametxt.Enabled = true;
                        othernamestxt.Enabled = true;
                        departmenttxt.Enabled = true;
                        facultytxt.Enabled = true;
                        programmetxt.Enabled = true;
                        leveltxt.Enabled = true;
                        roomtxt.Enabled = true;
                        halltxt.Enabled = true;
                        addresstxt.Enabled = true;
                        phone1txt.Enabled = true;
                        phone2txt.Enabled = true;
                        emailtxt.Enabled = true;
                        button1.Enabled = true;
                        button2.Text = "OK";
                    }
                    else
                    {
                        FileInfo inf = new FileInfo(fname);
                        ext = inf.Extension;

                        string name = surnametxt.Text + " " + firstnametxt.Text + " " + othernamestxt.Text;

                        string statement = "INSERT INTO registration VALUES (@id, @name, @department, " +
                                           "@faculty, @programme, @level, @roomNo, @hall, @address, @phone1, " +
                                           "@phone2, @email, @photo, @photo_ext, @photo_size, @type)";

                        try
                        {
                            MySqlConnection con = new MySqlConnection(ConnectionString);
                            con.Open();

                            MySqlCommand com = new MySqlCommand(statement, con);

                            com.Parameters.AddWithValue("@id", idtxt.Text);
                            com.Parameters.AddWithValue("@name", name);
                            com.Parameters.AddWithValue("@department", departmenttxt.Text);
                            com.Parameters.AddWithValue("@faculty", facultytxt.Text);
                            com.Parameters.AddWithValue("@programme", programmetxt.Text);
                            com.Parameters.AddWithValue("@level", leveltxt.Text);
                            com.Parameters.AddWithValue("@roomNo", roomtxt.Text);
                            com.Parameters.AddWithValue("@hall", halltxt.Text);
                            com.Parameters.AddWithValue("@address", addresstxt.Text);
                            com.Parameters.AddWithValue("@phone1", phone1txt.Text);
                            com.Parameters.AddWithValue("@phone2", phone2txt.Text);
                            com.Parameters.AddWithValue("@email", emailtxt.Text);
                            com.Parameters.AddWithValue("@photo", image);
                            com.Parameters.AddWithValue("@photo_ext", ext);
                            com.Parameters.AddWithValue("@photo_size", FileSize);
                            com.Parameters.AddWithValue("@type", "student");

                            int affectedRows = com.ExecuteNonQuery();
                            con.Close();

                            this.Owner.Text += ".";
                            MessageBox.Show(affectedRows.ToString() + " Record Added Successfully!!", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            idtxt.Enabled = false;
                            surnametxt.Enabled = false;
                            firstnametxt.Enabled = false;
                            othernamestxt.Enabled = false;
                            departmenttxt.Enabled = false;
                            facultytxt.Enabled = false;
                            programmetxt.Enabled = false;
                            leveltxt.Enabled = false;
                            roomtxt.Enabled = false;
                            halltxt.Enabled = false;
                            addresstxt.Enabled = false;
                            phone1txt.Enabled = false;
                            phone2txt.Enabled = false;
                            emailtxt.Enabled = false;
                            button1.Enabled = false;
                            photobox.CreateGraphics().Clear(Color.Black);
                            button2.Text = "NEW";
                        }
                        catch (Exception f) { MessageBox.Show(f.Message, "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (modify)
            {
                change_pic = true;
            }
            open.Title = "Select Photo...";
            open.Filter = "JPEG Files (.jpg)|*.jpg|PNG Files (.png)|*.png";
            open.ShowDialog();


            if (File.Exists(open.FileName))
            {
                fname = open.FileName;
                Image im = Image.FromFile(fname);
                Graphics g = photobox.CreateGraphics();
                g.Clear(Color.Black);
                g.DrawImage(im, 0, 0, photobox.Width, photobox.Height);
                g.Dispose();
                im.Dispose();

                try
                {
                    FileStream f = new FileStream(fname, FileMode.Open, FileAccess.Read);
                    FileSize = int.Parse(f.Length.ToString());

                    image = new byte[FileSize];
                    f.Read(image, 0, FileSize); 
                    f.Close();


                    /*WebClient client = new WebClient();
                    //client.UploadData("http://localhost/photos/sed.jpg", "PUT", image);
                    client.UploadFile("http://localhost/photos/key.html", fname);
                    //client.DownloadFile("http://localhost/photos/seg.jpg", "C://seg.jpg");*/
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.Message);
                }
            }
        }

        void Show_Picture()
        {
            Image im = Image.FromFile(temp);
            Graphics g = photobox.CreateGraphics();
            g.Clear(Color.Black);
            g.DrawImage(im, 0, 0, photobox.Width, photobox.Height);
            g.Dispose();
            im.Dispose();
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (modify && modifyval != "" && change_pic == false)
            {
                Show_Picture();
            }
        }
    }
}
