using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Geometry;

using CoastalGIS.SpatialDataBase;
using CoastalGIS.ExportMapProj;
using CoastalGIS.MapEditing;
using CoastalGIS.CallMap;

using Janus.Windows.UI;
using Janus.Windows.UI.StatusBar;
using Janus.Windows.Common;


namespace CoastalGIS.MainGIS
{
    public class ToolLib
    {
        public string CurrentControl = null;//用于判断当前控件

        private IMapControlDefault m_mapControl;
        private IPageLayoutControlDefault m_pageLayoutControl;
        private ISceneControlDefault m_sceneControl;
        private ICommand m_command;//AE内置工具接口
        private SpatialDataBase.SDEToORAPra m_oraPra;
        private IWorkspace m_workSpace;
        private SpatialDataBase.GDBConenction m_gcon;
        private string m_connStr;
        private frmMain m_frmMian;
        private UIStatusBar m_statusBar = null;
        private IActiveView m_activeView;

        public ToolLib(frmMain frmMian) 
        {
            m_frmMian = frmMian;
            m_mapControl = frmMian.MapControl;
            m_pageLayoutControl = frmMian.PageLayoutControl;
            m_sceneControl = frmMian.SceneControl;
            m_workSpace = frmMian.WorkSpace;
            m_gcon = frmMian.Gcon;
            m_connStr = frmMian.ConnectionString;
            m_statusBar = frmMian.StatusBar;
            m_activeView = frmMian.MapControl.ActiveView;
        }

        public void StandardToolLibrary(string toolName) 
        {
            switch (toolName) 
            {
                case "AddData":
                    m_command = new ControlsAddDataCommandClass();                    
                    break;
                case"New":
                    m_command = new CreateNewDocument();
                    break;
                case"Open":
                    m_command = new ControlsOpenDocCommandClass();
                    break;
                case"Save":
                    break;
                case"SaveAs":
                    m_command = new ControlsSaveAsDocCommandClass();
                    break;
                case "CADToVec":
                    m_command = new AddCADToFeatures();
                    break;

                case "CADToRas":
                    m_command = new AddCADToRaster();
                    break;
            }
            if(m_command!=null)
            {
                m_command.OnCreate(m_mapControl.Object);
                m_command.OnClick();
                m_command = null;
            }

            if (toolName == "Open") 
            {
                m_frmMian.ControlsSynchronizer.ReplaceMap(m_mapControl.Map);
            }

            if (toolName == "AddTin") 
            {
                FolderBrowserDialog fdlg = new FolderBrowserDialog();
                fdlg.Description = "打开TIN";
                if (fdlg.ShowDialog() == DialogResult.OK) 
                {
                    IWorkspaceFactory wsFac = new TinWorkspaceFactoryClass();
                    if (wsFac.IsWorkspace(System.IO.Path.GetDirectoryName(fdlg.SelectedPath)))
                    {
                        ITinWorkspace tinWS = wsFac.OpenFromFile(System.IO.Path.GetDirectoryName(fdlg.SelectedPath), 0) as ITinWorkspace;
                        ITinLayer tinLyr = new TinLayerClass();
                        try 
                        {
                            tinLyr.Dataset = tinWS.OpenTin(System.IO.Path.GetFileName(fdlg.SelectedPath));
                            tinLyr.Name = System.IO.Path.GetFileName(fdlg.SelectedPath);
                            this.m_mapControl.Map.AddLayer((ILayer)tinLyr);
                        }
                        catch
                        {
                            MessageBox.Show("请选择有效的TIN文件！");
                        }
                    }
                    else 
                    {
                        MessageBox.Show("请选择有效的TIN文件！");
                        return;
                    }

                }
            }

            if (toolName == "AddDEM")
            {
                FolderBrowserDialog fdlg = new FolderBrowserDialog();
                fdlg.Description = "打开DEM";
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    IWorkspaceFactory wsFac = new RasterWorkspaceFactoryClass();
                    if (wsFac.IsWorkspace(System.IO.Path.GetDirectoryName(fdlg.SelectedPath)))
                    {
                        IRasterWorkspace tinWS = wsFac.OpenFromFile(System.IO.Path.GetDirectoryName(fdlg.SelectedPath), 0) as IRasterWorkspace;
                        IRasterLayer rasLyr = new RasterLayerClass();
                        try
                        {
                            rasLyr.CreateFromDataset(tinWS.OpenRasterDataset(System.IO.Path.GetFileName(fdlg.SelectedPath)));
                            this.m_mapControl.Map.AddLayer((ILayer)rasLyr);
                        }
                        catch 
                        {
                            MessageBox.Show("请选择有效的DEM文件！");
                        }

                    }
                    else
                    {
                        MessageBox.Show("请选择有效的DEM文件！");
                        return;
                    }

                }
            }
        }

