using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.DataManagementTools;


namespace CoastalGIS.SpatialDataBase
{
    /// <summary>
    ///空间数据库中的数据操作
    /// </summary>
    public  class GDBData
    {
        private IWorkspace m_workSpace=null;
        private OleDbCommand m_oraCmd = null;
        public OleDbCommand OraCmd 
        {
            get { return m_oraCmd; }
            set { m_oraCmd = value; }
        }

        public GDBData(IWorkspace workspace) 
        {
            this.m_workSpace = workspace;
        }

        public GDBData()
        {
        }

        public IList<FeatureName> get_FeatureDataSetNames() //遍历矢量要素集
        {
            IList<FeatureName> names=new List<FeatureName>();           
            if (this.m_workSpace != null) 
            {
                IFeatureClass feaClass;
                IEnumDatasetName enumDatasetName = m_workSpace.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
                IDatasetName datasetName = enumDatasetName.Next();//DataSet的名字
                while (datasetName != null) 
                {
                    FeatureName name = new FeatureName();
                    name.FeatDSName = datasetName.Name.ToString();
                    IFeatureDatasetName featureDatasetName = (IFeatureDatasetName)datasetName;
                    IEnumDatasetName enumDatasetNameFC = featureDatasetName.FeatureClassNames;	//是FeatureClass的名字集合							
                    IDatasetName datasetNameFC = enumDatasetNameFC.Next();
                    while(datasetNameFC != null)
                    {
                        //添加FeatureClass的名字
                        name.FCName.Add(datasetNameFC.Name.ToString());
                        feaClass = ((IFeatureWorkspace)m_workSpace).OpenFeatureClass(datasetNameFC.Name.ToString());
                        name.ShapType.Add(feaClass.ShapeType);
                        name.FeatureType.Add(feaClass.FeatureType);
                        datasetNameFC = enumDatasetNameFC.Next();


                    }
                    names.Add(name);

                    datasetName = enumDatasetName.Next();
                }

            }
            return names;
        }

        public IList<string> get_FeatureClassNames() //矢量要素
        {
            IList<string> names=new List<string>();
            if (this.m_workSpace != null) 
            {

                IEnumDatasetName enumDatasetName = m_workSpace.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
                IDatasetName datasetName = enumDatasetName.Next();//DataSet的名字
                while (datasetName != null) 
                {
                    names.Add(datasetName.Name.ToString());
                    datasetName = enumDatasetName.Next();
                }
            }
            return names;
        }

        public IList<RasterName> get_RasterDataSetNames()
        {
            IList<RasterName> names = new List<RasterName>();
            if (this.m_workSpace != null)
            {
                
                IEnumDatasetName enumDatasetName = m_workSpace.get_DatasetNames(esriDatasetType.esriDTRasterCatalog);
                IDatasetName datasetName = enumDatasetName.Next();//DataSet的名字
                IRasterWorkspaceEx rasterWorkspaceEx=m_workSpace as IRasterWorkspaceEx;
                while (datasetName != null)
                {
                    RasterName name = new RasterName();
                    name.RasDCName = datasetName.Name.ToString();
                    IRasterCatalogName rasterCaName = (IRasterCatalogName)datasetName;
                    IRasterCatalog rasterCatalog= rasterWorkspaceEx.OpenRasterCatalog(name.RasDCName);
                    ITable table = rasterCatalog as ITable;
                    ICursor cursor = table.Search(null,false);
                    IRow row = cursor.NextRow();
                    while(row != null)
                    {
                        name.RDSName.Add(row.get_Value(cursor.FindField("NAME")).ToString());

                        row = cursor.NextRow();
                    }
                    names.Add(name);
                    datasetName = enumDatasetName.Next();
                }
            }
            return names;
        }

        public IFeatureLayer AddFeatureClassToMap(string layerName) 
        {
            IFeatureWorkspace feaWorkSpace = this.m_workSpace as IFeatureWorkspace;
            IFeatureClass featClass = feaWorkSpace.OpenFeatureClass(layerName);
            if (featClass.FeatureType == esriFeatureType.esriFTAnnotation)
            {
                IDataset dataSet = featClass as IDataset;
                IFDOGraphicsLayerFactory pGLF = new FDOGraphicsLayerFactoryClass();
                IFDOGraphicsLayer pFDOGLayer = pGLF.OpenGraphicsLayer((IFeatureWorkspace)dataSet.Workspace, featClass.FeatureDataset, dataSet.Name) as IFDOGraphicsLayer;
                ((IFeatureLayer)pFDOGLayer).Name = layerName;
                return (IFeatureLayer)pFDOGLayer;
            }
            else 
            {
                IFeatureLayer feaLyr = new FeatureLayerClass();
                feaLyr.FeatureClass = featClass;
                feaLyr.Name = layerName;
                return feaLyr;
            }

        }

