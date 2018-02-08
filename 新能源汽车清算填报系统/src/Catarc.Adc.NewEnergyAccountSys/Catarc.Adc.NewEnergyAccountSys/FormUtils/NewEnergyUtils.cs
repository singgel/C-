using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Catarc.Adc.NewEnergyAccountSys.Properties;


namespace Catarc.Adc.NewEnergyAccountSys.FormUtils
{
    public class NewEnergyUtils
    {
        public static NewEnergyWeb.INewEnergyClearingServiceService newEnergyservice = new NewEnergyWeb.INewEnergyClearingServiceService();

        // 将client端的VehicleBasicInfo array对象转换为DataTable
        public static DataTable NewEnergyInfoS2DT(NewEnergyWeb.newEnergyVehicle[] serviceBasicList)
        {
       
            DataTable dt = new DataTable();
            dt.Columns.Add("MFRS");
            dt.Columns.Add("MODEL_VEHICLE");
            dt.Columns.Add("NAME_VEHICLE");
            dt.Columns.Add("CONFIG_ID");
            dt.Columns.Add("CATALOG_LOT");
            dt.Columns.Add("LENGTH_CAR");
            dt.Columns.Add("Ekg");
            dt.Columns.Add("CONDITION_NORMAL");
            dt.Columns.Add("SPEED_NORMAL");
            dt.Columns.Add("CONDITION_ELE");
            dt.Columns.Add("SPEED_ELE");
            dt.Columns.Add("TYPE_ENERGY");
            dt.Columns.Add("MODEL_SINGLE");

            dt.Columns.Add("MFRS_SINGLE");
            dt.Columns.Add("MODEL_WHOLE");
            dt.Columns.Add("CAPACITY_BAT");
            dt.Columns.Add("MFRS_BAT");
            dt.Columns.Add("MODEL_DRIVE");
            dt.Columns.Add("RATEPOW_DRIVE");
            dt.Columns.Add("MFRS_DRIVE");
            dt.Columns.Add("MDEL_FUEL");
            dt.Columns.Add("RATEPOW_FUEL");
            dt.Columns.Add("MFRS_FUEL");
            dt.Columns.Add("TIME_RELEASE");
            dt.Columns.Add("BATCH");
            dt.Columns.Add("DATASOURCE");

            foreach (NewEnergyWeb.newEnergyVehicle serviceBasic in serviceBasicList)
            {
                DataRow dr = dt.NewRow();
                dr["MFRS"] = serviceBasic.qymc;
                dr["MODEL_VEHICLE"] = serviceBasic.cpxh;
                dr["NAME_VEHICLE"] = serviceBasic.cpmc;
                dr["CONFIG_ID"] = serviceBasic.pz_id;
                dr["CATALOG_LOT"] = serviceBasic.nerdspc;
                dr["LENGTH_CAR"] = serviceBasic.chang;
                dr["Ekg"] = serviceBasic.ekg;
                dr["CONDITION_NORMAL"] = serviceBasic.ddqcyblc;
                dr["SPEED_NORMAL"] = serviceBasic.ddqcxslc;
                dr["CONDITION_ELE"] = serviceBasic.ddqcyblc_cun;
                dr["SPEED_ELE"] = serviceBasic.ddqcxslc_cun;
                dr["TYPE_ENERGY"] = serviceBasic.dlxdczl;
                dr["MODEL_SINGLE"] = serviceBasic.dlxdcxh;
                dr["MFRS_SINGLE"] = serviceBasic.dlxdcc;
                dr["MODEL_WHOLE"] = serviceBasic.cxhcnzzxh;
                dr["CAPACITY_BAT"] = serviceBasic.xnzzzdl;
                dr["MFRS_BAT"] = serviceBasic.cnzzscqy;
                dr["MODEL_DRIVE"] = serviceBasic.ddqcqdxh;
                dr["RATEPOW_DRIVE"] = serviceBasic.ddqcqdgl;
                dr["MFRS_DRIVE"] = serviceBasic.ddqcqdc;

                dr["MDEL_FUEL"] = serviceBasic.rldcqpxh;
                dr["RATEPOW_FUEL"] = serviceBasic.rldcxtedgl;
                dr["MFRS_FUEL"] = serviceBasic.rldcqpqy;
       
                dr["TIME_RELEASE"] = Settings.Default.ClearYear;
                dr["BATCH"] = serviceBasic.ggpc;
                dr["DATASOURCE"] = serviceBasic.lb;

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable QyNameInfoS2DT(NewEnergyWeb.qynameByYear[] qynameByyearList)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MFRSName");
            
            dt.Columns.Add("ClearYear");
     
            foreach (NewEnergyWeb.qynameByYear qynameByyear in qynameByyearList)
            {
                DataRow dr = dt.NewRow();
                dr["MFRSName"] = qynameByyear.qymc;
                dr["ClearYear"] = qynameByyear.year;

                dt.Rows.Add(dr);
            }

            return dt;
        }
       

   
    }
}
