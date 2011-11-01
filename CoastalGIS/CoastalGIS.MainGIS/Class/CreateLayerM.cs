using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;



namespace CoastalGIS.MainGIS
{
   public  class CreateLayerM
    {
        private IMapControlDefault m_mapControl;

        string LocalFilePath = "";//文件路径，带文件名
        string FilePath = "";//文件路径，不带文件名
        string FileName = "";//文件名

        public CreateLayerM(IMapControlDefault mapcontrol)
        {
            m_mapControl = mapcontrol;
        }

        /// <summary>
        /// 新建点图层
        /// </summary>
        public void CreatePointLayer()
        {
            SaveFileDialog sfdPoint = new SaveFileDialog();
            sfdPoint.Title = "请选择点图层的存储位置";
            sfdPoint.Filter = "Shapefile(*.shp)|*.shp|All files(*.*)|*.*";
            sfdPoint.RestoreDirectory = true;
            if (sfdPoint.ShowDialog() == DialogResult.OK)
            {
                LocalFilePath = sfdPoint.FileName;
                FilePath = System.IO.Path.GetDirectoryName(LocalFilePath);
                FileName = System.IO.Path.GetFileName(LocalFilePath);

                IFields pFields = new FieldsClass();
                IFieldsEdit pFieldsEdit = pFields as IFieldsEdit;
                IField pField = new FieldClass();
                IFieldEdit pFieldEdit = pField as IFieldEdit;
                pFieldEdit.Name_2 = "SHAPE";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;

                IGeometryDef pGeometryDef = new GeometryDefClass();
                IGeometryDefEdit pGeometryDefEdit = pGeometryDef as IGeometryDefEdit;
                pGeometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
                pFieldEdit.GeometryDef_2 = pGeometryDef;
                pFieldsEdit.AddField(pField);

                IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                IFeatureWorkspace pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(FilePath, 0) as IFeatureWorkspace;
                pFeatureWorkspace.CreateFeatureClass(FileName, pFields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
                m_mapControl.AddShapeFile(FilePath, FileName);
                m_mapControl.ActiveView.Refresh();
            }
        }

        /// <summary>
        /// 新建线图层
        /// </summary>
        public void CreatePolylineLayer()
        {
            SaveFileDialog sfdPoint = new SaveFileDialog();
            sfdPoint.Title = "请选择线图层的存储位置";
            sfdPoint.Filter = "Shapefile(*.shp)|*.shp|All files(*.*)|*.*";
            sfdPoint.RestoreDirectory = true;
            if (sfdPoint.ShowDialog() == DialogResult.OK)
            {
                LocalFilePath = sfdPoint.FileName;
                FilePath = System.IO.Path.GetDirectoryName(LocalFilePath);
                FileName = System.IO.Path.GetFileName(LocalFilePath);

                IFields pFields = new FieldsClass();
                IFieldsEdit pFieldsEdit = pFields as IFieldsEdit;
                IField pField = new FieldClass();
                IFieldEdit pFieldEdit = pField as IFieldEdit;
                pFieldEdit.Name_2 = "SHAPE";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;

                IGeometryDef pGeometryDef = new GeometryDefClass();
                IGeometryDefEdit pGeometryDefEdit = pGeometryDef as IGeometryDefEdit;
                pGeometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline;
                pFieldEdit.GeometryDef_2 = pGeometryDef;
                pFieldsEdit.AddField(pField);

                IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                IFeatureWorkspace pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(FilePath, 0) as IFeatureWorkspace;
                pFeatureWorkspace.CreateFeatureClass(FileName, pFields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
                m_mapControl.AddShapeFile(FilePath, FileName);
                m_mapControl.ActiveView.Refresh();
            }
        }

        /// <summary>
        /// 新建面图层
        /// </summary>
       public IFeatureClass CreatePolygonLayer(IPolygon polygon)
       {
           //LocalFilePath = sfdPoint.FileName;
           FilePath = Application.StartupPath + "\\tempSHP";
           FileName = "test";

           IFields pFields = new FieldsClass();
           IFieldsEdit pFieldsEdit = pFields as IFieldsEdit;
           IField pField = new FieldClass();
           IFieldEdit pFieldEdit = pField as IFieldEdit;
           pFieldEdit.Name_2 = "SHAPE";
           pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;

           IGeometryDef pGeometryDef = new GeometryDefClass();
           IGeometryDefEdit pGeometryDefEdit = pGeometryDef as IGeometryDefEdit;
           pGeometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
           pFieldEdit.GeometryDef_2 = pGeometryDef;
           pFieldsEdit.AddField(pField);

           pField = new FieldClass();
           pFieldEdit = pField as IFieldEdit;
           pFieldEdit.Name_2 = "HEIGHT";
           pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
           pFieldsEdit.AddField(pField);

           

           IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
           IFeatureWorkspace pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(FilePath, 0) as IFeatureWorkspace;
           IFeatureClass feaC = pFeatureWorkspace.CreateFeatureClass(FileName, pFields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
           IFeature pFeature = feaC.CreateFeature();
           pFeature.Shape = polygon as IGeometry;
           pFeature.Store();

           IFeatureCursor feaCur = feaC.Search(null, true);
           pFeature = feaCur.NextFeature();
           while (pFeature != null)
           {
               //pFeature.set_Value(2, "181");
               pFeature.Store();
               pFeature = feaCur.NextFeature();
           }

           //m_mapControl.AddShapeFile(FilePath, FileName);
           //m_mapControl.ActiveView.Refresh();
           return feaC;
       }

    }
}
