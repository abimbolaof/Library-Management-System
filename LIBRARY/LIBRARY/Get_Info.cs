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

namespace LIBRARY
{
    public partial class Get_Info : Form
    {
        public Get_Info()
        {
            InitializeComponent();
        }

        private void Get_Info_Load(object sender, EventArgs e)
        {
            picbox.Visible = false;

            if (ssinfo)
                button1.Visible = true;

            try
            {
                //extract photo from server database
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();

                MySqlCommand com = new MySqlCommand(infoStatement, con);
                MySqlDataReader reader = com.ExecuteReader();
                int i = 0;
                string name = "", value="";

                while (reader.Read()) 
                {
                    for (int x = 0; x <= reader.FieldCount-1; x++)
                    {
                        name = reader.GetName(x);
                        value = reader.GetValue(x).ToString();

                        infotxt.Items.Add(name + ": " + value);
                    }
                    i++;
                }
                reader.Close();
                con.Close();

            }
            catch (Exception f) { MessageBox.Show(f.Message + ":search", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Show Photo")
            {
                ShowPicClick = true;
                picbox.Visible = true;
                picbox.Refresh();
                button1.Text = "Hide Photo";
            }
            else
            {
                ShowPicClick = false;
                picbox.Visible = false;
                button1.Text = "Show Photo";
            }
        }

        private void picbox_Paint(object sender, PaintEventArgs e)
        {
            if (ShowPicClick)
            {
                //Draw photo
                try
                {
                    //extract photo from server database
                    MySqlConnection con = new MySqlConnection(ConnectionString);
                    con.Open();

                    MySqlCommand com = new MySqlCommand(infoStatement, con);

                    byte[] data;
                    int fsize = 0;
                    string temp = "";

                    MySqlDataReader reader = com.ExecuteReader();
                    reader.Read();

                    temp = Application.StartupPath + "/cache/userphoto.jpg";

                    fsize = reader.GetInt32(reader.GetOrdinal("photo_size"));

                    data = new byte[fsize];

                    reader.GetBytes(reader.GetOrdinal("photo"), 0, data, 0, fsize);

                    reader.Close();
                    con.Close();

                    //save photo to local disk
                    FileStream fs = new FileStream(temp, FileMode.OpenOrCreate, FileAccess.Write);
                    fs.Write(data, 0, fsize);
                    fs.Close();

                    //draw photo in photo box
                    Image im = Image.FromFile(temp);
                    Graphics g = picbox.CreateGraphics();
                    g.Clear(Color.Black);
                    g.DrawImage(im, 0, 0, picbox.Width, picbox.Height);
                    g.Dispose();
                    im.Dispose();
                }
                catch (Exception f) { MessageBox.Show(f.Message + ":search", "LMIS", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
    }
}
