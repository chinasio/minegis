using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CoastalGIS.SpatialDataBase
{
    public partial class frmCheckName : Form
    {
        string m_name = "";
        private string m_checkedName = "";
        public string CheckedName
        {
            get { return m_checkedName; }
        }

        public frmCheckName(string name)
        {
            InitializeComponent();
            m_name = name;
        }

        private void frmCheckName_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = m_name;
            this.textBox1.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_checkedName = this.textBox1.Text;
            this.Close();
        }
    }
}