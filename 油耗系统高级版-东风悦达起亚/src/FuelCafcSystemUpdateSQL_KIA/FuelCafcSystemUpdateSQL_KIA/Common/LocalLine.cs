using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{

    public class SelectLine
    {

        //正式标准线路
        //public static string FormalStandardLine = "http://tsoap.catarc.info/FuelDataSysWebService.asmx?wsdl";
        public static string FormalStandardLine = "http://soap.catarc.info/FuelDataSysWebService.asmx?wsdl";
        //正式油耗核算线路
        public static string FormalCafcLine = "http://soap.catarc.info/CafcWebService.asmx?wsdl";
        //正式合格证线路
        public static string FormalCertificatedLine = "http://soap.catarc.info/CertificateComparison.asmx?wsdl";
        //正式文件线路
        public static string FormalFielLine = "http://soap.catarc.info/FileUploadService.asmx?wsdl";

        //测试标准线路
        public static string TestStandardLine = "http://tsoap.catarc.info/FuelDataSysWebService.asmx?wsdl";
        //测试油耗核算线路
        public static string TestCafcLine = "http://tsoap.catarc.info/CafcWebService.asmx?wsdl";
        //测试合格证线路
        public static string TestCertificatedLine = "http://tsoap.catarc.info/CertificateComparison.asmx?wsdl";
        //测试文件线路
        public static string TestFielLine = "http://tsoap.catarc.info/FileUploadService.asmx?wsdl";
    }

    
}
