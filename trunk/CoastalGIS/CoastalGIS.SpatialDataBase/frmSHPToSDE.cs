using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace CoastalGIS.SpatialDataBase
{
    public partial class frmSHPToSDE : Form
    {
        private GDBConenction m_gcon = null;
        private GDBData m_gdata = null;
        private IWorkspace m_workSpace = null;
        private IList<string> m_shpPath = null;
        private IList<string> m_shpName = null;
        private IList<string> m_shpName1 = null;
        private OleDbCommand m_oraCmd = null;
        private string m_ds = "";
        private delegate void DoWork();
        Common.frmWaiting frm = new CoastalGIS.Common.frmWaiting();

        public frmSHPToSDE(GDBConenction gcon,OleDbCommand oraCmd)
        {
            InitializeComponent();
            m_workSpace = gcon.OpenSDEWorkspace();
            m_gcon = gcon;
            m_shpPath = new List<string>();
            m_shpName = new List<string>();
            m_gdata = new GDBData(m_workSpace);
            m_shpName1 = new List<string>();
            m_oraCmd = oraCmd;
            m_gdata.OraCmd = this.m_oraCmd;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FileInfo fileInfo;
            //m_shpPath.Clear();
            //m_shpName.Clear();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "选择数据";
            openFileDialog1.Filter = "shapfile文件|*.shp";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++) 
                {
                    fileInfo = new FileInfo(openFileDialog1.FileNames[i]);
                    m_shpPath.Add(fileInfo.DirectoryName);
                    this.listBox1.Items.Add(openFileDialog1.FileNames[i]);
                    this.m_shpName.Add(System.IO.Path.GetFileNameWithoutExtension(openFileDialog1.FileNames[i].ToString()));
                }
            }          
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count > 0) 
            {
                int i = this.listBox1.SelectedIndex;
                this.listBox1.Items.Remove(this.listBox1.SelectedItem);
                this.m_shpName.RemoveAt(i);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            this.m_shpName.Clear();
            this.m_shpName1.Clear();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count == 0) 
            {
                MessageBox.Show("请先加载数据！","提示");
                return;
            }
            if (this.cmbDS.SelectedItem==null) 
            {
                MessageBox.Show("请选择数据类型！", "提示");
                return;
            }
            string errorMessage;
            for (int i = 0; i < this.m_shpName.Count; i++)
            {
                errorMessage = CommonFunc.NameCheck(this.m_shpName[i].ToString());
                if (errorMessage != "true")
                {
                    MessageBox.Show(errorMessage + "，请修改！", "提示");
                    return;
                }
            }

            this.m_ds = this.cmbDS.Text;
            for (int i = 0; i < this.listBox1.Items.Count; i++) 
            {
                this.m_shpName1.Add(this.listBox1.Items[i].ToString());
            }

            //this.Hide();
            //Importshp();
            frm.Show();
            Importshp();
            //Thread myThread;
            //myThread = new Thread(new ThreadStart(Importshp));
            //myThread.IsBackground = true;
            //myThread.Start();

        }

        private void frmSHPToSDE1_Load(object sender, EventArgs e)
        {
        }

        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex != -1)
            {
                frmCheckName frmCheckName = new frmCheckName(this.m_shpName[this.listBox1.SelectedIndex]);
                frmCheckName.ShowDialog();
                this.m_shpName[this.listBox1.SelectedIndex] = frmCheckName.CheckedName;
                this.listBox1.Refresh();
            }
        }

        private void ImportThread() 
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DoWork(Importshp));
            }
            else
            {
                this.Importshp();
            }
        }

        private void Importshp() 
        {
            //this.Hide();
            //frm.Show();
            IFeatureWorkspace feaworkspace;
            FileInfo fileInfo;
            string path;
            string name;
            string shapetype = "";
            try
            {
                for (int i = 0; i < this.m_shpName1.Count; i++)
                {
                    
                    fileInfo = new FileInfo(this.m_shpName1[i]);
                    path = fileInfo.DirectoryName;
                    name = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf('.'));
                    //UIWaiting("正在导入"+name+"层");
                    frm.WaitingLabel = "正在导入" + name + "层";
                    Application.DoEvents();

                    feaworkspace = m_gcon.OpenSHPWorkspace(path);
                    IFeatureClass feaFC = feaworkspace.OpenFeatureClass(name);
                    if (feaFC.FeatureType == esriFeatureType.esriFTSimple)
                    {
                        if (m_gdata.CheckSheetName(this.m_shpName[i].ToString(),"1")) 
                        {
                            MessageBox.Show("已存在名为 " + this.m_shpName[i].ToString() + " 的数据，请修改名称", "提示");
                            continue;
                        }
                        IFeatureWorkspace feaworkspaceSDE = this.m_workSpace as IFeatureWorkspace;
                        Application.DoEvents();
                        m_gdata.ConvertFeatureClassToGDB((IWorkspace)feaworkspace, (IWorkspace)feaworkspaceSDE, name, this.m_shpName[i].ToString(), null);
                        Application.DoEvents();
                        switch (feaFC.ShapeType)
                        {
                            case esriGeometryType.esriGeometryPoint:
                                shapetype = "point";
                                break;
                            case esriGeometryType.esriGeometryPolyline:
                                shapetype = "line";
                                break;
                            case esriGeometryType.esriGeometryPolygon:
                                shapetype = "polygon";
                                break;
                            default:
                                shapetype = "else";
                                break;
                        }
                        m_gdata.InsertMataData(this.m_shpName[i].ToString(), shapetype, m_ds, "1");
                        Application.DoEvents();
                    }
                }
                
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                return;
            }
            MessageBox.Show("导入已成功完成!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frm.Close();
            this.Close();
            //UIWaiting("");
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
            public bool Finish;
            public MyProgressEvents(string status)
            {
                Status = status;
                //Finish = finish;
            }
        }

        private void UpdateUI(object sender, MyProgressEvents e)
        {
            //this.label1.Text = e.Msg;
            //this.progressBar1.Value = e.PercentDone;

            try
            {
                if (e.Status != "")
                {
                    frm.WaitingLabel = e.Status;
                   // frm.Show();
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
    }
}