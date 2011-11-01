using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

using MSChart20Lib;

namespace CoastalGIS.MainGIS
{
    public partial class frmChart : Form
    {
        OleDbCommand m_oraCmd=null;
        int m_comSelect = -1;
        private Dictionary<string, string> zsList = new Dictionary<string, string>();

        public frmChart(OleDbCommand oraCmd)
        {
            InitializeComponent();
            this.m_oraCmd = oraCmd;
            zsList.Add("丰度指数","FD");
            zsList.Add("植被覆盖指数", "ZBFG");
            zsList.Add("地质环境指数", "DZHJ");
            zsList.Add("土地退化指数", "TDTH");
        }

        private void frmChart_Load(object sender, EventArgs e)
        {
            InitCheck();
            this.comboBox1.SelectedIndex = 0;
 
        }

        private void InitCheck()
        {
            m_oraCmd.CommandText = "select distinct YEARTIME from [INDEX] order by YEARTIME";
            OleDbDataReader dr = m_oraCmd.ExecuteReader();
            while(dr.Read())
            {
                this.checkedListBox1.Items.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            m_oraCmd.CommandText = "select distinct PLACE from [INDEX]";
            dr = m_oraCmd.ExecuteReader();
            while (dr.Read())
            {
                this.checkedListBox2.Items.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            this.checkedListBox3.Items.Add("丰度指数");
            this.checkedListBox3.Items.Add("植被覆盖指数");
            this.checkedListBox3.Items.Add("地质环境指数");
            this.checkedListBox3.Items.Add("土地退化指数");
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_comSelect = -1;
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }

            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.SetItemChecked(i, false);
            }
            m_comSelect = comboBox1.SelectedIndex;

        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (m_comSelect == 1)
            {
                for (int i = 0; i < checkedListBox1.Items.Count;i++)
                {
                    if (i != e.Index)
                    {
                        checkedListBox1.SetItemChecked(i, false);
                    }
                }
            }
        }

        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (m_comSelect == 0)
            {
                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        checkedListBox2.SetItemChecked(i, false);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Object[,] _data=null;
            IList<string> zs = new List<string>();
            if(this.m_comSelect==0)
            {
                IList<string> year=new List<string>();
                string kqName = "";

                for (int i = 0; i < checkedListBox1.Items.Count; i++) 
                {
                    if (checkedListBox1.GetItemChecked(i))
                    {
                        year.Add(checkedListBox1.Items[i].ToString());
                    }
                }

                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    if (checkedListBox2.GetItemChecked(i))
                    {
                        kqName = checkedListBox2.Items[i].ToString();
                    }
                }

                for (int i = 0; i < checkedListBox3.Items.Count; i++)
                {
                    if (checkedListBox3.GetItemChecked(i))
                    {
                        zs.Add(checkedListBox3.Items[i].ToString());
                    }
                }

                _data = new Object[year.Count, zs.Count];

                for (int i = 0; i < zs.Count;i++)
                {
                    _data[0, i] = zs[i].ToString();
                    for (int j = 0; j < year.Count;j++ )
                    {
                        m_oraCmd.CommandText = "select " + zsList[zs[i].ToString()].ToString() + " from [INDEX] WHERE [PLACE]='" + kqName.Trim() + "' and [YEARTIME]='" + year[j].ToString().Trim()+"'";
                        OleDbDataReader dr = m_oraCmd.ExecuteReader();
                        while (dr.Read())
                        {
                            _data[j,i] = dr.GetValue(0).ToString();
                        }
                        dr.Close();
                    }
                }
            }

            if (this.m_comSelect == 1)
            {
                IList<string> kq = new List<string>();
                string yearName = "";

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.GetItemChecked(i))
                    {
                        yearName = checkedListBox1.Items[i].ToString();
                    }
                }

                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    if (checkedListBox2.GetItemChecked(i))
                    {
                        kq.Add(checkedListBox2.Items[i].ToString());
                    }
                }

                for (int i = 0; i < checkedListBox3.Items.Count; i++)
                {
                    if (checkedListBox3.GetItemChecked(i))
                    {
                        zs.Add(checkedListBox3.Items[i].ToString());
                    }
                }

                _data = new Object[kq.Count, zs.Count];

                for (int i = 0; i < zs.Count; i++)
                {
                    _data[0,i] = zs[i].ToString();
                    for (int j = 0; j < kq.Count; j++)
                    {
                        m_oraCmd.CommandText = "select " + zsList[zs[i].ToString()].ToString() + " from [INDEX] WHERE [PLACE]='" + kq[j].ToString() + "' and [YEARTIME]='" + yearName.Trim() + "'";
                        OleDbDataReader dr = m_oraCmd.ExecuteReader();
                        while (dr.Read())
                        {
                            _data[j,i] = dr.GetValue(0).ToString();
                        }
                        dr.Close();
                    }
                }
            }
            if (_data==null)
            {
                return;
            }
            frmShowChart chart = new frmShowChart(_data,zs.Count);
            chart.ShowDialog();
        }
    }
}