        public void ConvertFeatureClassToGDB(IWorkspace sourceWorkspace, IWorkspace targetWorkspace, string nameOfSourceFeatureClass, string nameOfTargetFeatureClass, IFeatureDataset feaDS)
        {
            //create source workspace name        
            IDataset sourceWorkspaceDataset = (IDataset)sourceWorkspace;
            IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;
            //create source dataset name        
            IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
            sourceDatasetName.WorkspaceName = sourceWorkspaceName;
            sourceDatasetName.Name = nameOfSourceFeatureClass;
            //create target workspace name        
            IDataset targetWorkspaceDataset = (IDataset)targetWorkspace;
            IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;
            //create target dataset name        
            IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
            targetDatasetName.WorkspaceName = targetWorkspaceName;
            targetDatasetName.Name = nameOfTargetFeatureClass;

            IFeatureDatasetName feaName = null;
            if (feaDS != null) 
            {
                feaName = (IFeatureDatasetName)feaDS.FullName;
            }


            //Open input Featureclass to get field definitions.        
            ESRI.ArcGIS.esriSystem.IName sourceName = (ESRI.ArcGIS.esriSystem.IName)sourceFeatureClassName;
            IFeatureClass sourceFeatureClass = (IFeatureClass)sourceName.Open();
            //Validate the field names because you are converting between different workspace types.        
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IFields targetFeatureClassFields;
            IFields sourceFeatureClassFields = sourceFeatureClass.Fields;
            IEnumFieldError enumFieldError;
            // Most importantly set the input and validate workspaces!        
            fieldChecker.InputWorkspace = sourceWorkspace;
            fieldChecker.ValidateWorkspace = targetWorkspace;
            fieldChecker.Validate(sourceFeatureClassFields, out enumFieldError, out targetFeatureClassFields);
            // Loop through the output fields to find the geomerty field        
            IField geometryField;
            for (int i = 0; i < targetFeatureClassFields.FieldCount; i++)
            {
                if (targetFeatureClassFields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                {
                    geometryField = targetFeatureClassFields.get_Field(i);
                    // Get the geometry field's geometry defenition                
                    IGeometryDef geometryDef = geometryField.GeometryDef;
                    //Give the geometry definition a spatial index grid count and grid size                
                    IGeometryDefEdit targetFCGeoDefEdit = (IGeometryDefEdit)geometryDef;
                    targetFCGeoDefEdit.GridCount_2 = 1;
                    targetFCGeoDefEdit.set_GridSize(0, 0);
                    //Allow ArcGIS to determine a valid grid size for the data loaded                
                    targetFCGeoDefEdit.SpatialReference_2 = geometryField.GeometryDef.SpatialReference;
                    // we want to convert all of the features                
                    IQueryFilter queryFilter = new QueryFilterClass();
                    queryFilter.WhereClause = "";
                    // Load the feature class                
                    IFeatureDataConverter fctofc = new FeatureDataConverterClass();
                    IEnumInvalidObject enumErrors = fctofc.ConvertFeatureClass(sourceFeatureClassName, queryFilter, feaName, targetFeatureClassName, geometryDef, targetFeatureClassFields, "", 1000, 0);
                    break;
                }
            }
        }

        public void ConvertMDBFeatureDatasetToSDE(IWorkspace sourceWorkspace, IWorkspace targetWorkspace, string nameOfSourceFeatureDataset, string nameOfTargetFeatureDataset)
        {
            if (sourceWorkspace == null || targetWorkspace == null)
            {
                return;
            }
            //创建源工作空间名      
            IDataset sourceWorkspaceDataset = (IDataset)sourceWorkspace;
            IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;
            //创建源数据集名
            IFeatureDatasetName sourceFeatureDatasetName =new FeatureDatasetNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureDatasetName;
            sourceDatasetName.WorkspaceName = sourceWorkspaceName;
            sourceDatasetName.Name = nameOfSourceFeatureDataset;
            //创建目标工作空间名
            IDataset targetWorkspaceDataset = (IDataset)targetWorkspace;
            IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;
            //创建目标数据集名
            IFeatureDatasetName targetFeatureDatasetName = new FeatureDatasetNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetFeatureDatasetName;
            targetDatasetName.WorkspaceName = targetWorkspaceName;
            targetDatasetName.Name = nameOfTargetFeatureDataset;
            //转换（复制）源数据集到目标数据集
            IFeatureDataConverter featureDataConverter = new FeatureDataConverterClass();
            featureDataConverter.ConvertFeatureDataset(sourceFeatureDatasetName, targetFeatureDatasetName, null, "", 1000, 0);

        }

        public IList<FCName> get_FeatureClassName() 
        {
            IList<FCName> names = new List<FCName>();
            if(this.m_workSpace!=null)
            {
                FCName fcname;
                IFeatureClass feaClass = null;
                IEnumDatasetName enumDatasetNameFC = m_workSpace.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
                IDatasetName datasetNameFC = enumDatasetNameFC.Next();
                try
               {
                    while (datasetNameFC != null)
                    {
                        feaClass = ((IFeatureWorkspace)m_workSpace).OpenFeatureClass(datasetNameFC.Name.ToString());
                        fcname.Name = datasetNameFC.Name.ToString();
                        fcname.ShapType = feaClass.ShapeType.ToString();
                        fcname.FeatureType = feaClass.FeatureType.ToString();
                        names.Add(fcname);
                        datasetNameFC = enumDatasetNameFC.Next();
                    }
                }
                catch (Exception ee)
                {
                }
            }
            return names;
        }


        public static IFeatureClass CreateFClassInPDB(IWorkspace accessworkspace, string feaDSname, string FCname, esriGeometryType esriGeometryType,ISpatialReference sprf)
        {
            try
            {
                IFeatureDataset featureDataset = ((IFeatureWorkspace)accessworkspace).OpenFeatureDataset(feaDSname);
                //IGeoDataset geoDataset = featureDataset as IGeoDataset;

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


                IFeatureClass fc = featureDataset.CreateFeatureClass(FCname, pFields, null, null, esriFeatureType.esriFTSimple, "Shape", "");
                return fc;
            }

            catch (Exception ee) 
            {
                MessageBox.Show(ee.Message.ToString());
                return null;
            }
   
        }

        public static IFeatureClass CreateFeatureClassInPGDB(IWorkspace2 workspace, IFeatureDataset featureDataset, string featureClassName, IFields fields, UID CLSID, UID CLSEXT, string strConfigKeyword, esriGeometryType esriGeometryType)
        {
            if (featureClassName == "") return null; // name was not passed in 
            ESRI.ArcGIS.Geodatabase.IFeatureClass featureClass;
            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace featureWorkspace = (ESRI.ArcGIS.Geodatabase.IFeatureWorkspace)workspace; // Explicit Cast
            if (workspace.get_NameExists(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass, featureClassName)) //feature class with that name already exists 
            {

                featureClass = featureWorkspace.OpenFeatureClass(featureClassName);
                return featureClass;
            }


            // assign the class id value if not assigned

            if (CLSID == null)
            {
                CLSID = new ESRI.ArcGIS.esriSystem.UIDClass();
                CLSID.Value = "esriGeoDatabase.Feature";
            }

            ESRI.ArcGIS.Geodatabase.IObjectClassDescription objectClassDescription = new ESRI.ArcGIS.Geodatabase.FeatureClassDescriptionClass();


            // if a fields collection is not passed in then supply our own

            if (fields == null)
            {

                // create the fields using the required fields method

                fields = objectClassDescription.RequiredFields;

                ESRI.ArcGIS.Geodatabase.IFieldsEdit fieldsEdit = (ESRI.ArcGIS.Geodatabase.IFieldsEdit)fields; // Explicit Cast
                ESRI.ArcGIS.Geodatabase.IField field = new ESRI.ArcGIS.Geodatabase.FieldClass();

                // create a user defined text field
                ESRI.ArcGIS.Geodatabase.IFieldEdit fieldEdit = (ESRI.ArcGIS.Geodatabase.IFieldEdit)field; // Explicit Cast

                // setup field properties
                fieldEdit.Name_2 = "SampleField";
                fieldEdit.Type_2 = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeString;
                fieldEdit.IsNullable_2 = true;
                fieldEdit.AliasName_2 = "Sample Field Column";
                fieldEdit.DefaultValue_2 = "test";
                fieldEdit.Editable_2 = true;
                fieldEdit.Length_2 = 100;

                // add field to field collection
                fieldsEdit.AddField(field);
                fields = (ESRI.ArcGIS.Geodatabase.IFields)fieldsEdit; // Explicit Cast
            }

            System.String strShapeField = "";

            // locate the shape field
            for (int j = 0; j < fields.FieldCount; j++)
            {
                if (fields.get_Field(j).Type == esriFieldType.esriFieldTypeGeometry)
                {
                    strShapeField = fields.get_Field(j).Name;
                    ((IGeometryDefEdit)fields.get_Field(j).GeometryDef).GeometryType_2 = esriGeometryType;
                }
            }

            // Use IFieldChecker to create a validated fields collection.
            ESRI.ArcGIS.Geodatabase.IFieldChecker fieldChecker = new ESRI.ArcGIS.Geodatabase.FieldCheckerClass();
            ESRI.ArcGIS.Geodatabase.IEnumFieldError enumFieldError = null;
            ESRI.ArcGIS.Geodatabase.IFields validatedFields = null;
            fieldChecker.ValidateWorkspace = (ESRI.ArcGIS.Geodatabase.IWorkspace)workspace;
            fieldChecker.Validate(fields, out enumFieldError, out validatedFields);
            // The enumFieldError enumerator can be inspected at this point to determine 
            // which fields were modified during validation.

            // finally create and return the feature class

            if (featureDataset == null)// if no feature dataset passed in, create at the workspace level
            {
                featureClass = featureWorkspace.CreateFeatureClass(featureClassName, validatedFields, CLSID, CLSEXT, ESRI.ArcGIS.Geodatabase.esriFeatureType.esriFTSimple, strShapeField, strConfigKeyword);
            }
            else
            {
                featureClass = featureDataset.CreateFeatureClass(featureClassName, validatedFields, CLSID, CLSEXT, ESRI.ArcGIS.Geodatabase.esriFeatureType.esriFTSimple, strShapeField, strConfigKeyword);
            }
            return featureClass;

        }

        public IFeatureClass CreateStandAloneAnnotationClass(IDataset dataset, string annoname)
        {
            #region 创建Anno图层
            IFeatureClass featureClass = null;
            try
            {
                //ILayer pLayer = m_pmap.get_Layer(0);
                //IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                //ILayer pSrcLayer = pLayer;
                //IFeatureClass featureClass = pFeatureLayer.FeatureClass;

                //IDataset dataset = (IDataset)featureClass;//cast for the feature workspace from the workspace
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)dataset.Workspace;
                IFeatureWorkspaceAnno featureWorkspaceAnno = (IFeatureWorkspaceAnno)dataset.Workspace;//set up the reference scale        
                ESRI.ArcGIS.Carto.IGraphicsLayerScale graphicLayerScale = new ESRI.ArcGIS.Carto.GraphicsLayerScaleClass();
                IGeoDataset geoDataset = (IGeoDataset)dataset;
                graphicLayerScale.Units = ESRI.ArcGIS.esriSystem.esriUnits.esriMeters;
                graphicLayerScale.ReferenceScale = 500;//set up symbol collection
                ESRI.ArcGIS.Display.ISymbolCollection symbolCollection = new ESRI.ArcGIS.Display.SymbolCollectionClass();
                #region "MakeText"
                ESRI.ArcGIS.Display.IFormattedTextSymbol myTextSymbol = new ESRI.ArcGIS.Display.TextSymbolClass();        //set the font for myTextSymbol        
                stdole.IFontDisp myFont = new stdole.StdFontClass() as stdole.IFontDisp;
                myFont.Name = "Courier New";
                myFont.Size = 9;
                myTextSymbol.Font = myFont;//set the Color for myTextSymbol to be Dark Red
                ESRI.ArcGIS.Display.IRgbColor rgbColor = new ESRI.ArcGIS.Display.RgbColorClass();
                rgbColor.Red = 150;
                rgbColor.Green = 0;
                rgbColor.Blue = 0;
                myTextSymbol.Color = (ESRI.ArcGIS.Display.IColor)rgbColor;//Set other properties for myTextSymbol
                myTextSymbol.Angle = 0;
                myTextSymbol.RightToLeft = false;
                myTextSymbol.VerticalAlignment = ESRI.ArcGIS.Display.esriTextVerticalAlignment.esriTVABaseline;
                myTextSymbol.HorizontalAlignment = ESRI.ArcGIS.Display.esriTextHorizontalAlignment.esriTHAFull;
                myTextSymbol.CharacterSpacing = 200;
                myTextSymbol.Case = ESRI.ArcGIS.Display.esriTextCase.esriTCNormal;
                #endregion
                symbolCollection.set_Symbol(0, (ESRI.ArcGIS.Display.ISymbol)myTextSymbol);//set up the annotation labeling properties including the expression
                ESRI.ArcGIS.Carto.IAnnotateLayerProperties annoProps = new ESRI.ArcGIS.Carto.LabelEngineLayerPropertiesClass();
                annoProps.FeatureLinked = false;//这里控制是不是关联的注记层
                annoProps.AddUnplacedToGraphicsContainer = false;
                annoProps.CreateUnplacedElements = true;
                annoProps.DisplayAnnotation = true;
                annoProps.UseOutput = true;
                ESRI.ArcGIS.Carto.ILabelEngineLayerProperties layerEngineLayerProps = (ESRI.ArcGIS.Carto.ILabelEngineLayerProperties)annoProps;
                ESRI.ArcGIS.Carto.IAnnotationExpressionEngine annoExpressionEngine = new ESRI.ArcGIS.Carto.AnnotationVBScriptEngineClass();
                layerEngineLayerProps.ExpressionParser = annoExpressionEngine;
                layerEngineLayerProps.Expression = "[RefName]";
                layerEngineLayerProps.IsExpressionSimple = true;
                layerEngineLayerProps.Offset = 0;
                layerEngineLayerProps.SymbolID = 0;
                layerEngineLayerProps.Symbol = myTextSymbol;
                ESRI.ArcGIS.Carto.IAnnotateLayerTransformationProperties annoLayerTransProp =
                    (ESRI.ArcGIS.Carto.IAnnotateLayerTransformationProperties)annoProps;
                annoLayerTransProp.ReferenceScale = graphicLayerScale.ReferenceScale;
                annoLayerTransProp.Units = graphicLayerScale.Units;
                annoLayerTransProp.ScaleRatio = 1;
                ESRI.ArcGIS.Carto.IAnnotateLayerPropertiesCollection annoPropsColl = new ESRI.ArcGIS.Carto.AnnotateLayerPropertiesCollectionClass();
                annoPropsColl.Add(annoProps);//use the AnnotationFeatureClassDescription to get the list of required
                //fields and the default name of the shape field
                IObjectClassDescription oCDesc = new ESRI.ArcGIS.Carto.AnnotationFeatureClassDescriptionClass();
                IFeatureClassDescription fCDesc = (IFeatureClassDescription)oCDesc;//create the new class   

                featureClass = featureWorkspaceAnno.CreateAnnotationClass(annoname, oCDesc.RequiredFields, oCDesc.InstanceCLSID, oCDesc.ClassExtensionCLSID,
                    fCDesc.ShapeFieldName, "", (IFeatureDataset)dataset, null, annoPropsColl, graphicLayerScale, symbolCollection, true);
                //给新建的图层添加子层
                ISubtypes subtypes = (ISubtypes)featureClass;
                subtypes.SubtypeFieldName = "AnnotationClassID";
                subtypes.AddSubtype(1, "GCD");
                subtypes.AddSubtype(2, "DLDW");
                subtypes.AddSubtype(3, "JMD");
                subtypes.AddSubtype(4, "SXSS");
                subtypes.AddSubtype(5, "DLSS");
                subtypes.AddSubtype(6, "ZBTZ");
                subtypes.AddSubtype(7, "TK");
                subtypes.DefaultSubtypeCode = 1;
                return featureClass;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return featureClass;
            }
            #endregion
        }

