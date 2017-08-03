namespace WindowsFormsApplication2
{
    partial class mainForm
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
            this.fill = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.excelButton = new System.Windows.Forms.Button();
            this.testButton = new System.Windows.Forms.Button();
            this.testread = new System.Windows.Forms.Button();
            this.marketFileTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mainWebBrowser = new System.Windows.Forms.WebBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.ggTextBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // fill
            // 
            this.fill.Location = new System.Drawing.Point(849, 1);
            this.fill.Margin = new System.Windows.Forms.Padding(4);
            this.fill.Name = "fill";
            this.fill.Size = new System.Drawing.Size(100, 29);
            this.fill.TabIndex = 0;
            this.fill.Text = "填单子";
            this.fill.UseVisualStyleBackColor = true;
            this.fill.Click += new System.EventHandler(this.button1_Click);
            // 
            // excelButton
            // 
            this.excelButton.Location = new System.Drawing.Point(0, -2);
            this.excelButton.Margin = new System.Windows.Forms.Padding(4);
            this.excelButton.Name = "excelButton";
            this.excelButton.Size = new System.Drawing.Size(169, 29);
            this.excelButton.TabIndex = 3;
            this.excelButton.Text = "选择市场部数据文件";
            this.excelButton.UseVisualStyleBackColor = true;
            this.excelButton.Click += new System.EventHandler(this.excelButton_click);
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(0, 38);
            this.testButton.Margin = new System.Windows.Forms.Padding(4);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(100, 29);
            this.testButton.TabIndex = 4;
            this.testButton.Text = "测试用";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_click);
            // 
            // testread
            // 
            this.testread.Location = new System.Drawing.Point(0, 74);
            this.testread.Margin = new System.Windows.Forms.Padding(4);
            this.testread.Name = "testread";
            this.testread.Size = new System.Drawing.Size(100, 29);
            this.testread.TabIndex = 5;
            this.testread.Text = "测试读excel";
            this.testread.UseVisualStyleBackColor = true;
            this.testread.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // marketFileTextBox
            // 
            this.marketFileTextBox.Enabled = false;
            this.marketFileTextBox.Location = new System.Drawing.Point(177, 0);
            this.marketFileTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.marketFileTextBox.Name = "marketFileTextBox";
            this.marketFileTextBox.Size = new System.Drawing.Size(221, 25);
            this.marketFileTextBox.TabIndex = 6;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1074, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mainWebBrowser
            // 
            this.mainWebBrowser.Location = new System.Drawing.Point(0, 35);
            this.mainWebBrowser.Margin = new System.Windows.Forms.Padding(4);
            this.mainWebBrowser.MinimumSize = new System.Drawing.Size(27, 25);
            this.mainWebBrowser.Name = "mainWebBrowser";
            this.mainWebBrowser.ScriptErrorsSuppressed = true;
            this.mainWebBrowser.Size = new System.Drawing.Size(1074, 406);
            this.mainWebBrowser.TabIndex = 1;
            this.mainWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.mainWebBrowser_DocumentCompleted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(120, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 15);
            this.label1.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(408, 0);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(171, 29);
            this.button1.TabIndex = 8;
            this.button1.Text = "选择公告页数据文件";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // ggTextBox
            // 
            this.ggTextBox.Enabled = false;
            this.ggTextBox.Location = new System.Drawing.Point(587, 2);
            this.ggTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.ggTextBox.Name = "ggTextBox";
            this.ggTextBox.Size = new System.Drawing.Size(253, 25);
            this.ggTextBox.TabIndex = 9;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(0, 110);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 29);
            this.button2.TabIndex = 10;
            this.button2.Text = "测试3";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(0, 437);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(1074, 94);
            this.textBox1.TabIndex = 11;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1074, 529);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.ggTextBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.marketFileTextBox);
            this.Controls.Add(this.testread);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.excelButton);
            this.Controls.Add(this.fill);
            this.Controls.Add(this.mainWebBrowser);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "mainForm";
            this.Text = "柔性参数申报系统";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button fill;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button excelButton;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.Button testread;
        private System.Windows.Forms.TextBox marketFileTextBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.WebBrowser mainWebBrowser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox ggTextBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
    }
}

