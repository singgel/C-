using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace download
{
    public partial class Form1 : Form
    {
        public const int threadNumber = 1;
        public HttpDownload[] d;
        public Form1()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ("下载".Equals(button1.Text))
            {
                HttpDownloadFile(textBox1.Text, "D:\\Study P2P\\target\\" + textBox2.Text);
            }
            else if ("继续".Equals(button1.Text))
            {
                listBox1.Items.Clear();
                for (int i = 0; i < d.Length; i++)
                {
                    d[i].IsStop = true;
                }
                Thread[] threads = new Thread[d.Length];
                for (int i = 0; i < d.Length; i++)
                {
                    threads[i] = new Thread(d[i].Receive);
                    threads[i].IsBackground = true;
                    threads[i].Start();
                }
            }
        }

        private void HttpDownloadFile(string sourceuri, string targetfilename)
        {
            if (IsWebResourceAvailable(sourceuri) == false)
            {
                MessageBox.Show("指定的资源无效");
                return;
            }
            listBox1.Items.Add("同时接受线程数：" + threadNumber);
            HttpWebRequest request;
            long fileSize = 0;
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(sourceuri);
                request.Method = WebRequestMethods.Http.Head;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                fileSize = response.ContentLength;
                listBox1.Items.Add("文件大小：" + Math.Ceiling(fileSize / 1024.0f) + "KB");
                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            int downloadFileSize = (int) (fileSize / threadNumber);
            d = new HttpDownload[threadNumber];
            for (int i = 0; i < threadNumber; i++)
            {
                d[i] = new HttpDownload(listBox1, i);
                d[i].StartPosition = downloadFileSize * i;
                if (i < threadNumber - 1)
                {
                    d[i].FileSize = downloadFileSize;
                }
                else
                {
                    d[i].FileSize = (int)(fileSize - downloadFileSize * (i - 1));
                }
                d[i].IsFinish = false;
                d[i].TargetFileName = "D:/Study P2P/target/tmp/" + Path.GetFileNameWithoutExtension(targetfilename) + i + "._tmp";
                //d[i].filename = Path.GetFileNameWithoutExtension(targetfilename) + ".$$" + i;
                d[i].SourceUri = textBox1.Text;
            }
            Thread [] threads = new Thread[threadNumber];
            for (int i = 0; i < threadNumber; i++)
            {
                threads[i] = new Thread(d[i].Receive);
                threads[i].IsBackground = true;
                threads[i].Start();
            }
            CombineFiles c = new CombineFiles(listBox1, d, "D:\\Study P2P\\target\\tmp\\" + textBox2.Text);
            Thread t = new Thread(c.Combine);
            t.IsBackground = true;
            t.Start();
        }

        public static bool IsWebResourceAvailable(string uri)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Http.Head;
                request.Timeout = 2000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch (WebException ex)
            {
                System.Diagnostics.Trace.Write(ex.Message);
                return false;
            }
        }

        public class HttpDownload
        {
            public bool IsFinish { get; set; }
            public bool IsStop = true;
            public string TargetFileName { get; set; }
            public string filename { get; set; }
            public int StartPosition { get; set; }
            public int FileSize { get; set; }
            public string SourceUri { get; set; }
            private int threadIndex;
            private ListBox listbox;
            public FileStream fs;
            private Stopwatch stopwatch = new Stopwatch();
            public HttpDownload(ListBox listbox, int threadIndex)
            {
                this.listbox = listbox;
                this.threadIndex = threadIndex;
            }
            public void Receive()
            {
                int contentLength = 0;
                if (File.Exists(TargetFileName))
                {
                    fs = File.OpenWrite(TargetFileName);
                    //AddStatus(fs.Length.ToString());
                    contentLength = (int)fs.Length;
                    StartPosition += (int)contentLength;
                    fs.Seek(contentLength, SeekOrigin.Current);
                    AddStatus("线程" + threadIndex + "开始接受");
                    if (contentLength >= FileSize)
                    {
                        ChangeStatus("线程" + threadIndex + "开始接受", "接受完毕!", contentLength);
                        this.IsFinish = true;
                        fs.Close();
                        return;
                    }
                }
                else
                {
                    fs = new FileStream(TargetFileName, FileMode.Create);
                    AddStatus("线程" + threadIndex + "开始接受");
                }
                stopwatch.Reset();
                stopwatch.Start();
               
                int i = 0;
                int totalBytes = 0;
                    try
                    {
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(SourceUri);
                        request.AddRange(StartPosition, StartPosition + FileSize - contentLength - 1);
                        Stream stream = request.GetResponse().GetResponseStream();
                        byte[] receiveBytes = new byte[1024];
                        int readBytes = stream.Read(receiveBytes, 0, receiveBytes.Length);
                        while (true)
                        {
                            while (readBytes > 0)
                            {
                                i++;
                                //AddStatus(i.ToString());
                                if (IsStop == true)
                                {
                                    //AddStatus(i.ToString());
                                    fs.Write(receiveBytes, 0, readBytes);
                                    totalBytes += readBytes;
                                    if (i %  200 == 0)
                                    {
                                        ChangeStatus("线程" + threadIndex + "开始接受", "已经接收：", totalBytes);
                                    }
                                    //Thread.Sleep(50);
                                    readBytes = stream.Read(receiveBytes, 0, receiveBytes.Length);
                                    //ChangeStatus("线程" + threadIndex + "开始接受", "已经接收：", totalBytes);
                                }

                                else
                                {
                                    stream.Close();
                                    fs.Close();
                                    stopwatch.Stop();
                                    return;
                                }
                            }
                            stream.Close();
                            fs.Close();
                            ChangeStatus("线程" + threadIndex + "开始接受", "接受完毕!", totalBytes);
                            stopwatch.Stop();
                            this.IsFinish = true;
                            break;
                        }
                        }
                    catch (Exception ex)
                    {
                        AddStatus("线程" + threadIndex + "接受出错" + ex.Message);
                    }
               
            
            }

            public delegate void AddStatusDelegate(string message);
            public void AddStatus(string message)
            {
                if (listbox.InvokeRequired)
                {
                    AddStatusDelegate d = AddStatus;
                    listbox.Invoke(d, message);
                }
                else
                {
                    listbox.Items.Add(message);
                }
            }

            public delegate void ChangeStatusDelegate(string oldMessage, string newMessage, int number);
            public void ChangeStatus(string oldMessage, string newMessage, int number)
            {
                if (listbox.InvokeRequired)
                {
                    ChangeStatusDelegate d = ChangeStatus;
                    listbox.Invoke(d, oldMessage, newMessage, number);
                }
                else
                {
                    int i = listbox.FindString(oldMessage);
                    if (i != -1)
                    {
                        string [] items = new string[listbox.Items.Count];
                        listbox.Items.CopyTo(items, 0);
                        items[i] = oldMessage + " " + newMessage + " 接受字节数：" + Math.Ceiling(number / 1024.0f) + "KB"
                            + "，用时：" + stopwatch.ElapsedMilliseconds / 1000.0f + "秒";
                        listbox.Items.Clear();
                        listbox.Items.AddRange(items);
                        listbox.SelectedIndex = i;
                    }
                }
            }
        }

        public class CombineFiles
        {
            private bool downloadFinish;
            private HttpDownload[] down;
            private ListBox listbox;
            string targetFileName;
            public CombineFiles(ListBox listbox, HttpDownload[] down, string targetFileName)
            {
                this.listbox = listbox;
                this.down = down;
                this.targetFileName = targetFileName;
            }
            public void Combine()
            {
                while (true)
                {
                    downloadFinish = true;
                    for (int i = 0; i < down.Length; i++)
                    {
                        if (down[i].IsFinish == false)
                        {
                            downloadFinish = false;
                            Thread.Sleep(100);
                            break;
                        }
                    }
                    if (downloadFinish == true)
                    {
                        break;
                    }
                }
                AddStatus("下载完毕，开始合并临时文件！");
                FileStream targetFileStream;
                FileStream sourceFileStream;

                int readfile;
                byte[] bytes = new byte[8192];
                targetFileStream = new FileStream(targetFileName, FileMode.Create);
                for (int k = 0; k < down.Length; k++)
                {
                    sourceFileStream = new FileStream(down[k].TargetFileName, FileMode.Open);
                    //FileStream testfilestream = new FileStream(down[k].filename, FileMode.Create);
                    while (true)
                    {
                        readfile = sourceFileStream.Read(bytes, 0, bytes.Length);
                        if (readfile > 0)
                        {
                            targetFileStream.Write(bytes, 0, readfile);
                        }
                        else
                        {
                            break;
                        }
                    }
                    sourceFileStream.Close();
                }
                targetFileStream.Close();
                //FileStream testfilestream = new FileStream("F:/study/test/test1.mp3", FileMode.Create);
                for (int i = 0; i < down.Length; i++)
                {
                    //FileStream testfilestream = new FileStream(down[i].filename, FileMode.Create);
                    //File.Delete(down[i].TargetFileName);
                }
                DateTime dt = DateTime.Now;
                AddStatus("合并完毕！ ");
            }
            public delegate void AddStatusDelegate(string message);
            public void AddStatus(string message)
            {
                if (listbox.InvokeRequired)
                {
                    AddStatusDelegate d = AddStatus;
                    listbox.Invoke(d, message);
                }
                else
                {
                    listbox.Items.Add(message);
                    listbox.SelectedIndex = -1;
                }
            }
        }




        private void button2_Click(object sender, EventArgs e)
        {
            button1.Text = "继续";
            AddStatus("暂时暂停接收");
            for (int i = 0; i < d.Length; i++)
            {
                d[i].IsStop = false;
                //FileStream testfilestream = new FileStream(down[i].filename, FileMode.Create);
                //File.Delete(down[i].TargetFileName);
            }
        }

        public delegate void AddStatusDelegate(string message);
        public void AddStatus(string message)
        {
            if (listBox1.InvokeRequired)
            {
                AddStatusDelegate d = AddStatus;
                listBox1.Invoke(d, message);
            }
            else
            {
                listBox1.Items.Add(message);
                listBox1.SelectedIndex = -1;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < d.Length; i++)
            {
                d[i].IsStop = true;
                //FileStream testfilestream = new FileStream(down[i].filename, FileMode.Create);
                //File.Delete(down[i].TargetFileName);
            }
            Thread [] threads = new Thread[d.Length];
            for (int i = 0; i < d.Length; i++)
            {
                threads[i] = new Thread(d[i].Receive);
                threads[i].Start();
            }
            /*if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }*/

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
            {
                this.textBox1.Text = (string)iData.GetData(DataFormats.Text);
            }
        }
    }
}
