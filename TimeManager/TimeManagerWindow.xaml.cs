using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Data;

namespace TimeManager
{
    /// <summary>
    /// Interaction logic for TimeManagerWindow.xaml
    /// </summary>
    public partial class TimeManagerWindow : Window
    {
        private DispatcherTimer swipeInTimer = new DispatcherTimer();
        private DispatcherTimer odcCounter = new DispatcherTimer();
        private Stopwatch odcTimer = new Stopwatch();
        private string currentTime = string.Empty;
        private TimeManagerBusiness.TimeManagerBusiness timeManagerBL;
        private DispatcherTimer taskCounter = new DispatcherTimer();
        private Stopwatch taskTimer = new Stopwatch();
        private static int rowCount = -1;
        public TimeManagerWindow()
        {
            InitializeComponent();
            CheckIfFileExist();
            //Start the clock
            swipeInTimer.Interval = TimeSpan.FromSeconds(1);
            swipeInTimer.Tick += (sender, e) => timer_Tick(swipeInTimer, e, lblTimer);
            swipeInTimer.Start();
            btnStart.IsEnabled = false;
            btnPause.IsEnabled = false;
            btnSwipeOut.IsEnabled = false;
            //For stop watch
            odcCounter.Tick += new EventHandler(dt_Tick);
            odcCounter.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timeManagerBL = new TimeManagerBusiness.TimeManagerBusiness();
            btnSaveReport.IsEnabled = false;
        }

        private void CheckIfFileExist()
        {
            string excelPath = "C:\\TimeManager";
            if (!Directory.Exists(excelPath))
            {
                string message = "It seems you are running the application for the first time.Click OK to create an excel file.";
                if (MessageBox.Show(message, "Welcome aboard!", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    var newWindow = new TimeManager.DetailsWindow();
                    Application.Current.Windows[0].Close();
                    newWindow.Show();
                }
            }
            else
            {
                string message = string.Empty;
                string[] files = Directory.GetFiles(excelPath);
                if (files.Length == 1)
                {
                    if (System.IO.Path.GetExtension(files[0]) != ".xls")
                    {
                        message = "It seems you are running the application for the first time.Click OK to create an excel file.";
                        if (MessageBox.Show(message, "Welcome aboard!", MessageBoxButton.OK) == MessageBoxResult.OK)
                        {
                            var newWindow = new TimeManager.DetailsWindow();
                            Application.Current.Windows[0].Close();
                            newWindow.Show();
                        }
                    }
                    else
                    {
                        txtEmployeeId.Text = System.IO.Path.GetFileName(files[0]).Split('_')[0];
                        App.Current.Properties["EmployeeId"] = txtEmployeeId.Text;
                    }
                }
                else if (files.Length > 1)
                {
                    message = "There seems to be more than one excel file in C:\\TimeManager.Please remove everything except yours\n If there is no excel file with your employee ID then remove all the excel files and run the application again\nApplciation will close now";
                    if (MessageBox.Show(message, "Oops!", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    message = "It seems you are running the application for the first time.Click OK to create an excel file.";
                    if (MessageBox.Show(message, "Welcome aboard!", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        var newWindow = new TimeManager.DetailsWindow();
                        Application.Current.Windows[0].Close();
                        newWindow.Show();
                    }
                }
            }
        }

        //Method saves the time we swiped in and as well as start the stop watch for ODC intime
        private void btnSwipeIn_Click(object sender, RoutedEventArgs e)
        {
            txtSwipeIn.Text = DateTime.Now.ToString("hh:mm:ss tt");
            odcTimer.Start();
            odcCounter.Start();
            btnSwipeIn.IsEnabled = false;
            btnSwipeOut.IsEnabled = true;
            btnPause.IsEnabled = true;
        }

        //Method saves the swipe put time by calculating current time and adding 5mins assuming it takes 5 minutes to actually get out of office
        private void btnSwipeOut_Click(object sender, RoutedEventArgs e)
        {
            string message = "Are you sure you are going to leave office?";
            if (MessageBox.Show(message, "Alert", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                txtSwipeOut.Text = DateTime.Now.AddMinutes(15).ToString("hh:mm:ss tt");
                if (odcTimer.IsRunning)
                {
                    //OdcTimer.Stop();
                    btnStart.IsEnabled = false;
                    btnPause.IsEnabled = false;
                }
                txtTotalOdcTime.Text = Convert.ToDateTime(currentTime).AddMinutes(15).ToString("HH:mm:ss");
                btnSwipeIn.IsEnabled = false;
                btnSaveReport.IsEnabled = true;
            }
        }

        //Method to Resume the stop watch
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            odcTimer.Start();
            odcCounter.Start();
            btnStart.IsEnabled = false;
            btnPause.IsEnabled = true;
        }

        //Method to pause the stop watch
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (odcTimer.IsRunning)
            {
                odcTimer.Stop();
                btnStart.IsEnabled = true;
                btnPause.IsEnabled = false;
            }
        }

        //Assigns time to the timer
        private void timer_Tick(object sender, EventArgs e, Label labelName)
        {
            labelName.Content = DateTime.Now.ToLongTimeString();
        }

        //For stop watch
        private void dt_Tick(object sender, EventArgs e)
        {
            if (odcTimer.IsRunning)
            {
                TimeSpan ts = odcTimer.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                txtOdcCounter.Text = currentTime;
            }
        }

        private void btnSaveReport_Click(object sender, RoutedEventArgs e)
        {
            string validationMessage = ValidateField(txtSwipeIn.Text, txtSwipeOut.Text, txtTotalOdcTime.Text);
            string validateTime = ValidateTime(txtSwipeIn.Text, txtSwipeOut.Text, txtTotalOdcTime.Text);

            if (string.IsNullOrEmpty(validationMessage))
            {
                if (string.IsNullOrEmpty(validateTime))
                {
                    timeManagerBL.SaveReport(txtSwipeIn.Text, txtSwipeOut.Text, txtTotalOdcTime.Text, txtEmployeeId.Text);
                    AfterSave();
                }
                else
                {
                    validateTime += "So do you want to wait for some more time?";
                    if (MessageBox.Show(validateTime, "What's the hurry?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        txtTotalOdcTime.Text = "";
                        txtSwipeOut.Text = "";
                        btnPause.IsEnabled = true;
                        btnSaveReport.IsEnabled = false;
                    }
                    else
                    {
                        timeManagerBL.SaveReport(txtSwipeIn.Text, txtSwipeOut.Text, txtTotalOdcTime.Text, txtEmployeeId.Text);
                    }
                }
            }
            else
            {
                MessageBox.Show(validationMessage, "Are you blind?");
            }
        }

        private string ValidateTime(string swipeIn, string swipeOut, string totalOdcTime)
        {
            string validationMessage = string.Empty;
            if (!string.IsNullOrEmpty(swipeIn) && !string.IsNullOrEmpty(swipeOut))
            {
                TimeSpan timeDifference = Convert.ToDateTime(swipeOut) - Convert.ToDateTime(swipeIn);
                if (timeDifference.Hours < 8)
                {
                    validationMessage += "You definitely wouldn't make the best employee of the month sitting less than 8 hours in office\n";
                }
                else if (!string.IsNullOrEmpty(totalOdcTime))
                {
                    if (Convert.ToDateTime(totalOdcTime).Hour < 8)
                    {
                        validationMessage += "You need to be in ODC for 8 hours\n";
                        txtTotalOdcTime.SetValue(Border.BorderBrushProperty, Brushes.Red);
                    }
                }
            }
            return validationMessage;
        }

        private string ValidateField(string swipeIn, string swipeOut, string totalOdcTime)
        {
            string validationMessage = string.Empty;
            if (string.IsNullOrEmpty(swipeIn))
            {
                validationMessage += "You should have swiped in as soon as you got in boo hoo\n";
                txtSwipeIn.SetValue(Border.BorderBrushProperty, Brushes.Red);
            }
            if (string.IsNullOrEmpty(swipeOut))
            {
                validationMessage += "Even peoplesoft cannot create a report while your still in office.SWIPE OUT!!!\n";
                txtSwipeOut.SetValue(Border.BorderBrushProperty, Brushes.Red);
            }
            if (string.IsNullOrEmpty(totalOdcTime))
            {
                validationMessage += "Stop the ODC timer if you want to update the pretty excel\n";
                txtTotalOdcTime.SetValue(Border.BorderBrushProperty, Brushes.Red);
            }

            return validationMessage;
        }

        private void txtSwipeIn_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtSwipeIn.ClearValue(TextBox.BorderBrushProperty);
        }

        private void txtSwipeOut_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtSwipeOut.ClearValue(TextBox.BorderBrushProperty);
        }

        private void txtTotalOdcTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtTotalOdcTime.ClearValue(TextBox.BorderBrushProperty);
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabReport.IsSelected == true)
            {
                ExcelData exceldata = new ExcelData();
                this.dataGrid1.DataContext = exceldata;
            }
            if (tabTasks.IsSelected == true)
            {

            }
        }

