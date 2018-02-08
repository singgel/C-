using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Common;
using System.Windows.Forms;

namespace FuelDataSysClient.Tool
{
    public class AutoSocket
    {
        ListBox userLoginLog;   //主窗体的日志控件
        //生产线1 
         string listenerPort1 = FuelDataSysClient.Properties.Settings.Default.SocketPort1;
         Socket listener1;
         private byte[] m_byBuff1 = new byte[45];


         //生产线2-1
         string listenerPort2_1 = FuelDataSysClient.Properties.Settings.Default.SocketPort2_1;
         Socket listener2_1;
         private byte[] m_byBuffRecieve2_1 = new byte[500];
         private byte[] m_byBuffSend2_1 = new byte[500];

         //生产线2-2
         string listenerPort2_2 = FuelDataSysClient.Properties.Settings.Default.SocketPort2_2;
         Socket listener2_2;
         private byte[] m_byBuffRecieve2_2 = new byte[500];
         private byte[] m_byBuffSend2_2 = new byte[500];


         //生产线3_1 
         string listenerPort3_1 = FuelDataSysClient.Properties.Settings.Default.SocketPort3_1;
         Socket listener3_1;
         private byte[] m_byBuffRecieve3_1 = new byte[500];
         private byte[] m_byBuffSend3_1 = new byte[500];
         string plname3_1 = "PL3_1";

         //生产线3_2 
         string listenerPort3_2 = FuelDataSysClient.Properties.Settings.Default.SocketPort3_2;
         Socket listener3_2;
         private byte[] m_byBuffRecieve3_2 = new byte[500];
         private byte[] m_byBuffSend3_2 = new byte[500];
         string plname3_2 = "PL3_2";

