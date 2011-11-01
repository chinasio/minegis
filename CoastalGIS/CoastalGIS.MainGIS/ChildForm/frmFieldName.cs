using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;

namespace CoastalGIS.MainGIS
{
    public partial class frmField : Form
    {
        private IMapControlDefault m_mapControl;
        private Janus.Windows.UI.StatusBar.UIStatusBar m_statusBar;

        public frmField(IMapControlDefault mapControl, Janus.Windows.UI.StatusBar.UIStatusBar statusBar)
        {
            InitializeComponent();
            m_mapControl = mapControl;
            m_statusBar = statusBar;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TextToShapefile pTextToPoint = new TextToShapefile(m_mapControl, m_statusBar);
            this.Close();
            pTextToPoint.TextToPoint(textBox1.Text.Trim(),this.comboBox1.SelectedItem.ToString());
            
        }
    }
}