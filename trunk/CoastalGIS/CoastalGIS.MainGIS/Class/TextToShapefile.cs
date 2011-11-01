using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Collections;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SampleTools;
using ESRI.ArcGIS.Geoprocessor;

using Janus.Windows.UI;
using Janus.Windows.UI.StatusBar;
using Janus.Windows.Common;

using CoastalGIS.Common;

namespace CoastalGIS.MainGIS
{
    class TextToShapefile
    {
        //ע�⣺ʹ��չ�㹦�ܵ�ʱ�򣬵㡢�ߡ��������ö��š��ո񡢷ֺŷָ���������
        //���ǣ��ߺ�������н�����־����END��������ֵ���ŵ�
        //����չ��Ŀ϶��ǵ�ͼ�ϵĵ�һ��ͼ��
        private UIStatusBar m_statusBar;
        private IMapControlDefault m_mapControl;
        private IFeatureLayer m_FeatureLayer;
        string LocalFilePath = "";
        private Common.frmWaiting frmW = null;

        public TextToShapefile(IMapControlDefault mapControl, UIStatusBar pStatusBar)
        {
            m_mapControl = mapControl;
            IMap pMap = m_mapControl.ActiveView.FocusMap;
            m_FeatureLayer = pMap.get_Layer(0) as IFeatureLayer;
            m_statusBar = pStatusBar;
            frmW = new frmWaiting();
        }

