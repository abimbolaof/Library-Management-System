using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LIBRARY
{
    public partial class Report_Settings : Form
    {
        public Report_Settings()
        {
            InitializeComponent();
        }

        private void Report_Settings_Load(object sender, EventArgs e)
        {

        }

        void Report_Settings_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            Report rep = new Report();
            rep.startDate = fromDate.Text;
            rep.endDate = toDate.Text;
            rep.Show();
        }
    }
}
