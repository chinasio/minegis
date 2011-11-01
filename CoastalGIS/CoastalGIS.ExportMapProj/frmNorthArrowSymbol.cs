using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DisplayUI;


namespace CoastalGIS.ExportMapProj
{
    public partial class frmNorthArrowSymbol : Form
    {
        private IStyleGalleryItem pStyleGalleryItem;
        //   private IStyleGalleryItem m_StyleGalleryItem ;
        public ISymbol pSymbol;
        private double m_Angle;

        public frmNorthArrowSymbol()
        {
            InitializeComponent();
        }

        private void frmNorthArrowSymbol_Load(object sender, EventArgs e)
        {
            string sInstal = ReadRegistry("SOFTWARE\\ESRI\\CoreRuntime");
            //加载ESRI.ServerStyle样式文件到SymbologyControl
            try
            {
                this.axSymbologyControl1.LoadStyleFile(sInstal + "\\Styles\\ESRI.ServerStyle");
            }
            catch
            {
                //MessageBox.Show(ex.ToString ());
            }
            //this.axSymbologyControl1.Refresh();
            m_Angle = 0;
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            m_Angle = 0;
            pStyleGalleryItem = null;
            this.Close();
        }
        public string ReadRegistry(string sKey)
        {
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sKey, true);
            if (rk == null)
                return " ";
            return (string)rk.GetValue("InstallDir");

        }

        private void axSymbologyControl1_OnItemSelected(object sender, ESRI.ArcGIS.Controls.ISymbologyControlEvents_OnItemSelectedEvent e)
        {
            pStyleGalleryItem = (IStyleGalleryItem)e.styleGalleryItem;
            this.PreviewImage();
        }
        public IStyleGalleryItem GetItem(ESRI.ArcGIS.Controls.esriSymbologyStyleClass styleClass)
        {
            pStyleGalleryItem = null;
            //Set the style class of SymbologyControl1
            axSymbologyControl1.StyleClass = styleClass;

            //Change cursor
            this.Cursor = Cursors.Default;

            //Show the modal form
            this.ShowDialog();

            //return the label style that has been selected from the SymbologyControl
            return pStyleGalleryItem;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            this.Close();
        }
        private void PreviewImage()
        {
            stdole.IPictureDisp picture = this.axSymbologyControl1.GetStyleClass(this.axSymbologyControl1.StyleClass).PreviewItem(pStyleGalleryItem, this.pictureBox1 .Width, this.pictureBox1.Height);
            System.Drawing.Image image = System.Drawing.Image.FromHbitmap(new System.IntPtr(picture.Handle));
            this.pictureBox1.Image = image;

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {  
           m_Angle =(double )this.numericUpDown1 .Value ;
           //((INorthArrow )this.pStyleGalleryItem.Item).CalibrationAngle = (double)this.numericUpDown1.Value;
            this.PreviewImage();
        }
        public double Angle()
        {
            return m_Angle;
        }

    }
}