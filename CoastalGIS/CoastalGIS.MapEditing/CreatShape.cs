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

using CoastalGIS.MainGIS;

using System.Windows.Forms;

namespace CoastalGIS.MapEditing
{
    /// <summary>
    /// Summary description for CreatShape.
    /// </summary>

    public sealed class CreatShape : BaseTool ,ICommand 
    {
         //  private ESRI .ArcGIS .Controls.MapControlClass   m_MapControl;
        private IHookHelper m_hookHelper = null;
        ILayer m_CurrentLayer;
        private IPoint m_PointStop;
        private IDisplayFeedback m_Feedback;
        private bool m_bInUse;
        private bool m_dig;
      
        public CreatShape(ILayer pCurrentLayer,bool dig)
        {
            //
            // TODO: Define values for the public properties
            //
             m_CurrentLayer = pCurrentLayer;
             m_dig = dig;
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
            m_bInUse = false;


            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add CreatShape.OnClick implementation
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add CreatShape.OnMouseDown implementation
            if (Button == 1)
            {
                IFeatureLayer m_FeatureLayer = (IFeatureLayer)m_CurrentLayer;
                if (m_FeatureLayer.FeatureClass == null) return;

                IActiveView m_ActiveView = m_hookHelper .ActiveView;
                IPoint m_PointMousedown = m_ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
                m_PointStop = m_PointMousedown;
                if (!m_bInUse)
                {
                    switch (m_FeatureLayer.FeatureClass.ShapeType)
                    {
                        case esriGeometryType.esriGeometryPoint:      //������
                            CreatFeature(m_PointMousedown);
                            m_hookHelper.ActiveView.Refresh();
                            break;
                        case esriGeometryType.esriGeometryPolyline:    //������
                            m_bInUse = true;
                            m_Feedback = new NewLineFeedback();
                            INewLineFeedback m_LineFeed = (INewLineFeedback)m_Feedback;
                            m_LineFeed.Start(m_PointMousedown);
                            break;
                        case esriGeometryType.esriGeometryPolygon:
                            m_bInUse = true;
                            m_Feedback = new NewPolygonFeedback();
                            INewPolygonFeedback m_PolyFeed = (INewPolygonFeedback)m_Feedback;
                            m_PolyFeed.Start(m_PointMousedown);
                            break;

                    }
                    if (m_Feedback != null)
                        m_Feedback.Display = m_ActiveView.ScreenDisplay;

                }
                else
                {
                    if (m_Feedback is INewLineFeedback)
                    {
                        INewLineFeedback m_LineFeed = (INewLineFeedback)m_Feedback;
                        m_LineFeed.AddPoint(m_PointMousedown);
                    }
                    else if (m_Feedback is INewPolygonFeedback)
                    {
                        INewPolygonFeedback m_PolyFeed = (INewPolygonFeedback)m_Feedback;
                        m_PolyFeed.AddPoint(m_PointMousedown);
                    }
                }

            }

        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add CreatShape.OnMouseMove implementation
            if (!m_bInUse || m_Feedback == null)
                return;
            //�ƶ�����γ��ߣ���Ľ��
            IActiveView m_ActiveView = m_hookHelper.ActiveView;
            m_Feedback.MoveTo(m_ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y));
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add CreatShape.OnMouseUp implementation
        }
        public override void OnKeyDown(int keyCode, int shift)
        {
            // TODO:  ��� CreateShape.OnKeyDown ʵ��
        }

        public override void OnKeyUp(int keyCode, int shift)
        {
            // TODO:  ��� CreateShape.OnKeyUp ʵ��
        }

        public override int Cursor
        {
            get
            {
                // TODO:  ��� CreateShape.Cursor getter ʵ��
                return 0;
            }
        }
        public override bool OnContextMenu(int x, int y)
        {
            // TODO:  ��� CreateShape.OnContextMenu ʵ��
            return false;
        }

        public override bool Deactivate()
        {
            // TODO:  ��� CreateShape.Deactivate ʵ��
            return true;                                          //����true����֤����ע��ʱ������
        }

