using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;


namespace CoastalGIS.ExportMapProj
{
    public partial class frmTextSymbol : Form
    {
       // private  string ptext=" " ;
       // private  string m_text = "";//用户最后得到的文本
        private ITextElement pTextElement;
        public frmTextSymbol( ref ITextElement textElement)
        {
            InitializeComponent();
            pTextElement = textElement;
        }

        private void frmTextSymbol_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = pTextElement .Text ;
            System.Drawing.Text.InstalledFontCollection fonts = new System.Drawing.Text.InstalledFontCollection();
           // System.Drawing.Font fontSize = new System.Drawing.Font();
            foreach (FontFamily family in fonts.Families)
            {
                comboFont.Items.Add(family.Name.ToString());
            }
   
            textFont.Text = pTextElement.Symbol.Font.Name;
            textSize.Text = pTextElement.Symbol.Size.ToString();
            textAngle.Text = pTextElement.Symbol.Angle.ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
          
            stdole.IFontDisp pFont = (stdole.IFontDisp)new stdole.StdFontClass();
            pFont.Name = textFont.Text.Trim();
           // pFont.Size = Convert.ToInt32(textSize.Text.Trim());
           // pTextElement.Symbol.Angle = Convert.ToDouble(textAngle.Text.Trim());
          
            ITextSymbol ptextSymbol = new TextSymbolClass();
            ptextSymbol.Angle = Convert.ToDouble(textAngle.Text.Trim());
            ptextSymbol.Font = pFont;
            ptextSymbol.Text = textBox1.Text;
            pTextElement.Text = textBox1.Text;
            ptextSymbol.Size = Convert.ToDouble(textSize.Text.Trim());
            pTextElement.Symbol = ptextSymbol;
            this.Close ();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            
            this.Close() ;
        }
      
        
        public ITextElement  getText()
        {
            return pTextElement;
        }

        private void comboFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            textFont.Text = comboFont.Text.Trim();
        }

        private void comboSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            textSize.Text = comboSize.Text.Trim();
        }

        private void numericAngle_ValueChanged(object sender, EventArgs e)
        {
            textAngle.Text = numericAngle.Value.ToString();
        }


       

       

       
    }
}