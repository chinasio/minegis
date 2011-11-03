using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.Geoprocessor;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.SpatialAnalystTools;
using ESRI.ArcGIS.DataSourcesRaster;

namespace CoastalGIS.MainGIS.Class
{
    public class ClipLayer
    {
        private IMapControlDefault m_mapControl;

        public ClipLayer(IMapControlDefault mapControl)
        {
            m_mapControl = mapControl;
        }

        /// <summary>
        /// 两图层进行裁剪运算
        /// </summary>
        /// <param name="inputLayer">被裁剪图层</param>
        /// <param name="clipLayer">裁剪图层</param>
        /// <param name="outputFullPath">输出图层完整路径</param>
        public void ClipByLayer(ILayer inputLayer, ILayer clipLayer, string outputFullPath)
        {
            string inputPath = GetLayerPath(inputLayer);
            string clipPath = GetLayerPath(clipLayer);

            Clip clipTool = new Clip();
            clipTool.in_features = inputPath;
            clipTool.clip_features = clipPath;
            clipTool.out_feature_class = outputFullPath;

            Geoprocessor processor = new Geoprocessor();
            IGPProcess process = null;
            processor.OverwriteOutput = true;
            process = clipTool;
            processor.Validate(process, true);
            processor.Execute(process, null);

            FileInfo fi = new FileInfo(outputFullPath);
            string pathDir = fi.Directory.FullName;
            string name = fi.Name;
            IWorkspaceFactory wsf = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace fws = wsf.OpenFromFile(pathDir, 0) as IFeatureWorkspace;
            IFeatureClass featCls = fws.OpenFeatureClass(name);
            IFeatureLayer layer = new FeatureLayerClass();
            layer.FeatureClass = featCls;
            layer.Name = featCls.AliasName;
            m_mapControl.Map.AddLayer(layer as ILayer);
        }

        /// <summary>
        /// 图层切割栅格数据
        /// </summary>
        /// <param name="rasterLayer">栅格数据图层</param>
        /// <param name="clipLayer">切割适量图层</param>
        /// <param name="outputFullPath">切割栅格完整路径</param>
        public void ClipRasterByLayer(ILayer rasterLayer, ILayer clipLayer, string outputFullPath)
        {
            string rasterPath = GetLayerPath(rasterLayer);
            string clipPath = GetLayerPath(clipLayer);

            ExtractByMask clipTool = new ExtractByMask();
            clipTool.in_raster = rasterPath;
            clipTool.in_mask_data = clipPath;
            clipTool.out_raster = outputFullPath;

            Geoprocessor processor = new Geoprocessor();
            IGPProcess process = null;
            processor.OverwriteOutput = true;
            process = clipTool;
            processor.Validate(process, true);
            processor.Execute(process, null);

            DirectoryInfo di = new DirectoryInfo(outputFullPath);
            string rasterDir = di.Parent.FullName;
            string name = di.Name;
            IWorkspaceFactory rasterWorkspaceFactory = new RasterWorkspaceFactoryClass();
            IRasterWorkspace rasterWorkspace = rasterWorkspaceFactory.OpenFromFile(rasterDir, 0) as IRasterWorkspace;
            IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(name);
            IRasterLayer rasterLyr = new RasterLayerClass();
            rasterLyr.CreateFromDataset(rasterDataset);
            m_mapControl.Map.AddLayer(rasterLyr as ILayer);
        }

        private string GetLayerPath(ILayer pLayer)
        {
            string path = null;
            IDataLayer2 dataLyr = pLayer as IDataLayer2;
            IDatasetName datasetName = dataLyr.DataSourceName as IDatasetName;
            IWorkspaceName ws = datasetName.WorkspaceName;
            path = ws.PathName + "\\" + pLayer.Name;
            return path;
        }
    }
}
