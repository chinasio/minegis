using System;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.SystemUI;
//using MapControl;
namespace CoastalGIS.CallMap
{

    [ClassInterface(ClassInterfaceType.None)]
    [Guid("D38DA441-A59F-4CE5-B68A-C3F532C4DE79")]

    //多边形调图

    public sealed class OpenMapByPolygon : BaseTool
    {
        IPolygon mGeomln;
        #region "Component Category Registration"
        [ComRegisterFunction()]
        static void Reg(string regKey)
        {
            ControlsCommands.Register(regKey);
        }

        [ComUnregisterFunction()]
        static void Unreg(string regKey)
        {
            ControlsCommands.Unregister(regKey);
        }
        #endregion
        static IPolygon pFirstGeom;
        private IHookHelper m_HookHelper = new HookHelperClass();

        System.Windows.Forms.Cursor m_Cursor;
        IActiveView m_pAV;
        IScreenDisplay m_pScrD;
        INewPolygonFeedback m_pNewPolygonFeedback;
        bool m_InUse = true;

        public OpenMapByPolygon()
        {

            base.m_deactivate = true;

        }
        //Destructor
        ~OpenMapByPolygon()
        {
            m_HookHelper = null;
            m_Cursor = null;
            m_pNewPolygonFeedback = null;
            mGeomln = null;
            pFirstGeom = null;

        }
        public void Relealse()
        {
            m_HookHelper = null;
            m_Cursor = null;
            m_pNewPolygonFeedback = null;
            mGeomln = null;
            pFirstGeom = null;

        }

        public override void OnCreate(object hook)
        {
            m_HookHelper.Hook = hook;
        }

       
        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            //m_Cursor = new System.Windows.Forms.Cursor(GetType().Assembly.GetManifestResourceStream(GetType(), "AopenMap.cur"));
            if (Button == 1)
            {
                if (m_InUse == true)
                {
                    m_pAV = m_HookHelper.ActiveView;
                    m_pScrD = m_pAV.ScreenDisplay;
                    IMap pMap = m_HookHelper.FocusMap;
                    IPoint pPnt;
                    pPnt = (IPoint)m_pScrD.DisplayTransformation.ToMapPoint(X, Y);
                    if (m_pNewPolygonFeedback == null)
                    {
                        m_pNewPolygonFeedback = new NewPolygonFeedbackClass();
                        ISimpleLineSymbol pSLnSym;
                        IRgbColor pRGB = new RgbColorClass();
                        pSLnSym = (ISimpleLineSymbol)m_pNewPolygonFeedback.Symbol;
                        pRGB.Red = 140;
                        pRGB.Green = 140;
                        pRGB.Blue = 255;
                        pSLnSym.Color = pRGB;
                        pSLnSym.Style = esriSimpleLineStyle.esriSLSSolid;
                        pSLnSym.Width = 2;
                        m_pNewPolygonFeedback.Display = m_pScrD;
                        m_pNewPolygonFeedback.Start(pPnt);

                    }
                    else
                    {
                        m_pNewPolygonFeedback.AddPoint(pPnt);
                    }

                }


            }
            else if (Button == 2)
            {

                IPolygon pGeomLn;
                pGeomLn = m_pNewPolygonFeedback.Stop();
                m_pNewPolygonFeedback = null;
                IMap pMap = m_HookHelper.FocusMap;
                ISpatialReference spatialReference = pMap.SpatialReference;
                //IBorder pBorder = new SymbolBorderClass();
                //*****************************************88888888
                if (pFirstGeom == null)
                {
                    pFirstGeom = pGeomLn;

                }
                if (mGeomln != null)
                {
                    pFirstGeom = mGeomln;
                }
                if (pFirstGeom != pGeomLn)
                {
                    IPolygon mFirstGeom;
                    mFirstGeom = pFirstGeom;
                    IRelationalOperator pROperator = (IRelationalOperator)mFirstGeom;
                    if (pROperator.Disjoint((IGeometry)pGeomLn) == false)
                    {

                        //先定义一个IGeometrycollection的多边形,将每个画出来的IgeometryCollection添加进去 
                        //先添一个构成一个IPolygon,转化为ITopo_ ,再同样构成另一个,进行Union
                        IGeometryCollection pcGeometry = new PolygonClass();
                        object o = System.Type.Missing;
                        IPolygon cFirstGeom = new PolygonClass();
                        cFirstGeom = pFirstGeom;
                        ITopologicalOperator tempTopo = (ITopologicalOperator)cFirstGeom;
                        tempTopo.Simplify();

                        ITopologicalOperator pTopo = (ITopologicalOperator)pGeomLn;
                        pTopo.Simplify();
                        IGeometry kGeom;
                        kGeom = pTopo.Union((IGeometry)cFirstGeom);
                        mGeomln = (IPolygon)kGeom;
                        mGeomln.SpatialReference = spatialReference; 
                        m_pAV.FocusMap.ClipGeometry = mGeomln;
                        //IBorder pBorder = new SymbolBorderClass();
                        //m_HookHelper.FocusMap.ClipBorder = pBorder;
                        m_pAV.Extent = mGeomln.Envelope;
                        m_pAV.Refresh();
                        m_Cursor = base.m_cursor;
                        //layVisbleExceptMap();

                    }

                }

                else
                {

                    //*************************************************8
                    //mGeomln = pGeomLn;
                    pGeomLn.SpatialReference = spatialReference;
                    LayerControl.LyrPolygon = pGeomLn;
                    m_HookHelper.FocusMap.ClipGeometry = pGeomLn;
                    IBorder pBorder = new SymbolBorderClass();
                    m_HookHelper.FocusMap.ClipBorder = pBorder;
                    m_pAV.Extent = pGeomLn.Envelope;
                    m_pAV.Refresh();
                    m_Cursor = base.m_cursor;
                    pGeomLn = null;
                }

            }



        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {

            if (m_pNewPolygonFeedback != null)
            {
                IPoint pPnt;
                pPnt = (IPoint)m_pScrD.DisplayTransformation.ToMapPoint(X, Y);
                m_pNewPolygonFeedback.MoveTo(pPnt);

            }

        }


        public override int Cursor
        {
            get
            {

                return m_Cursor.Handle.ToInt32();

            }
        }


    }
}