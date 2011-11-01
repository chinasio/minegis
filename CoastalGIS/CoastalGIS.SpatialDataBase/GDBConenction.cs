using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;

namespace CoastalGIS.SpatialDataBase
{
    /// <summary>
    ///连接空间数据库
    /// </summary>
    public class GDBConenction
    {

        private string m_fgdbPath;

        private IWorkspaceFactory2 m_workspaceFactory;
        private IWorkspace m_workSpace;

        public GDBConenction(string fgdbPath) 
        {
            this.m_fgdbPath = fgdbPath;
        }

        public IWorkspace OpenSDEWorkspace()
        {
            m_workspaceFactory = new FileGDBWorkspaceFactoryClass();

            IFeatureWorkspace featureWorkspace = null;
            try
            {
                featureWorkspace = m_workspaceFactory.OpenFromFile(m_fgdbPath, 0) as IFeatureWorkspace;

            }
            catch
            {
                MessageBox.Show("请检查连接SDE服务器的连接字符串");
                return null;
            }
            IWorkspace workspace = featureWorkspace as IWorkspace;
            this.m_workSpace = workspace;
            return workspace;
        }


        public IFeatureWorkspace OpenSHPWorkspace(string shpPath) 
        {
            m_workspaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace featureWorkspce = m_workspaceFactory.OpenFromFile(shpPath, 0) as IFeatureWorkspace;
            return featureWorkspce;
        }

        public IFeatureWorkspace OpenCadWorkspace(string cadPath)
        {
            m_workspaceFactory = new CadWorkspaceFactoryClass();
            IFeatureWorkspace featureWorkspce = m_workspaceFactory.OpenFromFile(cadPath, 0) as IFeatureWorkspace;
            return featureWorkspce;
        }

    }
}
