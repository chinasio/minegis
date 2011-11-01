using System;
using System.Collections.Generic;
using System.Text;

namespace CoastalGIS.SpatialDataBase
{
    /// <summary>
    /// 存储连接Oracle数据库的参数
    /// </summary>
    public struct SDEToORAPra
    {
        public string Server;
        public string Instance;
        public string User;
        public string Password;
        public string Version;
        public string SDEPath;
    }
}
