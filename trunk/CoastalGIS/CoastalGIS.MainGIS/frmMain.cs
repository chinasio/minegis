using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Drawing.Printing;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.DataSourcesFile;

using Janus.Windows.UI;
using Janus.Windows.UI.Dock;
using Janus.Windows.UI.Tab;

using CoastalGIS.SpatialDataBase;
using CoastalGIS.ExportMapProj;
using CoastalGIS.MapEditing;

//using 遥感功能;

namespace CoastalGIS.MainGIS
{
    public partial class frmMain : Form
    {
        #region 系统变量
        private frmMain m_currentWin = null;

        //地图控件对象
        private IMapControlDefault m_mapControl = null;
        public IMapControlDefault MapControl //设置为只读公共属性
        {
            get { return m_mapControl; }
        }

        private IPageLayoutControlDefault m_pageLayoutControl = null;
        public IPageLayoutControlDefault PageLayoutControl //设置为只读公共属性
        {
            get { return m_pageLayoutControl; }
        }

        private ISceneControlDefault m_sceneControl = null;
        public ISceneControlDefault SceneControl 
        {
            get { return m_sceneControl; }
        }

        private ITOCControlDefault m_tocControl = null;
        public ITOCControlDefault TOCControl 
        {
            get { return m_tocControl; }
        }

        //TOCControl被选中的图层
        private ILayer m_layer=null;

        //TreeView中被选中的节点
        private TreeNode m_node = null;

        //同步类,使mapcontrol和pagelayoutcontrol中的数据保持一致
        private ControlsSynchronizer m_controlsSynchronizer = null;
        public ControlsSynchronizer ControlsSynchronizer 
        {
            get { return m_controlsSynchronizer; }
        }


        //工具类
        private ToolLib m_toolLib = null;

        //连接GDB,数据操作
        private GDBConenction m_gcon=null;
        public GDBConenction Gcon //设置为只读公共属性
        {
            get { return m_gcon; }
        }


        private GDBData m_gdata=null;

        private IWorkspace m_workSpace = null;//空间数据库的工作空间
        public IWorkspace WorkSpace //设置为只读公共属性
        {
            get { return m_workSpace; }

        }
        //FGDB路径
        private string m_fgdbPath="";
 
        //判断SDE窗口是否是首次打开
        private bool m_SDEClick;

        //关系数据库连接字符串
        private string m_connectionString;
        public string ConnectionString //设置为只读公共属性
        {
            get { return m_connectionString; }
        }

        //关系数据库连接
        private OleDbConnection m_oraConn=null;


        //关系数据库操作
        private OleDbCommand m_oraCmd = null;
        public OleDbCommand OraCmd 
        {
            get { return m_oraCmd; }
        }

        //数据库树状显示
        public TreeView DBtreeView 
        {
            get { return this.treeVSDE; }
        }

        private ITin m_tin = null;

        //打印输出
        private ExportMapProj.DesignPageLayout m_designPageLayout = null; //制图输出
        private IElement m_element = null;//当前在AxPagelayOutControl中选择的元素
        private IElement m_origenalElement = null; //在选中当前元素时的上一选中元素
        private IResizeEnvelopeFeedback m_resizeEnvelopeFeedback = null;  //用于改变地体整饰元素的包络线大小
        private ISelectionTracker m_selectionTracker; //跟踪元素显示
        private IToolbarMenu m_menuPageLayout = null;//Pagelayout右键菜单
        //declare a PrintDocument object named document, to be displayed in the print preview
        private System.Drawing.Printing.PrintDocument document = new System.Drawing.Printing.PrintDocument();
        //cancel tracker which is passed to the output function when printing to the print preview
        private ITrackCancel m_TrackCancel = new CancelTrackerClass(); //输出
        //the page that is currently printed to the print preview
        private short m_CurrentPrintPage;//当前打印页面

        //地图编辑
        private IMap m_Map=null; //地图编辑
        private ILayer m_CurrentLayer=null; //当前编辑图层
        private bool m_bModify=false;//判断是否在编辑修改要素
        private bool m_bSketch=false;
        private bool m_bEditNode = false;
        private  MapEditing.AoEditor m_editor;

        //影像数据量算标志
        private bool m_rasterMeasureDis = false;
        private bool m_rasterMeasureArea = false;

        //影像数据配准标志
        private bool m_rasterAdjust = false;

        //外业资料
        private bool m_out = false;

        //数字化
        private bool m_dig = false;

        //进度条
        Janus.Windows.UI.StatusBar.UIStatusBar m_statusBar = null;
        public Janus.Windows.UI.StatusBar.UIStatusBar StatusBar //设置为只读公共属性
        {
            get { return m_statusBar;}
        }

        private delegate void showSpalash();

        #endregion 系统变量

