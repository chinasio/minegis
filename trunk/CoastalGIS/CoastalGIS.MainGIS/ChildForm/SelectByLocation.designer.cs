namespace CoastalGIS.MainGIS
{
    partial class SpatialQueryForm
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
            this.LayerCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnApply = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.MethordList = new System.Windows.Forms.ListBox();
            this.officeFormAdorner1 = new Janus.Windows.Ribbon.OfficeFormAdorner(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).BeginInit();
            this.SuspendLayout();
            // 
            // LayerCheckedListBox
            // 
            this.LayerCheckedListBox.FormattingEnabled = true;
            this.LayerCheckedListBox.Location = new System.Drawing.Point(12, 37);
            this.LayerCheckedListBox.Name = "LayerCheckedListBox";
            this.LayerCheckedListBox.Size = new System.Drawing.Size(375, 132);
            this.LayerCheckedListBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 343);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(10, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(231, 14);
            this.label2.TabIndex = 6;
            this.label2.Text = "从下列图层中选择需要查询的图层：";
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(264, 328);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(69, 27);
            this.BtnClose.TabIndex = 21;
            this.BtnClose.Text = "退 出";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnApply
            // 
            this.BtnApply.Location = new System.Drawing.Point(70, 328);
            this.BtnApply.Name = "BtnApply";
            this.BtnApply.Size = new System.Drawing.Size(69, 27);
            this.BtnApply.TabIndex = 20;
            this.BtnApply.Text = "查 询";
            this.BtnApply.UseVisualStyleBackColor = true;
            this.BtnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(9, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 14);
            this.label3.TabIndex = 22;
            this.label3.Text = "空间关系选择：";
            // 
            // MethordList
            // 
            this.MethordList.FormattingEnabled = true;
            this.MethordList.ItemHeight = 12;
            this.MethordList.Items.AddRange(new object[] {
            "与查询几何对象关系未定义",
            "与查询几何对象相交",
            "与查询几何对象包络线相交",
            "与查询几何对象边界处相接",
            "与查询几何对象同维且相叠加",
            "与查询几何对象相交，面与面无此关系",
            "包含查询几何对象",
            "被查询几何对象包含"});
            this.MethordList.Location = new System.Drawing.Point(12, 202);
            this.MethordList.Name = "MethordList";
            this.MethordList.Size = new System.Drawing.Size(375, 112);
            this.MethordList.TabIndex = 23;
            // 
            // officeFormAdorner1
            // 
            this.officeFormAdorner1.Form = this;
            this.officeFormAdorner1.Office2007CustomColor = System.Drawing.Color.Empty;
            // 
            // SpatialQueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(405, 364);
            this.Controls.Add(this.MethordList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnApply);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LayerCheckedListBox);
            this.ForeColor = System.Drawing.SystemColors.Highlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SpatialQueryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "空间查询";
            this.Load += new System.EventHandler(this.SelectByLocation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox LayerCheckedListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnApply;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox MethordList;
        private Janus.Windows.Ribbon.OfficeFormAdorner officeFormAdorner1;
    }
}