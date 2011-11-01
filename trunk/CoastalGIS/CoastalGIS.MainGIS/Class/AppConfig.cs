using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Configuration;

namespace CoastalGIS.MainGIS
{
    /// <summary>
    /// C#中动态读写App.config配置文件
    /// </summary>
    class AppConfig
    {

        public AppConfig()
        {
            ///
            /// TODO: 在此处添加构造函数逻辑
            ///
        }

        public static void ConfigSetValue(string appKey, string appValue) 
        {
            System.Configuration.Configuration config =ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection appSection = (AppSettingsSection)config.GetSection("appSettings");
            appSection.Settings[appKey].Value = appValue;
            config.Save();

        }
    }
}