        public frmMain()
        {
            InitializeComponent();

            m_currentWin = this;

            //初始化地图控件对象
            m_mapControl = this.mapCtlMain.Object as IMapControlDefault;
            m_pageLayoutControl = this.axPageLayoutControl1.Object as IPageLayoutControlDefault;
            m_tocControl = this.axTOCControl1.Object as ITOCControlDefault;
            m_sceneControl = this.axSceneControl1.Object as ISceneControlDefault;
            m_pageLayoutControl.ActiveView.ShowScrollBars = true;
            m_pageLayoutControl.ActiveView.ShowRulers = true;
            PageLayoutToolBarInit(); //PageLayout右键菜单

            //同步类,使mapcontrol和pagelayoutcontrol中的数据保持一致
            m_controlsSynchronizer = new ControlsSynchronizer(m_mapControl, m_pageLayoutControl);
            m_controlsSynchronizer.BindControls(true);
            m_controlsSynchronizer.AddFrameWorkControl(this.axTOCControl1.Object);

            //绑定地图控件
            this.axTOCControl1.SetBuddyControl(this.mapCtlMain);


            m_fgdbPath = ConfigurationManager.AppSettings["FGDBPath"];      
            //连接数据库，返回工作空间
            m_gcon = new GDBConenction(this.m_fgdbPath);
            m_workSpace = m_gcon.OpenSDEWorkspace();
            m_SDEClick = false;
            m_gdata = new GDBData(m_workSpace);
            //获取关系数据库连接字符串
            m_connectionString = ConfigurationManager.AppSettings["ConnectionString"];

            //获取主界面进度条
            m_statusBar = this.uiStatusBar1;



            //初始化工具类
            m_toolLib = new ToolLib(m_currentWin);
            m_toolLib.CurrentControl = "map";//默认mapcontrol为出示当前控件

            //打开关系数据库
            m_oraConn = new OleDbConnection(m_connectionString);
            try
            {
                m_oraConn.Open();
                m_oraCmd = m_oraConn.CreateCommand();
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.Message);
            }

        }
    
        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void uiCommandManager1_CommandClick(object sender, Janus.Windows.UI.CommandBars.CommandEventArgs e)
        {
            this.m_mapControl.CurrentTool = null;
            this.StatusBar.Panels[0].Text = "当前任务：" + e.Command.Text.ToString();
            this.m_rasterMeasureDis = false;
            this.m_rasterMeasureArea = false;
            //this.mapCtlMain.CurrentTool = null;
            if (e.Command.Key == "menuExit") //退出程序
            {
                Application.Exit();
            }



            switch (e.Command.CategoryName)
            {

                #region 标准工具栏
                case "Standard":
                    m_toolLib.StandardToolLibrary(e.Command.Key.Substring(4, e.Command.Key.Length - 4));
                    break;
                #endregion

                #region 导航工具栏
                case"Navigation":
                    m_toolLib.NavigationToolLibrary(e.Command.Key.Substring(4, e.Command.Key.Length - 4));
                    break;  
                 #endregion 导航工具栏

                #region 数据库工具栏
                case "DB":
                    m_toolLib.DBToolLibrary(e.Command.Key.Substring(4, e.Command.Key.Length - 4));

                    break;
                #endregion 数据库工具栏

                #region 工具工具栏
                case "Tool":
                    m_toolLib.ToolToolLibrary(e.Command.Key.Substring(4, e.Command.Key.Length - 4));
                    break;
                #endregion 工具工具栏

                case"CallMap":
                    m_toolLib.CallMapToolLibrary(e.Command.Key.Substring(4, e.Command.Key.Length - 4));
                    break;

                case"3D":
                    m_toolLib.ThreeDToolLibrary(e.Command.Key.Substring(4, e.Command.Key.Length - 4));
                    break;

                case"Index":
                    m_toolLib.IndexToolibrry(e.Command.Key.Substring(4, e.Command.Key.Length - 4));
                    break;
            }

            #region 出图制图栏
            if (e.Command.Key == "menuLegend") //设置比例尺
            {
                DesignPageLayout designPageLayout = new DesignPageLayout(m_mapControl, m_pageLayoutControl);
                designPageLayout.AddLegend();//添加图例
            }

            if (e.Command.Key == "menuText")
            {
                m_designPageLayout = new DesignPageLayout(m_mapControl, m_pageLayoutControl);
                m_designPageLayout.AddText();//添加文字
            }

            if (e.Command.Key == "menuNorthArrow")
            {
                m_designPageLayout = new DesignPageLayout(m_mapControl, m_pageLayoutControl);
                m_designPageLayout.AddNorthArrow();//添加指北针

            }

            if (e.Command.Key == "menuScaleBar")
            {
                m_designPageLayout = new DesignPageLayout(m_pageLayoutControl);
                m_designPageLayout.AddScaleBar();//添加比例尺
            }

            if (e.Command.Key == "menuScaleText")
            {
                m_designPageLayout = new DesignPageLayout(m_pageLayoutControl);
                m_designPageLayout.AddTextScale();//添加文字比例尺

            }

            if (e.Command.Key == "menuMapGrid")
            {
                m_designPageLayout = new DesignPageLayout(m_pageLayoutControl);
                m_designPageLayout.AddMapGrid();//添加地图格网
            }

            if (e.Command.Key == "menuPageAndPrintaSet")
            {
                #region//页面设置
                DialogResult result = pageSetupDialog1.ShowDialog();
                document.PrinterSettings = pageSetupDialog1.PrinterSettings;
                document.DefaultPageSettings = pageSetupDialog1.PageSettings;

                int i;
                IEnumerator paperSizes = pageSetupDialog1.PrinterSettings.PaperSizes.GetEnumerator();
                paperSizes.Reset();
                for (i = 0; i < pageSetupDialog1.PrinterSettings.PaperSizes.Count; ++i)
                {
                    paperSizes.MoveNext();
                    if (((PaperSize)paperSizes.Current).Kind == document.DefaultPageSettings.PaperSize.Kind)
                    {
                        document.DefaultPageSettings.PaperSize = ((PaperSize)paperSizes.Current);
                    }
                }
                IPaper paper;
                paper = new PaperClass(); //create a paper object

                IPrinter printer;
                printer = new EmfPrinterClass(); //create a printer object
                paper.Attach(pageSetupDialog1.PrinterSettings.GetHdevmode(pageSetupDialog1.PageSettings).ToInt32(), pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());
                printer.Paper = paper;
                m_pageLayoutControl.Printer = printer;
                #endregion
            }

            if (e.Command.Key == "menuPrintView")
            {
                #region//打印预览
                m_CurrentPrintPage = 0;
                if (axPageLayoutControl1.ActiveView.FocusMap == null) return;
                document.DocumentName = axPageLayoutControl1.DocumentFilename;
                printPreviewDialog1.Document = document;
                printPreviewDialog1.ShowDialog();
                #endregion

            }

            if (e.Command.Key == "menuPrintExport")
            {
                #region //打印
                //allow the user to choose the page range to be printed
                printDialog1.AllowSomePages = true;
                //show the help button.
                printDialog1.ShowHelp = true;

                //set the Document property to the PrintDocument for which the PrintPage Event 
                //has been handled. To display the dialog, either this property or the 
                //PrinterSettings property must be set 
                printDialog1.Document = document;

                //show the print dialog and wait for user input
                DialogResult result = printDialog1.ShowDialog();

                // If the result is OK then print the document.
                if (result == DialogResult.OK) document.Print();
                #endregion

            }

            if (e.Command.Key == "menuPrintByAnyPolygonRegion")
            {
                if (this.mapCtlMain.LayerCount == 0)
                {
                    MessageBox.Show("请先添加数据！", "提示");
                    return;
                }
                ICommand pArePrintMapClass = new ExportMapProj.AreaPrintMapClass(this.mapCtlMain.Map);
                pArePrintMapClass.OnCreate(this.mapCtlMain.Object);
                this.mapCtlMain.CurrentTool = (ITool)pArePrintMapClass;
                mapCtlMain.ActiveView.Refresh();
            }

            if (e.Command.Key == "menuExportMap")
            {
                ICommand pExportMapAsPicture = new ExportMapProj.CmdExoprtMapAsPicture();
                pExportMapAsPicture.OnCreate(m_pageLayoutControl.Object);
                pExportMapAsPicture.OnClick();

            }
            #endregion

            #region 要素编辑
            if (e.Command.Key == "menuStart")
            {

                if (mapCtlMain.LayerCount == 0)
                {
                    MessageBox.Show("请先加载数据！");
                    return;
                }
                m_Map = mapCtlMain.Map;

                IFeatureLayer feaLayer = null;
                IDataset dataSet = null;
                IWorkspaceEdit wsEdit = null;
                for (int i = 0; i < m_Map.LayerCount; i++)
                {
                    if (m_Map.get_Layer(i) is IFeatureLayer) 
                    {
                        feaLayer = m_Map.get_Layer(i) as IFeatureLayer;
                        dataSet = (IDataset)feaLayer.FeatureClass;
                        wsEdit = dataSet.Workspace as IWorkspaceEdit;
                        uiComboBox5.Items.Add(m_Map.get_Layer(i).Name);
                    }

                    if (wsEdit.IsBeingEdited()==true) 
                    {
                        return;
                    }
                }

                this.menuStart.Enabled = Janus.Windows.UI.InheritableBoolean.False;
                this.menuStop2.Enabled = Janus.Windows.UI.InheritableBoolean.True;
                this.menuSketch1.Enabled = Janus.Windows.UI.InheritableBoolean.True;
                this.menuSaveEditing2.Enabled = Janus.Windows.UI.InheritableBoolean.True;
                this.menuDelete1.Enabled = Janus.Windows.UI.InheritableBoolean.True;
                this.menuRedoEditing1.Enabled = Janus.Windows.UI.InheritableBoolean.True;
                this.menuUndoEditing1.Enabled = Janus.Windows.UI.InheritableBoolean.True;
                this.menuTask1.Enabled = Janus.Windows.UI.InheritableBoolean.True;
                this.menuLayer1.Enabled = Janus.Windows.UI.InheritableBoolean.True;

                uiComboBox5.SelectedIndex = 0;
                for (int i = 0; i < m_Map.LayerCount; i++)
                {
                    if (m_Map.get_Layer(i).Name == uiComboBox5.Text)
                    {
                        m_CurrentLayer = m_Map.get_Layer(i);
                        break;
                    }
                }
                m_bModify = false;
                m_bSketch = true;
                uiComboBox4.SelectedIndex = 0;
                StartEditing();
                MapEditing.CreatShape m_CreateShapeStart = new CreatShape(m_CurrentLayer,this.m_dig);
                m_CreateShapeStart.OnCreate(this.mapCtlMain.Object);
                mapCtlMain.CurrentTool = (ITool)m_CreateShapeStart;
            }

            if (e.Command.Key == "menuStop")
            {
                if (mapCtlMain.LayerCount == 0)
                {
                    MessageBox.Show("请加载数据！");
                    return;
                }
                if (StopEditing() == 1)
                {
                    mapCtlMain.Map.ClearSelection();
                    mapCtlMain.ActiveView.Refresh();
                    mapCtlMain.MousePointer = esriControlsMousePointer.esriPointerDefault;
                    this.menuStart.Enabled = Janus.Windows.UI.InheritableBoolean.True;
                    this.menuStop2.Enabled = Janus.Windows.UI.InheritableBoolean.False;
                    this.menuSketch1.Enabled = Janus.Windows.UI.InheritableBoolean.False;
                    this.menuSaveEditing2.Enabled = Janus.Windows.UI.InheritableBoolean.False;
                    this.uiComboBox4.Text = "";
                    this.uiComboBox5.Text ="";
                    this.menuDelete1.Enabled = Janus.Windows.UI.InheritableBoolean.False;
                    this.menuRedoEditing1.Enabled = Janus.Windows.UI.InheritableBoolean.False;
                    this.menuUndoEditing1.Enabled = Janus.Windows.UI.InheritableBoolean.False;
                    this.menuTask1.Enabled = Janus.Windows.UI.InheritableBoolean.False;
                    this.menuLayer1.Enabled = Janus.Windows.UI.InheritableBoolean.False;
                    this.uiComboBox5.Items.Clear();
                }
            }

            if (e.Command.Key == "menuSaveEditing")
            {
                if (mapCtlMain.LayerCount == 0)
                {
                    MessageBox.Show("请加载数据！");
                    return;
                }
                if (SaveEditing() == 1)
                {
                    mapCtlMain.Map.ClearSelection();
                    mapCtlMain.ActiveView.Refresh();
                }
            }

            if (e.Command.Key == "menuSketch")        //草图画笔工具
            {
                if (mapCtlMain.LayerCount == 0)
                {
                    MessageBox.Show("请加载数据！");
                    return;
                }
                if (m_bSketch)
                {
                    CreatShape m_CreateShapeSketch = new CreatShape(m_CurrentLayer,this.m_dig);
                    m_CreateShapeSketch.OnCreate(this.mapCtlMain.Object);
                    mapCtlMain.CurrentTool = (ITool)m_CreateShapeSketch;
                }
                else if (m_bModify)
                {
                    ModifyShape m_ModifyShape = new ModifyShape(m_CurrentLayer);
                    m_ModifyShape.OnCreate(this.mapCtlMain.Object);
                    mapCtlMain.CurrentTool = (ITool)m_ModifyShape;

                }
                else if (m_bEditNode) 
                {

                }
                else
                {
                    MessageBox.Show("编辑操作尚未开始，请先开始编辑！");
                    return;
                }
                StartEditing();//开始编辑
            }

            if (e.Command.Key == "menuDelete")
            {
                if (mapCtlMain.LayerCount == 0)
                {
                    MessageBox.Show("请先加载数据！");
                    return;
                }
                DeleteSelectedFeatures();            //调用删除要素方法
            }

            if (e.Command.Key == "menuUndoEditing")
            {
                if (m_CurrentLayer == null)
                    return;
                IWorkspaceEdit pWorkSpaceEdit = GetWorkspaceEdit();
                bool bHasUndos = false;
                pWorkSpaceEdit.HasUndos(ref bHasUndos);
                if (bHasUndos)
                    pWorkSpaceEdit.UndoEditOperation();
                IActiveView pActiveView = m_Map as IActiveView;
                pActiveView.Refresh();
            }

            if (e.Command.Key == "menuRedoEditing")
            {
                if (m_CurrentLayer == null)
                    return;
                IWorkspaceEdit pWorkSpaceEdit = GetWorkspaceEdit();
                bool bHasRedo = false;
                pWorkSpaceEdit.HasRedos(ref bHasRedo);
                if (bHasRedo)
                    pWorkSpaceEdit.RedoEditOperation();
                IActiveView pActiveView = m_Map as IActiveView;
                pActiveView.Refresh();
            }
            #endregion

            if (e.Command.Key == "menuSelectbyAttribute")
            {
                if (this.mapCtlMain.LayerCount == 0) 
                {
                    MessageBox.Show("请先添加数据！");
                    return;
                }
                QueryForm pQueryForm = new QueryForm(this.m_mapControl);
                pQueryForm.Show();
            }

            if (e.Command.Key == "menuSelectbyLocation")
            {
                if (this.mapCtlMain.LayerCount == 0)
                {
                    MessageBox.Show("请先添加数据！");
                    return;
                }
                SpatialQueryForm pSpatialQueryForm = new SpatialQueryForm(this.m_mapControl);
                pSpatialQueryForm.Show();
            }

            if (e.Command.Key == "menuBuffer")
            {
                if (this.mapCtlMain.LayerCount == 0)
                {
                    MessageBox.Show("请先添加数据！");
                    return;
                }
                BufferForm pBufferForm = new BufferForm(this.m_mapControl);
                pBufferForm.Show();
            }

            if (e.Command.Key == "menuOverLay")
            {
                if (this.mapCtlMain.LayerCount == 0)
                {
                    MessageBox.Show("请先添加数据！");
                    return;
                }
                OverLayerForm pOverLayerForm = new OverLayerForm(this.m_mapControl);
                pOverLayerForm.Show();
            }

            if (e.Command.Key == "menu3DForm")
            {
                if (e.Command.Checked == Janus.Windows.UI.InheritableBoolean.False)
                {
                    this.ThreeDtap.TabVisible = true;
                    this.mapTab.Enabled = false;
                    this.pageTab.Enabled = false;
                    this.uiTab1.SelectedTab = this.ThreeDtap;

                    this.ThreeDBar.Visible = true;

                    e.Command.Checked = Janus.Windows.UI.InheritableBoolean.True;
                    this.axTOCControl1.SetBuddyControl(this.axSceneControl1);
                }
                else 
                {
                    this.ThreeDtap.TabVisible = false;
                    this.mapTab.Enabled = true;
                    this.pageTab.Enabled = true;
                    this.uiTab1.SelectedTab = this.mapTab;
                    this.ThreeDBar.Visible = false;
                    e.Command.Checked = Janus.Windows.UI.InheritableBoolean.False;
                    this.axSceneControl1.CurrentTool = null;
                    this.axSceneControl1.Scene.ClearLayers();
                    this.axTOCControl1.SetBuddyControl(this.mapCtlMain);
                }

            }

            if (e.Command.Key == "menuCacu") 
            {
                if (this.mapCtlMain.LayerCount == 0)
                {
                    MessageBox.Show("请先添加数据！");
                    return;
                }

                ICommand ff = new AreaPrintMapClass();
                ff.OnCreate(this.m_mapControl);
                this.m_mapControl.CurrentTool = (ITool)ff;
 
            }

            if (e.Command.Key == "menuRasDis") 
            {
                this.mapCtlMain.CurrentTool = null;
                bool isRaster = false;
                ILayer layer = null;
                for (int i = 0; i < this.mapCtlMain.LayerCount; i++) 
                {
                    layer = this.mapCtlMain.get_Layer(i);
                    if (layer is IRasterLayer) 
                    {
                        isRaster = true;
                        break;
                    }
                }
                if (isRaster == false) 
                {
                    MessageBox.Show("请先添加影像数据！", "提示");
                    return;
                }
                this.m_rasterMeasureDis = true;
                this.mapCtlMain.MousePointer = esriControlsMousePointer.esriPointerCrosshair;

            }

            if (e.Command.Key == "menuRasArea")
            {
                this.mapCtlMain.CurrentTool = null;
                bool isRaster = false;
                ILayer layer = null;

                for (int i = 0; i < this.mapCtlMain.LayerCount; i++)
                {
                    layer = this.mapCtlMain.get_Layer(i);
                    if (layer is IRasterLayer)
                    {
                        isRaster = true;
                        break;
                    }
                }
                if (isRaster == false)
                {
                    MessageBox.Show("请先添加影像数据！", "提示");
                    return;
                }
                this.m_rasterMeasureArea = true;
                this.mapCtlMain.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            }

            if (e.Command.Key == "menuTemplate")
            {
                ChoseTemple chosetem = new ChoseTemple();
                chosetem.ShowDialog();
                if (chosetem.m_templateName != "")
                {
                    this.axPageLayoutControl1.ActiveView.Clear();
                    if (chosetem.m_templateName.Contains("\\"))
                    {
                        this.axPageLayoutControl1.LoadMxFile(chosetem.m_templateName, Type.Missing);
                    }
                    else 
                    {
                        this.axPageLayoutControl1.LoadMxFile(System.Windows.Forms.Application.StartupPath + @"\pagelayoutTemplate\" + chosetem.m_templateName, Type.Missing);
                    }
                    m_controlsSynchronizer.ReplaceMap(m_mapControl.Map);//重新联动

                    SetMapFrame();
                    m_pageLayoutControl.ActiveView.ShowScrollBars = true;

                }
            }

            if (e.Command.Key == "menuAbout") 
            {
                frmAbout frmAbout = new frmAbout();
                frmAbout.ShowDialog();
            }

            if (e.Command.Key == "menuTOCControl")
            {
                if (this.menuTOCControl1.Checked == Janus.Windows.UI.InheritableBoolean.True)
                {
                    this.TOCPanel.Visible = false;
                    this.menuTOCControl1.Checked = Janus.Windows.UI.InheritableBoolean.False;
                }
                else 
                {
                    this.TOCPanel.Visible = true;
                    this.menuTOCControl1.Checked = Janus.Windows.UI.InheritableBoolean.True;
                }
            }

            if (e.Command.Key == "menuAtrriWin")
            {
                if (this.menuAtrriWin1.Checked == Janus.Windows.UI.InheritableBoolean.True)
                {
                    this.SearchPanel.Closed = true;
                    this.menuAtrriWin1.Checked = Janus.Windows.UI.InheritableBoolean.False;
                }
                else
                {
                    this.SearchPanel.Closed = false;
                    this.menuAtrriWin1.Checked = Janus.Windows.UI.InheritableBoolean.True;
                }
            }

            if (e.Command.Key == "menuOut")
            {
                if (e.Command.Checked == Janus.Windows.UI.InheritableBoolean.True)
                {
                    e.Command.Checked = Janus.Windows.UI.InheritableBoolean.False;
                    m_out = false;
                }
                else 
                {
                    e.Command.Checked = Janus.Windows.UI.InheritableBoolean.True;
                    m_out = true;
                    this.mapCtlMain.ClearLayers();

                    IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactory();
                    IWorkspace workspace;
                    workspace = workspaceFactory.OpenFromFile(Application.StartupPath + "\\out", 0); //inPath栅格数据存储路径
                    IRasterWorkspace rastWork = (IRasterWorkspace)workspace;
                    IRasterDataset rastDataset;
                    rastDataset = rastWork.OpenRasterDataset("ps2010.img");//inName栅格文件名
                    IRasterLayer layer = new RasterLayerClass();
                    layer.CreateFromDataset(rastDataset);
                    m_mapControl.AddLayer(layer, 0);
                    m_mapControl.AddShapeFile(Application.StartupPath + "\\out", "WYDCZL.shp");


                    m_mapControl.ActiveView.Refresh();
                }

            }
            if (e.Command.Key == "menuDig")
            {
                if (e.Command.Checked == Janus.Windows.UI.InheritableBoolean.False)
                {
                    e.Command.Checked = Janus.Windows.UI.InheritableBoolean.True;
                    this.m_dig = true;
                }
                else 
                {
                    e.Command.Checked = Janus.Windows.UI.InheritableBoolean.False;
                    this.m_dig = false;
                }
            }

            if (e.Command.Key == "menuIndexCacu") 
            {
                frmCacuIndex index = new frmCacuIndex();
                index.ShowDialog();
            }

            if (e.Command.Key == "menuSKC")
            {
                frmChart chart = new frmChart(this.m_oraCmd);
                chart.ShowDialog();
            }
            //******************************增加----陈*************************************************
            //if (e.Command.Key == "menuKMEANS")
            //{
            //    KMEANS kMEANS = new KMEANS(m_mapControl);
            //    kMEANS.ShowDialog();
            //}
            //if (e.Command.Key == "menuCalibration")
            //{
            //    OpenFileDialog openGD = new OpenFileDialog();
            //    openGD.Filter = "ERDAS格式(*.img)|*.img|所有文件|*.*";
            //    if (openGD.ShowDialog() == DialogResult.OK)
            //    {
            //        string filename = openGD.FileName;
            //        int bandCount;
            //        try
            //        {
            //            IRasterLayer rasterLayer = new RasterLayerClass();
            //            rasterLayer.CreateFromFilePath(filename);
            //            bandCount = rasterLayer.BandCount;
            //        }
            //        catch
            //        {
            //            MessageBox.Show("输入文件有误");
            //            return;
            //        }
            //        Calibration cali = new Calibration(filename, bandCount, m_mapControl);
            //        cali.ShowDialog();
            //    }

            //}
            //if (e.Command.Key == "menuISODATA")
            //{
            //    ISODATA isodata = new ISODATA(m_mapControl);
            //    isodata.ShowDialog();
            //}
            //if (e.Command.Key == "menuNDVI")
            //{
            //    NDVI ndvi = new NDVI(m_mapControl);
            //    ndvi.ShowDialog();
            //}
            //if (e.Command.Key == "menuRVI")
            //{
            //    RVI rvi = new RVI(m_mapControl);
            //    rvi.ShowDialog();
            //}
            //if (e.Command.Key == "menuSAVI")
            //{
            //    SAVI savi = new SAVI(m_mapControl);
            //    savi.ShowDialog();
            //}
            //if (e.Command.Key == "menuVC")
            //{
            //    VegCover vegCover = new VegCover(m_mapControl);
            //    vegCover.ShowDialog();
            //}
            //if (e.Command.Key == "menuTemprature")
            //{
            //    Temperature temperature = new Temperature(m_mapControl);
            //    temperature.ShowDialog();
            //}
            //if (e.Command.Key == "menuChangeD")
            //{
            //    ChangeDetection changeD = new ChangeDetection(m_mapControl);
            //    changeD.ShowDialog();

            //}
            //if (e.Command.Key == "menuIce")
            //{
            //    IceExtract iceExtract = new IceExtract(m_mapControl);
            //    iceExtract.ShowDialog();
            //}
            //if (e.Command.Key == "menuWater")
            //{
            //    WaterExtract waterExtract = new WaterExtract(m_mapControl);
            //    waterExtract.ShowDialog();

            //}
            //if (e.Command.Key == "menuHOT")
            //{
            //    HOT hot = new HOT(m_mapControl);
            //    hot.ShowDialog();

            //}
            //if (e.Command.Key == "menuQAC")
            //{
            //    QAC qac = new QAC(m_mapControl);
            //    qac.ShowDialog();
            //}
            //if (e.Command.Key == "menuLUC")
            //{
            //    LandUseChange luc = new LandUseChange(m_mapControl);
            //    luc.ShowDialog();
            //}
            //if (e.Command.Key == "menuComposite")
            //{
            //    Composite comp = new Composite(m_mapControl);
            //    comp.ShowDialog();
            //}
            //*****************************************************************************************
        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            IBasicMap map = null;//实现TOC的右键快捷菜单
            object other = null;
            object index = null;
            esriTOCControlItem item = new esriTOCControlItem();
            this.axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref m_layer, ref other, ref index);
            if (e.button == 2 && item == esriTOCControlItem.esriTOCControlItemLayer)                                      
            {
                this.OpenAttribute.Enabled = false;
                this.ClearAttribute.Enabled = false;
                this.SelectBand.Enabled = false;
                if (m_layer is IFeatureLayer)
                {
                    this.OpenAttribute.Enabled = true;
                    this.ClearAttribute.Enabled = true;
                }
                else 
                {
                    this.SelectBand.Enabled = true;
                }

                System.Drawing.Point point = new System.Drawing.Point(e.x, e.y);
                this.conMenuTOC.Show(axTOCControl1, point);
            }
        }

