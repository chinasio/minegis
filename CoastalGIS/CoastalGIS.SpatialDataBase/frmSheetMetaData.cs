using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace CoastalGIS.SpatialDataBase
{
    public partial class frmSheetMetaData : Form
    {
        IWorkspace m_workSpace = null;
        GDBData m_gdata = null;
        OracleCommand m_oraCmd = null;
        string m_selectNode = "";

        TextBox[] m_textBox=null;
        Label[] m_label=null;

        IList<TextBox> m_textBoxActive = null;
        IList<Label> m_labelActive = null;
        IList<string> m_customField = null;

        bool m_isEdit = false;

        public frmSheetMetaData(IWorkspace workSpace,OracleCommand oraCmd)
        {
            m_textBoxActive = new List<TextBox>();
            m_labelActive = new List<Label>();
            m_customField = new List<string>();

            InitializeComponent();
            m_textBox = new TextBox[9];
            m_textBox[0] = textBox24;
            m_textBox[1] = textBox18;
            m_textBox[2] = textBox20;
            m_textBox[3] = textBox16;
            m_textBox[4] = textBox15;
            m_textBox[5] = textBox17;
            m_textBox[6] = textBox21;
            m_textBox[7] = textBox19;
            m_textBox[8] = textBox14;

            m_label = new Label[9];
            m_label[0] = label26;
            m_label[1] = label20;
            m_label[2] = label25;
            m_label[3] = label18;
            m_label[4] = label17;
            m_label[5] = label19;
            m_label[6] = label24;
            m_label[7] = label21;
            m_label[8] = label16;

            this.m_workSpace = workSpace;
            m_gdata = new GDBData(this.m_workSpace);
            m_oraCmd = oraCmd;
            InitCustom();
        }

        private void frmSheetMetaData_FormClosed(object sender, FormClosedEventArgs e)
        {
            ShowSDETree();
        }

        public void ShowSDETree() //空间数据库树的显示
        {

            if (this.treeVSDE.Nodes.Count > 0)
            {
                this.treeVSDE.Nodes.Clear();
            }

            TreeNode firstNode = new TreeNode("江苏沿海空间数据库");
            firstNode.ImageIndex = 32;
            firstNode.SelectedImageIndex = 32;
            firstNode.Tag = "DB";
            this.treeVSDE.Nodes.Add(firstNode);

            TreeNode secondNodeSHP = new TreeNode("矢量数据");
            secondNodeSHP.ImageIndex = 31;
            secondNodeSHP.SelectedImageIndex = 31;
            secondNodeSHP.Tag = "VectorDB";
            firstNode.Nodes.Add(secondNodeSHP);

            TreeNode secondNodeRaster = new TreeNode("影像数据");
            secondNodeRaster.ImageIndex = 31;
            secondNodeRaster.SelectedImageIndex = 31;
            secondNodeRaster.Tag = "RasterDB";
            firstNode.Nodes.Add(secondNodeRaster);
            string sqlText = "select * from SPATIALDATASET";
            this.m_oraCmd.CommandText = sqlText;
            OracleDataReader drDS = m_oraCmd.ExecuteReader();
            OracleDataReader drSheet;
            while (drDS.Read())
            {
                //MessageBox.Show(drDS.GetValue(1).ToString());
                switch (drDS.GetValue(1).ToString())
                {
                    case "矢量":
                        TreeNode thirdNode = new TreeNode(drDS.GetValue(0).ToString());
                        thirdNode.ImageIndex = 36;
                        thirdNode.SelectedImageIndex = 36;
                        thirdNode.Tag = "FeatureDataSet";
                        secondNodeSHP.Nodes.Add(thirdNode);
                        sqlText = "select name,shapetype from SHEETMETADATA where dataset='" + drDS.GetValue(0).ToString() + "'";
                        this.m_oraCmd.CommandText = sqlText;
                        drSheet = this.m_oraCmd.ExecuteReader();
                        while (drSheet.Read())
                        {
                            TreeNode forthNode = new TreeNode(drSheet.GetValue(0).ToString());
                            switch (drSheet.GetValue(1).ToString())
                            {
                                case "Point":
                                    forthNode.ImageIndex = 33;
                                    forthNode.SelectedImageIndex = 33;
                                    forthNode.Tag = "Point";
                                    break;
                                case "Polyline":
                                    forthNode.ImageIndex = 35;
                                    forthNode.SelectedImageIndex = 35;
                                    forthNode.Tag = "Line";
                                    break;
                                case "Polygon":
                                    forthNode.ImageIndex = 34;
                                    forthNode.SelectedImageIndex = 34;
                                    forthNode.Tag = "Polygon";
                                    break;
                                case "Annotation":
                                    forthNode.ImageIndex = 85;
                                    forthNode.SelectedImageIndex = 85;
                                    forthNode.Tag = "Annotation";
                                    break;
                                case "Else":
                                    forthNode.ImageIndex = -1;
                                    forthNode.SelectedImageIndex = -1;
                                    forthNode.Tag = "Else";
                                    break;
                            }
                            thirdNode.Nodes.Add(forthNode);
                        }
                        break;

                    case "影像":
                        thirdNode = new TreeNode(drDS.GetValue(0).ToString());
                        thirdNode.ImageIndex = 30;
                        thirdNode.SelectedImageIndex = 30;
                        thirdNode.Tag = "RasterDataSet";
                        secondNodeRaster.Nodes.Add(thirdNode);
                        sqlText = "select name,shapetype from SHEETMETADATA where dataset='" + drDS.GetValue(0).ToString() + "'";
                        this.m_oraCmd.CommandText = sqlText;
                        drSheet = this.m_oraCmd.ExecuteReader();
                        while (drSheet.Read())
                        {
                            TreeNode forthNode = new TreeNode(drSheet.GetValue(0).ToString());
                            forthNode.ImageIndex = 30;
                            forthNode.SelectedImageIndex = 30;
                            forthNode.Tag = "Raster";
                            thirdNode.Nodes.Add(forthNode);
                        }
                        break;
                }

            }
            this.treeVSDE.ExpandAll();
        }

        private void frmSheetMetaData_Load(object sender, EventArgs e)
        {
            ShowSDETree();
        }

        private void uiGroupBox1_Click(object sender, EventArgs e)
        {

        }

        private void treeVSDE_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (this.m_isEdit == true) 
            {
                MessageBox.Show("请先保存修改！");
                return;
            }
            string sqlText = "select *from SHEETMETADATA where NAME='" + e.Node.Text + "'";
            m_oraCmd.CommandText = sqlText;
            OracleDataReader dr = m_oraCmd.ExecuteReader();
            if (dr.Read())
            {
                this.textBox1.Text = dr.GetValue(0).ToString();
                this.textBox5.Text = dr.GetValue(2).ToString();
                this.textBox7.Text = dr.GetValue(1).ToString();
                this.textBox2.Text = dr.GetValue(4).ToString();
                this.textBox3.Text = dr.GetValue(3).ToString();
                this.textBox4.Text = dr.GetValue(7).ToString();
                this.textBox6.Text = dr.GetValue(6).ToString();
                this.richTextBox1.Text = dr.GetValue(5).ToString();

                this.textBox9.Text = dr.GetValue(6).ToString();
                this.textBox10.Text = dr.GetValue(9).ToString();
                this.textBox8.Text = dr.GetValue(11).ToString();
                this.textBox13.Text = dr.GetValue(12).ToString();
                this.textBox12.Text = dr.GetValue(13).ToString();

                this.m_selectNode = e.Node.Text;
            }
            else 
            {
                this.textBox1.Text = "";
                this.textBox5.Text = "";
                this.textBox7.Text = "";
                this.textBox2.Text = "";
                this.textBox3.Text = "";
                this.textBox4.Text = "";
                this.textBox6.Text = "";
                this.richTextBox1.Text = "";

                this.textBox9.Text = "";
                this.textBox10.Text = "";
                this.textBox8.Text = "";
                this.textBox13.Text = "";
                this.textBox12.Text = "";
                this.m_selectNode = "";
            }

            sqlText = "select *from CUSTOMMETADATA where NAME='" + e.Node.Text + "'";
            m_oraCmd.CommandText = sqlText;
            dr = m_oraCmd.ExecuteReader();
            while (dr.Read()) 
            {
                for(int i = 0;i<this.m_textBoxActive.Count;i++)
                {
                    this.m_textBoxActive[i].Text = dr.GetValue(2 + i).ToString();
                }

            }

        }

        private void explorerBar1_ItemClick(object sender, Janus.Windows.ExplorerBar.ItemEventArgs e)
        {
            switch (e.Item.Text.ToString())
            {
                case "修改":
                    if (this.m_selectNode == "")
                    {
                        MessageBox.Show("请先选择数据！");
                        return;
                    }
                    this.m_isEdit = true;
                    this.richTextBox1.ReadOnly = false;
                    this.textBox3.ReadOnly = false;
                    this.textBox6.ReadOnly = false;
                    this.textBox4.ReadOnly = false;
                    this.textBox9.ReadOnly = false;
                    this.textBox10.ReadOnly = false;
                    this.textBox8.ReadOnly = false;
                    this.textBox13.ReadOnly = false;
                    this.textBox12.ReadOnly = false;

                    this.richTextBox1.BackColor = Color.White;
                    this.textBox3.BackColor = Color.White;
                    this.textBox6.BackColor = Color.White;
                    this.textBox4.BackColor = Color.White;
                    this.textBox9.BackColor = Color.White;
                    this.textBox10.BackColor = Color.White;
                    this.textBox8.BackColor = Color.White;
                    this.textBox13.BackColor = Color.White;
                    this.textBox12.BackColor = Color.White;

                    for (int i = 0; i < m_labelActive.Count; i++)
                    {
                        m_textBoxActive[i].ReadOnly = false;
                        m_textBoxActive[i].BackColor = Color.White;
                    }
                    break;

                case "保存":
                    if (this.m_selectNode == "")
                    {
                        MessageBox.Show("请先选择数据！");
                        return;
                    }
                    if (this.m_isEdit == false) 
                    {
                        MessageBox.Show("请先修改数据！");
                        return;
                    }
                    this.richTextBox1.ReadOnly = true;
                    this.textBox3.ReadOnly = true;
                    this.textBox6.ReadOnly = true;
                    this.textBox4.ReadOnly = true;
                    this.textBox9.ReadOnly = true;
                    this.textBox10.ReadOnly = true;
                    this.textBox8.ReadOnly = true;
                    this.textBox13.ReadOnly = true;
                    this.textBox12.ReadOnly = true;

                    this.richTextBox1.BackColor = Color.AliceBlue;
                    this.textBox3.BackColor = Color.AliceBlue;
                    this.textBox6.BackColor = Color.AliceBlue;
                    this.textBox4.BackColor = Color.AliceBlue;
                    this.textBox9.BackColor = Color.AliceBlue;
                    this.textBox10.BackColor = Color.AliceBlue;
                    this.textBox8.BackColor = Color.AliceBlue;
                    this.textBox13.BackColor = Color.AliceBlue;
                    this.textBox12.BackColor = Color.AliceBlue;

                    for (int i = 0; i < m_labelActive.Count; i++)
                    {
                        m_textBoxActive[i].ReadOnly = false;
                        m_textBoxActive[i].BackColor = Color.AliceBlue;
                    }

                    string sqlText = "update SHEETMETADATA set SOURCEFORMAT='" + textBox3.Text + "'," + "DESCRIPTION='" + richTextBox1.Text + "'," + "ALIANAME='" + textBox6.Text + "'," + "SOURCE='" + textBox4.Text + "'," + "HEIGHTDATUM='" + textBox10.Text + "'," + "SHEETNO='" + textBox9.Text + "'," + "COOR='" + textBox8.Text + "'," + "SCALE='" + textBox13.Text + "'," + "EDITER='" + textBox12.Text + "' where NAME='" + textBox1.Text + "'";
                    m_oraCmd.CommandText = sqlText;
                    m_oraCmd.ExecuteNonQuery();
                    if (this.m_labelActive.Count > 0) 
                    {
                        sqlText = "update CUSTOMMETADATA set ";
                        for (int i = 0; i < this.m_customField.Count; i++) 
                        {
                            if (i != this.m_customField.Count - 1)
                            {
                                sqlText += this.m_customField[i].ToString() + "='" + this.m_textBoxActive[i].Text.Trim().ToString() + "',";
                            }
                            else 
                            {
                                sqlText += this.m_customField[i].ToString() + "='" + this.m_textBoxActive[i].Text.Trim().ToString() + "'";
                            }
                        }
                        sqlText += " where NAME='" + textBox1.Text + "'";
                        m_oraCmd.CommandText = sqlText;
                        m_oraCmd.ExecuteNonQuery();
                    }
                    this.m_isEdit = false;
                    MessageBox.Show("更新成功！");

                    break;

                case "按名称":
                    frmSearchByName frmSearch = new frmSearchByName();
                    frmSearch.ShowDialog();
                    string name = frmSearch.Name;
                    if (name != "")
                    {
                        sqlText = "select *from SHEETMETADATA where NAME='" + name + "'";
                        m_oraCmd.CommandText = sqlText;
                        OracleDataReader dr = m_oraCmd.ExecuteReader();
                        if (dr.Read())
                        {
                            this.textBox1.Text = dr.GetValue(0).ToString();
                            this.textBox5.Text = dr.GetValue(2).ToString();
                            this.textBox7.Text = dr.GetValue(1).ToString();
                            this.textBox2.Text = dr.GetValue(4).ToString();
                            this.textBox3.Text = dr.GetValue(3).ToString();
                            this.textBox4.Text = dr.GetValue(7).ToString();
                            this.textBox6.Text = dr.GetValue(6).ToString();
                            this.richTextBox1.Text = dr.GetValue(5).ToString();

                            this.textBox9.Text = dr.GetValue(6).ToString();
                            this.textBox10.Text = dr.GetValue(9).ToString();
                            this.textBox8.Text = dr.GetValue(11).ToString();
                            this.textBox13.Text = dr.GetValue(12).ToString();
                            this.textBox12.Text = dr.GetValue(13).ToString();
                            //this.m_selectNode = e.Node.Text;
                        }
                        else
                        {
                            this.textBox1.Text = "";
                            this.textBox5.Text = "";
                            this.textBox7.Text = "";
                            this.textBox2.Text = "";
                            this.textBox3.Text = "";
                            this.textBox4.Text = "";
                            this.textBox6.Text = "";
                            this.richTextBox1.Text = "";

                            this.textBox9.Text = "";
                            this.textBox10.Text = "";
                            this.textBox8.Text = "";
                            this.textBox13.Text = "";
                            this.textBox12.Text = "";
                            //this.m_selectNode = "";
                        }
                    }
                    break;

            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void uiTab1_SelectedTabChanged(object sender, Janus.Windows.UI.Tab.TabEventArgs e)
        {
        }

        private void InitCustom() 
        {
            label12.Visible = true;

            for (int i = 0; i < m_textBox.Length; i++)
            {
                m_textBox[i].Visible = false;
                m_label[i].Visible = false;
            }

            this.m_textBoxActive.Clear();
            this.m_labelActive.Clear();
            this.m_customField.Clear();
            m_oraCmd.CommandText = "select * from CUSTOMFIELD";
            OracleDataReader dr = m_oraCmd.ExecuteReader();
            int j = 0;
            while (dr.Read())
            {
                this.m_textBoxActive.Add(this.m_textBox[j]);
                this.m_label[j].Text = dr.GetValue(1).ToString()+"：";
                this.m_labelActive.Add(this.m_label[j]);
                this.m_customField.Add(dr.GetValue(0).ToString());
                this.m_textBox[j].Visible = true;
                this.m_label[j].Visible = true;
                j++;
            }
            if (this.m_textBoxActive.Count > 0) 
            {
                label12.Visible = false;
            }
        }

    }
}