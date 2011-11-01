using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;

namespace CoastalGIS.MainGIS
{
    public partial class frmType : Form
    {

        private  Dictionary<string, double> fdDic = new Dictionary<string, double>();//丰度指数
        private IFeature m_fea = null;
        private string m_key="";

        public frmType(IFeature fea)
        {
            InitializeComponent();
            m_fea = fea;
            initfdDic();

        }

        private void initfdDic() 
        {
            fdDic.Add("有林地", 0.35 * 0.6);
            fdDic.Add("灌木林地", 0.35 * 0.25);
            fdDic.Add("疏林地和其它林地", 0.35 * 0.15);
            fdDic.Add("高覆盖度草地", 0.21 * 0.6);
            fdDic.Add("中覆盖度草地", 0.21 * 0.3);
            fdDic.Add("低覆盖度草地", 0.21*0.1);
            fdDic.Add("河流", 0.28*0.6);
            fdDic.Add("湖泊（库)", 0.28 * 0.4);
            fdDic.Add("水浇地", 0.11*0.6);
            fdDic.Add("旱地", 0.11 * 0.4);
            fdDic.Add("城镇建设用地", 0.04*0.2);
            fdDic.Add("农村居民点", 0.04 * 0.5);
            fdDic.Add("其它建设用地", 0.04 * 0.3);
            fdDic.Add("沙地", 0.01*0.2);
            fdDic.Add("盐碱地", 0.01 * 0.3);
            fdDic.Add("裸土地", 0.01 * 0.3);
            fdDic.Add("裸岩石砾", 0.01 * 0.2);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex != -1) 
            {
                this.comboBox2.SelectedIndex = -1;
                this.comboBox3.SelectedIndex = -1;
                this.comboBox4.SelectedIndex = -1;
                m_key = this.comboBox1.SelectedItem.ToString();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox2.SelectedIndex != -1)
            {
                this.comboBox1.SelectedIndex = -1;
                this.comboBox3.SelectedIndex = -1;
                this.comboBox4.SelectedIndex = -1;
                m_key = this.comboBox2.SelectedText.Trim();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox3.SelectedIndex != -1)
            {
                this.comboBox1.SelectedIndex = -1;
                this.comboBox2.SelectedIndex = -1;
                this.comboBox4.SelectedIndex = -1;
                m_key = this.comboBox3.SelectedText.Trim();
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox4.SelectedIndex != -1)
            {
                this.comboBox2.SelectedIndex = -1;
                this.comboBox3.SelectedIndex = -1;
                this.comboBox1.SelectedIndex = -1;
                m_key = this.comboBox4.SelectedText.Trim();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (m_key == "")
            {
                MessageBox.Show("请选择类型!");
                return;
            }
            else 
            {
                double weight = fdDic[m_key];
                m_fea.set_Value(2, m_key.ToString());
                m_fea.set_Value(3,weight.ToString());
                this.Close();
            }
        }


    }
}