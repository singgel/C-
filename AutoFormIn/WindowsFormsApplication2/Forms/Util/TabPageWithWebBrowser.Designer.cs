using System.Windows.Forms;
namespace Assistant.Forms
{
    partial class TabPageWithWebBrowser
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.webBrowser1 = new WebBrowserUtils.ExtendWebBrowser.WebBrowser2();
            webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(this.webBrowser1);
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private WebBrowserUtils.ExtendWebBrowser.WebBrowser2 webBrowser1;

        public WebBrowserUtils.ExtendWebBrowser.WebBrowser2 WebBrowser1
        {
            get { return webBrowser1; }
        }
    }
}
