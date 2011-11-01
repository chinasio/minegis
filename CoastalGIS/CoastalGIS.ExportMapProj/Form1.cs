using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;

using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;


namespace CoastalGIS.ExportMapProj
{
    public partial class frmLegendWizard : Form
    {
        private IPageLayoutControlDefault m_pageLayoutControl=null ;
       
        private ILegend  m_legend=null ;
       // ILegend2 m_legend2 = null;
        private IMapControlDefault m_mapControl; 
        private IMapLayers m_mapLayers;
        private IEnumLayer m_emuLayer;//添加图层信息的
        private ILegendFormat plegendFormat;//图例格式设置，如间距，标题样式等
        private  IMapSurroundFrame mapSurroundFrame; //获取图例，添加图例；
        private ITextSymbol pTextSymbol;//图例项文字字体；
        private ITextElement pTitleElement;//图例标题的字体样式；
         private  IMapSurroundFrame pMapSurrounFrame;  
        private IMap m_Map;
        private IFrameProperties m_FrameProperties;
        private  ISymbolBackground m_SymbolBackground ;//背景
        private ISymbolBorder m_SymbolBorder;//边框       
        private ISymbolShadow m_SymbolShadow;//阴影
       
        public frmLegendWizard(IMap pmap,IPageLayoutControlDefault  pPageLayoutControl,IMapSurroundFrame pMapSurroundFrame)
        {//用于在 任意范围打印的对话框 中编辑图例
            InitializeComponent();
            mapSurroundFrame  = pMapSurroundFrame;
            m_pageLayoutControl = pPageLayoutControl;
            m_Map = pmap;

        }
        public frmLegendWizard(IMapControlDefault mapControl, IPageLayoutControlDefault pagelayoutControl)
        {
            InitializeComponent();
            m_mapControl = mapControl;
            m_pageLayoutControl = pagelayoutControl;
         //   m_legend = pLegend;
         
        }
        private void LegendWizard_Load(object sender, EventArgs e)
        {
        
            if (m_pageLayoutControl.ActiveView.FocusMap == null)
            {
                MessageBox.Show("请先添加地图");
                return;

            }
            #region //点击 添加图例按钮 或在 PageLayoutControl中双击图例时
            if (mapSurroundFrame == null)  //当点击 添加图例按钮 或在 PageLayoutControl中双击图例时 先创建图例 或从地图上获得已经添加的图例；
            {

                //Get the GraphicsContainer
                IGraphicsContainer graphicsContainer = m_pageLayoutControl.GraphicsContainer;
                graphicsContainer.Reset();
                IElementProperties pElementPerties = graphicsContainer.Next() as IElementProperties;
                //Get the MapFrame
                IMapFrame mapFrame = (IMapFrame)graphicsContainer.FindFrame(m_pageLayoutControl.ActiveView.FocusMap);
                if (mapFrame == null) return;
                while (pElementPerties != null) //从地图中获得已有的图例
                {
                    if (pElementPerties.Type == "Map Surround Frame")
                    {
                        pMapSurrounFrame = pElementPerties as IMapSurroundFrame;
                        if (pMapSurrounFrame.MapSurround.Name == "Legend")
                        {
                            m_legend = pMapSurrounFrame.MapSurround as ILegend;
                            break;
                        }
                    }
                    pElementPerties = (IElementProperties)graphicsContainer.Next();
                }
                if (m_legend == null) //当地图中没有添加图例时 创建新图例
                {
                    //Create a legend
                    UID uID = new UIDClass();
                    uID.Value = "esriCarto.Legend";

                    //Create a MapSurroundFrame from the MapFrame
                    mapSurroundFrame = mapFrame.CreateSurroundFrame(uID, null);

                    if (mapSurroundFrame == null) return;
                    if (mapSurroundFrame.MapSurround == null) return;

                    m_legend = mapSurroundFrame.MapSurround as ILegend;
                }
                else
                {
                    mapSurroundFrame = pMapSurrounFrame;

                }

               

                UID m_uid = new UIDClass();
                m_uid.Value = "{34C20002-4D3C-11D0-92D8-00805F7C28B0}";
                m_mapLayers = m_mapControl.Map as IMapLayers;
                m_emuLayer = m_mapLayers.get_Layers(m_uid, true);//获取图层
                if (m_emuLayer != null)
                {
                    m_emuLayer.Reset();
                    ILayer m_layer = null;
                    while ((m_layer = m_emuLayer.Next()) != null)
                    {
                        this.listBox1.Items.Add(m_layer.Name);//将图层名添加到图例备选项中
                    }
                }
            }
       #endregion
            else //在双击任意范围打印框中图例时，获得图例，及图层信息；
            {
                if (m_Map == null)
                {
                    MessageBox.Show("请先添加地图！");
                }
                for (int k = 0; k < m_Map.LayerCount; k++)
                {
                    listBox1.Items.Add(m_Map.get_Layer(k).Name.ToString());
                }
                m_legend = mapSurroundFrame .MapSurround as ILegend;
            }
            //以下设置需要用到的变量 及初始化 对话框
            plegendFormat = m_legend.Format as ILegendFormat; //设置图例格式需要用到的

            pTextSymbol = new TextSymbolClass();
            pTextSymbol.Size = 10;
            pTextSymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHALeft; //初始化图例标签的文本格式

            if (m_legend.Title != null)
                textBox1.Text = m_legend.Title.ToString(); //初始化对话框中的文本框 文本
            else
                textBox1.Text = "Legend";

            this.textBoxWidth.Text = "36"; 
            this.textBoxHeight.Text = "15";
            IMapSurround mapSurround = m_legend as IMapSurround;
            mapSurroundFrame.MapSurround = mapSurround;
            m_FrameProperties = (IFrameProperties)mapSurroundFrame;
            ISymbolBackground pSymbolBack = m_FrameProperties.Background as ISymbolBackground;
            ISymbolBorder pSymbolBorder = m_FrameProperties.Border as ISymbolBorder;
            ISymbolShadow pSymbolShadow = m_FrameProperties.Shadow as ISymbolShadow;
            if (pSymbolBack !=null )
            this.btnBackGroundColor.BackColor =ColorTranslator.FromOle ( pSymbolBack.FillSymbol.Color.RGB ) ;
            if (pSymbolBorder != null)
            this.btnBolderColor.BackColor = ColorTranslator.FromOle(pSymbolBorder.LineSymbol.Color.RGB);
             if (pSymbolShadow != null)
            this.btnShadowColor.BackColor = ColorTranslator.FromOle(pSymbolShadow.FillSymbol.Color.RGB);

        }