        private void AfterSave()
        {
            tabReport.IsSelected = true;
            btnStart.IsEnabled = false;
            btnPause.IsEnabled = false;
            btnSwipeIn.IsEnabled = false;
            btnSwipeOut.IsEnabled = false;
            btnSaveReport.IsEnabled = false;
        }

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            rowCount += 1;
            string labelName = "lblTaskName_" + rowCount;
            string textBlockname = "txtTaskCounter_" + rowCount;
            Label lblTaskName = new Label();
            lblTaskName.Name = labelName;
            lblTaskName.Content = txtTask.Text;
            TextBlock txtTaskCounter = new TextBlock();
            txtTaskCounter.Name = textBlockname;
            taskCounter.Tick += (senderNew, eventNew) => dt_TaskTick(btnAddTask, e, txtTaskCounter);
            taskCounter.Interval = new TimeSpan(0, 0, 0, 0, 1);
            taskTimer.Start();
            taskCounter.Start();
            Grid.SetRow(lblTaskName, rowCount);
            Grid.SetColumn(lblTaskName, 0);
            Grid.SetRow(txtTaskCounter, rowCount);
            Grid.SetColumn(txtTaskCounter, 1);
            TaskGridPanel.Children.Add(lblTaskName;
            TaskGridPanel.Children.Add(txtTaskCounter);
        }
        private void dt_TaskTick(object sender, EventArgs e, TextBlock txtTaskCounter)
        {
            if (taskTimer.IsRunning)
            {
                TimeSpan ts = taskTimer.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                txtTaskCounter.Text = currentTime;
            }
        }
        private void btnSartTask(object sender, RoutedEventArgs e)
        {

        }

        private void btnPauseTask(object sender, RoutedEventArgs e)
        {
        }

        private void btnStopTask(object sender, RoutedEventArgs e)
        {
        }
    }
}