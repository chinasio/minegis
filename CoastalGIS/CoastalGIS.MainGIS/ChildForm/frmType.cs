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

        private  Dictionary<string, double> fdDic = new Dictionary<string, double>();//���ָ��
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
            fdDic.Add("���ֵ�", 0.35 * 0.6);
            fdDic.Add("��ľ�ֵ�", 0.35 * 0.25);
            fdDic.Add("���ֵغ������ֵ�", 0.35 * 0.15);
            fdDic.Add("�߸��ǶȲݵ�", 0.21 * 0.6);
            fdDic.Add("�и��ǶȲݵ�", 0.21 * 0.3);
            fdDic.Add("�͸��ǶȲݵ�", 0.21*0.1);
            fdDic.Add("����", 0.28*0.6);
            fdDic.Add("��������)", 0.28 * 0.4);
            fdDic.Add("ˮ����", 0.11*0.6);
            fdDic.Add("����", 0.11 * 0.4);
            fdDic.Add("�������õ�", 0.04*0.2);
            fdDic.Add("ũ������", 0.04 * 0.5);
            fdDic.Add("���������õ�", 0.04 * 0.3);
            fdDic.Add("ɳ��", 0.01*0.2);
            fdDic.Add("�μ��", 0.01 * 0.3);
            fdDic.Add("������", 0.01 * 0.3);
            fdDic.Add("����ʯ��", 0.01 * 0.2);
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
                MessageBox.Show("��ѡ������!");
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