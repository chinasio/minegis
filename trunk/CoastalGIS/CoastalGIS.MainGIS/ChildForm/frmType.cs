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
        private Dictionary<string, double> zbfgDic = new Dictionary<string, double>();//ֲ������ָ��
        private Dictionary<string, double> dbphDic = new Dictionary<string, double>();//�ر��ƻ�ָ��
        private Dictionary<string, double> tdthDic = new Dictionary<string, double>();//�����˻�ָ��

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

        private void initzbfgDic()
        {
            zbfgDic.Add("���ֵ�", 0.38 * 0.6);
            zbfgDic.Add("��ľ�ֵ�", 0.38 * 0.25);
            zbfgDic.Add("���ֵغ������ֵ�", 0.38 * 0.15);
            zbfgDic.Add("�߸��ǶȲݵ�", 0.34 * 0.6);
            zbfgDic.Add("�и��ǶȲݵ�", 0.34 * 0.3);
            zbfgDic.Add("�͸��ǶȲݵ�", 0.34 * 0.1);
            zbfgDic.Add("ˮ��", 0.19 * 0.7);
            zbfgDic.Add("����", 0.19 * 0.3);
            zbfgDic.Add("�������õ�", 0.07 * 0.2);
            zbfgDic.Add("ũ������", 0.07 * 0.5);
            zbfgDic.Add("���������õ�", 0.07 * 0.3);
            zbfgDic.Add("ɳ��", 0.02 * 0.2);
            zbfgDic.Add("�μ��", 0.02 * 0.3);
            zbfgDic.Add("������", 0.02 * 0.3);
            zbfgDic.Add("����ʯ��", 0.02 * 0.2);
        }

        private void initdbphDic()
        {
            dbphDic.Add("�ر�����", 0.4);
            dbphDic.Add("���ѷ�", 0.4);
            dbphDic.Add("����", 0.2);
        }

        private void inittdthDic()
        {
            tdthDic.Add("�����ʴ", 0.05);
            tdthDic.Add("�ж���ʴ", 0.25);
            tdthDic.Add("�ض���ʴ", 0.7);
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
                MessageBox.Show("��ѡ������!");
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