using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MSChart20Lib;

namespace CoastalGIS.MainGIS
{
    public partial class frmShowChart : Form
    {
        private Object[,] m_data = null;
        private int m_legendLength = 0;

        public frmShowChart(Object[,] data,int length)
        {
            InitializeComponent();
            this.m_data = data;
            m_legendLength = length;
        }

        private void frmShowChart_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;

            //ͼ�����
            axMSChart1.TitleText = "ָ��ͳ��ͼ��";

            //����ͼ��
            axMSChart1.ShowLegend = true;
            axMSChart1.Legend.Location.LocationType = VtChLocationType.VtChLocationTypeRight;

            //��������ͼPlot

            //����Plot��Shadow
            axMSChart1.Plot.Backdrop.Shadow.Style = VtShadowStyle.VtShadowStyleDrop;
            axMSChart1.Plot.Backdrop.Shadow.Offset.Set(60, 60);//'����Shadow�Ĵ�С
            //����Plot�ı߿�
            axMSChart1.Plot.Backdrop.Frame.Style = VtFrameStyle.VtFrameStyleSingleLine;
            //����Plot�ı���ɫ
            axMSChart1.Plot.Backdrop.Fill.Style = VtFillStyle.VtFillStyleBrush;
            axMSChart1.Plot.Backdrop.Fill.Brush.FillColor.Set(255, 255, 255);

            //for (int i = 0; i < m_legendLength; i++)
            //{
            //    axMSChart1.Plot.SeriesCollection[(short)i]._LegendText = m_data[0, i].ToString();

            //}

            //axMSChart1.

            axMSChart1.ChartData = m_data;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex == 0) 
            {
                axMSChart1.chartType = VtChChartType.VtChChartType2dBar;
            }

            if (this.comboBox1.SelectedIndex == 1)
            {
                axMSChart1.chartType = VtChChartType.VtChChartType2dPie;
            }

            if (this.comboBox1.SelectedIndex == 2)
            {
                axMSChart1.chartType = VtChChartType.VtChChartType2dLine;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //MSChart.EditCopy();

            //System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
            //  sfd.Filter = "BMP�ļ�|*.bmp|JPEG�ļ�|*.jpg";
            //  if (sfd.ShowDialog() == DialogResult.OK)
            //  {
            //      if (sfd.FilterIndex == 1)
            //      {
            //          im.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            //      }
            //      else 
            //      {
            //          im.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            //      }

            //  }
            //this.axMSChart1.EditCopy();
            //Bitmap chartCapture =
            //  (Bitmap)Clipboard.GetDataObject().GetData("Bitmap", true);
            //chartCapture.Save("Image.Jpeg");

        }
    }
}