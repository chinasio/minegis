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
    public partial class frmImageImport : Form
    {
        private OleDbCommand m_oraCmd = null;
        private string[] m_fileNames;
        public frmImageImport(OleDbCommand oraCmd)
        {
            InitializeComponent();
            m_oraCmd = oraCmd;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                m_fileNames = null;
                this.listBox1.Items.Clear();
                this.comboBox1.Items.Clear();
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Multiselect = true;
                dlg.Title = "选择图片文件";
                dlg.Filter = "JPEG图像(*.jpg)|*.jpg|PNG图像(*.png)|*.png|Windows位图(*.bmp)|*.bmp|所有文件|*.*;";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    m_fileNames = dlg.FileNames;
                    for (int i = 0; i < dlg.FileNames.Length; i++)
                    {
                        this.listBox1.Items.Add(dlg.FileNames[i]);
                    }
                    m_oraCmd.CommandText = "select distinct ImageSet from ImageSet";
                    OleDbDataReader dr = m_oraCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        this.comboBox1.Items.Add(dr.GetValue(0).ToString());
                    }
                    dr.Close();

                    if (this.comboBox1.Items.IndexOf("其它") == -1)
                    {
                        this.comboBox1.Items.Add("其它");
                    }
                }
            }
            catch (Exception ee) 
            {
                MessageBox.Show(ee.Message);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == true)
            {
                this.textBox1.Enabled = true;
                this.comboBox1.Enabled = false;
            }
            else 
            {
                this.textBox1.Enabled = false;
                this.comboBox1.Enabled = true;
            }
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count == 0) 
            {
                MessageBox.Show("请先选择数据！");
                return;
            }

            string set="";
            string satelite = "";
            if (this.checkBox1.Checked == false)
            {
                if (this.comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("图片集不能为空！");
                    return;
                }
                else 
                {
                    set = this.comboBox1.SelectedItem.ToString();
                }

            }
            else 
            {
                if (this.textBox1.Text == "")
                {
                    MessageBox.Show("图片集不能为空！");
                    return;
                }
                else 
                {
                    set = this.textBox1.Text.Trim();
                }
            }
            for (int i = 0; i < m_fileNames.Length; i++)
            {
                try
                {
                    m_oraCmd.Parameters.Clear();
                    FileStream file = new FileStream(m_fileNames[i].ToString(), FileMode.Open, FileAccess.Read);
                    Byte[] imgByte = new Byte[file.Length];//把图片转成 Byte型 二进制流   
                    file.Read(imgByte, 0, imgByte.Length);//把二进制流读入缓冲区   
                    file.Close();
                    m_oraCmd.CommandText = "insert into ImageSet([IMAGENAME],[IMAGESET],[IMAGE]) values ('" + System.IO.Path.GetFileNameWithoutExtension(m_fileNames[i].ToString()) + "','" + set + "',@IMAGE)";
                    //m_oraCmd.CommandText = "insert into ImageSet(IMAGE) values (@IMAGE)";

                    OleDbParameter spFile = new OleDbParameter("@IMAGE", OleDbType.Binary);
                    spFile.Value = imgByte;

                    m_oraCmd.Parameters.Add(spFile);
                    //m_oraCmd.Parameters[0].Value = imgByte;
                    m_oraCmd.ExecuteNonQuery();
                }
                catch (Exception ee) 
                {
                    MessageBox.Show(ee.Message);
                }

            }
            MessageBox.Show("入库完成！");
            this.listBox1.Items.Clear();
            this.m_fileNames = null;
            this.comboBox1.Items.Clear();
            m_oraCmd.Parameters.Clear();

        }

        private void uiTab1_SelectedTabChanged(object sender, Janus.Windows.UI.Tab.TabEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //m_fileNames = null;
                this.textBox3.Text = "";
                this.comboBox2.Items.Clear();
                OpenFileDialog dlg = new OpenFileDialog();
                //dlg.Multiselect = true;
                dlg.Title = "选择解译标志文件";
                dlg.Filter = "JPEG图像(*.jpg)|*.jpg|PNG图像(*.png)|*.png|Windows位图(*.bmp)|*.bmp|所有文件|*.*;";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //m_fileNames = dlg.FileNames;
                    //for (int i = 0; i < dlg.FileNames.Length; i++)
                    //{
                    //    this.listBox1.Items.Add(dlg.FileNames[i]);
                    //}

                    this.textBox3.Text = dlg.FileName;

                    m_oraCmd.CommandText = "select distinct ImageSet from MarkSet";
                    OleDbDataReader dr = m_oraCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        this.comboBox2.Items.Add(dr.GetValue(0).ToString());
                    }
                    dr.Close();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked == true)
            {
                this.textBox2.Enabled = true;
                this.comboBox2.Enabled = false;
            }
            else
            {
                this.textBox2.Enabled = false;
                this.comboBox2.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.textBox3.Text=="")
            {
                MessageBox.Show("请先选择数据！");
                return;
            }

            if (this.richTextBox1.Text == "")
            {
                MessageBox.Show("描述不能为空！");
                return;
            }

            string set = "";
            string satelite = "";
            if (this.checkBox2.Checked == false)
            {
                if (this.comboBox2.SelectedItem == null)
                {
                    MessageBox.Show("图片集不能为空！");
                    return;
                }
                else
                {
                    set = this.comboBox2.SelectedItem.ToString();
                }

            }
            else
            {
                if (this.textBox2.Text == "")
                {
                    MessageBox.Show("图片集不能为空！");
                    return;
                }
                else
                {
                    set = this.textBox2.Text.Trim();
                }
            }

            if (this.comboBox3.SelectedItem == null)
            {
                MessageBox.Show("卫星名称不能为空！");
                return;
            }
            else 
            {
                satelite = this.comboBox3.SelectedItem.ToString();
            }

            try
            {
                m_oraCmd.Parameters.Clear();
                FileStream file = new FileStream(this.textBox3.Text.Trim(), FileMode.Open, FileAccess.Read);
                Byte[] imgByte = new Byte[file.Length];//把图片转成 Byte型 二进制流   
                file.Read(imgByte, 0, imgByte.Length);//把二进制流读入缓冲区   
                file.Close();
                m_oraCmd.CommandText = "insert into MarkSet([IMAGENAME],[IMAGESET],[DES],[SATELITE],[IMAGE]) values ('" + System.IO.Path.GetFileNameWithoutExtension(this.textBox3.Text.Trim()) + "','" + set + "','" + this.richTextBox1.Text.Trim() + "','" +satelite+ "',@IMAGE)";
                //m_oraCmd.CommandText = "insert into ImageSet(IMAGE) values (@IMAGE)";

                OleDbParameter spFile = new OleDbParameter("@IMAGE", OleDbType.Binary);
                spFile.Value = imgByte;

                m_oraCmd.Parameters.Add(spFile);
                //m_oraCmd.Parameters[0].Value = imgByte;
                m_oraCmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            MessageBox.Show("入库完成！");
            this.listBox1.Items.Clear();
            this.m_fileNames = null;
            this.comboBox1.Items.Clear();
            m_oraCmd.Parameters.Clear();
        }
    }
}