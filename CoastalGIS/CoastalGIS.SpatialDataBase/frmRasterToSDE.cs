using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;

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
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {
                    this.listBox1.Items.Add(openFileDialog1.FileNames[i]);
                    m_name.Add(openFileDialog1.FileNames[i]);
                }
            } 
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItems.Count > 0)
            {
                int i = this.listBox1.SelectedIndex;
                this.listBox1.Items.Remove(this.listBox1.SelectedItem);
                this.m_name.RemoveAt(i);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
        }

        private void frmRasterToSDE_Load(object sender, EventArgs e)
        {

            this.SetDesktopLocation(205, 160);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count == 0)
            {
                MessageBox.Show("请先加载数据！");
                return;
            }
            if (this.cmbDS.SelectedItem == null)
            {
                MessageBox.Show("请选择卫星名称！");
                return;
            }

            if (this.comboBox1.SelectedItem == null)
            {
                MessageBox.Show("请选择年份！");
                return;
            }

            string errorMessage;
            for (int i = 0; i < this.m_name.Count; i++)
            {
                errorMessage = CommonFunc.NameCheck(System.IO.Path.GetFileNameWithoutExtension(this.m_name[i].ToString()));
                if (errorMessage != "true")
                {
                    MessageBox.Show(errorMessage + "，请修改！", "提示");
                    return;
                }
            }

            this.m_count = this.listBox1.Items.Count;
            this.m_ds = this.cmbDS.Text.ToString();
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
            try 
            {
                IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactoryClass();
                IRasterWorkspace rasterWorkspace = null;
                IRasterDataset rasterDataset = null;
                for (int i = 0; i < this.m_count; i++) 
                {
                    UIWaiting("正在导入" + System.IO.Path.GetFileName(this.m_name[i].ToString()) + "层");
                    Application.DoEvents();
                    if (m_gdata.CheckSheetName(System.IO.Path.GetFileName(this.m_name[i].ToString()),"2")) 
                    {
                        MessageBox.Show("已存在名为 " + System.IO.Path.GetFileName(this.m_name[i].ToString()) + " 的数据，请修改名称", "提示");
                        continue;
                    }
                    rasterWorkspace = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(this.m_name[i].ToString()), 0) as IRasterWorkspace;
                    rasterDataset = rasterWorkspace.OpenRasterDataset(System.IO.Path.GetFileName(this.m_name[i].ToString()));
                    m_gdata.ImportRas(rasterDataset, System.IO.Path.GetFileNameWithoutExtension(this.m_name[i].ToString()));
                    //File.Copy(this.m_name[i].ToString(), Application.StartupPath + "\\temp\\" + System.IO.Path.GetFileName(this.m_name[i].ToString()));
                    m_gdata.InsertMataData(System.IO.Path.GetFileName(this.m_name[i].ToString()), m_ds, this.comboBox1.Text.ToString(),"2");
                    Application.DoEvents();
                }
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





    }
}