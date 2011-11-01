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
    [Guid("771dafd4-af53-42ae-b975-557757cc1ee8")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ExportMapProj.RemoveElement")]
    public sealed class DeleteElement : BaseCommand
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
       // private IMapControlDefault m_mapControl;
        private IPageLayoutControlDefault m_pageLayoutControl;
        private IElement m_element;
        public DeleteElement(IPageLayoutControlDefault  pageLayoutControl,IElement element)
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "删除选中元素"; //localizable text
            base.m_caption = "删除选中元素";  //localizable text 
            base.m_message = "This should work in ArcMap/MapControl/PageLayoutControl";  //localizable text
            base.m_toolTip = "删除选中元素";  //localizable text
            base.m_name = "删除选中元素";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")
           // m_mapControl = mapControl;
            m_pageLayoutControl = pageLayoutControl;
            m_element = element;
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
            // TODO: Add RemoveElement.OnClick implementation
            if (m_element != null)
            {

                if (m_element is IMapFrame)
                {
                  
                   // m_tocControl.ActiveView.Refresh();
                    m_pageLayoutControl.GraphicsContainer.DeleteAllElements();
                   // m_mapControl.Map.ClearLayers();
                    m_pageLayoutControl.ActiveView.FocusMap.ClearLayers();
                    m_pageLayoutControl.ActiveView.Refresh();
             
                    m_pageLayoutControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
                }
                else
                {
                    m_pageLayoutControl.GraphicsContainer.DeleteElement(m_element);
                    m_element = null;
                    DesignPageLayout m_desiagnPageLayout = new DesignPageLayout(m_pageLayoutControl);
                    //.m_elementToMove = null;
                    m_desiagnPageLayout.m_elementToMove = null;
                    m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                    m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                    m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewNone, null, null);
                    m_pageLayoutControl.ActiveView.Refresh();
                    m_pageLayoutControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
                }
            }
        }

        #endregion
    }
}
