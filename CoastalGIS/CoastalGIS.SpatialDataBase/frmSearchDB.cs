using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;

namespace CoastalGIS.SpatialDataBase
{
    public partial class frmSearchDB : Form
    {
        private GDBConenction m_gcon = null;
        private IWorkspace m_workSpace = null;
        private GDBData m_gdata = null;
        private bool m_isSearched = false;
        private IMapControlDefault m_mapControl;
        private OleDbCommand m_oraCmd = null;

        public frmSearchDB(GDBConenction gcon, IMapControlDefault mapControl, OleDbCommand oraCmd)
        {
            InitializeComponent();

            m_workSpace = gcon.OpenSDEWorkspace();
            m_gcon = gcon;
            m_gdata = new GDBData(m_workSpace);
            m_mapControl = mapControl;
            m_oraCmd = oraCmd;

        }



        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (this.txtName.Text == "") 
            {
                MessageBox.Show("请输入要查找的数据的名称！");
                return;
            }

            if (this.lstSearch.Items.Count > 0) 
            {
                this.lstSearch.Items.Clear();
            }


            string sqlText="";


            switch (this.cmbType.SelectedIndex)
            {
                case 0:
                    sqlText = "select VECTORNAME from VECTORMETADATA where VECTORNAME like '%" + this.txtName.Text.ToString() + "%'";

                    break;
                case 1:
                    sqlText = "select IMAGENAME from IMAGEMETADATA where IMAGENAME like '%" + this.txtName.Text.ToString() + "%'";
                    break;
            }


            //MessageBox.Show(sqlText);
            this.m_oraCmd.CommandText = sqlText;
            OleDbDataReader dr = this.m_oraCmd.ExecuteReader();
            ListViewItem item = null;
            while (dr.Read()) 
            {
                item = new ListViewItem(dr.GetValue(0).ToString());
                //item.SubItems.Add(dr.GetValue(1).ToString());
                //item.SubItems.Add(dr.GetValue(2).ToString());
                this.lstSearch.Items.Add(item);
            }

            if (this.lstSearch.Items.Count == 0)
            {
                MessageBox.Show("无查询结果！");
            }
            else
            {
                this.m_isSearched = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.lstSearch.Items.Clear();
            this.txtName.Text = "";
            this.cmbType.SelectedIndex = -1;
            this.m_isSearched = false;
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            //if (this.m_isSearched == false) 
            //{
            //    return;
            //}

            //foreach (ListViewItem item in this.lstSearch.Items)
            //{
            //    if (item.Checked == true) 
            //    {
            //        if (item.SubItems[1].Text.ToString() == "矢量") 
            //        {
            //            IFeatureLayer feaLyr;
            //            feaLyr = m_gdata.AddFeatureClassToMap(item.SubItems[0].Text.ToString());
            //            this.m_mapControl.Map.AddLayer(feaLyr);
            //        }
            //        if (item.SubItems[1].Text.ToString() == "影像")
            //        {
            //            IRasterWorkspaceEx rasterWS = m_workSpace as IRasterWorkspaceEx;
            //            IRasterDataset rasterDS = rasterWS.OpenRasterDataset("COASTALGIS." + item.SubItems[0].Text.ToString());
            //            IRasterLayer rasterLayer = new RasterLayerClass();
            //            rasterLayer.CreateFromDataset(rasterDS);
            //            this.m_mapControl.Map.AddLayer(rasterLayer);
            //        }
            //    }
            //}  
        }

        private void btnClearMap_Click(object sender, EventArgs e)
        {
            if (this.m_mapControl.Map == null)
                return;

            IMap map = m_mapControl.Map;
            for (int i = 0; i < map.LayerCount; i++)
            {
                map.DeleteLayer(map.get_Layer(i));

            }
            this.m_mapControl.ActiveView.Refresh();
        }

        private void frmSearchDB_Load(object sender, EventArgs e)
        {
            this.SetDesktopLocation(205, 160);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == true)
            {
                this.cmbType.Enabled = true;
            }
            else 
            {
                this.cmbType.Enabled = false;
            }
        }

    }
}