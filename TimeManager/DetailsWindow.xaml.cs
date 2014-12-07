using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Excel = Microsoft.Office.Interop.Excel;

namespace TimeManager
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        public DetailsWindow()
        {
            InitializeComponent();
        }

        private void CreateExcelFile(string employeeId)
        {
            Excel.Application excelApp;
            Excel.Workbook workbook;
            Excel.Worksheet worksheet;
            object misValue = System.Reflection.Missing.Value;

            excelApp = new Excel.ApplicationClass();
            workbook = excelApp.Workbooks.Add(misValue);

            worksheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);
            worksheet.Name = "Report";
            worksheet.Cells[1, 1] = "Employee ID";
            worksheet.Cells[1, 2] = "Date";
            worksheet.Cells[1, 3] = "Swipe In Time";
            worksheet.Cells[1, 4] = "Swipe Out Time";
            worksheet.Cells[1, 5] = "Total Office Time";
            worksheet.Cells[1, 6] = "Total ODC In Time";
            ((Excel.Range)worksheet.Cells[1, 1]).EntireColumn.ColumnWidth = 15;
            ((Excel.Range)worksheet.Cells[1, 2]).EntireColumn.ColumnWidth = 15;
            ((Excel.Range)worksheet.Cells[1, 3]).EntireColumn.ColumnWidth = 15;
            ((Excel.Range)worksheet.Cells[1, 4]).EntireColumn.ColumnWidth = 15;
            ((Excel.Range)worksheet.Cells[1, 5]).EntireColumn.ColumnWidth = 15;
            ((Excel.Range)worksheet.Cells[1, 6]).EntireColumn.ColumnWidth = 17;
            System.IO.Directory.CreateDirectory("C:\\TimeManager");
            workbook.SaveAs("C:\\TimeManager\\" + employeeId + "_Report.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            workbook.Close(true, misValue, misValue);
            excelApp.Quit();
            Marshal.ReleaseComObject(worksheet);
            Marshal.ReleaseComObject(workbook);
            var mainWindow = new TimeManager.TimeManagerWindow();
            App.Current.Properties["EmployeeId"] = employeeId;
            mainWindow.txtEmployeeId.Text = employeeId;
            Application.Current.Windows[0].Close();
            mainWindow.Show();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmployeeId.Text))
            {
                MessageBox.Show("Please enter your Employee ID");
                txtEmployeeId.SetValue(Border.BorderBrushProperty, Brushes.Red);
            }
            else
            {
                CreateExcelFile(txtEmployeeId.Text);
            }
        }

        private void txtEmployeeId_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtEmployeeId.ClearValue(TextBox.BorderBrushProperty);
        }
    }
}