        /// <summary>
        /// �����ļ�չ���ͼ��
        /// </summary>
        public void TextToPoint()
        {
            if (m_FeatureLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
            {
                MessageBox.Show("��ȷ��չ��ĵ�һ��ͼ��Ϊ��ͼ��", "��ʾ��Ϣ", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Title = "��ѡ��չ���ͼ��������ļ�";
            openfileDialog.Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*";
            openfileDialog.RestoreDirectory = true;
            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {
                frmW.WaitingLabel = "������������...";
                frmW.Show();
                Application.DoEvents();
                LocalFilePath = openfileDialog.FileName;
                StreamReader strReadToEnd = new StreamReader(LocalFilePath);
                string sAllLine = strReadToEnd.ReadToEnd();
                int LineCount = sAllLine.Split('\n').Length + 1;
                string[] pline = new string[LineCount];
                double[] x = new double[LineCount];
                double[] y = new double[LineCount];
                string[] pSeparate = new string[] { };

                


                //m_statusBar.Panels[0].ProgressBarMinValue = 0;
                //m_statusBar.Panels[0].ProgressBarMaxValue = LineCount - 1;

                StreamReader strRead = new StreamReader(LocalFilePath);
                int i = 0;
                int j = 0;
                try
                {
                    if (strRead != null)
                    {
                        pline[i] = strRead.ReadLine();
                        while (pline[i] != null)
                        {
                            pSeparate = pline[i].Split(',', ' ', ';');
                            if (pSeparate.Length >= 3)
                            {
                                x[j] = double.Parse(pSeparate[1]);
                                y[j] = double.Parse(pSeparate[2]);
                                IFeatureClass pFeaCls = m_FeatureLayer.FeatureClass;
                                IFeature pFeature = pFeaCls.CreateFeature();
                                IPoint pPoint = new PointClass();
                                pPoint.X = x[j];
                                pPoint.Y = y[j];
                                pFeature.Shape = pPoint as IGeometry;
                                pFeature.Store();
                                m_mapControl.ActiveView.Refresh();
                                j++;
                                i++;
                                pline[i] = strRead.ReadLine();
                            }
                            else
                            {
                                i++;
                                pline[i] = strRead.ReadLine();
                            }
                            m_statusBar.Panels[0].ProgressBarValue = i;
                            frmW.WaitingLabel = "���ڻ��Ƶ� " + i.ToString() + " ����";
                            Application.DoEvents();
                        }
                    }
                    frmW.Close();
                    MessageBox.Show("������ɣ�");
                }
                catch
                {
                    MessageBox.Show("�����ı��ļ��ĵ�" + i + "��", "������Ϣ", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                }
            }
            ICommand pCommand = new ControlsMapFullExtentCommandClass();
            pCommand.OnCreate(m_mapControl.Object);
            pCommand.OnClick();
            m_statusBar.Panels[0].ProgressBarValue = 0;
        }

        /// <summary>
        /// �����ļ�չ����ͼ��
        /// </summary>
        public void TextToPolyline()
        {
            if (m_FeatureLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
            {
                MessageBox.Show("��ȷ��չ��ĵ�һ��ͼ��Ϊ��ͼ��", "��ʾ��Ϣ", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Title = "��ѡ��չ����ͼ��������ļ�";
            openfileDialog.Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*";
            openfileDialog.RestoreDirectory = true;
            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {
                frmW.WaitingLabel = "������������...";
                frmW.Show();
                Application.DoEvents();
                LocalFilePath = openfileDialog.FileName;
                StreamReader strReadToEnd = new StreamReader(LocalFilePath);
                string sAllLine = strReadToEnd.ReadToEnd();
                int LineCount = sAllLine.Split('\n').Length + 1;
                string[] pline = new string[LineCount];
                double[] x = new double[LineCount];
                double[] y = new double[LineCount];
                string[] pSeparate = new string[] { };

                m_statusBar.Panels[0].ProgressBarMinValue = 0;
                m_statusBar.Panels[0].ProgressBarMaxValue = LineCount - 1;

                StreamReader strRead = new StreamReader(LocalFilePath);
                int i = 0;
                int j = 0;
                int count = 0;
                try
                {
                    if (strRead != null)
                    {
                        pline[i] = strRead.ReadLine();
                        IFeatureClass pFeaCls = m_FeatureLayer.FeatureClass;
                        IFeature pFeature = pFeaCls.CreateFeature();
                        IPointCollection pPointCol = new PolylineClass();
                        IPolyline pPolyline = pPointCol as IPolyline;
                        while (pline[i] != null)
                        {
                            pSeparate = pline[i].Split(',', ' ', ';');
                            if (pSeparate.Length >= 3)
                            {
                                x[j] = double.Parse(pSeparate[1]);
                                y[j] = double.Parse(pSeparate[2]);
                                IPoint pPoint = new PointClass();
                                pPoint.X = x[j];
                                pPoint.Y = y[j];
                                object miss = Type.Missing;
                                pPointCol.AddPoint(pPoint, ref miss, ref miss);
                                j++;
                                i++;
                                pline[i] = strRead.ReadLine();
                            }
                            else
                            {
                                if (count != 0)
                                {
                                    pFeature.Shape = pPointCol as IGeometry;
                                    pFeature.Store();
                                    m_mapControl.ActiveView.Refresh();
                                    pPointCol = new PolylineClass();
                                    pFeature = pFeaCls.CreateFeature();
                                    pPolyline = pPointCol as IPolyline;
                                }
                                count++;
                                i++;
                                pline[i] = strRead.ReadLine();
                            }
                            m_statusBar.Panels[0].ProgressBarValue = i;
                            frmW.WaitingLabel = "���ڻ��Ƶ� " + i.ToString() + " ����";
                            Application.DoEvents();
                        }
                    }
                    frmW.Close();
                    MessageBox.Show("������ɣ�");
                }
                catch
                {
                    MessageBox.Show("�����ı��ļ��ĵ�" + i + "��", "������Ϣ", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                }
            }
            ICommand pCommand = new ControlsMapFullExtentCommandClass();
            pCommand.OnCreate(m_mapControl.Object);
            pCommand.OnClick();
            m_statusBar.Panels[0].ProgressBarValue = 0;
        }

        /// <summary>
        /// �����ļ�չ����ͼ��
        /// </summary>
        public void TextToPolygon()
        {
            if (m_FeatureLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
            {
                MessageBox.Show("��ȷ��չ��ĵ�һ��ͼ��Ϊ��ͼ��", "��ʾ��Ϣ", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Title = "��ѡ��չ����ͼ��������ļ�";
            openfileDialog.Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*";
            openfileDialog.RestoreDirectory = true;
            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {
                frmW.WaitingLabel = "������������...";
                frmW.Show();
                Application.DoEvents();
                LocalFilePath = openfileDialog.FileName;
                StreamReader strReadToEnd = new StreamReader(LocalFilePath);
                string sAllLine = strReadToEnd.ReadToEnd();
                int LineCount = sAllLine.Split('\n').Length + 1;
                string[] pline = new string[LineCount];
                double[] x = new double[LineCount];
                double[] y = new double[LineCount];
                string[] pSeparate = new string[] { };

                m_statusBar.Panels[0].ProgressBarMinValue = 0;
                m_statusBar.Panels[0].ProgressBarMaxValue = LineCount - 1;

                StreamReader strRead = new StreamReader(LocalFilePath);
                int i = 0;
                int j = 0;
                int count = 0;
                try
                {
                    if (strRead != null)
                    {
                        pline[i] = strRead.ReadLine();
                        IFeatureClass pFeaCls = m_FeatureLayer.FeatureClass;
                        IFeature pFeature = pFeaCls.CreateFeature();
                        IPointCollection pPointCol = new PolygonClass();
                        IPolygon pPolygon = pPointCol as IPolygon;
                        while (pline[i] != null)
                        {
                            pSeparate = pline[i].Split(',', ' ', ';');
                            if (pSeparate.Length >= 3)
                            {
                                x[j] = double.Parse(pSeparate[1]);
                                y[j] = double.Parse(pSeparate[2]);
                                IPoint pPoint = new PointClass();
                                pPoint.X = x[j];
                                pPoint.Y = y[j];
                                object miss = Type.Missing;
                                pPointCol.AddPoint(pPoint, ref miss, ref miss);
                                j++;
                                i++;
                                pline[i] = strRead.ReadLine();
                            }
                            else
                            {
                                if (count != 0)
                                {
                                    pFeature.Shape = pPointCol as IGeometry;
                                    pFeature.Store();
                                    m_mapControl.ActiveView.Refresh();
                                    pPointCol = new PolygonClass();
                                    pFeature = pFeaCls.CreateFeature();
                                    pPolygon = pPointCol as IPolygon;
                                }
                                count++;
                                i++;
                                pline[i] = strRead.ReadLine();
                            }
                            m_statusBar.Panels[0].ProgressBarValue = i;
                            frmW.WaitingLabel = "���ڻ��Ƶ� " + i.ToString() + " ����";
                            Application.DoEvents();
                        }
                    }
                    frmW.Close();
                    MessageBox.Show("������ɣ�");
                }
                catch
                {
                    MessageBox.Show("�����ı��ļ��ĵ�" + i + "��", "������Ϣ", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                }
            }
            ICommand pCommand = new ControlsMapFullExtentCommandClass();
            pCommand.OnCreate(m_mapControl.Object);
            pCommand.OnClick();
            m_statusBar.Panels[0].ProgressBarValue = 0;
        }

        public void TextToPoint(string fieldName,string type)
        {
            if (m_FeatureLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
            {
                MessageBox.Show("��ȷ��չ��ĵ�һ��ͼ��Ϊ��ͼ��", "��ʾ��Ϣ", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Title = "��ѡ��չ���ͼ��������ļ�";
            openfileDialog.Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*";
            openfileDialog.RestoreDirectory = true;
            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {

                frmW.WaitingLabel = "������������...";
                frmW.Show();
                Application.DoEvents();
                frmW.WaitingLabel = "����ֶ�...";
                Application.DoEvents();
                IFeatureClass pFeaCls = m_FeatureLayer.FeatureClass;
                IField field=new FieldClass();
                IFieldEdit fieldEdit = field as IFieldEdit;
                fieldEdit.Name_2 = fieldName;
                //pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                switch (type) 
                {
                    case "string":
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                        break;
                    case"double":
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        break;
                };
                pFeaCls.AddField(fieldEdit);

              
                LocalFilePath = openfileDialog.FileName;
                StreamReader strRead = new StreamReader(LocalFilePath);
                IList<string> liusu=new List<string>();
                frmW.WaitingLabel = "��������...";
                Application.DoEvents();
                string line;
                line = strRead.ReadLine();
                while (line != null) 
                {
                    liusu.Add(line);
                    line = strRead.ReadLine();
                }

                //StreamReader strRead = new StreamReader(LocalFilePath);

                string[] pSeparate;
                IPoint pPoint = new PointClass();
                double liusuValue;
                for (int i = 0; i < liusu.Count; i++) 
                {
                    frmW.WaitingLabel = "���ڻ��Ƶ� " + i.ToString() + " ����";
                    Application.DoEvents();
                    pSeparate = liusu[i].Split(',');
                    pPoint.X = double.Parse(pSeparate[0].Trim());
                    pPoint.Y = double.Parse(pSeparate[1].Trim());
                    liusuValue = double.Parse(pSeparate[2].Trim());
                    IFeature pFeature = pFeaCls.CreateFeature();
                    pFeature.Shape = pPoint as IGeometry;
                    pFeature.set_Value(pFeature.Fields.FindField(fieldName), liusuValue);
                    pFeature.Store();
                    
                }
                m_mapControl.ActiveView.Refresh();
                frmW.Close();
                MessageBox.Show("������ɣ�");

                
            }
            ICommand pCommand = new ControlsMapFullExtentCommandClass();
            pCommand.OnCreate(m_mapControl.Object);
            pCommand.OnClick();
            m_statusBar.Panels[0].ProgressBarValue = 0;
        }

    }
}
