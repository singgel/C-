using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;

using WebBrowserUtils.HtmlUtils.Setters;
using WebBrowserUtils.HtmlUtils.Actions;


namespace Assistant
{
    class LoginAction
    {
        public static Boolean tryLogin(WebBrowser mainWebBrowser)
        {
            try
            {
                HtmlElementValueSetter setter = new HtmlElementValueSetter(mainWebBrowser);
                HtmlElementActions act = new HtmlElementActions(mainWebBrowser);
                String username = Properties.Settings.Default.USERNAME;
                String password = Properties.Settings.Default.PASSWORD;
                if (!checkUsernameAndPassword()) {
                    throw new Exception("用户名或密码为空");
                }
                setter.setTextValue("UserName", username);
                setter.setTextValue("Password", password);
                act.click("loginButton");
            }
            catch
            {
                return false;
            }
            return true;
        }


        private static Boolean checkUsernameAndPassword() {
            String username = Properties.Settings.Default.USERNAME;
            String password = Properties.Settings.Default.PASSWORD;
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                return false;
            }
            else {
                return true;
            }
        }

        public static Boolean saveUsernameAndPassword(String username, String password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                throw new Exception("用户名或密码为空,无法保存");
            }
            else
            {
                Properties.Settings.Default.USERNAME = username;
                Properties.Settings.Default.PASSWORD = password;
                Properties.Settings.Default.Save();
                return true;
            }
        }

        public static Boolean saveParamToDefaultSetting(String paramname, String value) {
            if (String.IsNullOrEmpty(paramname) || String.IsNullOrEmpty(value))
            {
                throw new Exception("参数名称或参数值为空,请重新保存");
            }
            else
            {
                Properties.Settings.Default.Properties[paramname].DefaultValue = value;
                Properties.Settings.Default.Save();
                return true;
            }
        
        }
    }
}
