using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelUtils
{
    class Program
    {
        public static void main(String[] args) {

        
        
        
        }


        public void getGMDataExcel() {
            ExcelInstance data = ExcelUtil.readExcel(@"D:\柔性参数填报系统\上海通用\data.xls");
            
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.dataTables[0].Rows.Count; i++)
            {
                for (int j = 0; j < data.dataTables[0].Columns.Count; j++) {
                    sb.AppendLine(ExcelUtil.reader.getCellValue(i, j, data.dataTables[0]));
                
                
                
                }

            }
            Console.WriteLine(sb.ToString());
            Console.ReadLine();
        
        
        }
    }
}
