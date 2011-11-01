namespace CoastalGIS.CallMap
{
    partial class frmOpenMapByCoordinate
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtXcood = new System.Windows.Forms.TextBox();
            this.txtYCood = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lstVwCood = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSvGrpCood = new System.Windows.Forms.Button();
            this.btnOpGrpCod = new System.Windows.Forms.Button();
            this.btnDetCood = new System.Windows.Forms.Button();
            this.btnAdCood = new System.Windows.Forms.Button();
            this.btnCancle = new System.Windows.Forms.Button();
            this.btnOpnMap = new System.Windows.Forms.Button();
            this.txtEdCdY = new System.Windows.Forms.TextBox();
            this.txtEdCdX = new System.Windows.Forms.TextBox();
            this.officeFormAdorner1 = new Janus.Windows.Ribbon.OfficeFormAdorner(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "X坐标";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Y坐标";
            // 
            // txtXcood
            // 
            this.txtXcood.Location = new System.Drawing.Point(27, 35);
            this.txtXcood.Name = "txtXcood";
            this.txtXcood.Size = new System.Drawing.Size(190, 21);
            this.txtXcood.TabIndex = 2;
            // 
            // txtYCood
            // 
            this.txtYCood.Location = new System.Drawing.Point(27, 74);
            this.txtYCood.Name = "txtYCood";
            this.txtYCood.Size = new System.Drawing.Size(190, 21);
            this.txtYCood.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lstVwCood);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnSvGrpCood);
            this.groupBox1.Controls.Add(this.btnOpGrpCod);
            this.groupBox1.Controls.Add(this.btnDetCood);
            this.groupBox1.Controls.Add(this.btnAdCood);
            this.groupBox1.Controls.Add(this.btnCancle);
            this.groupBox1.Controls.Add(this.txtYCood);
            this.groupBox1.Controls.Add(this.btnOpnMap);
            this.groupBox1.Controls.Add(this.txtXcood);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(18, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(558, 259);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "请输入坐标";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 191);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(227, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "_____________________________________";
            // 
            // lstVwCood
            // 
            this.lstVwCood.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lstVwCood.AllowDrop = true;
            this.lstVwCood.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lstVwCood.GridLines = true;
            this.lstVwCood.HoverSelection = true;
            this.lstVwCood.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lstVwCood.LabelEdit = true;
            this.lstVwCood.Location = new System.Drawing.Point(263, 46);
            this.lstVwCood.MultiSelect = false;
            this.lstVwCood.Name = "lstVwCood";
            this.lstVwCood.Size = new System.Drawing.Size(272, 193);
            this.lstVwCood.TabIndex = 13;
            this.lstVwCood.UseCompatibleStateImageBehavior = false;
            this.lstVwCood.View = System.Windows.Forms.View.Details;
            this.lstVwCood.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstVwCood_MouseDoubleClick);
            this.lstVwCood.MouseHover += new System.EventHandler(this.lstVwCood_MouseHover);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "点号";
            this.columnHeader1.Width = 50;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "X坐标";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 110;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Y坐标";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 110;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(261, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "您已经输入了以下坐标：";
            // 
            // btnSvGrpCood
            // 
            this.btnSvGrpCood.Location = new System.Drawing.Point(131, 159);
            this.btnSvGrpCood.Name = "btnSvGrpCood";
            this.btnSvGrpCood.Size = new System.Drawing.Size(86, 31);
            this.btnSvGrpCood.TabIndex = 11;
            this.btnSvGrpCood.Text = "保存坐标组";
            this.btnSvGrpCood.UseVisualStyleBackColor = true;
            this.btnSvGrpCood.Click += new System.EventHandler(this.btnSvGrpCood_Click);
            // 
            // btnOpGrpCod
            // 
            this.btnOpGrpCod.Location = new System.Drawing.Point(27, 159);
            this.btnOpGrpCod.Name = "btnOpGrpCod";
            this.btnOpGrpCod.Size = new System.Drawing.Size(86, 31);
            this.btnOpGrpCod.TabIndex = 10;
            this.btnOpGrpCod.Text = "打开坐标组";
            this.btnOpGrpCod.UseVisualStyleBackColor = true;
            this.btnOpGrpCod.Click += new System.EventHandler(this.btnOpGrpCod_Click);
            // 
            // btnDetCood
            // 
            this.btnDetCood.Location = new System.Drawing.Point(131, 110);
            this.btnDetCood.Name = "btnDetCood";
            this.btnDetCood.Size = new System.Drawing.Size(86, 31);
            this.btnDetCood.TabIndex = 9;
            this.btnDetCood.Text = "删除坐标";
            this.btnDetCood.UseVisualStyleBackColor = true;
            this.btnDetCood.Click += new System.EventHandler(this.btnDetCood_Click);
            // 
            // btnAdCood
            // 
            this.btnAdCood.Location = new System.Drawing.Point(27, 110);
            this.btnAdCood.Name = "btnAdCood";
            this.btnAdCood.Size = new System.Drawing.Size(86, 31);
            this.btnAdCood.TabIndex = 7;
            this.btnAdCood.Text = "添加坐标";
            this.btnAdCood.UseVisualStyleBackColor = true;
            this.btnAdCood.Click += new System.EventHandler(this.btnAdCood_Click);
            // 
            // btnCancle
            // 
            this.btnCancle.Location = new System.Drawing.Point(131, 222);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(86, 31);
            this.btnCancle.TabIndex = 6;
            this.btnCancle.Text = "取 消 ";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnEnd_Click);
            // 
            // btnOpnMap
            // 
            this.btnOpnMap.Location = new System.Drawing.Point(27, 222);
            this.btnOpnMap.Name = "btnOpnMap";
            this.btnOpnMap.Size = new System.Drawing.Size(86, 31);
            this.btnOpnMap.TabIndex = 5;
            this.btnOpnMap.Text = "调 图";
            this.btnOpnMap.UseVisualStyleBackColor = true;
            this.btnOpnMap.Click += new System.EventHandler(this.btnOpnMap_Click);
            // 
            // txtEdCdY
            // 
            this.txtEdCdY.Location = new System.Drawing.Point(443, 276);
            this.txtEdCdY.Name = "txtEdCdY";
            this.txtEdCdY.Size = new System.Drawing.Size(110, 21);
            this.txtEdCdY.TabIndex = 15;
            this.txtEdCdY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEdCdY_KeyDown);
            // 
            // txtEdCdX
            // 
            this.txtEdCdX.Location = new System.Drawing.Point(292, 276);
            this.txtEdCdX.Name = "txtEdCdX";
            this.txtEdCdX.Size = new System.Drawing.Size(110, 21);
            this.txtEdCdX.TabIndex = 14;
            this.txtEdCdX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEdCdX_KeyDown);
            // 
            // officeFormAdorner1
            // 
            this.officeFormAdorner1.ApplicationName = "坐标调图";
            this.officeFormAdorner1.Form = this;
            this.officeFormAdorner1.Office2007CustomColor = System.Drawing.Color.Empty;
            // 
            // frmOpenMapByCoordinate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(588, 298);
            this.Controls.Add(this.txtEdCdY);
            this.Controls.Add(this.txtEdCdX);
            this.Controls.Add(this.groupBox1);
            this.ForeColor = System.Drawing.SystemColors.Highlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOpenMapByCoordinate";
            this.Load += new System.EventHandler(this.frmCopenMap_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtXcood;
        private System.Windows.Forms.TextBox txtYCood;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOpnMap;
        private System.Windows.Forms.Button btnCancle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSvGrpCood;
        private System.Windows.Forms.Button btnOpGrpCod;
        private System.Windows.Forms.Button btnDetCood;
        private System.Windows.Forms.Button btnAdCood;
        private System.Windows.Forms.ListView lstVwCood;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox txtEdCdY;
        private System.Windows.Forms.TextBox txtEdCdX;
        private System.Windows.Forms.Label label4;
        private Janus.Windows.Ribbon.OfficeFormAdorner officeFormAdorner1;
    }
}