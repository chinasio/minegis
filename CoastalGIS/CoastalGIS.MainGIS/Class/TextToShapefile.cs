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
        //注意：使用展点功能的时候，点、线、面坐标用逗号、空格、分号分隔开来均可
        //但是，线和面必须有结束标志，如END和其他数值符号等
        //而且展绘的肯定是地图上的第一个图层
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
        /// 坐标文件展绘点图层
        /// </summary>
        public void TextToPoint()
        {
            if (m_FeatureLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
            {
                MessageBox.Show("请确定展绘的第一个图层为点图层", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Title = "请选择展绘点图层的坐标文件";
            openfileDialog.Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*";
            openfileDialog.RestoreDirectory = true;
            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {
                frmW.WaitingLabel = "正在启动绘制...";
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
                            frmW.WaitingLabel = "正在绘制第 " + i.ToString() + " 坐标";
                            Application.DoEvents();
                        }
                    }
                    frmW.Close();
                    MessageBox.Show("绘制完成！");
                }
                catch
                {
                    MessageBox.Show("请检查文本文件的第" + i + "行", "错误信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                }
            }
            ICommand pCommand = new ControlsMapFullExtentCommandClass();
            pCommand.OnCreate(m_mapControl.Object);
            pCommand.OnClick();
            m_statusBar.Panels[0].ProgressBarValue = 0;
        }

        /// <summary>
        /// 坐标文件展绘线图层
        /// </summary>
        public void TextToPolyline()
        {
            if (m_FeatureLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
            {
                MessageBox.Show("请确定展绘的第一个图层为线图层", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Title = "请选择展绘线图层的坐标文件";
            openfileDialog.Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*";
            openfileDialog.RestoreDirectory = true;
            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {
                frmW.WaitingLabel = "正在启动绘制...";
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
                            frmW.WaitingLabel = "正在绘制第 " + i.ToString() + " 坐标";
                            Application.DoEvents();
                        }
                    }
                    frmW.Close();
                    MessageBox.Show("绘制完成！");
                }
                catch
                {
                    MessageBox.Show("请检查文本文件的第" + i + "行", "错误信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                }
            }
            ICommand pCommand = new ControlsMapFullExtentCommandClass();
            pCommand.OnCreate(m_mapControl.Object);
            pCommand.OnClick();
            m_statusBar.Panels[0].ProgressBarValue = 0;
        }

        /// <summary>
        /// 坐标文件展绘面图层
        /// </summary>
        public void TextToPolygon()
        {
            if (m_FeatureLayer.FeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
            {
                MessageBox.Show("请确定展绘的第一个图层为面图层", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Title = "请选择展绘面图层的坐标文件";
            openfileDialog.Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*";
            openfileDialog.RestoreDirectory = true;
            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {
                frmW.WaitingLabel = "正在启动绘制...";
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
                            frmW.WaitingLabel = "正在绘制第 " + i.ToString() + " 坐标";
                            Application.DoEvents();
                        }
                    }
                    frmW.Close();
                    MessageBox.Show("绘制完成！");
                }
                catch
                {
                    MessageBox.Show("请检查文本文件的第" + i + "行", "错误信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
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
                MessageBox.Show("请确定展绘的第一个图层为点图层", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Title = "请选择展绘点图层的坐标文件";
            openfileDialog.Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*";
            openfileDialog.RestoreDirectory = true;
            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {

                frmW.WaitingLabel = "正在启动绘制...";
                frmW.Show();
                Application.DoEvents();
                frmW.WaitingLabel = "添加字段...";
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
                frmW.WaitingLabel = "读入数据...";
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
                    frmW.WaitingLabel = "正在绘制第 " + i.ToString() + " 坐标";
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
                MessageBox.Show("绘制完成！");

                
            }
            ICommand pCommand = new ControlsMapFullExtentCommandClass();
            pCommand.OnCreate(m_mapControl.Object);
            pCommand.OnClick();
            m_statusBar.Panels[0].ProgressBarValue = 0;
        }

    }
}