        private void DeleteLayer_Click(object sender, EventArgs e)
        {//删除图层
            if (this.mapCtlMain.Map == null)
                return;

            IMap map = mapCtlMain.Map;
            for (int i = 0; i < map.LayerCount; i++)
            {
                if (map.get_Layer(i) == m_layer)
                {
                    map.DeleteLayer(m_layer);
                }
            }

            if (map.LayerCount == 0) 
            {
                this.mapCtlMain.SpatialReference = null;
                this.StatusBar.Panels[2].Text = "";
                this.StatusBar.Panels[3].Text = "";
            }

            this.mapCtlMain.Update();
            this.mapCtlMain.Refresh();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            ESRI.ArcGIS.ADF.COMSupport.AOUninitialize.Shutdown();
            Process.GetCurrentProcess().Kill();
            this.m_oraConn.Close();
        }

        private void OpenAttribute_Click(object sender, EventArgs e)
        {//打开属性表
            //数据表中出现当前图层数据
            //获取有效的图层名称 a_b被解析为a.b
            string layerName = LayerDataTable.getValidFeatureClassName(m_layer.Name);
            DataTable dt = LayerDataTable.CreateDataTable(m_layer, layerName);
            DataSet ds=new DataSet();
            ds.Tables.Add(dt);
            this.bindingSource1.DataSource = ds;
            this.bindingSource1.DataMember = layerName;
            this.dataGridView1.DataSource = bindingSource1;
            this.SearchPanel.Text = "数据表[" + layerName + "]" + "  记录数：" + ds.Tables[layerName].Rows.Count.ToString();
            this.dataGridView1.Refresh();
            this.SearchPanel.AutoHide = false;

        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {//点击属性表
            if (e.RowIndex != -1)
            {
                IFeature feat = null;
                try
                {
                    IFeatureClass featCls = (m_layer as IFeatureLayer).FeatureClass;
                    //寻找该行记录对应的要素
                    feat = featCls.GetFeature(Convert.ToInt32(this.dataGridView1.Rows[e.RowIndex].Cells[0].Value));
                }
                catch (Exception ex)
                {
                    feat = null;
                }
                if (feat != null)
                {
                    //要素的定义
                    if (feat.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                    {
                        this.mapCtlMain.CenterAt((IPoint)feat.Shape);
                    }
                    else
                    {
                        IEnvelope env = feat.Shape.Envelope;
                        env.Expand(5, 5, true);
                        this.mapCtlMain.ActiveView.Extent = env;
                    }
                    this.mapCtlMain.ActiveView.Refresh();
                    this.mapCtlMain.ActiveView.ScreenDisplay.UpdateWindow();
                    //用于解决先定位后闪烁的问题
                    //自定义闪烁功能
                    switch (feat.Shape.GeometryType)
                    {
                        case esriGeometryType.esriGeometryPoint:
                            FlashFeature.FlashPoint(this.m_mapControl, this.mapCtlMain.ActiveView.ScreenDisplay, feat.Shape);
                            break;
                        case esriGeometryType.esriGeometryPolyline:
                            FlashFeature.FlashLine(this.m_mapControl, this.mapCtlMain.ActiveView.ScreenDisplay, feat.Shape);
                            break;
                        case esriGeometryType.esriGeometryPolygon:
                            FlashFeature.FlashPolygon(this.m_mapControl, this.mapCtlMain.ActiveView.ScreenDisplay, feat.Shape);
                            break;
                        default:
                            break;
                    }

                    this.mapCtlMain.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                    this.mapCtlMain.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }
        }

        private void ClearAttribute_Click(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = null;
            this.SearchPanel.Text = "属性数据浏览";
            this.SearchPanel.AutoHide = true;
        }

        private void uiTab1_SelectedTabChanged(object sender, Janus.Windows.UI.Tab.TabEventArgs e)
        {
            if (m_controlsSynchronizer != null)
            {
                if (e.Page.Name == "mapTab")
                {
                    m_controlsSynchronizer.ActivateMap();
                    m_toolLib.CurrentControl = "map";
                    mapCtlMain.ActiveView.Refresh();
                    mapCtlMain.Refresh();
                    this.ExportMapToolBar.Visible = false;
                    this.menuCarto1.Enabled = Janus.Windows.UI.InheritableBoolean.False;
                }
                else if (e.Page.Name == "pageTab")
                {
                    m_controlsSynchronizer.ActivatePageLayout();
                    m_toolLib.CurrentControl = "pagelayout";
                    axPageLayoutControl1.ActiveView.Refresh();
                    this.ExportMapToolBar.Visible = true;
                    this.menuCarto1.Enabled = Janus.Windows.UI.InheritableBoolean.True;
                }
            }
        }

        private void TOCPanel_SelectedPanelChanged(object sender, PanelActionEventArgs e)
        {
            if (this.TOCPanel.SelectedPanel == this.SDEPanel&&this.m_SDEClick == false) 
            {
                ShowSDETree();
                this.m_SDEClick = true;
            }
        }

        private void treeVSDE_AfterCheck(object sender, TreeViewEventArgs e)
        {
            IFeatureLayer feaLyr;
            if (e.Node.Checked == true)
            {
                if (e.Node.Tag.ToString() == "point" || e.Node.Tag.ToString() == "line" || e.Node.Tag.ToString() == "polygon" ||e.Node.Tag.ToString() == "Annotation")
                {
                    if (e.Node.Parent.Tag.ToString() == "PLACE")
                    {
                        feaLyr = m_gdata.AddFeatureClassToMap(e.Node.Parent.Text.ToString()+"_" + e.Node.Text.ToString());
                        feaLyr.Name = e.Node.Text.ToString();
                        this.mapCtlMain.Map.AddLayer(feaLyr);
                    }
                    else 
                    {
                        feaLyr = m_gdata.AddFeatureClassToMap(e.Node.Text.ToString());
                        feaLyr.Name = e.Node.Text.ToString();
                        this.mapCtlMain.Map.AddLayer(feaLyr);
                    }

                }
                if (e.Node.Tag.ToString() == "Raster")
                {
                    IRasterWorkspaceEx rasterWS = m_workSpace as IRasterWorkspaceEx;
                    IRasterDataset rasterDS = rasterWS.OpenRasterDataset(e.Node.Parent.Text.ToString()+"_"+ e.Node.Text.ToString());
                    IRasterLayer rasterLayer = new RasterLayerClass();
                    rasterLayer.CreateFromDataset(rasterDS);
                    rasterLayer.Name = e.Node.Text;
                    this.mapCtlMain.Map.AddLayer(rasterLayer);
                    this.mapCtlMain.Refresh();
                }
            }
            else 
            {
                if (e.Node.Tag.ToString() == "point" || e.Node.Tag.ToString() == "line" || e.Node.Tag.ToString() == "polygon"|| e.Node.Tag.ToString() == "Annotation")
                {
                    for (int i = 0; i < this.mapCtlMain.Map.LayerCount; i++) 
                    {
                        ILayer layer=this.mapCtlMain.Map.get_Layer(i);
                        if(e.Node.Text.ToString()==layer.Name.ToString())
                        {
                            this.mapCtlMain.Map.DeleteLayer(layer);
                            this.mapCtlMain.ActiveView.Refresh();
                        }
                    }
                }

                if (e.Node.Tag.ToString() == "Raster") 
                {
                    ILayer layer;
                    for (int i = 0; i < this.mapCtlMain.Map.LayerCount; i++)
                    {
                        layer = this.mapCtlMain.Map.get_Layer(i);
                        if (layer.Name.ToString()==e.Node.Text.ToString())
                        {
                            this.mapCtlMain.Map.DeleteLayer(layer);
                            this.mapCtlMain.ActiveView.Refresh();
                        }
                    }
                }
                if (this.mapCtlMain.Map.LayerCount == 0) 
                {
                    this.mapCtlMain.SpatialReference = null;
                    this.StatusBar.Panels[2].Text = "";
                    this.StatusBar.Panels[3].Text = "";

                }
            }
        }

        private void NavigationBar_CommandClick(object sender, Janus.Windows.UI.CommandBars.CommandEventArgs e)
        {

        }

        #region PageLayoutControl事件
        private void axPageLayoutControl1_OnMouseDown(object sender, IPageLayoutControlEvents_OnMouseDownEvent e)
        {
            int m_reszEng;//返回鼠标位置
            IPoint m_point = m_pageLayoutControl.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            m_origenalElement = m_element;
            if (e.button == 1)
            {
                getElement(e.pageX, e.pageY);
                if (e.pageX < -0.03 || e.pageY < -0.03 || e.pageX > 29.71 || e.pageY > 29.71)
                {
                    m_element = null;
                }
                if (m_element != null)
                {
                    m_selectionTracker = m_element.SelectionTracker;
                    if (m_element is IFrameElement || m_element is ITextElement)
                    {
                        m_reszEng = Convert.ToInt32(m_selectionTracker.HitTest(m_point));// 测试点击的位置，并获得位置值

                        if (m_reszEng == 0 || m_reszEng == 1 || m_element is ITextElement) //当鼠标在选中元素的中间部位时
                        {
                            m_selectionTracker = null;
                            m_pageLayoutControl.MousePointer = esriControlsMousePointer.esriPointerSize; //设置鼠标样式为移动元素样式
                        }
                        else if (m_reszEng == 2 || m_reszEng == 3 || m_reszEng == 4 || m_reszEng == 5 || m_reszEng == 6 || m_reszEng == 7 || m_reszEng == 8 || m_reszEng == 9)
                        {
                            m_reszEng = m_reszEng - 2;
                            if (m_reszEng == 4 || m_reszEng == 3)
                                m_pageLayoutControl.MousePointer = esriControlsMousePointer.esriPointerSizeWE;//设置鼠标样式 用来改变元素大小
                            if (m_reszEng == 2 || m_reszEng == 5)
                                m_pageLayoutControl.MousePointer = esriControlsMousePointer.esriPointerSizeNESW;
                            if (m_reszEng == 1 || m_reszEng == 6)
                                m_pageLayoutControl.MousePointer = esriControlsMousePointer.esriPointerSizeNS;
                            if (m_reszEng == 0 || m_reszEng == 7)
                                m_pageLayoutControl.MousePointer = esriControlsMousePointer.esriPointerSizeNWSE;

                            m_resizeEnvelopeFeedback = new ResizeEnvelopeFeedbackClass();

                            m_resizeEnvelopeFeedback.Display = m_pageLayoutControl.ActiveView.ScreenDisplay; //设置Display
                            m_resizeEnvelopeFeedback.ResizeEdge = (esriEnvelopeEdge)m_reszEng;//设置改变大小时，需要移动的边


                            m_resizeEnvelopeFeedback.Constraint = esriEnvelopeConstraints.esriEnvelopeConstraintsNone;
                            m_resizeEnvelopeFeedback.Start(m_element.Geometry.Envelope, m_point); //设置移动的启动位置
                            // m_resizeEnvelopeFeedback.MoveTo(m_point);
                        }

                    }
                }
                if (m_element == null && m_origenalElement != null)//当没有选中元素时，将原来选择的元素跟踪包络线隐藏
                {
                    m_selectionTracker = m_origenalElement.SelectionTracker;
                    m_selectionTracker.Deactivate();//隐藏包络线

                }

            }
            #region//右击
            if (e.button == 2)  //右击鼠标
            {
                if (m_resizeEnvelopeFeedback != null)
                {
                    m_resizeEnvelopeFeedback.Constraint = esriEnvelopeConstraints.esriEnvelopeConstraintsSquare;
                }
                #region// 右键菜单
                m_menuPageLayout.AddItem(new DeleteElement(m_pageLayoutControl, m_element), -1, 7,
                    false, esriCommandStyles.esriCommandStyleIconOnly);//删除选中元素

                m_menuPageLayout.AddItem(new ChangeFrameBackGround(m_element, m_pageLayoutControl), 8, 8,
               false, esriCommandStyles.esriCommandStyleIconOnly);//设置背景
                m_menuPageLayout.AddItem(new ChangeFrameShadow(m_element, m_pageLayoutControl), 9, 9,
                    false, esriCommandStyles.esriCommandStyleIconOnly);//设置阴影
                m_menuPageLayout.AddItem(new ChangeFrameBorder(m_element, m_pageLayoutControl), 10, 10,
                    false, esriCommandStyles.esriCommandStyleIconOnly);//设置边框
                m_menuPageLayout.AddItem(new CmdExoprtMapAsPicture(), 11, 11, false, esriCommandStyles.esriCommandStyleIconOnly);
                m_menuPageLayout.PopupMenu(e.x, e.y, m_pageLayoutControl.hWnd);
                m_menuPageLayout.Remove(11);
                m_menuPageLayout.Remove(10);
                m_menuPageLayout.Remove(9);
                m_menuPageLayout.Remove(8);
                m_menuPageLayout.Remove(7);

                #endregion
            }
            #endregion
        }

        private void axPageLayoutControl1_OnAfterDraw(object sender, IPageLayoutControlEvents_OnAfterDrawEvent e)
        {
            if (e.viewDrawPhase.Equals(32)) //设置元素跟踪显示
            {

                if (m_element == null && m_origenalElement != null)//当没有选中元素时，将原来选择的元素跟踪包络线隐藏
                {
                    m_selectionTracker = m_origenalElement.SelectionTracker;
                    m_selectionTracker.Deactivate();//隐藏包络线

                    return;
                }
                if (m_element == null)
                {
                    return;
                }
                ISelectionTracker m_SelectionTracker;
                m_SelectionTracker = m_element.SelectionTracker;
                m_SelectionTracker.Draw(e.display as IDisplay, 0, esriTrackerStyle.esriTrackerDominant);//显示元素跟踪包络线
            }
        }

        private void axPageLayoutControl1_OnMouseMove(object sender, IPageLayoutControlEvents_OnMouseMoveEvent e)
        {
            //  m_designPageLayout = new DesignPageLayout(m_mapControl, m_pageLayoutControl);

            if (m_pageLayoutControl.MousePointer == esriControlsMousePointer.esriPointerSize)
            {
                m_designPageLayout.fMoveElement(e.pageX, e.pageY); //移动元素
            }
            if (m_pageLayoutControl.MousePointer == esriControlsMousePointer.esriPointerSizeNESW ||
                m_pageLayoutControl.MousePointer == esriControlsMousePointer.esriPointerSizeNS ||
                m_pageLayoutControl.MousePointer == esriControlsMousePointer.esriPointerSizeNWSE ||
                m_pageLayoutControl.MousePointer == esriControlsMousePointer.esriPointerSizeWE)     //resize元素
            {
                IPoint m_point = m_pageLayoutControl.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);


                if (m_resizeEnvelopeFeedback != null)
                {
                    m_resizeEnvelopeFeedback.MoveTo(m_point); //移动终点
                }
            }
        }

        private void axPageLayoutControl1_OnMouseUp(object sender, IPageLayoutControlEvents_OnMouseUpEvent e)
        {
            //移动结束，将选中需要移动的元素归零，刷新地图
            m_designPageLayout.m_elementToMove = null; //归零

            IEnvelope m_envelopeResult;
            IGraphicsContainer m_graphicsContainer;

            if (m_resizeEnvelopeFeedback != null)
            {
                m_envelopeResult = m_resizeEnvelopeFeedback.Stop();//停止移动
                if (m_envelopeResult != null)
                {
                    m_element.Geometry = m_envelopeResult;//将改变大小后新的几何形状赋给元素

                    m_graphicsContainer = m_pageLayoutControl.GraphicsContainer;

                    m_graphicsContainer.UpdateElement(m_element);//显示Resize后的元素

                }
                m_pageLayoutControl.ActiveView.Refresh();
                m_resizeEnvelopeFeedback = null;
            }
            m_pageLayoutControl.MousePointer = esriControlsMousePointer.esriPointerDefault;//设置鼠标样式为默认样式
        }

        private void axPageLayoutControl1_OnDoubleClick(object sender, IPageLayoutControlEvents_OnDoubleClickEvent e)
        {
            if (m_element != null)
            {
                IElementProperties pElementProp = m_element as IElementProperties;

                if (pElementProp.Type == "Text")
                {
                    ITextElement pTextElement = pElementProp as ITextElement;
                    frmTextSymbol frmText = new frmTextSymbol(ref pTextElement);
                    frmText.ShowDialog();
                    pElementProp = pTextElement as IElementProperties;

                    m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphicSelection, null, null);
                    m_pageLayoutControl.ActiveView.Refresh();
                }
                else if (pElementProp.Type == "Map Surround Frame")
                {
                    IMapSurroundFrame pMapSurrounFrame = pElementProp as IMapSurroundFrame;
                    if (pMapSurrounFrame.MapSurround is ILegend)
                    {
                        frmLegendWizard frmLegedWizard = new frmLegendWizard(m_mapControl, m_pageLayoutControl);
                        frmLegedWizard.ShowDialog();


                    }
                    else if (pMapSurrounFrame.MapSurround is IScaleBar)
                    {
                        ESRI.ArcGIS.Controls.esriSymbologyStyleClass styleClass = esriSymbologyStyleClass.esriStyleClassScaleBars;
                        frmLegendItemSymbol frmScaleBar = new frmLegendItemSymbol();
                        IStyleGalleryItem pstyGallertItem = frmScaleBar.GetItem(styleClass);
                        IScaleBar pscaleBar = pMapSurrounFrame.MapSurround as IScaleBar;
                        esriUnits punits = pscaleBar.Units;
                        if (pstyGallertItem != null)

                            pscaleBar = (IScaleBar)pstyGallertItem.Item;
                        else
                            return;
                        pscaleBar.Map = m_mapControl.ActiveView.FocusMap;
                        pscaleBar.Units = punits;
                        pscaleBar.UseMapSettings();
                        pscaleBar.Refresh();
                        pMapSurrounFrame.MapSurround = pscaleBar as IMapSurround;
                        m_pageLayoutControl.Refresh(esriViewDrawPhase.esriViewGraphics, null, null);
                    }
                    else if (pMapSurrounFrame.MapSurround is IScaleText)
                    {
                        frmLegendItemSymbol frmTextScal = new frmLegendItemSymbol();
                        IStyleGalleryItem pStyGalleryItem = frmTextScal.GetItem(esriSymbologyStyleClass.esriStyleClassScaleTexts);
                        IScaleText pScaleText = pMapSurrounFrame.MapSurround as IScaleText;
                        esriUnits pUnits = pScaleText.MapUnits;
                        if (pStyGalleryItem != null)
                            pScaleText = (IScaleText)pStyGalleryItem.Item;
                        else
                            return;
                        pScaleText.Map = m_pageLayoutControl.ActiveView.FocusMap;
                        pScaleText.MapUnits = pScaleText.MapUnits;
                        pMapSurrounFrame.MapSurround = pScaleText as IMapSurround;
                        m_pageLayoutControl.Refresh(esriViewDrawPhase.esriViewGraphics, null, null);

                    }
                    else if (pMapSurrounFrame.MapSurround is INorthArrow)
                    {
                        INorthArrow northArrow = pMapSurrounFrame.MapSurround as INorthArrow;
                        frmNorthArrowSymbol frmNorthArrow = new frmNorthArrowSymbol();
                        IStyleGalleryItem pstyGallertItem = frmNorthArrow.GetItem(ESRI.ArcGIS.Controls.esriSymbologyStyleClass.esriStyleClassNorthArrows); ;
                        if (pstyGallertItem != null)
                        {
                            northArrow = (INorthArrow)pstyGallertItem.Item;
                            northArrow.CalibrationAngle = frmNorthArrow.Angle();

                        }
                        else
                        {
                            return;
                        }
                        pMapSurrounFrame.MapSurround = (IMapSurround)northArrow;
                        m_pageLayoutControl.Refresh(esriViewDrawPhase.esriViewGraphics, null, null);
                    }
                }
            }
        }

        private void axPageLayoutControl1_OnKeyDown(object sender, IPageLayoutControlEvents_OnKeyDownEvent e)
        {
            #region
            if (m_element != null)
            {
                if (e.keyCode == 46)//删除选中元素，按delete键 时执行删除元素
                {
                    if (m_element is IMapFrame)
                    {

                        m_pageLayoutControl.GraphicsContainer.DeleteAllElements();//当选中的元素是Data Frame 时，删除所有的元素（包括图例，比例尺等），同时删除所有图层；
                        m_pageLayoutControl.ActiveView.Refresh();
                        m_pageLayoutControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
                    }
                    else
                    {
                        m_pageLayoutControl.GraphicsContainer.DeleteElement(m_element);//删除选择元素
                        m_element = null;
                        m_designPageLayout.m_elementToMove = null;
                        m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);//刷新
                        m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                        m_pageLayoutControl.ActiveView.Refresh();
                        m_pageLayoutControl.MousePointer = esriControlsMousePointer.esriPointerDefault;//默认鼠标样式
                    }
                }
            }
            #endregion
        }
        #endregion

