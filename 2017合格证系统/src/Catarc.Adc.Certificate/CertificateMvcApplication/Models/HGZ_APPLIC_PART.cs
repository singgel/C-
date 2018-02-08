using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CertificateMvcApplication.Models
{
    /// <summary>
    /// 页面展示部分MODEL
    /// </summary>
    public class HGZ_APPLIC_PART
    {
        //"H_ID", "CJH", "WZHGZBH", "FZRQ", "CLZZQYMC", "CLLX", "CLMC", "CLPP", "CLXH", "DPXH", "DPHGZBH", "FDJXH"

        public virtual string H_ID { get; set; }
        public virtual string WZHGZBH { get; set; }
        public virtual string DPHGZBH { get; set; }
        public virtual string FZRQ { get; set; }
        public virtual string CLZZQYMC { get; set; }
        public virtual string CLLX { get; set; }
        public virtual string CLMC { get; set; }
        public virtual string CLPP { get; set; }
        public virtual string CLXH { get; set; }
        public virtual string DPXH { get; set; }
        public virtual string CJH { get; set; }
        public virtual string FDJXH { get; set; }

    }
}