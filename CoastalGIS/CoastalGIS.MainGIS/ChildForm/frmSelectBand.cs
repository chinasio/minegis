using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

namespace CoastalGIS.MainGIS
{
    public partial class frmSelectBand : Form
    {
        IMapControlDefault m_mapCon = null;
        IRasterLayer m_rasLayer = null;
        IWorkspace m_workSpace = null;
        IRasterBandCollection m_rc = null;

        public frmSelectBand(IMapControlDefault mapCon, IRasterLayer rasLayer, IWorkspace workSpace)
        {
            InitializeComponent();
            this.m_mapCon = mapCon;
            m_rasLayer = rasLayer;
            m_workSpace = workSpace;
        }

        private void frmSelectBand_Load(object sender, EventArgs e)
        {
            this.comboBox1.Items.Clear();
            IDataLayer2 dataLayer = m_rasLayer as IDataLayer2;
            string dataSource = dataLayer.DataSourceName.NameString;
            if (dataSource.Contains("RASTER")) //文件
            {
                string path = dataSource.Substring(dataSource.IndexOf('=') + 1, dataSource.IndexOf(';') - dataSource.IndexOf('=') - 2);
                string name = dataSource.Substring(dataSource.LastIndexOf('=') + 1, dataSource.LastIndexOf(';') - dataSource.LastIndexOf('=') - 1);
                IWorkspaceFactory m_workspaceFactory = new RasterWorkspaceFactoryClass();
                IRasterWorkspace rasterWorkspce = m_workspaceFactory.OpenFromFile(path.Trim(), 0) as IRasterWorkspace;
                IRasterDataset rasDataset = rasterWorkspce.OpenRasterDataset(name.Trim());
                m_rc = rasDataset as IRasterBandCollection;
                for (int i = 0; i < m_rc.Count; i++)
                {
                    this.comboBox1.Items.Add(m_rc.Item(i).Bandname.ToString());
                }
            }
            else 
            {
                string name = dataSource;
                IRasterWorkspaceEx rasterWorkspce = m_workSpace as IRasterWorkspaceEx;
                IRasterDataset rasDataset = rasterWorkspce.OpenRasterDataset(name);
                m_rc = rasDataset as IRasterBandCollection;
                for (int i = 0; i < m_rc.Count; i++)
                {
                    this.comboBox1.Items.Add(m_rc.Item(i).Bandname.ToString());
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("请选择波段名称!");
                return;
            }

            IRasterBand rb = m_rc.Item(m_rc.get_BandIndex(this.comboBox1.SelectedItem.ToString().Trim()));
            IRasterDataset rasterDS = rb as IRasterDataset;
            IRasterLayer rasterLayer = new RasterLayerClass();
            rasterLayer.CreateFromDataset(rasterDS);
            this.m_mapCon.Map.AddLayer(rasterLayer);
        }
    }
}