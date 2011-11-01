using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;

namespace CoastalGIS.MainGIS
{
    public partial class QueryForm : Form
    {
        //private frmMain ParentForm;
        private IMap pMap;
        private IMapControlDefault m_mapControl = null;
        private IFeatureClass m_feaFC;


        public QueryForm(IMapControlDefault mapControl)
        {
            InitializeComponent();
            pMap = mapControl.ActiveView.FocusMap;
            m_mapControl = mapControl;
        }

        

        
       
        

        private void QueryForm_Load(object sender, EventArgs e)
        {
            if (pMap.LayerCount == 0)
            {
                this.comboBox1.Text = "";
                return;
            }
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                ILayer pLayer = pMap.get_Layer(i);
                this.comboBox1.Items.Add(pLayer.Name);
            }
            ILayer pLayer1 = pMap.get_Layer(0);
            this.comboBox1.Text = pLayer1.Name;
            this.QueryText.Text = "";
            

            if (this.FieldListbox.Items.Count > 0)
            {
                this.FieldListbox.SelectedIndex = 0;
            }
            
        }

        /// <summary>
        /// 文本框中添加field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FieldListbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (FieldListbox.Items.Count != 0 && FieldListbox.SelectedItem != null)
            {
                //QueryText.Text = QueryText.Text + FieldListbox.Text;
                string FieldName = FieldListbox.SelectedItem.ToString();
                this.QueryText.Text += FieldName ;
            }
        }

        /// <summary>
        /// 符号写入text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    
        #region
        private void btn1_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn1.Text;
        }


        private void btn2_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn2.Text;
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn3.Text;
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn4.Text;
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn5.Text;
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn6.Text;
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn7.Text;
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn8.Text;
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn9.Text;
        }

        private void btn10_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn10.Text;
        }

        private void btn11_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn11.Text;
        }

        private void btn12_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn12.Text;
        }

        private void btn13_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn13.Text;
        }

        private void btn14_Click(object sender, EventArgs e)
        {
            QueryText.Text = QueryText.Text + btn14.Text;
        }
        #endregion

        private void FieldListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if(FieldListbox.SelectedIndex >= 0 )
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.FieldListbox.Items.Clear();
            int LayerNum = this.comboBox1.SelectedIndex;
            ILayer pLayer = pMap.get_Layer(LayerNum);
            if (pLayer is IFeatureLayer)
            {
                IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                IFields pFields = pFeatureLayer.FeatureClass.Fields;
                m_feaFC = pFeatureLayer.FeatureClass;
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    this.FieldListbox.Items.Add(pFields.get_Field(i).Name);
                }
            }

            //this.FieldListbox.SelectedIndex = 0;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            QueryText.Text = "";
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string WhereClouse = QueryText.Text;
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                
                ILayer pLayer = pMap.get_Layer(i);
                if (pLayer.Name == comboBox1.Text)
                {
                    IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                    SearchFeature(WhereClouse, pFeatureLayer);
                    break;
                }
            }
            this.Dispose();
        }

        private void SearchFeature(string sqlFilter, IFeatureLayer pFeatureLyr)
        {
            IFeatureLayer pFeatureLayer;
            pFeatureLayer = pFeatureLyr;
            IQueryFilter pFilter;
            pFilter = new QueryFilterClass();
            pFilter.WhereClause = sqlFilter;

           // IFeatureCursor pFeatureCursor;
            IFeatureSelection pFtSelection = pFeatureLyr as IFeatureSelection;
            pFtSelection.SelectFeatures(pFilter, esriSelectionResultEnum.esriSelectionResultNew , false);
            this.m_mapControl.ActiveView.Refresh();
            //pFeatureCursor = pFeatureLayer.Search(pFilter, true);
            //IFeature pFeature;
            //pFeature = pFeatureCursor.NextFeature();

            /*while (pFeature != null)
            {
                
                //if (pFeature != null)
                //{
                ISimpleFillSymbol pFillsyl;
                pFillsyl = new SimpleFillSymbolClass();
                pFillsyl.Color = getRGB(220, 100, 50);
                object oFillsyl;
                oFillsyl = pFillsyl;
                IPolygon pPolygon;
                pPolygon = pFeature.Shape as IPolygon;

                //ParentForm.mapCtlMain.FlashShape(pPolygon, 15, 20, pFillsyl);

                //ParentForm.mapCtlMain.DrawShape(pPolygon, ref oFillsyl);
                switch (pFeature.Shape.GeometryType)
                /*{
                    case esriGeometryType.esriGeometryPoint:
                        FlashFeature.FlashPoint(this.m_mapControl, this.m_mapControl.ActiveView.ScreenDisplay, pFeature.Shape);
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        FlashFeature.FlashLine(this.m_mapControl, this.m_mapControl.ActiveView.ScreenDisplay, pFeature.Shape);
                        break;
                    case esriGeometryType.esriGeometryPolygon:
                        FlashFeature.FlashPolygon(this.m_mapControl, this.m_mapControl.ActiveView.ScreenDisplay, pFeature.Shape);
                        break;
                    default:
                        break;
                }*/

               // pFeature = pFeatureCursor.NextFeature();
                //}
            


            //ParentForm.mapCtlMain.Refresh();
 
        }

        private IRgbColor getRGB(int r, int g, int b)
        {
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pColor.Red = r;
            pColor.Green = g;
            pColor.Blue = b;
            return pColor;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            string WhereClouse = QueryText.Text;
            for (int i = 0; i < pMap.LayerCount; i++)
            {

                ILayer pLayer = pMap.get_Layer(i);
                if (pLayer.Name == comboBox1.Text)
                {
                    IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                    SearchFeature(WhereClouse, pFeatureLayer);
                    break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            IFeatureCursor pCursor = m_feaFC.Search(null, false);
            IDataStatistics pData = new  DataStatisticsClass();
            pData.Field = this.FieldListbox.SelectedItem.ToString() ;
            pData.Cursor = pCursor  as  ICursor;
            IEnumerator pEnumVar = pData.UniqueValues;
            IField pFied = m_feaFC.Fields.get_Field(this.FieldListbox.SelectedIndex);

            string text;

            if (pFied.Type == esriFieldType.esriFieldTypeOID || pFied.Type == esriFieldType.esriFieldTypeDouble || pFied.Type == esriFieldType.esriFieldTypeInteger)
            {
                text = "";
            }
            else 
            {
                text = "'";
            }
            while (pEnumVar.MoveNext())
            {
                this.listBox1.Items.Add(text + pEnumVar.Current.ToString() + text);
            }  

        }

        private void FieldListbox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            this.button1.Enabled = true;
            this.listBox1.Items.Clear();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.Items.Count != 0 && listBox1.SelectedItem != null)
            {
                //QueryText.Text = QueryText.Text + FieldListbox.Text;
                string FieldName = listBox1.SelectedItem.ToString();
                this.QueryText.Text += FieldName;
            }
        }





    }
}