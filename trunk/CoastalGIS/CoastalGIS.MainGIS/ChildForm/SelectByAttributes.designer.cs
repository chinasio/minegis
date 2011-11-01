namespace CoastalGIS.MainGIS
{
    partial class QueryForm
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
            this.FieldListbox = new System.Windows.Forms.ListBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn1 = new System.Windows.Forms.Button();
            this.btn10 = new System.Windows.Forms.Button();
            this.btn12 = new System.Windows.Forms.Button();
            this.btn11 = new System.Windows.Forms.Button();
            this.btn13 = new System.Windows.Forms.Button();
            this.btn9 = new System.Windows.Forms.Button();
            this.btn8 = new System.Windows.Forms.Button();
            this.btn7 = new System.Windows.Forms.Button();
            this.btn6 = new System.Windows.Forms.Button();
            this.btn5 = new System.Windows.Forms.Button();
            this.btn4 = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.QueryText = new System.Windows.Forms.TextBox();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.btn14 = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.officeFormAdorner1 = new Janus.Windows.Ribbon.OfficeFormAdorner(this.components);
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).BeginInit();
            this.SuspendLayout();
            // 
            // FieldListbox
            // 
            this.FieldListbox.FormattingEnabled = true;
            this.FieldListbox.ItemHeight = 12;
            this.FieldListbox.Location = new System.Drawing.Point(14, 48);
            this.FieldListbox.Name = "FieldListbox";
            this.FieldListbox.Size = new System.Drawing.Size(339, 76);
            this.FieldListbox.TabIndex = 0;
            this.FieldListbox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.FieldListbox_MouseDoubleClick);
            this.FieldListbox.SelectedIndexChanged += new System.EventHandler(this.FieldListbox_SelectedIndexChanged_1);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(106, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(247, 20);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "图层选择:";
            // 
            // btn1
            // 
            this.btn1.Location = new System.Drawing.Point(15, 142);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(45, 26);
            this.btn1.TabIndex = 3;
            this.btn1.Text = "=";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.btn1_Click);
            // 
            // btn10
            // 
            this.btn10.Location = new System.Drawing.Point(16, 238);
            this.btn10.Name = "btn10";
            this.btn10.Size = new System.Drawing.Size(45, 26);
            this.btn10.TabIndex = 4;
            this.btn10.Text = "Is";
            this.btn10.UseVisualStyleBackColor = true;
            this.btn10.Click += new System.EventHandler(this.btn10_Click);
            // 
            // btn12
            // 
            this.btn12.Location = new System.Drawing.Point(117, 238);
            this.btn12.Name = "btn12";
            this.btn12.Size = new System.Drawing.Size(45, 26);
            this.btn12.TabIndex = 5;
            this.btn12.Text = "Not";
            this.btn12.UseVisualStyleBackColor = true;
            this.btn12.Click += new System.EventHandler(this.btn12_Click);
            // 
            // btn11
            // 
            this.btn11.Location = new System.Drawing.Point(66, 238);
            this.btn11.Name = "btn11";
            this.btn11.Size = new System.Drawing.Size(45, 26);
            this.btn11.TabIndex = 6;
            this.btn11.Text = "()";
            this.btn11.UseVisualStyleBackColor = true;
            this.btn11.Click += new System.EventHandler(this.btn11_Click);
            // 
            // btn13
            // 
            this.btn13.Location = new System.Drawing.Point(15, 270);
            this.btn13.Name = "btn13";
            this.btn13.Size = new System.Drawing.Size(24, 26);
            this.btn13.TabIndex = 7;
            this.btn13.Text = "_";
            this.btn13.UseVisualStyleBackColor = true;
            this.btn13.Click += new System.EventHandler(this.btn13_Click);
            // 
            // btn9
            // 
            this.btn9.Location = new System.Drawing.Point(117, 206);
            this.btn9.Name = "btn9";
            this.btn9.Size = new System.Drawing.Size(45, 26);
            this.btn9.TabIndex = 8;
            this.btn9.Text = "Or";
            this.btn9.UseVisualStyleBackColor = true;
            this.btn9.Click += new System.EventHandler(this.btn9_Click);
            // 
            // btn8
            // 
            this.btn8.Location = new System.Drawing.Point(66, 206);
            this.btn8.Name = "btn8";
            this.btn8.Size = new System.Drawing.Size(45, 26);
            this.btn8.TabIndex = 9;
            this.btn8.Text = "<=";
            this.btn8.UseVisualStyleBackColor = true;
            this.btn8.Click += new System.EventHandler(this.btn8_Click);
            // 
            // btn7
            // 
            this.btn7.Location = new System.Drawing.Point(15, 206);
            this.btn7.Name = "btn7";
            this.btn7.Size = new System.Drawing.Size(45, 26);
            this.btn7.TabIndex = 10;
            this.btn7.Text = "<";
            this.btn7.UseVisualStyleBackColor = true;
            this.btn7.Click += new System.EventHandler(this.btn7_Click);
            // 
            // btn6
            // 
            this.btn6.Location = new System.Drawing.Point(117, 176);
            this.btn6.Name = "btn6";
            this.btn6.Size = new System.Drawing.Size(45, 26);
            this.btn6.TabIndex = 11;
            this.btn6.Text = "And";
            this.btn6.UseVisualStyleBackColor = true;
            this.btn6.Click += new System.EventHandler(this.btn6_Click);
            // 
            // btn5
            // 
            this.btn5.Location = new System.Drawing.Point(66, 174);
            this.btn5.Name = "btn5";
            this.btn5.Size = new System.Drawing.Size(45, 26);
            this.btn5.TabIndex = 12;
            this.btn5.Text = ">=";
            this.btn5.UseVisualStyleBackColor = true;
            this.btn5.Click += new System.EventHandler(this.btn5_Click);
            // 
            // btn4
            // 
            this.btn4.Location = new System.Drawing.Point(15, 174);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(45, 26);
            this.btn4.TabIndex = 13;
            this.btn4.Text = ">";
            this.btn4.UseVisualStyleBackColor = true;
            this.btn4.Click += new System.EventHandler(this.btn4_Click);
            // 
            // btn3
            // 
            this.btn3.Location = new System.Drawing.Point(117, 142);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(45, 26);
            this.btn3.TabIndex = 14;
            this.btn3.Text = "Like";
            this.btn3.UseVisualStyleBackColor = true;
            this.btn3.Click += new System.EventHandler(this.btn3_Click);
            // 
            // btn2
            // 
            this.btn2.Location = new System.Drawing.Point(66, 142);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(45, 26);
            this.btn2.TabIndex = 15;
            this.btn2.Text = "<>";
            this.btn2.UseVisualStyleBackColor = true;
            this.btn2.Click += new System.EventHandler(this.btn2_Click);
            // 
            // QueryText
            // 
            this.QueryText.Location = new System.Drawing.Point(12, 324);
            this.QueryText.Multiline = true;
            this.QueryText.Name = "QueryText";
            this.QueryText.Size = new System.Drawing.Size(288, 73);
            this.QueryText.TabIndex = 16;
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(306, 367);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(47, 30);
            this.BtnOK.TabIndex = 17;
            this.BtnOK.Text = "查 询";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(289, 426);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(69, 27);
            this.BtnClose.TabIndex = 19;
            this.BtnClose.Text = "关 闭";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // btn14
            // 
            this.btn14.Location = new System.Drawing.Point(38, 270);
            this.btn14.Name = "btn14";
            this.btn14.Size = new System.Drawing.Size(23, 26);
            this.btn14.TabIndex = 20;
            this.btn14.Text = "%";
            this.btn14.UseVisualStyleBackColor = true;
            this.btn14.Click += new System.EventHandler(this.btn14_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(306, 324);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(47, 31);
            this.btnClear.TabIndex = 21;
            this.btnClear.Text = "清 空";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // officeFormAdorner1
            // 
            this.officeFormAdorner1.ApplicationName = "属性查询";
            this.officeFormAdorner1.Form = this;
            this.officeFormAdorner1.Office2007CustomColor = System.Drawing.Color.Empty;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(178, 142);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(175, 124);
            this.listBox1.TabIndex = 22;
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDoubleClick);
            this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(178, 272);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(81, 27);
            this.button1.TabIndex = 23;
            this.button1.Text = "获取字段值";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 309);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 24;
            this.label2.Text = "查询语句";
            // 
            // QueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(370, 465);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btn14);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.QueryText);
            this.Controls.Add(this.btn2);
            this.Controls.Add(this.btn3);
            this.Controls.Add(this.btn4);
            this.Controls.Add(this.btn5);
            this.Controls.Add(this.btn6);
            this.Controls.Add(this.btn7);
            this.Controls.Add(this.btn8);
            this.Controls.Add(this.btn9);
            this.Controls.Add(this.btn13);
            this.Controls.Add(this.btn11);
            this.Controls.Add(this.btn12);
            this.Controls.Add(this.btn10);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.FieldListbox);
            this.ForeColor = System.Drawing.SystemColors.Highlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "QueryForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.QueryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.officeFormAdorner1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox FieldListbox;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btn10;
        private System.Windows.Forms.Button btn12;
        private System.Windows.Forms.Button btn11;
        private System.Windows.Forms.Button btn13;
        private System.Windows.Forms.Button btn9;
        private System.Windows.Forms.Button btn8;
        private System.Windows.Forms.Button btn7;
        private System.Windows.Forms.Button btn6;
        private System.Windows.Forms.Button btn5;
        private System.Windows.Forms.Button btn4;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.TextBox QueryText;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button btn14;
        private System.Windows.Forms.Button btnClear;
        private Janus.Windows.Ribbon.OfficeFormAdorner officeFormAdorner1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label2;
    }
}