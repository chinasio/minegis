using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Microsoft.VisualBasic;


using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.PublisherControls;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.ArcMapUI;
//using CoastalGIS.MainGIS;
using ESRI.ArcGIS.OutputUI;
using ESRI.ArcGIS.Output ;


namespace CoastalGIS.ExportMapProj
{
    [Guid("60fe9f49-7b0d-45ed-a203-caf920b22259")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ExportMapProj.DesignPageLayout")]
    public class DesignPageLayout
    {
       //  private IPageLayoutControlDefault  m_PageLayoutControl = null;
        public  IPageLayoutControlDefault m_pagelayoutControl;
        public IElement m_elementToMove;
        private IStyleSelector m_styleSelector;
        private  bool m_Ok;

       private IMapControlDefault m_mapControl;
        

        public DesignPageLayout(IMapControlDefault mapcontrol,IPageLayoutControlDefault pagelayoutControl)
        {
            m_pagelayoutControl = pagelayoutControl;
           m_mapControl = mapcontrol;
        }
        public DesignPageLayout(IPageLayoutControlDefault pagelayoutControl)
        {
            m_pagelayoutControl = pagelayoutControl;
        }
     

        public void AddLegend()
        {  
           
            #region//直接调用AE的生成图例Form;
            ///////////////////////////////////////////////////////////////////////
            ////Create a legend wizard and grab hold of the ILegendWizard interface
          /*  ILegendWizard m_legendWizard = new LegendWizardClass();
            //Set the legend wizard pageLayout property
            m_legendWizard.PageLayout = m_pagelayoutControl.PageLayout;
           // Give the LegendWizard a legendframe - must do this
            m_legendWizard.InitialLegendFrame = mapSurroundFrame;
            
            //显示legendWizard
            m_Ok = m_legendWizard.DoModal(m_pagelayoutControl.hWnd);
            if (m_Ok == true)
            {
                IEnvelope envelope = new EnvelopeClass();
                //Envelope for the legend
                envelope.PutCoords(10, 10,30, 30);
                //Set the geometry of the MapSurroundFrame 
                IElement element = (IElement)mapSurroundFrame;
                element.Geometry = envelope;
                //Add the legend to the PageLayout
                m_pagelayoutControl.AddElement(element, Type.Missing, Type.Missing, "Legend", 0);
            }
             */

            ////////////////////////////////////////////////////
            #endregion
           ExportMapProj.frmLegendWizard frmLegend = new ExportMapProj.frmLegendWizard( m_mapControl ,m_pagelayoutControl );
              frmLegend.ShowDialog (); 
           m_pagelayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        
        }
 

        public void AddText()
        {
            string m_text;
            m_text = "Text";
            IElement m_elment = new TextElement();
            ITextElement m_textElement = m_elment as ITextElement;
           // ITextSymbol m_textSymbol =new TextSymbolClass ();
           // m_textSymbol.Text= m_text;
           // m_textSymbol.Size = 48;
             m_textElement.Text = m_text;
            frmTextSymbol form1 = new frmTextSymbol( ref m_textElement);
             form1.ShowDialog ();
             // m_textElement  = form1.getText();
            if (m_textElement .Text =="") 
               return;
            IEnvelope m_envelope=new EnvelopeClass ();
            m_envelope.PutCoords(13, 13, 17, 18);
            m_elment.Geometry = m_envelope;
            #region//直接调用AE添加文本对话框
            // ITextSymbolEditor m_textSymbolEditor=new TextSymbolEditorClass ();
           // m_Ok = m_textSymbolEditor.EditTextSymbol(ref  m_textSymbol, m_pagelayoutControl.hWnd);

           // if (m_Ok==true  )
           // {
               // m_textElement.Symbol = m_textSymbol;
            // m_textElement.Text = m_text;
            #endregion
            m_textElement.ScaleText = true;
                m_elment = (IElement)m_textElement;
                m_elment.Locked = false;
                m_pagelayoutControl.AddElement(m_elment, Type.Missing, Type.Missing, "Text", 5);
                m_pagelayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        

        }
        public void AddNorthArrow()
        {   //添加指北针
            IGraphicsContainer graphicsContainer = m_pagelayoutControl.GraphicsContainer;

            //Get the MapFrame
            IMapFrame mapFrame = (IMapFrame)graphicsContainer.FindFrame(m_pagelayoutControl.ActiveView.FocusMap);
            if (mapFrame == null) return;
            //Create a legend
            UID uID = new UIDClass();
            uID.Value = "esriCarto.MarkerNorthArrow";

            //Create a MapSurroundFrame from the MapFrame
            IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uID, null);
            if (mapSurroundFrame == null) return;
            if (mapSurroundFrame.MapSurround == null) return;
          
            IEnvelope m_envelop = new EnvelopeClass();
            m_envelop.PutCoords(15, 15, 20, 20);
            INorthArrow northArrow = null;
            frmNorthArrowSymbol frmNorthArrow = new frmNorthArrowSymbol();
            IStyleGalleryItem pstyGallertItem = frmNorthArrow.GetItem(ESRI.ArcGIS.Controls.esriSymbologyStyleClass.esriStyleClassNorthArrows); ;
            if (pstyGallertItem != null)
            {
                northArrow = (INorthArrow)pstyGallertItem.Item;
                northArrow.Map = this.m_pagelayoutControl.ActiveView.FocusMap;
                northArrow.CalibrationAngle = frmNorthArrow.Angle();

            }
            else
            {
                return;
            }

            IMapSurround m_mapSurround;
            m_mapSurround = (IMapSurround)northArrow;
            mapSurroundFrame.MapSurround = m_mapSurround;
            IElement m_element = (IElement)mapSurroundFrame;
            m_element.Geometry = m_envelop;
            m_pagelayoutControl.AddElement(m_element, Type.Missing, Type.Missing, "NorthArrow", 2);
            m_pagelayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        public void AddScaleBar()
        {
            IGraphicsContainer graphicsContainer = m_pagelayoutControl.GraphicsContainer;
            //Get the MapFrame
            IMapFrame mapFrame = (IMapFrame)graphicsContainer.FindFrame(m_pagelayoutControl.ActiveView.FocusMap);
            if (mapFrame == null) return;

            //Create a legend
            UID uID = new UIDClass();
            uID.Value = "esriCarto.ScaleLine";
            //Create a MapSurroundFrame from the MapFrame
            IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uID, null);

            if (mapSurroundFrame == null) return;
            if (mapSurroundFrame.MapSurround == null) return;

            // m_styleSelector = new ScaleBarSelectorClass();
            IEnvelope m_envelop = new EnvelopeClass();
            m_envelop.PutCoords(5, 5, 10, 10);

            ESRI.ArcGIS.Controls.esriSymbologyStyleClass styleClass = esriSymbologyStyleClass.esriStyleClassScaleBars;
            frmLegendItemSymbol frmScaleBar = new frmLegendItemSymbol();
            IStyleGalleryItem pstyGallertItem = frmScaleBar.GetItem(styleClass);
            if (pstyGallertItem == null)
            {
                return;
            }
            IScaleBar pScalebar = pstyGallertItem.Item as IScaleBar;
            pScalebar.Units = m_pagelayoutControl.ActiveView.FocusMap.MapUnits;
            pScalebar.Map = m_pagelayoutControl.ActiveView.FocusMap;
            pScalebar.UseMapSettings();

            IMapSurround m_mapSurround;
            m_mapSurround = (IMapSurround)pScalebar;
            mapSurroundFrame.MapSurround = m_mapSurround;
            IElement m_element = (IElement)mapSurroundFrame;
            m_element.Geometry = m_envelop;
            m_pagelayoutControl.AddElement(m_element, Type.Missing, Type.Missing, "ScaleLine", 3);
            m_pagelayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

        }

        public  void AddTextScale()
        {
            IGraphicsContainer graphicsContainer = m_pagelayoutControl.GraphicsContainer;
            //Get the MapFrame
            IMapFrame mapFrame = (IMapFrame)graphicsContainer.FindFrame(m_pagelayoutControl.ActiveView.FocusMap);
            if (mapFrame == null) return;

            //Create a legend
            UID uID = new UIDClass();
            uID.Value = "esriCarto.ScaleText";
            //Create a MapSurroundFrame from the MapFrame
            IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uID, null);

            if (mapSurroundFrame == null) return;
            if (mapSurroundFrame.MapSurround == null) return;
            IEnvelope m_envelop = new EnvelopeClass();
            m_envelop.PutCoords(15, 15, 20, 20);

            frmLegendItemSymbol frmTextScal = new frmLegendItemSymbol();
            IStyleGalleryItem pStyGalleryItem = frmTextScal.GetItem(esriSymbologyStyleClass.esriStyleClassScaleTexts);
            IScaleText pScaleText = null;
            if (pStyGalleryItem != null)
            {
                pScaleText = (IScaleText)pStyGalleryItem.Item;
                pScaleText.Map = this.m_pagelayoutControl.ActiveView.FocusMap;
                pScaleText.MapUnits = this.m_pagelayoutControl.ActiveView.FocusMap.MapUnits;
            }

            else
               return;
          IMapSurround m_mapSurround;
           m_mapSurround = (IMapSurround)pScaleText ;
           mapSurroundFrame.MapSurround = m_mapSurround;
          IElement m_element = (IElement)mapSurroundFrame;
           m_element.Geometry = m_envelop;
          m_pagelayoutControl.AddElement(m_element, Type.Missing, Type.Missing, "ScaleText", 4);
          m_pagelayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public void AddMapGrid()
        {

            m_styleSelector = new MapGridSelectorClass();//Create a style selector and grab holf of the IMapGridSelector interface

            m_Ok = m_styleSelector.DoModal(m_pagelayoutControl.hWnd);//Display the style selector to the user

            IGraphicsContainer graphicsContainer = m_pagelayoutControl.GraphicsContainer;
            //Get the MapFrame
            IMapFrame mapFrame = (IMapFrame)graphicsContainer.FindFrame(m_pagelayoutControl.ActiveView.FocusMap);
            if (mapFrame == null) return;

            if (m_Ok == true)
            {
                IMapGrid m_mapGrid;
                m_mapGrid =(IMapGrid ) m_styleSelector.GetStyle(0);//获得所选的MapGrid的类型；

                IMapGrids m_mapGrids = mapFrame as IMapGrids ;

                m_mapGrids.ClearMapGrids();//删除原有的地图格网

                if (m_mapGrid != null)
                {
                    m_mapGrids.AddMapGrid(m_mapGrid);//添加地图格网到地图上
                }
                m_pagelayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);//刷新
                
            
            }
        
        }

 
        public  void ExportFileDialog()
        {
            IActiveView pActiveView;
            IEnvelope pPixelBoundsEnv;
            int iOutputResolution;
            int iScreenResolution;
             int hDC;
            pActiveView = m_pagelayoutControl .ActiveView;
            //设置分辨率
            iScreenResolution = 96;
            iOutputResolution = 300;
            tagRECT exportRECT;
            exportRECT.left = 0;
            exportRECT.top = 0;
            exportRECT.right = pActiveView.ExportFrame.right * (iOutputResolution /
            iScreenResolution);
            exportRECT.bottom = pActiveView.ExportFrame.bottom * (iOutputResolution /
            iScreenResolution);
            pPixelBoundsEnv = new EnvelopeClass();
            pPixelBoundsEnv.PutCoords(exportRECT.left, exportRECT.top,
            exportRECT.right, exportRECT.bottom);//设置输出范围
            IExportFileDialog pExportDialog;
            pExportDialog = new ExportFileDialogClass();
            //使用输出对话框
            m_Ok = pExportDialog.DoModal(pPixelBoundsEnv, pActiveView.Extent,pActiveView.Extent, 300);
            if (!(m_Ok)) return;//如果按下“取消”键，则退出
            IExport pExport;
            pExport = pExportDialog.Export;//设置转换输出的属性
            pExport.Resolution = iOutputResolution;

            pExport.PixelBounds = pPixelBoundsEnv;
            hDC = pExport.StartExporting();//产生文件
            pActiveView.Output(hDC, (int)pExport.Resolution, ref exportRECT, null, null);
            pExport.FinishExporting();
            pExport.Cleanup();
        }

