using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelDataSysClient.Tool.Tool_JBLH
{
    public class CtnyRllx
    {
        // 发动机型号
        private string fdjxh;

        public string Fdjxh
        {
            get { return fdjxh; }
            set { fdjxh = value; }
        }

        // 发动机气缸数目
        private string qgs;

        public string Qgs
        {
            get { return qgs; }
            set { qgs = value; }
        }

        // 发动机排量
        private string pl;

        public string Pl
        {
            get { return pl; }
            set { pl = value; }
        }

        // 发动机功率
        private string edgl;

        public string Edgl
        {
            get { return edgl; }
            set { edgl = value; }
        }

        // 发动机净功率
        private string jgl;

        public string Jgl
        {
            get { return jgl; }
            set { jgl = value; }
        }

        // 变速器型式
        private string bsqxs;

        public string Bsqxs
        {
            get { return bsqxs; }
            set { bsqxs = value; }
        }

        // 变速器档位数
        private string bsqdws;

        public string Bsqdws
        {
            get { return bsqdws; }
            set { bsqdws = value; }
        }

        // 汽车节能技术
        private string qcjnjs;

        public string Qcjnjs
        {
            get { return qcjnjs; }
            set { qcjnjs = value; }
        }

        // 其他信息
        private string qtxx;

        public string Qtxx
        {
            get { return qtxx; }
            set { qtxx = value; }
        }

        // 燃料消耗量（市区）
        private string sqgkrlxhl;

        public string Sqgkrlxhl
        {
            get { return sqgkrlxhl; }
            set { sqgkrlxhl = value; }
        }

        // 燃料消耗量（市郊）
        private string sjgkrlxhl;

        public string Sjgkrlxhl
        {
            get { return sjgkrlxhl; }
            set { sjgkrlxhl = value; }
        }

        // 燃料消耗量（综合）
        private string zhgkrlxhl;

        public string Zhgkrlxhl
        {
            get { return zhgkrlxhl; }
            set { zhgkrlxhl = value; }
        }

        // CO2排放量（综合）
        private string zhgkco2pfl;

        public string Zhgkco2pfl
        {
            get { return zhgkco2pfl; }
            set { zhgkco2pfl = value; }
        }

        // 发动机生产厂--国环申报用
        private string gh_Fdjscc;

        public string Gh_Fdjscc
        {
            get { return gh_Fdjscc; }
            set { gh_Fdjscc = value; }
        }
    }
}