        #region 打印
        private void InitializePrintPreviewDialog() //初始化打预览对话框
        {
            // create a new PrintPreviewDialog using constructor
            printPreviewDialog1 = new PrintPreviewDialog();
            //set the size, location, name and the minimum size the dialog can be resized to
            printPreviewDialog1.ClientSize = new System.Drawing.Size(800, 600);
            printPreviewDialog1.Location = new System.Drawing.Point(29, 29);
            printPreviewDialog1.Name = "PrintPreviewDialog1";
            printPreviewDialog1.MinimumSize = new System.Drawing.Size(375, 250);
            //set UseAntiAlias to true to allow the operating system to smooth fonts
            printPreviewDialog1.UseAntiAlias = true;

            //associate the event-handling method with the document's PrintPage event
            this.document.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(document_PrintPage);
        }

        private void InitializePageSetupDialog()//初始化页面设置对话框
        {
            //create a new PageSetupDialog using constructor
            pageSetupDialog1 = new PageSetupDialog();
            //initialize the dialog's PrinterSettings property to hold user defined printer settings
            pageSetupDialog1.PageSettings = new System.Drawing.Printing.PageSettings();
            //initialize dialog's PrinterSettings property to hold user set printer settings
            pageSetupDialog1.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
            //do not show the network in the printer dialog
            pageSetupDialog1.ShowNetwork = false;
        }

