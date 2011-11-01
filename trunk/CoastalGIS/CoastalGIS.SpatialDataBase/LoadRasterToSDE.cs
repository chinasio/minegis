using System;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;


namespace CoastalGIS.SpatialDataBase
{
    /// <summary>
    /// publicFunction ��ժҪ˵����
    /// </summary>
    public class LoadRasterToSDE
    {
        public LoadRasterToSDE()
        {
            //
            // TODO: �ڴ˴���ӹ��캯���߼�
            //
        }
        //Open RasterDataset by pathName and fileName
        public IRasterDataset openRasterDataset(string pathName, string fileName)
        {
            IWorkspaceFactory workspaceFactory;
            IRasterWorkspace rasterWorkspace;
            IRasterDataset rasterDataset = null;

            workspaceFactory = new RasterWorkspaceFactoryClass();

            //ע�����Ľ�׳��һ��Ҫ��·������ǰ�ж�
            if (workspaceFactory.IsWorkspace(pathName) == true)
            {
                rasterWorkspace = workspaceFactory.OpenFromFile(pathName, 0) as IRasterWorkspace;
                rasterDataset = rasterWorkspace.OpenRasterDataset(fileName);
            }
            return rasterDataset;
        }

        //Open Personal Geodatabase RasterCatalog
        //   ESRI.ArcGIS.Geodatabase, ESRI.ArcGIS.DataSourcesGDB
        public IRasterCatalog OpenPGDBRasterCatalog(string pathName, string rasterCatalogName)
        {
            // Open personal geodatabase raster catalog with the given name
            // pathName is the PGDB path (location of the personal geodatabase)
            // rasterCatalogName is the name of the raster catalog to be opened

            //Open the Access workspace
            IWorkspaceFactory2 workspaceFactory = new AccessWorkspaceFactoryClass();

            IRasterWorkspaceEx rasterWorkspaceEx = workspaceFactory.OpenFromFile(pathName, 0) as IRasterWorkspaceEx;

            //Open the PGDB raster Catalog
            IRasterCatalog rasterCatalog = null;
            rasterCatalog = rasterWorkspaceEx.OpenRasterCatalog(rasterCatalogName);

            return rasterCatalog;
        }


        //Open SDE RasterCatalog
        // Libraries needed to run the code:
        //   ESRI.ArcGIS.esriSystem, ESRI.ArcGIS.Geodatabase, and ESRI.ArcGIS.DataSourcesGDB
        public IRasterCatalog OpenSDERasCata(string server, string instance, string database, string user,
            string password, string rasterCatalogName, string version)
        {

            // Open an ArcSDE raster Catalog with the given name
            // server, instance, database, user, password, version are database connection info
            // rasterCatalogName is the name of the raster Catalog

            //Open the ArcSDE workspace
            IPropertySet propertySet = new PropertySetClass();

            propertySet.SetProperty("server", server);
            propertySet.SetProperty("instance", instance);
            propertySet.SetProperty("database", database);
            propertySet.SetProperty("user", user);
            propertySet.SetProperty("password", password);
            propertySet.SetProperty("version", version);

            // cocreate the workspace factory
            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();

            // Open the raster workspace using the previously defined porperty set
            // and QI to the desired IRasterWorkspaceEx interface to access the existing catalog
            IRasterWorkspaceEx rasterWorkspaceEx = workspaceFactory.Open(propertySet, 0) as IRasterWorkspaceEx;

            //Open the ArcSDE raster Catalog
            IRasterCatalog rasterCatalog = null;
            rasterCatalog = rasterWorkspaceEx.OpenRasterCatalog(rasterCatalogName);

            return rasterCatalog;
        }

        //Open SDE RasterCatalog2
        // Libraries needed to run the code:
        //   ESRI.ArcGIS.esriSystem, ESRI.ArcGIS.Geodatabase, and ESRI.ArcGIS.DataSourcesGDB
        public IRasterCatalog OpenSDERasCata2(IWorkspace pWorkspace, string rasterCatalogName)
        {
            // Open the raster workspace using the previously defined porperty set
            // and QI to the desired IRasterWorkspaceEx interface to access the existing catalog
            IRasterWorkspaceEx rasterWorkspaceEx = pWorkspace as IRasterWorkspaceEx;

            //Open the ArcSDE raster Catalog
            IRasterCatalog rasterCatalog = null;
            rasterCatalog = rasterWorkspaceEx.OpenRasterCatalog(rasterCatalogName);

            return rasterCatalog;
        }



