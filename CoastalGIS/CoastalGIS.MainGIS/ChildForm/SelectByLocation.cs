using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;

namespace CoastalGIS.MainGIS
{
    public partial class SpatialQueryForm : Form
    {
        //private frmMain ParentForm;
        //private IMap pMap;
        private IMapControlDefault m_mapControl = null;
        public SpatialQueryForm(IMapControlDefault mapControl)
        {
            InitializeComponent();
            //ParentForm = pMainForm;
            //pMap = ParentForm.mapCtlMain.ActiveView.FocusMap;
            this.m_mapControl = mapControl;
        }

        private void SelectByLocation_Load(object sender, EventArgs e)
        {
            LayerCheckedListBox.Items.Clear();
            if (m_mapControl.Map.LayerCount != 0)//添加layer的名称到checkedbox
            {
                for (int i = 0; i < m_mapControl.Map.LayerCount; i++)
                {
                    ILayer pLayer = m_mapControl.Map.get_Layer(i);
                    LayerCheckedListBox.Items.Add(pLayer.Name);
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            IEnumFeature pEnumFeature;
            pEnumFeature = (IEnumFeature)m_mapControl.Map.FeatureSelection;
            pEnumFeature.Reset();
            IFeature pFeature;

            pFeature = pEnumFeature.Next();//选择的要素
            while (pFeature != null)
            {

                if (LayerCheckedListBox.CheckedItems.Count != 0)//选中的图层名
                {
                    for (int j = 0; j < LayerCheckedListBox.CheckedItems.Count; j++)
                    {
                        for (int i = 0; i < m_mapControl.Map.LayerCount; i++)
                        {
                            ILayer pLayer;
                            pLayer = m_mapControl.Map.get_Layer(i);
                            string LayerName = pLayer.Name;
                            
                            if (LayerCheckedListBox.CheckedItems[j].ToString() == LayerName)//找到要查找的图层
                            {
                                ISpatialFilter pFilter;
                                pFilter = new SpatialFilterClass();
                                pFilter.Geometry = pFeature.Shape;
                                pFilter.GeometryField = "SHAPE";

                                if (MethordList.SelectedIndex >= 0 && MethordList.SelectedIndex <= 7)
                                {
                                    int n = MethordList.SelectedIndex;//选择空间方法
                                    switch (n)
                                    {
                                        case 0:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelUndefined;
                                            break;
                                        case 1:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                                            break;
                                        case 2:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelEnvelopeIntersects;
                                            break;
                                        case 3:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;
                                            break;
                                        case 4:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;
                                            break;
                                        case 5:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                                            break;
                                        case 6:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                                            break;
                                        case 7:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                                            break;

                                    }
                                    IFeatureLayer pFeatureLayer;
                                    pFeatureLayer = pLayer as IFeatureLayer;
                                    SearchFeature(pFilter, pFeatureLayer);//调用函数，查找和显示
                                    
                                }
                                else
                                {
                                    MessageBox.Show("请选择一种查找方法");
                                    return;
                                }
                                break;//唯一匹配，所以跳出此循环
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请选择要查找的图层");
                    return;
                }
                pFeature = pEnumFeature.Next();
            }
        }

        private void SearchFeature(ISpatialFilter pSpatialFilter, IFeatureLayer pFeatureLyr)
        {
            ISpatialFilter pFilter;
            pFilter = pSpatialFilter;
            IFeatureLayer pFeatureLayer;
            pFeatureLayer = pFeatureLyr;
            IFeatureClass pFC = pFeatureLayer.FeatureClass;
            IFeatureCursor pFeatureCursor;
            pFeatureCursor = pFeatureLayer.Search(pFilter, true);

            IFeature pFeature;
            pFeature = pFeatureCursor.NextFeature();
            IGeometry pShape;
            switch (pFC.ShapeType) 
            {
                case esriGeometryType.esriGeometryPolyline:
                    while (pFeature != null)
                    {
                        ISimpleLineSymbol pFillsyl;
                        pFillsyl = new SimpleLineSymbolClass();
                        pFillsyl.Color = getRGB(220, 100, 50);
                        object oFillsyl;
                        oFillsyl = pFillsyl;
                        pShape = pFeature.Shape as IPolyline;
                        m_mapControl.FlashShape(pShape, 15, 20, pFillsyl);
                        m_mapControl.DrawShape(pShape, ref oFillsyl);
                        pFeature = pFeatureCursor.NextFeature();
                    }
                    break;

                case esriGeometryType.esriGeometryPolygon:
                    while (pFeature != null)
                    {
                        ISimpleFillSymbol pFillsyl;
                        pFillsyl = new SimpleFillSymbolClass();
                        pFillsyl.Color = getRGB(220, 100, 50);
                        object oFillsyl;
                        oFillsyl = pFillsyl;
                        pShape = pFeature.Shape as IPolygon;
                        m_mapControl.FlashShape(pShape, 15, 20, pFillsyl);
                        m_mapControl.DrawShape(pShape, ref oFillsyl);
                        pFeature = pFeatureCursor.NextFeature();
                    }
                    break;

                case esriGeometryType.esriGeometryPoint:
                    while (pFeature != null)
                    {
                        ISimpleMarkerSymbol pFillsyl;
                        pFillsyl = new SimpleMarkerSymbolClass();
                        pFillsyl.Color = getRGB(220, 100, 50);
                        object oFillsyl;
                        oFillsyl = pFillsyl;
                        pShape = pFeature.Shape as IPoint;
                        m_mapControl.FlashShape(pShape, 15, 20, pFillsyl);
                        m_mapControl.DrawShape(pShape, ref oFillsyl);
                        pFeature = pFeatureCursor.NextFeature();
                    }
                    break;
            }




        }

        private IRgbColor getRGB(int r, int g, int b)
        {
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pColor.Red = r;
            pColor.Green = g;
            pColor.Blue = b;
            return pColor;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (m_mapControl.Map.SelectionCount == 0) 
            {
                MessageBox.Show("请先选择待查询要素！");
                return;
            }

            IEnumFeature pEnumFeature;
            pEnumFeature = (IEnumFeature)m_mapControl.Map.FeatureSelection;
            pEnumFeature.Reset();
            IFeature pFeature;

            pFeature = pEnumFeature.Next();//选择的要素
            while (pFeature != null)
            {

                if (LayerCheckedListBox.CheckedItems.Count != 0)//选中的图层名
                {
                    for (int j = 0; j < LayerCheckedListBox.CheckedItems.Count; j++)
                    {
                        for (int i = 0; i < m_mapControl.Map.LayerCount; i++)
                        {
                            ILayer pLayer;
                            pLayer = m_mapControl.Map.get_Layer(i);
                            string LayerName = pLayer.Name;

                            if (LayerCheckedListBox.CheckedItems[j].ToString() == LayerName)//找到要查找的图层
                            {
                                ISpatialFilter pFilter;
                                pFilter = new SpatialFilterClass();
                                pFilter.Geometry = pFeature.Shape;
                                pFilter.GeometryField = "SHAPE";

                                if (MethordList.SelectedIndex >= 0 && MethordList.SelectedIndex <= 7)
                                {
                                    int n = MethordList.SelectedIndex;//选择空间方法
                                    switch (n)
                                    {
                                        case 0:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelUndefined;
                                            break;
                                        case 1:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                                            break;
                                        case 2:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelEnvelopeIntersects;
                                            break;
                                        case 3:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;
                                            break;
                                        case 4:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;
                                            break;
                                        case 5:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                                            break;
                                        case 6:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                                            break;
                                        case 7:
                                            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                                            break;

                                    }
                                    IFeatureLayer pFeatureLayer;
                                    pFeatureLayer = pLayer as IFeatureLayer;
                                    SearchFeature(pFilter, pFeatureLayer);//调用函数，查找和显示

                                }
                                else
                                {
                                    MessageBox.Show("请选择一种查找方法");
                                    return;
                                }
                                break;//唯一匹配，所以跳出此循环
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请选择要查找的图层");
                    return;
                }
                pFeature = pEnumFeature.Next();
            }
        }
    }
}