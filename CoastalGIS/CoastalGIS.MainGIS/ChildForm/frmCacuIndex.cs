using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace CoastalGIS.MainGIS
{
    public partial class frmCacuIndex : Form
    {
        IFeatureClass m_fc = null;
        int typeNum=-1;
        int weihtNum = -1;
        string m_index = "";
        //string path = "";
        public frmCacuIndex()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "ѡ������";
            openFileDialog1.Filter = "shapfile�ļ�|*.shp";
            openFileDialog1.FilterIndex = 0;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = openFileDialog1.FileName;
                addFiled(openFileDialog1.FileName);

            } 
        }

        private void showInfo(string path) 
        {
            if (this.comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("��ѡ��ָ������!");
                return;
            }

            if (this.comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("��ѡ���ò�����ֶ�!");
                return;
            }

            if (this.comboBox3.SelectedIndex == -1)
            {
                MessageBox.Show("��ѡ��Ȩ���ֶ�!");
                return;
            }

            IFields pFields = m_fc.Fields;
            typeNum = pFields.FindField(this.comboBox2.SelectedItem.ToString().Trim());
            weihtNum = pFields.FindField(this.comboBox3.SelectedItem.ToString().Trim());

            IFeatureCursor cursor = m_fc.Search(null, false);
            IFeature pfeature = cursor.NextFeature();
            int count=0;
            while(pfeature!=null)
            {
                //IArea pArea = (IArea)pfeature.ShapeCopy;
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[count].Cells[0].Value = (object)count;
                this.dataGridView1.Rows[count].Cells[1].Value = (object)pfeature.get_Value(typeNum).ToString();
                this.dataGridView1.Rows[count].Cells[2].Value = (object)pfeature.get_Value(weihtNum).ToString();

                IArea pArea = (IArea)pfeature.ShapeCopy;
                this.dataGridView1.Rows[count].Cells[3].Value = pArea.Area.ToString();

                pfeature = cursor.NextFeature();
                count++;
            }

            this.button3.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            showInfo(this.textBox1.Text.Trim());
        }

        private void addFiled(string path) 
        {
            this.comboBox2.Items.Clear();
            this.comboBox3.Items.Clear();

            IWorkspaceFactory m_workspaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace featureWorkspce = m_workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(path), 0) as IFeatureWorkspace;
            m_fc = featureWorkspce.OpenFeatureClass(System.IO.Path.GetFileName(path));

            IFields pFields = m_fc.Fields;
            for (int i = 0; i < pFields.FieldCount; i++)
            {
                this.comboBox2.Items.Add(pFields.get_Field(i).Name);
                this.comboBox3.Items.Add(pFields.get_Field(i).Name);
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (m_fc != null)
            {
                IFeatureCursor cursor = m_fc.Search(null, false);
                IFeature pfeature = cursor.NextFeature();
                double index = 0;
                double areaSum = 0;
                while (pfeature != null)
                {
                    IArea pArea = (IArea)pfeature.ShapeCopy;

                    index += Convert.ToDouble(pfeature.get_Value(this.weihtNum).ToString()) * Convert.ToDouble(pArea.Area.ToString());
                    areaSum += Convert.ToDouble(pArea.Area.ToString());
                    pfeature = cursor.NextFeature();
                }
                index = index / areaSum;
                m_index = index.ToString();
                MessageBox.Show("ָ����"+index.ToString());
                this.button4.Enabled = true;
                this.button2.Enabled = false;
                this.button3.Enabled = false;

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (m_index=="")
            {
                MessageBox.Show("��Чָ��!");
                return;
            }
            frmIndexImport indexIm = new frmIndexImport();
            indexIm.ShowDialog();

        }
    }
}