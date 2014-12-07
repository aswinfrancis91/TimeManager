using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace TimeManager
{
    public class ExcelData
    {
        public DataView Data
        {
             get
            {
                string employeeId = App.Current.Properties["EmployeeId"].ToString();
                string excelPath = "C:\\TimeManager\\" + employeeId + "_Report.xls";
                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook;
                Excel.Worksheet worksheet;
                Excel.Range range;
                workbook = excelApp.Workbooks.Open(excelPath);
                worksheet = (Excel.Worksheet)workbook.Sheets["Report"];

                int column = 0;
                int row = 0;

                range = worksheet.UsedRange;
                DataTable dt = new DataTable();
                dt.Columns.Add("Employee ID");
                dt.Columns.Add("Date");
                dt.Columns.Add("Swipe In Time");
                dt.Columns.Add("Swipe Out Time");
                dt.Columns.Add("Total Office Time");
                dt.Columns.Add("Total ODC In Time");
                for (row = 2; row <= range.Rows.Count; row++)
                {
                    DataRow dr = dt.NewRow();
                    for (column = 1; column < 7; column++)
                    {
                        dr[column - 1] = Convert.ToString((range.Cells[row, column] as Excel.Range).Text);
                    }
                    dt.Rows.Add(dr);
                    dt.AcceptChanges();
                }
                workbook.Close(true, Missing.Value, Missing.Value);
                excelApp.Quit();
                Marshal.ReleaseComObject(worksheet);
                Marshal.ReleaseComObject(workbook);
                return dt.DefaultView;
            }
        }
    }
}
