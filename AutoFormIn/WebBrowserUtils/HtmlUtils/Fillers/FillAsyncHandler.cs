using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 在执行异步填写时同步填写动作。
    /// </summary>
    internal class FillAsyncHandler : IDisposable
    {
        private ManualResetEvent _waitHandle;

        public FillAsyncHandler()
        {
            _waitHandle = new ManualResetEvent(true);
        }

        ~FillAsyncHandler()
        {
            Dispose(false);
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_waitHandle == null)
                {
                    _waitHandle = new ManualResetEvent(false);
                }
                return _waitHandle;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _waitHandle != null)
                _waitHandle.Close();
        }
        /// <summary>
        /// 阻塞调用Wait方法的线程。
        /// </summary>
        public bool Reset()
        {
            return _waitHandle.Reset();
        }
        /// <summary>
        /// 恢复一个阻塞线程。
        /// </summary>
        public void Resume()
        {
            _waitHandle.Set();
        }
        /// <summary>
        /// 挂起当前线程。
        /// </summary>
        public void Suspend()
        {
            _waitHandle.Reset();
            _waitHandle.WaitOne();
        }
        /// <summary>
        /// 阻塞当前线程，直到其它线程完成，若当前无争用线程，此方法将立即返回。
        /// </summary>
        public void Wait()
        {
            _waitHandle.WaitOne();
        }
    }
}