         public void Import(ListBox listBox)
        {
            this.userLoginLog = listBox;
            //获取本机ip
            String strHostName = string.Empty;
            IPAddress[] aryLocalAddr = null;
            do 
            {
                try
                {
                    strHostName = Dns.GetHostName();
                    IPHostEntry ipEntry = Dns.GetHostByName(strHostName);
                    aryLocalAddr = ipEntry.AddressList;
                    break;
                }
                catch (Exception ex)
                {
                    string error = string.Format("本地IP地址错误 {0} ", ex.Message);
                    LogManager.Log("SocketLog", "Error", error);
                }

                if (aryLocalAddr == null || aryLocalAddr.Length < 1)
                {
                    string info = "无法获得本地地址";
                    LogManager.Log("SocketLog", "Error", info);
                }
            } while (true);
           
            //创建监听端口
            try
            {
                listener1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener1.Bind(new IPEndPoint(aryLocalAddr[0], Convert.ToInt32(listenerPort1)));
                listener1.Listen(5);
                
                listener2_1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener2_1.Bind(new IPEndPoint(aryLocalAddr[0], Convert.ToInt32(listenerPort2_1)));
                listener2_1.Listen(5);

                listener2_2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener2_2.Bind(new IPEndPoint(aryLocalAddr[0], Convert.ToInt32(listenerPort2_2)));
                listener2_2.Listen(5);

               
                listener3_1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener3_1.Bind(new IPEndPoint(aryLocalAddr[0], Convert.ToInt32(listenerPort3_1)));
                listener3_1.Listen(5);

                listener3_2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener3_2.Bind(new IPEndPoint(aryLocalAddr[0], Convert.ToInt32(listenerPort3_2)));
                listener3_2.Listen(5);

                Thread thread1 = new Thread(ListenClientConnect1);
                thread1.IsBackground = true;
                thread1.Start();

                Thread thread2_1 = new Thread(ListenClientConnect2_1);
                thread2_1.IsBackground = true;
                thread2_1.Start();

                Thread thread2_2 = new Thread(ListenClientConnect2_2);
                thread2_2.IsBackground = true;
                thread2_2.Start();

                Thread thread3_1 = new Thread(ListenClientConnect3_1);
                thread3_1.IsBackground = true;
                thread3_1.Start();

                Thread thread3_2 = new Thread(ListenClientConnect3_2);
                thread3_2.IsBackground = true;
                thread3_2.Start();
            }
            catch (System.Exception ex)
            {
                LogManager.Log("SocketLog", "Error", ex.Message);
            }
        }
        public void ListenClientConnect1()
        {
            while (true)
            {
                string receiveStr = string.Empty;
                IPEndPoint localIP = null;
                bool result = false;
                string strAddress = "";
                Int32 Port = 0;
                try
                {
                    Socket clientSocket = listener1.Accept();
                    var remoteIP = ((IPEndPoint)clientSocket.RemoteEndPoint);
                    localIP = ((IPEndPoint)clientSocket.LocalEndPoint);
                    strAddress = remoteIP.Address.ToString();
                    Port = remoteIP.Port;
                    int receiveNumber = clientSocket.Receive(m_byBuff1);
                    receiveStr = Encoding.ASCII.GetString(m_byBuff1, 0, receiveNumber);

                    Log("生产线1:" + strAddress + ":" + Port + " 接收数据" + receiveStr);
                    clientSocket.Send(m_byBuff1);
                    clientSocket.Close();
                    result = true;
                }
                catch (Exception ex)
                {
                    LogManager.Log("SocketLog", "Error", ex.Message);
                }
                if (result)
                {
                    AnalysisData1(receiveStr, Port, strAddress);
                }
            }
        }
        public void AnalysisData1(string receiveStr, Int32 port, string ip)
        {
           /* string str = "";
            str  = receiveStr.Substring(0, 1);
            str = receiveStr.Substring(1, 4);
            str = receiveStr.Substring(5, 10);
            str = receiveStr.Substring(15, 2);
            str = receiveStr.Substring(17, 14);
            str = receiveStr.Substring(31, 8);
            str = receiveStr.Substring(39,4);
            str = receiveStr.Substring(43, 3);
            str = receiveStr.Substring(46, 3);
            str = receiveStr.Substring(49, 14);
            str = receiveStr.Substring(63, 37);*/
            try
            {
                string strVin = receiveStr.Substring(0, 17);
                string strDate = receiveStr.Substring(17, 12)+"00";
                string strOCN = receiveStr.Substring(31, 4);

                InsertVin(strVin, strDate, strOCN, receiveStr, port, ip, "1");
                this.userLoginLog.Invoke(new Action(() =>
                {
                    this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "== 自动接收到 VIN:" + strVin + " 生产制造日期:" + strDate + " 生产OCN:" + strOCN + " ==");
                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                }));
            }
            catch (System.Exception ex)
            {
                LogManager.Log("SocketLog", "Error", ex.Message);
            }
            
        }
        public void ListenClientConnect2_1()
        {
          
            StringBuilder receiveStr = new StringBuilder();
            StringBuilder sendStr = new StringBuilder();
            IPEndPoint localIP = null;
            bool result = false;
            string strAddress = "";
            Int32 Port = 0;
            StringBuilder sendStrPlace = new StringBuilder();
            sendStrPlace.Append("00");//DataType
            sendStrPlace.Append("00000");//spoolPoint
            sendStrPlace.Append("00000000");//prod Date
            sendStrPlace.Append("0000");//STN CD
            sendStrPlace.Append("0000");//SEQ
            sendStrPlace.Append("000000000000");//BodyNo
            int z = 0;
            while (true)
            {
                result = false;
                
                try
                {

                    Socket clientSocket = listener2_1.Accept();
                    var remoteIP = ((IPEndPoint)clientSocket.RemoteEndPoint);
                    localIP = ((IPEndPoint)clientSocket.LocalEndPoint);
                    strAddress = remoteIP.Address.ToString();
                    Port = remoteIP.Port;

                    int receiveNumber = clientSocket.Receive(m_byBuffRecieve2_1);
                    receiveStr = new StringBuilder();
                    receiveStr.Append(Encoding.ASCII.GetString(m_byBuffRecieve2_1, 0, receiveNumber));
                    Log("生产线2_1:" + strAddress + ":" + Port + "第一次接收数据" + receiveStr);

                    sendStr = new StringBuilder();
                    sendStr.Append(receiveStr.ToString());
                    sendStr.Append(sendStrPlace.ToString());
                    Log("生产线2_1:" + strAddress + ":" + Port + "第一次发送数据" + sendStr);
                    m_byBuffSend2_1 = Encoding.Default.GetBytes(sendStr.ToString());
                    clientSocket.Send(m_byBuffSend2_1);


                    receiveNumber = clientSocket.Receive(m_byBuffRecieve2_1);
                    receiveStr = new StringBuilder();
                    receiveStr.Append(Encoding.ASCII.GetString(m_byBuffRecieve2_1, 0, receiveNumber));
                    Log("生产线2_1:" + strAddress + ":" + Port + "第二次接收数据" + receiveStr);

                    sendStr = new StringBuilder();
                    sendStr.Append(receiveStr.ToString().Substring(0, 45));
                    sendStr.Append("OK");

                    Log("生产线2_1:" + strAddress + ":" + Port + "第二次发送数据" + sendStr);
                    m_byBuffSend2_1 = Encoding.Default.GetBytes(sendStr.ToString());
                    clientSocket.Send(m_byBuffSend2_1);


                    sendStrPlace = new StringBuilder();
                    sendStrPlace.Append(receiveStr.ToString().Substring(10, 35));
                   
 
                    clientSocket.Close();
                    result = true;
                }
                catch (Exception ex)
                {
                    LogManager.Log("SocketLog", "Error", ex.Message);
                }
                if (result)
                {
                    AnalysisData2(receiveStr.ToString(), Port, strAddress);
                }
            }
        }