        //RasterDataset to RasterCatalog
        public void RasDSToCatalog(IRasterDataset pRasterDs, IFeatureClass pCatalog)
        {

            //IRasterCatalogItem pCatalogFeature;

            IFeatureCursor pCursor;
            IFeatureBuffer pRow;

            pCursor = pCatalog.Insert(false);
            IRasterCatalog pCat = pCatalog as IRasterCatalog;

            IDataset pDs;

            // loop through all the datasets and load
            if (pRasterDs != null)
            {
                pDs = pRasterDs as IDataset;
                pRow = pCatalog.CreateFeatureBuffer();
                pRow.set_Value(pCat.RasterFieldIndex, createRasterValue(pRasterDs));
                /*try 
                {
                    pCursor.InsertFeature(pRow);
                }
                catch
                {

                }*/
                pCursor.InsertFeature(pRow);
            }
            //pCursor.Flush();
            pRasterDs = null;
        }
        //
        //		//Create a RasterValue from IRasterDataset
        //		//�ڴ˴�Ӧ������Ӱ�����ݵĴ洢����
        //		public IRasterValue createRasterValue(IRasterDataset pRasterDs)
        //		{
        //			IRasterValue pRasterVal;
        //			IRasterStorageDef pRasStoreDef;
        //			
        //			// ++ set storage parameter
        //			pRasStoreDef = new RasterStorageDefClass();//createRasterStorageDef
        //  
        //			pRasStoreDef.CompressionType = esriRasterSdeCompressionTypeEnum.esriRasterSdeCompressionTypeRunLength;
        //			pRasStoreDef.PyramidResampleType = rstResamplingTypes.RSP_CubicConvolution;
        //			pRasStoreDef.PyramidLevel = -1;
        //
        //			// ++ set raster value to raster field
        //			pRasterVal = new RasterValueClass();
        //			pRasterVal.RasterDataset = pRasterDs;
        //			pRasterVal.RasterStorageDef = pRasStoreDef;
        //            
        //			return(pRasterVal);
        //  	
        //		}

        //Delete a Raster in RasterCatalog by it's name
        public void DelRasterInCatalog(IFeatureClass pCatalog, string RasterDSName)
        {
            //1.����������featureClass ���ҵ�feature
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = "NAME ='" + RasterDSName + "'";

            IFeatureCursor pFeatCur = pCatalog.Search(pQueryFilter, false);

            int i = pFeatCur.FindField("Name");

            IFeature pFeat = pFeatCur.NextFeature();

            pFeat.Delete();
        }


        //����pCatalog�������Լ�Name�ֶλ�ȡIRasterDataset
        //ע��ֻ��ɾ����һ����������������
        public IRasterDataset GetRasterDatasetbyName(IFeatureClass pCatalog, string RasterDSName)
        {
            //1.����������featureClass ���ҵ�feature
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = "NAME ='" + RasterDSName + "'";

            IFeatureCursor pFeatCur = pCatalog.Search(pQueryFilter, false);

            int i = pFeatCur.FindField("Name");

            IFeature pFeat = pFeatCur.NextFeature();

            //2.�ҵ�Raster�ֶ�,�������е�ֵ����IRasterDataset
            IRasterCatalogItem pRasCatItem = pFeat as IRasterCatalogItem;
            return (pRasCatItem.RasterDataset);

        }

