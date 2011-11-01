using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.MapControl;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using System.IO;

namespace CoastalGIS.CallMap
{
    public partial class frmOpenMapByCoordinate : System.Windows.Forms.Form
    {

        private static int btnNextCNum = 0;
        private IPoint pPnt = new PointClass();
        private IMapControlDefault pMapControl = null;
        INewPolygonFeedback pNPolFeback;
        IActiveView pActivew;
        IScreenDisplay pScreen = new ScreenDisplayClass();

        public frmOpenMapByCoordinate(IMapControlDefault aMap)
        {
            InitializeComponent();
            pMapControl = aMap;
        }


        private void frmCopenMap_Load(object sender, EventArgs e)
        {
            txtXcood.Text = "";
            txtYCood.Text = "";
            //this.AcceptButton = this.btnOpnMap;
            this.txtEdCdX.Visible = false;
            this.txtEdCdY.Visible = false;
            this.lstVwCood.Items.Clear();
        }



        private void btnEnd_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        

        private bool IsNum(string strCood)
        {
            int len;
            bool IsNumber = true;
            len = strCood.Length;
            for (int i = 0; i < len; )
            {
                if ((Convert.ToByte(strCood[i]) >= '0') & (Convert.ToByte(strCood[i]) <= '9') || (strCood[i] == '.'))
                {
                    i++;
                }
                else
                {
                    IsNumber = false;
                    break;
                }
            }

            return IsNumber;
        }

        private void btnAdCood_Click(object sender, EventArgs e)
        {
            string txtCoodX, txtCoodY;
            txtCoodX = txtXcood.Text;
            txtCoodY = txtYCood.Text;
            string[] lstVwItm = new string[3];
            ListViewItem[] items = new ListViewItem[20];

            int lstVwNum;
            lstVwNum = lstVwCood.Items.Count;
            int colmNum;
            colmNum = lstVwCood.Columns.Count;
            if (txtCoodX == "" || txtCoodY == "")
            {
                MessageBox.Show("坐标输入错误,请重新输入");
                txtXcood.Text = "";
                txtYCood.Text = "";
            }
            else if (IsNum(txtCoodX) == false || IsNum(txtCoodY) == false)
            {
                MessageBox.Show("坐标输入错误,请重新输入");
                txtXcood.Text = "";
                txtYCood.Text = "";
            }
            else
            {
                lstVwItm[0] = (lstVwNum+1).ToString();
                lstVwItm[1] = txtCoodX;
                lstVwItm[2] = txtCoodY;
                items[lstVwNum] = new ListViewItem(lstVwItm, 0);
                this.lstVwCood.Items.AddRange(new ListViewItem[] { items[lstVwNum] });
                txtXcood.Text = "";
                txtYCood.Text = "";
            }
        }


