using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;

namespace CoastalGIS.MainGIS
{
    public partial class OverLayerForm : Form
    {
        private IMapControlDefault m_mapControl = null;
        //private frmMain ParentForm;
        //private IMap pMap;
        //private IActiveView pActiveview;

        public OverLayerForm(IMapControlDefault mapControl)
        {
            InitializeComponent();
            //ParentForm = pMainForm;
            //pMap = ParentForm.mapCtlMain.ActiveView.FocusMap;
            //pActiveview = ParentForm.mapCtlMain.ActiveView;
            this.m_mapControl = mapControl;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFD = new SaveFileDialog();
            SaveFD.Filter = "矢量数据(*.shp)|*.shp";
            SaveFD.Title = "保存文件";
            //FileInfo path;
            //string n;
            if (SaveFD.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = SaveFD.FileName;
                btnOK.Enabled = true;
            }
            //FileInfo path = new FileInfo(textBox1.Text);
            //textBox1.Text = path.Directory.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            FileInfo openpath = new FileInfo(textBox1.Text);
            string ss = openpath.Directory.ToString();//路径的父目录

            ILayer pLayer;
            pLayer = m_mapControl.Map.get_Layer(comboBox1.SelectedIndex);
            IFeatureLayer pInputFeatureLayer;
            pInputFeatureLayer = pLayer as IFeatureLayer;
            IFeatureClass pInputFeatureClass = pInputFeatureLayer.FeatureClass;
            

            ITable pInputTable;
            pInputTable = pInputFeatureClass as ITable;

            
            pInputFeatureClass = pInputFeatureLayer.FeatureClass;

            pLayer = m_mapControl.Map.get_Layer(comboBox2.SelectedIndex);
            IFeatureLayer pOverlayFeatureLayer;
            pOverlayFeatureLayer = pLayer as IFeatureLayer;
            IFeatureClass pOverlayFeatureClass;
            pOverlayFeatureClass = pOverlayFeatureLayer.FeatureClass;

            ITable pOverlayTable;
            pOverlayTable = pOverlayFeatureClass as ITable;


            //IFeatureClass pOverlayFeatureClass;
            //pOverlayFeatureClass = pOverlayFeatureLayer.FeatureClass;

            if (pInputTable == null)
            {
                MessageBox.Show("输入图层为空");
                return;
            }

            if (pOverlayTable == null)
            {
                MessageBox.Show("叠置图层为空");
                return;
            }

            IFeatureClassName pFeatureClassName;
            pFeatureClassName = new FeatureClassNameClass();
            pFeatureClassName.FeatureType = esriFeatureType.esriFTSimple;
            pFeatureClassName.ShapeFieldName = "Shape";
            pFeatureClassName.ShapeType = pInputFeatureClass.ShapeType;

            IWorkspaceName pNewWSName;
            pNewWSName = new WorkspaceNameClass();
            pNewWSName.WorkspaceFactoryProgID = "esriDataSourcesFile.ShapefileWorkspaceFactory";
            pNewWSName.PathName = ss;

            IDatasetName pDatasetName;
            pDatasetName = pFeatureClassName as IDatasetName;
            pDatasetName.Name = openpath.Name;  //"Intersect_Result";
            pDatasetName.WorkspaceName = pNewWSName;

            double tol = 0.1;

            IBasicGeoprocessor pBGP;
            pBGP = new BasicGeoprocessorClass();
            pBGP.SpatialReference = m_mapControl.Map.SpatialReference;
            IFeatureClass pOutputFeatureClass;
            IFeatureLayer pOutputFeatLayer;
            pOutputFeatLayer = new FeatureLayerClass();
            int n = listBox1.SelectedIndex;
            
            if (n>= 0 && n< 3)
            {
                switch (n)
                {
                    case(0):
                        pOutputFeatureClass = pBGP.Intersect(pInputTable, false, pOverlayTable, false, tol, pFeatureClassName);
                        pOutputFeatLayer.FeatureClass = pOutputFeatureClass;
                        pOutputFeatLayer.Name = pOutputFeatureClass.AliasName;
                        m_mapControl.Map.AddLayer(pOutputFeatLayer);
                        break;
                    case(1):
                        pOutputFeatureClass = pBGP.Union(pInputTable, false, pOverlayTable, false, tol, pFeatureClassName);
                        pOutputFeatLayer.FeatureClass = pOutputFeatureClass;
                        pOutputFeatLayer.Name = pOutputFeatureClass.AliasName;
                        m_mapControl.Map.AddLayer(pOutputFeatLayer);
                        break;
                    case(2):
                        pOutputFeatureClass = pBGP.Clip(pInputTable, false, pOverlayTable, false, tol, pFeatureClassName);
                        pOutputFeatLayer.FeatureClass = pOutputFeatureClass;
                        pOutputFeatLayer.Name = pOutputFeatureClass.AliasName;
                        m_mapControl.Map.AddLayer(pOutputFeatLayer);
                        break;
                }
            }
            this.Dispose();
        }

        private void OverLayerForm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            if (m_mapControl.Map.LayerCount != 0)//添加layer的名称到checkedbox
            {
                for (int i = 0; i < m_mapControl.Map.LayerCount; i++)
                {
                    ILayer pLayer = m_mapControl.Map.get_Layer(i);
                    comboBox1.Items.Add(pLayer.Name);
                    comboBox2.Items.Add(pLayer.Name);
                }
            }
        }




    }
}