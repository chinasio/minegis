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

namespace CoastalGIS.ExportMapProj
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

        string textTitle = ""; //标题，制作者等信息
        string textName = "";
        string textProject = "";
        string textDate = "";
        string textelevation = "";
        string textOtherInfo = "";
        frmTempleteProperties frmTempletePro = null;
        public AreaPrintMapClass(IMap pMap)
        {
            base.m_deactivate = true;
            base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            frmTempletePro = new frmTempleteProperties(pMap, m_Geometry);
            frmTempletePro.ShowDialog();
            textTitle = frmTempletePro.ptextTitle;
            textName = frmTempletePro.ptextName;
            textProject = frmTempletePro.ptextProject;
            textDate = frmTempletePro.ptextTime;

            textelevation = frmTempletePro.elevationName;
            textOtherInfo = frmTempletePro.pOtherInfo;
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
            m_Geometry = (IGeometry)m_NewPolygonFeedback.Stop();
            IMap pMap = m_hookHelper.FocusMap;
            ISpatialReference spatialReferencr = pMap.SpatialReference;
            m_Geometry.SpatialReference = spatialReferencr;
            IBorder pBorder = new SymbolBorderClass();
            m_hookHelper.FocusMap.ClipBorder = pBorder;
            // m_hookHelper.FocusMap.ClipGeometry = m_Geometry;
            m_ActiveView.Extent = m_Geometry.Envelope;
            m_ActiveView.Refresh();
            m_cursor = base.m_cursor;
            m_InUse = false;
         /* string textTitle=""; //标题，制作者等信息
          string textName="";
          string textProject="";
          string textDate="";
          string textelevation="";
          string textOtherInfo="";
            frmTempleteProperties frmTempletePro = new frmTempleteProperties( pMap   , m_Geometry);
            frmTempletePro.ShowDialog();
            textTitle = frmTempletePro.ptextTitle;
            textName = frmTempletePro.ptextName;
            textProject = frmTempletePro.ptextProject;
            textDate = frmTempletePro.ptextTime;
          
            textelevation = frmTempletePro.elevationName;
            textOtherInfo =frmTempletePro .pOtherInfo ;*/
            m_ActiveView.Refresh();
            if (!frmTempletePro.ifCancle)
            {
                frmPrintByAnyRegion frmPrint = new frmPrintByAnyRegion(pMap , m_Geometry, textTitle,
                     textName, textProject, textDate, textelevation, textOtherInfo);// checkLegend, checkNorthArrow, checkMapGrid, checkScaleBar, checkText);
                frmPrint.ShowDialog();

            }
            else
            { 
            
            }

        }
        #endregion
        private void ClipArea(ILayer pLayer,int index)
        {
            ISpatialFilter pFilter = new SpatialFilterClass();
            pFilter.Geometry = m_Geometry;
            pFilter.GeometryField = "SHAPE";
            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            IFeatureClass pFlnClass;
          //  IFields plnFCFields;
            IFeatureCursor plnCursor;
            IFeature pFeature;

            IFeatureClass pFOutClass;
            IFeatureCursor pOutCur;
            //IFeatureCursor pOutFeatureCur;
            IFeatureBuffer pOutBuff;
            IFeature pOutFeature;

            //裁剪
            ITopologicalOperator pTopo;
            //int indexl;
            // int i;
            IGeometry pGeom;
            IFields plnFields;

            if (pLayer is IFeatureLayer)
            {
                IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                pFlnClass = pFeatureLayer.FeatureClass;
                plnCursor = pFlnClass.Search(pFilter, false);
                plnFields = pFlnClass.Fields;
                IFeatureLayer pNewFeatrueLayer = null;
                try
                {
                    pNewFeatrueLayer = CreateFeatureLayerInmemeory(pLayer.Name, pLayer .Name +index .ToString (),m_hookHelper .FocusMap .SpatialReference , pFlnClass.ShapeType, plnFields);
                }
                catch(Exception ee)
                {
                    MessageBox.Show(ee.ToString());
                }
                pFOutClass = pNewFeatrueLayer.FeatureClass;
                pFeature = plnCursor.NextFeature();
                while (pFeature != null)
                {
                    pOutCur = pFOutClass.Insert(true);
                    pOutBuff = pFOutClass.CreateFeatureBuffer();
                    pOutFeature = pOutBuff as IFeature;

                   // indexl = pFeature.Fields.FindField("Shape");
                    pTopo = pFeature.Shape as ITopologicalOperator;//这里判断一下该地物几何类型
                    pGeom = pFeature.Shape;
                    if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                    {
                        pGeom = pTopo.Intersect(m_Geometry, esriGeometryDimension.esriGeometry0Dimension); //这里的第二个参数就是区分点、线、面的

                    }
                    else if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
                    {
                        pGeom = pTopo.Intersect(m_Geometry, esriGeometryDimension.esriGeometry1Dimension);
                    }
                    else if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                    {
                        pGeom = pTopo.Intersect(m_Geometry, esriGeometryDimension.esriGeometry2Dimension);

                    }
                    pOutFeature.Shape = pGeom;

                    CopyAttributes(pFeature, pOutFeature);
                    pOutCur.InsertFeature(pOutBuff);
                    pNewFeatrueLayer.FeatureClass = pFOutClass;

                    ///////////////////////////////////
                    string FilePath = @"C:\任意打印\";
                    ILayerFile layerFile = new LayerFileClass();
                    layerFile.New(FilePath + pLayer .Name+ ".lyr");
                    layerFile.ReplaceContents(pNewFeatrueLayer as ILayer);
                    layerFile.Save();

                    /////////////////////////////////


                    try
                    {
                        m_ClipMap.AddLayer(layerFile.Layer );
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.ToString());
                    }
                    pFeature = plnCursor.NextFeature();


                }

            }
            else if (pLayer is IRasterLayer)
            {

            }

        }
       private void CopyAttributes(IFeature pSourceFeature, IFeature pDestinationFeature)
        {
            IField pField;
            IFields pFields;
            //IRow pRow;
            int pCount=0;
            int i;
            int pindex;
            bool bGet;
            try
            {
                pFields = pDestinationFeature.Fields;
                if (pSourceFeature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                    pCount = 1;
                else if (pSourceFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
                    pCount = 2;
                else if (pSourceFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                    pCount = 3;
                for (i = 0; i < pDestinationFeature.Fields.FieldCount; i++)
                {
                    pField = pFields.get_Field(i);
                    if (pField.Name.ToUpper ()!= "OBJECTID" && pField.Name.ToUpper () != "FID"  && pField.Name.ToUpper () != "SHAPE")
                    {
                        bGet = false;
                        for (int j = 0; j < pSourceFeature.Fields.FieldCount - pCount; j++)
                        {
                            if (pSourceFeature.Fields.get_Field(j).AliasName == pField.Name)
                            {
                                if (pSourceFeature.get_Value(j) != null)
                                {
                                    pDestinationFeature.set_Value (i, pSourceFeature.get_Value(j));
                                }
                                bGet = true;
                            } 
                        }
                        if (!bGet)
                        {
                            if (pField.Name == "SHAPE_Length")
                            {
                                pindex = pSourceFeature.Fields.FindField("SHAPE_Length");
                                pDestinationFeature.set_Value(i, pSourceFeature.get_Value(pindex));
                            }
                            else if (pField.Name == "SHAPE_Area")
                            {
                                pindex = pSourceFeature.Fields.FindField("SHAPE_Area");
                                pDestinationFeature.set_Value(i, pSourceFeature.get_Value(pindex));
                            }
                        }
                    }
                    

                }
              }
            catch
            { 
            
            }
            pField = null;
            pFields = null;
           // pRow = null;
        }

        public IFeatureLayer  CreateLayer(IFeatureClass pFeatureClass,int index)  //新建任意范围打印地图的图层
        {
          
               string FilePath = @"C:\任意打印\";
               string FileName = "bb"+index .ToString ();
            /////////////////////////////////////////////////
              // ILayerFile layerFile = new LayerFileClass();

              // layerFile.New(FilePath + FileName);

            /////////////////////////////////////////////


               IFields pFields = pFeatureClass.Fields;
               IClone pClone = pFields as IClone;
               IFields pCloneFields = pClone.Clone() as IFields;
               IDataset pDataSet = pFeatureClass as IDataset;
               IWorkspace pWorkSpace = pDataSet.Workspace;
              IFeatureWorkspace pFeatWorkspace = pWorkSpace as IFeatureWorkspace;

               IFeatureClass newFeatcls = pFeatWorkspace.CreateFeatureClass(FileName, pCloneFields,
                   null, null, esriFeatureType.esriFTSimple, pFeatureClass.ShapeFieldName, "");
               IFeatureLayer pFeatureLayer = new FeatureLayerClass();
               pFeatureLayer.FeatureClass = newFeatcls;
               pFeatureLayer.Name = newFeatcls.AliasName;

               return pFeatureLayer;

              // System.Runtime.InteropServices.Marshal.ReleaseComObject(FilePath);
              // System.Runtime.InteropServices.Marshal.ReleaseComObject(FileName );
              // System.Runtime.InteropServices.Marshal.ReleaseComObject(pFields );
               //System.Runtime.InteropServices.Marshal.ReleaseComObject(pDataSet );
              // System.Runtime.InteropServices.Marshal.ReleaseComObject(newFeatcls);
              

        }

        /// <summary>
        /// 在内存中创建图层
        /// </summary>
        /// <param name="DataSetName">数据集名称</param>
        /// <param name="AliaseName">别名</param>
        /// <param name="SpatialRef">空间参考</param>
        /// <param name="GeometryType">几何类型</param>
        /// <param name="PropertyFields">属性字段集合</param>
        /// <returns>IfeatureLayer</returns>
          public static IFeatureLayer CreateFeatureLayerInmemeory(string DataSetName, string AliaseName, ISpatialReference SpatialRef, esriGeometryType GeometryType, IFields PropertyFields)
          {
              IWorkspaceFactory workspaceFactory = new InMemoryWorkspaceFactoryClass();
              ESRI.ArcGIS.Geodatabase.IWorkspaceName workspaceName = workspaceFactory.Create("", "MyWorkspace", null, 0);
              ESRI.ArcGIS.esriSystem.IName name = (IName)workspaceName;
              ESRI.ArcGIS.Geodatabase.IWorkspace inmemWor = (IWorkspace)name.Open();

              IField oField = new FieldClass();
              IFields oFields = new FieldsClass();
              IFieldsEdit oFieldsEdit = null;
              IFieldEdit oFieldEdit = null;
              IFeatureClass oFeatureClass = null;
              IFeatureLayer oFeatureLayer = null;

              try
              {
                  oFieldsEdit = oFields as IFieldsEdit;
                  oFieldEdit = oField as IFieldEdit;
                  if (PropertyFields != null)
                  {
                      for (int i = 0; i < PropertyFields.FieldCount; i++)
                      {
                          oFieldsEdit.AddField(PropertyFields.get_Field(i));
                      }
                  }

                  IGeometryDef geometryDef = new GeometryDefClass();
                  IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
                  geometryDefEdit.AvgNumPoints_2 = 5;
                  geometryDefEdit.GeometryType_2 = GeometryType;
                  geometryDefEdit.GridCount_2 = 1;
                  geometryDefEdit.HasM_2 = false;
                  geometryDefEdit.HasZ_2 = false;
                  geometryDefEdit.SpatialReference_2 = SpatialRef;
                  geometryDefEdit.SpatialReference_2 = new UnknownCoordinateSystemClass();//没有这一句就报错，说尝试读取或写入受保护的内存。
                  geometryDefEdit.SpatialReference.SetDomain(-450359962737.05, 450359962737.05, -450359962737.05, 450359962737.05);//没有这句就抛异常来自HRESULT：0x8004120E。


                  oFieldEdit.Name_2 = "SHAPE";
                  oFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
                  oFieldEdit.GeometryDef_2 = geometryDef;
                  oFieldEdit.IsNullable_2 = true;
                  oFieldEdit.Required_2 = true;
                  oFieldsEdit.AddField(oField);

                  oFeatureClass = (inmemWor as IFeatureWorkspace).CreateFeatureClass(DataSetName, oFields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
                  (oFeatureClass as IDataset).BrowseName = DataSetName;

                  oFeatureLayer = new FeatureLayerClass();
                  oFeatureLayer.Name = AliaseName;
                  oFeatureLayer.FeatureClass = oFeatureClass;
              }
              catch (Exception ex)
              {
              }
              finally
              {
                  try
                  {
                      System.Runtime.InteropServices.Marshal.ReleaseComObject(oField);
                      System.Runtime.InteropServices.Marshal.ReleaseComObject(oFields);
                      System.Runtime.InteropServices.Marshal.ReleaseComObject(oFieldsEdit);
                      System.Runtime.InteropServices.Marshal.ReleaseComObject(oFieldEdit);
                      System.Runtime.InteropServices.Marshal.ReleaseComObject(name);
                      System.Runtime.InteropServices.Marshal.ReleaseComObject(workspaceFactory);
                      System.Runtime.InteropServices.Marshal.ReleaseComObject(workspaceName);
                      System.Runtime.InteropServices.Marshal.ReleaseComObject(inmemWor);
                      System.Runtime.InteropServices.Marshal.ReleaseComObject(oFeatureClass);
                  }
                  catch { }

                  GC.Collect();
              }
              return oFeatureLayer;
          }


    }
}