        public void NavigationToolLibrary(string toolName) 
        {
            switch (toolName) 
            {
                case"ZoomIn":
                    m_command = new ControlsMapZoomInToolClass();
                    break;

                case "ZoomOut":
                    m_command = new ControlsMapZoomOutToolClass();
                    break;

                case "Pan":
                    m_command = new ControlsMapPanToolClass();
                    break;

                case "FullExtent":
                    m_command = new ControlsMapFullExtentCommandClass();
                    m_mapControl.ActiveView.FocusMap.ClipGeometry = null;
                    m_mapControl.ActiveView.Refresh();
                    break;


                case "Back":
                    m_command = new ControlsMapZoomToLastExtentBackCommandClass();
                    break;

                case "Forward":
                    m_command = new ControlsMapZoomToLastExtentForwardCommandClass();
                    break;

                case"SelectFeature":
                    m_command = new ControlsSelectFeaturesToolClass();
                    break;

                case"SelectElement":
                    m_command = new ControlsSelectToolClass();
                    break;

                case"Identify":
                    m_command = new ControlsMapIdentifyToolClass();
                    break;

                case"ClearSel":
                    m_command = new ControlsClearSelectionCommandClass();
                    break;

            }
            if (this.CurrentControl == "map" && m_command is ITool)
            {
                m_command.OnCreate(m_mapControl);
                m_mapControl.CurrentTool = (ITool)m_command;
            }
            else if (this.CurrentControl == "pagelayout" && m_command is ITool)
            {
                m_command.OnCreate(m_pageLayoutControl);
                m_pageLayoutControl.CurrentTool = (ITool)m_command;
            }
            else if (this.CurrentControl == "map")
            {
                m_command.OnCreate(m_mapControl);
                m_command.OnClick();
            }
            else 
            {
                m_command.OnCreate(m_pageLayoutControl);
                m_command.OnClick();
            }
        }

        public void DBToolLibrary(string toolName) 
        {
            switch (toolName)
            {
                case "SHPToSDE":
                    SpatialDataBase.frmSHPToSDE frmSHP = new frmSHPToSDE(m_gcon, this.m_frmMian.OraCmd);
                    frmSHP.ShowDialog();
                    m_frmMian.ShowSDETree();
                    break;
                case "RasterToSDE":
                    SpatialDataBase.frmRasterToSDE frmRaster = new frmRasterToSDE(m_gcon, this.m_frmMian.OraCmd);
                    frmRaster.ShowDialog();
                    m_frmMian.ShowSDETree();
                    break;
                case "Search":
                    SpatialDataBase.frmSearchDB frmSearch = new frmSearchDB(m_gcon, this.m_mapControl, this.m_frmMian.OraCmd);
                    frmSearch.ShowDialog();
                    break;


                //case "SheetMetaData":
                //    SpatialDataBase.frmSheetMetaData frmMetaData = new frmSheetMetaData(m_workSpace, this.m_frmMian.OraCmd);
                //    frmMetaData.Show();
                //    break;
                case "ImageManage":
                    SpatialDataBase.frmImage frmIm = new frmImage(this.m_frmMian.OraCmd);
                    frmIm.ShowDialog();
                    break;

            }
        }