        public void fGetElements(double mapX, double mapY, ref  IElement pElement)
        {
            IGraphicsContainerSelect m_graphicsContainerSelect;
            IGraphicsContainer m_graphicsContainer = m_pagelayoutControl.ActiveView as IGraphicsContainer;
            int i, m_selectCount;
            IElement m_element, m_selectElement = null;
            IGeometry m_geometry;
            IMapFrame m_mapFrame;
            bool ifElementHited;

          
           
                IPoint m_point = new ESRI.ArcGIS.Geometry.Point();
                m_point.X = mapX;
                m_point.Y = mapY;

                m_mapFrame = m_graphicsContainer.FindFrame(m_pagelayoutControl.ActiveView.FocusMap) as IMapFrame;

                IFrameElement pFramElement = m_mapFrame as IFrameElement;

            ///////////////////////////////////////////////////////////////////////////////////////

                m_graphicsContainerSelect = m_pagelayoutControl.PageLayout as IGraphicsContainerSelect;
                m_selectCount = m_graphicsContainerSelect.ElementSelectionCount;//m_graphicsContainerSelect 中有的可供选择的元素
                if (m_selectCount == 0)
                {
                    m_elementToMove = null;
                    if (pFramElement != null)
                        pElement = pFramElement as IElement;

                }
                else
                {
                    for (i = 0; i < m_selectCount; i++)
                    {
                        m_element = m_graphicsContainerSelect.SelectedElement(i);
                        m_geometry = m_element.Geometry;
                        m_graphicsContainer.LocateElementsByEnvelope(m_geometry.Envelope);//定位第i个元素所在的位置
                        ifElementHited = m_element.HitTest(mapX, mapY, 0.1);//判断鼠标点击位置是否与元素位置相同
                        if (ifElementHited)
                        {

                            m_graphicsContainerSelect.SelectElement(m_element);//选中元素，一遍对其进行移动
                            m_selectElement = m_element;

                        }

                    }
                    if (m_selectElement == null)//当鼠标没有选择graphicsContainerSelect中的元素时
                    {
                          m_elementToMove = pFramElement as IElement ; 
                        pElement = pFramElement as IElement;//鼠标选择地图数据框
                    }
                    else
                    {
                        pElement = m_selectElement;
                        m_elementToMove = m_selectElement;//是则可以移动该元素并选择元素
                    }


                }

        }

        public void fMoveElement(double mapX, double mapY)//移动元素
        {
            IEnvelope pEnv;
            IGeometry m_geometry;
            IPoint pt;
            if (m_elementToMove != null)
            {
                m_geometry = m_elementToMove.Geometry;
                pEnv = m_geometry.Envelope;
                pt = new ESRI.ArcGIS.Geometry.Point();
                pt.X = mapX;
                pt.Y = mapY;
                pEnv.CenterAt(pt);
                m_elementToMove.Geometry = pEnv;
               m_pagelayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
              //  m_pagelayoutControl.ActiveView.Refresh();
            }

        }


    }
}
