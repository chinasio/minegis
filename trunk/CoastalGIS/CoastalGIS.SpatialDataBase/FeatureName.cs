using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

namespace CoastalGIS.SpatialDataBase
{
    public class FeatureName
    {
        public string FeatDSName;
        public IList<string> FCName=new List<string>();
        public IList<esriGeometryType> ShapType = new List<esriGeometryType>();
        public IList<esriFeatureType> FeatureType = new List<esriFeatureType>();
    }
}
