using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Configuration;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.Geoprocessor;

using CoastalGIS.Common;

namespace CoastalGIS.SpatialDataBase
{
    public partial class frmRasterToSDE : Form
    {
        private GDBConenction m_gcon = null;
        private GDBData m_gdata = null;
        private IWorkspace m_workSpace = null;
        private LoadRasterToSDE m_loadRaster = null;
        private int m_count = 0;
        private IList<string> m_name = null;
        private string m_ds = null;
        IRasterCatalog rasterCat = null;
        private delegate void DoWork();
        private OleDbCommand m_oraCmd=null;
        Common.frmWaiting frm = new CoastalGIS.Common.frmWaiting();

        private string m_place="";
        private string m_time="";
        private string m_satelite="";

        private string m_FGDB= ConfigurationManager.AppSettings["FGDBPath"];      

        public frmRasterToSDE(GDBConenction gcon,OleDbCommand oraCmd)
        {
            InitializeComponent();
            m_workSpace = gcon.OpenSDEWorkspace();
            m_gcon = gcon;
            m_gdata = new GDBData(m_workSpace);
            m_loadRaster = new LoadRasterToSDE();
            m_name = new List<string>();
            this.m_oraCmd = oraCmd;
            m_gdata.OraCmd = this.m_oraCmd;

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "选择数据";
            openFileDialog1.Filter = "影像图像(*.Tiff)|*.tif|JPEG(*.jpg)|*.jpg|IMG(*.img)|*.img";
            openFileDialog1.FilterIndex = 0;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = openFileDialog1.FileName;
            } 
        }

        private void frmRasterToSDE_Load(object sender, EventArgs e)
        {

            this.SetDesktopLocation(205, 160);
            m_oraCmd.CommandText = "select distinct [PLACE] from IMAGEMETADATA";
            OleDbDataReader dr = m_oraCmd.ExecuteReader();

            while (dr.Read())
            {
                this.comboBox2.Items.Add(dr.GetValue(0).ToString());
            }
            dr.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim()=="")
            {
                MessageBox.Show("请先加载数据！");
                return;
            }
            if (this.cmbDS.SelectedItem == null)
            {
                MessageBox.Show("请选择卫星名称！");
                return;
            }
            this.m_satelite = this.cmbDS.SelectedItem.ToString();

            if (this.comboBox1.SelectedItem == null)
            {
                MessageBox.Show("请选择年份！");
                return;
            }
            this.m_time = this.comboBox1.SelectedItem.ToString();

            if (this.comboBox2.SelectedIndex == -1 && this.textBox2.Text.Trim() == "")
            {
                MessageBox.Show("请选择矿区！", "提示");
                return;
            }

            if (this.textBox2.Text.Trim() != "")
            {
                this.m_place = this.textBox2.Text.Trim();
            }
            else 
            {
                this.m_place = this.comboBox2.SelectedItem.ToString();
            }

            frm.Show();
            frm.WaitingLabel = "正在导入影像数据";
            Application.DoEvents();
            ImportRaster();
            /*Thread myThread;
            myThread = new Thread(new ThreadStart(ImportThread));
            myThread.IsBackground = true;
            myThread.Start();*/
        }

        private void ImportRaster() 
        {
            Geoprocessor gp = new Geoprocessor();//定义处理对象
            gp.OverwriteOutput = true;//覆盖输出
            RasterToGeodatabase pRasterToGeodatabase = new RasterToGeodatabase();

            try 
            {
                UIWaiting("正在导入" + this.textBox1.Text + "层");
                Application.DoEvents();

                FileInfo fileInfo = new FileInfo(this.textBox1.Text.ToString());
                string fpath = fileInfo.DirectoryName;
                string fname = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf('.'));

                pRasterToGeodatabase.Input_Rasters = this.textBox1.Text;
                pRasterToGeodatabase.Output_Geodatabase = m_FGDB;
                gp.Execute(pRasterToGeodatabase, null);

                IRasterWorkspaceEx rasterWS = m_workSpace as IRasterWorkspaceEx;
                IRasterDataset rasterDS = rasterWS.OpenRasterDataset(fname);
                IDataset ds = rasterDS as IDataset;
                ds.Rename(this.m_place+"_"+this.m_satelite+"_"+this.m_time);


                m_gdata.InsertMataData(this.m_place + "_" + this.m_satelite + "_" + this.m_time, m_satelite, this.m_time, "2",this.m_place);
                Application.DoEvents();

                MessageBox.Show("入库成功！","提示");
                frm.Close();
                this.Close();
                UIWaiting("");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                return;
            }
        }

        private void ImportThread()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DoWork(ImportRaster));
            }
            else
            {
                this.ImportRaster();
            }
        }

        private void UIWaiting(string status)
        {
            System.EventArgs e = new MyProgressEvents(status);
            object[] pList = { this, e };
            BeginInvoke(new MyProgressEventsHandler(UpdateUI), pList);
        }

        private delegate void MyProgressEventsHandler(object sender, MyProgressEvents e);

        private class MyProgressEvents : EventArgs
        {
            public string Status;
           // public bool Finish;
            public MyProgressEvents(string status)
            {
                Status = status;
                //Finish = finish;
            }
        }

        private void UpdateUI(object sender, MyProgressEvents e)
        {
            try
            {
                if (e.Status != "")
                {
                    frm.WaitingLabel = e.Status;
                }
                else
                {
                    frm.Close();
                    this.Close();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == true)
            {
                this.textBox2.Enabled = true;
                this.comboBox2.Enabled = false;
            }
            else 
            {
                this.textBox2.Enabled = false;
                this.comboBox2.Enabled = true;
            }
        }





    }
}