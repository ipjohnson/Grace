using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.LanguageExtensions
{
    /// <summary>
    /// Date time resolution
    /// </summary>
    public enum DateTimeResolution
    {
        /// <summary>
        /// Equal to the millisecond
        /// </summary>
        Millisecond,

        /// <summary>
        /// Equal to the second
        /// </summary>
        Second,

        /// <summary>
        /// Equal to the minute
        /// </summary>
        Minute,

        /// <summary>
        /// Equal to the hour
        /// </summary>
        Hour,

        /// <summary>
        /// Equal to the date
        /// </summary>
        Date,

        /// <summary>
        /// Equal to the same week
        /// </summary>
        Week,

        /// <summary>
        /// Equal to the month
        /// </summary>
        Month,

        /// <summary>
        /// Equal to the year
        /// </summary>
        Year,
    }

    /// <summary>
    /// C# extension methods for DateTime
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Compares two date time instances down to the millisecond
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="compareDateTime"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static bool IsEqualTo(this DateTime dateTime, DateTime compareDateTime, DateTimeResolution resolution = DateTimeResolution.Millisecond)
        {
            switch (resolution)
            {
                case DateTimeResolution.Millisecond:
                    if (dateTime.Millisecond != compareDateTime.Millisecond)
                    {
                        return false;
                    }
                    goto case DateTimeResolution.Second;

                case DateTimeResolution.Second:
                    if (dateTime.Second != compareDateTime.Second)
                    {
                        return false;
                    }
                    goto case DateTimeResolution.Minute;

                case DateTimeResolution.Minute:
                    if (dateTime.Minute != compareDateTime.Minute)
                    {
                        return false;
                    }
                    goto case DateTimeResolution.Hour;

                case DateTimeResolution.Hour:
                    if (dateTime.Hour != compareDateTime.Hour)
                    {
                        return false;
                    }
                    goto case DateTimeResolution.Date;

                case DateTimeResolution.Date:
                    if (dateTime.Date != compareDateTime.Date)
                    {
                        return false;
                    }
                    break;

                case DateTimeResolution.Month:
                    if (dateTime.Month != compareDateTime.Month)
                    {
                        return false;
                    }
                    goto case DateTimeResolution.Year;

                case DateTimeResolution.Year:
                    if (dateTime.Year != compareDateTime.Year)
                    {
                        return false;
                    }
                    break;

                case DateTimeResolution.Week:
                    if (dateTime.Year != compareDateTime.Year)
                    {
                        return false;
                    }

                    int diff = dateTime.DayOfYear - compareDateTime.DayOfYear;

                    if (diff < -6 || diff > 6)
                    {
                        return false;
                    }

                    if (diff < 0 && 6 - (int)dateTime.DayOfWeek > Math.Abs(diff))
                    {
                        return false;
                    }

                    if (diff > 0 && diff > 6 - (int)compareDateTime.DayOfWeek)
                    {
                        return false;
                    }

                    break;
            }

            return true;
        }

        /// <summary>
        /// Compares two date time 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="compareDateTime"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static bool IsEqualTo(this DateTime dateTime, DateTime? compareDateTime, DateTimeResolution resolution = DateTimeResolution.Millisecond)
        {
            return compareDateTime.HasValue && IsEqualTo(dateTime, compareDateTime.Value, resolution);
        }

        /// <summary>
        /// Compare two date times
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="compareDateTime"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static bool IsEqualTo(this DateTime? dateTime, DateTime compareDateTime, DateTimeResolution resolution = DateTimeResolution.Millisecond)
        {
            return dateTime.HasValue && IsEqualTo(dateTime.Value, compareDateTime);
        }

        /// <summary>
        /// Compare two nullable datetime to a particular resolution
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="compareDateTime"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static bool IsEqualTo(this DateTime? dateTime,
            DateTime? compareDateTime,
            DateTimeResolution resolution = DateTimeResolution.Millisecond)
        {
            if (!dateTime.HasValue && !compareDateTime.HasValue)
            {
                return true;
            }


            if (dateTime.HasValue && compareDateTime.HasValue)
            {
                return IsEqualTo(dateTime.Value, compareDateTime.Value, resolution);
            }

            return false;
        }
    }
}