        private void btnOneToR_Click(object sender, EventArgs e)
        {
            int index; //记录ListBox中选中项的编号
            if (this.listBox1.SelectedItem != null)
            {
                index = listBox1.SelectedIndex;
                this.listBox2.Items.Add(this.listBox1.SelectedItem);
                listBox1.Items.RemoveAt(index);
            }
        }

        private void btnAllToR_Click(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count != 0)
            {
                foreach (object item in listBox1.Items)
                {
                    this.listBox2.Items.Add(item);
                }
                this.listBox1.Items.Clear();
            }

        }

        private void btnOneToL_Click(object sender, EventArgs e)
        {
            int index;
            if (this.listBox2.SelectedItem != null)
            {
                index = listBox2.SelectedIndex;
                this.listBox1.Items.Add(this.listBox1.SelectedItem);
                listBox2.Items.RemoveAt(index);
            }
        }

        private void btnAllToL_Click(object sender, EventArgs e) 
        {
            if (this.listBox2.Items.Count != 0)
            {
                foreach (object item in listBox2.Items)
                {
                    this.listBox1.Items.Add(item);
                }
                this.listBox2.Items.Clear();
            }

        }

        private void btnUp_Click(object sender, EventArgs e)
        { //将选中的项向上移动一位
            int index;
            int i=0;
            if (this.listBox2.SelectedItem != null)
            {
                if (this.listBox2.SelectedIndex != 0)
                {
                    index = this.listBox2.SelectedIndex;
                    object item = this.listBox2.SelectedItem;
                    object[] allItems = new object[this.listBox2.Items.Count];
                    foreach (object itm in listBox2.Items)//将所有的项复制到数组中
                    {
                        allItems[i] = itm;
                        i++;
                    }
                    listBox2.Items.Clear();
                    allItems[index] = allItems[index - 1];
                    allItems[index - 1] = item;
                    for (int j = 0; j < i; j++)
                    {
                        listBox2.Items.Add(allItems[j]);//将在数组中排好序的项存入ListBox2
                    }
                }
                else
                    return;
            }
            else 
            {
             MessageBox .Show("请选中要移动的项！");
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {//将选中的项向下移动一位
            int index;
            int i = 0;
            if (this.listBox2.SelectedItem != null)
            {
                if (this.listBox2.SelectedIndex != this.listBox2.Items.Count - 1)
                {
                    index = this.listBox2.SelectedIndex;
                    object item = this.listBox2.SelectedItem;
                    object[] allItems = new object[this.listBox2.Items.Count];
                    foreach (object itm in listBox2.Items)//将所有的项复制到数组中
                    {
                        allItems[i] = itm;
                        i++;
                    }
                    listBox2.Items.Clear();

                    allItems[index] = allItems[index + 1];
                    allItems[index + 1] = item;
                    for (int j = 0; j < i; j++)
                    {
                        listBox2.Items.Add(allItems[j]);//将在数组中排好序的项存入ListBox2
                    }
                }
                else
                    return;

            }
            else
            {
                MessageBox.Show("请选中要移动的项！");
            }
        }

        private void btnLegendSymbol_Click(object sender, EventArgs e)
        { //未设置成功？？？？？？？？？？？？？？
            frmLegendItemSymbol frmLengendSymbol = new frmLegendItemSymbol();
            ESRI.ArcGIS.Controls.esriSymbologyStyleClass pStyleClass = ESRI.ArcGIS.Controls.esriSymbologyStyleClass.esriStyleClassLegendItems;
           IStyleGalleryItem pStyleGalleryItem= frmLengendSymbol.GetItem(pStyleClass);
      
           
           // pTextSymbol = frmLengendSymbol.pTextSymbol;
          

        }

        private void btnTextSymbol_Click(object sender, EventArgs e)
        {
            ITextElement textElement = new TextElementClass();
            textElement.Symbol = pTextSymbol;
            textElement.Text = "图层名";
            frmTextSymbol frmText = new frmTextSymbol(ref textElement);
            frmText.ShowDialog();
            pTextSymbol = textElement.Symbol; //这里是设置标注的字体样式
         

        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            m_legend.Name = this.textBoxElement.Text.Trim();
            m_legend.Title = this.textBox1.Text; //图例的标题
            ILayer player;
            ILegendItem pLegendItem = null;
            #region //设置图例显示的项
            if (this.listBox2.Items.Count != 0)
            {
                m_legend.ClearItems();
                foreach (object item in this.listBox2 .Items ) //设置需要的图例项  根据用户需要添加
                {
                    for (int j = 0; j < m_pageLayoutControl.ActiveView.FocusMap.LayerCount; j++)
                    {
                        player = m_pageLayoutControl.ActiveView.FocusMap.get_Layer(j);
                        if (player.Name == item .ToString ())
                        {

                             IFeatureLayer pFLayer = player as IFeatureLayer; 
                             IFeatureClass pFeatureClass = pFLayer.FeatureClass; 
                           if (pFeatureClass.FeatureType == esriFeatureType.esriFTAnnotation) 
                             { 
                                continue; 
                              } 
                              else 
                               { 
                              pLegendItem = new HorizontalLegendItemClass(); //图例项标签的样式    
                              pLegendItem.Layer = player; 
                              pLegendItem.Columns = 1; 
                              pLegendItem.ShowDescriptions = false; 
                              pLegendItem.ShowHeading = false; 
                              pLegendItem.ShowLabels = true;
                              switch (comboBox1.SelectedItem.ToString().Trim () )
                              { 
                                  case "所有文本符号":
                                      pLegendItem.LayerNameSymbol = pTextSymbol; //设置字体样式
                                      pLegendItem.HeadingSymbol = pTextSymbol;
                                      pLegendItem.LegendClassFormat.LabelSymbol = pTextSymbol;
                                      break;
                                  case "所有图层名称":
                                      pLegendItem.LayerNameSymbol = pTextSymbol;
                                      break;
                                  case "所有标题项":
                                      pLegendItem.HeadingSymbol = pTextSymbol;
                                      break;
                                  case "所有标注项":
                                      pLegendItem.LegendClassFormat.LabelSymbol = pTextSymbol;
                                      break;
                                  case "所有描述性文字":
                                      break;
                                 
                              }
                
                              } 
                            m_legend.AddItem(pLegendItem );
                        }
                    }
                
                }
            }
            #endregion
            plegendFormat.DefaultPatchHeight = Convert.ToDouble(this.textBoxHeight.Text);//块的高度
            plegendFormat.DefaultPatchWidth = Convert.ToDouble(this.textBoxWidth.Text);//块的宽度


            #region //设置图例的底色边框
            IMapSurround pmapsurronud = m_legend as IMapSurround;
            mapSurroundFrame.MapSurround = pmapsurronud;
            m_FrameProperties = (IFrameProperties)mapSurroundFrame;
           // m_SymbolBackground.FillSymbol.Color = ConvertColorToIColor(this.btnBackGroundColor.BackColor); 
            m_FrameProperties.Background =(IBackground ) m_SymbolBackground;
           // m_SymbolBorder.LineSymbol.Color = ConvertColorToIColor(this.btnBolderColor.BackColor);
            m_FrameProperties.Border = (IBorder)m_SymbolBorder;
           // m_SymbolShadow.FillSymbol.Color = ConvertColorToIColor(this.btnShadowColor.BackColor);
            m_FrameProperties.Shadow = (IShadow)m_SymbolShadow;

            #endregion



                IEnvelope pEnvelop = new EnvelopeClass();  //图例的位置
                pEnvelop.PutCoords(Convert.ToDouble(this.textBoxX.Text), Convert.ToDouble(this.textBoxY.Text),
                Convert.ToDouble(this.textBoxX.Text) + Convert.ToDouble(this.textBoxWidth1.Text),
                Convert.ToDouble(this.textBoxY.Text) + Convert.ToDouble(this.textBoxHeight1.Text));
          
           
            IElement m_element = mapSurroundFrame   as IElement;
            m_element.Geometry = pEnvelop;
            m_pageLayoutControl.AddElement(m_element ,Type.Missing ,Type .Missing ,m_legend .Name ,0);
            m_pageLayoutControl.ActiveView.Refresh();
            this.Close();
        }

        private void btnSymbol_Click(object sender, EventArgs e)
        { //图例的大标题字体；
            m_legend.Title = this.textBox1.Text;
            plegendFormat = m_legend.Format;
            pTitleElement = new TextElementClass();
            pTitleElement.Text = m_legend.Title;
            pTitleElement.Symbol = plegendFormat.TitleSymbol;
            frmTextSymbol frmtext = new frmTextSymbol(ref pTitleElement);
            frmtext.ShowDialog();
            this.textBox1.Text = pTitleElement.Text;
            plegendFormat.TitleSymbol = pTitleElement.Symbol;
        }

        private void btnArea_Click(object sender, EventArgs e)
        {
            frmLegendItemSymbol frmAreaSymbol = new frmLegendItemSymbol();

            //Get the IStyleGalleryItem that has been selected in the SymbologyControl
          
            IStyleGalleryItem styleGalleryItem = frmAreaSymbol.GetItem(esriSymbologyStyleClass.esriStyleClassAreaPatches);
            //change the default area （面样式）patch
            plegendFormat.DefaultAreaPatch = (IAreaPatch)styleGalleryItem.Item;
            
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            frmLegendItemSymbol frmAreaSymbol = new frmLegendItemSymbol();
            //Get the IStyleGalleryItem that has been selected in the SymbologyControl
            IStyleGalleryItem styleGalleryItem = frmAreaSymbol.GetItem(esriSymbologyStyleClass.esriStyleClassLinePatches );
            //change the default line(线样式) patch
            if (styleGalleryItem != null)
                plegendFormat.DefaultLinePatch = (ILinePatch)styleGalleryItem.Item;
            else
                return;
        }

        private void btrChangBorderStlye_Click(object sender, EventArgs e)
        {  
            Color pColor;
            frmLegendItemSymbol frmBackColor = new frmLegendItemSymbol();
            IStyleGalleryItem pStyleGalleryItem = frmBackColor.GetItem(esriSymbologyStyleClass.esriStyleClassBackgrounds);
            if (pStyleGalleryItem != null)
            {
                m_SymbolBackground = (ISymbolBackground)pStyleGalleryItem.Item;
                pColor = this.ConvertIRgbColorToColor(((ISymbolBackground )pStyleGalleryItem.Item).FillSymbol .Color as IRgbColor);

            }
            else
            {
                m_SymbolBackground = new SymbolBackgroundClass();
                pColor = Color.White;
            }
            this.btnBackGroundColor.BackColor = pColor;
            IStyleGalleryClass pStyleGalleryClass = new BackgroundStyleGalleryClassClass();
          Bitmap pBitmap = StyleGalleryItemToBmp(this.pictureBox1.Width, this.pictureBox1.Height, pStyleGalleryClass, pStyleGalleryItem);
           this.pictureBox1.Image = pBitmap as Image;

        }
        public Color ConvertIRgbColorToColor(IRgbColor pRgbColor)
        {

            return ColorTranslator.FromOle(pRgbColor.RGB);

        }

        public IColor ConvertColorToIColor(Color color)
        {

            IColor pColor = new RgbColorClass();

            pColor.RGB = color.B * 65536 + color.G * 256 + color.R;

            return pColor;

        }

        private void btnChangeBCStyle_Click(object sender, EventArgs e)
        {
            Color pColor;
            frmLegendItemSymbol frmBolder = new frmLegendItemSymbol();
            IStyleGalleryItem pStyleGalleryItem = frmBolder.GetItem(esriSymbologyStyleClass.esriStyleClassBorders);
            if (pStyleGalleryItem != null)
            {
                m_SymbolBorder = (ISymbolBorder)pStyleGalleryItem.Item;
                pColor = this.ConvertIRgbColorToColor(((ISymbolBorder )pStyleGalleryItem.Item).LineSymbol .Color as IRgbColor);

            }
            else
            {
               m_SymbolBorder = new SymbolBorderClass ();
                pColor = Color.White;
            }
            this.btnBolderColor .BackColor = pColor;
            IStyleGalleryClass pStyleGalleryClass = new BorderStyleGalleryClassClass ();
            Bitmap pBitmap = StyleGalleryItemToBmp(this.pictureBox2.Width, this.pictureBox2.Height, pStyleGalleryClass, pStyleGalleryItem);
            this.pictureBox2.Image = pBitmap as Image;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Color pColor;
            frmLegendItemSymbol frmShadow = new frmLegendItemSymbol();
            IStyleGalleryItem pStyleGalleryItem = frmShadow.GetItem(esriSymbologyStyleClass.esriStyleClassShadows);
            if (pStyleGalleryItem != null)
            {
               m_SymbolShadow = (ISymbolShadow)pStyleGalleryItem.Item;
                pColor = this.ConvertIRgbColorToColor(((ISymbolShadow)pStyleGalleryItem.Item).FillSymbol.Color as IRgbColor);
            }
            else
            {
                m_SymbolShadow = new SymbolShadowClass();
                pColor = Color.White;
            }
            this.btnShadowColor .BackColor = pColor;
           IStyleGalleryClass pStyleGalleryClass = new ShadowStyleGalleryClassClass ();
            Bitmap pBitmap = StyleGalleryItemToBmp(this.pictureBox3.Width, this.pictureBox3.Height, pStyleGalleryClass, pStyleGalleryItem);
            this.pictureBox3.Image = pBitmap as Image;

        }

        public static Bitmap StyleGalleryItemToBmp(int pWidth, int pHeight, IStyleGalleryClass pStyleGalleryClass, IStyleGalleryItem pStyleGalleryItem)
        { ///  通过符号库中的IStyleGalleryItem 和 IStyleGalleryClass类别生成图片预览

            Bitmap bitmap = new Bitmap(pWidth, pHeight);
            System.Drawing.Graphics pGraphics = System.Drawing.Graphics.FromImage(bitmap);
            tagRECT rect = new tagRECT();
            rect.right = bitmap.Width;
            rect.bottom = bitmap.Height;
            //生成预览
            IntPtr hdc = new IntPtr();
            hdc = pGraphics.GetHdc();
            pStyleGalleryClass.Preview(pStyleGalleryItem.Item, hdc.ToInt32(), ref rect);
            pGraphics.ReleaseHdc(hdc);
            pGraphics.Dispose();
            return bitmap;
        }

     

      
    }
}