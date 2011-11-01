using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;

namespace CoastalGIS.CallMap
{
    public partial class frmEditCallMap : Form
    {
        private OracleCommand m_oraCmd;

        public frmEditCallMap(OracleCommand oraCmd)
        {
            InitializeComponent();
            m_oraCmd = oraCmd;
        }

        private void frmEditCallMap_Load(object sender, EventArgs e)
        {
            string sqlText = "select Name from SHEETMETADATA";
            m_oraCmd.CommandText = sqlText;
            OracleDataReader dr = m_oraCmd.ExecuteReader();
            while (dr.Read()) 
            {
                this.listBox1.Items.Add(dr.GetValue(0).ToString());
            }

            sqlText = "select * from SHEETNOINDEX";
            m_oraCmd.CommandText = sqlText;
            dr = m_oraCmd.ExecuteReader();
            while (dr.Read())
            {
                this.listBox2.Items.Add(dr.GetValue(0).ToString());
            }
        }

        private void btnSelectbySingle_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex == -1) 
            {
                MessageBox.Show("请选择图幅名！");
                return;
            }
            string sqlText1="";
            string sqlText2 = "";
            OracleDataReader dr;
            try
            {
                for (int i = 0; i < this.listBox1.SelectedItems.Count; i++)
                {
                    sqlText1 = "select NAME,DATASET,DATASTRUCTURE from SHEETMETADATA where NAME='" + listBox1.SelectedItems[i].ToString() + "'";
                    m_oraCmd.CommandText = sqlText1;
                    dr = m_oraCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        sqlText2 = "insert into SHEETNOINDEX (NAME,DATASET,DATATYPE) values('" + dr.GetValue(0) + "','" + dr.GetValue(1) + "','" + dr.GetValue(2) + "')";
                        m_oraCmd.CommandText = sqlText2;
                        m_oraCmd.ExecuteNonQuery();
                        this.listBox2.Items.Add(listBox1.SelectedItems[i].ToString());
                    }
                }
            }
            catch (Exception ee) 
            {
                MessageBox.Show("该图幅已存在！");
            }

        }

        private void btnDeleteSeleBysingle_Click(object sender, EventArgs e)
        {
            if(this.listBox2.SelectedIndex == -1) 
            {
                MessageBox.Show("请选择图幅名！");
                return;
            }

            for (int i = 0; i < this.listBox2.SelectedItems.Count; i++) 
            {
                string sqlText="delete from SHEETNOINDEX where NAME='"+listBox2.SelectedItems[i].ToString()+"'";
                m_oraCmd.CommandText = sqlText;
                m_oraCmd.ExecuteNonQuery();
                this.listBox2.Items.Remove(listBox2.SelectedItems[i]);

            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}