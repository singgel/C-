namespace HFSoft.Component.Windows
{
    partial class LoadingForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingForm));
            this.progressState = new System.Windows.Forms.ProgressBar();
            this.picState = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picState)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressState
            // 
            this.progressState.BackColor = System.Drawing.Color.White;
            this.progressState.ForeColor = System.Drawing.Color.Pink;
            this.progressState.Location = new System.Drawing.Point(-182, -1);
            this.progressState.Name = "progressState";
            this.progressState.Size = new System.Drawing.Size(381, 10);
            this.progressState.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressState.TabIndex = 3;
            // 
            // picState
            // 
            this.picState.Image = ((System.Drawing.Image)(resources.GetObject("picState.Image")));
            this.picState.Location = new System.Drawing.Point(0, 0);
            this.picState.Name = "picState";
            this.picState.Size = new System.Drawing.Size(214, 15);
            this.picState.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picState.TabIndex = 2;
            this.picState.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.progressState);
            this.panel1.Controls.Add(this.picState);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(214, 50);
            this.panel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "系统处理中，请耐心等待...";
            // 
            // LoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 50);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(214, 50);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(214, 40);
            this.Name = "LoadingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.picState)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressState;
        private System.Windows.Forms.PictureBox picState;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;

    }
}