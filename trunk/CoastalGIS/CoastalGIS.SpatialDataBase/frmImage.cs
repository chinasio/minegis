using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CoastalGIS.SpatialDataBase
{
    public partial class frmImage : Form
    {
        private OleDbCommand m_oraCmd;
        private TreeNode m_node = null;

        public PictureBox PicBox
        {
            get { return this.pictureBox1; }
        }

        public frmImage(OleDbCommand oraCmd)
        {
            this.m_oraCmd = oraCmd;
            InitializeComponent();
            ShowImageTree();
        }

        private void frmImage_Load(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            frmImageImport frmImageIm = new frmImageImport(this.m_oraCmd);
            frmImageIm.ShowDialog();
            ShowImageTree();
        }

        private void ShowImageTree() 
        {

            if (this.treeView1.Nodes.Count > 0)
            {
                this.treeView1.Nodes.Clear();
            }
            TreeNode firstNode = new TreeNode("图片库");
            firstNode.ImageIndex = 1;
            firstNode.SelectedImageIndex = 1;
            firstNode.Tag = "ImageDB";
            this.treeView1.Nodes.Add(firstNode);
            firstNode.Expand();
            m_oraCmd.CommandText = "select distinct ImageSet from ImageSet";
            OleDbDataReader dr = m_oraCmd.ExecuteReader();

            IList<string> temp = new List<string>();
            while(dr.Read())
            {
                temp.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            for (int i = 0; i < temp.Count;i++)
            {
                TreeNode secondNode = new TreeNode(temp[i].ToString());
                secondNode.ImageIndex = 1;
                secondNode.SelectedImageIndex = 1;
                secondNode.Tag = "ImageSet";
                firstNode.Nodes.Add(secondNode);

                m_oraCmd.CommandText = "select [IMAGENAME] from ImageSet where Imageset='" + temp[i].ToString() + "'";
                OleDbDataReader dr2 = m_oraCmd.ExecuteReader();
                while (dr2.Read())
                {
                    TreeNode thirdNode = new TreeNode(dr2.GetValue(0).ToString());
                    thirdNode.ImageIndex = 2;
                    thirdNode.SelectedImageIndex = 2;
                    thirdNode.Tag = "Image";
                    secondNode.Nodes.Add(thirdNode);
                }
                dr2.Close();
            }
            firstNode.Expand();
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 0;
            e.Node.SelectedImageIndex = 0;
        }

        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 1;
            e.Node.SelectedImageIndex = 1;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (this.uiTab1.SelectedTab.Key == "Img")
            {
                if (e.Node.Tag.ToString() == "Image")
                {
                    m_oraCmd.CommandText = "select * from ImageSet where IMAGENAME='" + e.Node.Text.ToString() + "'";
                    OleDbDataReader dr = m_oraCmd.ExecuteReader();
                    if (!dr.HasRows)
                    {
                        MessageBox.Show("该图片不存在！");
                        this.ShowImageTree();
                        dr.Close();
                        return;
                    }
                    else
                    {
                        dr.Close();
                    }

                }

                if (e.Node.Tag.ToString() == "ImageSet")
                {
                    m_oraCmd.CommandText = "select * from ImageSet where Imageset='" + e.Node.Text.ToString() + "'";
                    OleDbDataReader dr = m_oraCmd.ExecuteReader();
                    if (!dr.HasRows)
                    {
                        MessageBox.Show("该图片集不存在！");
                        this.ShowImageTree();
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
                if (e.Button == MouseButtons.Right && e.Node.Tag.ToString() != "ImageDB")
                {
                    this.Delete.Enabled = true;
                    System.Drawing.Point point = new System.Drawing.Point(e.X, e.Y);
                    this.contextMenuStrip1.Show(this.treeView1, point);

                }
                else
                {
                    if (e.Node.Tag.ToString() == "Image")
                    {
                        m_oraCmd.CommandText = "select image from IMAGESET where IMAGENAME = '" + e.Node.Text.ToString() + "'";
                        OleDbDataReader dr = m_oraCmd.ExecuteReader();
                        while (dr.Read())
                        {
                            if (dr["image"] != DBNull.Value)
                            {
                                MemoryStream ms = new MemoryStream((byte[])dr["image"]);//把照片读到MemoryStream里      
                                Image imageBlob = Image.FromStream(ms, true);//用流创建Image  
                                if (imageBlob.Height > this.pictureBox1.Size.Height || imageBlob.Width > this.pictureBox1.Size.Width)
                                {
                                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                }
                                else
                                {
                                    this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                                }
                                this.pictureBox1.Image = imageBlob;
                            }
                        }
                        dr.Close();
                    }
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            File.Delete(Application.StartupPath + "\\tempImage\\temp.jpg");
            if (this.pictureBox1.Image != null) 
            {
                this.pictureBox1.Image.Save(Application.StartupPath+"\\tempImage\\temp.jpg");
                //建立新的系统进程  
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                //设置文件名，此处为图片的真实路径+文件名   
                process.StartInfo.FileName = Application.StartupPath + "\\tempImage\\temp.jpg";
                //此为关键部分。设置进程运行参数，此时为最大化窗口显示图片。   
                process.StartInfo.Arguments = "rundll32.exe C:\\WINDOWS\\system32\\shimgvw.dll,ImageView_Fullscreen";
                //此项为是否使用Shell执行程序，因系统默认为true，此项也可不设，但若设置必须为true   
                process.StartInfo.UseShellExecute = true;
                //此处可以更改进程所打开窗体的显示样式，可以不设   
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.Start();   
            }
        }

        private void frmImage_FormClosed(object sender, FormClosedEventArgs e)
        {
            File.Delete(Application.StartupPath + "\\tempImage\\temp.jpg");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image!=null&&this.m_node!=null) 
            {
                FolderBrowserDialog fdlg = new FolderBrowserDialog();
                fdlg.Description = "选择保存路径";
                if (fdlg.ShowDialog() == DialogResult.OK) 
                {
                    this.pictureBox1.Image.Save(fdlg.SelectedPath + "\\" + m_node.Text.ToString() + ".jpg");
                    MessageBox.Show("保存成功！");
                }
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (this.m_node != null && this.uiTab1.SelectedTab.Key == "Img")
            {
                if (MessageBox.Show("确定删除此此项吗？", "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    if (this.m_node.Tag.ToString() == "ImageSet")
                    {
                        m_oraCmd.CommandText = "delete from imageset where imageset='" + this.m_node.Text.ToString() + "'";
                    }
                    if (this.m_node.Tag.ToString() == "Image")
                    {
                        m_oraCmd.CommandText = "delete from imageset where IMAGENAME='" + this.m_node.Text.ToString() + "'";
                    }
                    m_oraCmd.ExecuteNonQuery();
                    this.pictureBox1.Image = null;
                    MessageBox.Show("删除成功");
                    ShowImageTree();
                }
            }

            if (this.m_node != null && this.uiTab1.SelectedTab.Key == "Mar")
            {

                if (MessageBox.Show("确定删除此此项吗？", "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    if (this.m_node.Tag.ToString() == "MarSet")
                    {
                        m_oraCmd.CommandText = "delete from markset where imageset='" + this.m_node.Text.ToString() + "' and SATELITE='"+this.m_node.Parent.Parent.Tag.ToString()+"'";
                    }
                    if (this.m_node.Tag.ToString() == "Mar")
                    {
                        m_oraCmd.CommandText = "delete from markset where IMAGENAME='" + this.m_node.Text.ToString() + "' and SATELITE='" + this.m_node.Parent.Parent.Tag.ToString() + "'";
                    }
                    m_oraCmd.ExecuteNonQuery();
                    this.pictureBox1.Image = null;
                    MessageBox.Show("删除成功");
                    ShowMarkTree();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmImageSearch frmSearch = new frmImageSearch(this.m_oraCmd,this);
            frmSearch.ShowDialog();
            ShowMarkTree();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            if (this.uiTab1.SelectedTab.Key == "Img")
            {
                ShowImageTree();
            }
            else 
            {
                ShowMarkTree();
            }



        }

        private void ShowMarkTree()
        {

            if (this.treeView2.Nodes.Count > 0)
            {
                this.treeView2.Nodes.Clear();
            }
            TreeNode firstNode = new TreeNode("标志库");
            firstNode.ImageIndex = 1;
            firstNode.SelectedImageIndex = 1;
            firstNode.Tag = "MarDB";
            this.treeView2.Nodes.Add(firstNode);
            firstNode.Expand();

            TreeNode secondNode1=new TreeNode("TM");
            secondNode1.ImageIndex=1;
            secondNode1.SelectedImageIndex=1;
            secondNode1.Tag="TM";

            TreeNode secondNode2=new TreeNode("SPOT");
            secondNode2.ImageIndex=1;
            secondNode2.SelectedImageIndex=1;
            secondNode2.Tag="SPOT";


            TreeNode secondNode3=new TreeNode("HJ-1");
            secondNode3.ImageIndex=1;
            secondNode3.SelectedImageIndex=1;
            secondNode3.Tag="HJ-1";

            firstNode.Nodes.Add(secondNode1);
            firstNode.Nodes.Add(secondNode2);
            firstNode.Nodes.Add(secondNode3);



            m_oraCmd.CommandText = "select distinct ImageSet from MarkSet where SATELITE='TM'";
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
                thirdNode.ImageIndex = 1;
                thirdNode.SelectedImageIndex = 1;
                thirdNode.Tag = "MarSet";
                secondNode1.Nodes.Add(thirdNode);

                m_oraCmd.CommandText = "select [IMAGENAME] from MarkSet where Imageset='" + temp[i].ToString() + "' and SATELITE='TM'";
                OleDbDataReader dr2 = m_oraCmd.ExecuteReader();
                while (dr2.Read())
                {
                    TreeNode forthNode = new TreeNode(dr2.GetValue(0).ToString());
                    forthNode.ImageIndex = 2;
                    forthNode.SelectedImageIndex = 2;
                    forthNode.Tag = "Mar";
                    thirdNode.Nodes.Add(forthNode);
                }
                dr2.Close();
            }

             m_oraCmd.CommandText = "select distinct ImageSet from MarkSet where SATELITE='SPOT'";
            dr = m_oraCmd.ExecuteReader();

            temp.Clear();
            while (dr.Read())
            {
                temp.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            for (int i = 0; i < temp.Count; i++)
            {
                TreeNode thirdNode = new TreeNode(temp[i].ToString());
                thirdNode.ImageIndex = 1;
                thirdNode.SelectedImageIndex = 1;
                thirdNode.Tag = "MarSet";
                secondNode2.Nodes.Add(thirdNode);

                m_oraCmd.CommandText = "select [IMAGENAME] from MarkSet where Imageset='" + temp[i].ToString() + "' and SATELITE='SPOT'";
                OleDbDataReader dr2 = m_oraCmd.ExecuteReader();
                while (dr2.Read())
                {
                    TreeNode forthNode = new TreeNode(dr2.GetValue(0).ToString());
                    forthNode.ImageIndex = 2;
                    forthNode.SelectedImageIndex = 2;
                    forthNode.Tag = "Mar";
                    thirdNode.Nodes.Add(forthNode);
                }
                dr2.Close();
            }

            m_oraCmd.CommandText = "select distinct ImageSet from MarkSet where SATELITE='HJ-1'";
            dr = m_oraCmd.ExecuteReader();

            temp.Clear();
            while (dr.Read())
            {
                temp.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            for (int i = 0; i < temp.Count; i++)
            {
                TreeNode thirdNode = new TreeNode(temp[i].ToString());
                thirdNode.ImageIndex = 1;
                thirdNode.SelectedImageIndex = 1;
                thirdNode.Tag = "MarSet";
                secondNode3.Nodes.Add(thirdNode);

                m_oraCmd.CommandText = "select [IMAGENAME] from MarkSet where Imageset='" + temp[i].ToString() + "' and SATELITE='HJ-1'";
                OleDbDataReader dr2 = m_oraCmd.ExecuteReader();
                while (dr2.Read())
                {
                    TreeNode forthNode = new TreeNode(dr2.GetValue(0).ToString());
                    forthNode.ImageIndex = 2;
                    forthNode.SelectedImageIndex = 2;
                    forthNode.Tag = "Mar";
                    thirdNode.Nodes.Add(forthNode);
                }
                dr2.Close();
            }






            firstNode.Expand();
        }

        private void uiTab1_ChangingSelectedTab(object sender, Janus.Windows.UI.Tab.TabCancelEventArgs e)
        {
            if (e.Page.Key == "Mar")
            {
                ShowMarkTree();
            }
            else 
            {
                ShowImageTree();
            }
        }

        private void treeView2_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 1;
            e.Node.SelectedImageIndex = 1;
        }

        private void treeView2_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 0;
            e.Node.SelectedImageIndex = 0;
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
                this.Delete.Enabled = true;
                System.Drawing.Point point = new System.Drawing.Point(e.X, e.Y);
                this.contextMenuStrip1.Show(this.treeView2, point);

            }
            else
            {
                if (e.Node.Tag.ToString() == "Mar")
                {
                    m_oraCmd.CommandText = "select image from MARKSET where IMAGENAME = '" + e.Node.Text.ToString() + "' and SATELITE='"+e.Node.Parent.Parent.Tag.ToString()+"'";
                    OleDbDataReader dr = m_oraCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr["image"] != DBNull.Value)
                        {
                            MemoryStream ms = new MemoryStream((byte[])dr["image"]);//把照片读到MemoryStream里      
                            Image imageBlob = Image.FromStream(ms, true);//用流创建Image  
                            if (imageBlob.Height > this.pictureBox1.Size.Height || imageBlob.Width > this.pictureBox1.Size.Width)
                            {
                                this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                            }
                            this.pictureBox1.Image = imageBlob;
                        }
                    }
                    dr.Close();
                }
            }
        }
    }
}