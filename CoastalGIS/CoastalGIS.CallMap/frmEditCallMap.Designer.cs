namespace CoastalGIS.CallMap
{
    partial class frmEditCallMap
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
            this.officeFormAdorner1 = new Janus.Windows.Ribbon.OfficeFormAdorner(this.components);
            this.uiGroupBox1 = new Janus.Windows.EditControls.UIGroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.uiGroupBox2 = new Janus.Windows.EditControls.UIGroupBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSelectbySingle = new System.Windows.Forms.Button();
            this.btnDeleteSeleBysingle = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiGroupBox1)).BeginInit();
            this.uiGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiGroupBox2)).BeginInit();
            this.uiGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // officeFormAdorner1
            // 
            this.officeFormAdorner1.ApplicationName = "编辑图幅名";
            this.officeFormAdorner1.Form = this;
            this.officeFormAdorner1.Office2007CustomColor = System.Drawing.Color.Empty;
            // 
            // uiGroupBox1
            // 
            this.uiGroupBox1.Controls.Add(this.listBox1);
            this.uiGroupBox1.Location = new System.Drawing.Point(12, 12);
            this.uiGroupBox1.Name = "uiGroupBox1";
            this.uiGroupBox1.Size = new System.Drawing.Size(130, 273);
            this.uiGroupBox1.TabIndex = 0;
            this.uiGroupBox1.Text = "可选图幅名";
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(3, 17);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox1.Size = new System.Drawing.Size(124, 244);
            this.listBox1.TabIndex = 0;
            // 
            // uiGroupBox2
            // 
            this.uiGroupBox2.Controls.Add(this.listBox2);
            this.uiGroupBox2.Location = new System.Drawing.Point(204, 12);
            this.uiGroupBox2.Name = "uiGroupBox2";
            this.uiGroupBox2.Size = new System.Drawing.Size(130, 273);
            this.uiGroupBox2.TabIndex = 1;
            this.uiGroupBox2.Text = "已选图幅名";
            // 
            // listBox2
            // 
            this.listBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Location = new System.Drawing.Point(3, 17);
            this.listBox2.Name = "listBox2";
            this.listBox2.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox2.Size = new System.Drawing.Size(124, 244);
            this.listBox2.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(135, 310);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 33);
            this.button1.TabIndex = 2;
            this.button1.Text = "确 定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSelectbySingle
            // 
            this.btnSelectbySingle.Location = new System.Drawing.Point(145, 90);
            this.btnSelectbySingle.Name = "btnSelectbySingle";
            this.btnSelectbySingle.Size = new System.Drawing.Size(49, 23);
            this.btnSelectbySingle.TabIndex = 11;
            this.btnSelectbySingle.Text = ">";
            this.btnSelectbySingle.UseVisualStyleBackColor = true;
            this.btnSelectbySingle.Click += new System.EventHandler(this.btnSelectbySingle_Click);
            // 
            // btnDeleteSeleBysingle
            // 
            this.btnDeleteSeleBysingle.Location = new System.Drawing.Point(148, 180);
            this.btnDeleteSeleBysingle.Name = "btnDeleteSeleBysingle";
            this.btnDeleteSeleBysingle.Size = new System.Drawing.Size(49, 23);
            this.btnDeleteSeleBysingle.TabIndex = 13;
            this.btnDeleteSeleBysingle.Text = "<";
            this.btnDeleteSeleBysingle.UseVisualStyleBackColor = true;
            this.btnDeleteSeleBysingle.Click += new System.EventHandler(this.btnDeleteSeleBysingle_Click);
            // 
            // frmEditCallMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(346, 355);
            this.Controls.Add(this.btnDeleteSeleBysingle);
            this.Controls.Add(this.btnSelectbySingle);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.uiGroupBox2);
            this.Controls.Add(this.uiGroupBox1);
            this.ForeColor = System.Drawing.SystemColors.Highlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmEditCallMap";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.frmEditCallMap_Load);
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiGroupBox1)).EndInit();
            this.uiGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiGroupBox2)).EndInit();
            this.uiGroupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Janus.Windows.Ribbon.OfficeFormAdorner officeFormAdorner1;
        private Janus.Windows.EditControls.UIGroupBox uiGroupBox2;
        private System.Windows.Forms.ListBox listBox2;
        private Janus.Windows.EditControls.UIGroupBox uiGroupBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnDeleteSeleBysingle;
        private System.Windows.Forms.Button btnSelectbySingle;
        private System.Windows.Forms.Button button1;
    }
}