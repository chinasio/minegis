using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;


namespace CoastalGIS.SpatialDataBase
{
    public partial class frmPrintMetaData : Form
    {
        public frmPrintMetaData(DataSet ds)
        {
            InitializeComponent();
            ConfigCrystalReports();
            this.CrystalReport21.SetDataSource(ds.Tables[0]);
        }

        public frmPrintMetaData()
        {
            InitializeComponent();
            ConfigCrystalReports();
        }

        private void ConfigCrystalReports()
        {
            ConnectionInfo ConnectionInfo = new ConnectionInfo();
            ConnectionInfo.ServerName = "";
            ConnectionInfo.DatabaseName = "";
            ConnectionInfo.UserID = "CoastalGIS";
            ConnectionInfo.Password = "fgis";
            //string ReportPath = Server.MapPath("CrystalReport1.rpt（这个是你的报表名称）");
            //crystalReportViewer1.ReportSource = ReportPath;
            SetDbLoginForReport(ConnectionInfo);

        }

        private void SetDbLoginForReport(ConnectionInfo ConnectionInfo)
        {
            TableLogOnInfos tableLogOnInfos = this.crystalReportViewer1.LogOnInfo;
            foreach (TableLogOnInfo tableLogOnInfo in tableLogOnInfos)
            {
                tableLogOnInfo.ConnectionInfo = ConnectionInfo;
            }
        }


    }
}