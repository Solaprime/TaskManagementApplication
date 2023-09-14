using System;
using System.Collections.Generic;
using System.Text;

namespace AppShared.Helpers
{
    public static class DateTimeOffsetExtensions
    {
        //numberofDays = 7;
        public static bool IsDueSoon(this DateTimeOffset dueDate, int numberOfDays)
        {
            DateTimeOffset today = DateTimeOffset.Now;
            DateTimeOffset oneWeekFromToday = today.AddDays(numberOfDays);
            return dueDate < oneWeekFromToday;
        }
    }
}



