using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

using CoastalGIS.Common;

namespace CoastalGIS.MapEditing
{
    public partial class frmAdjustPoint : Form
    {
        public ListView ListPoint 
        {
            set { this.listView1 = value; }
            get { return this.listView1; }
        }

        public bool ButtonEnable 
        {
            set { this.button3.Visible=value;}
        }

        public IPointCollection FromPoint = new Multipoint();
        public IPointCollection ToPoint = new Multipoint();

        private IGeoReference m_geoRef;
        private IActiveView m_activeView;
        private IMap m_map;
        string m_fileName;
        Common.frmWaiting frmW = new frmWaiting();
        private string m_selectPath;

        //private delegate void DoWork();

        public frmAdjustPoint(IGeoReference geoRef, IActiveView activeView,IMap map,string fileName)
        {
            InitializeComponent();
            m_geoRef = geoRef;
            m_activeView = activeView;
            m_map = map;
            m_fileName = fileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (FromPoint.PointCount == 0) 
            {
                MessageBox.Show("请选择控制点对！");
                return;
            }

            ((IGraphicsContainer)m_map).DeleteAllElements();


            m_geoRef.Warp(FromPoint, ToPoint, 0);
            //m_geoRef.Register();
            m_activeView.Refresh();
            MessageBox.Show("配准完成！");
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK) 
            {
                m_selectPath = dlg.SelectedPath;
                frmW.Show();
                Thread myThread;
                myThread = new Thread(new ThreadStart(Save));
                myThread.IsBackground = true;
                myThread.Start();
            }

        }

        private void Save() 
        {
            string format = System.IO.Path.GetExtension(m_fileName);
            m_geoRef.Rectify(m_selectPath + "\\" + m_fileName + "_Rectify.tif", "TIFF");

            //frmW.Close();
            MessageBox.Show("保存成功！");
        }


        private void button2_Click(object sender, EventArgs e)
        {
            ((IGraphicsContainer)m_map).DeleteAllElements();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            FromPoint.RemovePoints(0, FromPoint.PointCount);
            ToPoint.RemovePoints(0, ToPoint.PointCount);
            int Count = 0;

            OpenFileDialog filedlg = new OpenFileDialog();
            filedlg.Title = "选择坐标文本文件";
            filedlg.Filter = "文本文件(.txt)|*.txt";
            if (filedlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string fullpath = filedlg.FileName;
            string strline = "";
            string strX0 = "";
            string strY0 = "";
            string strX1 = "";
            string strY1 = "";
            //char []split = new char
            using (StreamReader sr = new StreamReader(fullpath))
            {
                object obj = Type.Missing;
                while ((strline = sr.ReadLine()) != null)
                {
                    if (strline.Contains(",") == false || strline == "控制点" || strline.Contains("****") || strline.Trim() == "")
                    {
                        continue;
                    }
                    string[] splits = strline.Split(new char[] { ',', ';' });
                    strY0 = splits[1];
                    strX0 = splits[2];
                    strY1 = splits[3];
                    strX1 = splits[4];

                    ListViewItem newItem = new ListViewItem();
                    newItem.Checked = true;
                    Count++;//坐标个数增加
                    newItem.Text = Count.ToString();
                    newItem.SubItems.Add(strY0.Trim());
                    newItem.SubItems.Add(strX0.Trim());
                    newItem.SubItems.Add(strY1.Trim());
                    newItem.SubItems.Add(strX1.Trim());
                    listView1.Items.Add(newItem);
                    IPoint newPt = new PointClass();
                    newPt.X = double.Parse(strX0.Trim());
                    newPt.Y = double.Parse(strY0.Trim());
                    FromPoint.AddPoint(newPt, ref obj, ref obj);
                    newPt = new PointClass();
                    newPt.X = double.Parse(strX1.Trim());
                    newPt.Y = double.Parse(strY1.Trim());
                    ToPoint.AddPoint(newPt, ref obj, ref obj);
                }
            }
        }




    }
}