        public override void Refresh(int hdc)
        {
            // TODO:  ��� CreateShape.Refresh ʵ��

        }
        public override  void  OnDblClick()
        {
            EndSketch();
        }



        #endregion

        public override string Message
        {
            get
            {
                // TODO:  ��� CreateShape.Message getter ʵ��
                return null;
            }
        }

        public override int Bitmap
        {
            get
            {
                // TODO:  ��� CreateShape.Bitmap getter ʵ��
                return 0;
            }
        }
        public override  string Caption
        {
            get
            {// TODO:  ��� CreateShape.Caption getter ʵ��
                return null;

            }
        }
        public override string Tooltip
        {
            get
            {
                // TODO:  ��� CreateShape.Tooltip getter ʵ��
                return null;
            }
        }
        public int HelpContexID
        {
            get
            {
                // TODO:  ��� CreateShape.HelpContextID getter ʵ��
                return 0;
            }
        }
        public override string Name
        {
            get
            {
                // TODO:  ��� CreateShape.Name getter ʵ��
                return null;
            }
        }

        public override bool Checked
        {
            get
            {
                // TODO:  ��� CreateShape.Checked getter ʵ��
                return false;
            }
        }

        public override bool Enabled
        {
            get
            {
                // TODO:  ��� CreateShape.Enabled getter ʵ��
                return false;
            }
        }

        public override string HelpFile
        {
            get
            {
                // TODO:  ��� CreateShape.HelpFile getter ʵ��
                return null;
            }
        }

        public override string Category
        {
            get
            {
                // TODO:  ��� CreateShape.Category getter ʵ��
                return null;
            }
        }
        private void EndSketch()
        {
            IGeometry m_Geometry = null;
            IPointCollection m_PointCollection = null;

            if (m_Feedback is INewLineFeedback)
            {
                INewLineFeedback m_LineFeed = (INewLineFeedback)m_Feedback;
                m_LineFeed.AddPoint(m_PointStop);
                IPolyline m_PolyLine = m_LineFeed.Stop();
                m_PointCollection = (IPointCollection)m_PolyLine;
                if (m_PointCollection.PointCount < 2)
                {
                    MessageBox.Show("��Ҫ�������������һ���ߣ�", "δ��������", MessageBoxButtons.OK);
                    return;
                }
                else
                {
                    m_Geometry = (IGeometry)m_PointCollection;
                }
            }
            else if (m_Feedback is INewPolygonFeedback)
            {
                INewPolygonFeedback m_PolyFeed = (INewPolygonFeedback)m_Feedback;
                m_PolyFeed.AddPoint(m_PointStop);
                IPolygon m_Polygon = m_PolyFeed.Stop();
                if (m_Polygon != null)
                    m_PointCollection = (IPointCollection)m_Polygon;
                if (m_PointCollection.PointCount < 3)
                {
                    MessageBox.Show("��Ҫ�������������һ���棡", "δ��������", MessageBoxButtons.OK);
                    return;
                }
                else
                {
                    m_Geometry = (IGeometry)m_PointCollection;
                }
            }

            CreatFeature(m_Geometry);
            m_hookHelper.ActiveView.Refresh();
            m_Feedback = null;
            m_bInUse = false;


        }