        public void ToolToolLibrary(string toolName)
        {
            bool isFea = false;
            ILayer layer = null;
            switch (toolName)
            {
                case "AddPoint":
                    MapEditing.CreateLayer pCreatePointLayer = new CreateLayer(m_mapControl);
                    pCreatePointLayer.CreatePointLayer();
                    break;
                case "AddPolyLine":
                    MapEditing.CreateLayer pCreatePolylineLayer = new CreateLayer(m_mapControl);
                    pCreatePolylineLayer.CreatePolylineLayer();
                    break;
                case "AddPolygon":
                    MapEditing.CreateLayer pCreatePolygonLayer = new CreateLayer(m_mapControl);
                    pCreatePolygonLayer.CreatePolygonLayer();
                    break;
                case "DrawPoint":
                    for (int i = 0; i < this.m_mapControl.LayerCount; i++)
                    {
                        layer = this.m_mapControl.get_Layer(i);
                        if (layer is IFeatureLayer)
                        {
                            isFea = true;
                            break;
                        }
                    }
                    if (isFea == false) 
                    {
                        MessageBox.Show("请先添加SHP数据！", "提示");
                        return;
                    }
                    TextToShapefile pTextToPoint = new TextToShapefile(m_mapControl, m_statusBar);
                    pTextToPoint.TextToPoint();
                    break;
                case"DrawPointandAtrri":
                    for (int i = 0; i < this.m_mapControl.LayerCount; i++)
                    {
                        layer = this.m_mapControl.get_Layer(i);
                        if (layer is IFeatureLayer)
                        {
                            isFea = true;
                            break;
                        }
                    }
                    if (isFea == false)
                    {
                        MessageBox.Show("请先添加SHP数据！", "提示");
                        return;
                    }
                    //TextToShapefile pTextToPoint = new TextToShapefile(m_mapControl, m_statusBar);
                    //pTextToPoint.TextToPoint();
                    frmField frmfield = new frmField(this.m_mapControl,this.m_statusBar);
                    frmfield.ShowDialog();
                    break;
                case "DrawPolyLine":
                    for (int i = 0; i < this.m_mapControl.LayerCount; i++)
                    {
                        layer = this.m_mapControl.get_Layer(i);
                        if (layer is IFeatureLayer)
                        {
                            isFea = true;
                            break;
                        }
                    }
                    if (isFea == false)
                    {
                        MessageBox.Show("请先添加SHP数据！", "提示");
                        return;
                    }
                    TextToShapefile pTextToPolyline = new TextToShapefile(m_mapControl, m_statusBar);
                    pTextToPolyline.TextToPolyline();
                    break;
                case "DrawPolygon":
                    for (int i = 0; i < this.m_mapControl.LayerCount; i++)
                    {
                        layer = this.m_mapControl.get_Layer(i);
                        if (layer is IFeatureLayer)
                        {
                            isFea = true;
                            break;
                        }
                    }
                    if (isFea == false)
                    {
                        MessageBox.Show("请先添加SHP数据！", "提示");
                        return;
                    }
                    TextToShapefile pTextToPolygon = new TextToShapefile(m_mapControl, m_statusBar);
                    pTextToPolygon.TextToPolygon();
                    break;
            }
        }

        public void CallMapToolLibrary(string toolName) 
        {
            switch (toolName) 
            {
                case"CallMapByCoor":
                    CallMap.frmOpenMapByCoordinate frmCoor = new frmOpenMapByCoordinate(m_mapControl);
                    frmCoor.ShowDialog();
                    break;
                case"CallMapByPolygon":
                    m_command = new CallMap.OpenMapByPolygon();
                    m_command.OnCreate(m_mapControl);
                    m_mapControl.CurrentTool = (ITool)m_command;
                    break;

                //case "CallMapByCommon":
                //    CallMap.frmCallMapByCommon frmCommon = new frmCallMapByCommon(this.m_frmMian.OraCmd,this.m_mapControl,m_workSpace);
                //    frmCommon.ShowDialog();
                //    break;


            }
        }

        public void ThreeDToolLibrary(string toolName)
        {
            switch (toolName) 
            {
                case"3DOpen":
                    m_command = new ControlsSceneOpenDocCommandClass();
                    break;

                case "Navigation":
                    m_command = new ControlsSceneNavigateToolClass();
                    break;

                case"3DZoomIn":
                    m_command = new ControlsSceneZoomInToolClass();
                    break;

                case "3DZoomOut":
                    m_command = new ControlsSceneZoomOutToolClass();
                    break;

                case "3DPan":
                    m_command = new ControlsScenePanToolClass();
                    break;

                case"3DExtent":
                    m_command = new ControlsSceneFullExtentCommandClass();
                    break;

                case"3DSel":
                    m_command = new ControlsSceneSelectGraphicsToolClass();
                    break;

            }
            if (m_command is ITool)
            {
                m_command.OnCreate(m_sceneControl.Object);
                m_sceneControl.CurrentTool = (ITool)m_command;
            }
            if (m_command is ICommand)
            {
                m_command.OnCreate(m_sceneControl.Object);
                m_command.OnClick();
            }
            m_command = null;

            if (toolName == "3DAddTIN")
            {
                FolderBrowserDialog fdlg = new FolderBrowserDialog();
                fdlg.Description = "打开TIN";
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    IWorkspaceFactory wsFac = new TinWorkspaceFactoryClass();
                    if (wsFac.IsWorkspace(System.IO.Path.GetDirectoryName(fdlg.SelectedPath)))
                    {
                        ITinWorkspace tinWS = wsFac.OpenFromFile(System.IO.Path.GetDirectoryName(fdlg.SelectedPath), 0) as ITinWorkspace;
                        ITinLayer tinLyr = new TinLayerClass();
                        try
                        {
                            tinLyr.Dataset = tinWS.OpenTin(System.IO.Path.GetFileName(fdlg.SelectedPath));
                            this.m_sceneControl.Scene.AddLayer((ILayer)tinLyr,false);
                            SetTinAsBase(tinLyr);

                        }
                        catch
                        {
                            MessageBox.Show("请选择有效的TIN文件！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("请选择有效的TIN文件！");
                        return;
                    }

                }
            }

            if (toolName == "3DAddDEM")
            {
                FolderBrowserDialog fdlg = new FolderBrowserDialog();
                fdlg.Description = "打开DEM";
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    IWorkspaceFactory wsFac = new RasterWorkspaceFactoryClass();
                    if (wsFac.IsWorkspace(System.IO.Path.GetDirectoryName(fdlg.SelectedPath)))
                    {
                        IRasterWorkspace tinWS = wsFac.OpenFromFile(System.IO.Path.GetDirectoryName(fdlg.SelectedPath), 0) as IRasterWorkspace;
                        IRasterLayer rasLyr = new RasterLayerClass();
                        try
                        {
                            rasLyr.CreateFromDataset(tinWS.OpenRasterDataset(System.IO.Path.GetFileName(fdlg.SelectedPath)));
                            this.m_sceneControl.Scene.AddLayer((ILayer)rasLyr, false);
                            SetlayerSurface(rasLyr, 1);
                            //this.axSceneControl1.Refresh();
                        }
                        catch 
                        {
                            MessageBox.Show("请选择有效的DEM文件！");
                        }

                    }
                    else
                    {
                        MessageBox.Show("请选择有效的DEM文件！");
                        return;
                    }

                }
            }
        }

