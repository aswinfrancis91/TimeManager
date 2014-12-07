using System;

namespace TimeManagerBusiness
{
    public class TimeManagerBusiness
    {
        private TimeManagerDataAccess.TimeManagerDataAccess _timeManagerDAL;

        public TimeManagerBusiness()
        {
            _timeManagerDAL = new TimeManagerDataAccess.TimeManagerDataAccess();
        }

        public void SaveReport(string swipeInTime, string swipeOutTime, string odcTime, string employeeId)
        {
            TimeSpan officeTime = Convert.ToDateTime(swipeOutTime) - Convert.ToDateTime(swipeInTime);
            _timeManagerDAL.SaveReport(swipeInTime, swipeOutTime, officeTime, odcTime, employeeId);
        }
    }
}