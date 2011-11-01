using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Carto;

namespace CoastalGIS.MainGIS
{
    public class AddCADToFeatures : BaseCommand
    {
        private IHookHelper m_hookHelper = null;

        public AddCADToFeatures()
        {
            //update the base properties
            base.m_category = ".NET Samples";
            base.m_caption = "AddCADToFeatures";
        }

        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
        }

        public override void OnClick()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "打开CAD文件";
            openFileDialog1.Filter = "CAD图形文件(*.dwg)|*.DWG";
            if (openFileDialog1.ShowDialog() == DialogResult.OK && m_hookHelper.Hook is IMapControlDefault)
            {
                FileInfo fileinfo = new FileInfo(openFileDialog1.FileName);
                string path = fileinfo.DirectoryName;
                string name = fileinfo.Name;
                IWorkspaceFactory cadWorkSpaceFactory = new CadWorkspaceFactoryClass();
                IFeatureWorkspace workspace = cadWorkSpaceFactory.OpenFromFile(path, 0) as IFeatureWorkspace;
                IFeatureDataset featDataset = workspace.OpenFeatureDataset(name);
                IFeatureClassContainer featClassContainer = featDataset as IFeatureClassContainer;
                IFeatureClass featClass;
                IFeatureLayer featLayer;

                for (int i = 0; i < featClassContainer.ClassCount; i++)
                {
                    featClass = featClassContainer.get_Class(i);
                    if (featClass.FeatureType == esriFeatureType.esriFTCoverageAnnotation)
                    {
                        //标注类型，必须设置为单位的标注图层
                        featLayer = new CadAnnotationLayerClass();
                    }
                    else
                    {
                        //点线面类型
                        featLayer = new FeatureLayerClass();
                    }

                    featLayer.Name = featClass.AliasName;
                    featLayer.FeatureClass = featClass;
                    ((IMapControlDefault)m_hookHelper.Hook).Map.AddLayer((ILayer)featLayer);
                }
            }

        }
    }
}