        private void txtEdCdX_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode.ToString() == "Return")
            {
                if (txtEdCdX.Text == "")
                {
                    MessageBox.Show("坐标输入错误,请重新输入");
                    txtEdCdX.Text = lstVwCood.FocusedItem.SubItems[1].Text;
                    //txtEdCdY.Text = lstVwCood.FocusedItem.SubItems[2].Text;
                }
                else if (IsNum(txtEdCdX.Text) == false)
                {
                    MessageBox.Show("坐标输入错误,请重新输入！");
                    txtEdCdX.Text = lstVwCood.FocusedItem.SubItems[1].Text;
                    //txtEdCdY.Text = lstVwCood.FocusedItem.SubItems[2].Text;
                }
                else
                {
                    lstVwCood.FocusedItem.SubItems[1].Text = txtEdCdX.Text;
                    //lstVwCood.FocusedItem.SubItems[2].Text = txtEdCdY.Text;
                    txtEdCdX.Visible = false;
                    //txtEdCdY.Visible = true;
                }

            }
        }


        private void txtEdCdY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Return")
            {
                if (txtEdCdY.Text == "")
                {
                    MessageBox.Show("坐标输入错误,请重新输入");
                    //txtEdCdX.Text = lstVwCood.FocusedItem.SubItems[1].Text;
                    txtEdCdY.Text = lstVwCood.FocusedItem.SubItems[2].Text;
                }
                else if (IsNum(txtEdCdY.Text) == false)
                {
                    MessageBox.Show("坐标输入错误,请重新输入");
                    //txtEdCdX.Text = lstVwCood.FocusedItem.SubItems[1].Text;
                    txtEdCdY.Text = lstVwCood.FocusedItem.SubItems[2].Text;
                }
                else
                {
                    //lstVwCood.FocusedItem.SubItems[1].Text = txtEdCdX.Text;
                    lstVwCood.FocusedItem.SubItems[2].Text = txtEdCdY.Text;
                    //txtEdCdX.Visible = false;
                    txtEdCdY.Visible = false;
                }

            }
        }

        private void btnDetCood_Click(object sender, EventArgs e)
        {
            int curNum;
            int lstVwCnt = this.lstVwCood.Items.Count;
            ListViewItem lselItem;
            lselItem = lstVwCood.FocusedItem;
            if (this.lstVwCood.FocusedItem == null)
            {
                MessageBox.Show("请选择要删除的坐标！");
            }
            else
            {
                curNum = lstVwCood.FocusedItem.Index + 1;
                if (curNum == lstVwCnt)
                {
                    lstVwCood.Items.Remove(lselItem);
                }
                else
                {
                    for (int i = curNum - 1; i < lstVwCnt - 1; i++)
                    {
                        //lstVwCood.Items[i].SubItems[0].Text = lstVwCood.Items[i + 1].SubItems[0].Text;
                        lstVwCood.Items[i].SubItems[1].Text = lstVwCood.Items[i + 1].SubItems[1].Text;
                        lstVwCood.Items[i].SubItems[2].Text = lstVwCood.Items[i + 1].SubItems[2].Text;
                    }
                    ListViewItem lstLast = lstVwCood.Items[lstVwCnt - 1];
                    lstVwCood.Items.Remove(lstLast);

                }
            }
        }

        private void lstVwCood_MouseHover(object sender, EventArgs e)
        {
            lstVwCood.Activation = ItemActivation.OneClick;
            lstVwCood.FullRowSelect = true;
        }



        private void lstVwCood_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.txtEdCdX.Left = lstVwCood.Left + 70;
            this.txtEdCdX.Top = 57 + lstVwCood.FocusedItem.Position.Y;
            this.txtEdCdX.Text = lstVwCood.FocusedItem.SubItems[1].Text;
            this.txtEdCdY.Left = lstVwCood.Left + 180;
            this.txtEdCdY.Top = 57 + lstVwCood.FocusedItem.Position.Y;
            this.txtEdCdY.Text = lstVwCood.FocusedItem.SubItems[2].Text;
            this.txtEdCdX.Visible = true;
            this.txtEdCdY.Visible = true;
        }

        private void btnOpnMap_Click(object sender, EventArgs e)
        {
            double dobX, dobY;
            string txtCoodX, txtCoodY;
            if (lstVwCood.Items.Count < 3)
            {
                MessageBox.Show("请输入三个以上的点！");
                return;
            }
            else
            {
                for (int i = 0; i < lstVwCood.Items.Count; i++)
                {
                    txtCoodX = lstVwCood.Items[i].SubItems[1].Text;
                    txtCoodY = lstVwCood.Items[i].SubItems[2].Text;
                    dobX = Convert.ToDouble(txtCoodX);
                    dobY = Convert.ToDouble(txtCoodY);
                    pPnt.PutCoords(dobX, dobY);
                    if (pNPolFeback == null)
                    {
                        pNPolFeback = new NewPolygonFeedbackClass();
                        ISimpleLineSymbol pSLnSym;
                        IRgbColor pRGB = new RgbColorClass();
                        pSLnSym = (ISimpleLineSymbol)pNPolFeback.Symbol;
                        pRGB.Red = 140;
                        pRGB.Green = 140;
                        pRGB.Blue = 255;
                        pSLnSym.Color = pRGB;
                        pSLnSym.Style = esriSimpleLineStyle.esriSLSSolid;
                        pSLnSym.Width = 2;
                        pNPolFeback.Display = pScreen;
                        pNPolFeback.Start(pPnt);
                    }
                    else
                    {
                        pNPolFeback.AddPoint(pPnt);
                    }
                }
                pActivew = pMapControl.ActiveView;
                pScreen = pActivew.ScreenDisplay;
                IGeometry pGeomLn;
                pGeomLn = (IGeometry)pNPolFeback.Stop();
                IMap pMap = pMapControl.Map;

                ISpatialReference spatialReference = pMap.SpatialReference;
                pGeomLn.SpatialReference = spatialReference;
                IBorder pBorder = new SymbolBorderClass();
                LayerControl.LyrPolygon = pGeomLn;
                pMapControl.Map.ClipBorder = pBorder;
                pMapControl.Map.ClipGeometry = pGeomLn;
                pActivew.Extent = pGeomLn.Envelope;
                //pActivew.Refresh();
                this.DialogResult = DialogResult.OK;
                ////打开地形图图层

                 //layVisbleExceptMap();
                this.Close();
                pActivew.Refresh();
            }
         }

        private void layVisbleExceptMap()
        {
            ICompositeLayer2 pGroupLayer;
            IGroupLayer pGLayer;
            for (int m = 0; m < pMapControl.Map.LayerCount; m++)
            {

                pGroupLayer = (ICompositeLayer2)pMapControl.Map.get_Layer(m);
                pGLayer = (IGroupLayer)pGroupLayer;
                //if (pGLayer.Name.ToString().Trim() != global.CurIndex.ToString().Trim())
                //{
                //    int sSubLayerCount = pGroupLayer.Count;
                //    for (int g = 0; g < sSubLayerCount; g++)
                //    {
                //        IFeatureLayer nFeatureLayer;
                //        nFeatureLayer = (IFeatureLayer)pGroupLayer.get_Layer(g);
                //        nFeatureLayer.Visible = true;
                //        nFeatureLayer.Selectable = true;
                //    }

                //}

            }
            //global.CurIndexLayer.Selectable = false;
            //global.CurIndexLayer.Visible = false;
            //global.CurIndexLayerNo.Selectable = false;
            //global.CurIndexLayerNo.Visible = false;

        }

        private void btnSvGrpCood_Click(object sender, EventArgs e)
        {
            string FILE_NAME;
            string strId, strXcd, strYcd;
            SaveFileDialog saveDiag = new SaveFileDialog();
            saveDiag.InitialDirectory = "D:\\";
            saveDiag.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveDiag.RestoreDirectory = true;
            //saveDiag.ShowDialog();

            if (lstVwCood.Items.Count <= 0)
            {
                MessageBox.Show("坐标组不能为空！","坐标调图",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            if (saveDiag .ShowDialog() ==DialogResult .OK )
            {
                FILE_NAME = saveDiag.FileName.ToString().Trim();
                using (StreamWriter sw = File.CreateText(FILE_NAME))
                {
                    for (int i = 0; i < lstVwCood.Items.Count; i++)
                    {
                        strId = lstVwCood.Items[i].SubItems[0].Text;
                        strXcd = lstVwCood.Items[i].SubItems[1].Text;
                        strYcd = lstVwCood.Items[i].SubItems[2].Text;
                        sw.WriteLine(strId + "," + strXcd + "," + strYcd);
                    }
                    sw.Close();  
                }
            }
            //if (saveDiag.ShowDialog() == DialogResult.Ignore)
            //{
            //    return;
            //}

        }

        private void btnOpGrpCod_Click(object sender, EventArgs e)
        {
            lstVwCood.Items.Clear();
            ListViewItem[] items = new ListViewItem[20];
            string FILE_NAME;
            string strRdLine;
            OpenFileDialog openDiag = new OpenFileDialog();
            openDiag.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            openDiag.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openDiag.RestoreDirectory = true;
            //openDiag.ShowDialog();
            
            if (openDiag.ShowDialog() == DialogResult.OK)
            {
                FILE_NAME = openDiag.FileName;
                if (!File.Exists(FILE_NAME))
                {
                    MessageBox.Show("不存在此文件");
                    return;
                }
                else
                {
                    using (StreamReader sr = File.OpenText(FILE_NAME))
                    {
                        strRdLine = sr.ReadLine().Trim();
                        while (strRdLine != null)
                        {
                            int i = lstVwCood.Items.Count;
                            string[] strLs = new string[3];
                            strLs = strRdLine.Split(',');
                            items[i] = new ListViewItem(strLs, 0);
                            this.lstVwCood.Items.AddRange(new ListViewItem[] { items[i] });
                            strRdLine = sr.ReadLine();
                        }
                        sr.Close();
                    }
                }
                
            }
            
           
        }
          
    }
}