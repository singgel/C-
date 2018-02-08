using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.FuelCafc
{
    public static class StaticUtil
    {

        public static string TeCafc = "TE_CAFC"; // 不计新能源的cafc
        public static string NeCafc = "NE_CAFC"; // 包含新能源的cafc

        public static string AddOp = "ADD";
        public static string EditOp = "EDIT";

        public static CafcService.CafcWebService cafcService = Utils.serviceCafc;
    }

    public enum CafcType
    {
        TeCafc,
        NeCafc
    }

    public enum OperateType
    {
        ADD,
        EDIT
    }

    public enum ParamType
    {
        Statistic,
        Detail
    }
}
