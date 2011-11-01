using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;


namespace CoastalGIS.SpatialDataBase
{
    public partial class frmImageSearch : Form
    {
        private OleDbCommand m_oraCmd = null;
        private frmImage m_frmImage = null;
        public frmImageSearch(OleDbCommand oraCmd, frmImage frmImage)
        {
            InitializeComponent();
            this.m_oraCmd = oraCmd;
            this.m_frmImage = frmImage;
            string sqlText = "select distinct imageset from imageset";
            m_oraCmd.CommandText = sqlText;
            OleDbDataReader dr = m_oraCmd.ExecuteReader();
            while (dr.Read()) 
            {
                this.comboBox1.Items.Add(dr["imageset"].ToString());
            }
            dr.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            this.comboBox1.SelectedIndex = -1;
            this.listView1.Items.Clear();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == true)
            {
                this.comboBox1.Enabled = true;
            }
            else
            {
                this.comboBox1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sqlText = "";
            if (this.textBox1.Text == "") 
            {
                MessageBox.Show("请输入图片名称！");
                return;
            }
            if (this.checkBox1.Checked == true)
            {
                if (this.comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("请选择图片集名称！");
                    return;
                }
                else
                {
                    sqlText = "select imagename,imageset from imageset where imagename like '%" + this.textBox1.Text.Trim() + "%' and imageset='" + this.comboBox1.SelectedItem.ToString() + "'";
                }
            }
            else 
            {
                sqlText = "select imagename,imageset from imageset where imagename like '%" + this.textBox1.Text.Trim() + "%'";
            }

            this.m_oraCmd.CommandText = sqlText;
            OleDbDataReader dr = this.m_oraCmd.ExecuteReader();
            if (!dr.HasRows)
            {
                MessageBox.Show("无搜索结果！");
                dr.Close();
                return;
            }

            ListViewItem item = null;
            while (dr.Read()) 
            {
                item = new ListViewItem(dr["imagename"].ToString());
                item.SubItems.Add(dr["imageset"].ToString());
                this.listView1.Items.Add(item);
            }
            dr.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems != null) 
            {
                m_oraCmd.CommandText = "select image from IMAGESET where imagename = '" + this.listView1.SelectedItems[0].SubItems[0].Text.ToString() + "'";
                OleDbDataReader dr = m_oraCmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["image"] != DBNull.Value)
                    {
                        MemoryStream ms = new MemoryStream((byte[])dr["image"]);//把照片读到MemoryStream里      
                        Image imageBlob = Image.FromStream(ms, true);//用流创建Image  
                        //this.m_frmImage.PicImage = imageBlob;
                        if (imageBlob.Height > this.m_frmImage.PicBox.Size.Height || imageBlob.Width > this.m_frmImage.PicBox.Size.Width)
                        {
                            this.m_frmImage.PicBox.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            this.m_frmImage.PicBox.SizeMode = PictureBoxSizeMode.CenterImage;
                        }
                        this.m_frmImage.PicBox.Image = imageBlob;
                    }
                }
                dr.Close();
            }
        }
    }
}