using mshtml;
using SHDocVw;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using WebBrowserUtils.ExtendWebBrowser.NativeMethods;

namespace WebBrowserUtils.ExtendWebBrowser
{    

    public class WebBrowser2 : System.Windows.Forms.WebBrowser
    {
        private int dwCookie;
        private IWebBrowser2 axIWebBrowser2;
        private WebBrowserEvent2 webBrowserEvent2;
        private AxHost.ConnectionPointCookie cookie;

        [ComImport, TypeLibType((short)0x1010), InterfaceType((short)2), Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
        public interface IWebBrowserEvent2
        {
            [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x111)]
            void NewWindow3([In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object ppDisp, [In, Out] ref bool Cancel, [In] uint dwFlags, [In, MarshalAs(UnmanagedType.BStr)] string bstrUrlContext, [In, MarshalAs(UnmanagedType.BStr)] string bstrUrl);
            [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),DispId(0x107)]
            void WindowClosing([In] bool IsChildWindow, [In] [Out] ref bool Cancel);
        }

        public class WebBrowserEvent2 : IWebBrowserEvent2
        {
            private WebBrowser2 parent;

            public WebBrowserEvent2(WebBrowser2 browser)
            {
                parent = browser;
            }

            public void NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
            {
                WebBrowserNavigatingEventArgs e = new WebBrowserNavigatingEventArgs(new Uri(bstrUrl), null);
                this.parent.OnNewWindow3(e);
                Cancel = e.Cancel;
            }
            
            public void WindowClosing(bool IsChildWindow, ref bool Cancel)
            {
                WebBrowserWindowClosingEventArgs e = new WebBrowserWindowClosingEventArgs(IsChildWindow, Cancel);
                this.parent.OnWindowClosing(e);
                Cancel = e.Cancel;
            }
        }

        public WebBrowser2()
        {
            //this.ScriptErrorsSuppressed = true;
        }

        public event WebBrowserNavigatingEventHandler NewWindow3;
        public event WebBrowserWindowClosingEventHandler WindowClosing;
        /// <summary>
        /// 在新窗口中打开链接时引发此事件。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNewWindow3(WebBrowserNavigatingEventArgs e)
        {
            if (NewWindow3 != null)
                NewWindow3(this, e);
        }
        /// <summary>
        /// .Net Bug，此事件不会被引发。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnWindowClosing(WebBrowserWindowClosingEventArgs e)
        {
            if (WindowClosing != null)
                WindowClosing(this, e);
        }
        /// <summary>
        /// 查询指定HTML文档中是否可执行jQuery。
        /// </summary>
        /// <param name="document">要查询的HTML文档。</param>
        /// <returns></returns>
        public static bool HasJQuery(HtmlDocument document)
        {
            return HasJQuery(document.DomDocument as IHTMLDocument);
        }
        /// <summary>
        /// 查询指定HTML文档中是否可执行jQuery。
        /// </summary>
        /// <param name="document">要查询的HTML文档。</param>
        /// <returns></returns>
        public static bool HasJQuery(IHTMLDocument document)
        {
            object result = InvokeScript(document, "hasJQuery");
            if (result == null)
            {
                AttachHasJQueryFunction(document);
                result = InvokeScript(document, "hasJQuery");
            }
            return (result != null && (bool)result);
        }

        public static void InstallJQuery(IHTMLDocument document)
        {
            InstallJQuery(document, false);
        }
        /// <summary>
        /// 向指定HTML文档中安装jQuery。
        /// </summary>
        /// <param name="document">要安装jQuery的HTML文档顶级节点。</param>
        public static void InstallJQuery(IHTMLDocument document, bool force)
        {
            IHTMLElement head = GetDocHead(document);
            if (force || HasJQuery(document) == false)
            {
                IHTMLElement script = ((IHTMLDocument2)document).createElement("script");
                script.setAttribute("type", "text/javascript");
                script.setAttribute("text", jQueryHelper.jQuery_1_11_1);
                ((IHTMLElement2)head).insertAdjacentElement("beforeEnd", script);
            }
        }
        /// <summary>
        /// 向指定HTML文档中附加js脚本。
        /// </summary>
        /// <param name="doc">要安装js脚本的HTML文档顶级节点。</param>
        /// <param name="code">js脚本代码。</param>
        public static void AttachScript(IHTMLDocument doc, string code, HtmlElementInsertionOrientation orient)
        {
            string position = "beforeEnd";
            switch (orient)
            {
                case HtmlElementInsertionOrientation.AfterBegin:
                    position = "afterBegin";
                    break;
                case HtmlElementInsertionOrientation.AfterEnd:
                    position = "afterEnd";
                    break;
                case HtmlElementInsertionOrientation.BeforeBegin:
                    position = "beforeBegin";
                    break;
            }
            IHTMLElement head = GetDocHead(doc);
            IHTMLElement script = ((IHTMLDocument2)doc).createElement("script");
            script.setAttribute("type", "text/javascript");
            script.setAttribute("text", code);
            ((IHTMLElement2)head).insertAdjacentElement(position, script);
        }
        /// <summary>
        /// 向指定HTML文档中附加js脚本。
        /// </summary>
        /// <param name="doc">要安装js脚本的HTML文档顶级节点。</param>
        /// <param name="code">js脚本代码。</param>
        public static void AttachScript(IHTMLDocument doc, string code)
        {
            AttachScript(doc, code, HtmlElementInsertionOrientation.BeforeEnd);
        }

