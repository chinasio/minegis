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
    public class AddCADToRaster:BaseCommand 
    {
        private IHookHelper m_hookHelper = null;
        public AddCADToRaster()
        {
            //update the base properties
            base.m_category = ".NET Samples";
            base.m_caption = "AddCADToRaster";
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
                ICadDrawingWorkspace cadworkspace = cadWorkSpaceFactory.OpenFromFile(path, 0) as ICadDrawingWorkspace;
                ICadDrawingDataset cadDataset = cadworkspace.OpenCadDrawingDataset(name);
                ICadLayer cadLayer = new CadLayerClass();
                cadLayer.CadDrawingDataset = cadDataset;
                ((IMapControlDefault)m_hookHelper.Hook).AddLayer(cadLayer,0);


            }
        }
    }
}
