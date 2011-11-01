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
        private OleDbCommand m_oraCmd = null;
        private string m_ds = "";
        private string path = "";
        private delegate void DoWork();
        Common.frmWaiting frm = new CoastalGIS.Common.frmWaiting();

        string m_type = "";
        string m_place = "";
        string m_time = "";
        string m_interType = "";

        public frmSHPToSDE(GDBConenction gcon,OleDbCommand oraCmd)
        {
            InitializeComponent();
            m_workSpace = gcon.OpenSDEWorkspace();
            m_gcon = gcon;
            m_gdata = new GDBData(m_workSpace);
            m_oraCmd = oraCmd;
            m_gdata.OraCmd = this.m_oraCmd;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "选择数据";
            openFileDialog1.Filter = "shapfile文件|*.shp";
            openFileDialog1.FilterIndex = 0;
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                this.textBox1.Text = openFileDialog1.FileName.ToString();
                path = openFileDialog1.FileName.ToString();
            }          
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() == "") 
            {
                MessageBox.Show("请先加载数据！","提示");
                return;
            }
            if (this.cmbDS.SelectedItem==null) 
            {
                MessageBox.Show("请选择数据大类！", "提示");
                return;
            }

            m_type = this.cmbDS.SelectedItem.ToString();

            if (m_type=="解译数据")
            {
                if (this.comboBox2.SelectedIndex == -1) 
                {
                    MessageBox.Show("请选择年份！", "提示");
                    return;
                }
                m_time = this.comboBox2.SelectedItem.ToString();

                if(this.comboBox1.SelectedIndex==-1&&this.textBox2.Text=="")
                {
                    MessageBox.Show("请选择矿区！", "提示");
                    return;
                }

                if (this.textBox2.Text != "")
                {
                    this.m_place = this.textBox2.Text;
                }
                else 
                {
                    this.m_place = this.comboBox1.SelectedItem.ToString();
                }

                if(this.comboBox3.SelectedIndex==-1)
                {
                    MessageBox.Show("请选择解译类型！", "提示");
                    return;
                }
                this.m_interType = this.comboBox3.SelectedItem.ToString();
                
            }


            frm.Show();
            Importshp();

        }


        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
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
            IFeatureWorkspace feaworkspace;
            FileInfo fileInfo;
            string path;
            string name;
            string shapetype = "";

            if (this.m_type == "解译数据")
            {
                try
                {
                    fileInfo = new FileInfo(this.path);
                    string fpath = fileInfo.DirectoryName;
                    string fname = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf('.'));
                    //UIWaiting("正在导入"+name+"层");
                    frm.WaitingLabel = "正在导入" + fname + "层";
                    Application.DoEvents();

                    feaworkspace = m_gcon.OpenSHPWorkspace(fpath);
                    IFeatureClass feaFC = feaworkspace.OpenFeatureClass(fname);
                    if (feaFC.FeatureType == esriFeatureType.esriFTSimple)
                    {
                        IFeatureWorkspace feaworkspaceSDE = this.m_workSpace as IFeatureWorkspace;
                        Application.DoEvents();
                        m_gdata.ConvertFeatureClassToGDB((IWorkspace)feaworkspace, (IWorkspace)feaworkspaceSDE, fname, this.m_place + "_" + this.m_interType + "_" + this.m_time, null);
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
                        m_gdata.InsertInterpData(shapetype, this.m_place, this.m_time, this.m_interType);
                        Application.DoEvents();
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
            }
            else 
            {
                try
                {
                    fileInfo = new FileInfo(this.path);
                    string fpath = fileInfo.DirectoryName;
                    string fname = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf('.'));
                    //UIWaiting("正在导入"+name+"层");
                    frm.WaitingLabel = "正在导入" + fname + "层";
                    Application.DoEvents();

                    feaworkspace = m_gcon.OpenSHPWorkspace(fpath);
                    IFeatureClass feaFC = feaworkspace.OpenFeatureClass(fname);
                    if (feaFC.FeatureType == esriFeatureType.esriFTSimple)
                    {
                        IFeatureWorkspace feaworkspaceSDE = this.m_workSpace as IFeatureWorkspace;
                        Application.DoEvents();
                        m_gdata.ConvertFeatureClassToGDB((IWorkspace)feaworkspace, (IWorkspace)feaworkspaceSDE, fname, fname, null);
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
                        m_gdata.InsertMataData(fname, shapetype, "地理数据", "1","");
                        Application.DoEvents();
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

        private void cmbDS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbDS.SelectedItem.ToString() == "解译数据")
            {
                this.comboBox2.Enabled = true;
                this.comboBox1.Enabled = true;
                this.checkBox1.Enabled = true;
                this.comboBox3.Enabled = true;
            }
            else 
            {
                this.comboBox2.Enabled = false;
                this.comboBox1.Enabled = false;
                this.checkBox1.Enabled = false;
                this.comboBox3.Enabled = false;
            }
        }

        private void frmSHPToSDE_Load(object sender, EventArgs e)
        {
            m_oraCmd.CommandText = "select distinct [PLACE] from interpdata";
            OleDbDataReader dr = m_oraCmd.ExecuteReader();

            while(dr.Read())
            {
                this.comboBox1.Items.Add(dr.GetValue(0).ToString());
            }
            dr.Close();
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == true)
            {
                this.textBox2.Enabled = true;
                this.comboBox1.Enabled = false;
            }
            else 
            {
                this.textBox2.Enabled = false;
                this.comboBox1.Enabled = true;
            }
        }
    }
}