        public IFeatureClass CreateStandAloneAnnotationClass(IWorkspace ws, string annoname)
        {
            #region 创建Anno图层
            IFeatureClass featureClass = null;
            try
            {
                //ILayer pLayer = m_pmap.get_Layer(0);
                //IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                //ILayer pSrcLayer = pLayer;
                //IFeatureClass featureClass = pFeatureLayer.FeatureClass;

                //IDataset dataset = (IDataset)featureClass;//cast for the feature workspace from the workspace
                //IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)dataset.Workspace;
                IFeatureWorkspaceAnno featureWorkspaceAnno = (IFeatureWorkspaceAnno)ws;//set up the reference scale        
                ESRI.ArcGIS.Carto.IGraphicsLayerScale graphicLayerScale = new ESRI.ArcGIS.Carto.GraphicsLayerScaleClass();
                //IGeoDataset geoDataset = (IGeoDataset)dataset;
                graphicLayerScale.Units = ESRI.ArcGIS.esriSystem.esriUnits.esriMeters;
                graphicLayerScale.ReferenceScale = 500;//set up symbol collection
                ESRI.ArcGIS.Display.ISymbolCollection symbolCollection = new ESRI.ArcGIS.Display.SymbolCollectionClass();
                #region "MakeText"
                ESRI.ArcGIS.Display.IFormattedTextSymbol myTextSymbol = new ESRI.ArcGIS.Display.TextSymbolClass();        //set the font for myTextSymbol        
                stdole.IFontDisp myFont = new stdole.StdFontClass() as stdole.IFontDisp;
                myFont.Name = "Courier New";
                myFont.Size = 9;
                myTextSymbol.Font = myFont;//set the Color for myTextSymbol to be Dark Red
                ESRI.ArcGIS.Display.IRgbColor rgbColor = new ESRI.ArcGIS.Display.RgbColorClass();
                rgbColor.Red = 150;
                rgbColor.Green = 0;
                rgbColor.Blue = 0;
                myTextSymbol.Color = (ESRI.ArcGIS.Display.IColor)rgbColor;//Set other properties for myTextSymbol
                myTextSymbol.Angle = 0;
                myTextSymbol.RightToLeft = false;
                myTextSymbol.VerticalAlignment = ESRI.ArcGIS.Display.esriTextVerticalAlignment.esriTVABaseline;
                myTextSymbol.HorizontalAlignment = ESRI.ArcGIS.Display.esriTextHorizontalAlignment.esriTHAFull;
                myTextSymbol.CharacterSpacing = 200;
                myTextSymbol.Case = ESRI.ArcGIS.Display.esriTextCase.esriTCNormal;
                #endregion
                symbolCollection.set_Symbol(0, (ESRI.ArcGIS.Display.ISymbol)myTextSymbol);//set up the annotation labeling properties including the expression
                ESRI.ArcGIS.Carto.IAnnotateLayerProperties annoProps = new ESRI.ArcGIS.Carto.LabelEngineLayerPropertiesClass();
                annoProps.FeatureLinked = false;//这里控制是不是关联的注记层
                annoProps.AddUnplacedToGraphicsContainer = false;
                annoProps.CreateUnplacedElements = true;
                annoProps.DisplayAnnotation = true;
                annoProps.UseOutput = true;
                ESRI.ArcGIS.Carto.ILabelEngineLayerProperties layerEngineLayerProps = (ESRI.ArcGIS.Carto.ILabelEngineLayerProperties)annoProps;
                ESRI.ArcGIS.Carto.IAnnotationExpressionEngine annoExpressionEngine = new ESRI.ArcGIS.Carto.AnnotationVBScriptEngineClass();
                layerEngineLayerProps.ExpressionParser = annoExpressionEngine;
                layerEngineLayerProps.Expression = "[RefName]";
                layerEngineLayerProps.IsExpressionSimple = true;
                layerEngineLayerProps.Offset = 0;
                layerEngineLayerProps.SymbolID = 0;
                layerEngineLayerProps.Symbol = myTextSymbol;
                ESRI.ArcGIS.Carto.IAnnotateLayerTransformationProperties annoLayerTransProp =
                    (ESRI.ArcGIS.Carto.IAnnotateLayerTransformationProperties)annoProps;
                annoLayerTransProp.ReferenceScale = graphicLayerScale.ReferenceScale;
                annoLayerTransProp.Units = graphicLayerScale.Units;
                annoLayerTransProp.ScaleRatio = 1;
                ESRI.ArcGIS.Carto.IAnnotateLayerPropertiesCollection annoPropsColl = new ESRI.ArcGIS.Carto.AnnotateLayerPropertiesCollectionClass();
                annoPropsColl.Add(annoProps);//use the AnnotationFeatureClassDescription to get the list of required
                //fields and the default name of the shape field
                IObjectClassDescription oCDesc = new ESRI.ArcGIS.Carto.AnnotationFeatureClassDescriptionClass();
                IFeatureClassDescription fCDesc = (IFeatureClassDescription)oCDesc;//create the new class   

                featureClass = featureWorkspaceAnno.CreateAnnotationClass(annoname, oCDesc.RequiredFields, oCDesc.InstanceCLSID, oCDesc.ClassExtensionCLSID,
                    fCDesc.ShapeFieldName, "", null, null, annoPropsColl, graphicLayerScale, symbolCollection, true);
                //给新建的图层添加子层
                ISubtypes subtypes = (ISubtypes)featureClass;
                subtypes.SubtypeFieldName = "AnnotationClassID";
                subtypes.AddSubtype(1, "GCD");
                subtypes.AddSubtype(2, "DLDW");
                subtypes.AddSubtype(3, "JMD");
                subtypes.AddSubtype(4, "SXSS");
                subtypes.AddSubtype(5, "DLSS");
                subtypes.AddSubtype(6, "ZBTZ");
                subtypes.AddSubtype(7, "TK");
                subtypes.DefaultSubtypeCode = 1;
                return featureClass;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return featureClass;
            }
            #endregion
        }

