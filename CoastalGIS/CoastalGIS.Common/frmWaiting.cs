using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CoastalGIS.Common
{
    public partial class frmWaiting : Form
    {
        public string WaitingLabel 
        {
            set { this.label.Text = value; }
        }

        public frmWaiting()
        {
            InitializeComponent();
        }

    }
}