        private void document_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)//打印出图
        {
            string sPageToPrinterMapping = "esriPageMappingTile";// (string)this.toolStripComboBox1.SelectedItem;
            if (sPageToPrinterMapping == null)
                //if no selection has been made the default is tiling
                axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingTile;
            else if (sPageToPrinterMapping.Equals("esriPageMappingTile"))
                axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingTile;
            else if (sPageToPrinterMapping.Equals("esriPageMappingCrop"))
                axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingCrop;
            else if (sPageToPrinterMapping.Equals("esriPageMappingScale"))
                axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingScale;
            else
                axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingTile;

            //get the resolution of the graphics device used by the print preview (including the graphics device)
            short dpi = (short)e.Graphics.DpiX;
            //envelope for the device boundaries
            IEnvelope devBounds = new EnvelopeClass();
            //get page
            IPage page = axPageLayoutControl1.Page;

            //the number of printer pages the page will be printed on
            short printPageCount;
            printPageCount = axPageLayoutControl1.get_PrinterPageCount(0);
            m_CurrentPrintPage++;

            //the currently selected printer
            IPrinter printer = axPageLayoutControl1.Printer;
            //get the device bounds of the currently selected printer
            page.GetDeviceBounds(printer, m_CurrentPrintPage, 0, dpi, devBounds);

            //structure for the device boundaries
            tagRECT deviceRect;
            //Returns the coordinates of lower, left and upper, right corners
            double xmin, ymin, xmax, ymax;
            devBounds.QueryCoords(out xmin, out ymin, out xmax, out ymax);
            //initialize the structure for the device boundaries
            deviceRect.bottom = (int)ymax;
            deviceRect.left = (int)xmin;
            deviceRect.top = (int)ymin;
            deviceRect.right = (int)xmax;

            //determine the visible bounds of the currently printed page
            IEnvelope visBounds = new EnvelopeClass();
            page.GetPageBounds(printer, m_CurrentPrintPage, 0, visBounds);

            //get a handle to the graphics device that the print preview will be drawn to
            IntPtr hdc = e.Graphics.GetHdc();

            //print the page to the graphics device using the specified boundaries 
            axPageLayoutControl1.ActiveView.Output(hdc.ToInt32(), dpi, ref deviceRect, visBounds, m_TrackCancel);

            //release the graphics device handle
            e.Graphics.ReleaseHdc(hdc);

            //check if further pages have to be printed
            if (m_CurrentPrintPage < printPageCount)
                e.HasMorePages = true; //document_PrintPage event will be called again
            else
                e.HasMorePages = false;

        }
        #endregion

        private void axTOCControl1_OnDoubleClick(object sender, ITOCControlEvents_OnDoubleClickEvent e)
        {
            IBasicMap map = null;//设置要素符号
            object other = null;
            object index = null;
            esriTOCControlItem item = new esriTOCControlItem();
            this.axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref m_layer, ref other, ref index);
            if (item == esriTOCControlItem.esriTOCControlItemLegendClass)
            {
                ILegendGroup legendGroup = other as ILegendGroup;
                ILegendClass legendClass = legendGroup.get_Class(Convert.ToInt32(index));
                ISymbol symbol = legendClass.Symbol;
                ISymbolSelector pSS = new SymbolSelectorClass();
                pSS.AddSymbol(symbol);
                Boolean bOK = pSS.SelectSymbol(0);
                if (bOK == true)
                {
                    legendClass.Symbol = pSS.GetSymbolAt(0);
                }
                axTOCControl1.Update();
                this.mapCtlMain.Refresh();
            }
        }

        public void ShowSDETree() //空间数据库树的显示
        {
            if (this.treeVSDE.Nodes.Count > 0)
            {
                this.treeVSDE.Nodes.Clear();
            }

            TreeNode secondNodeInter = new TreeNode("解译成果数据");
            secondNodeInter.ImageIndex = 32;
            secondNodeInter.SelectedImageIndex = 32;
            secondNodeInter.Tag = "InterpDB";
            this.treeVSDE.Nodes.Add(secondNodeInter);

            TreeNode secondNodeSHP = new TreeNode("基础地理数据");
            secondNodeSHP.ImageIndex = 32;
            secondNodeSHP.SelectedImageIndex = 32;
            secondNodeSHP.Tag = "VectorDB";
            this.treeVSDE.Nodes.Add(secondNodeSHP);

            TreeNode secondNodeRaster = new TreeNode("遥感影像数据");
            secondNodeRaster.ImageIndex = 32;
            secondNodeRaster.SelectedImageIndex = 32;
            secondNodeRaster.Tag = "RasterDB";
            this.treeVSDE.Nodes.Add(secondNodeRaster);

            #region 加载影像
            string sqlText = "select distinct [PLACE] from IMAGEMETADATA";
            this.m_oraCmd.CommandText = sqlText;
            OleDbDataReader drDS = m_oraCmd.ExecuteReader();
            IList<string> sl = new List<string>();
            while (drDS.Read())
            {
                sl.Add(drDS.GetValue(0).ToString());
            }
            drDS.Close();

            for (int i = 0; i < sl.Count; i++) 
            {
                TreeNode thirdNodeSL = new TreeNode(sl[i]);
                thirdNodeSL.ImageIndex = 89;
                thirdNodeSL.SelectedImageIndex = 89;
                thirdNodeSL.Tag = "Raster";
                secondNodeRaster.Nodes.Add(thirdNodeSL);

                sqlText = "select [PLACE],[SATELITE],[TIME] from IMAGEMETADATA where [PLACE]='" + sl[i] + "'";
                m_oraCmd.CommandText = sqlText;
                drDS = m_oraCmd.ExecuteReader();
                while(drDS.Read())
                {
                    TreeNode forthNodeRas = new TreeNode(drDS.GetValue(1).ToString() + "_" + drDS.GetValue(2).ToString());
                    forthNodeRas.ImageIndex = 30;
                    forthNodeRas.SelectedImageIndex = 30;
                    forthNodeRas.Tag = "Raster";
                    thirdNodeSL.Nodes.Add(forthNodeRas);
                }
                drDS.Close();
            }
            sl.Clear();
            #endregion 加载影像

            #region 加载地理数据
            sqlText = "select VECTORNAME,Geometry from VECTORMETADATA";
            m_oraCmd.CommandText = sqlText;
            drDS = m_oraCmd.ExecuteReader();
            while (drDS.Read()) 
            {
                TreeNode thirdNode = new TreeNode(drDS.GetValue(0).ToString());
                switch (drDS.GetValue(1).ToString())
                {
                    case "point":
                        thirdNode.ImageIndex = 33;
                        thirdNode.SelectedImageIndex = 33;
                        thirdNode.Tag = "point";
                        break;
                    case "line":
                        thirdNode.ImageIndex = 35;
                        thirdNode.SelectedImageIndex = 35;
                        thirdNode.Tag = "line";
                        break;
                    case "polygon":
                        thirdNode.ImageIndex = 34;
                        thirdNode.SelectedImageIndex = 34;
                        thirdNode.Tag = "polygon";
                        break;
                }
                secondNodeSHP.Nodes.Add(thirdNode);
                
            }
            drDS.Close();
            #endregion 加载地理数据

            sqlText = "select distinct [PLACE] from INTERPDATA";
            this.m_oraCmd.CommandText = sqlText;
            drDS = m_oraCmd.ExecuteReader();
            while (drDS.Read())
            {
                TreeNode thirdNodeInterp = new TreeNode(drDS.GetValue(0).ToString());
                thirdNodeInterp.ImageIndex = 89;
                thirdNodeInterp.SelectedImageIndex = 89;
                thirdNodeInterp.Tag = "PLACE";
                secondNodeInter.Nodes.Add(thirdNodeInterp);
            }
            drDS.Close();

            for (int i = 0; i < secondNodeInter.Nodes.Count; i++)
            {
                sqlText = "select [PLACE],[YEARDATE],[TYPE],[GEOMETRY] from INTERPDATA where [PLACE]='" + secondNodeInter.Nodes[i].Text.ToString() + "'";
                this.m_oraCmd.CommandText = sqlText;
                drDS = m_oraCmd.ExecuteReader();
                while (drDS.Read())
                {
                    TreeNode forthNodeInterp = new TreeNode(drDS.GetValue(2).ToString() + "_" + drDS.GetValue(1).ToString());
                    switch (drDS.GetValue(3).ToString())
                    {
                        case "point":
                            forthNodeInterp.ImageIndex = 33;
                            forthNodeInterp.SelectedImageIndex = 33;
                            forthNodeInterp.Tag = "point";
                            break;
                        case "line":
                            forthNodeInterp.ImageIndex = 35;
                            forthNodeInterp.SelectedImageIndex = 35;
                            forthNodeInterp.Tag = "line";
                            break;
                        case "polygon":
                            forthNodeInterp.ImageIndex = 34;
                            forthNodeInterp.SelectedImageIndex = 34;
                            forthNodeInterp.Tag = "polygon";
                            break;
                    }
                    secondNodeInter.Nodes[i].Nodes.Add(forthNodeInterp);
                }
                drDS.Close();
            }




            this.treeVSDE.ExpandAll();
        }

        private void treeVSDE_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (this.SearchPanel.AutoHide == false)
            {
                this.SearchPanel.AutoHide = true;
            }
            this.dataGridView1.DataSource = null;
            this.SearchPanel.Text = "属性数据浏览";

            this.ExportToSHP.Enabled = false;
            this.ExportToRaster.Enabled = false;
            this.AddToCallMap.Enabled = false;
            this.Delete.Enabled = true;
            this.SelectBand.Enabled = false;
            System.Drawing.Point point = new System.Drawing.Point(e.X, e.Y);



            if (e.Button == MouseButtons.Right)
            {
                if (e.Node.Tag.ToString() == "point" || e.Node.Tag.ToString() == "line" || e.Node.Tag.ToString() == "polygon" ||e.Node.Tag.ToString() == "Annotation")
                {
                    this.ExportToSHP.Enabled = true;
                    this.AddToCallMap.Enabled = true;
                    this.conMenuTree.Show(this.treeVSDE, point);
                    this.m_node = e.Node;
                }
                if (e.Node.Tag.ToString() == "Raster")
                {
                    this.ExportToRaster.Enabled = true;
                    this.AddToCallMap.Enabled = true;
                    this.SelectBand.Enabled = true;
                    this.conMenuTree.Show(this.treeVSDE, point);
                    this.m_node = e.Node;
                }

            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定删除此此项吗？", "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                IFeatureClass feaClass = null;
                IRasterDataset rasDS = null;
                string sqlText = "";
                switch (this.m_node.Tag.ToString())
                {
                    case "point":
                    case "line":
                    case "polygon":
                        feaClass = ((IFeatureWorkspace)this.m_workSpace).OpenFeatureClass(this.m_node.Parent.Text.ToString()+"_"+ this.m_node.Text);
                        IDataset ds = feaClass as IDataset;
                        ds.Delete();
                        //ShowSDETree();
                        sqlText = "delete from VECTORMETADATA where VECTORNAME='" + this.m_node.Text.ToString() + "'";
                        this.m_oraCmd.CommandText = sqlText;
                        this.m_oraCmd.ExecuteNonQuery();
                        break;
                   
                    case "Raster":
                        rasDS = ((IRasterWorkspaceEx)this.m_workSpace).OpenRasterDataset(this.m_node.Parent.Text.ToString() + "_" + this.m_node.Text);
                        ((IDataset)rasDS).Delete();
                        ShowSDETree();
                       // System.IO.File.Delete(Application.StartupPath + "\\temp" + this.m_node.Text.ToString());
                        sqlText = "delete from IMAGEMETADATA where IMAGENAME='" + this.m_node.Text.ToString() + "'";
                        this.m_oraCmd.CommandText = sqlText;
                        this.m_oraCmd.ExecuteNonQuery();
                        break;
                }
                ShowSDETree();
                this.m_node = null;
            }

        }

        #region 编辑要素函数
        private void StartEditing() //开始编辑
        {
            if (m_CurrentLayer == null)
                return;
            if ((IGeoFeatureLayer)m_CurrentLayer == null)
                return;
            IFeatureLayer m_FeatureLayer = (IFeatureLayer)m_CurrentLayer;
            IDataset m_DataSet = (IDataset)m_FeatureLayer.FeatureClass;
            if (m_DataSet == null)
                return;

            switch (uiComboBox3.SelectedIndex)
            {
                case 0:
                    m_bSketch = false;
                    m_bModify = false;
                    return;

                case 1:
                    mapCtlMain.MousePointer = esriControlsMousePointer.esriPointerPencil;
                    m_bSketch = true;
                    m_bModify = false;
                    break;

                case 2:
                    mapCtlMain.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                    m_bModify = true;
                    m_bSketch = false;
                    break;
            }
            IWorkspaceEdit m_WorkspaceEdit = (IWorkspaceEdit)m_DataSet.Workspace;
            if (!m_WorkspaceEdit.IsBeingEdited())
            {
                try
                {
                    m_WorkspaceEdit.StartEditing(true);
                    m_WorkspaceEdit.EnableUndoRedo();
                }
                catch { }

            }
        }

        private int SaveEditing() //保存编辑
        { 
            //先停止编辑
            if (m_CurrentLayer == null)
                return 0;
            IFeatureLayer m_FeatureLayer = (IFeatureLayer)m_CurrentLayer;
            if (m_FeatureLayer.FeatureClass == null)
                return 0;
            IDataset m_Dataset = (IDataset)m_FeatureLayer.FeatureClass;
            if (m_Dataset == null)
                return 0;

            IWorkspaceEdit m_WorkspaceEditng = (IWorkspaceEdit)m_Dataset.Workspace;
            if (m_WorkspaceEditng.IsBeingEdited())
            {
                bool bHasEdits = false;
                m_WorkspaceEditng.HasEdits(ref bHasEdits);
                bool bSave = false;

                if (bHasEdits)
                {
                    DialogResult result;
                    result = MessageBox.Show(this, "需要保存当前的编辑啊？", "保存",
                       MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (DialogResult.Yes == result)
                        bSave = true;
                    else if (DialogResult.Cancel == result)
                        return 0;
                }
                m_WorkspaceEditng.StopEditing(bSave);
            }
            //再启动编辑
            if (!m_WorkspaceEditng.IsBeingEdited())
            {
                m_WorkspaceEditng.StartEditing(true);
                m_WorkspaceEditng.EnableUndoRedo();
            }
            return 1;

        }

        private int StopEditing() //停止编辑
        {
            if (m_CurrentLayer == null)
                return 0;
            IFeatureLayer m_FeatureLayer = (IFeatureLayer)m_CurrentLayer;
            if (m_FeatureLayer.FeatureClass == null)
                return 0;
            IDataset m_Dataset = (IDataset)m_FeatureLayer.FeatureClass;
            if (m_Dataset == null)
                return 0;

            IWorkspaceEdit m_WorkspaceEditng = (IWorkspaceEdit)m_Dataset.Workspace;
            if (m_WorkspaceEditng.IsBeingEdited())
            {
                bool bHasEdits = false;
                m_WorkspaceEditng.HasEdits(ref bHasEdits);
                bool bSave = false;

                if (bHasEdits)
                {
                    DialogResult result;
                    result = MessageBox.Show(this, "需要保存当前的编辑吗？", "保存",
                       MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (DialogResult.Yes == result)
                        bSave = true;
                    else if (DialogResult.Cancel == result)
                        return 0;

                }
                m_WorkspaceEditng.StopEditing(bSave);
            }
            m_bSketch = false;

            m_bModify = false;
            IActiveView m_ActiveView = (IActiveView)m_Map;
            m_ActiveView.Refresh();
            return 1;
        }

        private bool InEdit()
        {
            // 检查当前地图是否具备编辑条件
            if (m_CurrentLayer == null) return false;
            IFeatureLayer pFeatureLayer = (IFeatureLayer)m_CurrentLayer;
            if (pFeatureLayer.FeatureClass == null) return false;
            IDataset pDataset = (IDataset)pFeatureLayer.FeatureClass;
            if (pDataset == null) return false;
            IWorkspaceEdit pWorkspaceEdit = (IWorkspaceEdit)pDataset.Workspace;
            if (pWorkspaceEdit.IsBeingEdited()) return true;
            return false;
        }
        #endregion 

        private void uiComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            // this.menuSketchTool.Enabled = Janus.Windows.UI.InheritableBoolean.True;
            mapCtlMain.MousePointer = esriControlsMousePointer.esriPointerDefault;
            m_bModify = false;
            m_bSketch = false;
            m_bEditNode = false;
            //选择编辑任务
            switch (uiComboBox4.SelectedIndex)
            {
                case 0:
                    mapCtlMain.MousePointer = esriControlsMousePointer.esriPointerPencil;
                    m_bSketch = true;
                    m_bModify = false;
                    m_bEditNode = false;
                    break;

                case 1:
                    mapCtlMain.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                    m_bModify = true;
                    m_bSketch = false;
                    m_bEditNode = false;
                    break;

                case 2:
                    mapCtlMain.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                    m_bModify = false;
                    m_bSketch = false;
                    m_bEditNode = true;
                    break;
            }
            if (m_bSketch)
            {
                CreatShape m_CreatShape = new CreatShape(m_CurrentLayer,this.m_dig);
                m_CreatShape.OnCreate(this.mapCtlMain.Object);
                mapCtlMain.CurrentTool = (ITool)m_CreatShape;
            }
            else if (m_bModify)
            {
                ModifyShape m_ModifyShape = new ModifyShape(m_CurrentLayer);
                m_ModifyShape.OnCreate(this.mapCtlMain.Object);
                mapCtlMain.CurrentTool = (ITool)m_ModifyShape;
            }
            else 
            {
                mapCtlMain.CurrentTool = null;
                m_editor = new AoEditor(m_CurrentLayer, this.m_mapControl.Map);
            }

        }

        private void uiComboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.menuSketch.Enabled = Janus.Windows.UI.InheritableBoolean.True;
            for (int i = 0; i <= m_Map.LayerCount - 1; i++)
            {
                if (uiComboBox5.SelectedIndex!=-1)
                {
                    if (m_Map.get_Layer(i).Name == uiComboBox5.SelectedItem.ToString())
                    {
                        m_CurrentLayer = m_Map.get_Layer(i);
                        break;
                    }
                }


            }
            if (m_bSketch)
            {
                CreatShape m_CreatShape = new CreatShape(m_CurrentLayer,this.m_dig);
                m_CreatShape.OnCreate(this.mapCtlMain.Object);
                mapCtlMain.CurrentTool = (ITool)m_CreatShape;

            }
            else if (m_bModify)
            {
                ModifyShape m_ModifyShape = new ModifyShape(m_CurrentLayer);
                m_ModifyShape.OnCreate(this.mapCtlMain.Object);
                mapCtlMain.CurrentTool = (ITool)m_ModifyShape;
            }
        }

        private void DeleteSelectedFeatures() //删除要素
        {
            if (m_CurrentLayer == null) return;

            // 先检查当前是否有要素被选中
            IFeatureCursor pFeatureCursor = GetSelectedFeatures();       //获取被选中要素
            if (pFeatureCursor == null) return;

            m_Map.ClearSelection();


            // 要素删除方法
            IWorkspaceEdit pWorkspaceEdit = GetWorkspaceEdit();
            pWorkspaceEdit.StartEditOperation();
            IFeature pFeature = pFeatureCursor.NextFeature();
            while (pFeature != null)
            {
                pFeature.Delete();
                pFeature = pFeatureCursor.NextFeature();
            }
            pWorkspaceEdit.StopEditOperation();

            IActiveView pActiveView = (IActiveView)m_Map;
            pActiveView.Refresh();
        }

        private IFeatureCursor GetSelectedFeatures() //获取被选中的要素
        {
            if (m_CurrentLayer == null) return null;

            // 检查是否有要素被选中，没有则MessageBox提醒
            IFeatureSelection pFeatSel = (IFeatureSelection)m_CurrentLayer;
            ISelectionSet pSelectionSet = pFeatSel.SelectionSet;
            if (pSelectionSet.Count == 0)
            {
                MessageBox.Show("在 '" + m_CurrentLayer.Name + "' 图层没有要素被选中！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            // 有则返回要素游标
            ICursor pCursor;
            pSelectionSet.Search(null, false, out pCursor);
            return (IFeatureCursor)pCursor;
        }

        private IWorkspaceEdit GetWorkspaceEdit() //获取当前编辑空间
        {
            if (m_CurrentLayer == null) return null;

            IFeatureLayer m_FeatureLayer = (IFeatureLayer)m_CurrentLayer;
            IFeatureClass m_FeatureClass = m_FeatureLayer.FeatureClass;
            IDataset m_Dataset = (IDataset)m_FeatureClass;
            if (m_Dataset == null) return null;
            return (IWorkspaceEdit)m_Dataset.Workspace;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.rdbMarker.Checked == true&&this.lstBackLayer.SelectedIndex!=-1)//用历史标志回溯
            {
                IFeatureLayer fealayer = m_gdata.AddFeatureClassToMap(this.lstBackLayer.SelectedItem.ToString()+"_h");
                this.mapCtlMain.Map.AddLayer(fealayer);
            }
        }

        private void PageLayoutToolBarInit() //PageLayout右键菜单
        {
            m_menuPageLayout = new ToolbarMenuClass();
            m_menuPageLayout.AddItem(new ControlsPageZoomInToolClass(), 1, 0,false, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.AddItem(new ControlsPageZoomOutToolClass(), 2, 1,false, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.AddItem(new ControlsPagePanToolClass(), 3, 2, false,esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.AddItem(new ControlsPageZoomWholePageCommandClass(), 4,3, false, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.AddItem(new ControlsPageZoomOutFixedCommandClass(), 5, 4,false, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.AddItem(new ControlsPageZoomPageToLastExtentBackCommandClass(), 6, 5,false, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.AddItem(new ControlsPageZoomPageToLastExtentForwardCommandClass(), 7, 6,false, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.SetHook(m_pageLayoutControl);
        }

        private void SetMapFrame()//设置地图基本信息，如比例尺，图例等：
        {

            IGraphicsContainer pGraphicsContainer = (IGraphicsContainer)this.axPageLayoutControl1.ActiveView;
            pGraphicsContainer.Reset();
            IElementProperties pElementProperties = (IElementProperties)pGraphicsContainer.Next();
            while (pElementProperties != null)
            {
                if (pElementProperties.Type == "Map Surround Frame")
                {

                    IMapSurroundFrame pMapSurrounFrame = pElementProperties as IMapSurroundFrame;
                    IElement pElement = (IElement)pElementProperties;

                    if (pMapSurrounFrame.MapSurround.Name.Trim() == "Legend")
                    {
                        ILegend pLegend = pMapSurrounFrame.MapSurround as ILegend;
                        pLegend.Map = this.mapCtlMain.Map;


                    }
                    if (pMapSurrounFrame.MapSurround.Name == "Alternating Scale Bar" || pMapSurrounFrame.MapSurround.Name == "Stepped Scale Line" || pMapSurrounFrame.MapSurround.Name == "Scale Line" ||
                        pMapSurrounFrame.MapSurround.Name == "Hollow Scale Bar" || pMapSurrounFrame.MapSurround.Name == "Single Division Scale Bar" ||
                        pMapSurrounFrame.MapSurround.Name == "Double Alternating Scale Bar")
                    {
                        // m_Map.MapScale = 5000;
                        IScaleBar pScaleBar = pMapSurrounFrame.MapSurround as IScaleBar;
                        pScaleBar.Map = this.mapCtlMain.Map;


                        pScaleBar.Units = this.mapCtlMain.Map.MapUnits;
                        pScaleBar.UseMapSettings();

                        pElement = pMapSurrounFrame.MapSurround as IElement;

                    }
                    if (pMapSurrounFrame.MapSurround.Name == "Scale Text")
                    {
                        IScaleText pScaleText = pMapSurrounFrame.MapSurround as IScaleText;
                        pScaleText.Map = this.mapCtlMain.Map;
                    }
                    if (pMapSurrounFrame.MapSurround.Name == "North Arrow")
                    {
                        INorthArrow pNorthArrow = pMapSurrounFrame.MapSurround as INorthArrow;

                        pNorthArrow.Map = this.mapCtlMain.Map;
                    }
                }
                if (pElementProperties.Type == "Data Frame")
                {
                    IFrameElement pFrameElement = pElementProperties as IFrameElement;
                    IMapFrame pMapframe = pFrameElement as IMapFrame;
                    // pMapframe.ExtentType = esriExtentTypeEnum.esriExtentBounds; 
                    IMapGrids pmapGrids = pMapframe as IMapGrids;
                    IMapGrid pMapGrid = null;
                    for (int i = 0; i < pmapGrids.MapGridCount; i++)
                    {
                        pMapGrid = pmapGrids.get_MapGrid(i);
                        //用户要求显示与否
                        pMapGrid.Visible = true;
                    }
                }

                pElementProperties = (IElementProperties)pGraphicsContainer.Next();
            }
            if (axPageLayoutControl1.Page.Orientation == 2)
            {
                IPaper paper;
                paper = new PaperClass(); //create a paper object
                IPrinter printer;
                printer = new EmfPrinterClass(); //create a printer object
                paper.Attach(pageSetupDialog1.PrinterSettings.GetHdevmode(pageSetupDialog1.PageSettings).ToInt32(), pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());
                paper.Orientation = 2;
                printer.Paper = paper;
                axPageLayoutControl1.Printer = printer;

                pageSetupDialog1.PageSettings.Landscape = true;
                pageSetupDialog1.AllowOrientation = true;


            }
            if (axPageLayoutControl1.Page.Orientation == 1 && axPageLayoutControl1.Printer.Paper.Orientation == 2)
            {
                IPaper paper;
                paper = new PaperClass(); //create a paper object
                IPrinter printer;
                printer = new EmfPrinterClass(); //create a printer object
                paper.Attach(pageSetupDialog1.PrinterSettings.GetHdevmode(pageSetupDialog1.PageSettings).ToInt32(), pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());
                paper.Orientation = 1;
                printer.Paper = paper;
                axPageLayoutControl1.Printer = printer;
                pageSetupDialog1.PageSettings.Landscape = false;
                pageSetupDialog1.AllowOrientation = true;
            }

        }

        private void getElement(double X, double Y)
        {

            IGraphicsContainer pGraphicsContainer = (IGraphicsContainer)this.axPageLayoutControl1.ActiveView;
            IGraphicsContainerSelect m_graphicsContainerSelect = pGraphicsContainer as IGraphicsContainerSelect;
            IElement pElement, p_SelectElement = null;
            IGeometry pGeometry;
            IMapFrame pmapframe = pGraphicsContainer.FindFrame(m_pageLayoutControl.ActiveView.FocusMap) as IMapFrame;
            IFrameElement pFrameElement = pmapframe as IFrameElement;
            bool m_ok = false;
            pGraphicsContainer.Reset();
            IElementProperties pElementProperties = (IElementProperties)pGraphicsContainer.Next();
            while (pElementProperties != null)  //选择元素
            {
                // MessageBox.Show(pElementProperties.Type.ToString());
                if (pElementProperties.Type == "Text")
                {

                    pElement = (IElement)pElementProperties;
                    pGeometry = pElement.Geometry;
                    m_ok = pElement.HitTest(X, Y, 0.1);
                    if (m_ok)
                    {
                        m_graphicsContainerSelect.SelectElement(pElement);
                        m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
                        p_SelectElement = pElement;
                    }
                }

                else if (pElementProperties.Type == "Map Surround Frame")
                {
                    IMapSurroundFrame pMapSurrounFrame = pElementProperties as IMapSurroundFrame;
                    string pMapSurroundName = pMapSurrounFrame.MapSurround.Name;
                    if (pMapSurroundName == "Legend" || pMapSurroundName == "AlternatingScaleBar" || pMapSurroundName == "HollowScaleBar" ||
                        pMapSurroundName == "DoubleAlternatingScale Bar" || pMapSurroundName == "ScaleLine" || pMapSurroundName == "SingleDivisionScaleBar" ||
                        pMapSurroundName == "SteppedScaleLine" || pMapSurroundName == "ScaleText" || pMapSurroundName == "NorthArrow")
                    {
                        pElement = (IElement)pElementProperties;
                        pGeometry = pElement.Geometry;
                        m_ok = pElement.HitTest(X, Y, 0.1);
                        if (m_ok)
                        {
                            m_graphicsContainerSelect.SelectElement(pElement);
                            m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
                            p_SelectElement = pElement;
                        }
                    }
                }
                pElementProperties = pGraphicsContainer.Next() as IElementProperties;

            }
            if (p_SelectElement == null)
            {
                m_element = pFrameElement as IElement;
                m_designPageLayout = new DesignPageLayout(m_pageLayoutControl);
                m_designPageLayout.m_elementToMove = m_element;
            }
            else
            {
                m_element = p_SelectElement;
                m_designPageLayout = new DesignPageLayout(m_pageLayoutControl);
                m_designPageLayout.m_elementToMove = m_element;

            }


            m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);



        }

        #region MapControl事件
        private void mapCtlMain_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (m_bEditNode) 
            {
                m_editor.EditFeatureMouseMove(e.x, e.y);
            }

            ILayer lyr;
            for (int i = 0; i < this.mapCtlMain.LayerCount; i++)
            {
                lyr = this.mapCtlMain.get_Layer(i);
                if (lyr is ITinLayer)
                {
                    this.m_tin = ((ITinLayer)lyr).Dataset;
                }
            }
            if (m_tin != null)
            {
                IPoint point = new PointClass();
                point.X = e.mapX;
                point.Y = e.mapY;
                ISurface sur = m_tin as ISurface;
                double z = sur.GetElevation(point);
                string zStr = ",Z=" + z.ToString("#######.##");
                this.StatusBar.Panels[2].Text = string.Format("{0}, {1}  {2}  {3}", "X=" + e.mapX.ToString("#######.##"), "Y=" + e.mapY.ToString("#######.##"), zStr, this.mapCtlMain.MapUnits.ToString().Substring(4));

            }
            else 
            {
                this.StatusBar.Panels[2].Text = string.Format("{0}, {1}  {2}", "X=" + e.mapX.ToString("#######.##"), "Y=" + e.mapY.ToString("#######.##"), this.mapCtlMain.MapUnits.ToString().Substring(4));
            }
            string XN = e.mapX.ToString();
            string YE = e.mapY.ToString();
            this.PlanarToGeoCoor(ref XN, ref YE);
            this.StatusBar.Panels[3].Text = YE + " N   " + XN + " E";
        }

        private void mapCtlMain_OnAfterDraw(object sender, IMapControlEvents2_OnAfterDrawEvent e)
        {
            if (this.mapCtlMain.Map.SelectionCount > 0)
            {
                this.menuClearSel1.Enabled = Janus.Windows.UI.InheritableBoolean.True;
            }
            else
            {
                this.menuClearSel1.Enabled = Janus.Windows.UI.InheritableBoolean.False;
            }
            if (m_bEditNode) 
            {
                IGeometry pGeom;
                IEnumFeature pSelected = (IEnumFeature)mapCtlMain.Map.FeatureSelection;
                IFeature pFeature = pSelected.Next();
                while (pFeature != null) 
                {
                    pGeom = pFeature.Shape as IGeometry;
                    m_editor.DrawEditSymbol(pGeom, (IDisplay)e.display);
                    pFeature = pSelected.Next();
                }
                
            }
        }

        private void mapCtlMain_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            if (m_bEditNode)
            {
                m_editor.EditFeatureEnd();
            }
        }

        private void mapCtlMain_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (this.m_rasterMeasureDis) //距离量算
            {
                IPolyline polyline = null;
                IGraphicsContainer graCont = (IGraphicsContainer)mapCtlMain.ActiveView;
                if (e.button == 1)
                {
                    double Dis = 0;
                    IRubberBand rubberPoly = new RubberLineClass();
                    polyline = (IPolyline)rubberPoly.TrackNew(mapCtlMain.ActiveView.ScreenDisplay, null);
                    IElement elem;
                    if (polyline != null)
                    {
                        elem = new LineElementClass();
                        elem.Geometry = polyline;
                        graCont.AddElement(elem, 0);
                    }
                    mapCtlMain.ActiveView.Refresh();
                    Dis = polyline.Length;
                    Dis = Dis / 1000;
                    frmDis frmdis = new frmDis("测量长度是：" + Dis.ToString("f3") + "公里", "距离量测");
                    frmdis.ShowDialog();
                    //MessageBox.Show("测量长度是：" + Dis.ToString("f3") + "公里", "确认");
                    graCont = (IGraphicsContainer)mapCtlMain.ActiveView;
                    graCont.DeleteAllElements();
                    mapCtlMain.ActiveView.Refresh();
                }

            }

            if (this.m_rasterMeasureArea) //面积量算
            {
                double Area = 0;
                double Area1 = 0;
                ISimpleLineSymbol lineSymbol = new SimpleLineSymbolClass();
                IRgbColor color = new RgbColorClass();
                color.Red = 0;
                color.Green = 0;
                color.Blue = 225;

                lineSymbol.Width = 1;
                lineSymbol.Color = color;
                lineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                //Dim pPolySymbol As ISimpleFillSymbol
                ISimpleFillSymbol polySymbol = new SimpleFillSymbolClass();

                polySymbol.Color = color;
                polySymbol.Outline = lineSymbol;
                polySymbol.Style = esriSimpleFillStyle.esriSFSHollow;
                //Dim pSymbol As ISymbol
                ISymbol symbol = (ISymbol)polySymbol;
                //设置结束
                IGraphicsContainer graCont = (IGraphicsContainer)mapCtlMain.ActiveView;
                IRubberBand rubberPoly = new RubberPolygonClass();
                IPolygon polygon = (IPolygon)rubberPoly.TrackNew(mapCtlMain.ActiveView.ScreenDisplay, symbol);
                IElement elem;
                if (polygon != null)
                {
                    elem = new PolygonElementClass();
                    elem.Geometry = polygon;
                    IFillShapeElement fillShapeElement = (IFillShapeElement)elem;
                    fillShapeElement.Symbol = (IFillSymbol)symbol;
                    graCont.AddElement(elem, 0);
                }
                mapCtlMain.ActiveView.Refresh();
                IArea area = (IArea)polygon;
                Area = Math.Abs(area.Area) / 1000000;
                Area1 = Area * 100;
                frmDis frmdis = new frmDis("测量面积是：" + Area.ToString("f3") + "平方公里" + "\n     (合" + Area1.ToString("f3") + "公顷)","面积量测");
                frmdis.ShowDialog();
                //MessageBox.Show("测量面积是：" + Area.ToString("f3") + "平方公里" + "(合" + Area1.ToString("f3") + "公顷)", "确认");
                graCont = (IGraphicsContainer)mapCtlMain.ActiveView;
                graCont.DeleteAllElements();
                mapCtlMain.ActiveView.Refresh();
            }

            if (this.m_bEditNode)
            {
                m_editor.EditFeatureMouseDown(e.x, e.y);

            }

            if(this.m_out)
            {

               IFeatureLayer featureLayer = this.mapCtlMain.get_Layer(0) as IFeatureLayer;
               IFeatureClass featureClass = featureLayer.FeatureClass;
               IPoint point = mapCtlMain.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
               IGeometry geometry = point as IGeometry;

               ITopologicalOperator pTopo = geometry as ITopologicalOperator;
               IGeometry buffer = pTopo.Buffer(200);
               geometry = buffer.Envelope as IGeometry;

               ISpatialFilter spatialFilter = new SpatialFilterClass();
               spatialFilter.Geometry = geometry;
               spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;

               spatialFilter.GeometryField = featureClass.ShapeFieldName;
               IQueryFilter filter = spatialFilter as IQueryFilter;

               IFeatureCursor cursor = featureClass.Search(filter, false);
               IFeature pfeature = cursor.NextFeature();
               if (pfeature!= null)
               {
                   frmTip tip = new frmTip(pfeature.get_Value(3).ToString(), pfeature.get_Value(4).ToString(),this.m_oraCmd);
                   tip.ShowDialog();
               }


            }
            


            
        }
        #endregion



        private void AddToCallMap_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    string sqlText1 = "select NAME,DATASET,DATASTRUCTURE from SHEETMETADATA where NAME='" + this.m_node.Text + "'";
            //    m_oraCmd.CommandText = sqlText1;
            //    OracleDataReader dr = m_oraCmd.ExecuteReader();
            //    while (dr.Read())
            //    {
            //        string sqlText2 = "insert into SHEETNOINDEX (NAME,DATASET,DATATYPE) values('" + dr.GetValue(0) + "','" + dr.GetValue(1) + "','" + dr.GetValue(2) + "')";
            //        m_oraCmd.CommandText = sqlText2;
            //        m_oraCmd.ExecuteNonQuery();
            //        MessageBox.Show("添加成功！");
            //    }
            //}
            //catch (Exception ee) 
            //{
            //    MessageBox.Show("该图幅以存在！");
            //}

        }

        private void ExportToSHP_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fdlg = new FolderBrowserDialog();
                fdlg.Description = "选择保存路径";
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    m_gdata.ExportToSHP(this.m_node.Text, fdlg.SelectedPath.ToString());
                    MessageBox.Show("导出成功！");
                }
            }
            catch (Exception ee) 
            {
                MessageBox.Show("导出出错，请检查目标数据！");
            }

        }

        private void ExportToRaster_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fdlg = new FolderBrowserDialog();
            fdlg.Description = "选择保存路径";
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                object selectPath = (object)fdlg.SelectedPath;
                this.ExportRaster(selectPath);
            }

        }

        private void ExportRaster(object selectPath) 
        {
            IRasterWorkspaceEx rasterWS = m_workSpace as IRasterWorkspaceEx;
            IRasterDataset rasterDS = rasterWS.OpenRasterDataset(this.m_node.Text);
            m_gdata.ExportRaster(rasterDS, (string)selectPath, "JPG", this.m_node.Text+".jpg");
            MessageBox.Show("导出成功！");
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            ShowSDETree();
        }

        private void PlanarToGeoCoor(ref string X,ref string Y) 
        {
            IProjectedCoordinateSystem pcs = this.m_mapControl.SpatialReference as IProjectedCoordinateSystem;
            WKSPoint pt = new WKSPoint();
            pt.X = Convert.ToDouble(X);
            pt.Y = Convert.ToDouble(Y);
            pcs.Inverse(1, ref pt);
            int dgree;
            int minute;
            double second;
            double tempValue;

            dgree = (int)pt.X;
            tempValue = Math.Abs(pt.X - dgree) * 60;
            second = Math.Abs(tempValue - (int)tempValue) * 60;
            minute = (int)tempValue;
            X = dgree.ToString() + "°" + minute.ToString() + "'" + second.ToString("#######.##") + "''";

            dgree = (int)pt.Y;
            tempValue = Math.Abs(pt.Y - dgree) * 60;
            second = Math.Abs(tempValue - (int)tempValue) * 60;
            minute = (int)tempValue;
            Y = dgree.ToString() + "°" + minute.ToString() + "'" + second.ToString("#######.##") + "''";
        }

        private void CallMapBar_CommandClick(object sender, Janus.Windows.UI.CommandBars.CommandEventArgs e)
        {

        }

        private void TOCPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.TOCPanel.Visible == false)
            {
                this.menuTOCControl1.Checked = Janus.Windows.UI.InheritableBoolean.False;
            }
            else 
            {
                this.menuTOCControl1.Checked = Janus.Windows.UI.InheritableBoolean.True;
            }
        }

        private void uiCommandManager1_ViewChanged(object sender, EventArgs e)
        {

        }

        private void SearchPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.SearchPanel.Visible == false) 
            {
                this.menuAtrriWin1.Checked = Janus.Windows.UI.InheritableBoolean.False;
            }
            else
            {
                this.menuAtrriWin1.Checked = Janus.Windows.UI.InheritableBoolean.True;
            }
        }

        private void treeVSDE_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag.ToString().IndexOf("sl") != -1 || e.Node.Tag.ToString().IndexOf("tp") != -1)
            {
                e.Node.ImageIndex = 90;
            }
        }

        private void treeVSDE_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag.ToString().IndexOf("sl") != -1 || e.Node.Tag.ToString().IndexOf("tp") != -1)
            {
                e.Node.ImageIndex = 89;
            }
        }

        private void 波段ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void SelectBand_Click_1(object sender, EventArgs e)
        {
            frmSelectBand frmB = new frmSelectBand(this.m_mapControl, (IRasterLayer)this.m_layer,this.m_workSpace);
            frmB.ShowDialog();
        }



    }
}