        private void CreatFeature(IGeometry m_Geometry)        //����Ҫ��
        {
            if (this.m_dig == true)
            {
                if (m_Geometry == null)
                    return;
                if (m_CurrentLayer == null)
                    return;
                IWorkspaceEdit m_WorkspaceEdit = GetWorkspaceEdit();
                IFeatureLayer m_FeatureLayer = (IFeatureLayer)m_CurrentLayer;
                IFeatureClass m_FeatureClass = m_FeatureLayer.FeatureClass;

                m_WorkspaceEdit.StartEditOperation();   //ʹ��WorkspaceEdit�ӿ��½�Ҫ��
                IFeature m_Feature = m_FeatureClass.CreateFeature();
                m_Feature.Shape = m_Geometry;
                frmType type = new frmType(m_Feature);
                type.ShowDialog();
                m_Feature.Store();
                m_WorkspaceEdit.StopEditOperation();

                //��һ�����巶Χˢ����ͼ
                IActiveView m_ActiveView = m_hookHelper.ActiveView;
                if (m_Geometry.GeometryType == esriGeometryType.esriGeometryPoint)
                {
                    double pLength;
                    pLength = ConvertPixelsToMapUnits(m_ActiveView, 30);
                    ITopologicalOperator m_Topo = (ITopologicalOperator)m_Geometry;
                    IGeometry m_Buffer = m_Topo.Buffer(pLength);
                    m_ActiveView.PartialRefresh((esriViewDrawPhase)(esriDrawPhase.esriDPGeography | esriDrawPhase.esriDPSelection), m_CurrentLayer, m_Buffer.Envelope);
                }
                else
                    m_ActiveView.PartialRefresh((esriViewDrawPhase)(esriDrawPhase.esriDPGeography | esriDrawPhase.esriDPSelection), m_CurrentLayer, m_Geometry.Envelope);
            }
            else 
            {
                if (m_Geometry == null)
                    return;
                if (m_CurrentLayer == null)
                    return;
                IWorkspaceEdit m_WorkspaceEdit = GetWorkspaceEdit();
                IFeatureLayer m_FeatureLayer = (IFeatureLayer)m_CurrentLayer;
                IFeatureClass m_FeatureClass = m_FeatureLayer.FeatureClass;

                m_WorkspaceEdit.StartEditOperation();   //ʹ��WorkspaceEdit�ӿ��½�Ҫ��
                IFeature m_Feature = m_FeatureClass.CreateFeature();
                m_Feature.Shape = m_Geometry;
                m_Feature.Store();

                m_WorkspaceEdit.StopEditOperation();

                //��һ�����巶Χˢ����ͼ
                IActiveView m_ActiveView = m_hookHelper.ActiveView;
                if (m_Geometry.GeometryType == esriGeometryType.esriGeometryPoint)
                {
                    double pLength;
                    pLength = ConvertPixelsToMapUnits(m_ActiveView, 30);
                    ITopologicalOperator m_Topo = (ITopologicalOperator)m_Geometry;
                    IGeometry m_Buffer = m_Topo.Buffer(pLength);
                    m_ActiveView.PartialRefresh((esriViewDrawPhase)(esriDrawPhase.esriDPGeography | esriDrawPhase.esriDPSelection), m_CurrentLayer, m_Buffer.Envelope);
                }
                else
                    m_ActiveView.PartialRefresh((esriViewDrawPhase)(esriDrawPhase.esriDPGeography | esriDrawPhase.esriDPSelection), m_CurrentLayer, m_Geometry.Envelope);
            }

        }
        private IWorkspaceEdit GetWorkspaceEdit()
        {
            if (m_CurrentLayer == null) return null;

            IFeatureLayer m_FeatureLayer = (IFeatureLayer)m_CurrentLayer;
            IFeatureClass m_FeatureClass = m_FeatureLayer.FeatureClass;
            IDataset m_DataSet = (IDataset)m_FeatureClass;
            if (m_DataSet == null)
                return null;
            return (IWorkspaceEdit)m_DataSet.Workspace;
        }

        private double ConvertPixelsToMapUnits(IActiveView pActiveView, double pixelUnits)
        {
            //���ݵ�ǰ��ͼ������ĻҪ��ת���ɵ�ͼ��λ
            IPoint Point1 = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.UpperLeft;
            IPoint Point2 = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.UpperRight;
            int x1, x2, y1, y2;
            pActiveView.ScreenDisplay.DisplayTransformation.FromMapPoint(Point1, out x1, out y1);
            pActiveView.ScreenDisplay.DisplayTransformation.FromMapPoint(Point1, out x2, out y2);
            double pixeExtent = x2 - x1;
            double realWorldDisplayExtent = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            double sizeOfOnePixel = realWorldDisplayExtent / pixeExtent;
            return pixelUnits * sizeOfOnePixel;

        }


    }
}
