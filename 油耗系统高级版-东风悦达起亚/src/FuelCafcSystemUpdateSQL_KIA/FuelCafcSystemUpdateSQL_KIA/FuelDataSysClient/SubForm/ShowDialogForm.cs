using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace FuelDataSysClient.SubForm
{
    public partial class ShowDialogForm : DevExpress.XtraEditors.XtraForm
    {
        #region Fields & Properties
        /// <summary>
        /// 标题
        /// </summary>
        private string caption;

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }
        /// <summary>
        /// 消息
        /// </summary>
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        /// <summary>
        /// 描述
        /// </summary>
        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        /// <summary>
        /// 进度条最小值
        /// </summary>
        private int minProcess = 1;

        public int MinProcess
        {
            get { return minProcess; }
            set { minProcess = value; }
        }
        /// <summary>
        /// 进度条最大值
        /// </summary>
        private int maxProcess = 100;

        public int MaxProcess
        {
            get { return maxProcess; }
            set { maxProcess = value; }
        }
        #endregion

        #region Constructed Function
        public ShowDialogForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="_caption">提示</param>
        public ShowDialogForm(string _caption)
            : this(_caption, "", "", 100)
        {
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="_caption"></param>
        /// <param name="_message"></param>
        public ShowDialogForm(string _caption,string _message) 
            : this(_caption, _message, "",100)
        {
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="_caption"></param>
        /// <param name="_message"></param>
        /// <param name="_content"></param>
        public ShowDialogForm(string _caption, string _message,string _content)
            : this(_caption, _message, _content, 100)
        {
        }
        
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="_caption">提示</param>
        /// <param name="_message">消息内容</param>
        /// <param name="_content">详细描述</param>
        /// <param name="_maxProcess">进度条最大值</param>
        public ShowDialogForm(string _caption, string _message,string _content,int _maxProcess)
            : this()
        {
            this.Caption = "";
            this.Message = "";
            this.Content = "";

            this.Caption = _caption == "" ? "提示" : _caption;
            this.Message = _message == "" ? "正在加载，请稍后......" : _message;
            this.Content = _content;
            this.maxProcess = _maxProcess > this.MinProcess ? _maxProcess : MinProcess;
            
            lblCaption.Text = this.Caption;
            lblMessage.Text = this.Message;
            lblContent.Text = this.Content;
            progressShow.Properties.Minimum = MinProcess;
            progressShow.Properties.Maximum = MaxProcess;
            progressShow.Properties.Step = 1;
            progressShow.PerformStep();

            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Show();
            this.Refresh();
        }
        #endregion

        #region Methods
        /// <summary>
        /// 设置提示
        /// </summary>
        /// <param name="newCaption"></param>
        public void SetCaption(string newCaption)
        {
            this.Caption = newCaption;
            lblCaption.Text = this.Caption;
            progressShow.PerformStep();
            this.Refresh();
        }
        /// <summary>
        /// 设置消息
        /// </summary>
        /// <param name="newMessage"></param>
        public void SetMessage(string newMessage)
        {
            this.Message = newMessage;
            lblMessage.Text = this.Message;
            progressShow.PerformStep();
            this.Refresh();
        }
        /// <summary>
        /// 设置描述
        /// </summary>
        /// <param name="newContent"></param>
        public void SetContent(string newContent)
        {
            this.Content = newContent;
            lblContent.Text = this.Content;
            progressShow.PerformStep();
            this.Refresh();
        }
        #endregion

        #region Events
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
        #endregion
    }
}