using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CoastalGIS.MainGIS
{
    public partial class frmDis : Form
    {
        public frmDis(string surveyInfo,string name)
        {
            InitializeComponent();
            this.label1.Text = surveyInfo;
            this.officeFormAdorner1.ApplicationName = name;
            //this.label1.TextAlign = ContentAlignment.MiddleCenter;
            this.label1.Left = (this.Width - this.label1.Width) / 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}