using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;
using WebBrowserUtils.HtmlUtils;

namespace WebBrowserUtils.HtmlUtils.Actions
{
    public class HtmlElementActions:BaseImpl
    {
        /// <summary>
        /// WebBrowser webBrowser 必录参数
        /// </summary>
        public HtmlElementActions(WebBrowser webBrowser) : base(webBrowser) { }

        /// <summary>
        /// InvokeMemeber方法
        /// </summary>
        public String doAction(String elementId, String action)
        {
            try
            {
                this.htmlDocument.All[elementId].InvokeMember(action);
            }
            catch
            {
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String raiseEvent(String elementId, String action)
        {
            try
            {
                this.htmlDocument.GetElementById(elementId).RaiseEvent(action);
            }
            catch
            {
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        //同时根据id和name设置元素动作
        public String doActionByIdAndName(String elementId, String elementName, String action) {
            try
            {
                foreach (HtmlElement he in this.htmlDocument.All)
                {
                    String eId = he.Id;
                    String eName = he.Name;
                    if (eId == elementId && eName == elementName)
                    {
                        he.InvokeMember(action);
                    }
                }
            }
            catch (Exception)
            {
                
                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        //同时根据id,name和value设置元素动作
        public String doActionByIdAndName(String elementId, String elementName, String value, String action)
        {
            try
            {
                foreach (HtmlElement he in this.htmlDocument.All)
                {
                    String eId = he.Id;
                    String eName = he.Name;
                    String eValue = he.GetAttribute("value");
                    if (eId == elementId && eName == elementName && eValue == value)
                    {
                        he.InvokeMember(action);
                        return HtmlConstants.ActionReturnValues.SUCCESS;
                    }
                }
            }
            catch (Exception)
            {
                
                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String doActionByIdAndName(String elementId, String elementName, String value, String frame, String action)
        {
            try
            {
                HtmlWindow hw = this.webBrowser.Document.Window.Frames[frame];
                foreach (HtmlElement he in hw.Document.All)
                {
                    String eId = he.Id;
                    String eName = he.Name;
                    String eValue = he.GetAttribute("value");
                    //逼不得已，不得不使用trim方法
                    if (eId == elementId && eName == elementName && eValue.Trim() == value.Trim())
                    {
                        he.InvokeMember(action);
                    }
                }
            }
            catch (Exception)
            {
                
                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String doActionByTitle(String title, String frame, String frameInside, String action)
        {
            try
            {
                HtmlWindow hw = this.webBrowser.Document.Window.Frames[frame];
                hw = hw.Document.Window.Frames[frameInside];
                foreach (HtmlElement he in hw.Document.All)
                {
                    String areaTitle = he.GetAttribute("title");

                    //逼不得已，不得不使用trim方法
                    if (areaTitle == title)
                    {
                        he.InvokeMember(action);
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        /// <summary>
        /// InvokeMemeber方法
        /// action表示执行的函数或者方法,value表示方法传递的参数值
        /// </summary>
        public String doAction(String elementId, String action, String value)
        {
            try
            {
                this.htmlDocument.All[elementId].InvokeMember(action, value);
            }
            catch
            {
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        /// <summary>
        /// 点击事件（click）
        /// </summary>
        public String click(String elementId)
        {
            return this.doAction(elementId, HtmlConstants.ActionType.CLICK);
        }
        /// <summary>
        /// 根据id和name进行进行查找然后执行动作
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public String click(String elementId, String elementName) {
            return this.doActionByIdAndName(elementId, elementName, HtmlConstants.ActionType.CLICK);
        }
        /// <summary>
        /// 根据id,name,value三个值来查找按钮并点击
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="elementName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public String click(String elementId, String elementName, String value) {
            return this.doActionByIdAndName(elementId, elementName, value, HtmlConstants.ActionType.CLICK);
        }

        public String click(String elementId, String elementName, String value, String frame)
        {
            return this.doActionByIdAndName(elementId, elementName, value, frame, HtmlConstants.ActionType.CLICK);
        }

        public String clickArea(String title, String frame, String frameInside) {
            return this.doActionByTitle(title, frame, frameInside, HtmlConstants.ActionType.CLICK);
        }

        /// <summary>
        /// 执行页面中的动态脚本
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public String invokeScript(String method) {
            try
            {
                this.htmlDocument.InvokeScript(method);
            }
            catch (Exception)
            {
                
                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String invokeScript(String method, String frameName)
        {
            try
            {
                HtmlWindow hw = this.webBrowser.Document.Window.Frames[frameName];
                hw.Document.InvokeScript(method);
            }
            catch (Exception)
            {
                
                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String navigateScript(String script)
        {
            try
            {
                this.webBrowser.Navigate("javascript:" + script);
            }
            catch (Exception)
            {
                
                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String navigateScript(String script, String frame)
        {
            try
            {
                this.webBrowser.Navigate("javascript:" + script, frame);
            }
            catch (Exception)
            {
                
                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String execScript(String script)
        {
            try
            {
                this.htmlDocument2.parentWindow.execScript(script, "javascript");
            }
            catch (Exception)
            {
                
                throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String execScript(String script, String frame)
        {
            try
            {
                HtmlWindow hw = this.webBrowser.Document.Window.Frames[frame];
                IHTMLDocument2 frameDoc = (IHTMLDocument2)hw.Document.DomDocument;
                frameDoc.parentWindow.execScript(script, "javascript");
            }
            catch (Exception)
            {
                
                throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }
    }
}
