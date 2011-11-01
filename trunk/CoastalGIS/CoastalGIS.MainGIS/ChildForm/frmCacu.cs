using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Analyst3DTools;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;

namespace CoastalGIS.MainGIS
{
    public partial class frmCacu : Form
    {
        private IMapControlDefault m_mapControl = null;
        private ITin m_tin = null;
        private IFeatureClass m_feaC=null;

        public frmCacu(IMapControlDefault mapControl,IFeatureClass feaC)
        {
            InitializeComponent();
            this.m_mapControl = mapControl;
            m_feaC=feaC;

        }

        private void frmCacu_Load(object sender, EventArgs e)
        {
            if (this.m_mapControl != null) 
            {
                for (int i = 0; i < m_mapControl.LayerCount; i++) 
                {
                    if (m_mapControl.get_Layer(i) is ITinLayer) 
                    {
                        this.comboBox1.Items.Add(m_mapControl.get_Layer(i).Name.ToString());
                    }
                }
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedItem.ToString() != "")
            {
                for (int i = 0; i < m_mapControl.LayerCount; i++)
                {
                    if (m_mapControl.get_Layer(i).Name == this.comboBox1.SelectedItem.ToString())
                    {
                        m_tin = ((ITinLayer)m_mapControl.get_Layer(i)).Dataset;
                        this.label6.Text = m_tin.Extent.ZMin.ToString() + "——" + m_tin.Extent.ZMax.ToString()+"米";
                    }
                }
            }
            else 
            {
                m_tin = null;
                this.label6.Text = "";
            }
            this.label7.Text = "";
            this.label8.Text = "";
            this.textBox1.Text = "0";
            this.textBox2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedItem.ToString() == "") 
            {
                MessageBox.Show("请选择有效图层！");
                return;
            }
            if (this.textBox1.Text == "")
            {
                MessageBox.Show("请输入相对高程下限！");
                return;
            }
            if (this.textBox2.Text == "")
            {
                MessageBox.Show("请输入相对高程上限！");
                return;
            }
            if (this.textBox1.Text != "")
            {
                if (!IsNumeric(this.textBox1.Text.ToString())) 
                {
                    MessageBox.Show("请输入数字！");
                    return;
                }
            }
            if (this.textBox2.Text != "")
            {
                if (!IsNumeric(this.textBox1.Text.ToString()))
                {
                    MessageBox.Show("请输入数字！");
                    return;
                }
            }
            IWorkspaceFactory m_workspaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace featureWorkspce = m_workspaceFactory.OpenFromFile(Application.StartupPath + "\\tempSHP", 0) as IFeatureWorkspace;
            IFeatureClass feaC = featureWorkspce.OpenFeatureClass("test");

