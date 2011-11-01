using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Windows.Forms;

namespace CoastalGIS.SpatialDataBase
{
    public partial class frmMark : Form
    {
        private OleDbCommand m_oraCmd;
        private TreeNode m_node = null;

        public PictureBox PicBox
        {
            get { return this.pictureBox1; }
        }

        public frmMark(OleDbCommand oraCmd)
        {

            this.m_oraCmd = oraCmd;
            InitializeComponent();
            ShowMarkTree();
        }

        private void frmImage_Load(object sender, EventArgs e)
        {

        }




        private void ShowMarkTree()
        {

            if (this.treeView2.Nodes.Count > 0)
            {
                this.treeView2.Nodes.Clear();
            }
            TreeNode firstNode = new TreeNode("标志库");
            firstNode.Tag = "MarDB";
            this.treeView2.Nodes.Add(firstNode);
            firstNode.Expand();

            m_oraCmd.CommandText = "select distinct ImageSet from MarkSet";
            OleDbDataReader dr = m_oraCmd.ExecuteReader();
            IList<string> temp = new List<string>();
            while (dr.Read())
            {
                temp.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            for (int i = 0; i < temp.Count; i++)
            {
                TreeNode thirdNode = new TreeNode(temp[i].ToString());
                thirdNode.Tag = "MarSet";
                firstNode.Nodes.Add(thirdNode);

                m_oraCmd.CommandText = "select distinct [IMAGENAME] from MarkSet where Imageset='" + temp[i].ToString()+"'";
                OleDbDataReader dr2 = m_oraCmd.ExecuteReader();
                while (dr2.Read())
                {
                    TreeNode forthNode = new TreeNode(dr2.GetValue(0).ToString());
                    forthNode.Tag = "Mar";
                    thirdNode.Nodes.Add(forthNode);
                }
                dr2.Close();
            }
            firstNode.Expand();
        }


        private void treeView2_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag.ToString() == "Mar")
            {
                m_oraCmd.CommandText = "select * from MarkSet where IMAGENAME='" + e.Node.Text.ToString() + "'";
                OleDbDataReader dr = m_oraCmd.ExecuteReader();
                if (!dr.HasRows)
                {
                    MessageBox.Show("该标志不存在！");
                    this.ShowMarkTree();
                    dr.Close();
                    return;
                }
                else
                {
                    dr.Close();
                }

            }

            if (e.Node.Tag.ToString() == "MarSet")
            {
                m_oraCmd.CommandText = "select * from MarkSet where Imageset='" + e.Node.Text.ToString() + "'";
                OleDbDataReader dr = m_oraCmd.ExecuteReader();
                if (!dr.HasRows)
                {
                    MessageBox.Show("该标志集不存在！");
                    this.ShowMarkTree();
                    dr.Close();
                    return;
                }
                else
                {
                    dr.Close();
                }
            }

            this.m_node = e.Node;
            this.Delete.Enabled = false;
            if (e.Button == MouseButtons.Right && e.Node.Tag.ToString() != "MarDB" && e.Node.Tag.ToString() != "TM" && e.Node.Tag.ToString() != "SPOT" && e.Node.Tag.ToString() != "HJ-1")
            {

            }
            else
            {
                if (e.Node.Tag.ToString() == "Mar")
                {
                    m_oraCmd.CommandText = "select image,SATELITE,DES from MARKSET where IMAGENAME = '" + e.Node.Text.ToString()+"'";
                    OleDbDataReader dr = m_oraCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr["image"] != DBNull.Value)
                        {
                            MemoryStream ms = new MemoryStream((byte[])dr["image"]);//把照片读到MemoryStream里      
                            Image imageBlob = Image.FromStream(ms, true);//用流创建Image  
                            //MessageBox.Show(dr["SATELITE"].ToString());
                            switch(dr["SATELITE"].ToString())
                            {
                                case"TM":
                                    if (imageBlob.Height > this.pictureBox1.Size.Height || imageBlob.Width > this.pictureBox1.Size.Width)
                                    {
                                        this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                                    }
                                    this.pictureBox1.Image = imageBlob;
                                    break;

                                case "SPOT":
                                    if (imageBlob.Height > this.pictureBox2.Size.Height || imageBlob.Width > this.pictureBox2.Size.Width)
                                    {
                                        this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        this.pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
                                    }
                                    this.pictureBox2.Image = imageBlob;
                                    break;


                                case "HJ-1":
                                    if (imageBlob.Height > this.pictureBox3.Size.Height || imageBlob.Width > this.pictureBox3.Size.Width)
                                    {
                                        this.pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        this.pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;
                                    }
                                    this.pictureBox3.Image = imageBlob;
                                    break;
                            }
                            this.richTextBox1.Text = dr["DES"].ToString();

                        }
                    }
                    dr.Close();
                }
            }
        }
    }
}