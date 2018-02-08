namespace FuelDataSysClient
{
    partial class UserInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.txtValidatePwd = new System.Windows.Forms.TextBox();
            this.txtOperatorPwd = new System.Windows.Forms.TextBox();
            this.txtOperatorName = new System.Windows.Forms.TextBox();
            this.gbOperatorInfo = new System.Windows.Forms.GroupBox();
            this.lblValidatePwd = new System.Windows.Forms.Label();
            this.lblOperatorPwd = new System.Windows.Forms.Label();
            this.lblOperatorName = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip();
            this.chkOperatorState = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gbOperatorInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(269, 141);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 14);
            this.label8.TabIndex = 17;
            this.label8.Text = "*";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(269, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 14);
            this.label7.TabIndex = 16;
            this.label7.Text = "*";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(269, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 14);
            this.label6.TabIndex = 15;
            this.label6.Text = "*";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(269, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 14);
            this.label5.TabIndex = 14;
            this.label5.Text = "*";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(96, 136);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(169, 22);
            this.txtUserName.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 14);
            this.label2.TabIndex = 8;
            this.label2.Text = "姓      名:";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(187, 247);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(60, 23);
            this.btnSubmit.TabIndex = 10;
            this.btnSubmit.Text = "保存";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // txtValidatePwd
            // 
            this.txtValidatePwd.Location = new System.Drawing.Point(96, 101);
            this.txtValidatePwd.Name = "txtValidatePwd";
            this.txtValidatePwd.PasswordChar = '*';
            this.txtValidatePwd.Size = new System.Drawing.Size(169, 22);
            this.txtValidatePwd.TabIndex = 5;
            // 
            // txtOperatorPwd
            // 
            this.txtOperatorPwd.Location = new System.Drawing.Point(96, 64);
            this.txtOperatorPwd.Name = "txtOperatorPwd";
            this.txtOperatorPwd.PasswordChar = '*';
            this.txtOperatorPwd.Size = new System.Drawing.Size(169, 22);
            this.txtOperatorPwd.TabIndex = 3;
            // 
            // txtOperatorName
            // 
            this.txtOperatorName.Location = new System.Drawing.Point(96, 27);
            this.txtOperatorName.Name = "txtOperatorName";
            this.txtOperatorName.Size = new System.Drawing.Size(169, 22);
            this.txtOperatorName.TabIndex = 1;
            // 
            // gbOperatorInfo
            // 
            this.gbOperatorInfo.Controls.Add(this.txtPhone);
            this.gbOperatorInfo.Controls.Add(this.label4);
            this.gbOperatorInfo.Controls.Add(this.label8);
            this.gbOperatorInfo.Controls.Add(this.label7);
            this.gbOperatorInfo.Controls.Add(this.label6);
            this.gbOperatorInfo.Controls.Add(this.label5);
            this.gbOperatorInfo.Controls.Add(this.txtUserName);
            this.gbOperatorInfo.Controls.Add(this.label2);
            this.gbOperatorInfo.Controls.Add(this.txtValidatePwd);
            this.gbOperatorInfo.Controls.Add(this.txtOperatorPwd);
            this.gbOperatorInfo.Controls.Add(this.lblValidatePwd);
            this.gbOperatorInfo.Controls.Add(this.txtOperatorName);
            this.gbOperatorInfo.Controls.Add(this.lblOperatorPwd);
            this.gbOperatorInfo.Controls.Add(this.lblOperatorName);
            this.gbOperatorInfo.Location = new System.Drawing.Point(26, 13);
            this.gbOperatorInfo.Name = "gbOperatorInfo";
            this.gbOperatorInfo.Size = new System.Drawing.Size(289, 218);
            this.gbOperatorInfo.TabIndex = 8;
            this.gbOperatorInfo.TabStop = false;
            this.gbOperatorInfo.Text = "用户信息";
            // 
            // lblValidatePwd
            // 
            this.lblValidatePwd.AutoSize = true;
            this.lblValidatePwd.Location = new System.Drawing.Point(25, 106);
            this.lblValidatePwd.Name = "lblValidatePwd";
            this.lblValidatePwd.Size = new System.Drawing.Size(59, 14);
            this.lblValidatePwd.TabIndex = 4;
            this.lblValidatePwd.Text = "确认密码:";
            // 
            // lblOperatorPwd
            // 
            this.lblOperatorPwd.AutoSize = true;
            this.lblOperatorPwd.Location = new System.Drawing.Point(25, 69);
            this.lblOperatorPwd.Name = "lblOperatorPwd";
            this.lblOperatorPwd.Size = new System.Drawing.Size(59, 14);
            this.lblOperatorPwd.TabIndex = 2;
            this.lblOperatorPwd.Text = "用户密码:";
            // 
            // lblOperatorName
            // 
            this.lblOperatorName.AutoSize = true;
            this.lblOperatorName.Location = new System.Drawing.Point(25, 32);
            this.lblOperatorName.Name = "lblOperatorName";
            this.lblOperatorName.Size = new System.Drawing.Size(59, 14);
            this.lblOperatorName.TabIndex = 0;
            this.lblOperatorName.Text = "登录名称:";
            // 
            // toolTip
            // 
            this.toolTip.ShowAlways = true;
            // 
            // chkOperatorState
            // 
            this.chkOperatorState.AutoSize = true;
            this.chkOperatorState.Location = new System.Drawing.Point(28, 249);
            this.chkOperatorState.Name = "chkOperatorState";
            this.chkOperatorState.Size = new System.Drawing.Size(128, 18);
            this.chkOperatorState.TabIndex = 9;
            this.chkOperatorState.Text = "同时激活此用户(&A)";
            this.chkOperatorState.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(253, 247);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(60, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(95, 175);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(169, 22);
            this.txtPhone.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 180);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 14);
            this.label4.TabIndex = 18;
            this.label4.Text = "电      话:";
            // 
            // UserInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(341, 286);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.gbOperatorInfo);
            this.Controls.Add(this.chkOperatorState);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserInfoForm";
            this.ShowIcon = false;
           // this.Text = "用户信息";
            this.gbOperatorInfo.ResumeLayout(false);
            this.gbOperatorInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.TextBox txtValidatePwd;
        private System.Windows.Forms.TextBox txtOperatorPwd;
        private System.Windows.Forms.TextBox txtOperatorName;
        private System.Windows.Forms.GroupBox gbOperatorInfo;
        private System.Windows.Forms.Label lblValidatePwd;
        private System.Windows.Forms.Label lblOperatorPwd;
        private System.Windows.Forms.Label lblOperatorName;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox chkOperatorState;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label label4;
    }
}