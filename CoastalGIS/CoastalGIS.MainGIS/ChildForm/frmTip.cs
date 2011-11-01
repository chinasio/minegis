using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace CoastalGIS.MainGIS
{
    public partial class frmTip : Form
    {
        private string m_img;
        private OleDbCommand m_oraCmd;
        public frmTip(string des,string img,OleDbCommand oraCmd)
        {
            InitializeComponent();
            this.richTextBox1.Text = des;
            m_img = img;
            m_oraCmd = oraCmd;
        }

        private void showImg(string img,OleDbCommand oraCmd) 
        {
            string[] s = img.Split(':');

            m_oraCmd.CommandText = "select image from IMAGESET where IMAGENAME = '" + s[1].ToString() + "' and IMAGESET='"+s[0].ToString()+"'";
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

        private void frmTip_Load(object sender, EventArgs e)
        {
            showImg(m_img, m_oraCmd);
        }
    }
}