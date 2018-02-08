using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Data;
using CertificateMvcApplication.Models;
using System.IO;

namespace CertificateMvcApplication.Utils
{
    public class ReadFilesUtils
    {
        string FILE_NAMEs = System.Configuration.ConfigurationManager.AppSettings["FileDataURL"];
        static string colNamePath = System.Configuration.ConfigurationManager.AppSettings["ColNamePath"];//列名转换
        /// <summary>
        /// 写入XML
        /// </summary>
        public void WriteFilesXmlData()
        {
            string fileName = System.AppDomain.CurrentDomain.BaseDirectory + FILE_NAMEs;
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode header = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDoc.AppendChild(header);
            XmlElement rootNode = xmlDoc.CreateElement("files");
            List<FileModel> list = GetFiles();
            foreach (var item in list)
            {
                XmlElement xn = InserFiles(item, xmlDoc);
                rootNode.AppendChild(xn);
            }
            xmlDoc.AppendChild(rootNode);
            xmlDoc.Save(fileName);
        }
        /// <summary>
        /// 读取XML
        /// </summary>
        public List<FileModel> ReadFilesXmlData()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + FILE_NAMEs;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode xn = xmlDoc.SelectSingleNode("fileName");
            XmlNodeList xnl = xn.ChildNodes;
            List<FileModel> listModel = new List<FileModel>();
            foreach (XmlNode nodel in xnl)
            {
                FileModel model = new FileModel();
                XmlElement xe = (XmlElement)nodel;
                XmlNodeList xnll = xe.ChildNodes;
                model.id = Int32.Parse(xnll.Item(0).InnerText);
                model.name = xnll.Item(1).InnerText;
                model.pId = Int32.Parse(xnll.Item(2).InnerText);
                listModel.Add(model);
            }
            return listModel;
        }
        /// <summary>
        /// 根据文件获取数据
        /// </summary>
        public List<FileModel> GetFiles()
        {
            List<FileModel> list = new List<FileModel>();
            FileModel fileModel = new FileModel();
            string filePath = System.Configuration.ConfigurationManager.AppSettings["FilePath"];
            DirectoryInfo TheFolder = new DirectoryInfo(filePath);
            DirectoryInfo[] folders = TheFolder.GetDirectories();
            int count = folders.Count();
            for (int i = 0; i < count; i++)
            {
                fileModel = new FileModel();
                fileModel.pId = 0;
                fileModel.id = int.Parse((i + 1) + "0");
                fileModel.name = folders[i].Name;
                list.Add(fileModel);
                FileInfo[] csvFiles = folders[i].GetFiles();
                int countCSV = csvFiles.Count();
                for (int j = 0; j < countCSV; j++)
                {
                    fileModel = new FileModel();
                    fileModel.pId = int.Parse((i + 1) + "0");
                    fileModel.id = int.Parse(String.Format("{0}0{1}", i + 1, j));
                    fileModel.name = csvFiles[j].Name;
                    list.Add(fileModel);
                }
            }
            return list;
        }
        /// <summary>
        /// 根据文件获取父子节点
        /// </summary>
        /// <param name="file">文件地址</param>
        /// <param name="xmlDoc">xml的节点信息</param>
        /// <returns>XmlElement</returns>
        private static XmlElement InserFiles(FileModel file, XmlDocument xmlDoc)
        {
            XmlElement xn = xmlDoc.CreateElement("FileModel");
            xn.AppendChild(GetXmlNode(xmlDoc, "id", file.id.ToString()));
            xn.AppendChild(GetXmlNode(xmlDoc, "pId", file.pId.ToString()));
            xn.AppendChild(GetXmlNode(xmlDoc, "name", file.name));
            return xn;
        }
        /// <summary>
        /// 根据xml的节点名获取数据
        /// </summary>
        /// <param name="xmlDoc">源xml</param>
        /// <param name="name">节点名</param>
        /// <param name="value">节点值</param>
        /// <returns>XmlElement</returns>
        private static XmlElement GetXmlNode(XmlDocument xmlDoc, string name, string value)
        {
            XmlElement xn = xmlDoc.CreateElement(name);
            xn.InnerText = value;
            return xn;
        }

        /// <summary>
        /// 读表头对应关系模板
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ReadColName()
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + colNamePath;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            ImportExcelNPOI npoi = new ImportExcelNPOI();
            DataTable dt = npoi.ExcelToDataTable(filePath, "Sheet1", true);

            dict = dt.Rows.Cast<DataRow>().ToDictionary(x => x[1].ToString(), x => x[0].ToString());
            return dict;
        }

        public static Dictionary<string, string> convertColName(DataRow dr)
        {
            Dictionary<string, string> dictData = new Dictionary<string, string>();
            Dictionary<string, string> dict =ReadColName();//列中文名
            int count = dr.Table.Columns.Count;
            for (int i = 0; i < count;i++ )
            {
                string col = dr.Table.Columns[i].ColumnName;//字段名
                //dictData[dict[col]] = dr[i].ToString();//数值
                dictData.Add(dict[col], dr[i].ToString());
            }
            return dictData;
        }
    }
}