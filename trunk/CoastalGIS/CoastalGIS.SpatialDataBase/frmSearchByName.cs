using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CoastalGIS.SpatialDataBase
{
    public partial class frmSearchByName : Form
    {

        public string Name="";

        public frmSearchByName()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Name = this.textBox1.Text.Trim();
            this.Close();
        }
    }
}