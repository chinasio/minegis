using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System.Collections;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.MapControl;


namespace CoastalGIS.CallMap
{
     public  class LayerControl
    {
        public static IFeatureLayer CurIndexLayer=new FeatureLayerClass();   //当前索引对象
        public static IFeatureLayer CurIndexLayerNo = new FeatureLayerClass();  //当前索引对象图号
        public static IFeatureLayer CurFactPBFQ = new FeatureLayerClass();    //平板企业分期
        public static IFeatureLayer CurFactPBA = new FeatureLayerClass();   //平板企业面
        public static string CurIndex = "1：1000索引图";
        public static string CurIndex_Map = "1：1000索引图";
        public static string CurIndex_MapNo = "1：1000索引图图号";
        public static string FactIndex = "平板企业";
        public static string FactPBFQ = "平板企业分期";
        public static string FactPBA = "平板企业面";
                
        public static IGeometry LyrPolygon=null; //数据浏览显示的范围
        public static IGeometry MaxPolygon = new PolygonClass();  //全图显示的范围

        public enum displayViewType
        {
            layViewVisble, layViewNoVisble, layVisbleViewExceptMap
        }   //视图显示类型
        //当前视图类型
        public static displayViewType CurdisplayViewType;

        //把所有图层设为可见
        public static void layVisble(AxMapControl myMapControl)
        {
                IEnumLayer pEnumLayers;
                UID pUID = new UID();
                pUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";

                pEnumLayers = myMapControl.Map.get_Layers(pUID, true);
                pEnumLayers.Reset();
                ILayer pLayer;
                pLayer = (ILayer)pEnumLayers.Next();
                while (pLayer != null)
                {
                    if (pLayer is IFeatureLayer)
                    {
                        IFeatureLayer nFeatureLayer = (IFeatureLayer)pLayer;
                        nFeatureLayer.Visible = true;
                        nFeatureLayer.Selectable = true;
                    }
                    if (pLayer is IRasterLayer)
                    {
                        IRasterLayer pRasterLayer = (IRasterLayer)pLayer;
                        pRasterLayer.Visible = true;

                    }
                    pLayer = pEnumLayers.Next();

                }


            if (CurdisplayViewType != displayViewType.layViewVisble)
            {
                CurdisplayViewType = displayViewType.layViewVisble;
            }
            //m_activeView.Refresh();
        }

        //把所有图层设为可见,除了索引珊格层
        public static void layVisbleExceptMap(AxMapControl myMapControl)
        {
            ICompositeLayer pGroupLayer;
            IGroupLayer pGLayer;
            for (int m = 0; m < myMapControl.Map.LayerCount; m++)
            {

                pGroupLayer = (ICompositeLayer)myMapControl.Map.get_Layer(m);
                pGLayer = (IGroupLayer)pGroupLayer;
                if (pGLayer.Name.ToString().Trim() != LayerControl.CurIndex.ToString().Trim())
                {
                    int sSubLayerCount = pGroupLayer.Count;
                    for (int g = 0; g < sSubLayerCount; g++)
                    {
                        if (pGroupLayer.get_Layer(g) is IFeatureLayer)
                        {
                            IFeatureLayer nFeatureLayer;
                            nFeatureLayer = (IFeatureLayer)pGroupLayer.get_Layer(g);
                            nFeatureLayer.Visible = true; ;
                            nFeatureLayer.Selectable = true;
                        }

                        if (pGroupLayer.get_Layer(g) is IRasterLayer)
                        {
                            IRasterLayer pRasterLayer = (IRasterLayer)pGroupLayer.get_Layer(g);
                            pRasterLayer.Visible = false;

                        }
                    }

                }

            }
            LayerControl.CurIndexLayer.Selectable = false;
            LayerControl.CurIndexLayer.Visible = false;
            LayerControl.CurIndexLayerNo.Selectable = false;
            LayerControl.CurIndexLayerNo.Visible = false;
            if (CurdisplayViewType != displayViewType.layVisbleViewExceptMap)
            {
                CurdisplayViewType = displayViewType.layVisbleViewExceptMap;
            }

            //m_activeView.Refresh();
        }

        //按照设置把所有地形图图层设为不可见,不可选
        public static void layNoVisble(AxMapControl myMapControl)
        {
            ICompositeLayer pGroupLayer;

            for (int m = 0; m < myMapControl.Map.LayerCount; m++)
            {
                if (myMapControl.Map.get_Layer(m).Name.ToString().Trim() != LayerControl.CurIndex.ToString().Trim())
                {
                    pGroupLayer = (ICompositeLayer)myMapControl.Map.get_Layer(m);
                    int sSubLayerCount = pGroupLayer.Count;
                    for (int g = 0; g < sSubLayerCount; g++)
                    {
                        if (pGroupLayer.get_Layer(g) is IFeatureLayer)
                        {
                            IFeatureLayer nFeatureLayer;
                            nFeatureLayer = (IFeatureLayer)pGroupLayer.get_Layer(g);
                            nFeatureLayer.Visible = false;
                            nFeatureLayer.Selectable = false;
                        }

                        if (pGroupLayer.get_Layer(g) is IRasterLayer)
                        {
                            IRasterLayer pRasterLayer = (IRasterLayer)pGroupLayer.get_Layer(g);
                            pRasterLayer.Visible = false;
                           
                        }


                    }
                }
            }

            LayerControl.CurIndexLayer.Selectable = true;
            LayerControl.CurIndexLayer.Visible = true;
            LayerControl.CurIndexLayerNo.Selectable = false;
            LayerControl.CurIndexLayerNo.Visible = true;
            if (CurdisplayViewType != displayViewType.layViewNoVisble)
            {
                CurdisplayViewType = displayViewType.layViewNoVisble;
            }
        }


    }
}
