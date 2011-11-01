using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Printing;


using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;

namespace CoastalGIS.ExportMapProj
{
    public partial class frmPrintByAnyRegion : Form
    {
        private IMap m_Map=null ;
        private IGeometry m_Geometry = null;
        private ICommand m_Command;
        private System.Drawing.Printing.PrintDocument document = new System.Drawing.Printing.PrintDocument();
        private short m_CurrentPrintPage;
        private ITrackCancel m_TrackCancel = new CancelTrackerClass();
        private IPageLayoutControlDefault m_pageLayoutControl;
        private ExportMapProj.DesignPageLayout m_designPageLayout = null; //制图输出
        private IElement m_element = null;//当前在AxPagelayOutControl中选择的元素
        private IElement m_origenalElement = null; //在选中当前元素时的上一选中元素
        private IResizeEnvelopeFeedback m_resizeEnvelopeFeedback = null;
        private ISelectionTracker m_selectionTracker;//显示跟踪线
        private IToolbarMenu m_menuPageLayout = null;//Pagelayout右键菜单
        private double m_mapScale = 0.0;

        private string m_textTitle; //标题，制作者等信息
        private string m_textName;
        private string m_textProject;
        private string m_textDate;
        private string m_textElevation;
        private string m_textOtherInfo;
        public frmPrintByAnyRegion()
        {
            InitializeComponent();

           
        }
        public frmPrintByAnyRegion(IMap pMap,IGeometry pGeometry,string  txtTitle ,string  txtName ,string  txtProj ,string  txtTime ,string textElevation,string textOtherInfo)
            //bool  checkBLengend ,bool  checkBNorthA ,bool  checkBMapG ,bool  checkBScaleB ,bool  checkBText )
        {
            InitializeComponent();
            m_pageLayoutControl = this.axPageLayoutControl1.Object as IPageLayoutControlDefault;
            m_Map = pMap;
            m_Geometry = pGeometry;
            m_textTitle = txtTitle;//标题
            m_textName = txtName;//制作人姓名
            m_textProject = txtProj;//投影系统
            m_textDate = txtTime;//时间
            m_textElevation = textElevation;//高程系统
            m_textOtherInfo = textOtherInfo;

            #region  PageLayout的右键菜单
            m_menuPageLayout = new ToolbarMenuClass();
            m_menuPageLayout.AddItem(new ControlsPageZoomInToolClass(), 1, 0,
                false, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.AddItem(new ControlsPageZoomOutToolClass(), 2, 1,
                false, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.AddItem(new ControlsPagePanToolClass(), 3, 2, false,
                 esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.AddItem(new ControlsPageZoomWholePageCommandClass(), 4,
               3, false, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.AddItem(new ControlsPageZoomOutFixedCommandClass(), 5, 4,
                false, esriCommandStyles.esriCommandStyleIconOnly);
            //Add PageLayOUTControl navigation commands.
            m_menuPageLayout.AddItem(new ControlsPageZoomPageToLastExtentBackCommandClass(), 6, 5,
              false, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.AddItem(new ControlsPageZoomPageToLastExtentForwardCommandClass(), 7, 6,
               false, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuPageLayout.SetHook(m_pageLayoutControl);

            #endregion
            this.axPageLayoutControl1.LoadMxFile(System.Windows.Forms.Application.StartupPath + @"\pagelayoutTemplate\海域图.mxt", Type.Missing);
            SetMapFrame();

            //产生一个地图容器IMaps对象
            IMaps maps = new Maps();
            maps.Add(m_Map);
            m_pageLayoutControl.PageLayout.ReplaceMaps(maps);

            if (m_Map.MapUnits == esriUnits.esriUnknownUnits)
            {
                m_Map.MapUnits = esriUnits.esriMeters;
                m_Map.DistanceUnits = esriUnits.esriMeters;
            }
            m_mapScale = m_Map.MapScale;
            
            axPageLayoutControl1.ActiveView.Refresh();
            axPageLayoutControl1.ActiveView.ShowScrollBars = true;
            InitializePrintPreviewDialog();
            printDialog1 = new PrintDialog();
            InitializePageSetupDialog();

          
        }
   
        private void SetMapFrame()//设置地图基本信息，如比例尺，图例等：
        {
           
            IGraphicsContainer pGraphicsContainer = (IGraphicsContainer)this.axPageLayoutControl1.ActiveView;
            pGraphicsContainer.Reset();
           IElementProperties pElementProperties = (IElementProperties)pGraphicsContainer.Next();
            while (pElementProperties != null)
            {
                if (pElementProperties is ITextElement)  //设置文本
                {
                    ITextElement ptextElement = new TextElementClass();
                    ptextElement = (ITextElement)pElementProperties;
                    if (ptextElement.Text.Trim() == "双击添加标题" ||ptextElement .Symbol.Size >25)
                    {
                        ptextElement.Text = m_textTitle;
                    }
                    if (ptextElement.Text.Trim().Contains("坐标系")) 
                    {
                        ptextElement.Text = "坐标系：" + m_textProject + "\r" + "高程系：" + m_textElevation;
                    }

                    if (ptextElement.Text.Trim().Contains("制作者"))
                    {
                        ptextElement.Text = "制图人："+m_textName+"\r"+"制作时间："+m_textDate;
                    }


                }
               if (pElementProperties.Type == "Map Surround Frame")
               {
                
                    IMapSurroundFrame pMapSurrounFrame = pElementProperties as IMapSurroundFrame;
                    IElement pElement = (IElement)pElementProperties;
                     
                    if (pMapSurrounFrame.MapSurround.Name.Trim() == "Legend")
                    {
                        ILegend pLegend = pMapSurrounFrame.MapSurround as ILegend;             
                            pLegend.Map = m_Map;
                          //  IMapSurround pMapSurround = pMapSurrounFrame.MapSurround;
                          //pElement = pMapSurrounFrame.MapSurround as IElement;
                          //  pMapSurround.Map = m_Map;
                          //  m_Map.AddMapSurround(pMapSurround);
                       
                    }
                     if (pMapSurrounFrame.MapSurround.Name == "Alternating Scale Bar" ||pMapSurrounFrame.MapSurround.Name == "Stepped Scale Line"||pMapSurrounFrame.MapSurround .Name =="Scale Line"||
                         pMapSurrounFrame .MapSurround .Name =="Hollow Scale Bar"||pMapSurrounFrame .MapSurround .Name =="Single Division Scale Bar"||
                         pMapSurrounFrame .MapSurround.Name =="Double Alternating Scale Bar")
                     {
                        // m_Map.MapScale = 5000;
                             IScaleBar pScaleBar = pMapSurrounFrame.MapSurround as IScaleBar;
                              pScaleBar.Map = m_Map;
                       

                              pScaleBar.Units = m_Map.MapUnits;
                              pScaleBar.UseMapSettings();
         
                              pElement = pMapSurrounFrame.MapSurround as IElement;
                    
                    }
                   if (pMapSurrounFrame.MapSurround.Name == "Scale Text")
                    {
                        IScaleText pScaleText = pMapSurrounFrame.MapSurround as IScaleText;
                            pScaleText.Map = m_Map;
                    }
                    if (pMapSurrounFrame.MapSurround.Name == "North Arrow")
                    {
                        INorthArrow pNorthArrow = pMapSurrounFrame.MapSurround as INorthArrow;

                            pNorthArrow.Map = m_Map;
                    }
                }
                if (pElementProperties.Type == "Data Frame")
                {
                    IFrameElement pFrameElement = pElementProperties as IFrameElement;
                    IMapFrame pMapframe = pFrameElement as IMapFrame;
                   // pMapframe.ExtentType = esriExtentTypeEnum.esriExtentBounds; 
                    IMapGrids pmapGrids = pMapframe as IMapGrids;
                    IMapGrid pMapGrid = null;
                    for(int i =0;i<pmapGrids.MapGridCount ;i++)
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
            if (axPageLayoutControl1.Page.Orientation == 1&&axPageLayoutControl1 .Printer .Paper .Orientation ==2)
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
            //this code will be called when the PrintPreviewDialog.Show method is called 打印出图
            //set the PageToPrinterMapping property of the Page. This specifies how the page 
            //is mapped onto the printer page. By default the page will be tiled 
            //get the selected mapping option
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

        private void getElement(double X,double Y)
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
                    if (pMapSurroundName == "Legend" || pMapSurroundName == "Alternating Scale Bar" || pMapSurroundName == "Hollow Scale Bar"||
                        pMapSurroundName == "Double Alternating Scale Bar" || pMapSurroundName == "Scale Line" || pMapSurroundName == "Single Division Scale Bar" ||
                        pMapSurroundName == "Stepped Scale Line" || pMapSurroundName == "Scale Text" || pMapSurroundName == "North Arrow")
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

        private void frmPrintByAnyRegion_Load(object sender, EventArgs e)
        {
            if (m_mapScale == 0.0) 
            {
                MessageBox.Show("地图无比例尺信息！", "提示");
            }
        }

        private void axPageLayoutControl1_OnMouseMove(object sender, IPageLayoutControlEvents_OnMouseMoveEvent e)
        {
            if (m_pageLayoutControl.MousePointer == esriControlsMousePointer.esriPointerSize && this.axPageLayoutControl1.CurrentTool == null) 
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
                    if (pMapSurrounFrame.MapSurround.Name == "Legend")
                    {
                        frmLegendWizard frmLegedWizard = new frmLegendWizard(m_Map, m_pageLayoutControl, pMapSurrounFrame);
                        frmLegedWizard.ShowDialog(); //修改图例属性

                    }
                    else if (pMapSurrounFrame.MapSurround.Name == "Alternating Scale Bar" || pMapSurrounFrame.MapSurround.Name == "Hollow Scale Bar" ||
                        pMapSurrounFrame.MapSurround.Name == "Double lternating Scale Bar" || pMapSurrounFrame.MapSurround.Name == "Scale Line"||
                       pMapSurrounFrame.MapSurround.Name == "Single Division Scale Bar" || pMapSurrounFrame.MapSurround.Name == "Stepped Scale Line")
                    {
                        ESRI.ArcGIS.Controls.esriSymbologyStyleClass styleClass = esriSymbologyStyleClass.esriStyleClassScaleBars;
                        frmLegendItemSymbol frmScaleBar = new frmLegendItemSymbol();
                        IStyleGalleryItem pstyGallertItem = frmScaleBar.GetItem(styleClass);//获取样式
                        IScaleBar pscaleBar = pMapSurrounFrame.MapSurround as IScaleBar;
                        esriUnits punits = pscaleBar.Units;
                        if (pstyGallertItem != null)

                            pscaleBar = (IScaleBar)pstyGallertItem.Item;
                        else
                            return;
                        pscaleBar.Map = m_pageLayoutControl.ActiveView.FocusMap; //重新设置比例尺依附的地图及单位
                        pscaleBar.Units = punits;
                        pscaleBar.UseMapSettings();
                        pscaleBar.Refresh();
                        pMapSurrounFrame.MapSurround = pscaleBar as IMapSurround;//旧的比例尺样式转换为新的比例尺样式
                        m_pageLayoutControl.Refresh(esriViewDrawPhase.esriViewGraphics, null, null);//刷新

                    }
                    else if (pMapSurrounFrame.MapSurround.Name == "Scale Text")
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
                    else if (pMapSurrounFrame.MapSurround.Name.Trim() == "North Arrow")
                    {
                        INorthArrow northArrow = pMapSurrounFrame.MapSurround as INorthArrow;
                        frmNorthArrowSymbol frmNorthArrow = new frmNorthArrowSymbol();
                        IStyleGalleryItem pstyGallertItem = frmNorthArrow.GetItem(esriSymbologyStyleClass.esriStyleClassNorthArrows); ;
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

        private void axPageLayoutControl1_OnAfterDraw(object sender, IPageLayoutControlEvents_OnAfterDrawEvent e)
        {
            this.uiComboBox1.Text = "1:"+this.axPageLayoutControl1.ActiveView.FocusMap.MapScale.ToString("#######.##");
            if (e.viewDrawPhase.Equals(32)) //设置元素跟踪显示
            {
                if (m_element == null && m_origenalElement != null)//当没有选中元素时，将原来选择的元素跟踪包络线隐藏
                {
                    m_selectionTracker = m_origenalElement.SelectionTracker;
                    m_selectionTracker.Deactivate();//隐藏包络线

                    return;
                }
                else if (m_element != null && m_origenalElement != null)
                {
                    m_selectionTracker = m_origenalElement.SelectionTracker;
                    m_selectionTracker.Deactivate();
                    m_selectionTracker = m_element.SelectionTracker;
                    m_selectionTracker.Draw(e.display as IDisplay, 0, esriTrackerStyle.esriTrackerDominant);//显示元素跟踪包络线
                }

                else if (m_element == null && m_origenalElement == null)
                {
                    return;
                }
                else if (m_element != null && m_origenalElement == null)
                {
                    m_selectionTracker = m_element.SelectionTracker;
                    m_selectionTracker.Draw(e.display as IDisplay, 0, esriTrackerStyle.esriTrackerDominant);//显示元素跟踪包络线
                }
            }
        }

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

        private void uiCommandBar2_CommandClick(object sender, Janus.Windows.UI.CommandBars.CommandEventArgs e)
        {
            if (e.Command.Key == "cmdPageSet")
            {
              /*  if (this.axPageLayoutControl1.PageLayout.Page.Orientation == 2)
                    pageSetupDialog1.PageSettings.Landscape = true ;
                else pageSetupDialog1.PageSettings.Landscape = false ; */
                #region//页面设置
                //Show the page setup dialog storing the result.
                DialogResult result = pageSetupDialog1.ShowDialog();

                //set the printer settings of the preview document to the selected printer settings
                document.PrinterSettings = pageSetupDialog1.PrinterSettings;

                //set the page settings of the preview document to the selected page settings
                document.DefaultPageSettings = pageSetupDialog1.PageSettings;

                //due to a bug in PageSetupDialog the PaperSize has to be set explicitly by iterating through the
                //available PaperSizes in the PageSetupDialog finding the selected PaperSize
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
                ///initialize the current printer from the printer settings selected
                ///in the page setup dialog

                IPaper paper;
                paper = new PaperClass(); //create a paper object

                IPrinter printer;
                printer = new EmfPrinterClass(); //create a printer object
                //in this case an EMF printer, alternatively a PS printer could be used

                //initialize the paper with the DEVMODE and DEVNAMES structures from the windows GDI
                //these structures specify information about the initialization and environment of a printer as well as
                //driver, device, and output port names for a printer
                paper.Attach(pageSetupDialog1.PrinterSettings.GetHdevmode(pageSetupDialog1.PageSettings).ToInt32(), pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());

                //pass the paper to the emf printer
                printer.Paper = paper;

                //set the page layout control's printer to the currently selected printer
                axPageLayoutControl1.Printer = printer;
                #endregion


            }
            if (e.Command.Key == "cmdPrintView")
            {
              

                #region//打印预览
                //initialize the currently printed page number
                m_CurrentPrintPage = 0;

                //check if a document is loaded into PageLayout  control
                if (axPageLayoutControl1.ActiveView.FocusMap == null) return;
                //set the name of the print preview document to the name of the mxd doc
                document.DocumentName = axPageLayoutControl1.DocumentFilename;
               
                //set the PrintPreviewDialog.Document property to the PrintDocument object selected by the user
                printPreviewDialog1.Document = document;
           //   printPreviewDialog1 .pa
                printPreviewDialog1.Document.DefaultPageSettings = pageSetupDialog1.PageSettings;

                //show the dialog - this triggers the document's PrintPage event
                printPreviewDialog1.ShowDialog();
                #endregion
            }
            if (e.Command.Key == "cmdPrint")
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
            if (e.Command.Key == "cmdOpenFile")
            {

                m_Command = new ControlsOpenDocCommandClass();
                m_Command.OnCreate(axPageLayoutControl1.Object);
                m_Command.OnClick();


            }
            if (e.Command.Key == "cmdSaveFile")
            {
                m_Command = new ControlsSaveAsDocCommandClass();
                m_Command.OnCreate(axPageLayoutControl1.Object);
                m_Command.OnClick();
            }
            if (e.Command.Key == "cmdSelectElement")
            {
                m_Command = new ControlsSelectToolClass();
                this.axPageLayoutControl1.CurrentTool = (ITool)m_Command;
                m_Command.OnCreate(axPageLayoutControl1.Object);
                m_Command.OnClick();
            }
            if (e.Command.Key == "CmdExportMapAsPicture")
            {
                ICommand pExportMapAsPicture = new ExportMapProj.CmdExoprtMapAsPicture();
                pExportMapAsPicture.OnCreate(m_pageLayoutControl.Object);
                pExportMapAsPicture.OnClick();

            }
            if (e.Command.Key == "cmdChoseTemplete")
            {
                ChoseTemple chosetem = new ChoseTemple();
                chosetem.ShowDialog();
                if (chosetem.m_templateName != "")
                {
                    this.axPageLayoutControl1.ActiveView.Clear();
                    this.axPageLayoutControl1.LoadMxFile(System.Windows.Forms.Application.StartupPath + @"\pagelayoutTemplate\" + chosetem.m_templateName, Type.Missing);
                    SetMapFrame();
                    IMaps maps = new Maps();
                    maps.Add(m_Map);
                    m_pageLayoutControl.PageLayout.ReplaceMaps(maps);
                    axPageLayoutControl1.ActiveView.Refresh();

                }    
            }
            if (e.Command.Key == "cmdScaleSetandPrint")//比例设置
            {
                frmSetMapScale frmSMS = new frmSetMapScale();
                 double pnewMapScale=frmSMS.setMapScale(m_mapScale,this .axPageLayoutControl1 .ActiveView .FocusMap.MapScale );
                 this.axPageLayoutControl1.ActiveView.FocusMap.MapScale = pnewMapScale;
                 this.axPageLayoutControl1.ActiveView.Refresh();
            }
        }


        private void frmPrintByAnyRegion_FormClosing(object sender, FormClosingEventArgs e)
        {
            //frmTemplatesPro.Dispose ();
        }

        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           // try
 
                string pMapscale = this.uiComboBox1.SelectedItem.Text.ToString();
                string pS = pMapscale.Substring(2, pMapscale.Length-2);
                this.axPageLayoutControl1.ActiveView.FocusMap.MapScale = double.Parse(pS);
                m_Map = this.axPageLayoutControl1.ActiveView.FocusMap;
                SetMapFrame();
                this.axPageLayoutControl1.ActiveView.Refresh();
        }

        private void uiCommandManager1_CommandClick(object sender, Janus.Windows.UI.CommandBars.CommandEventArgs e)
        {

        }

    

    }
}