using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;

using CoastalGIS.SpatialDataBase;

namespace CoastalGIS.CallMap
{
    public partial class frmCallMapByCommon : Form
    {
        private OracleCommand m_oraCmd;
        private IMapControlDefault m_mapControl;
        private GDBData m_gdata;
        private IWorkspace m_workSpace;
        public frmCallMapByCommon(OracleCommand oraCmd, IMapControlDefault mapControl, IWorkspace workSpace)
        {
            InitializeComponent();
            m_oraCmd = oraCmd;
            m_mapControl = mapControl;
            m_gdata = new GDBData(workSpace);
            m_workSpace = workSpace;
        }

        private void frmCallMapByCommon_Load(object sender, EventArgs e)
        {
            string sqlText = "select * from SHEETNOINDEX";
            m_oraCmd.CommandText = sqlText;
            OracleDataReader dr = m_oraCmd.ExecuteReader();
            while (dr.Read()) 
            {
                this.listBox1.Items.Add(dr.GetValue(0).ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItems == null) 
            {
                MessageBox.Show("请选择图幅名！");
                return;
            }

            string sqlText = "";
            for (int i = 0; i < this.listBox1.SelectedItems.Count; i++) 
            {
                sqlText = "select name,DATASTRUCTURE from sheetmetadata where NAME='" + listBox1.SelectedItems[i] + "'";
                m_oraCmd.CommandText = sqlText;
                OracleDataReader dr = m_oraCmd.ExecuteReader();
                if (!dr.HasRows) 
                {
                    MessageBox.Show("图幅不存在！");
                    return;
                }
                while (dr.Read())
                {
                   // this.listBox1.Items.Add(dr.GetValue(0).ToString());
                    if (dr.GetValue(1).ToString() == "矢量")
                    {
                        IFeatureLayer feaLyr;
                        feaLyr = m_gdata.AddFeatureClassToMap(dr.GetValue(0).ToString());
                        this.m_mapControl.Map.AddLayer(feaLyr);
                    }
                    if (dr.GetValue(1).ToString() == "影像")
                    {
                        IRasterWorkspaceEx rasterWS = m_workSpace as IRasterWorkspaceEx;
                        IRasterCatalog rasterCatalog = rasterWS.OpenRasterCatalog("COASTALGIS." + dr.GetValue(1).ToString());
                        ITable table = rasterCatalog as ITable;
                        ICursor cursor = table.Search(null, false);
                        IRow row = cursor.NextRow();
                        IRasterDataset rasterDS = null;
                        while (row != null)
                        {
                            IRasterCatalogItem rasterCatalogItem = row as IRasterCatalogItem;

                            if (dr.GetValue(0).ToString() == row.get_Value(cursor.FindField("NAME")).ToString())
                            {
                                rasterDS = rasterCatalogItem.RasterDataset;
                                break;
                            }
                            row = cursor.NextRow();
                        }
                        IRasterLayer rasterLayer = new RasterLayerClass();
                        rasterLayer.CreateFromDataset(rasterDS);
                        this.m_mapControl.Map.AddLayer(rasterLayer);
                    }
                }


            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmEditCallMap frmEdit = new frmEditCallMap(this.m_oraCmd);
            frmEdit.ShowDialog();
            this.listBox1.Items.Clear();
            string sqlText = "select * from SHEETNOINDEX";
            m_oraCmd.CommandText = sqlText;
            OracleDataReader dr = m_oraCmd.ExecuteReader();
            while (dr.Read())
            {
                this.listBox1.Items.Add(dr.GetValue(0).ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}