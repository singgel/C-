using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelDataSysClient.Tool.Tool_BMW
{
    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e); //定义一个委托事件，用于处理进度条
    public delegate void ProgressCountEventHandel(object sender,ProgressCountArgs e);
    public class ProgressEventArgs:EventArgs
    {
        private int _value; //进度条当前值（当前完成图标数量）
        private int _max;   //进度条最大值（你要生成图标的总数）
        

        public ProgressEventArgs(int value, int max)
        {
            _value = value;
            _max = max;
           
        }

        #region 属性
        public int Value
        {
            get { return this._value; }
        }

        public int Max
        {
            get { return this._max; }
        }

     

        #endregion

    }

    public class ProgressCountArgs:EventArgs
    {
        private int _count;

        public int Count
        {
          get { return _count; }
          
        }
        private int _modern;

        public int Modern
        {
          get { return _modern; }
          
        }
        public ProgressCountArgs(int count,int modern)
        {
            _count = count;
            _modern = modern;
        }
        
    }
}
