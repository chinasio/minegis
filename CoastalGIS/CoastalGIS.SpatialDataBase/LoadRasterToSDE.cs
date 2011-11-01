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
    /// publicFunction 的摘要说明。
    /// </summary>
    public class LoadRasterToSDE
    {
        public LoadRasterToSDE()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        //Open RasterDataset by pathName and fileName
        public IRasterDataset openRasterDataset(string pathName, string fileName)
        {
            IWorkspaceFactory workspaceFactory;
            IRasterWorkspace rasterWorkspace;
            IRasterDataset rasterDataset = null;

            workspaceFactory = new RasterWorkspaceFactoryClass();

            //注意程序的健壮性一定要对路径的提前判断
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
        //		//在此处应该设置影像数据的存储参数
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
            //1.根据名称在featureClass 中找到feature
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = "NAME ='" + RasterDSName + "'";

            IFeatureCursor pFeatCur = pCatalog.Search(pQueryFilter, false);

            int i = pFeatCur.FindField("Name");

            IFeature pFeat = pFeatCur.NextFeature();

            pFeat.Delete();
        }


        //根据pCatalog的名称以及Name字段获取IRasterDataset
        //注：只能删除第一个满足条件的数据
        public IRasterDataset GetRasterDatasetbyName(IFeatureClass pCatalog, string RasterDSName)
        {
            //1.根据名称在featureClass 中找到feature
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = "NAME ='" + RasterDSName + "'";

            IFeatureCursor pFeatCur = pCatalog.Search(pQueryFilter, false);

            int i = pFeatCur.FindField("Name");

            IFeature pFeat = pFeatCur.NextFeature();

            //2.找到Raster字段,并把其中的值返回IRasterDataset
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
            //1.根据名称在featureClass 中找到feature
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = "NAME ='" + RasterDSName + "'";

            IFeatureCursor pFeatCur = pCatalog.Search(pQueryFilter, false);

            int i = pFeatCur.FindField("Name");

            IFeature pFeat = pFeatCur.NextFeature();

            //2.找到Raster字段,并把其中的值返回IRasterDataset
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
        //#######################以下函数可配合使用########################

        //**************************Load Raster Begin****************************		

        //函数编号：RasterLoad-01
        //函数名：DirToMosaic 
        //函数功能：把文件夹中的Raster文件合并到新的Raster文件中去
        //参数：sDir：栅格文件夹
        //      pDataset：最后合并的RasterDataset
        //备注：pDataset可以使Geodataset中的参数，如果不存在利用函数创建
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
                //!!!注意IRasterDataset2::CreateFullRaster()方法可以简单实现从RasterDataset到Raster的转换
                pRasterDs = pEunmDatasets.Next() as IRasterDataset2;
            }

            //cleanup

            pEunmDatasets = null;
            pRasterDs = null;
            pWsFact = null;
            pSDEDs = null;
            pFileWs = null;
        }
        //函数编号：Raster-07
        //函数名:MosaicRasterToGDBRaster
        //函数功能:Raster文件Mosaic到SDE Raster文件中
        //参数:
        public void MosaicRasterToGDBRaster(IRasterDataset pRasterFile, IRasterDatasetEdit pGDBRasterDs)
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

        //函数号：
        //函数名：LoadRasterToCatalogDatabase
        //函数功能：Raster导入Database
        //参数：pWorkspace：数据库，pWorkspace可以是Access 或者SDE
        //		strCata：Catalog名称
        //		pRasterDs：需要入库的Rasterdataset
        //备注：insert a raster dataset with given path and file name to an existing raster catalog
        public void LoadRasterToCatalogDatabase(IWorkspace pWorkspace, string strCata, IRasterDataset pRasterDs,string name)
        {
            //QI IRasterWorkspaceEx to IWorkspace
            IRasterWorkspaceEx pRasWKS = pWorkspace as IRasterWorkspaceEx;

            //Open raster catalog
            //IRasterCatalog相当于一个FeatureClass（表），表中的每一个记录可以存放Raster
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
            //这里可以设置新的RasterStorageDef

            // insert the rastervalue  ' it will update name, metadata and geometry field
            IFeatureBuffer pRow = pFeatureClass.CreateFeatureBuffer();
            pRow.set_Value(iRaster, pRasterValue);
            pCursor.InsertFeature(pRow);

        }

        //函数编号：Raster-08
        //函数名：	DirToGDB
        //函数功能：把一个空间中的所有RasterDataset对象全部存入IRasterCatalog的对象中。
        //参数：
        //		pWs：包含RasterDataset 的空间
        //		pCatalog：IRasterCatalog
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
        //函数编号：RasterCreate-01
        //函数名：createSDERasterDs
        //函数功能：在给定的数据库中创建新的RasterDataset
        //参数：
        //		rasterWorkspaceEx == destination geodatabase workspace (personal or ArcSDE)
        //		rasterDatasetName == Name of raster dataset to create
        //		numberOfBands == number of bands in the raster dataset that will be created
        //		pixelType == type of pixel in target raster dataset
        //		spatialReference == desired spatial reference in raster dataset
        //		rasterStorageDef == RasterStorageDef object of Raster dataset -- defines pyramids, tiling, etc
        //		rasterDef == definition for spatial reference
        //		sKeyword == ArcSDE only, configuration keyword
        //备注：
        //		调用的函数：createGeometryDef(),createRasterStorageDef()，createRasterDef()等函数配合。
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


        //函数编号：RasterCreate-02
        //函数名：GetSRFromFeatureDataset
        //功能：从Dataset中获取空间参考
        //参数：IDataset（可以是IFeatureDataset也可以是IRasterDataset)
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

        //函数编号：RasterCreate-03
        //函数名：createRasterStorageDef
        //函数功能：创建栅格存储定义
        //备注：这些信息为ArcSDE可选信息，其他的不需要
        //		
        public IRasterStorageDef createRasterStorageDef()
        {
            // Create rasterstoragedef
            IRasterStorageDef rasterStorageDef = new RasterStorageDefClass();

            //选择压缩方式
            rasterStorageDef.CompressionType = esriRasterCompressionType.esriRasterCompressionJPEG2000;// esriRasterSdeCompressionTypeEnum.esriRasterSdeCompressionTypeJPEG2000;
            //可以选择压缩比
            //rasterStorageDef.CompressionQuality=75;
            //选择金字塔的层数
            rasterStorageDef.PyramidLevel = 2;
            //选择金字塔重采样的方法
            rasterStorageDef.PyramidResampleType = rstResamplingTypes.RSP_CubicConvolution;
            //分块的大小
            rasterStorageDef.TileHeight = 128;
            rasterStorageDef.TileWidth = 128;

            return rasterStorageDef;
        }


        //函数编号：RasterCreate-04
        //函数名：createGeometryDef
        //功能：创建几何定义
        //参数：spatialReference：空间参考
        //
        public IGeometryDef createGeometryDef(ISpatialReference spatialReference)
        {
            // Create GeometryDef
            IGeometryDefEdit geometryDefEdit = new GeometryDefClass();

            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            geometryDefEdit.AvgNumPoints_2 = 4;
            //设置空间索引的层数
            geometryDefEdit.GridCount_2 = 1;
            geometryDefEdit.set_GridSize(0, 1000);

            // Set unknown spatial reference is not set
            if (spatialReference == null)
                spatialReference = new UnknownCoordinateSystemClass();

            //设置空间参考
            geometryDefEdit.SpatialReference_2 = spatialReference;

            return (IGeometryDef)geometryDefEdit;
        }

        //函数编号：RasterCreate-05
        //函数名:	createRasterDef
        //函数功能：设置raster定义
        //参数：
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
        //函数编号：Raster-08
        //函数名：createRasterValue
        //函数功能：把RasterDataset存储为某一个记录，以便于在RasterCatalog中存储
        //参数：pRasterDs：需要存储的RasterDataset
        //备注：通过该函数可以把设置RasterStorageDef。包括压缩、金字塔的建立等
        public IRasterValue createRasterValue(IRasterDataset pRasterDs)
        {
            IRasterValue pRasterVal;
            IRasterStorageDef pRasStoreDef;

            // ++  storage parameter
            pRasStoreDef = new RasterStorageDef();

            //createRasterStorageDef
            pRasStoreDef.CompressionType = esriRasterCompressionType.esriRasterCompressionJPEG2000; //esriRasterSdeCompressionTypeEnum.esriRasterSdeCompressionTypeJPEG2000;
            pRasStoreDef.PyramidResampleType = rstResamplingTypes.RSP_CubicConvolution;

            //如果PyramidLevel=-1则建立全金字塔，
            pRasStoreDef.PyramidLevel = -1;
            // ++  raster value to raster field

            pRasterVal = new RasterValueClass();
            pRasterVal.RasterDataset = pRasterDs;
            pRasterVal.RasterStorageDef = pRasStoreDef;

            return (pRasterVal);

            // ++ cleanup
            //？问题，已经return后的语句是否能用？
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
            //？问题，已经return后的语句是否能用？
            //pRasterVal = null;
            //pRasStoreDef = null;
        }
        public void CreateSDERasterBaseOnFile(IRasterStorageDef pRasterStorage, string NewFileName, IRasterWorkspaceEx pRasterWSEx, string strRasterPath, string strRasterName)
        {
            //1、get original Raster
            IRasterDataset pRasterDataset = OpenRasterDataset(strRasterPath, strRasterName);

            //2、get the Raster's property
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

        //函数名：	OpenRasterDatasetFromSDE
        //函数功能：在SDE中获得RasterDataset
        //参数：	rasterDatasetName函数名。
        //备注：
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

        //函数名：OpenRasterDataset
        //功能：给定空间和Dataset的名称，获取RasterDataset
        //参数：directoryName:workspace's file path
        //		fileName:Dataset's name including the  extension for example .tif
        //备注： Libraries needed to run the code:ESRI.ArcGIS.Geodatabase, ESRI.ArcGIS.DataSourcesRaster
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

        //函数名：openAccessWorkspace
        //函数功能：打开个人数据库空间
        public IWorkspace openAccessWorkspace(string connString)
        {
            //Open the AccessWorkSpace by the given name which including extention(.mdb)
            IWorkspace ws = null;
            IWorkspaceFactory wsf = new AccessWorkspaceFactoryClass();
            ws = wsf.OpenFromFile(connString, 0);
            return ws;
        }

        //函数名：openSDEWorkspace
        //函数功能：create and open the sde workspace based on the provided information
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
