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

        string LocalFilePath = "";//�ļ�·�������ļ���
        string FilePath = "";//�ļ�·���������ļ���
        string FileName = "";//�ļ���

        public CreateLayerM(IMapControlDefault mapcontrol)
        {
            m_mapControl = mapcontrol;
        }

        /// <summary>
        /// �½���ͼ��
        /// </summary>
        public void CreatePointLayer()
        {
            SaveFileDialog sfdPoint = new SaveFileDialog();
            sfdPoint.Title = "��ѡ���ͼ��Ĵ洢λ��";
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
        /// �½���ͼ��
        /// </summary>
        public void CreatePolylineLayer()
        {
            SaveFileDialog sfdPoint = new SaveFileDialog();
            sfdPoint.Title = "��ѡ����ͼ��Ĵ洢λ��";
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
        /// �½���ͼ��
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