        //RasterDataset to RasterCatalog
        //add data to the featureclass
        public void RasDSToCatalogWithTime(IRasterDataset pRasterDs, IFeatureClass pCatalog, string DataField, string strData)
        {

            //IRasterCatalogItem pCatalogFeature;

            IFeatureCursor pCursor;
            IFeatureBuffer pRow;

            pCursor = pCatalog.Insert(false);
            IRasterCatalog pCat = pCatalog as IRasterCatalog;

            IDataset pDs;

            // loop through all the datasets and load
            if (pRasterDs != null)
            {
                pDs = pRasterDs as IDataset;
                pRow = pCatalog.CreateFeatureBuffer();
                pRow.set_Value(pCat.RasterFieldIndex, createRasterValue(pRasterDs));
                pRow.set_Value(pCatalog.FindField(DataField), Convert.ToDateTime(strData));
                pCursor.InsertFeature(pRow);
            }

            pRasterDs = null;
        }
        public void RasDSToCatalogWithTime(IRasterDataset pRasterDs, IFeatureClass pCatalog, string DataField, string strData, IRasterStorageDef pRasStoreDef)
        {

            //IRasterCatalogItem pCatalogFeature;

            IFeatureCursor pCursor;
            IFeatureBuffer pRow;

            pCursor = pCatalog.Insert(false);
            IRasterCatalog pCat = pCatalog as IRasterCatalog;

            IDataset pDs;

            // loop through all the datasets and load
            if (pRasterDs != null)
            {
                pDs = pRasterDs as IDataset;
                pRow = pCatalog.CreateFeatureBuffer();
                pRow.set_Value(pCat.RasterFieldIndex, createRasterValue(pRasterDs, pRasStoreDef));
                pRow.set_Value(pCatalog.FindField(DataField), Convert.ToDateTime(strData));
                pCursor.InsertFeature(pRow);
            }

            pRasterDs = null;
        }
        public IRasterDataset GetRasterDatasetInCatalog(IFeatureClass pCatalog, string RasterDSName)
        {
            //1.����������featureClass ���ҵ�feature
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = "NAME ='" + RasterDSName + "'";

            IFeatureCursor pFeatCur = pCatalog.Search(pQueryFilter, false);

            int i = pFeatCur.FindField("Name");

            IFeature pFeat = pFeatCur.NextFeature();

            //2.�ҵ�Raster�ֶ�,�������е�ֵ����IRasterDataset
            IRasterCatalogItem pRasCatItem = pFeat as IRasterCatalogItem;
            return (pRasCatItem.RasterDataset);

        }
        public void ShowAllNameInCata(IFeatureClass pCata, System.Windows.Forms.ComboBox cobBox)
        {
        }

        //++++++++GeoDatabase Part++++++
        public IFeatureClass openFeatureClass(string mdbName, string featName)
        {
            //Open the AccessWorkSpace by the given name which including extention(.mdb)
            IWorkspace ws = null;
            IWorkspaceFactory wsf = new AccessWorkspaceFactoryClass();
            ws = wsf.OpenFromFile(mdbName, 0);
            IFeatureWorkspace pFeatWorkspace = ws as IFeatureWorkspace;
            IFeatureClass pFeatCls = pFeatWorkspace.OpenFeatureClass(featName);
            return pFeatCls;
        }
        //#######################���º��������ʹ��########################

        //**************************Load Raster Begin****************************		