        public void IndexToolibrry(string toolName) 
        {
            switch (toolName) 
            {
                case "Stan":
                    string path = Application.StartupPath + "\\山西省煤炭矿山生态环境状况评价技术规范.docx";
                    Process.Start(path);
                    break;
                case "Mark":
                    frmMark mark = new frmMark(this.m_frmMian.OraCmd);
                    mark.Show();
                    break;

                     
            }

        }

        private I3DProperties get3DProps(ITinLayer tinLayer) 
        {
            I3DProperties _3DProps = null;
            ILayerExtensions lyrExt = tinLayer as ILayerExtensions;
            for (int i = 0; i < lyrExt.ExtensionCount; i++) 
            {
                if (lyrExt.get_Extension(i) is I3DProperties) 
                {
                    _3DProps = lyrExt.get_Extension(i) as I3DProperties;
                }
            }
            return _3DProps;
        }

        private void SetTinAsBase(ITinLayer tinLayer) 
        {
            I3DProperties _3DProps = get3DProps(tinLayer);
            if (_3DProps != null) 
            {
                _3DProps.BaseOption = esriBaseOption.esriBaseSurface;
                _3DProps.BaseSurface = tinLayer as IFunctionalSurface;
                _3DProps.Apply3DProperties(tinLayer);
            }
        }

        public void SetTinLayerHeight(ITinLayer pTinLayer, double tinHeight)
        {
            I3DProperties p3DProps = get3DProps(pTinLayer);
            if (p3DProps != null)
            {
                p3DProps.BaseOption = esriBaseOption.esriBaseExpression;
                p3DProps.BaseExpressionString = tinHeight.ToString();
                p3DProps.Apply3DProperties(pTinLayer);
            }

        }

        public IRasterSurface GetSurfFormLayer(IRasterLayer pRLayer)
        {
            I3DProperties p3DProp = Get3DPropsFromLayer((pRLayer));
            //获取图层的基准面
            IRasterSurface pRSurf = (IRasterSurface)p3DProp.BaseSurface;
            //如果空 则取栅格的第一个波段
            if (pRSurf == null)
            {
                //MessageBox.Show("null");
                pRSurf = new RasterSurfaceClass();
                IRasterBandCollection bands = (IRasterBandCollection)pRLayer.Raster;
                pRSurf.RasterBand = bands.Item(0);
            }
            return pRSurf;
        }

        public void SetlayerSurface(IRasterLayer pRlayer, int ZFactor)
        {
            //获取3DProp
            I3DProperties p3DProp = Get3DPropsFromLayer((ILayer)pRlayer);
            p3DProp.BaseOption = esriBaseOption.esriBaseSurface;

            //设置基准高
            IRasterSurface pRSurf = GetSurfFormLayer(pRlayer);
            p3DProp.BaseSurface = (IFunctionalSurface)pRSurf;

            //设置Z因子
            p3DProp.ZFactor = ZFactor;

            //图层渲染优先级，越小优先级越大；
            p3DProp.DepthPriorityValue = 0;

            p3DProp.Apply3DProperties(pRlayer);//I3DProperties中唯一的一个方法，用来执行变更后的属性。
        }

        public I3DProperties Get3DPropsFromLayer(ILayer pLyr)
        {
            ILayerExtensions pLyrExts = (ILayerExtensions)pLyr;
            I3DProperties p3DProp = null;

            for (int i = 0; i < pLyrExts.ExtensionCount; i++)
            {
                if (pLyrExts.get_Extension(i) is I3DProperties)
                {
                    p3DProp = (I3DProperties)pLyrExts.get_Extension(i);
                }
            }//get 3d properties from extension

            return p3DProp;
        }
    }
}
