using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.PublisherControls;
using ESRI.ArcGIS.Display;

namespace CoastalGIS.ExportMapProj
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// </summary>
    [Guid("ff12809d-2f94-463e-899a-6945b49b17f4")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ExportMapProj.ChangeFrameShadow")]
    public sealed class ChangeFrameShadow : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper = null;
        private IPageLayoutControlDefault m_pageLayoutControl;
        private IMapFrame m_MapFrame;
        private IElement m_element;
        public ChangeFrameShadow(IElement pElement, IPageLayoutControlDefault pageLayoutControl)
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "设置图框阴影"; //localizable text
            base.m_caption = "设置图框阴影";  //localizable text 
            base.m_message = "This should work in ArcMap/MapControl/PageLayoutControl";  //localizable text
            base.m_toolTip = "设置图框阴影";  //localizable text
            base.m_name = "设置图框阴影";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

            m_pageLayoutControl = pageLayoutControl;
            m_element = pElement;
            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (m_hookHelper.ActiveView == null)
                    m_hookHelper = null;
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                base.m_enabled = false;
            else
                base.m_enabled = true;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add ChangeFrameShadow.OnClick implementation
           /* IActiveView pActiveView;
            IGraphicsContainer pGraphicsContainer;
            IMapFrame pMapFrame;
            IMap pMap;
            pActiveView = m_pageLayoutControl.PageLayout as IActiveView;
            pMap = pActiveView.FocusMap;
            pGraphicsContainer = pActiveView as IGraphicsContainer;
            pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            IStyleSelector pStyleSelector;
            pStyleSelector = new ShadowSelectorClass();
            //􁰒􂼎􄗝􁢽􁇍􄈵􁸚􁇍􄈵􃒭􂫼􁠋
            bool m_bOK;
            m_bOK = pStyleSelector.DoModal(m_pageLayoutControl.hWnd);
            //􀽖􁵰􂫼􁠋􁣝cancel􁣝􄪂􀟭􄗔􀟎
            if (!m_bOK) return;
            IShadow pShadow;
            //􀒢􄗝􁢽􀰼􀐁􁕫􀠄􀏔􀏾IShadow􁇍􄈵
            pShadow = pStyleSelector.GetStyle(0) as IShadow;
            IFrameProperties pFrameProperties;
            pFrameProperties = pMapFrame as IFrameProperties;
            //􄆒􃕂*/
            IElementProperties pElementProties = m_element as IElementProperties;
            if (m_element is IMapFrame)
                m_MapFrame = m_element as IMapFrame;
            else if (pElementProties.Type == "Map Surround Frame")
            {
                IMapSurroundFrame pMapSurrounFrame = pElementProties as IMapSurroundFrame;
                if (pMapSurrounFrame.MapSurround.Name == "Legend")
                {
                    //ILegend pLegend = m_element as ILegend;
                    //为了改变边框，背景，阴影等样式
                    //IMapSurround pmapSurd = (IMapSurround)pLegend;
                    IMapSurroundFrame pmapSurdFrm = pMapSurrounFrame;
                    //  pmapSurdFrm.MapSurround = pmapSurd;
                    IMapFrame pMapFrm = new MapFrameClass();
                    pMapFrm = pmapSurdFrm.MapFrame;
                    m_MapFrame = pMapFrm;
                }
                else if (pMapSurrounFrame.MapSurround.Name == "Alternating Scale Bar")
                {
                    // IScaleBar pScaleBatr = m_element as IScaleBar;
                    // IMapSurround pmapSurd = (IMapSurround)pScaleBatr ;
                    IMapSurroundFrame pmapSurdFrm = pMapSurrounFrame;
                    //  pmapSurdFrm.MapSurround = pmapSurd;
                    IMapFrame pMapFrm = new MapFrameClass();
                    pMapFrm = pmapSurdFrm.MapFrame;
                    m_MapFrame = pMapFrm;
                }
                else if (pMapSurrounFrame.MapSurround.Name == "Scale Text")
                {
                    //IScaleText pScaleText = m_element as IScaleText;
                    // IMapSurround pmapSurd = (IMapSurround)pScaleText;
                    IMapSurroundFrame pmapSurdFrm = pMapSurrounFrame;
                    //pmapSurdFrm.MapSurround = pmapSurd;
                    IMapFrame pMapFrm = new MapFrameClass();
                    pMapFrm = pmapSurdFrm.MapFrame;
                    m_MapFrame = pMapFrm;
                }
                else if (pMapSurrounFrame.MapSurround.Name == "North Arrow")
                {
                    // INorthArrow pNorthArrow = m_element as INorthArrow;
                    //  IMapSurround pmapSurd = (IMapSurround)pNorthArrow ;
                    IMapSurroundFrame pmapSurdFrm = pMapSurrounFrame;
                    // pmapSurdFrm.MapSurround = pmapSurd;
                    IMapFrame pMapFrm = new MapFrameClass();
                    pMapFrm = pmapSurdFrm.MapFrame;
                    m_MapFrame = pMapFrm;
                }
            }
            IFrameProperties pFrameProperties;
            pFrameProperties =m_MapFrame as IFrameProperties;
            frmLegendItemSymbol frmBackGround = new frmLegendItemSymbol();
            ESRI.ArcGIS.Controls.esriSymbologyStyleClass styleClass = esriSymbologyStyleClass.esriStyleClassShadows ;
            IStyleGalleryItem pStyleGalleryItem = frmBackGround.GetItem(styleClass);
            if (pStyleGalleryItem != null)
                pFrameProperties .Shadow  = (IShadow )pStyleGalleryItem.Item;
            else
                return;
           // pFrameProperties.Shadow = pShadow;
            m_pageLayoutControl.Refresh (esriViewDrawPhase.esriViewBackground, null, null);
            m_pageLayoutControl.ActiveView.Refresh();
        }

        #endregion
    }
}
