using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.DataSourcesRaster;

namespace CoastalGIS.MainGIS.Class
{
    public class DEMCalc
    {
        public DEMCalc(IMapControlDefault mapControl)
        {
            m_mapControl = mapControl;
        }

        private IMapControlDefault m_mapControl;

        /// <summary>
        /// 计算DEM坡度
        /// </summary>
        /// <param name="rasterLayer">输入DEM图层</param>
        /// <param name="outputFullPath">输出坡度图路径，类型为IMAGE，文件名为img</param>
        public void CreateRasterSlope(ILayer rasterLayer, string outputFullPath)
        {
            if (File.Exists(outputFullPath))
            {
                MessageBox.Show(outputFullPath + "已经存在，请重命名");
                return;
            }
            IRasterLayer rasterLyr = rasterLayer as IRasterLayer;
            IRaster raster = rasterLyr.Raster;
            RasterSurfaceOpClass rasterOpCls = new RasterSurfaceOpClass();
            object o = 1;
            IGeoDataset geoDataset = raster as IGeoDataset;
            esriGeoAnalysisSlopeEnum geoType = esriGeoAnalysisSlopeEnum.esriGeoAnalysisSlopeDegrees;
            IGeoDataset outGeodataset = rasterOpCls.Slope(geoDataset, geoType, ref o);
            IRaster pRaster = outGeodataset as IRaster;

            FileInfo fi = new FileInfo(outputFullPath);
            string fileDir = fi.Directory.FullName;
            string name = fi.Name;
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);
            IWorkspaceFactory pWSF = new RasterWorkspaceFactoryClass();
            IWorkspace pWS = pWSF.OpenFromFile(fileDir, 0);
            ISaveAs pSaveAs = pRaster as ISaveAs;
            pSaveAs.SaveAs(name, pWS, "IMAGINE Image");
            IRasterLayer rasterLyr2 = new RasterLayerClass();
            rasterLyr2.CreateFromRaster(pRaster);
            rasterLyr2.Name = name;
            m_mapControl.Map.AddLayer(rasterLyr2 as ILayer);
        }

        /// <summary>
        /// 计算DEM坡向
        /// </summary>
        /// <param name="rasterLayer">输入DEM图层</param>
        /// <param name="outputFullPath">输出坡度图路径，类型为IMAGE，文件名为img</param>
        public void CreateRasterAspect(ILayer rasterLayer, string outputFullPath)
        {
            if (File.Exists(outputFullPath))
            {
                MessageBox.Show(outputFullPath + "已经存在，请重命名");
                return;
            }
            IRasterLayer rasterLyr = rasterLayer as IRasterLayer;
            IRaster raster = rasterLyr.Raster;
            RasterSurfaceOpClass rasterOpCls = new RasterSurfaceOpClass();
            IGeoDataset geoDataset = raster as IGeoDataset;
            IGeoDataset outGeodataset = rasterOpCls.Aspect(geoDataset);
            IRaster pRaster = outGeodataset as IRaster;

            FileInfo fi = new FileInfo(outputFullPath);
            string fileDir = fi.Directory.FullName;
            string name = fi.Name;
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);
            IWorkspaceFactory pWSF = new RasterWorkspaceFactoryClass();
            IWorkspace pWS = pWSF.OpenFromFile(fileDir, 0);
            ISaveAs pSaveAs = pRaster as ISaveAs;
            pSaveAs.SaveAs(name, pWS, "IMAGINE Image");
            IRasterLayer rasterLyr2 = new RasterLayerClass();
            rasterLyr2.CreateFromRaster(pRaster);
            rasterLyr2.Name = name;
            m_mapControl.Map.AddLayer(rasterLyr2 as ILayer);
        }
    }
}
