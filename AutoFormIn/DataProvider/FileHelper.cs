using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Data.Collection;
using System.Xml;

namespace Assistant.DataProviders
{
    public class FileHelper
    {
        private static readonly MD5 _md5;
        private static readonly XmlDocument doc;
        private const string configName = "config.xml";

        static FileHelper()
        {
            _md5 = MD5.Create();
            doc = new XmlDocument();
            if (File.Exists(configName))
                doc.Load(configName);
        }

        public static string GetStrMd5(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            buffer = _md5.ComputeHash(buffer);
            StringBuilder result = new StringBuilder();
            foreach (var item in buffer)
            {
                result.Append(item.ToString("x2"));
            }
            return result.ToString();
        }

        public static string GetFileMd5(string filename)
        {
            if (File.Exists(filename) == false)
                return null;
            byte[] bytes = null;
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                bytes = _md5.ComputeHash(stream);
            }
            StringBuilder result = new StringBuilder();
            foreach (var item in bytes)
            {
                result.Append(item.ToString("x2"));
            }
            return result.ToString();
        }
        /// <summary>
        /// 获取企业名称。
        /// </summary>
        /// <returns></returns>
        public static string GetEntName()
        {
            XmlNode node = doc.SelectSingleNode("/Configurations/EntName");
            XmlAttribute attr = node == null ? null : node.Attributes["Name"];
            return attr == null ? "" : attr.Value;
        }
        /// <summary>
        /// 获取指定站点的可用车辆类型。
        /// </summary>
        /// <param name="fillType">站点。</param>
        /// <returns></returns>
        public static TreeModel GetCarTypeList(string fillType)
        {
            TreeModel result = new TreeModel();
            RuleCompareNode ruleNode = new RuleCompareNode() { Header = fillType, IsExpanded = true };
            result.AddChild(ruleNode);
            XmlNodeList nodeList = doc.SelectNodes(string.Format("//Sites/Site[@Name=\"{0}\"]/Standard", fillType));
            if (nodeList == null)
                return result;
            foreach (XmlNode item in nodeList)
            {
                RuleCompareNode child = new RuleCompareNode();
                XmlAttribute attr = item.Attributes["Name"];
                child.Header = attr == null ? "" : attr.Value;
                attr = item.Attributes["Value"];
                child.Content = attr == null ? null : attr.Value;
                ruleNode.AddChild(child);
                foreach (XmlNode childItem in item.ChildNodes)
                {
                    XmlText text = childItem.FirstChild as XmlText;
                    if (text != null)
                    {
                        RuleCompareNode textNode = new RuleCompareNode();
                        textNode.Header = text.Value;
                        child.AddChild(textNode);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 获取所有可用站点。
        /// </summary>
        /// <returns></returns>
        public static List<string> GetWebsites()
        {
            List<string> sites = new List<string>();
            XmlNodeList list = doc.SelectNodes("/Configurations/Sites/Site");
            if (list == null)
                return sites;
            foreach (XmlNode item in list)
            {
                XmlAttribute attr = item.Attributes["Name"];
                if (attr != null)
                    sites.Add(attr.Value);
            }
            return sites;
        }
        /// <summary>
        /// 获取所有底盘类型。
        /// </summary>
        /// <param name="fillType"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetAppendixes(string fillType)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            XmlNodeList nodeList = doc.SelectNodes(string.Format("//Appendixes/Appendix", fillType));
            if (nodeList == null)
                return list;
            foreach (XmlNode node in nodeList)
            {
                XmlAttribute attr = node.Attributes["Name"];
                string key = attr == null ? "" : attr.Value;
                attr = node.Attributes["Value"];
                string value = attr == null ? "" : attr.Value;
                list.Add(new KeyValuePair<string, string>(key, value));
            }
            return list;
        }

        public static string GetTextValue(string nodeName)
        {
            XmlNode node = doc.SelectSingleNode(string.Format("/Configurations/{0}/text()[1]", nodeName));
            return node == null ? "" : node.Value;
        }
        /// <summary>
        /// 获得规则文件列表。
        /// </summary>
        /// <param name="fillType">规则类型。</param>
        /// <param name="basePath">父目录。</param>
        /// <param name="canUseAppendix">当前规则是否可使用底盘信息。</param>
        /// <returns></returns>
        public static List<UpdateRuleParameter> GetRuleFileList(string version, string fillType, string basePath, out bool canUseAppendix)
        {
            string path = string.Format(fillType);
            List<UpdateRuleParameter> result = new List<UpdateRuleParameter>();
            XmlNode node = doc.SelectSingleNode(string.Format("//FillRuleFileList[@Version=\"{0}\"]/FillRule[@Name=\"{1}\"]/PublicPage/@RelativeFilePath", version, fillType));
            UpdateRuleParameter parameter = new UpdateRuleParameter();
            if (node != null)
            {
                parameter.Type = fillType;
                parameter.FileName = node == null ? "" : string.Format("{0}\\{1}", basePath, node.Value);
                parameter.Standard = "All";
                parameter.CarType = "All";
                result.Add(parameter);
            }
            node = doc.SelectSingleNode(string.Format("//FillRuleFileList[@Version=\"{0}\"]/FillRule[@Name=\"{1}\"]/RuleFiles", version, fillType));
            if (node == null)
            {
                canUseAppendix = false;
                return result;
            }
            XmlAttribute attr = node.Attributes["CanUseAppendix"];
            if (attr == null || bool.TryParse(attr.Value, out canUseAppendix) == false)
                canUseAppendix = false;

            foreach (XmlNode child in node.ChildNodes)
            {
                parameter = new UpdateRuleParameter();
                parameter.Type = fillType;
                attr = child.Attributes["Standard"];
                parameter.Standard = attr == null ? "All" : (string.IsNullOrEmpty(attr.Value) ? "All" : attr.Value);
                attr = child.Attributes["CarType"];
                parameter.CarType = attr == null ? "All" : (string.IsNullOrEmpty(attr.Value) ? "All" : attr.Value);
                attr = child.Attributes["RelativeFilePath"];
                parameter.FileName = node == null ? "" : string.Format("{0}\\{1}", basePath, attr.Value);
                result.Add(parameter);
            }
            return result;
        }

        public static List<string> GetFillTypes()
        {
            List<string> result = new List<string>();
            XmlNodeList nodeList = doc.SelectNodes("//FillTypes/FillType/text()[1]");
            if (nodeList == null)
                return result;
            foreach (XmlNode node in nodeList)
            {
                if (node != null && string.IsNullOrEmpty(node.Value) == false)
                    result.Add(node.Value.ToUpper());
            }
            return result;
        }

        /// <summary>
        /// 获取当前正在运行的程序的权限功能。
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFillRights()
        {
            List<string> result = new List<string>();
            XmlNodeList nodeList = doc.SelectNodes("//FillRights/FillType/text()[1]");
            if (nodeList == null)
                return result;
            foreach (XmlNode node in nodeList)
            {
                if (node != null && string.IsNullOrEmpty(node.Value) == false)
                    result.Add(node.Value);
            }
            return result;
        }

        public static List<FillRuleVersion> GetFillRuleVersions()
        {
            List<FillRuleVersion> result = new List<FillRuleVersion>();
            XmlNodeList nodeList = doc.SelectNodes("//FillRuleVersion/RuleVersion");
            if (nodeList == null)
                return result;
            foreach (XmlNode node in nodeList)
            {
                FillRuleVersion ruleVersion = new FillRuleVersion();
                XmlAttribute attr = node.Attributes["VersionName"];
                ruleVersion.Name = attr == null ? "" : attr.Value;
                attr = node.Attributes["UseAppendix"];
                bool a;
                if (attr == null || bool.TryParse(attr.Value, out a) == false)
                    a = false;
                ruleVersion.UseAppendix = a;
                XmlText text = node.FirstChild as XmlText;
                if (text != null && string.IsNullOrEmpty(text.Value) == false)
                {
                    ruleVersion.Value = text.Value.ToUpper();
                    result.Add(ruleVersion);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取当前正在运行的程序的版本号。
        /// </summary>
        /// <returns></returns>
        public static Version GetCurrentVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetName().Version;
        }
        /// <summary>
        /// 使用填报规则的版本名称获取规则文件的存储目录。
        /// </summary>
        /// <param name="versionName">填报规则的版本名称。</param>
        /// <returns></returns>
        public static string GetFillVersionByName(string versionName)
        {
            XmlNode node = doc.SelectSingleNode(string.Format("//FillRuleVersion/RuleVersion[@VersionName=\"{0}\"]/text()[1]", versionName));
            return node == null ? "" : node.Value == null ? "" : node.Value.ToUpper();
        }
        /// <summary>
        /// 获取在指定的填报版本中是否根据“底盘类型”分别填报。
        /// </summary>
        /// <param name="versionName">填报版本。</param>
        /// <returns></returns>
        public static bool GetIsUseAppendixInFillVersion(string versionName)
        {
            XmlNode node = doc.SelectSingleNode(string.Format("//FillRuleVersion/RuleVersion[@VersionName=\"{0}\"]/@UseAppendix", versionName));
            if (node != null)
            {
                bool result;
                if (bool.TryParse(node.Value, out result))
                    return result;
                return false;
            }
            return false;
        }
        /// <summary>
        /// 获取在填报时第一页的Url地址。
        /// </summary>
        /// <param name="fillType">填报类型</param>
        /// <returns></returns>
        public static string GetStartPageUri(string fillType)
        {
            XmlNode node = doc.SelectSingleNode(string.Format("//Sites/Site[@Name=\"{0}\"]/StartFillPageUri/text()[1]", fillType));
            return node == null ? "" : node.Value;
        }
        /// <summary>
        /// 获取在填报时最后一页的Url地址。
        /// </summary>
        /// <param name="fillType">填报类型</param>
        /// <returns></returns>
        public static string GetEndPageUri(string fillType)
        {
            XmlNode node = doc.SelectSingleNode(string.Format("//Sites/Site[@Name=\"{0}\"]/EndFillPageUri/text()[1]", fillType));
            return node == null ? "" : node.Value;
        }
        /// <summary>
        /// 获取在登录后进入的第一页的Url地址。
        /// </summary>
        /// <param name="fillType">填报类型</param>
        /// <returns></returns>
        public static string GetJumpPage(string fillType)
        {
            XmlNode node = doc.SelectSingleNode(string.Format("//Sites/Site[@Name=\"{0}\"]/JumpWhenPage/text()[1]", fillType));
            return node == null ? "" : node.Value;
        }
        /// <summary>
        /// 获取填报类型所对应的的公共文件。
        /// </summary>
        /// <param name="fillType">填报类型。</param>
        /// <returns></returns>
        public static string GetPublicPage(string version, string fillType)
        {
            XmlNode node = doc.SelectSingleNode(string.Format("//FillRuleFileList[@Version=\"{0}\"]/FillRule[@Name=\"{1}\"]/PublicPage/@RelativeFilePath", version, fillType));
            if (node == null)
                return "";
            return node.Value;
        }
        /// <summary>
        /// 获取由填报类型、排放标准及车辆类型参数所确定的填报规则文件。
        /// </summary>
        /// <param name="fillType">填报类型。</param>
        /// <param name="standard">排放标准，可为空。</param>
        /// <param name="carType">车辆类型，可为空。</param>
        /// <returns></returns>
        public static string GetFillRuleFile(string version, string fillType, string standard, string carType)
        {
            XmlNode node = doc.SelectSingleNode(string.Format("//FillRuleFileList[@Version=\"{0}\"]/FillRule[@Name=\"{1}\"]/RuleFiles", version , fillType));
            if (node == null)
                return "";
            bool useStandard = false;
            XmlAttribute attr = node.Attributes["UseStandard"];
            if (attr == null || bool.TryParse(attr.Value, out useStandard) == false)
                useStandard = false;
            if (string.IsNullOrEmpty(standard) || useStandard == false)
                standard = "All";
            if (string.IsNullOrEmpty(carType))
                carType = "All";
            node = node.SelectSingleNode(string.Format("RuleFile[@Standard=\"{0}\" and @CarType=\"{1}\"]/@RelativeFilePath", standard, carType));
            if (node == null)
                return "";
            return node.Value;
        }

        public static Hashtable Get3CSpecialParameter()
        {
            Hashtable result = new Hashtable();
            XmlNode node = doc.SelectSingleNode(string.Format("//FillRuleFileList[@Version=\"填报规则\"]/FillRule[@Name=\"CCC\"]/SpecialParameter"));
            if (node == null)
                return result;
            XmlText text = node.FirstChild as XmlText;
            string content = text == null ? "" : text.Value;
            foreach (var item in content.Split(','))
            {
                result.Add(item.Trim(), null);
            }
            return result;
        }

        public static string GetValidateFile(string fillType)
        {
            XmlNode node = doc.SelectSingleNode(string.Format("//FillRuleFileList[@Version=\"填报规则\"]/FillRule[@Name=\"{1}\"]/PublicPage/@RelativeFilePath", fillType));
            if (node == null)
                return "";
            return node.Value;
        }

        public static bool GetUseStandard(string version, string type)
        {
            XmlNode node = doc.SelectSingleNode(string.Format("//FillRuleFileList[@Version=\"{0}\"]/FillRule[@Name=\"{1}\"]/RuleFiles", version, type));
            if (node == null)
                return false;
            bool useStandard = false;
            XmlAttribute attr = node.Attributes["UseStandard"];
            if (attr == null || bool.TryParse(attr.Value, out useStandard) == false)
                useStandard = false;
            return useStandard;
        }

        public static string GetConverterFile(string fillType)
        {
            XmlNode node = doc.SelectSingleNode("//FillRule[@Name=\"转换规则\"]/PublicPage/@RelativeFilePath");
            return node == null ? "" : node.Value == null ? "" : node.Value.ToUpper();
        }

        public static bool GetIsUseConverter(string version, string fillType)
        {
            XmlNode node = doc.SelectSingleNode(string.Format("//FillRuleFileList[@Version=\"{0}\"]/FillRule[@Name=\"{1}\"]/@UseConverter", version, fillType));
            if (node == null)
                return false;
            bool useConverter = false;
            if (bool.TryParse(node.Value, out useConverter) == false)
                useConverter = false;
            return useConverter;
        }

        public static Hashtable GetAllFilesFromDirectory(string directory)
        {
            Hashtable table = new Hashtable();
            string[] files = Directory.GetFiles(directory);
            files = Directory.GetFiles(directory);
            List<string> fileList = null;
            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (table.ContainsKey(name))
                    fileList = table[name] as List<string>;
                else
                {
                    fileList = new List<string>();
                    table.Add(name, fileList);
                }
                fileList.Add(file);
            }
            return table;
        }
    }
}
