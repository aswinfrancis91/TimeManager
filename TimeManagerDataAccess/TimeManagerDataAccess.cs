using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace TimeManagerDataAccess
{
    public class TimeManagerDataAccess
    {
        private static Excel.Workbook workbook = null;
        private static Excel.Application excelApp = null;
        private static Excel.Worksheet worksheet = null;

        public void SaveReport(string swipeInTime, string swipeOutTime, TimeSpan officeTime, string odcTime, string employeeId)
        {
            try
            {
                string excelPath = "C:\\TimeManager\\" + employeeId + "_Report.xls";
                excelApp = new Excel.Application();
                excelApp.Visible = false;
                workbook = excelApp.Workbooks.Open(excelPath);
                worksheet = (Excel.Worksheet)workbook.Sheets[1]; // Explicit cast is not required here
                long lastRow = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;
                lastRow += 1;
                worksheet.Cells[lastRow, 1] = employeeId;
                worksheet.Cells[lastRow, 2] = DateTime.Now.ToString("dd/MM/yyyy");
                worksheet.Cells[lastRow, 3] = swipeInTime;
                worksheet.Cells[lastRow, 4] = swipeOutTime;
                worksheet.Cells[lastRow, 5] = officeTime.ToString();
                worksheet.Cells[lastRow, 6] = odcTime;
                workbook.Save();
                workbook.Close();
                excelApp.Quit();
                Marshal.ReleaseComObject(worksheet);
                Marshal.ReleaseComObject(workbook);
            }
            catch (Exception ex)
            {
                workbook.Save();
                workbook.Close();
                excelApp.Quit();
                Marshal.ReleaseComObject(worksheet);
                Marshal.ReleaseComObject(workbook);
                MessageBox.Show(ex.ToString());
            }
        }
    }
}