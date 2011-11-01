using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;


using System.Windows.Forms;

namespace CoastalGIS.MapEditing
{
    /// <summary>
    /// Summary description for SelectPoint.
    /// </summary>
    [Guid("28be9ca3-1ea8-4b5c-915a-e215e77dfe0b")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("CoastalGIS.MapEditing.SelectPoint")]
    public sealed class SelectPoint : BaseTool
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
            ControlsCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        IMap pMap;
        IActiveView pActiveView;
        IGraphicsContainer pGraphicsContainer;
        INewLineFeedback pLineFeedback;
        frmAdjustPoint frmAdjPoint;
        ListViewItem newItem;
        IPoint toPoint;
        int pointCount = 1;
        private IHookHelper m_hookHelper;
        private object obj = Type.Missing;
        private IGeoReference m_geoRef;
        private string m_fileName;
        //int clickCount = 0;
        public SelectPoint(IGeoReference geoRef,string fileName)
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text 
            base.m_caption = "";  //localizable text 
            base.m_message = "";  //localizable text
            base.m_toolTip = "";  //localizable text
            base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
            m_geoRef = geoRef;
            m_fileName = fileName;
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
            frmAdjPoint = new frmAdjustPoint(m_geoRef, m_hookHelper.ActiveView, m_hookHelper.FocusMap, m_fileName);
            frmAdjPoint.Show();
            // TODO:  Add SelectPoint.OnCreate implementation
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add SelectPoint.OnClick implementation
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add SelectPoint.OnMouseDown implementation
            
            pActiveView = m_hookHelper.ActiveView;
            pMap = m_hookHelper.FocusMap;
            pGraphicsContainer = pMap as IGraphicsContainer;
            IPoint pPt = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X,
            Y);

            if (newItem==null) 
            {
                newItem = new ListViewItem(pointCount.ToString());
                newItem.SubItems.Add(pPt.X.ToString("f3"));
                newItem.SubItems.Add(pPt.Y.ToString("f3"));
                frmAdjPoint.FromPoint.AddPoint(pPt, ref obj, ref obj);
            }

            /*if (clickCount == 2&&newItem.SubItems.Count<=4)
            {
                newItem.SubItems.Add(pPt.X.ToString("f3"));
                newItem.SubItems.Add(pPt.Y.ToString("f3"));
                frmAdjPoint.ToPoint.AddPoint(pPt, ref obj, ref obj);
                clickCount++;
            }*/

            AddPointElement((IGeometry)pPt, pGraphicsContainer);

            if (pLineFeedback == null)
            {
                pLineFeedback = new NewLineFeedbackClass();
                pLineFeedback.Display = pActiveView.ScreenDisplay;
                pLineFeedback.Start(pPt);
            }
            else
            {
                pLineFeedback.AddPoint(pPt);
            }
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add SelectPoint.OnMouseMove implementation
            IPoint pPt = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            if (pLineFeedback != null) 
            {
                pLineFeedback.MoveTo(pPt);
                toPoint = pPt;
            }


        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add SelectPoint.OnMouseUp implementation
        }

        public override void OnDblClick()
        {
            newItem.SubItems.Add(toPoint.X.ToString("f3"));
            newItem.SubItems.Add(toPoint.Y.ToString("f3"));
            frmAdjPoint.ToPoint.AddPoint(toPoint, ref obj, ref obj);

            IGeometry pGeom;
            pGeom = pLineFeedback.Stop();
            pLineFeedback = null;
            AddLineElement(pGeom, pGraphicsContainer);
            frmAdjPoint.ListPoint.Items.Add(newItem);
            pointCount++;
            //clickCount = 0;
            newItem = null;
        }
        #endregion

        private void AddLineElement(IGeometry pGeom, IGraphicsContainer pGraphicsContainer)
        {
            ISimpleLineSymbol pLineSym;
            pLineSym = new SimpleLineSymbolClass();
            pLineSym.Color = getRGB(255, 0, 0);
            pLineSym.Width = 1;
            pLineSym.Style = esriSimpleLineStyle.esriSLSSolid;
            ILineElement plineEle;
            plineEle = new LineElementClass();
            plineEle.Symbol = pLineSym;
            IElement pEles;
            pEles = plineEle as IElement;
            pEles.Geometry = pGeom as IPolyline;
            pGraphicsContainer.AddElement(pEles, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void AddPointElement(IGeometry pGeom, IGraphicsContainer pGraphicsContainer)
        {
            ISimpleMarkerSymbol pSimpleSym = new SimpleMarkerSymbolClass();
            pSimpleSym.Style = esriSimpleMarkerStyle.esriSMSCross;
            pSimpleSym.Size = 10;
            pSimpleSym.Color = getRGB(255, 0, 0); ;

            IMarkerElement pMarkerElem = null;
            IElement pElem;
            pElem = new MarkerElementClass();
            pElem.Geometry = pGeom as IPoint;
            pMarkerElem = pElem as IMarkerElement;
            pMarkerElem.Symbol = pSimpleSym;
            pGraphicsContainer.AddElement(pElem, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
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
    }
}
