using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CoastalGIS.ExportMapProj
{
    public partial class frmSetResolution : Form
    {
        public long m_Resolution = 1;
        private long m_DefaultValue = 1;

        public frmSetResolution(long defaultResolution)
        {
            InitializeComponent();
            m_DefaultValue = defaultResolution;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_Resolution = m_DefaultValue;
            this.Close();
        }

        private void frmSetResolution_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = m_DefaultValue;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            m_Resolution = long.Parse(numericUpDown1.Value.ToString());
            this.Close();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            m_Resolution = long.Parse(numericUpDown1.Value.ToString());
        }
    }
}