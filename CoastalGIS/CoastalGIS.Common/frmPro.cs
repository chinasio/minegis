using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CoastalGIS.Common
{
    public partial class frmPro : Form
    {
        public string ProLabel 
        {
            set { this.label2.Text = value; }
        }

        public int ProValue 
        {
            set 
            {
                if (value >= 0 && value <= 100) 
                {
                    this.progressBar1.Value = value; 
                }
            }
        }


        public frmPro()
        {
            InitializeComponent();
        }

        private void frmPro_Load(object sender, EventArgs e)
        {

        }
    }
}