using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
namespace HFSoft.Component.Windows
{
    public enum LoadingStyle {
        None,
        Progress
    }
    public enum LoadingState {
        Initializtion,
        Handling,
        Completion,
        Error

    }
    public class LoadingArgs:EventArgs
    {
        public LoadingArgs() {
            MaxValue = 100;
        }
        public int MaxValue {
            get;
            set;
        }
        public int MinValue {
            get;
            set;
        }
        public int Value {
            get;
            set;
        }
        private LoadingState mState = LoadingState.Initializtion;
        public LoadingState State {
            get {
                return mState;
            }
            set {
                mState = value;
            }
        }
        
        public string Error {
            get;
            set;
        }
        public void Change() {
            if (OnChange != null)
                OnChange(this);
        }
        internal Action<LoadingArgs> OnChange {
            get;
            set;
        }
        internal System.Windows.Forms.Form Owner
        {
            get;
            set;
        }
        public void Execute(Action<object> handler)
        {
            Owner.Invoke(new Action<object>(handler), Owner);
        }
    }
    public class LoadingHandler {
        public LoadingStyle Style {
            get;
            set;
        }
        public void Load(Form owner, Action<LoadingArgs> action) {
            LoadingForm frm = new LoadingForm(Style);
           
            frm.Execute = action;
            frm.ShowDialog(owner);
            

        }
        public static void Show(Form owner, Action<LoadingArgs> action)
        {
            Show(owner, LoadingStyle.None, action);
        }
        public static void Show(Form owner,LoadingStyle style ,Action<LoadingArgs> action)
        {
            LoadingHandler lh = new LoadingHandler();
            lh.Style = style;
            lh.Load(owner, action);
        }
    }
}
