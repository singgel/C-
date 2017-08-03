using Assistant.DataProviders;
using AssistantUpdater.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AssistantUpdater
{
    public class ServiceHelper
    {
        private static readonly FlexServiceClient client;
        private static readonly DataContractJsonSerializer returnSerializer;

        static ServiceHelper()
        {
            string address = FileHelper.GetTextValue("UpdateAddress");
            if (string.IsNullOrEmpty(address))
                client = new FlexServiceClient("FlexServicePort");
            else
                client = new FlexServiceClient("FlexServicePort", address);
            returnSerializer = new DataContractJsonSerializer(typeof(ReturnCode));
        }

        public static LoginUser Login(string userName, string password, string regCode)
        {
            string result = client.userLogin(userName, password, regCode);
            ReturnCode code = GetReturn(result);
            VerifyReturnCode(code.code);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(LoginUser));
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(result)))
            {
                LoginUser user = serializer.ReadObject(stream) as LoginUser;
                if (user != null)
                {
                    user.userName = userName;
                    user.password = password;
                }
                return user;
            }
        }

        public static List<string> GetAllEnterprises()
        {
            string result = client.getAllEnterprise();
            ReturnCode code = GetReturn(result);
            VerifyReturnCode(code.code);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Enterprise>));
            List<Enterprise> list = null;
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(result)))
            {
                list = serializer.ReadObject(stream) as List<Enterprise>;
            }
            List<string> resultList = new List<string>();
            if (list == null)
                return resultList;
            foreach (var item in list)
            {
                resultList.Add(item.entCname);
            }
            return resultList;
        }
        /// <summary>
        /// 从服务端下载应用程序文件。
        /// </summary>
        /// <param name="entName">企业名称。</param>
        /// <param name="version">应用程序版本。</param>
        /// <param name="fileName">文件绝对路径。</param>
        public static void DownloadAppFile(string entName, string version, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("上传的文件名不能为空！");
            FileInfo info = new FileInfo(fileName);
            string name = info.Name;
            if (info.Extension == ".exe")
                name = string.Format("{0}.remove", info.Name);
            byte[] buffer = client.ufileDownload(entName, version, name);
            if (buffer == null)
                return; 
            using (FileStream stream = info.Create())
            {
                stream.Write(buffer, 0, buffer.Length);
            }
        }
        /// <summary>
        /// 从服务端下载应用程序文件。
        /// </summary>
        /// <param name="entName">企业名称。</param>
        /// <param name="fileName">文件绝对路径。</param>
        public static void DeleteAppFile(string entName, string fileName)
        {
            FileInfo info = new FileInfo(fileName);
            string name = info.Name;
            if (info.Extension == ".exe")
                name = string.Format("{0}.remove", info.Name);
            string result = client.ufileDelete(entName, name);
            ReturnCode code = GetReturn(result);
            VerifyReturnCode(code.code);
        }
        /// <summary>
        /// 获取指定企业所使用的应用程序版本。
        /// </summary>
        /// <param name="entName">企业名称。</param>
        /// <returns></returns>
        public static string GetAppVersion(string entName)
        {
            string result = client.getBbh(entName);
            ReturnCode code = GetReturn(result);
            VerifyReturnCode(code.code);
            return code.message;
        }
        /// <summary>
        /// 将应用程序文件上传到服务端。
        /// </summary>
        /// <param name="entName">企业名称。</param>
        /// <param name="version">应用程序版本。</param>
        /// <param name="fileName">文件绝对路径。</param>
        public static void UploadAppFile(string entName, string version, string fileName)
        {
            FileInfo info = new FileInfo(fileName);
            if(info.Exists == false)
                throw new FileNotFoundException(string.Format("文件{0}不存在！", fileName));
            byte[] buffer = null;
            using (FileStream stream = info.OpenRead())
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
            }
            string name = info.Name;
            if (info.Extension == ".exe")
                name = string.Format("{0}.remove", name);
            string result = client.ufileUpload(entName, version, name, buffer);
            ReturnCode code = GetReturn(result);
            VerifyReturnCode(code.code);
        }
        /// <summary>
        /// 从服务端下载应用程序文件列表。
        /// </summary>
        /// <param name="entName">企业名称。</param>
        /// <param name="version">应用程序版本。</param>
        public static List<AppFileInfo> GetAllFiles(string entName, string version)
        {
            string result = client.ufileDetailGet(entName, version);
            ReturnCode code = GetReturn(result);
            VerifyReturnCode(code.code);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<AppFileInfo>));
            List<AppFileInfo> list = null;
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(result)))
            {
                list = serializer.ReadObject(stream) as List<AppFileInfo>;
            }
            foreach (var item in list)
            {
                if (item.fileName.EndsWith(".remove"))
                    item.fileName = item.fileName.Substring(0, item.fileName.Length - 7);
            }
            return list;
        }
        /// <summary>
        /// 上传规则文件到服务端。
        /// </summary>
        /// <param name="type">规则类型（国环、北环、CCC）。</param>
        /// <param name="version">规则版本（fill、detect、compare）</param>
        /// <param name="standard">排放标准。</param>
        /// <param name="carType">车辆类型。</param>
        /// <param name="filename">文件绝对路径。</param>
        public static void UploadFillRule(string type, string versionName, string standard, string carType, string filename)
        {
            FileInfo info = new FileInfo(filename);
            if(info.Exists == false)
                throw new FileNotFoundException(string.Format("文件{0}不存在！", filename));
            string name = info.Name;
            byte[] bytes = null;
            using (FileStream stream = info.OpenRead())
            {
                bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
            }
            bool useStandard = FileHelper.GetUseStandard(versionName, type);
            if (string.IsNullOrEmpty(standard) || useStandard == false)
                standard = "All";
            if (string.IsNullOrEmpty(carType))
                carType = "All";
            string version = FileHelper.GetFillVersionByName(versionName);
            string result = client.fillRuleUpload(name, type, version, standard, carType, bytes);
            ReturnCode code = GetReturn(result);
            VerifyReturnCode(code.code);
        }
        /// <summary>
        /// 下载规则文件。
        /// </summary>
        /// <param name="type">规则类型（国环、北环、CCC）。</param>
        /// <param name="versionName">规则版本（fill、detect、compare）</param>
        /// <param name="standard">排放标准。</param>
        /// <param name="carType">车辆类型。</param>
        /// <param name="filename">文件绝对路径。</param>
        public static void DownloadFillRule(string type, string versionName, string standard, string carType, string filename)
        {
            FileInfo info = new FileInfo(filename);
            string name = info.Name;
            bool useStandard = FileHelper.GetUseStandard(versionName, type);
            if (string.IsNullOrEmpty(standard) || useStandard == false)
                standard = "All";
            if (string.IsNullOrEmpty(carType))
                carType = "All";
            string version = FileHelper.GetFillVersionByName(versionName);
            byte[] bytes = client.fillRuleDowload(name, type, version, standard, carType);
            if (bytes == null)
                return;
            if (info.Directory.Exists == false)
                info.Directory.Create();
            using (FileStream stream = info.Create())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }
        /// <summary>
        /// 获取指定规则的MD5。
        /// </summary>
        /// <param name="type">规则类型（国环、北环、CCC）。</param>
        /// <param name="version">规则版本（fill、detect、compare）</param>
        /// <param name="standard">排放标准。</param>
        /// <param name="carType">车辆类型。</param>
        /// <param name="filename">文件绝对路径。</param>
        public static string GetFillRuleMd5(string type, string versionName, string standard, string carType, string filename)
        {
            bool useStandard = FileHelper.GetUseStandard(versionName, type);
            FileInfo info = new FileInfo(filename);
            string name = info.Name;
            if (string.IsNullOrEmpty(standard) || useStandard == false)
                standard = "All";
            if (string.IsNullOrEmpty(carType))
                carType = "All";
            string version = FileHelper.GetFillVersionByName(versionName);
            string result = client.fillRuleMdStr(name, type, version, standard, carType);
            try
            {
                ReturnCode code = GetReturn(result);
                if (code.code == "62")
                    return "";
                VerifyReturnCode(code.code);
            }
            catch
            {
                return result;
            }
            return "";
        }

        public static void RegisteUser(string userName, string password, string entName, string regCode)
        {
            string result = client.userRegister(userName, password, entName, regCode);
            ReturnCode code = GetReturn(result);
            VerifyReturnCode(code.code);
        }

        public static string DeleteUser(string userName)
        {
            string result = client.userDelete(userName);
            ReturnCode code = GetReturn(result);
            VerifyReturnCode(code.code);
            return code.message;
        }

        private static void VerifyReturnCode(string code)
        {
            switch (code)
            {
            case "00":
                return;
            case "02":
                throw new ArgumentException("用户名已存在！");
            case "04":
                throw new ArgumentException("超出企业可用license个数！");
            case "99":
                throw new ArgumentException("服务端系统故障，请稍候再试！");
            case "12":
                throw new ArgumentException("该企业不存在！");
            case "22":
            case "52":
                throw new ArgumentException("上传文件失败！");
            case "13":
                throw new ArgumentException("未找到更新程序！");
            case "01":
            case "11":
            case "21":
            case "31":
            case "41":
            case "51":
            case "61":
                throw new ArgumentException("请确认是否已填写所有必填项！");
            case "32":
            case "42":
            case "62":
                throw new FileNotFoundException("未找到指定文件！");
            case "72":
            case "73":
                throw new ArgumentException("登录失败！可能的原因为：\n1：用户不存在！\n2：用户名与密码不相符！\n3：不是在注册所使用的计算机上登录！");
            }
        }

        private static ReturnCode GetReturn(string result)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(result)))
            {
                return returnSerializer.ReadObject(stream) as ReturnCode;
            }
        }
    }
}
