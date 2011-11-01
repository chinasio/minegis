using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.MapControl;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace CoastalGIS.MapEditing
{
    /// <summary>
    /// Summary description for ModifyShape.
    /// </summary>

    public sealed class ModifyShape : BaseTool
    {
        ESRI.ArcGIS.Controls.MapControl m_MapControl;
        ESRI.ArcGIS.Carto.ILayer m_CurrentLayer;
        private IDisplayFeedback m_Feedback;
        private IFeature m_EditFeature;

        public ModifyShape(ILayer pCurrentLayer)
        {
            //
            // TODO: Define values for the public properties
            //
            m_CurrentLayer = pCurrentLayer;
       
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            m_MapControl = hook as ESRI.ArcGIS.Controls.MapControl;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add ModifyShape.OnClick implementation
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add ModifyShape.OnMouseDown implementation

            m_MapControl.Map.ClearSelection();
            m_MapControl.ActiveView.Refresh();
            SelectMouseDown(X, Y);
            IEnumFeature pSelected = (IEnumFeature)m_MapControl.Map.FeatureSelection;
            IFeature pFeature = pSelected.Next();
            if (pFeature == null)
                return;
            IActiveView pActiveView = m_MapControl.ActiveView;
            IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);

            //设置捕捉容差
            double tol = ConvertPixelsToMapUnits(pActiveView, 4);
            IGeometry pGeom = pFeature.Shape;
            IObjectClass pObjectClass = pFeature.Class;
            m_EditFeature = pFeature;

            switch (pGeom.GeometryType)
            { 
                case esriGeometryType .esriGeometryPoint :
                    m_Feedback = new MovePointFeedbackClass();
                    m_Feedback.Display = pActiveView.ScreenDisplay;
                    IMovePointFeedback pPointMove = (IMovePointFeedback)m_Feedback;
                    pPointMove.Start((IPoint)pGeom, pPoint);
                    break;
                case esriGeometryType .esriGeometryPolyline :
                    m_Feedback = new MoveLineFeedbackClass ();
                    m_Feedback.Display = pActiveView.ScreenDisplay;
                    IMoveLineFeedback m_MoveLineFeedback = (IMoveLineFeedback)m_Feedback;
                    m_MoveLineFeedback.Start((IPolyline)pGeom, pPoint);
                    break;
                case esriGeometryType .esriGeometryPolygon :
                    m_Feedback = new MovePolygonFeedbackClass();
                    m_Feedback.Display = pActiveView.ScreenDisplay;
                    IMovePolygonFeedback m_MovePolygonFeedback = (IMovePolygonFeedback)m_Feedback;
                    m_MovePolygonFeedback.Start((IPolygon)pGeom, pPoint);
                    break;
            }


        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add ModifyShape.OnMouseMove implementation

            if (m_Feedback == null)
                return;
            IActiveView pActiveView = m_MapControl.ActiveView;
            IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            m_Feedback.MoveTo(pPoint);
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add ModifyShape.OnMouseUp implementation
            if (m_Feedback == null) 
                return;
            IActiveView pActiveView = m_MapControl.ActiveView;
            IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            if (m_Feedback is IMovePointFeedback)
            {
                IMovePointFeedback pPointMove = (IMovePointFeedback)m_Feedback;
                IGeometry pGeometry = pPointMove.Stop();
                UpdateFeature(m_EditFeature, pGeometry);
            }
            else if (m_Feedback is IMoveLineFeedback) //线 对象移动
            {
                IMoveLineFeedback pLineMove = (IMoveLineFeedback)m_Feedback;
                IGeometry pGeometry = pLineMove.Stop();
                UpdateFeature(m_EditFeature, pGeometry);

            }
            else if (m_Feedback is IMovePolygonFeedback)
            {
                IMovePolygonFeedback pPolygonMove = (IMovePolygonFeedback)m_Feedback;
                IGeometry pGeometry = pPolygonMove.Stop();
                UpdateFeature(m_EditFeature, pGeometry);

            }

            m_Feedback = null;
            pActiveView.Refresh();
        }
        public override int Cursor
        {
            get
            {
                // TODO:  添加 CreateShape.Cursor getter 实现
                return 0;
            }
        }
        public override bool OnContextMenu(int x, int y)
        {
            // TODO:  添加 CreateShape.OnContextMenu 实现
            return false;
        }

        public override bool Deactivate()
        {
            // TODO:  添加 CreateShape.Deactivate 实现
            return true;                                          //返回true，保证对象注销时不报错
        }

        public override void Refresh(int hdc)
        {
            // TODO:  添加 CreateShape.Refresh 实现

        }
        public override void OnDblClick()
        {
            // TODO:  添加 ModifyShape.OnDblClick 实现
        }

        public override string Message
        {
            get
            {
                // TODO:  添加 CreateShape.Message getter 实现
                return null;
            }
        }

        public override int Bitmap
        {
            get
            {
                // TODO:  添加 CreateShape.Bitmap getter 实现
                return 0;
            }
        }
        public override string Caption
        {
            get
            {// TODO:  添加 CreateShape.Caption getter 实现
                return null;

            }
        }
        public override string Tooltip
        {
            get
            {
                // TODO:  添加 CreateShape.Tooltip getter 实现
                return null;
            }
        }
        public int HelpContexID
        {
            get
            {
                // TODO:  添加 CreateShape.HelpContextID getter 实现
                return 0;
            }
        }
        public override string Name
        {
            get
            {
                // TODO:  添加 CreateShape.Name getter 实现
                return null;
            }
        }

        public override bool Checked
        {
            get
            {
                // TODO:  添加 CreateShape.Checked getter 实现
                return false;
            }
        }

        public override bool Enabled
        {
            get
            {
                // TODO:  添加 CreateShape.Enabled getter 实现
                return false;
            }
        }
        public override string HelpFile
        {
            get
            {
                // TODO:  添加 CreateShape.HelpFile getter 实现
                return null;
            }
        }

        public override string Category
        {
            get
            {
                // TODO:  添加 CreateShape.Category getter 实现
                return null;
            }
        }
        #endregion
        private void SelectMouseDown(int x, int y)
        {
            IFeatureLayer pFeatureLayer = (IFeatureLayer)m_CurrentLayer;
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            if (pFeatureClass == null)
                return;
            IActiveView pActiveView = m_MapControl.ActiveView;
            IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
            IGeometry pGeometry = pPoint;

            double pLength = ConvertPixelsToMapUnits(pActiveView, 4);
            ITopologicalOperator pTopo = (ITopologicalOperator)pGeometry;
            IGeometry pBuffer = pTopo.Buffer(pLength);
            pGeometry = (IGeometry)pBuffer.Envelope;

            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.Geometry = pGeometry;
            switch (pFeatureClass.ShapeType)
            { 
                case esriGeometryType.esriGeometryPoint :
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case esriGeometryType .esriGeometryPolyline :
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                case esriGeometryType .esriGeometryPolygon :
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIndexIntersects;
                    break;
            
            }
            pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;
            IQueryFilter pFilter = pSpatialFilter;

            IFeatureCursor pCursor = pFeatureLayer.Search(pFilter, false);
            IFeature pFeature = pCursor.NextFeature();
            while (pFeature != null)
            {
                m_MapControl.Map.SelectFeature(m_CurrentLayer, pFeature);
                pFeature = pCursor.NextFeature();
            }
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
        }
        private double ConvertPixelsToMapUnits(IActiveView pActiveView, double pixelUnits)
        {
            // 依据当前视图，将屏幕像素转换成地图单位
            IPoint Point1 = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.UpperLeft;
            IPoint Point2 = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.UpperRight;
            int x1, x2, y1, y2;
            pActiveView.ScreenDisplay.DisplayTransformation.FromMapPoint(Point1, out x1, out y1);
            pActiveView.ScreenDisplay.DisplayTransformation.FromMapPoint(Point2, out x2, out y2);
            double pixelExtent = x2 - x1;
            double realWorldDisplayExtent = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            double sizeOfOnePixel = realWorldDisplayExtent / pixelExtent;
            return pixelUnits * sizeOfOnePixel;
        }
        private bool TestGeometryHit(double tolerance, IPoint pPoint, IFeature pFeature, ref IPoint pHitPoint, ref double hitDist, ref int partIndex, ref int vertexIndex, ref int vertexOffset, ref bool vertexHit)
        {
            // Function returns true if a feature's shape is hit and further defines
            // if a vertex lies within the tolorance
            bool bRetVal = false;
            IGeometry pGeom = (IGeometry)pFeature.Shape;

            IHitTest pHitTest = (IHitTest)pGeom;
            pHitPoint = new ESRI.ArcGIS.Geometry.Point();
            bool bTrue = true;
            // 检查顶点是否被点击
            if (pHitTest.HitTest(pPoint, tolerance, esriGeometryHitPartType.esriGeometryPartVertex, pHitPoint, ref hitDist, ref partIndex, ref vertexIndex, ref bTrue))
            {
                bRetVal = true;
                vertexHit = true;
            }
            // 检查边界是否被点击
            else if (pHitTest.HitTest(pPoint, tolerance, esriGeometryHitPartType.esriGeometryPartBoundary, pHitPoint, ref hitDist, ref partIndex, ref vertexIndex, ref bTrue))
            {
                bRetVal = true;
                vertexHit = false;
            }

            // 统计vertexOffset顶点数目
            if (partIndex > 0)
            {
                IGeometryCollection pGeomColn = (IGeometryCollection)pGeom;
                vertexOffset = 0;
                for (int i = 0; i < partIndex; i++)
                {
                    IPointCollection pPointColn = (IPointCollection)pGeomColn.get_Geometry(i);
                    vertexOffset = vertexOffset + pPointColn.PointCount;
                }
            }
            return bRetVal;
        }
        private void UpdateFeature(IFeature pFeature, IGeometry pGeometry)   //修改要素后，更新要素
        { 
          //检查是否在编辑操作中
            IDataset pDataset = (IDataset)pFeature.Class;
            IWorkspaceEdit pWorkspaceEdit = (IWorkspaceEdit)pDataset.Workspace;

            //保存当前编辑的要素
            pWorkspaceEdit.StartEditOperation();
            pFeature.Shape = pGeometry;
            pFeature.Store();
           pWorkspaceEdit.StopEditOperation();
        }

    }
}
