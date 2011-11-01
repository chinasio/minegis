using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CoastalGIS.ExportMapProj
{
    public partial class ChoseTemple : Form
    {
        public  string m_templateName="";
        public ChoseTemple()
        {
            InitializeComponent();
      
        }

        private void ChoseTemple_Load(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            DirectoryInfo dInfo = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + @"\pagelayoutTemplate\");
            for (int i = 0; i < dInfo.GetFiles().Length; i++) 
            {
                if (dInfo.GetFiles()[i].Extension == ".mxt") 
                {
                    this.listBox1.Items.Add(dInfo.GetFiles()[i].Name);
                }

            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*if (this.listBox1.SelectedIndex != -1) 
            {
                switch (this.listBox1.SelectedItem.ToString())
                {
                    case "JangSu.mxt":
                        this.pictureBox1.Image = this.imageList1.Images[3];
                        break;
                    case "Templete6.mxt":
                        this.pictureBox1.Image = this.imageList1.Images[2];
                        break;
                    case "Templete7.mxt":
                        this.pictureBox1.Image = this.imageList1.Images[1];
                        break;
                    case "Templete8.mxt":
                        this.pictureBox1.Image = this.imageList1.Images[0];
                        break;
                    case "062301.mxt":
                        this.pictureBox1.Image = this.imageList1.Images[4];
                        break;
                    case "100705.mxt":
                        this.pictureBox1.Image = this.imageList1.Images[5];
                        break;
                }
            }*/
            if (this.listBox1.SelectedItem != null) 
            {
                string pathWithoutExtension = System.Windows.Forms.Application.StartupPath + @"\pagelayoutTemplate\" + Path.GetFileNameWithoutExtension(this.listBox1.SelectedItem.ToString());
                if (File.Exists(pathWithoutExtension + ".jpg"))
                {
                    this.pictureBox1.ImageLocation = pathWithoutExtension + ".jpg";
                }
                else if (File.Exists(pathWithoutExtension + ".png"))
                {
                    this.pictureBox1.ImageLocation = pathWithoutExtension + ".png";
                }
                else if (File.Exists(this.pictureBox1.ImageLocation = pathWithoutExtension + ".bmp"))
                {
                    this.pictureBox1.ImageLocation = pathWithoutExtension + ".bmp";
                }
            }

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex != -1) 
            {
                m_templateName = this.listBox1.SelectedItem.ToString();
            }
            this.Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "选择模板";
            dlg.Multiselect = false;
            dlg.Filter = " mxt files(*.mxt)|*.mxt";
            if (dlg.ShowDialog() == DialogResult.OK) 
            {
                m_templateName = dlg.FileName;
                this.Close();
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "导入模板";
            dlg.Multiselect = true;
            dlg.Filter = " 所有文件|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < dlg.FileNames.Length; i++) 
                {
                    File.Copy(dlg.FileNames[i].ToString(), Application.StartupPath + "\\pagelayoutTemplate\\" + System.IO.Path.GetFileName(dlg.FileNames[i].ToString()));
                }
            }

            this.listBox1.Items.Clear();
            DirectoryInfo dInfo = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + @"\pagelayoutTemplate\");
            for (int i = 0; i < dInfo.GetFiles().Length; i++)
            {
                if (dInfo.GetFiles()[i].Extension == ".mxt")
                {
                    this.listBox1.Items.Add(dInfo.GetFiles()[i].Name);
                }

            }
        }
    }
}