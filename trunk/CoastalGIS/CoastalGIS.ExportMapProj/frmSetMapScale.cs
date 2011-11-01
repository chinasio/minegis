using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CoastalGIS.ExportMapProj
{
    public partial class frmSetMapScale : Form
    {
        private double  m_mapScale;
        private bool  ifNewMapScale=false ;
        private bool  ifBestMapScale=false ;
     


        public frmSetMapScale()
        {
            InitializeComponent();
        }

        private void frmSetMapScale_Load(object sender, EventArgs e)
        {

        }
        public double setMapScale(double pOldMapScale ,double pNowMapScale)
        {
            int aft=0; //计数比例尺分母的零的个数；
            int pmax=0;
            double pbef = pOldMapScale;
            this.txtNowMapScale.Text = "1:" + pNowMapScale.ToString();
            double pTempMS = 0.0;
            while  (pbef>10)
            {
                pbef = pbef / 10;
                aft++;
            }
            switch (aft )
            {
                case 1:
                    pmax =Convert .ToInt32 ( pOldMapScale / 10);
                    pTempMS = Convert.ToDouble(pmax * 10);
                    if (pTempMS > pOldMapScale)
                    {
                        this.txtBestMapScale.Text = "1:" + pTempMS.ToString();
                    }
                    else
                    {
                        this.txtBestMapScale.Text = "1:" + (pTempMS + 10).ToString();
                    }
                    break;
                case 2:
                    pmax =Convert .ToInt32 ( pOldMapScale / 100);
                    pTempMS = Convert.ToDouble(pmax * 100);
                    if (pTempMS > pOldMapScale)
                    {
                        this.txtBestMapScale.Text = "1:" + pTempMS.ToString();
                    }
                    else
                    {
                        this.txtBestMapScale.Text = "1:" + (pTempMS + 100).ToString();
                    }
                    break;
                case 3:
                    pmax =Convert .ToInt32 ( pOldMapScale / 1000);
                    pTempMS = Convert.ToDouble(pmax * 1000);
                    if (pTempMS > pOldMapScale)
                    {
                        this.txtBestMapScale.Text = "1:" + pTempMS.ToString();
                    }
                    else
                    {
                        this.txtBestMapScale.Text = "1:" + (pTempMS + 1000).ToString();
                    }
                    break;
                case 4:
                    pmax =Convert .ToInt32 ( pOldMapScale / 10000);
                    pTempMS = Convert.ToDouble(pmax * 10000);
                    if (pTempMS > pOldMapScale)
                    {
                        this.txtBestMapScale.Text = "1:" + pTempMS.ToString();
                    }
                    else
                    {
                        this.txtBestMapScale.Text = "1:" + (pTempMS + 10000).ToString();
                    }
                    break;
                case 5:
                    pmax =Convert .ToInt32 ( pOldMapScale / 100000);
                    pTempMS = Convert.ToDouble(pmax * 100000);
                    if (pTempMS > pOldMapScale)
                    {
                        this.txtBestMapScale.Text = "1:" + pTempMS.ToString();
                    }
                    else
                    {
                        this.txtBestMapScale.Text = "1:" + (pTempMS + 100000).ToString();
                    }
                    break;
                case 6:
                    pmax =Convert .ToInt32 ( pOldMapScale / 1000000);
                    pTempMS = Convert.ToDouble(pmax * 1000000);
                    if (pTempMS > pOldMapScale)
                    {
                        this.txtBestMapScale.Text = "1:" + pTempMS.ToString();
                    }
                    else
                    {
                        this.txtBestMapScale.Text = "1:" + (pTempMS + 1000000).ToString();
                    }
                    break;
                case 7:
                    pmax =Convert .ToInt32 ( pOldMapScale / 10000000);
                    pTempMS = Convert.ToDouble(pmax * 10000000);
                    if (pTempMS > pOldMapScale)
                    {
                        this.txtBestMapScale.Text = "1:" + pTempMS.ToString();
                    }
                    else
                    {
                        this.txtBestMapScale.Text = "1:" + (pTempMS + 10000000).ToString();
                    }
                    break;
            }

            this.ShowDialog();

            if (ifNewMapScale)
            {
                m_mapScale = Convert.ToDouble(this.uiComboBoxMapscale.SelectedItem.Text.Substring(2, this.uiComboBoxMapscale.SelectedItem.Text.Length-2));
            }
           else  if (ifBestMapScale)
            {
                m_mapScale = Convert.ToDouble(this.txtBestMapScale.Text.Substring(2, this.txtBestMapScale.Text.Length - 2));
            }
            else
            {
                m_mapScale = pNowMapScale ;
            }

            return m_mapScale;
        }

        private void uiComboBoxMapscale_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void btnNewMapScale_Click(object sender, EventArgs e)
        {
            if (this.uiComboBoxMapscale.SelectedItem == null)
            {
                MessageBox.Show("请选择需要自定义的比例尺", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ifNewMapScale = true;
                this.Close();
            }
        }

        private void btnBestMapScale_Click(object sender, EventArgs e)
        {
            ifBestMapScale = true;
            this.Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}