            if (this.textBox1.Text == "0")
            {
                IFeatureCursor feaCur = feaC.Search(null, true);
                IFeature pFeature = feaCur.NextFeature();
                IArea area = pFeature.Shape as IArea;
                double areaD = Math.Abs(area.Area)/3;
                this.label11.Text = areaD.ToString("#######") + "平方米";
                while (pFeature != null)
                {
                    pFeature.set_Value(2, this.m_tin.Extent.ZMin + Convert.ToDouble(this.textBox2.Text.ToString()));
                    pFeature.Store();
                    pFeature = feaCur.NextFeature();
                }
                IDataset pDataset = feaC as IDataset;
                IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                IFeatureWorkspace pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(Application.StartupPath + "\\tempSHP", 0) as IFeatureWorkspace;
                IWorkspace pWorkspace = pFeatureWorkspace as IWorkspace;
                pDataset.Copy("test2", pFeatureWorkspace as IWorkspace);

                pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(Application.StartupPath + "\\tempSHP", 0) as IFeatureWorkspace;
                IFeatureClass fea2 = pFeatureWorkspace.OpenFeatureClass("test2");

                TinPolygonVolume ff = new TinPolygonVolume(m_tin, fea2, "HEIGHT");
                ff.reference_plane = "BELOW";
                Geoprocessor gp = new Geoprocessor();
                gp.Execute(ff, null);
                feaCur = fea2.Search(null, true);
                pFeature = feaCur.NextFeature();

                while (pFeature != null)
                {
                    double volume = (double)pFeature.get_Value(3);
                    this.label7.Text = volume.ToString("#######") + "立方米";
                    double sarea = (double)pFeature.get_Value(4);
                    this.label8.Text = sarea.ToString("#######") + "平方米";
                    pFeature = feaCur.NextFeature();
                }
            }
            else 
            {
                IFeatureCursor feaCur = feaC.Search(null, true);
                IFeature pFeature = feaCur.NextFeature();
                IArea area = pFeature.Shape as IArea;
                double areaD = Math.Abs(area.Area)/3;
                this.label11.Text = areaD.ToString("#######") + "平方米";
                while (pFeature != null)
                {
                    pFeature.set_Value(2, this.m_tin.Extent.ZMin + Convert.ToDouble(this.textBox2.Text.ToString()));
                    pFeature.Store();
                    pFeature = feaCur.NextFeature();
                }
                IDataset pDataset = feaC as IDataset;
                IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                IFeatureWorkspace pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(Application.StartupPath + "\\tempSHP", 0) as IFeatureWorkspace;
                IWorkspace pWorkspace = pFeatureWorkspace as IWorkspace;
                pDataset.Copy("test2", pFeatureWorkspace as IWorkspace);

                pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(Application.StartupPath + "\\tempSHP", 0) as IFeatureWorkspace;
                IFeatureClass fea2 = pFeatureWorkspace.OpenFeatureClass("test2");

                TinPolygonVolume ff = new TinPolygonVolume(m_tin, fea2, "HEIGHT");
                ff.reference_plane = "BELOW";
                Geoprocessor gp = new Geoprocessor();
                gp.Execute(ff, null);
                feaCur = fea2.Search(null, true);
                pFeature = feaCur.NextFeature();
                double volume1=0;
                double sarea1=0;
                while (pFeature != null)
                {
                    volume1 = (double)pFeature.get_Value(3);
                    //this.label7.Text = volume.ToString("#######") + "立方米";
                    sarea1 = (double)pFeature.get_Value(4);
                    //this.label8.Text = sarea.ToString("#######") + "平方米";
                    pFeature = feaCur.NextFeature();
                }

                feaCur = feaC.Search(null, true);
                pFeature = feaCur.NextFeature();
                double volume2=0;
                double sarea2=0;
                while (pFeature != null)
                {
                    pFeature.set_Value(2, this.m_tin.Extent.ZMin + Convert.ToDouble(this.textBox1.Text.ToString()));
                    pFeature.Store();
                    pFeature = feaCur.NextFeature();
                }
                pDataset = feaC as IDataset;
                pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(Application.StartupPath + "\\tempSHP", 0) as IFeatureWorkspace;
                pWorkspace = pFeatureWorkspace as IWorkspace;
                pDataset.Copy("test3", pFeatureWorkspace as IWorkspace);

                pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(Application.StartupPath + "\\tempSHP", 0) as IFeatureWorkspace;
                fea2 = pFeatureWorkspace.OpenFeatureClass("test3");

                ff = new TinPolygonVolume(m_tin, fea2, "HEIGHT");
                ff.reference_plane = "BELOW";
                gp = new Geoprocessor();
                gp.Execute(ff, null);
                feaCur = fea2.Search(null, true);
                pFeature = feaCur.NextFeature();
                while (pFeature != null)
                {
                    volume2 = (double)pFeature.get_Value(3);
                    //this.label7.Text = volume.ToString("#######") + "立方米";
                    sarea2 = (double)pFeature.get_Value(4);
                    //this.label8.Text = sarea.ToString("#######") + "平方米";
                    pFeature = feaCur.NextFeature();
                }
                double volume = volume1 - volume2;
                double sarea = sarea1 - sarea2;
                this.label7.Text = volume.ToString("#######") + "立方米";
                this.label8.Text = sarea.ToString("#######") + "平方米";

            }

            string[] files = System.IO.Directory.GetFiles(Application.StartupPath + "\\tempSHP");
            for (int i = 0; i < files.Length; i++) 
            {
                System.IO.File.Delete(files[i]);
            }
        }

        private bool IsNumeric(string str)
        {
            foreach (char c in str)
            {
                if (!Char.IsNumber(c))
                {
                    return false;
                }
            }
            return true;
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}