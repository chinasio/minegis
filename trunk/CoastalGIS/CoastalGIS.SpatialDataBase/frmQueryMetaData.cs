using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data;
using System.Data.OracleClient;

namespace CoastalGIS.SpatialDataBase
{

	public class frmQueryMetaData : System.Windows.Forms.Form
    {
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnEqual;
		private System.Windows.Forms.Button btnLarger;
		private System.Windows.Forms.Button btnSmaller;
		private System.Windows.Forms.Button btnUnequal;
		private System.Windows.Forms.Button btnAnd;
        private System.Windows.Forms.Button btnOr;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		public System.Windows.Forms.TextBox txtquery;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button btnless;
        private System.Windows.Forms.Button btnbig;
        private System.Windows.Forms.Button btnlike;
        private Janus.Windows.Ribbon.OfficeFormAdorner officeFormAdorner1;
        private IContainer components;

        private OracleCommand m_oraCmd;
		public frmQueryMetaData(OracleCommand oraCmd)
		{
			InitializeComponent();
            this.m_oraCmd = oraCmd;
		}

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQueryMetaData));
            this.btnlike = new System.Windows.Forms.Button();
            this.btnbig = new System.Windows.Forms.Button();
            this.btnless = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnOr = new System.Windows.Forms.Button();
            this.btnAnd = new System.Windows.Forms.Button();
            this.btnUnequal = new System.Windows.Forms.Button();
            this.btnSmaller = new System.Windows.Forms.Button();
            this.btnLarger = new System.Windows.Forms.Button();
            this.btnEqual = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtquery = new System.Windows.Forms.TextBox();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.officeFormAdorner1 = new Janus.Windows.Ribbon.OfficeFormAdorner(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnlike
            // 
            this.btnlike.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnlike.Location = new System.Drawing.Point(225, 78);
            this.btnlike.Name = "btnlike";
            this.btnlike.Size = new System.Drawing.Size(40, 24);
            this.btnlike.TabIndex = 14;
            this.btnlike.Text = "like";
            this.btnlike.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnbig
            // 
            this.btnbig.Location = new System.Drawing.Point(185, 78);
            this.btnbig.Name = "btnbig";
            this.btnbig.Size = new System.Drawing.Size(40, 24);
            this.btnbig.TabIndex = 12;
            this.btnbig.Text = ">";
            this.btnbig.Click += new System.EventHandler(this.btnbig_Click);
            // 
            // btnless
            // 
            this.btnless.Location = new System.Drawing.Point(145, 78);
            this.btnless.Name = "btnless";
            this.btnless.Size = new System.Drawing.Size(40, 24);
            this.btnless.TabIndex = 11;
            this.btnless.Text = "<";
            this.btnless.Click += new System.EventHandler(this.btnless_Click);
            // 
            // listBox1
            // 
            this.listBox1.ItemHeight = 12;
            this.listBox1.Items.AddRange(new object[] {
            "NAME",
            "IMPORTTIME",
            "DATASTRUCTURE",
            "SOURCEFORMAT",
            "DATASET",
            "DESCRIPTION",
            "ALIANAME",
            "SOURCE"});
            this.listBox1.Location = new System.Drawing.Point(11, 30);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(126, 76);
            this.listBox1.TabIndex = 10;
            this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick_1);
            // 
            // btnOr
            // 
            this.btnOr.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOr.Location = new System.Drawing.Point(225, 54);
            this.btnOr.Name = "btnOr";
            this.btnOr.Size = new System.Drawing.Size(40, 24);
            this.btnOr.TabIndex = 8;
            this.btnOr.Text = "or";
            this.btnOr.Click += new System.EventHandler(this.btnOr_Click);
            // 
            // btnAnd
            // 
            this.btnAnd.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAnd.Location = new System.Drawing.Point(225, 30);
            this.btnAnd.Name = "btnAnd";
            this.btnAnd.Size = new System.Drawing.Size(40, 24);
            this.btnAnd.TabIndex = 7;
            this.btnAnd.Text = "And";
            this.btnAnd.Click += new System.EventHandler(this.btnAnd_Click);
            // 
            // btnUnequal
            // 
            this.btnUnequal.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnUnequal.Location = new System.Drawing.Point(185, 30);
            this.btnUnequal.Name = "btnUnequal";
            this.btnUnequal.Size = new System.Drawing.Size(40, 24);
            this.btnUnequal.TabIndex = 6;
            this.btnUnequal.Text = "<>";
            this.btnUnequal.Click += new System.EventHandler(this.btnUnequal_Click);
            // 
            // btnSmaller
            // 
            this.btnSmaller.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSmaller.Location = new System.Drawing.Point(185, 54);
            this.btnSmaller.Name = "btnSmaller";
            this.btnSmaller.Size = new System.Drawing.Size(40, 24);
            this.btnSmaller.TabIndex = 5;
            this.btnSmaller.Text = "<=";
            this.btnSmaller.Click += new System.EventHandler(this.btnSmaller_Click);
            // 
            // btnLarger
            // 
            this.btnLarger.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLarger.Location = new System.Drawing.Point(145, 54);
            this.btnLarger.Name = "btnLarger";
            this.btnLarger.Size = new System.Drawing.Size(40, 24);
            this.btnLarger.TabIndex = 4;
            this.btnLarger.Text = ">=";
            this.btnLarger.Click += new System.EventHandler(this.btnLarger_Click);
            // 
            // btnEqual
            // 
            this.btnEqual.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnEqual.Location = new System.Drawing.Point(145, 30);
            this.btnEqual.Name = "btnEqual";
            this.btnEqual.Size = new System.Drawing.Size(40, 24);
            this.btnEqual.TabIndex = 3;
            this.btnEqual.Text = "=";
            this.btnEqual.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "字段列表";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(11, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "查询语句";
            // 
            // txtquery
            // 
            this.txtquery.AcceptsReturn = true;
            this.txtquery.ContextMenu = this.contextMenu1;
            this.txtquery.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtquery.Location = new System.Drawing.Point(11, 138);
            this.txtquery.Multiline = true;
            this.txtquery.Name = "txtquery";
            this.txtquery.Size = new System.Drawing.Size(254, 64);
            this.txtquery.TabIndex = 0;
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "清除";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(45, 219);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(54, 30);
            this.button1.TabIndex = 2;
            this.button1.Text = "选 择";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(171, 219);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(54, 30);
            this.button2.TabIndex = 3;
            this.button2.Text = "取 消";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // officeFormAdorner1
            // 
            this.officeFormAdorner1.ApplicationName = "元数据选择";
            this.officeFormAdorner1.Form = this;
            this.officeFormAdorner1.Office2007CustomColor = System.Drawing.Color.Empty;
            // 
            // frmQueryMetaData
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(279, 254);
            this.Controls.Add(this.btnlike);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnbig);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnless);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnOr);
            this.Controls.Add(this.txtquery);
            this.Controls.Add(this.btnAnd);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnUnequal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSmaller);
            this.Controls.Add(this.btnEqual);
            this.Controls.Add(this.btnLarger);
            this.ForeColor = System.Drawing.SystemColors.Highlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmQueryMetaData";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		private void btnEqual_Click(object sender, System.EventArgs e)
		{
			txtquery.Text=txtquery.Text+" "+"=";
		}

		private void btnLarger_Click(object sender, System.EventArgs e)
		{
		    txtquery.Text=txtquery.Text+" "+">=";
		}

		private void btnSmaller_Click(object sender, System.EventArgs e)
		{
			txtquery.Text=txtquery.Text+" "+"<=";
		}

		private void btnUnequal_Click(object sender, System.EventArgs e)
		{
			txtquery.Text=txtquery.Text+" "+"<>";
		}

		private void btnAnd_Click(object sender, System.EventArgs e)
		{
			txtquery.Text=txtquery.Text+" "+"and"+" ";
		}

		private void btnOr_Click(object sender, System.EventArgs e)
		{
			txtquery.Text=txtquery.Text+" "+"or"+" ";
		}

		private void button1_Click(object sender, System.EventArgs e) //查询按钮
		{
            if (txtquery.Text.Trim() == "") 
            {
                MessageBox.Show("无查询语句！");
                return;
            }

            string sqlText = "select * from SHEETMETADATA where " + this.txtquery.Text;
            OracleDataAdapter oraAdapter=new OracleDataAdapter(sqlText,this.m_oraCmd.Connection);
            DataSet ds = new DataSet();
            oraAdapter.Fill(ds,"data");
            if (ds.Tables[0].Rows.Count == 0) 
            {
                MessageBox.Show("无查询结果！");
                return;
            }

            frmPrintMetaData frmMD = new frmPrintMetaData(ds);
            frmMD.Show();
            this.Close();

		}

		private void button2_Click(object sender, System.EventArgs e)
		{
            string sqlText = "select * from SHEETMETADATA";
            OracleDataAdapter oraAdapter = new OracleDataAdapter(sqlText, this.m_oraCmd.Connection);
            DataSet ds = new DataSet();
            oraAdapter.Fill(ds, "data");
            frmPrintMetaData frmMD = new frmPrintMetaData(ds);
            frmMD.Show();
            this.Close();
		}


		private void listBox1_DoubleClick_1(object sender, System.EventArgs e)
		{
			txtquery.Text=txtquery.Text+listBox1.SelectedItem.ToString();
		
		}

		private void btnless_Click(object sender, System.EventArgs e)
		{
		  txtquery.Text=txtquery.Text+" "+"<";
		}

		private void btnbig_Click(object sender, System.EventArgs e)
		{
			txtquery.Text=txtquery.Text+" "+">";
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			 txtquery.Text=txtquery.Text+" "+"like";
		
		}

	}
}
