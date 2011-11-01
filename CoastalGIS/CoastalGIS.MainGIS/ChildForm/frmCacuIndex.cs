using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;

namespace CoastalGIS.MainGIS
{
    public partial class frmCacuIndex : Form
    {
        public frmCacuIndex()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "选择数据";
            openFileDialog1.Filter = "shapfile文件|*.shp";
            openFileDialog1.FilterIndex = 0;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = openFileDialog1.FileName;
                

            } 
        }

        private void showInfo(string path) 
        {
            IWorkspaceFactory m_workspaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace featureWorkspce = m_workspaceFactory.OpenFromFile(path, 0) as IFeatureWorkspace;
            IFeatureClass fc = featureWorkspce.OpenFeatureClass(System.IO.Path.GetFileName(path));
            IFeatureCursor cursor = fc.Search(null, false);
            IFeature pfeature = cursor.NextFeature();
            while(pfeature!=null)
            {

            }
        }
    }
}