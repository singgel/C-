using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Utils_Form.Utils_Configure;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Collections;

namespace FuelDataSysClient.Form_Configure
{
    public partial class UpdaterForm : DevExpress.XtraEditors.XtraForm
    {
        private string updateUrl = string.Empty;
        private string tempUpdatePath = string.Empty;
        XmlFiles updaterXmlFiles = null;
        private int availableUpdate = 0;
        string mainAppExe = "";

        public UpdaterForm()
        {
            InitializeComponent();
        }

        private void UpdaterForm_Load(object sender, EventArgs e)
        {
            string localXmlFile = Application.StartupPath + "\\ExcelHeaderTemplate\\UpdateList.xml";
            string serverXmlFile = string.Empty;
            try
            {
                //从本地读取更新配置文件信息
                updaterXmlFiles = new XmlFiles(localXmlFile);
            }
            catch
            {
                MessageBox.Show("配置文件出错!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            //获取服务器地址
            AppUpdater appUpdater = new AppUpdater();
            try
            {
                updateUrl = updaterXmlFiles.GetNodeValue("//Url");
                appUpdater.UpdaterUrl = updateUrl + "/UpdateList.xml";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("获取服务器地址失败,失败原因为：" + ex.Message);
                this.Close();
                return;
            }

            //与服务器连接,下载更新配置文件
            try
            {
                string strTime = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
                tempUpdatePath = String.Format("{0}\\_{1}_{2}\\", Environment.GetEnvironmentVariable("Temp"), updaterXmlFiles.FindNode("//Application").Attributes["applicationId"].Value, strTime);
                appUpdater.DownAutoUpdateFile(tempUpdatePath);
            }
            catch
            {
                MessageBox.Show("与服务器连接失败,操作超时!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;

            }

            //获取更新文件列表
            Hashtable htUpdateFile = new Hashtable();
            try
            {
                serverXmlFile = tempUpdatePath + "UpdateList.xml";
                if (!File.Exists(serverXmlFile))
                {
                    return;
                }

                availableUpdate = appUpdater.CheckForUpdate(serverXmlFile, localXmlFile, out htUpdateFile);
                if (availableUpdate > 0)
                {
                    for (int i = 0; i < htUpdateFile.Count; i++)
                    {
                        string[] fileArray = (string[])htUpdateFile[i];
                        lvUpdateList.Items.Add(new ListViewItem(fileArray));
                    }
                }
                else
                {
                    System.IO.Directory.Delete(tempUpdatePath, true);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("更新文件列表更新失败,失败原因为：" + ex.Message);
            }

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (availableUpdate > 0)
                {
                    Thread threadDown = new Thread(DownUpdateFile) { IsBackground = true };
                    threadDown.Start();
                }
                else
                {
                    MessageBox.Show("没有可用的更新!", "自动更新", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("下载更新失败,失败原因为：" + ex.Message);
            }

        }
        private void DownUpdateFile()
        {
            bool bRet = false;
            Invoke((MethodInvoker)delegate
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    mainAppExe = updaterXmlFiles.GetNodeValue("//EntryPoint");
                    for (int i = 0; i < this.lvUpdateList.Items.Count; i++)
                    {
                        int readFileLength = 0;
                        try
                        {
                            string UpdateFile = lvUpdateList.Items[i].Text.Trim();
                            string updateFileUrl = updateUrl + lvUpdateList.Items[i].Text.Trim();
                            long fileLength = 0;

                            WebRequest webReq = WebRequest.Create(updateFileUrl);
                            WebResponse webRes = webReq.GetResponse();
                            fileLength = webRes.ContentLength;

                            lbState.Text = "正在下载更新文件,请稍后...";
                            pbDownFile.Value = 0;
                            pbDownFile.Maximum = (int)fileLength;

                            try
                            {
                                Stream srm = webRes.GetResponseStream();
                                StreamReader srmReader = new StreamReader(srm);
                                byte[] bufferbyte = new byte[fileLength];
                                int allByte = (int)bufferbyte.Length;
                                int startByte = 0;
                                while (fileLength > 0)
                                {
                                    Application.DoEvents();
                                    int downByte = srm.Read(bufferbyte, startByte, allByte);
                                    if (downByte == 0) { break; };
                                    startByte += downByte;
                                    allByte -= downByte;
                                    pbDownFile.Value += downByte;
                                    readFileLength += downByte;
                                    float part = (float)startByte / 1024;
                                    float total = (float)bufferbyte.Length / 1024;
                                    int percent = Convert.ToInt32((part / total) * 100);

                                    this.lvUpdateList.Items[i].SubItems[2].Text = percent.ToString() + "%";

                                }
                                if (readFileLength != fileLength)
                                {
                                    throw new Exception("文件读取不完整");
                                }
                                string tempPath = tempUpdatePath + UpdateFile;
                                CreateDirtory(tempPath);
                                FileStream fs = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.Write);
                                fs.Write(bufferbyte, 0, bufferbyte.Length);
                                srm.Close();
                                srmReader.Close();
                                fs.Close();


                            }
                            catch (WebException ex)
                            {
                                MessageBox.Show("更新文件下载失败！" + ex.Message.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                bRet = true;
                                break;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("更新文件下载失败！" + ex.Message.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            bRet = true;
                            break;
                        }

                    }

                    this.Cursor = Cursors.Default;
                    if (bRet)
                    {
                        System.IO.Directory.Delete(tempUpdatePath, true);
                        lbState.Text = "更新文件下载失败,无法安装,请重新下载文件...";
                    }
                    else
                    {
                        btnFinish.Visible = true;
                        btnNext.Visible = false;
                        lbState.Text = "下载文件完成,请点击安装更新文件...";
                    }

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("下载更新失败,失败原因为：" + ex.Message);
                }

            });

        }

        //创建目录
        private void CreateDirtory(string path)
        {
            if (!File.Exists(path))
            {
                string[] dirArray = path.Split('\\');
                string temp = string.Empty;
                for (int i = 0; i < dirArray.Length - 1; i++)
                {
                    temp += dirArray[i].Trim() + "\\";
                    if (!Directory.Exists(temp))
                        Directory.CreateDirectory(temp);
                }
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
                this.DialogResult = DialogResult.OK;
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "AutoUpdater.exe" /*临时文件路径  */, Arguments = "\"\"" + tempUpdatePath + " \"" + mainAppExe + "\"" /*主进程*/, WindowStyle = ProcessWindowStyle.Normal };
                Process.Start(startInfo);

            }
            catch (Exception ex)
            {
                MessageBox.Show("拷贝失败，失败原因为:" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}