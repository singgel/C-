using AssistantUpdater.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            client = new FlexServiceClient("FlexServicePort");
            returnSerializer = new DataContractJsonSerializer(typeof(ReturnCode));
        }

        public static string Login(string userName, string password)
        {
            string md5 = FileHelper.GetStrMd5(password);
            string result = client.userLogin(userName, md5.ToUpper());
            return result;
        }

        public static string GetAllEnterprise()
        {
            string result = client.getAllEnterprise();
            return result;
        }
        /// <summary>
        /// 从服务端下载应用程序文件。
        /// </summary>
        /// <param name="entName">企业名称。</param>
        /// <param name="version">应用程序版本。</param>
        /// <param name="fileName">文件绝对路径。</param>
        public static void DownloadAppFile(string entName, string version, string fileName)
        {
            FileInfo info = new FileInfo(fileName);
            byte[] buffer = client.ufileDownload(entName, version, info.Name);
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
            string result = client.ufileDelete(entName, info.Name);
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
            string result = client.ufileUpload(entName, version, info.Name, buffer);
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
            VerifyReturnCode(result);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<AppFileInfo>));
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(result)))
            {
                object o = serializer.ReadObject(stream) as List<AppFileInfo>;
                return o as List<AppFileInfo>;
            }
        }
        /// <summary>
        /// 上传规则文件到服务端。
        /// </summary>
        /// <param name="type">规则类型（国环、北环、CCC）。</param>
        /// <param name="version">规则版本（fill、detect、compare）</param>
        /// <param name="standard">排放标准。</param>
        /// <param name="carType">车辆类型。</param>
        /// <param name="filename">文件绝对路径。</param>
        public static void UploadFillRule(string type, string version, string standard, string carType, string filename)
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
            if (string.IsNullOrEmpty(standard))
                standard = "Default";
            if (string.IsNullOrEmpty(carType))
                carType = name;
            string result = client.fillRuleUpload(name, type, version, standard, carType, bytes);
            ReturnCode code = GetReturn(result);
            VerifyReturnCode(code.code);
        }
        /// <summary>
        /// 下载规则文件。
        /// </summary>
        /// <param name="type">规则类型（国环、北环、CCC）。</param>
        /// <param name="version">规则版本（fill、detect、compare）</param>
        /// <param name="standard">排放标准。</param>
        /// <param name="carType">车辆类型。</param>
        /// <param name="filename">文件绝对路径。</param>
        public static void DownloadFillRule(string type, string version, string standard, string carType, string filename)
        {
            FileInfo info = new FileInfo(filename);
            string name = info.Name;
            if (string.IsNullOrEmpty(standard))
                standard = "Default";
            if (string.IsNullOrEmpty(carType))
                carType = name;
            byte[] bytes = client.fillRuleDowload(name, type, version, standard, carType);
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
        public static string GetFillRuleMd5(string type, string version, string standard, string carType, string filename)
        {
            FileInfo info = new FileInfo(filename);
            string name = info.Name;
            if (string.IsNullOrEmpty(standard))
                standard = "Default";
            if (string.IsNullOrEmpty(carType))
                carType = name;
            string result = client.fillRuleMdStr(name, type, version, standard, carType);
            if (result == "62")
                return "";
            VerifyReturnCode(result);
            return result;
        }

        public static void RegisteUser(string userName, string password, string entName, string regCode)
        {
            string result = client.userRegister(userName, password, entName, regCode);
            ReturnCode code = GetReturn(result);
            VerifyReturnCode(code.code);
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