        public void ListenClientConnect2_2()
        {

            StringBuilder receiveStr = new StringBuilder();
            StringBuilder sendStr = new StringBuilder();
            IPEndPoint localIP = null;
            bool result = false;
            string strAddress = "";
            Int32 Port = 0;
            StringBuilder sendStrPlace = new StringBuilder();
            sendStrPlace.Append("00");//DataType
            sendStrPlace.Append("00000");//spoolPoint
            sendStrPlace.Append("00000000");//prod Date
            sendStrPlace.Append("0000");//STN CD
            sendStrPlace.Append("0000");//SEQ
            sendStrPlace.Append("000000000000");//BodyNo
            int z = 0;
            while (true)
            {
                result = false;

                try
                {

                    Socket clientSocket = listener2_2.Accept();
                    var remoteIP = ((IPEndPoint)clientSocket.RemoteEndPoint);
                    localIP = ((IPEndPoint)clientSocket.LocalEndPoint);
                    strAddress = remoteIP.Address.ToString();
                    Port = remoteIP.Port;

                    int receiveNumber = clientSocket.Receive(m_byBuffRecieve2_2);
                    receiveStr = new StringBuilder();
                    receiveStr.Append(Encoding.ASCII.GetString(m_byBuffRecieve2_2, 0, receiveNumber));
                    Log("生产线2_2:" + strAddress + ":" + Port + "第一次接收数据" + receiveStr);

                    sendStr = new StringBuilder();
                    sendStr.Append(receiveStr.ToString());
                    sendStr.Append(sendStrPlace.ToString());
                    Log("生产线2_2:" + strAddress + ":" + Port + "第一次发送数据" + sendStr);
                    m_byBuffSend2_2 = Encoding.Default.GetBytes(sendStr.ToString());
                    clientSocket.Send(m_byBuffSend2_2);


                    receiveNumber = clientSocket.Receive(m_byBuffRecieve2_2);
                    receiveStr = new StringBuilder();
                    receiveStr.Append(Encoding.ASCII.GetString(m_byBuffRecieve2_2, 0, receiveNumber));
                    Log("生产线2_2:" + strAddress + ":" + Port + "第二次接收数据" + receiveStr);

                    sendStr = new StringBuilder();
                    sendStr.Append(receiveStr.ToString().Substring(0, 45));
                    sendStr.Append("OK");

                    Log("生产线2_2:" + strAddress + ":" + Port + "第二次发送数据" + sendStr);
                    m_byBuffSend2_2 = Encoding.Default.GetBytes(sendStr.ToString());
                    clientSocket.Send(m_byBuffSend2_2);


                    sendStrPlace = new StringBuilder();
                    sendStrPlace.Append(receiveStr.ToString().Substring(10, 35));


                    clientSocket.Close();
                    result = true;
                }
                catch (Exception ex)
                {
                    LogManager.Log("SocketLog", "Error", ex.Message);
                }
                if (result)
                {
                    AnalysisData2(receiveStr.ToString(), Port, strAddress);
                }
            }
        }
 
