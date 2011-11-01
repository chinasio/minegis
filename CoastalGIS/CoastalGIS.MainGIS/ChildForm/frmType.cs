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
        private Dictionary<string, double> zbfgDic = new Dictionary<string, double>();//植被覆盖指数
        private Dictionary<string, double> dbphDic = new Dictionary<string, double>();//地表破坏指数
        private Dictionary<string, double> tdthDic = new Dictionary<string, double>();//土地退化指数

        private IFeature m_fea = null;
        private string m_key="";

        private int m_selected = -1;

        public frmType(IFeature fea)
        {
            InitializeComponent();
            m_fea = fea;
            initfdDic();
            initzbfgDic();
            initdbphDic();
            inittdthDic();
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

        private void initzbfgDic()
        {
            zbfgDic.Add("有林地", 0.38 * 0.6);
            zbfgDic.Add("灌木林地", 0.38 * 0.25);
            zbfgDic.Add("疏林地和其它林地", 0.38 * 0.15);
            zbfgDic.Add("高覆盖度草地", 0.34 * 0.6);
            zbfgDic.Add("中覆盖度草地", 0.34 * 0.3);
            zbfgDic.Add("低覆盖度草地", 0.34 * 0.1);
            zbfgDic.Add("水田", 0.19 * 0.7);
            zbfgDic.Add("旱地", 0.19 * 0.3);
            zbfgDic.Add("城镇建设用地", 0.07 * 0.2);
            zbfgDic.Add("农村居民点", 0.07 * 0.5);
            zbfgDic.Add("其它建设用地", 0.07 * 0.3);
            zbfgDic.Add("沙地", 0.02 * 0.2);
            zbfgDic.Add("盐碱地", 0.02 * 0.3);
            zbfgDic.Add("裸土地", 0.02 * 0.3);
            zbfgDic.Add("裸岩石砾", 0.02 * 0.2);
        }

        private void initdbphDic()
        {
            dbphDic.Add("地表塌陷", 0.4);
            dbphDic.Add("地裂缝", 0.4);
            dbphDic.Add("滑坡", 0.2);
        }

        private void inittdthDic()
        {
            tdthDic.Add("轻度侵蚀", 0.05);
            tdthDic.Add("中度侵蚀", 0.25);
            tdthDic.Add("重度侵蚀", 0.7);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex != -1) 
            {
                this.comboBox2.SelectedIndex = -1;
                this.comboBox3.SelectedIndex = -1;
                this.comboBox4.SelectedIndex = -1;
                m_key = this.comboBox1.SelectedItem.ToString();
                this.m_selected = 1;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox2.SelectedIndex != -1)
            {
                this.comboBox1.SelectedIndex = -1;
                this.comboBox3.SelectedIndex = -1;
                this.comboBox4.SelectedIndex = -1;
                m_key = this.comboBox2.SelectedItem.ToString();
                this.m_selected = 2;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox3.SelectedIndex != -1)
            {
                this.comboBox1.SelectedIndex = -1;
                this.comboBox2.SelectedIndex = -1;
                this.comboBox4.SelectedIndex = -1;
                m_key = this.comboBox3.SelectedItem.ToString();
                this.m_selected = 3;
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox4.SelectedIndex != -1)
            {
                this.comboBox2.SelectedIndex = -1;
                this.comboBox3.SelectedIndex = -1;
                this.comboBox1.SelectedIndex = -1;
                m_key = this.comboBox4.SelectedItem.ToString();
                this.m_selected = 4;
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
                double weight = 0;
                switch (this.m_selected) 
                {
                    case 1:
                        weight = fdDic[m_key];
                        break;
                    case 2:
                        weight = zbfgDic[m_key];
                        break;
                    case 3:
                        weight = dbphDic[m_key];
                        break;
                    case 4:
                        weight = tdthDic[m_key];
                        break;
                }
                //double weight = fdDic[m_key];
                m_fea.set_Value(2, m_key.ToString());
                m_fea.set_Value(3,weight.ToString());
                this.Close();
            }
        }


    }
}