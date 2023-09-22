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
            // "2023-9-24T00:00:00"
            //19th September 2023
            DateTimeOffset today = DateTimeOffset.Now;
            DateTimeOffset oneWeekFromToday = today.AddDays(numberOfDays);
            if (dueDate < oneWeekFromToday && dueDate >= today)
            {
                return true;
            }
            return false;
          //  return dueDate < oneWeekFromToday;
        }
        //Due date 29th 9 2023
        //today 19th 9 2023     26
        public static bool IsDueSoon2(this DateTimeOffset dueDate, int numberOfDays)
        {
            DateTimeOffset today = DateTimeOffset.Now;
            DateTimeOffset oneWeekFromToday = today.AddDays(numberOfDays);
            return dueDate < oneWeekFromToday;
        }
    }
}



