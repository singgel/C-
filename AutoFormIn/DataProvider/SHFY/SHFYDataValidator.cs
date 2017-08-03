using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.DataProviders.SHFY
{
    public class SHFYDataValidator : DataValidator
    {
        private Hashtable rules;

        private class ValidateExpression
        {
            public string Expression
            {
                get;
                set;
            }

            public bool IsRequired
            {
                get;
                set;
            }
        }

        public SHFYDataValidator(string validateFile)
            :base(validateFile)
        {
            rules = new Hashtable();
        }

        public override bool IsValid(string key, string value)
        {
            ValidateExpression expr = rules[key] as ValidateExpression;
            if (expr == null)
                return true;
            else if (string.IsNullOrEmpty(value))
                return !expr.IsRequired;
            else if (string.IsNullOrEmpty(expr.Expression) || value.Trim() == "N/A")
                return true;
            return System.Text.RegularExpressions.Regex.IsMatch(value, expr.Expression);
        }

        public override bool IsSpecial(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            ValidateExpression expr = rules[key] as ValidateExpression;
            if (expr != null && expr.Expression == "-1" && value.Trim() != "N/A")
                return true;
            return false;
        }

        public override void ReadValidateRule()
        {
            Hashtable columnHeader = new Hashtable();
            Hashtable fillParameters = new Hashtable();
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(base.ValidateFileName))
            {
                reader.Open();
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate("填报规则") as Office.Excel.ForwardReadWorksheet;
                if (sheet != null)
                {
                    object header = null;
                    if (sheet.ReadNextRow() && sheet.CurrentRowIndex == 1)
                    {
                        while (sheet.ReadNextCell(false))
                        {
                            header = sheet.GetContent();
                            columnHeader.Add(sheet.CurrentCell.ColumnIndex, header == null ? "" : header.ToString());
                        }
                    }
                    object content = null;
                    string key = "", str = "";
                    while (sheet.ReadNextRow())
                    {
                        ValidateExpression expr = new ValidateExpression();
                        while (sheet.ReadNextCell(false))
                        {
                            content = sheet.GetContent();
                            str = content == null ? "" : content.ToString();
                            switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                            {
                                case "参数编号":
                                    key = str;
                                    break;
                                case "校验规则":
                                    expr.Expression = str;
                                    break;
                                case "是否必填":
                                    expr.IsRequired = (str == "是");
                                    break;
                            }
                        }
                        if (string.IsNullOrEmpty(key) == false)
                        {
                            rules.Add(key, expr);
                        }
                    }
                }
            }
        }
    }
}