        public void AnalysisData2(string receiveStr, Int32 port, string ip)
        {
            try
            {
                string str = "";
                str = receiveStr.Substring(0, 3);//flag
                str = receiveStr.Substring(3, 7);//device id
                str = receiveStr.Substring(10, 2);//data type
                str = receiveStr.Substring(12, 5);//spool point
                str = receiveStr.Substring(17, 8);//prod date
                str = receiveStr.Substring(25, 4);//stn cd
                str = receiveStr.Substring(29, 4);//seq
                str = receiveStr.Substring(33, 12);//body no

                string strVin = receiveStr.Substring(45, 17);
                string strDate = receiveStr.Substring(62, 12)+"00";
                string strOCN = receiveStr.Substring(76, 4);
                InsertVin(strVin, strDate, strOCN, receiveStr, port, ip, "2");
                this.userLoginLog.Invoke(new Action(() =>
                {
                    this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "== 自动接收到 VIN:" + strVin + " 生产制造日期:" + strDate + " 生产OCN:" + strOCN + " ==");
                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                }));
            }
            catch (System.Exception ex)
            {
                LogManager.Log("SocketLog", "Error", ex.Message);
            }
           
        }
        public void ListenClientConnect3_1()
        {
            //判断数据库中是否有PL3字段数据
            bool bIni = false;//是否初始化过 true 初始化过 false 未初始化过
            String SND = "";//
            do
            {
                try
                {
                    SND = QuerySNDData(plname3_1);
                    //如果不为空 表示已初始化
                    if (!string.IsNullOrEmpty(SND))
                    {
                        bIni = true;
                    }
                    break;
                }
                catch (System.Exception ex)
                {

                }
            } while (true);

            while (true)
            {
                StringBuilder receiveStr = new StringBuilder();
                StringBuilder strSend = new StringBuilder("");
                IPEndPoint localIP = null;
                bool result = false;
                string strAddress = "";
                string strTest = "";
                Int32 Port = 0;
                bool bSNDChange = false;
                do
                {
                    try
                    {

                        //等待连接
                        Socket clientSocket = listener3_1.Accept();
                        var remoteIP = ((IPEndPoint)clientSocket.RemoteEndPoint);
                        localIP = ((IPEndPoint)clientSocket.LocalEndPoint);
                        strAddress = remoteIP.Address.ToString();
                        Port = remoteIP.Port;

                        //接收数据
                        int receiveNumber = clientSocket.Receive(m_byBuffRecieve3_1);
                        receiveStr = new StringBuilder();
                        receiveStr.Append(Encoding.ASCII.GetString(m_byBuffRecieve3_1, 0, receiveNumber));
                        Log("生产线3_1:"+strAddress + ":" + Port + "第一次接收数据" + receiveStr);
                        //获取设备名
                        string strDevice = receiveStr.ToString().Substring(3, 7);

                        //如果已初始化
                        if (bIni)
                        {
                            strSend = new StringBuilder("");
                            strSend.Append("REQ");
                            strSend.Append(SND);
                        }
                        else
                        {
                            strSend = new StringBuilder("");
                            strSend.Append("INI" + strDevice);
                            strSend.Append("00");
                            strSend.Append("00000");
                            strSend.Append("00000000");
                            strSend.Append("0000");
                            strSend.Append("0000");
                            strSend.Append("000000000000");
                        }
                        Log("生产线3_1:" + strAddress + ":" + Port + "第一次发送数据" + strSend);
                        //发送数据
                        m_byBuffSend3_1 = Encoding.Default.GetBytes(strSend.ToString());
                        int size = clientSocket.Send(m_byBuffSend3_1);

                        //接收数据
                        receiveNumber = clientSocket.Receive(m_byBuffRecieve3_1);
                        if (receiveNumber < 45)
                        {
                            bIni = false;
                            break;
                        }
                        receiveStr = new StringBuilder();
                        receiveStr.Append(Encoding.ASCII.GetString(m_byBuffRecieve3_1, 0, receiveNumber));
                        Log("生产线3_1:" + strAddress + ":" + Port + "第二次接收数据" + receiveStr);

                        strSend = new StringBuilder("");
                        string strCMD = receiveStr.ToString().Substring(0, 3);//flag
                        //如果是SND 或PUD 为数据 需要解析数据
                        if (strCMD == "SND" || strCMD == "PUD")
                        {
                            strSend.Append("SND");
                            SND = receiveStr.ToString().Substring(3, 42);
                            bSNDChange = true;
                        }
                        else
                        {
                            strSend.Append("REQ");
                        }

                        strSend.Append(receiveStr.ToString().Substring(3, 42));// = ;
                        strSend.Append("OK");
                        Log("生产线3_1:" + strAddress + ":" + Port + "第二次发送数据" + strSend);
                        //发送数据
                        m_byBuffSend3_1 = Encoding.Default.GetBytes(strSend.ToString());
                        size = clientSocket.Send(m_byBuffSend3_1);
                        //关闭连接
                        clientSocket.Close();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        LogManager.Log("SocketLog", "Error", ex.Message);
                    }
                    if (result)
                    {
                        if (bSNDChange)
                        {
                            AnalysisData3(receiveStr.ToString(), Port, strAddress);
                            if (SaveSNDData(plname3_1, SND))
                            {
                                bIni = true;
                            }
                        }
                    }
                } while (false);
                
            }
        }

        public void ListenClientConnect3_2()
        {
            //判断数据库中是否有PL3字段数据
            bool bIni = false;//是否初始化过 true 初始化过 false 未初始化过
            String SND = "";//
            do
            {
                try
                {
                    SND = QuerySNDData(plname3_2);
                    //如果不为空 表示已初始化
                    if (!string.IsNullOrEmpty(SND))
                    {
                        bIni = true;
                    }
                    break;
                }
                catch (System.Exception ex)
                {

                }
            } while (true);

            while (true)
            {
                StringBuilder receiveStr = new StringBuilder();
                StringBuilder strSend = new StringBuilder("");
                IPEndPoint localIP = null;
                bool result = false;
                string strAddress = "";
                string strTest = "";
                Int32 Port = 0;
                bool bSNDChange = false;
                do
                {
                    try
                    {

                        //等待连接
                        Socket clientSocket = listener3_2.Accept();
                        var remoteIP = ((IPEndPoint)clientSocket.RemoteEndPoint);
                        localIP = ((IPEndPoint)clientSocket.LocalEndPoint);
                        strAddress = remoteIP.Address.ToString();
                        Port = remoteIP.Port;

                        //接收数据
                        int receiveNumber = clientSocket.Receive(m_byBuffRecieve3_2);
                        receiveStr = new StringBuilder();
                        receiveStr.Append(Encoding.ASCII.GetString(m_byBuffRecieve3_2, 0, receiveNumber));
                        Log("生产线3_2:" + strAddress + ":" + Port + "第一次接收数据" + receiveStr);
                        //获取设备名
                        string strDevice = receiveStr.ToString().Substring(3, 7);

                        //如果已初始化
                        if (bIni)
                        {
                            strSend = new StringBuilder("");
                            strSend.Append("REQ");
                            strSend.Append(SND);
                        }
                        else
                        {
                            strSend = new StringBuilder("");
                            strSend.Append("INI" + strDevice);
                            strSend.Append("00");
                            strSend.Append("00000");
                            strSend.Append("00000000");
                            strSend.Append("0000");
                            strSend.Append("0000");
                            strSend.Append("000000000000");
                        }
                        Log("生产线3_2:" + strAddress + ":" + Port + "第一次发送数据" + strSend);
                        //发送数据
                        m_byBuffSend3_2 = Encoding.Default.GetBytes(strSend.ToString());
                        int size = clientSocket.Send(m_byBuffSend3_2);

                        //接收数据
                        receiveNumber = clientSocket.Receive(m_byBuffRecieve3_2);
                        if (receiveNumber < 45)
                        {
                            bIni = false;
                            break;
                        }
                        receiveStr = new StringBuilder();
                        receiveStr.Append(Encoding.ASCII.GetString(m_byBuffRecieve3_2, 0, receiveNumber));
                        Log("生产线3_2:" + strAddress + ":" + Port + "第二次接收数据" + receiveStr);

                        strSend = new StringBuilder("");
                        string strCMD = receiveStr.ToString().Substring(0, 3);//flag
                        //如果是SND 或PUD 为数据 需要解析数据
                        if (strCMD == "SND" || strCMD == "PUD")
                        {
                            strSend.Append("SND");
                            SND = receiveStr.ToString().Substring(3, 42);
                            bSNDChange = true;
                        }
                        else
                        {
                            strSend.Append("REQ");
                        }

                        strSend.Append(receiveStr.ToString().Substring(3, 42));// = ;
                        strSend.Append("OK");
                        Log("生产线3_2:" + strAddress + ":" + Port + "第二次发送数据" + strSend);
                        //发送数据
                        m_byBuffSend3_2 = Encoding.Default.GetBytes(strSend.ToString());
                        size = clientSocket.Send(m_byBuffSend3_2);
                        //关闭连接
                        clientSocket.Close();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        LogManager.Log("SocketLog", "Error", ex.Message);
                    }
                    if (result)
                    {
                        if (bSNDChange)
                        {
                            AnalysisData3(receiveStr.ToString(), Port, strAddress);
                            if (SaveSNDData(plname3_2, SND))
                            {
                                bIni = true;
                            }
                        }
                    }
                } while (false);

            }
        }
        public void AnalysisData3(string receiveStr, Int32 port, string ip )
        {
           
            try
            {
               
                string str = "";
                str = receiveStr.Substring(0, 3);//flag
                str = receiveStr.Substring(3, 7);//device id
                str = receiveStr.Substring(10, 2);//data type
                str = receiveStr.Substring(12, 5);//spool point
                str = receiveStr.Substring(17, 8);//prod date
                str = receiveStr.Substring(25, 4);//stn cd
                str = receiveStr.Substring(29, 4);//seq
                str = receiveStr.Substring(33, 12);//body no
                string strVin = receiveStr.Substring(45, 17);
                string strDate = receiveStr.Substring(62, 12)+"00";
                string strOCN = receiveStr.Substring(76, 4);
                InsertVin(strVin, strDate, strOCN, receiveStr, port, ip, "3");
                this.userLoginLog.Invoke(new Action(() =>
                {
                    this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "== 自动接收到 VIN:" + strVin + " 生产制造日期:" + strDate + " 生产OCN:" + strOCN + " ==");
                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                }));
            }
            catch (System.Exception ex)
            {
                LogManager.Log("SocketLog", "Error", ex.Message);
            }
            return ;
        }
        public bool InsertVin(string strVin, string strDate, string strOCN, string receiveStr, Int32 port, string ip,string baseID)
        {
            StringBuilder sb = new StringBuilder();
            string vId = "";
            try
            {
                vId = CreateApp_ID();
                using (OracleConnection con = new OracleConnection(OracleHelper.conn))
                {
                    con.Open();
                    // 创建事务
                    OracleTransaction tra = null;
                    try
                    {
                        // 开始执行事务
                        tra = con.BeginTransaction();
                        string sqlInsert = string.Format("INSERT INTO VIN_INFO(ID,VIN,SC_OCN,CLZZRQ,IPV4,PORT,MERGER_STATUS,"
                                                        + "CREATE_TIME,PRIMARY_DATA,BASEID ) VALUES ("
                                                        + "'{0}','{1}','{2}',to_date('{3}','yyyy-mm-dd,hh24:mi:ss'),'{4}','{5}','{6}',to_date('{7}','yyyy-mm-dd,hh24:mi:ss'),'{8}','{9}') ",
                                                        vId, strVin, strOCN, strDate, ip, port, "0",
                                                        DateTime.Now, receiveStr, baseID);

                        OracleHelper.ExecuteNonQuery(tra, sqlInsert, null);
                        // 提交事务
                        tra.Commit();
                    }
                    catch (Exception ex)
                    {
                        tra.Rollback();//事务回滚
                        LogManager.Log("SocketLog", "Error", ex.Message);
                        LogManager.Log("SocketLog", "VIN", "vin:" + strVin + "生产日期：" + strDate + "OCN:" + strOCN + "插入失败");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //创建ID
        protected static string CreateApp_ID()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(OracleHelper.conn))
                {
                    OracleCommand cmd = new OracleCommand("nextval", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    cmd.Parameters.Add("SEQ", OracleDbType.NVarchar2);
                    cmd.Parameters[0].Value = "VIN_SOCKET";
                    OracleParameter retValParam = cmd.Parameters.Add("SEQUENCE_ID", OracleDbType.Int32);
                    retValParam.Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    object obj = retValParam.Value;
                    string objstring = obj.ToString();
                    string rand = "PL";
                    return obj.ToString();
                    //return String.Format("V{0}{1:00000000000000000000}", rand, Convert.ToDecimal(objstring));
                    //return String.Format("V{0}{1:00000000000000000000}", rand, Convert.ToDecimal(objstring));
                }
            }
            catch (Exception ex)
            {
                LogManager.Log("SocketLog", "Error", ex.Message);
                return null;
            }
        }

        private string QuerySNDData(string plNumber)
        {
            try
            {
                string sqlStr = string.Format(@"select * from SND_DATA where PLNUMBER = '{0}'", plNumber);
                DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr, null);
                return ds.Tables[0].Rows[0][1].ToString();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            

        }
        private bool SaveSNDData(string plNumber,string strSNDData)
        {
            try
            {
                string sqlStr = string.Format(@"Update SND_DATA  set SND = '{1}' where PLNUMBER = '{0}'", plNumber, strSNDData);
                using (OracleConnection con = new OracleConnection(OracleHelper.conn))
                {
                    con.Open();
                    OracleTransaction tra = con.BeginTransaction();
                    try
                    {

                       OracleHelper.ExecuteNonQuery(tra, sqlStr,null);
                        tra.Commit();
                        return true;
                    }
                    catch (System.Exception ex)
                    {
                        tra.Rollback();
                        LogManager.Log("SocketLog", "Error", ex.Message);
                    }
                }
                return false;
            }
            catch (System.Exception ex)
            {
                LogManager.Log("SocketLog", "Error", ex.Message);
                return false;
            }
        }
        private void Log(String strMessage)
        {
            this.userLoginLog.Invoke(new Action(() =>
            {
                this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + strMessage);
                this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
            }));
            LogManager.Log("SocketLog", "LOG", strMessage);
        }
    }
}
