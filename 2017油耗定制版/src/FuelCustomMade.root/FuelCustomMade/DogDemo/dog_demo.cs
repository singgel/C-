////////////////////////////////////////////////////////////////////
// Copyright (C) 2012 SafeNet, Inc. All rights reserved.
//
// SuperDog(R) is a trademark of SafeNet, Inc.
//
//
////////////////////////////////////////////////////////////////////
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;
using SuperDog;


namespace DogDemo
{
    /// <summary>
    /// SuperDog Sample Form
    /// </summary>
    public class FormDogDemo : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox groupAPIdemo;
        private System.Windows.Forms.GroupBox groupRUS;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.GroupBox groupHistory;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Button buttonC2V;
        private System.Windows.Forms.Button buttonV2C;
        private System.Windows.Forms.TextBox textHistory;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Cursor current = null;

        /// <summary>
        /// The form's constructor.
        /// </summary>
        public FormDogDemo()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }

            base.Dispose( disposing );
        }


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDogDemo));
            this.groupAPIdemo = new System.Windows.Forms.GroupBox();
            this.buttonRun = new System.Windows.Forms.Button();
            this.groupRUS = new System.Windows.Forms.GroupBox();
            this.buttonV2C = new System.Windows.Forms.Button();
            this.buttonC2V = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textHistory = new System.Windows.Forms.TextBox();
            this.groupHistory = new System.Windows.Forms.GroupBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.groupAPIdemo.SuspendLayout();
            this.groupRUS.SuspendLayout();
            this.groupHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupAPIdemo
            // 
            this.groupAPIdemo.Controls.Add(this.buttonRun);
            this.groupAPIdemo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupAPIdemo.Location = new System.Drawing.Point(8, 8);
            this.groupAPIdemo.Name = "groupAPIdemo";
            this.groupAPIdemo.Size = new System.Drawing.Size(264, 117);
            this.groupAPIdemo.TabIndex = 1;
            this.groupAPIdemo.TabStop = false;
            this.groupAPIdemo.Text = "API Demo";
            // 
            // buttonRun
            // 
            this.buttonRun.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonRun.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonRun.Location = new System.Drawing.Point(28, 50);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(208, 24);
            this.buttonRun.TabIndex = 1;
            this.buttonRun.Text = "&Run Demo";
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // groupRUS
            // 
            this.groupRUS.Controls.Add(this.buttonV2C);
            this.groupRUS.Controls.Add(this.buttonC2V);
            this.groupRUS.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupRUS.Location = new System.Drawing.Point(8, 145);
            this.groupRUS.Name = "groupRUS";
            this.groupRUS.Size = new System.Drawing.Size(264, 151);
            this.groupRUS.TabIndex = 2;
            this.groupRUS.TabStop = false;
            this.groupRUS.Text = "Remote Update System";
            // 
            // buttonV2C
            // 
            this.buttonV2C.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonV2C.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonV2C.Location = new System.Drawing.Point(28, 92);
            this.buttonV2C.Name = "buttonV2C";
            this.buttonV2C.Size = new System.Drawing.Size(208, 24);
            this.buttonV2C.TabIndex = 1;
            this.buttonV2C.Text = "&Update  (V2C) ...";
            this.buttonV2C.Click += new System.EventHandler(this.buttonV2C_Click);
            // 
            // buttonC2V
            // 
            this.buttonC2V.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonC2V.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonC2V.Location = new System.Drawing.Point(28, 41);
            this.buttonC2V.Name = "buttonC2V";
            this.buttonC2V.Size = new System.Drawing.Size(208, 24);
            this.buttonC2V.TabIndex = 0;
            this.buttonC2V.Text = "&Generate Status Information (C2V) ...";
            this.buttonC2V.Click += new System.EventHandler(this.buttonC2V_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonClose.Location = new System.Drawing.Point(40, 312);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(200, 24);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textHistory
            // 
            this.textHistory.BackColor = System.Drawing.SystemColors.Info;
            this.textHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.textHistory.ForeColor = System.Drawing.SystemColors.InfoText;
            this.textHistory.Location = new System.Drawing.Point(304, 32);
            this.textHistory.Multiline = true;
            this.textHistory.Name = "textHistory";
            this.textHistory.ReadOnly = true;
            this.textHistory.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textHistory.Size = new System.Drawing.Size(384, 264);
            this.textHistory.TabIndex = 3;
            this.textHistory.WordWrap = false;
            // 
            // groupHistory
            // 
            this.groupHistory.Controls.Add(this.buttonClear);
            this.groupHistory.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupHistory.Location = new System.Drawing.Point(288, 8);
            this.groupHistory.Name = "groupHistory";
            this.groupHistory.Size = new System.Drawing.Size(416, 344);
            this.groupHistory.TabIndex = 3;
            this.groupHistory.TabStop = false;
            this.groupHistory.Text = "Demo History";
            // 
            // buttonClear
            // 
            this.buttonClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonClear.Location = new System.Drawing.Point(120, 304);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(200, 24);
            this.buttonClear.TabIndex = 0;
            this.buttonClear.Text = "Clear &History";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // FormDogDemo
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(714, 367);
            this.Controls.Add(this.textHistory);
            this.Controls.Add(this.groupHistory);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupRUS);
            this.Controls.Add(this.groupAPIdemo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormDogDemo";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SuperDog Demo for C#";
            this.groupAPIdemo.ResumeLayout(false);
            this.groupRUS.ResumeLayout(false);
            this.groupHistory.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

				}
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        //static void Main(string[] args) 
        //{
        //    if (args.Length > 0 && args[0] == "/q")
        //    {
        //        // run SuperDog API demo
        //        DogDemo demo = new DogDemo(null);
        //        demo.RunDemo(DogDemo.defaultScope);
        //        return;
        //    }


        //    Application.EnableVisualStyles();
        //    Application.Run(new FormDogDemo());
        //}

        /// <summary>
        /// 
        /// </summary>
        private bool LockDialog
        {
            set
            {
                if ((null == current) && !value)
                    return;

                if (value)
                {
                    current = this.Cursor;
                    this.Cursor =  System.Windows.Forms.Cursors.WaitCursor;
                }
                else
                {
                    this.Cursor = current;
                    current = null;
                }

                this.groupAPIdemo.Enabled = !value;
                this.groupRUS.Enabled = !value;
                this.buttonClear.Enabled = !value;

                this.Refresh();
            }
        }


        ////////////////////////////////////////////////////////////
        /// Event handlers
        ////////////////////////////////////////////////////////////
        

        /// <summary>
        /// Clears the "History" text box.
        /// </summary>
        private void buttonClear_Click(object sender, System.EventArgs e)
        {
            this.textHistory.Clear();
        }


        /// <summary>
        /// Closes the dialog
        /// </summary>
        private void buttonClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// "Generate Status Information..." button handler
        /// </summary>
        private void buttonC2V_Click(object sender, System.EventArgs e)
        {
            // create an "Save As" dialog instance.
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save Generated Status Information As";
            saveDialog.Filter = "Customer To Vendor Files (*.c2v)|*.c2v|All Files (*.*)|*.*";
            saveDialog.FilterIndex = 1;
            saveDialog.DefaultExt = "c2v";
            saveDialog.FileName = "Dog_demo.c2v";

            if (DialogResult.OK != saveDialog.ShowDialog())
                return;

            LockDialog = true;

            // get the update information from SuperDog
            DogDemoC2v c2v = new DogDemoC2v(this.textHistory);
            string info = c2v.RunDemo();

            LockDialog = false;

            if (0 == info.Length)
                return;

            try
            {
                // save the info in a file
                Stream stream = saveDialog.OpenFile();
                if (null == stream)
                {
                    MessageBox.Show(this, 
                        "Failed to save file \"" + saveDialog.FileName + "\".", 
                        "Save Generated Status Information", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);

                    return;
                }

                StreamWriter writer = new StreamWriter(stream);
                writer.Write(info);
                writer.Close();
                stream.Close();

                WriteToTextbox(textHistory, "Status Information written to: \"" +
                    saveDialog.FileName + 
                    "\"\r\n");
            }
            catch (Exception)
            {
                MessageBox.Show(this,
                    "Failed to save file.", 
                    "Save Generated Status Information", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// "Update SuperDog info..." button handler
        /// </summary>
        private void buttonV2C_Click(object sender, System.EventArgs e)
        {
            // create an instance of the "File Open" dialog
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Open SuperDog Update File";
            openDialog.Filter = "Vendor To Customer Files (*.v2c)|*.v2c|All Files (*.*)|*.*";
            openDialog.FilterIndex = 1;
            openDialog.DefaultExt = "v2c";

            if (DialogResult.OK != openDialog.ShowDialog())
                return;

            string info = null;

            try
            {
                // open the file and read its contents
                Stream stream = openDialog.OpenFile();
                if (null == stream)
                {
                    MessageBox.Show(this, 
                        "Failed to open file \"" + openDialog.FileName + "\".",
                        "SuperDog Update", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);

                    return;
                }

                StreamReader reader = new StreamReader(stream);

                Int64 size = stream.Length;
                char[] chars = new char[(int)size];

                reader.Read(chars, 0, chars.Length);
                info = new string(chars);

                reader.Close();
                stream.Close();
            }
            catch (Exception)
            {
                MessageBox.Show(this,
                    "Failed to open file.",
                    "SuperDog Update", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);

                return;
            }
    
            LockDialog = true;

            // perform a dog update
            DogDemoV2c v2c = new DogDemoV2c(this.textHistory);
            v2c.RunDemo(info);

            LockDialog = false;
        }

        
        /// <summary>
        /// "Run Demo" button handler
        /// </summary>
        private void buttonRun_Click(object sender, System.EventArgs e)
        {
            string scope = DogDemo.defaultScope;

            LockDialog = true;

            // run SuperDog API demo
            DogDemo demo = new DogDemo(this.textHistory);
            demo.RunDemo(scope);

            LockDialog = false;
        }
        public static void WriteToTextbox(TextBox textHistory, string text)
        {
            if (textHistory == null)
            {
                Console.Write(text);
                return;
            }
            textHistory.Text += text;
            textHistory.SelectionStart = textHistory.TextLength;
            textHistory.ScrollToCaret();
            textHistory.Refresh();
        }
    }
}
