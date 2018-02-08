using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting.Messaging;
namespace HFSoft.Component.Windows
{
     partial class LoadingForm : Form {
        public LoadingForm(LoadingStyle style ) {
            InitializeComponent();
            Style = style;
        }
        public LoadingStyle Style {
            get;
            set;
        }
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Owner.Enabled = false;
            Owner.Opacity = 0.6;
            if (Style != LoadingStyle.None) {
                picState.Visible = false;
                progressState.Visible = true;
                progressState.Dock = DockStyle.Fill;
            } else {
                picState.Visible = true;
                progressState.Visible = false;
                picState.Dock = DockStyle.Fill;
            }
            Action<Form> onExecute = e_ => {
                LoadingArgs args = new LoadingArgs();
                args.Owner = Owner;
                args.OnChange = state => {
                    Invoke(new Action<LoadingArgs>(SetState), args);
                };
                try {
                   
                   OnExecute(args);
                    args.State = LoadingState.Completion;
                    args.Change();
                } catch (Exception err) {
                    args.State = LoadingState.Error;
                    args.Error = err.Message;
                    args.Change();
                }
               
                
            };
           
            ExecuteInvoke(onExecute);
        }
        private void ExecuteInvoke(Action<Form> execute)
        {
            execute.BeginInvoke(this, ac =>
            {
                Action<Form> ehandler = (Action<Form>)((AsyncResult)ac).AsyncDelegate;
                ehandler.EndInvoke(ac);
            }, null);
        }
        private void SetState(LoadingArgs e) {
          
            switch (e.State) {
                case LoadingState.Initializtion:
                case LoadingState.Handling:
                    progressState.Maximum = e.MaxValue;
                    progressState.Minimum = e.MinValue;
                    progressState.Value = e.Value;
                    break;
                case LoadingState.Completion:
                    Owner.Opacity = 1;
                    Owner.Enabled = true;
                    Close();
                    break;
                case LoadingState.Error:
                    if (e.Error != null)
                        MessageBox.Show(this, e.Error, "处理错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Owner.Opacity = 1;
                    Owner.Enabled = true;
                    Close();
                    break;
            }
        }
        protected virtual void OnExecute(LoadingArgs args)
        {
            Execute(args);
        }
        public Action<LoadingArgs> Execute {
            get;
            set;
        }
    }
}
