using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using System.IO;

namespace CoastalGIS.ExportMapProj
{
    public partial class frmTempleteProperties : Form
    {
        private IMap m_Map;
        private IGeometry m_Geometry;
        public ITextElement m_TitleElement;
      // private frmTempleteProperties frmTempPro = null;
         private string  m_txtTitle = "";
        private string m_txtTime = "";
        private string m_txtName = "";
        private string m_txtProj = "";
        private string m_txtelevation = "";
        private string m_txtOthereInfo = "";
      //  private bool m_checkBLengend = false;
       // private bool m_checkBNorthA = false;
       // private bool m_checkBScaleB = false;
       // private bool m_checkBMapG = false;
       // private bool m_checkBText = false;
        private bool m_ifCancle = true ;
        public bool ifCancle
        {
            get 
            {
                return m_ifCancle;
            }
        }
        public string elevationName
        {
            get
             {
                 return m_txtelevation;
            }
        }
        public string  ptextTitle
        {
           get
            {
               return m_txtTitle;
            }
        }

        public string ptextName
        {
            get
            {
                return m_txtName;
            }
        }

        public string ptextProject
        {
            get 
            {
               return m_txtProj;
            }
        }
        
       public string ptextTime
        {
            get { return m_txtTime; }
        }
        public string pOtherInfo
        {
            get
            {
                return m_txtOthereInfo;
            }
        }
      
     /*  public bool  pcheckLegend
        {
           get { return m_checkBLengend; }
        }

        public bool pcheckNorthArr
        { 
            get { return m_checkBNorthA; } 
        }

        public bool pcheckScaleB
        {
            get { return m_checkBScaleB; }
        }

        public bool pcheckMapG
        {
            get { return m_checkBMapG; }
        }

        public bool pcheckText
        {
          get { return m_checkBText; }
        }*/
        public frmTempleteProperties(IMap pMap, IGeometry pGeometry)
        {
            InitializeComponent();
            m_Map = pMap;
            m_Geometry = pGeometry;
           // textTitle = ptextTitle ;
           // textName = ptextName ;
           // textProject = ptextProject ;
           // textDate  = ptextTime ;
           // checkLegend = pcheckLegend ;
           // checkNorthArrow = pcheckNorthArr ;
           // checkMapGrid = pcheckMapG;
           // checkScaleBar = pcheckScaleB ;
           // checkText = pcheckText;

        }

        private void frmTempleteProperties_Load(object sender, EventArgs e)
        {

            ISpatialReference pSpatiaRer = this.m_Map.SpatialReference;
            IProjectedCoordinateSystem pProjCoordiantaSys = pSpatiaRer as IProjectedCoordinateSystem;
            if (pProjCoordiantaSys != null)
            {
                if (pProjCoordiantaSys.GeographicCoordinateSystem.Name != "")
                {
                    string CoordinateSysName = pProjCoordiantaSys.GeographicCoordinateSystem.Name.Substring(4, pProjCoordiantaSys.GeographicCoordinateSystem.Name.Length - 4);
                    if (CoordinateSysName.ToString().Contains("1984"))
                    {
                        this.comboBox1.SelectedIndex = 0;
                    }
                    else if(CoordinateSysName.ToString().Contains("1954"))
                    {
                        this.comboBox1.SelectedIndex = 1;
                    }
                    else
                    {
                        this.comboBox1.SelectedIndex = 2;
                    }

                }
                else if (pProjCoordiantaSys.Name.Contains("Guass"))
                {
                    this.comboBox1.SelectedIndex = 6;
                }
                else if (pProjCoordiantaSys.Name.Contains("Mercator"))
                {
                    this.comboBox1.SelectedIndex = 7;
                }
                else
                {
                    this.comboBox1.Text = pProjCoordiantaSys.Name;
                }
            }
            this.textBoxTime.Text = DateTime.Now.ToLongDateString();//
            this.comboBox2.SelectedIndex = 1;

        }

        private void button1_Click(object sender, EventArgs e)
        {
             m_TitleElement = new TextElementClass();
            m_TitleElement.Text  = this.textBoxTitle.Text;
            frmTextSymbol frmtext = new frmTextSymbol(ref m_TitleElement);
            frmtext.ShowDialog();
            this.textBoxTitle.Text = m_TitleElement.Text;
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            m_txtTitle = this.textBoxTitle.Text ;
            m_txtName = this.textBoxName.Text ;
            m_txtProj = this.comboBox1.Text;
            m_txtTime = this.textBoxTime.Text ;
            m_txtelevation = this.comboBox2.Text;
           // m_checkBText = this.checkBoxText.Checked ;
           // m_checkBScaleB = this.checkBoxScaleBar.Checked ;
           // m_checkBLengend = this.checkBoxLegend.Checked ;
           // m_checkBNorthA = this.checkBoxNorthArrow.Checked ;
           // m_checkBMapG = this.checkBoxMapGrid.Checked ;
            m_ifCancle = false;
            m_txtOthereInfo = this.textBoxOtherInfo.Text;
            this.Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            m_ifCancle = true ;
            this.Close();

        }

     
    }
}