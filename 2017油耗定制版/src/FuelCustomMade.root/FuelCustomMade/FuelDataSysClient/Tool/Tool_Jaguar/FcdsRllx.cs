using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelDataSysClient.Tool.Tool_Jaguar
{
    public class FcdsRllx
    {
        private string _fcds_hhdl_bsqdws;
        private string _fcds_hhdl_bsqxs;
        private string _fcds_hhdl_cddmsxzgcs;
        private string _fcds_hhdl_cddmsxzhgkxslc;
        private string _fcds_hhdl_dlxdcbnl;
        private string _fcds_hhdl_dlxdczbcdy;
        private string _fcds_hhdl_dlxdczzl;
        private string _fcds_hhdl_dlxdczznl;
        private string _fcds_hhdl_edgl;
        private string _fcds_hhdl_fdjxh;
        private string _fcds_hhdl_hhdljgxs;
        private string _fcds_hhdl_hhdlzddglb;
        private string _fcds_hhdl_jgl;
        private string _fcds_hhdl_pl;
        private string _fcds_hhdl_qddjedgl;
        private string _fcds_hhdl_qddjfznj;
        private string _fcds_hhdl_qddjlx;
        private string _fcds_hhdl_qgs;
        private string _ct_qtxx;
        private string _fcds_hhdl_sjgkrlxhl;
        private string _fcds_hhdl_sqgkrlxhl;
        private string _fcds_hhdl_xsmssdxzgn;
        private string _fcds_hhdl_zhkgco2pl;
        private string _fcds_hhdl_zhgkrlxhl;
        private string _fcds_GH_FDJSCC;

        /// <summary>
        /// 发动机生产厂
        /// </summary>
        public string FCDS_GH_FDJSCC
        {
            get { return _fcds_GH_FDJSCC; }
            set { _fcds_GH_FDJSCC = value; }
        }

        /// <summary>
        /// 变速器档位数
        /// </summary>
        public string FCDS_HHDL_BSQDWS
        {
            set { _fcds_hhdl_bsqdws = value; }
            get { return _fcds_hhdl_bsqdws; }
        }
        /// <summary>
        /// 变速器型式
        /// </summary>
        public string FCDS_HHDL_BSQXS
        {
            set { _fcds_hhdl_bsqxs = value; }
            get { return _fcds_hhdl_bsqxs; }
        }
        /// <summary>
        /// 纯电动模式下1km最高车速
        /// </summary>
        public string FCDS_HHDL_CDDMSXZGCS
        {
            set { _fcds_hhdl_cddmsxzgcs = value; }
            get { return _fcds_hhdl_cddmsxzgcs; }
        }
        /// <summary>
        /// 纯电动模式下综合工况续驶里程
        /// </summary>
        public string FCDS_HHDL_CDDMSXZHGKXSLC
        {
            set { _fcds_hhdl_cddmsxzhgkxslc = value; }
            get { return _fcds_hhdl_cddmsxzhgkxslc; }
        }
        /// <summary>
        /// 动力蓄电池组比能量
        /// </summary>
        public string FCDS_HHDL_DLXDCBNL
        {
            set { _fcds_hhdl_dlxdcbnl = value; }
            get { return _fcds_hhdl_dlxdcbnl; }
        }
        /// <summary>
        /// 动力蓄电池组标称电压
        /// </summary>
        public string FCDS_HHDL_DLXDCZBCDY
        {
            set { _fcds_hhdl_dlxdczbcdy = value; }
            get { return _fcds_hhdl_dlxdczbcdy; }
        }
        /// <summary>
        /// 动力蓄电池组种类
        /// </summary>
        public string FCDS_HHDL_DLXDCZZL
        {
            set { _fcds_hhdl_dlxdczzl = value; }
            get { return _fcds_hhdl_dlxdczzl; }
        }
        /// <summary>
        /// 动力蓄电池组总能量
        /// </summary>
        public string FCDS_HHDL_DLXDCZZNL
        {
            set { _fcds_hhdl_dlxdczznl = value; }
            get { return _fcds_hhdl_dlxdczznl; }
        }
        /// <summary>
        /// 额定功率
        /// </summary>
        public string FCDS_HHDL_EDGL
        {
            set { _fcds_hhdl_edgl = value; }
            get { return _fcds_hhdl_edgl; }
        }
        /// <summary>
        /// 发动机型号
        /// </summary>
        public string FCDS_HHDL_FDJXH
        {
            set { _fcds_hhdl_fdjxh = value; }
            get { return _fcds_hhdl_fdjxh; }
        }
        /// <summary>
        /// 混合动力结构型式
        /// </summary>
        public string FCDS_HHDL_HHDLJGXS
        {
            set { _fcds_hhdl_hhdljgxs = value; }
            get { return _fcds_hhdl_hhdljgxs; }
        }
        /// <summary>
        /// 混合动力最大电功率比
        /// </summary>
        public string FCDS_HHDL_HHDLZDDGLB
        {
            set { _fcds_hhdl_hhdlzddglb = value; }
            get { return _fcds_hhdl_hhdlzddglb; }
        }
        /// <summary>
        /// 最大净功率
        /// </summary>
        public string FCDS_HHDL_JGL
        {
            set { _fcds_hhdl_jgl = value; }
            get { return _fcds_hhdl_jgl; }
        }
        /// <summary>
        /// 排量
        /// </summary>
        public string FCDS_HHDL_PL
        {
            set { _fcds_hhdl_pl = value; }
            get { return _fcds_hhdl_pl; }
        }
        /// <summary>
        /// 驱动电机额定功率
        /// </summary>
        public string FCDS_HHDL_QDDJEDGL
        {
            set { _fcds_hhdl_qddjedgl = value; }
            get { return _fcds_hhdl_qddjedgl; }
        }
        /// <summary>
        /// 驱动电机峰值扭矩
        /// </summary>
        public string FCDS_HHDL_QDDJFZNJ
        {
            set { _fcds_hhdl_qddjfznj = value; }
            get { return _fcds_hhdl_qddjfznj; }
        }
        /// <summary>
        /// 驱动电机类型
        /// </summary>
        public string FCDS_HHDL_QDDJLX
        {
            set { _fcds_hhdl_qddjlx = value; }
            get { return _fcds_hhdl_qddjlx; }
        }
        /// <summary>
        /// 气缸数
        /// </summary>
        public string FCDS_HHDL_QGS
        {
            set { _fcds_hhdl_qgs = value; }
            get { return _fcds_hhdl_qgs; }
        }
        /// <summary>
        /// 其他信息
        /// </summary>
        public string CT_QTXX
        {
            set { _ct_qtxx = value; }
            get { return _ct_qtxx; }
        }
        /// <summary>
        /// 市郊工况燃料消耗量
        /// </summary>
        public string FCDS_HHDL_SJGKRLXHL
        {
            set { _fcds_hhdl_sjgkrlxhl = value; }
            get { return _fcds_hhdl_sjgkrlxhl; }
        }
        /// <summary>
        /// 市区工况燃料消耗量
        /// </summary>
        public string FCDS_HHDL_SQGKRLXHL
        {
            set { _fcds_hhdl_sqgkrlxhl = value; }
            get { return _fcds_hhdl_sqgkrlxhl; }
        }
        /// <summary>
        /// 是否具有行驶模式手动选择功能
        /// </summary>
        public string FCDS_HHDL_XSMSSDXZGN
        {
            set { _fcds_hhdl_xsmssdxzgn = value; }
            get { return _fcds_hhdl_xsmssdxzgn; }
        }
        /// <summary>
        /// 综合工况CO2排放量
        /// </summary>
        public string FCDS_HHDL_ZHKGCO2PL
        {
            set { _fcds_hhdl_zhkgco2pl = value; }
            get { return _fcds_hhdl_zhkgco2pl; }
        }
        /// <summary>
        /// 综合工况燃料消耗量
        /// </summary>
        public string FCDS_HHDL_ZHGKRLXHL
        {
            set { _fcds_hhdl_zhgkrlxhl = value; }
            get { return _fcds_hhdl_zhgkrlxhl; }
        }
    }
}