        //������ţ�RasterLoad-01
        //��������DirToMosaic 
        //�������ܣ����ļ����е�Raster�ļ��ϲ����µ�Raster�ļ���ȥ
        //������sDir��դ���ļ���
        //      pDataset�����ϲ���RasterDataset
        //��ע��pDataset����ʹGeodataset�еĲ�����������������ú�������
        public void DirToMosaic(string sDir, IRasterDataset pDataset)
        {
            //load all raster datasets in the input directory to the raster dataset
            //the geodatabase raster dataset has to exist, 
            //if not create it first with proper storage parameters.
            IWorkspaceFactory pWsFact = new RasterWorkspaceFactoryClass(); ;
            IWorkspace pFileWs = pWsFact.OpenFromFile(sDir, 0);
            
            IRasterDatasetEdit pSDEDs = pDataset as IRasterDatasetEdit;

            IRasterDataset2 pRasterDs;

            // load raster datasets from the dir
            IEnumDataset pEunmDatasets = pFileWs.get_Datasets(esriDatasetType.esriDTRasterDataset);
            pEunmDatasets.Reset();

            pRasterDs = pEunmDatasets.Next() as IRasterDataset2;
            while (!(pRasterDs == null))
            {
                //!!!pRasterDs.CompleteName;
                pSDEDs.Mosaic(pRasterDs.CreateFullRaster(), 0.5);
                //!!!ע��IRasterDataset2::CreateFullRaster()�������Լ�ʵ�ִ�RasterDataset��Raster��ת��
                pRasterDs = pEunmDatasets.Next() as IRasterDataset2;
            }

            //cleanup

            pEunmDatasets = null;
            pRasterDs = null;
            pWsFact = null;
            pSDEDs = null;
            pFileWs = null;
        }
        //������ţ�Raster-07
        //������:MosaicRasterToGDBRaster
        //��������:Raster�ļ�Mosaic��SDE Raster�ļ���
        //����:
        public void MosaicRasterToGDBRaster(IRasterDataset pRasterFile, IRasterDatasetEdit pGDBRasterDs)
        {
            try
            {
                IRaster pRaster;
                IRasterDataset2 pRasterDs = pRasterFile as IRasterDataset2;
                //IDataset pDs;

                pRaster = pRasterDs.CreateFullRaster();//CreateFullRaster()����ֻ��IRasterDataset2��
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

        //�����ţ�
        //��������LoadRasterToCatalogDatabase
        //�������ܣ�Raster����Database
        //������pWorkspace�����ݿ⣬pWorkspace������Access ����SDE
        //		strCata��Catalog����
        //		pRasterDs����Ҫ����Rasterdataset
        //��ע��insert a raster dataset with given path and file name to an existing raster catalog
        public void LoadRasterToCatalogDatabase(IWorkspace pWorkspace, string strCata, IRasterDataset pRasterDs,string name)
        {
            //QI IRasterWorkspaceEx to IWorkspace
            IRasterWorkspaceEx pRasWKS = pWorkspace as IRasterWorkspaceEx;

            //Open raster catalog
            //IRasterCatalog�൱��һ��FeatureClass���������е�ÿһ����¼���Դ��Raster
            IRasterCatalog pInCatalog = pRasWKS.OpenRasterCatalog(strCata);
            // get raster field index
            int iRaster = pInCatalog.RasterFieldIndex;

            // QI IFeatureClass for inserting
            IFeatureClass pFeatureClass = pInCatalog as IFeatureClass;

            // get insert cursor
            IFeatureCursor pCursor = pFeatureClass.Insert(false);

            // create raster value from raster dataset with default storagedef
            IRasterValue pRasterValue = new RasterValueClass();
            pRasterValue.RasterDataset = pRasterDs;
            pRasterValue.RasterStorageDef = new RasterStorageDefClass();

            pRasterValue.RasterDatasetName.NameString = name;
            //������������µ�RasterStorageDef

            // insert the rastervalue  ' it will update name, metadata and geometry field
            IFeatureBuffer pRow = pFeatureClass.CreateFeatureBuffer();
            pRow.set_Value(iRaster, pRasterValue);
            pCursor.InsertFeature(pRow);

        }

        //������ţ�Raster-08
        //��������	DirToGDB
        //�������ܣ���һ���ռ��е�����RasterDataset����ȫ������IRasterCatalog�Ķ����С�
        //������
        //		pWs������RasterDataset �Ŀռ�
        //		pCatalog��IRasterCatalog
        public void DirToGDB(IWorkspace pWs, IFeatureClass pCatalog)
        {
            //IRasterWorkspaceEx pSDERasterWs;
            // pEunmDatasets;
            //IRasterCatalogItem pCatalogFeature;
            IRasterDataset pRasterDs;
            //pCursor;
            IFeatureBuffer pRow;
            //get the list of datasets in the input workspace
            IEnumDataset pEunmDatasets = pWs.get_Datasets(esriDatasetType.esriDTRasterDataset);
            pEunmDatasets.Reset();

            //load raster datasets from the input workspace
            pRasterDs = pEunmDatasets.Next() as IRasterDataset;
            IFeatureCursor pCursor = pCatalog.Insert(false);
            IRasterCatalog pCat = pCatalog as IRasterCatalog;
            IDataset pDs;
            //loop through all the datasets and load
            while (pRasterDs != null)
            {
                pDs = pRasterDs as IDataset;
                //StatusBar.Message(0) = "Loading " & pDs.Name & "......"
                pRow = pCatalog.CreateFeatureBuffer();
                //pRow.set_Value(pCat.RasterFieldIndex , createRasterValue(pRasterDs));
                pCursor.InsertFeature(pRow);
                pRasterDs = pEunmDatasets.Next() as IRasterDataset;
            }

            //cleanup
            pCatalog = null;
            //pSDERasterWs = null;
            pEunmDatasets = null;
            //pCatalogFeature = null;
            pRasterDs = null;
        }

        //************************Raster Load End***************************

        //************************Raster Create Begin***********************
        //������ţ�RasterCreate-01
        //��������createSDERasterDs
        //�������ܣ��ڸ��������ݿ��д����µ�RasterDataset
        //������
        //		rasterWorkspaceEx == destination geodatabase workspace (personal or ArcSDE)
        //		rasterDatasetName == Name of raster dataset to create
        //		numberOfBands == number of bands in the raster dataset that will be created
        //		pixelType == type of pixel in target raster dataset
        //		spatialReference == desired spatial reference in raster dataset
        //		rasterStorageDef == RasterStorageDef object of Raster dataset -- defines pyramids, tiling, etc
        //		rasterDef == definition for spatial reference
        //		sKeyword == ArcSDE only, configuration keyword
        //��ע��
        //		���õĺ�����createGeometryDef(),createRasterStorageDef()��createRasterDef()�Ⱥ�����ϡ�
        //		Libraries:	ESRI.ArcGIS.Geodatabase, ESRI.ArcGIS.Geometry

        public IRasterDataset createSDERasterDs(IRasterWorkspaceEx rasterWorkspaceEx,
            string rasterDatasetName, int numberOfBands,
            rstPixelType pixelType, ISpatialReference spatialReference,
            IRasterStorageDef rasterStorageDef, IRasterDef rasterDef,
            string keyword)
        {
            // Create a raster dataset in a geodatabase workspace
            IRasterDataset rasterDataset = null;
            IGeometryDef geometryDef;

            // if rasterdef is missing, create one with specified/unknown spatialreference
            if (rasterDef == null)
                rasterDef = createRasterDef(false, spatialReference);

            // if rasterstoragedef is missing, use default parameters
            if (rasterStorageDef == null)
                rasterStorageDef = createRasterStorageDef();

            // create geometry definition
            geometryDef = createGeometryDef(spatialReference);

            // if keyword is missing, use default
            if (keyword.Length == 0)
                keyword = "DEFAULTS";
            Console.WriteLine("bb");
            rasterDataset = rasterWorkspaceEx.CreateRasterDataset(rasterDatasetName, numberOfBands, pixelType,
                rasterStorageDef, keyword, rasterDef, geometryDef);

            return rasterDataset;
        }


        //������ţ�RasterCreate-02
        //��������GetSRFromFeatureDataset
        //���ܣ���Dataset�л�ȡ�ռ�ο�
        //������IDataset��������IFeatureDatasetҲ������IRasterDataset)
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

        //������ţ�RasterCreate-03
        //��������createRasterStorageDef
        //�������ܣ�����դ��洢����
        //��ע����Щ��ϢΪArcSDE��ѡ��Ϣ�������Ĳ���Ҫ
        //		
        public IRasterStorageDef createRasterStorageDef()
        {
            // Create rasterstoragedef
            IRasterStorageDef rasterStorageDef = new RasterStorageDefClass();

            //ѡ��ѹ����ʽ
            rasterStorageDef.CompressionType = esriRasterCompressionType.esriRasterCompressionJPEG2000;// esriRasterSdeCompressionTypeEnum.esriRasterSdeCompressionTypeJPEG2000;
            //����ѡ��ѹ����
            //rasterStorageDef.CompressionQuality=75;
            //ѡ��������Ĳ���
            rasterStorageDef.PyramidLevel = 2;
            //ѡ��������ز����ķ���
            rasterStorageDef.PyramidResampleType = rstResamplingTypes.RSP_CubicConvolution;
            //�ֿ�Ĵ�С
            rasterStorageDef.TileHeight = 128;
            rasterStorageDef.TileWidth = 128;

            return rasterStorageDef;
        }


        //������ţ�RasterCreate-04
        //��������createGeometryDef
        //���ܣ��������ζ���
        //������spatialReference���ռ�ο�
        //
        public IGeometryDef createGeometryDef(ISpatialReference spatialReference)
        {
            // Create GeometryDef
            IGeometryDefEdit geometryDefEdit = new GeometryDefClass();

            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            geometryDefEdit.AvgNumPoints_2 = 4;
            //���ÿռ������Ĳ���
            geometryDefEdit.GridCount_2 = 1;
            geometryDefEdit.set_GridSize(0, 1000);

            // Set unknown spatial reference is not set
            if (spatialReference == null)
                spatialReference = new UnknownCoordinateSystemClass();

            //���ÿռ�ο�
            geometryDefEdit.SpatialReference_2 = spatialReference;

            return (IGeometryDef)geometryDefEdit;
        }

        //������ţ�RasterCreate-05
        //������:	createRasterDef
        //�������ܣ�����raster����
        //������
        public IRasterDef createRasterDef(bool isManaged, ISpatialReference spatialReference)
        {
            // Create rasterdef
            IRasterDef rasterDef = new RasterDefClass();

            rasterDef.Description = "Raster Dataset";
            if (spatialReference == null)
                spatialReference = new UnknownCoordinateSystemClass();

            rasterDef.SpatialReference = spatialReference;
            rasterDef.IsManaged = isManaged;

            return rasterDef;
        }
        //������ţ�Raster-08
        //��������createRasterValue
        //�������ܣ���RasterDataset�洢Ϊĳһ����¼���Ա�����RasterCatalog�д洢
        //������pRasterDs����Ҫ�洢��RasterDataset
        //��ע��ͨ���ú������԰�����RasterStorageDef������ѹ�����������Ľ�����
        public IRasterValue createRasterValue(IRasterDataset pRasterDs)
        {
            IRasterValue pRasterVal;
            IRasterStorageDef pRasStoreDef;

            // ++  storage parameter
            pRasStoreDef = new RasterStorageDef();

            //createRasterStorageDef
            pRasStoreDef.CompressionType = esriRasterCompressionType.esriRasterCompressionJPEG2000; //esriRasterSdeCompressionTypeEnum.esriRasterSdeCompressionTypeJPEG2000;
            pRasStoreDef.PyramidResampleType = rstResamplingTypes.RSP_CubicConvolution;

            //���PyramidLevel=-1����ȫ��������
            pRasStoreDef.PyramidLevel = -1;
            // ++  raster value to raster field

            pRasterVal = new RasterValueClass();
            pRasterVal.RasterDataset = pRasterDs;
            pRasterVal.RasterStorageDef = pRasStoreDef;

            return (pRasterVal);

            // ++ cleanup
            //�����⣬�Ѿ�return�������Ƿ����ã�
            //pRasterVal = null;
            //pRasStoreDef = null;
        }
        public IRasterValue createRasterValue(IRasterDataset pRasterDs, IRasterStorageDef pRasStoreDef)
        {
            IRasterValue pRasterVal;

            pRasterVal = new RasterValueClass();
            pRasterVal.RasterDataset = pRasterDs;
            pRasterVal.RasterStorageDef = pRasStoreDef;

            return (pRasterVal);

            // ++ cleanup
            //�����⣬�Ѿ�return�������Ƿ����ã�
            //pRasterVal = null;
            //pRasStoreDef = null;
        }
        public void CreateSDERasterBaseOnFile(IRasterStorageDef pRasterStorage, string NewFileName, IRasterWorkspaceEx pRasterWSEx, string strRasterPath, string strRasterName)
        {
            //1��get original Raster
            IRasterDataset pRasterDataset = OpenRasterDataset(strRasterPath, strRasterName);

            //2��get the Raster's property
            IRaster pRaster = pRasterDataset.CreateDefaultRaster();
            IRasterProps pRasterProp = pRaster as IRasterProps;

            rstPixelType pPixelType = pRasterProp.PixelType;

            //get the BandNumber of the Raster
            IRasterBandCollection pBands = pRasterDataset as IRasterBandCollection;
            int iBandNumber = pBands.Count;

            ISpatialReference pSR = pRasterProp.SpatialReference;

            Console.WriteLine("AAA");
            IRasterDataset NewRaster = createSDERasterDs(pRasterWSEx, NewFileName, iBandNumber, pPixelType, pSR, pRasterStorage, null, "");
            MosaicRasterToGDBRaster(pRasterDataset, NewRaster as IRasterDatasetEdit);

        }

        // Handle exception and continue 
        // executing.
        //*************************Raster Create End***************************


        //**************************Raster Open Begin**************************

        //��������	OpenRasterDatasetFromSDE
        //�������ܣ���SDE�л��RasterDataset
        //������	rasterDatasetName��������
        //��ע��
        //Libraries needed to run this code:
        //ESRI.ArcGIS.esriSystem, ESRI.ArcGIS.Geodatabase, ESRI.ArcGIS.DataSourcesGDB
        public IRasterDataset OpenRasterDatasetFromSDE(string server, string instance, string database, string user,
            string password, string rasterDatasetName, string version)
        {

            // Open an ArcSDE raster dataset with the given name
            // server, instance, database, user, password, version are database connection info
            // rasterDatasetName is the name of the raster dataset to be opened//Open the ArcSDE workspace
            IPropertySet propertySet = new PropertySetClass();

            propertySet.SetProperty("server", server);
            propertySet.SetProperty("instance", instance);
            propertySet.SetProperty("database", database);
            propertySet.SetProperty("user", user);
            propertySet.SetProperty("password", password);
            propertySet.SetProperty("version", version);

            // cocreate the workspace factory
            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();

            // Open the raster workspace using the previously defined porperty set and
            //  QI to the desired IRasterWorkspaceEx interface to access the existing dataset
            IRasterWorkspaceEx rasterWorkspaceEx = workspaceFactory.Open(propertySet, 0) as IRasterWorkspaceEx;

            //Open the ArcSDE raster dataset
            IRasterDataset rasterDataset = null;
            rasterDataset = rasterWorkspaceEx.OpenRasterDataset(rasterDatasetName);

            return rasterDataset;
        }

        //��������OpenRasterDataset
        //���ܣ������ռ��Dataset�����ƣ���ȡRasterDataset
        //������directoryName:workspace's file path
        //		fileName:Dataset's name including the  extension for example .tif
        //��ע�� Libraries needed to run the code:ESRI.ArcGIS.Geodatabase, ESRI.ArcGIS.DataSourcesRaster
        public IRasterDataset OpenRasterDataset(string directoryName, string fileName)
        {
            //Open the raster dataset with the given name.
            //directoryName is the directory where the file resides
            //fileName is the filename to be opened

            //Open the workspace
            IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactoryClass();

            // define the directory as a raster workspace
            IRasterWorkspace rasterWorkspace = workspaceFactory.OpenFromFile(directoryName, 0) as IRasterWorkspace;

            //Open the raster dataset
            IRasterDataset rasterDataset = null;
            rasterDataset = rasterWorkspace.OpenRasterDataset(fileName);

            // Return raster dataset
            return rasterDataset;
        }

        //��������openAccessWorkspace
        //�������ܣ��򿪸������ݿ�ռ�
        public IWorkspace openAccessWorkspace(string connString)
        {
            //Open the AccessWorkSpace by the given name which including extention(.mdb)
            IWorkspace ws = null;
            IWorkspaceFactory wsf = new AccessWorkspaceFactoryClass();
            ws = wsf.OpenFromFile(connString, 0);
            return ws;
        }

        //��������openSDEWorkspace
        //�������ܣ�create and open the sde workspace based on the provided information
        //
        public IWorkspace openSDEWorkspace(string Server, string Instance, string User,
            string Password, string Database, string version)
        {
            IWorkspace ws = null;

            try
            {

                IPropertySet pPropSet = new PropertySetClass();
                IWorkspaceFactory pSdeFact = new SdeWorkspaceFactoryClass();

                pPropSet.SetProperty("SERVER", Server);
                pPropSet.SetProperty("INSTANCE", Instance);
                pPropSet.SetProperty("DATABASE", Database);
                pPropSet.SetProperty("USER", User);
                pPropSet.SetProperty("PASSWORD", Password);
                pPropSet.SetProperty("VERSION", version);
                ws = pSdeFact.Open(pPropSet, 0);
                return ws;
            }
            catch
            {
                return ws;
                //if (e is StackOverflowException ||
                //	e is OutOfMemoryException)
                //	throw;								
            }

        }

    }
}