        public static void AttachElement(IHTMLDocument doc, HtmlElement element)
        {
            IHTMLElement head = GetDocHead(doc);
            ((IHTMLElement2)head).insertAdjacentElement("beforeEnd", element.DomElement as IHTMLElement);
        }

        private static void AttachHasJQueryFunction(IHTMLDocument document)
        {
            IHTMLElement head = GetDocHead(document);
            IHTMLElement script = ((IHTMLDocument2)document).createElement("script");
            script.setAttribute("type", "text/javascript");
            script.setAttribute("text", @"function hasJQuery(){return window.jQuery != null && window.jQuery != undefined;}");
            ((IHTMLElement2)head).insertAdjacentElement("beforeEnd", script);
        }

        private static IHTMLElement GetDocHead(IHTMLDocument doc)
        {
            IHTMLElementCollection collection = ((IHTMLDocument3)doc).getElementsByTagName("html");
            IHTMLElement head = null;
            if (collection != null && collection.length > 0)
                head = collection.item(0) as IHTMLElement;
            else
                head = ((IHTMLDocument2)doc).body;
            return head;
        }

        public static object InvokeScript(IHTMLDocument doc, string scriptName)
        {
            return InvokeScript(doc, scriptName, null);
        }

        public static object InvokeScript(IHTMLDocument doc, string scriptName, object[] args)
        {
            object result = null;
            NativeMethods.tagDISPPARAMS tagDISPPARAMS = new NativeMethods.tagDISPPARAMS();
            tagDISPPARAMS.rgvarg = IntPtr.Zero;
            try
            {
                IDispatch dispatch = doc.Script as IDispatch;
                if (dispatch != null)
                {
                    Guid empty = Guid.Empty;
                    string[] rgszNames = new string[]
					{
						scriptName
					};
                    int[] array = new int[]
					{
						-1
					};
                    int iDsOfNames = dispatch.GetIDsOfNames(ref empty, rgszNames, 1, SafeNativeMethods.GetThreadLCID(), array);
                    if (SafeNativeMethods.Succeeded(iDsOfNames) && array[0] != -1)
                    {
                        if (args != null)
                        {
                            Array.Reverse(args);
                        }
                        tagDISPPARAMS.rgvarg = ((args == null) ? IntPtr.Zero : SafeNativeMethods.ArrayToVARIANTVector(args));
                        tagDISPPARAMS.cArgs = ((args == null) ? 0 : args.Length);
                        tagDISPPARAMS.rgdispidNamedArgs = IntPtr.Zero;
                        tagDISPPARAMS.cNamedArgs = 0;
                        object[] array2 = new object[1];
                        if (dispatch.Invoke(array[0], ref empty, SafeNativeMethods.GetThreadLCID(), 1, tagDISPPARAMS, array2, new NativeMethods.tagEXCEPINFO(), null) == 0)
                        {
                            result = array2[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
            finally
            {
                if (tagDISPPARAMS.rgvarg != IntPtr.Zero)
                {
                    SafeNativeMethods.FreeVARIANTVector(tagDISPPARAMS.rgvarg, args.Length);
                }
            }
            return result;
        }
        // 该方法由系统自动调用，从该函数的调用中截获到IWebBrowser2接口 
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void AttachInterfaces(object nativeActiveXObject)
        {
            this.axIWebBrowser2 = (IWebBrowser2)nativeActiveXObject;
            base.AttachInterfaces(nativeActiveXObject);
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void DetachInterfaces()
        {
            this.axIWebBrowser2 = null;
            base.DetachInterfaces();
        }

        // 返回WebBrowser的自动化对象    
        public object ApplicationObject
        {
            get { return axIWebBrowser2.Application; }
        }

        // 此方法增加自定义的事件对象   
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void CreateSink()
        {
            //  一定在此调用基类的方法，否则WebBrowser自己实现的事件就不能触发了 
            base.CreateSink();
            webBrowserEvent2 = new WebBrowserEvent2(this);
            cookie = new AxHost.ConnectionPointCookie(this.ActiveXInstance, webBrowserEvent2, typeof(IWebBrowserEvent2));
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void DetachSink()
        {
            if (null != cookie)
            {
                cookie.Disconnect();
                cookie = null;
            }
            base.DetachSink();
        } 
    }
}
