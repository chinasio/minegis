using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using System.IO;
using ESRI.ArcGIS.DataSourcesGDB;

namespace CoastalGIS.MainGIS
{
    /// <summary>
    /// Summary description for AreaPrintMapClass.
    /// </summary>
    [Guid("06321a84-b7c4-4cf6-9cbe-9fd03f0bc1ee")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ExportMapProj.AreaPrintMapClass")]
    public sealed class AreaPrintMapClass : BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            // ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            // ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>

        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>


        #endregion
        #endregion

        private IHookHelper m_hookHelper = null;
        private IMap m_ClipMap = null;
        // System.Windows.Forms.Cursor m_Cursor;
        IActiveView m_ActiveView;
        IScreenDisplay m_ScreenDisplay;
        INewPolygonFeedback m_NewPolygonFeedback;
        bool m_InUse = true;
        IGeometry m_Geometry;

        public AreaPrintMapClass()
        {
            base.m_deactivate = true;
            //base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");

        }

        ~AreaPrintMapClass()
        {
            m_hookHelper = null;
            m_cursor = null;
            m_NewPolygonFeedback = null;
           
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (m_hookHelper.ActiveView == null)
                {
                    m_hookHelper = null;
                }
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                base.m_enabled = false;
            else
                base.m_enabled = true;

            m_ClipMap = new MapClass();

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add AreaPrintMapClass.OnClick implementation
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AreaPrintMapClass.OnMouseDown implementation

            if (Button == 1)
            {
                if (m_InUse == true)
                {
                    m_ActiveView = m_hookHelper.ActiveView;
                    m_ScreenDisplay = m_ActiveView.ScreenDisplay;
                    IMap pMap = m_ActiveView.FocusMap;
                    IPoint pPoint;
                    pPoint = (IPoint)m_ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
                    if (m_NewPolygonFeedback == null)
                    {
                        m_NewPolygonFeedback = new NewPolygonFeedbackClass();
                        ISimpleLineSymbol pSlnSym;
                        IRgbColor pRGB = new RgbColorClass();
                        pSlnSym = (ISimpleLineSymbol)m_NewPolygonFeedback.Symbol;
                        pRGB.Red = 225;
                        pRGB.Green = 0;
                        pRGB.Blue = 0;
                        pSlnSym.Color = pRGB;
                        pSlnSym.Style = esriSimpleLineStyle.esriSLSSolid;
                        pSlnSym.Width = 2;
                        m_NewPolygonFeedback.Display = m_ScreenDisplay;
                        m_NewPolygonFeedback.Start(pPoint);
                    }
                    else
                    {
                        m_NewPolygonFeedback.AddPoint(pPoint);
                    }
                }
            }
        }
  
        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AreaPrintMapClass.OnMouseMove implementation
            if (m_NewPolygonFeedback != null)
            {
                IPoint pPoint;
                pPoint = (IPoint)m_ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
                m_NewPolygonFeedback.MoveTo(pPoint);
            }

        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AreaPrintMapClass.OnMouseUp implementation
        }
        public override void OnDblClick()
        {
            IPolygon polygon = m_NewPolygonFeedback.Stop();
            CreateLayerM cl = new CreateLayerM((IMapControlDefault)m_hookHelper.Hook);
            IFeatureClass feac = cl.CreatePolygonLayer(polygon);
            //frmCacu frmCacul = new frmCacu((IMapControlDefault)m_hookHelper.Hook,feac);
            //frmCacul.ShowDialog();
        }
        #endregion



    }
}