        public void AppendAnnoElements(ILayer pSrcLayer, IFeatureClass pTargetFC,IActiveView activeview)
        {
            //首先从cad的注记层得到一个注记，然后得到他的Annotation，再赋给新建的要素
            //IQueryFilter queryfilter = new QueryFilterClass();
            //queryfilter.WhereClause = "Layer = '" + filter + "'";            
            try
            {
                IFeatureLayer pSrcFLayer = pSrcLayer as IFeatureLayer;
                IFeatureClass pSrcFC = pSrcFLayer.FeatureClass;
                IFeature cadFeat;
                IElement featElement;
                IDataset pDataset;
                pDataset = pTargetFC as IDataset;
                IFDOGraphicsLayer pFDOGLayer;
                IFDOGraphicsLayerFactory pGLF = new FDOGraphicsLayerFactoryClass();
                ICoverageAnnotationLayer pCadAnnoLayer;
                pCadAnnoLayer = pSrcLayer as ICoverageAnnotationLayer;
                IWorkspace pWorkspace = pDataset.Workspace as IWorkspace;
                IWorkspaceEdit pWorkspaceEdit = pWorkspace as IWorkspaceEdit;

                IScreenDisplay screendis = activeview.ScreenDisplay;
                pFDOGLayer = pGLF.OpenGraphicsLayer((IFeatureWorkspace)pDataset.Workspace, pTargetFC.FeatureDataset, pDataset.Name) as IFDOGraphicsLayer;
                pCadAnnoLayer.StartGeneratingGraphics(null, screendis, false);
                pCadAnnoLayer.NextFeatureAndGraphic(out cadFeat, out featElement);
                //IFeature newFeature;
                IFeature cadFeature;
                IAnnotationFeature pAnnoFeature;
                IPolygon poly = new PolygonClass();
                //IEnvelope box;
                pWorkspaceEdit.StartEditing(false);
                pWorkspaceEdit.StartEditOperation();
                //int index;
                //pFDOGLayer.BeginAddElements();
                int count = 0;
                //pstepro.StepValue = 1;
                IFeatureBuffer featbuffer = pTargetFC.CreateFeatureBuffer();
                IFeatureCursor featCur = pTargetFC.Insert(true);

                while (cadFeat != null && featElement != null)
                {
                    //pFDOGLayer.DoAddFeature(cadFeat, featElement, 0);
                    //判断分在那一个层
                    count++;
                    //pstepro.Position = count;//进度条增加
                    cadFeature = pSrcFC.GetFeature(cadFeat.OID);
                    //if (cadFeature.get_Value(cadFeature.Fields.FindField("Layer")).ToString() != filter)
                    //{
                    //    pCadAnnoLayer.NextFeatureAndGraphic(out cadFeat, out featElement);
                    //    continue;
                    //}
                    //newFeature = pTargetFC.CreateFeature();
                    poly = cadFeat.ShapeCopy as IPolygon;
                    featbuffer.Shape = poly as IGeometry;
                    pAnnoFeature = featbuffer as IAnnotationFeature;
                    pAnnoFeature.Annotation = featElement;


                    int index1 = getLayerIndex(cadFeature.get_Value(cadFeature.Fields.FindField("Layer")).ToString());

                    featbuffer.set_Value(4, index1);
                    featCur.InsertFeature(featbuffer);
                    //newFeature.Store();
                    ///////////////
                    pCadAnnoLayer.NextFeatureAndGraphic(out cadFeat, out featElement);
                }
                featCur.Flush();
                //pFDOGLayer.EndAddElements();
                pWorkspaceEdit.StopEditOperation();
                pWorkspaceEdit.StopEditing(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AppendAnnoElementsSDE(IFeatureClass pSrcFClass, IFeatureClass pTargetFClass)
        {
            try
            {
                IFields pOFlds;
                IFields pIFlds;
                pOFlds = pTargetFClass.Fields;
                pIFlds = pSrcFClass.Fields;
                IField pFld;
                int[] lSrcFlds;
                int[] lTrgFlds;
                IDataset pDataset;
                int lFld, lExFld, i;
                lExFld = 0;
                for (lFld = 0; lFld < pIFlds.FieldCount; lFld++)
                {
                    pFld = pIFlds.get_Field(lFld);
                    if (pFld.Type != esriFieldType.esriFieldTypeOID && pFld.Type != esriFieldType.esriFieldTypeGeometry && pFld.Name.ToUpper() != "ELEMENT"
                        && pFld.Name.ToUpper() != "ANNOTATIONCLASSID" && pFld.Name.ToUpper() != "ZORDER" && pFld.Editable)
                    {
                        lExFld++;
                    }
                }
                lSrcFlds = new int[lExFld];
                lTrgFlds = new int[lExFld];
                i = 0;
                for (lFld = 0; lFld < pIFlds.FieldCount; lFld++)
                {
                    pFld = pIFlds.get_Field(lFld);
                    if (pFld.Type != esriFieldType.esriFieldTypeOID && pFld.Type != esriFieldType.esriFieldTypeGeometry && pFld.Name.ToUpper() != "ELEMENT"
                        && pFld.Name.ToUpper() != "ANNOTATIONCLASSID" && pFld.Name.ToUpper() != "ZORDER" && pFld.Editable)
                    {
                        lSrcFlds[i] = lFld;
                        lTrgFlds[i] = pOFlds.FindField(pFld.Name);
                        i++;
                    }
                }
                IFeatureCursor pICursor;
                pICursor = pSrcFClass.Search(null, false);
                IFeature pIFeat;
                pIFeat = pICursor.NextFeature();
                IFDOGraphicsLayerFactory pGLF;
                pGLF = new FDOGraphicsLayerFactoryClass();

                pDataset = pTargetFClass as IDataset;
                IFDOGraphicsLayer pFDOGLayer;
                pFDOGLayer = pGLF.OpenGraphicsLayer((IFeatureWorkspace)pDataset.Workspace, pTargetFClass.FeatureDataset, pDataset.Name) as IFDOGraphicsLayer;
                IFDOAttributeConversion pFDOACon;

                pFDOACon = pFDOGLayer as IFDOAttributeConversion; ;

                pFDOGLayer.BeginAddElements();
                pFDOACon.SetupAttributeConversion2(lExFld, lSrcFlds, lTrgFlds);
                IAnnotationFeature pAnnoFeature;
                IClone pAClone;
                IElement pGSElement;
                while (pIFeat != null)
                {
                    pAnnoFeature = pIFeat as IAnnotationFeature;
                    pAClone = pAnnoFeature.Annotation as IClone;
                    pGSElement = pAClone.Clone() as IElement;
                    pFDOGLayer.DoAddFeature(pIFeat, pGSElement, 0);
                    pIFeat = pICursor.NextFeature();
                }
                pFDOGLayer.EndAddElements();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int getLayerIndex(string Layername)
        {
            int layerindex = 0;
            try
            {
                switch (Layername)
                {
                    case "GCD":
                        layerindex = 1;
                        break;
                    case "DLDW":
                        layerindex = 2;
                        break;
                    case "JMD":
                        layerindex = 3;
                        break;
                    case "SXSS":
                        layerindex = 4;
                        break;
                    case "DLSS":
                        layerindex = 5;
                        break;
                    case "ZBTZ":
                        layerindex = 6;
                        break;
                    case "TK":
                        layerindex = 7;
                        break;
                }
                return layerindex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return layerindex;
            }
        }

        public void InsertMataData(string name,string shapetype,string type,string table) 
        {
            if (this.m_oraCmd != null&&table=="1") 
            {
                string sqlText = "insert into VECTORMETADATA (VECTORNAME,TYPE,Geometry) values('" + name + "','" + type + "','" + shapetype + "')";
                m_oraCmd.CommandText = sqlText;
                m_oraCmd.ExecuteNonQuery();
            }
            if (this.m_oraCmd != null && table == "2") 
            {
                string sqlText = "insert into IMAGEMETADATA (IMAGENAME,SATELITE,[TIME]) values('" + name + "','" + shapetype + "','" + type + "')";
                m_oraCmd.CommandText = sqlText;
                m_oraCmd.ExecuteNonQuery();
            }
        }

        public void ExportToSHP(string name,string outputPath) 
        {
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            IWorkspace pWorkspace = (IWorkspace)pWorkspaceFactory.OpenFromFile(outputPath, 0);
            ConvertFeatureClassToGDB(this.m_workSpace, pWorkspace, name, name, null);
        }

        public void ExportToMDB(string name, string outputPath) 
        {
            IWorkspaceFactory wsFct = new AccessWorkspaceFactoryClass();
            IWorkspace pWorkspace = (IWorkspace)wsFct.OpenFromFile(outputPath, 0);
            ConvertFeatureClassToGDB(this.m_workSpace, pWorkspace, name, name, null);
        }

        public void ExportRaster(IRasterDataset rasterDataset, string path, string sFormat, string sName)
        {
            IWorkspaceFactory wsF = new RasterWorkspaceFactoryClass();
            IWorkspace ws = wsF.OpenFromFile(path,0);

            ISaveAs saveAs = rasterDataset as ISaveAs;
            saveAs.SaveAs(sName, ws, sFormat);
        }

        public bool CheckDataSetName(string name,string type) 
        {
            bool result = false;
            string sqlText="";
            if (type == "矢量") 
            {
                sqlText = "select name from SPATIALDATASET where TYPE='矢量'";
            }
            if (type == "影像") 
            {
                sqlText = "select name from SPATIALDATASET where TYPE='影像'";
            }
            m_oraCmd.CommandText = sqlText;
            OleDbDataReader dr = m_oraCmd.ExecuteReader();
            while (dr.Read())
            {
                if (dr.GetValue(0).ToString() == name)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public bool CheckSheetName(string name,string table) 
        {
            string sqlText;
            bool result = false;
            if(table=="1")
            {
                sqlText = "select * from IMAGEMETADATA where IMAGENAME='" + name + "'";
            }
            else
            {
                sqlText = "select * from VECTORMETADATA where VECTORNAME='" + name + "'";
            }

            m_oraCmd.CommandText = sqlText;
            OleDbDataReader dr = m_oraCmd.ExecuteReader();
            if (dr.Read()) 
            {
                result = true;
            }
            dr.Close();
            return result;
        }

        public IRasterCatalog CreateRasterCat(string name, string sdePath) 
        {
            ESRI.ArcGIS.Geoprocessor.Geoprocessor geoProcessor = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
            CreateRasterCatalog createRasterCatalog = new CreateRasterCatalog();
            IGPCoordinateSystem rSR = new GPCoordinateSystemClass();
            IGPSpatialReference gSR = new GPSpatialReferenceClass();
            rSR.SpatialReference = new UnknownCoordinateSystemClass();
            gSR.SpatialReference = new UnknownCoordinateSystemClass();

            createRasterCatalog.out_path = sdePath;
            createRasterCatalog.out_name = name;
            createRasterCatalog.raster_spatial_reference = (object)rSR;
            createRasterCatalog.spatial_reference = (object)gSR;
            geoProcessor.Execute(createRasterCatalog, null);
            IRasterCatalog rastercat = ((IRasterWorkspaceEx)this.m_workSpace).OpenRasterCatalog(name);
            return rastercat;
        }

        private IRasterDataset CreateRasterDataset(ref IRasterDataset pRasterDataset, string name, ref IWorkspace pWorkspace, ISpatialReference pSpatialReference)
        {
            IRasterWorkspaceEx pRasterWorkspaceEx = (IRasterWorkspaceEx)pWorkspace;
            IRasterDataset newRasterDataset = null;
            IRaster pRaster = pRasterDataset.CreateDefaultRaster();
            IRasterBandCollection pRasterBandCollection = (IRasterBandCollection)pRaster;
            int numbands = pRasterBandCollection.Count;
            IRasterProps pRasterProps = (IRasterProps)pRaster;
            rstPixelType pPixelType = pRasterProps.PixelType;
            IRasterStorageDef pRasterStorageDef = null;

            if (pRasterStorageDef == null)
            {
                pRasterStorageDef = new RasterStorageDefClass();
                pRasterStorageDef.CompressionType = esriRasterCompressionType.esriRasterCompressionLZ77;
                pRasterStorageDef.PyramidLevel = 2;
                pRasterStorageDef.PyramidResampleType = rstResamplingTypes.RSP_BilinearInterpolation;
                pRasterStorageDef.TileHeight = 128;
                pRasterStorageDef.TileWidth = 128;
            }

            IRasterDef pRasterDef = new RasterDefClass();
            pRasterDef.SpatialReference = pSpatialReference;
            pRasterDef.IsRasterDataset = true;
            pRasterDef.Description = "rasterdataset";
            try
            {
                newRasterDataset = pRasterWorkspaceEx.CreateRasterDataset(name, numbands, pPixelType, pRasterStorageDef, "", pRasterDef, this.createGeometryDef(pSpatialReference));

               // newRasterDataset = pRasterWorkspaceEx.CreateRasterDataset(name, "IMAGINE Image", pOrigin, 100, 100, 30, 30, 1, rstPixelType.PT_LONG, pSpatialReference, true);
                return newRasterDataset;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void MosaicRasterToGDBRaster(IRasterDataset pRasterFile, IRasterDatasetEdit pGDBRasterDs)
        {
            try
            {
                IRaster pRaster;
                IRasterDataset2 pRasterDs = pRasterFile as IRasterDataset2;
                //IDataset pDs;

                pRaster = pRasterDs.CreateFullRaster();//CreateFullRaster()方法只有IRasterDataset2有
                pGDBRasterDs.Mosaic(pRaster, 0.5); //no resample

                pRasterDs = null;
                pRaster = null;
                //pDs = null;
            }
            catch (Exception e)
            {
                if (e is StackOverflowException ||
                    e is OutOfMemoryException)
                    throw;
            }
        }

        private IGeometryDef createGeometryDef(ISpatialReference spatialReference)
        {
            // Create GeometryDef
            IGeometryDefEdit geometryDefEdit = new GeometryDefClass();

            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            geometryDefEdit.AvgNumPoints_2 = 4;
            //设置空间索引的层数
            geometryDefEdit.GridCount_2 = 1;
            //geometryDefEdit.set_GridSize(0, 1000);

            // Set unknown spatial reference is not set
            if (spatialReference == null)
                spatialReference = new UnknownCoordinateSystemClass();

            //设置空间参考
            geometryDefEdit.SpatialReference_2 = spatialReference;

            return (IGeometryDef)geometryDefEdit;
        }

        public void ImportRas(IRasterDataset rasDS,string name) 
        {
            IRasterDataset rasdsSDE = this.CreateRasterDataset(ref rasDS, name, ref this.m_workSpace, this.GetSRFromFeatureDataset((IDataset)rasDS));
            if (rasdsSDE != null) 
            {
                this.MosaicRasterToGDBRaster(rasDS, (IRasterDatasetEdit)rasdsSDE);
            }
           
        }

        private void AddSimpleField(IFieldsEdit fieldsEdit,esriFieldType type,string name,string aliasName) 
        {
            IField field = new FieldClass();
            IFieldEdit fieldEdit = field as IFieldEdit;
            fieldEdit.Type_2 = type;
            fieldEdit.Name_2 = name;
            fieldEdit.AliasName_2 = aliasName;
            fieldsEdit.AddField(field);
        }

        public ISpatialReference GetSRFromFeatureDataset(IDataset pDataset)
        {
            IGeoDataset pGeoDataset;
            if (pDataset is IFeatureDataset)
            {
                pGeoDataset = (IFeatureDataset)pDataset as IGeoDataset;
                return (pGeoDataset.SpatialReference);
            }
            else if (pDataset is IRasterDataset)
            {
                pGeoDataset = (IRasterDataset)pDataset as IGeoDataset;
                return (pGeoDataset.SpatialReference);
            }
            else
            {
                //MessageBox.Show("wrong");
                return null;
            }
        }
    }
}
