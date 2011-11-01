using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Utility.BaseClasses;
using ESRI.ArcGIS.esriSystem;

namespace CoastalGIS.MainGIS
{
    public partial class BufferForm : Form
    {
        private IMapControlDefault m_mapControl = null;
        //private IMap pMap;
        //private IActiveView pActiveview;

        public BufferForm(IMapControlDefault mapControl)
        {
            InitializeComponent();
           // pMap = ParentForm.mapCtlMain.ActiveView.FocusMap;
           // pActiveview = ParentForm.mapCtlMain.ActiveView;
            this.m_mapControl = mapControl;
        }

        private void BufferForm_Load(object sender, EventArgs e)
        {
            switch (m_mapControl.MapUnits)
            {
                case esriUnits .esriMeters:
                    label2.Text = "米";
                    break;
                case esriUnits.esriKilometers :
                    label2.Text = "千米";
                    break;
                case esriUnits .esriCentimeters:
                    label2.Text = "分米";
                    break;
                case esriUnits .esriDecimalDegrees:
                    label2.Text = "度";
                    break;
                case esriUnits .esriFeet:
                    label2.Text = "英忖";
                    break;
                case esriUnits.esriInches:
                    label2.Text = "英尺";
                    break;
                case esriUnits.esriMiles:
                    label2.Text = "英里";
                    break;
                case esriUnits.esriMillimeters:
                    label2.Text = "毫米";
                    break;
                default:
                    label2.Text = "位置单位";
                    break;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            BufferText.Text = "";
        }

        private void Btnok_Click(object sender, EventArgs e)
        {
            if (m_mapControl.Map.SelectionCount == 0)
            {
                MessageBox.Show("请先选择待查询要素！");
                return;
            }
            IEnumFeature pEnumfeature;
            IFeature pFeature;
            IFeature newFeat;
            //ITopologicalOperator pToplogicalOper;
            //IPolygon pPolygon;
            double bufferDistence;

            if (m_mapControl.Map.SelectionCount == 0)
            {
                return;
            }
            bufferDistence = Convert.ToDouble(BufferText.Text);
            pEnumfeature = m_mapControl.Map.FeatureSelection as IEnumFeature;
            pEnumfeature.Reset();
            pFeature = pEnumfeature.Next();
            IFeatureClass pFeatureclass;
            FileInfo n = new FileInfo(textBox1.Text);
            string openpath = n.Directory.ToString();//取路径的父目录用于存新的layer
            pFeatureclass = CreatNewShapefile(openpath, n.Name,m_mapControl.SpatialReference);

            //IFeatureBuffer pFeatureBuffer = pFeatureclass.CreateFeatureBuffer();
            //IFeatureCursor pFeatureCursor = pFeatureclass.Insert(true);
            int iFieldAttribute = pFeatureclass.FindField("Text");
            //while (pFeature != null)
            //{
            //    pToplogicalOper = pFeature.Shape as ITopologicalOperator;
            //    pPolygon = new PolygonClass();
            //    pPolygon = pToplogicalOper.Buffer(bufferDistence) as IPolygon;
            //    pFeatureBuffer.Shape = pPolygon;
            //    //pFeatureBuffer.set_Value(iFieldAttribute, pFeature.OID);
            //    pFeatureCursor.InsertFeature(pFeatureBuffer);
            //    pFeature = pEnumfeature.Next();
            //}
            //pFeatureCursor.Flush();

            //ML修改代码
            IGeometryCollection inputGeom = new GeometryBagClass();
            IGeometryBag geomBag = inputGeom as IGeometryBag;
            object missing = Type.Missing;
            while (pFeature != null)
            {
                inputGeom.AddGeometry(pFeature.ShapeCopy, ref missing, ref missing);
                pFeature = pEnumfeature.Next();
            }



            IBufferConstruction bfCon = new BufferConstructionClass();
            IBufferConstructionProperties bfConProp = bfCon as IBufferConstructionProperties;
            ISpatialReferenceFactory spatialRefFac = new SpatialReferenceEnvironmentClass();
            bfConProp.EndOption = esriBufferConstructionEndEnum.esriBufferRound;
            bfConProp.SideOption = esriBufferConstructionSideEnum.esriBufferFull;
            bfConProp.ExplodeBuffers = false;
            bfConProp.OutsideOnly = false;
            bfConProp.GenerateCurves = true;
            bfConProp.UnionOverlappingBuffers = true;
            bfConProp.DensifyDeviation = -1;
            IGeometryCollection outGeom=new GeometryBagClass ();
            bfCon.ConstructBuffers(inputGeom as IEnumGeometry, bufferDistence, outGeom);
            for (int i = 0; i < outGeom.GeometryCount; i++)
            {
                newFeat = pFeatureclass.CreateFeature();
                newFeat.Shape = outGeom.get_Geometry(i);
                newFeat.Store();
            }
            newFeat = null;
            //添加图层进map
            IFeatureLayer pOutputFeatureLayer;
            pOutputFeatureLayer = new FeatureLayerClass();
            pOutputFeatureLayer.FeatureClass = pFeatureclass;
            pOutputFeatureLayer.Name = pFeatureclass.AliasName;
            m_mapControl.Map.AddLayer(pOutputFeatureLayer);


            this.Dispose();
        }

        private IFeatureClass CreatNewShapefile(string Path, string Name,ISpatialReference sref)
        {
            IWorkspaceFactory workspaceFactory;
            workspaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace featureWorkspace;
            featureWorkspace = workspaceFactory.OpenFromFile(Path, 0) as IFeatureWorkspace;

            IFields fields;
            fields = new FieldsClass();
            IFieldsEdit fieldsEdit;
            fieldsEdit = fields as IFieldsEdit;

            IField field;
            IFieldEdit fieldEdit;

            field = new FieldClass();
            fieldEdit = field as IFieldEdit;
            fieldEdit.Name_2 = "Shape";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;

            IGeometryDef geometryDef;
            geometryDef = new GeometryDefClass();
            IGeometryDefEdit geometryDefEdit;
            geometryDefEdit = geometryDef as IGeometryDefEdit;
            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            //geometryDefEdit.SpatialReference_2 = new UnknownCoordinateSystemClass() as ISpatialReference;
            geometryDefEdit.SpatialReference_2 = sref;
            fieldEdit.GeometryDef_2 = geometryDef;
            fieldsEdit.AddField(field);

            field = new FieldClass();
            fieldEdit = field as IFieldEdit;
            fieldEdit.Name_2 = "Text";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldEdit.Length_2 = 25;
            fieldsEdit.AddField(field);

            IFeatureClass featureClass;
            featureClass = featureWorkspace.CreateFeatureClass(Name, fields, null, null, esriFeatureType.esriFTSimple, "Shape", "");
            return featureClass;
        }

        /// <summary>
        /// 添加路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFD = new SaveFileDialog();
            SaveFD.Filter = "矢量数据(*.shp)|*.shp";
            SaveFD.Title = "保存文件";
            if (SaveFD.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = SaveFD.FileName;
                Btnok.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}