using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Assistant.Properties;
using Assistant.Entity;

namespace Assistant.Service
{
    class LoginServiceForHCBM
    {
        public static Boolean checkTimeLine()
        {
            String timeLine = Properties.Settings.Default.INSTNAME;
            if (String.IsNullOrEmpty(timeLine))
            {
                return false;
            }
            else
            {
                DateTime now = DateTime.Now;
                DateTime dateTimeLine = DateTime.Parse(timeLine);
                if (now < dateTimeLine)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static Boolean checkUser(String username, String password)
        {
            if (String.IsNullOrWhiteSpace(username))
            {
                throw new Exception("用户名为空,请重新输入");
            }
            if (String.IsNullOrEmpty(password))
            {
                throw new Exception("密码为空,请重新输入");
            }
            String computerInfo = MachineCodeUtils.MachineCodeUtil.GetComputerInfo();
            if (String.IsNullOrEmpty(computerInfo))
            {
                throw new Exception("获取机器码错误");
            }
            if (String.IsNullOrEmpty(Properties.Settings.Default.INSTNAME))
            {
                throw new Exception("机构信息为空");
            }
            
            bool b = ClassFactory.fuss.checkUserInfo(username, password, computerInfo);
            
            return b;
        }

        public static Boolean checkRegUser(String username, String password, String passwordRepeat)
        {
            if (String.IsNullOrWhiteSpace(username))
            {
                throw new Exception("用户名为空,请重新输入");
            }
            if (String.IsNullOrEmpty(password) || String.IsNullOrEmpty(passwordRepeat))
            {
                throw new Exception("密码为空,请重新输入");
            }
            if (password != passwordRepeat) {
                throw new Exception("两次密码输入不一致");
            }
            String computerInfo = MachineCodeUtils.MachineCodeUtil.GetComputerInfo();
            if (String.IsNullOrEmpty(computerInfo))
            {
                throw new Exception("获取机器码错误");
            }
            if (String.IsNullOrEmpty(Properties.Settings.Default.INSTNAME))
            {
                throw new Exception("机构信息为空");
            }
            bool b = ClassFactory.fuss.regUserInfo(username, password, computerInfo, Properties.Settings.Default.INSTNAME);
            return b;
        }
    }
}
