namespace CoastalGIS.ExportMapProj
{
    partial class frmSetMapScale
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem1 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem2 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem3 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem4 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem5 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem6 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem7 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem8 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem9 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem10 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem11 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem12 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem13 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem14 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem15 = new Janus.Windows.EditControls.UIComboBoxItem();
            Janus.Windows.EditControls.UIComboBoxItem uiComboBoxItem16 = new Janus.Windows.EditControls.UIComboBoxItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBestMapScale = new System.Windows.Forms.TextBox();
            this.btnNewMapScale = new System.Windows.Forms.Button();
            this.btnBestMapScale = new System.Windows.Forms.Button();
            this.btnCancle = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNowMapScale = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.uiComboBoxMapscale = new Janus.Windows.EditControls.UIComboBox();
            this.officeFormAdorner1 = new Janus.Windows.Ribbon.OfficeFormAdorner(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "自定义比例尺";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "最适宜比例尺";
            // 
            // txtBestMapScale
            // 
            this.txtBestMapScale.Enabled = false;
            this.txtBestMapScale.Location = new System.Drawing.Point(107, 103);
            this.txtBestMapScale.Name = "txtBestMapScale";
            this.txtBestMapScale.Size = new System.Drawing.Size(186, 21);
            this.txtBestMapScale.TabIndex = 3;
            // 
            // btnNewMapScale
            // 
            this.btnNewMapScale.Location = new System.Drawing.Point(176, 152);
            this.btnNewMapScale.Name = "btnNewMapScale";
            this.btnNewMapScale.Size = new System.Drawing.Size(117, 23);
            this.btnNewMapScale.TabIndex = 4;
            this.btnNewMapScale.Text = "设置为自定义比例";
            this.btnNewMapScale.UseVisualStyleBackColor = true;
            this.btnNewMapScale.Click += new System.EventHandler(this.btnNewMapScale_Click);
            // 
            // btnBestMapScale
            // 
            this.btnBestMapScale.Location = new System.Drawing.Point(176, 190);
            this.btnBestMapScale.Name = "btnBestMapScale";
            this.btnBestMapScale.Size = new System.Drawing.Size(117, 23);
            this.btnBestMapScale.TabIndex = 5;
            this.btnBestMapScale.Text = "设置为最适宜比例";
            this.btnBestMapScale.UseVisualStyleBackColor = true;
            this.btnBestMapScale.Click += new System.EventHandler(this.btnBestMapScale_Click);
            // 
            // btnCancle
            // 
            this.btnCancle.Location = new System.Drawing.Point(176, 228);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(117, 23);
            this.btnCancle.TabIndex = 6;
            this.btnCancle.Text = "取  消";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "目前比例尺";
            // 
            // txtNowMapScale
            // 
            this.txtNowMapScale.Location = new System.Drawing.Point(107, 21);
            this.txtNowMapScale.Name = "txtNowMapScale";
            this.txtNowMapScale.Size = new System.Drawing.Size(186, 21);
            this.txtNowMapScale.TabIndex = 8;
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox3.Location = new System.Drawing.Point(12, 152);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(143, 99);
            this.textBox3.TabIndex = 9;
            this.textBox3.Text = "这里的最适宜比例尺指的是比例尺为整数，且使要打印的图形区域放大到最清晰化，这样有利于在地图上进行量算。\r\n";
            // 
            // uiComboBoxMapscale
            // 
            uiComboBoxItem1.FormatStyle.Alpha = 0;
            uiComboBoxItem1.IsSeparator = false;
            uiComboBoxItem1.Text = "1:1000";
            uiComboBoxItem2.FormatStyle.Alpha = 0;
            uiComboBoxItem2.IsSeparator = false;
            uiComboBoxItem2.Text = "1:5000";
            uiComboBoxItem3.FormatStyle.Alpha = 0;
            uiComboBoxItem3.IsSeparator = false;
            uiComboBoxItem3.Text = "1:10000";
            uiComboBoxItem4.FormatStyle.Alpha = 0;
            uiComboBoxItem4.IsSeparator = false;
            uiComboBoxItem4.Text = "1:20000";
            uiComboBoxItem5.FormatStyle.Alpha = 0;
            uiComboBoxItem5.IsSeparator = false;
            uiComboBoxItem5.Text = "1:50000";
            uiComboBoxItem6.FormatStyle.Alpha = 0;
            uiComboBoxItem6.IsSeparator = false;
            uiComboBoxItem6.Text = "1:100000";
            uiComboBoxItem7.FormatStyle.Alpha = 0;
            uiComboBoxItem7.IsSeparator = false;
            uiComboBoxItem7.Text = "1:200000";
            uiComboBoxItem8.FormatStyle.Alpha = 0;
            uiComboBoxItem8.IsSeparator = false;
            uiComboBoxItem8.Text = "1:300000";
            uiComboBoxItem9.FormatStyle.Alpha = 0;
            uiComboBoxItem9.IsSeparator = false;
            uiComboBoxItem9.Text = "1:500000";
            uiComboBoxItem10.FormatStyle.Alpha = 0;
            uiComboBoxItem10.IsSeparator = false;
            uiComboBoxItem10.Text = "1:600000";
            uiComboBoxItem11.FormatStyle.Alpha = 0;
            uiComboBoxItem11.IsSeparator = false;
            uiComboBoxItem11.Text = "1:700000";
            uiComboBoxItem12.FormatStyle.Alpha = 0;
            uiComboBoxItem12.IsSeparator = false;
            uiComboBoxItem12.Text = "1:1000000";
            uiComboBoxItem13.FormatStyle.Alpha = 0;
            uiComboBoxItem13.IsSeparator = false;
            uiComboBoxItem13.Text = "1:2000000";
            uiComboBoxItem14.FormatStyle.Alpha = 0;
            uiComboBoxItem14.IsSeparator = false;
            uiComboBoxItem14.Text = "1:3000000";
            uiComboBoxItem15.FormatStyle.Alpha = 0;
            uiComboBoxItem15.IsSeparator = false;
            uiComboBoxItem15.Text = "1:5000000";
            uiComboBoxItem16.FormatStyle.Alpha = 0;
            uiComboBoxItem16.IsSeparator = false;
            uiComboBoxItem16.Text = "1:10000000";
            this.uiComboBoxMapscale.Items.AddRange(new Janus.Windows.EditControls.UIComboBoxItem[] {
            uiComboBoxItem1,
            uiComboBoxItem2,
            uiComboBoxItem3,
            uiComboBoxItem4,
            uiComboBoxItem5,
            uiComboBoxItem6,
            uiComboBoxItem7,
            uiComboBoxItem8,
            uiComboBoxItem9,
            uiComboBoxItem10,
            uiComboBoxItem11,
            uiComboBoxItem12,
            uiComboBoxItem13,
            uiComboBoxItem14,
            uiComboBoxItem15,
            uiComboBoxItem16});
            this.uiComboBoxMapscale.Location = new System.Drawing.Point(107, 57);
            this.uiComboBoxMapscale.Name = "uiComboBoxMapscale";
            this.uiComboBoxMapscale.Size = new System.Drawing.Size(186, 21);
            this.uiComboBoxMapscale.TabIndex = 10;
            this.uiComboBoxMapscale.VisualStyle = Janus.Windows.UI.VisualStyle.Office2007;
            this.uiComboBoxMapscale.SelectedIndexChanged += new System.EventHandler(this.uiComboBoxMapscale_SelectedIndexChanged);
            // 
            // officeFormAdorner1
            // 
            this.officeFormAdorner1.ApplicationName = "比例尺设置";
            this.officeFormAdorner1.Form = this;
            this.officeFormAdorner1.Office2007CustomColor = System.Drawing.Color.Empty;
            // 
            // frmSetMapScale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(309, 259);
            this.Controls.Add(this.uiComboBoxMapscale);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.txtNowMapScale);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnBestMapScale);
            this.Controls.Add(this.btnNewMapScale);
            this.Controls.Add(this.txtBestMapScale);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.SystemColors.Highlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "frmSetMapScale";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.frmSetMapScale_Load);
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBestMapScale;
        private System.Windows.Forms.Button btnNewMapScale;
        private System.Windows.Forms.Button btnBestMapScale;
        private System.Windows.Forms.Button btnCancle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNowMapScale;
        private System.Windows.Forms.TextBox textBox3;
        private Janus.Windows.EditControls.UIComboBox uiComboBoxMapscale;
        private Janus.Windows.Ribbon.OfficeFormAdorner officeFormAdorner1;
    }
}