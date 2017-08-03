using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;

using WebBrowserUtils.HtmlUtils.Setters;
using WebBrowserUtils.HtmlUtils.Actions;
using System.Data;
using Assistant.Container;
using ExcelUtils;
using Assistant.Entity;
using Assistant.ConfigCodeService;
using Assistant.Service;

namespace Assistant
{
    class ImportAction
    {



        /// <summary>
        /// 导入rule和data
        /// </summary>
        /// <param name="ruleFilePath"></param>
        /// <param name="dataFilePath"></param>
        /// <returns>package code的列表</returns>
        public List<String> importDataAndRules(String ruleFilePath, String dataFilePath) {
            ParamsCollection.carParams.Clear();
            ParamsCollection.dicRules.Clear();
            ImportServiceForHCBM.importRuleData(ruleFilePath);
            ImportServiceForHCBM.importCarsParams(dataFilePath);
            StringBuilder sb = new StringBuilder();
            return getPackageCodeFromListCarParams(ParamsCollection.carParams);
        }

        //从参数合集中获取全部packageCode
        private List<String> getPackageCodeFromListCarParams(List<CarParams> list) {
            List<String> result = new List<String>();
            foreach (CarParams cp in list) {
                if (!String.IsNullOrEmpty(cp.packageCode)) {
                    result.Add(cp.packageCode);
                }
            }
            return result;
        }

        public String deleteUnselectedCarParams(List<String> list) {
            for (int i = 0; i < ParamsCollection.carParams.Count; i++) {
                String packagecode = ParamsCollection.carParams[i].packageCode;
                if (String.IsNullOrEmpty(packagecode) || !list.Contains(packagecode)) {
                    ParamsCollection.carParams.RemoveAt(i);
                    i--;
                }
            }
            return Constants.SUCCESS;
        }
    }
}