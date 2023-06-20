using System;

namespace FeCoEventos.Domain.ValueObjects
{
    public class DateTimeTools
    {
        public static bool TryParse(string dateTime)
        {
            DateTime dateOut;

            if (DateTime.TryParse(dateTime, out dateOut))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DateTime Parse(string dateTime, string hour)
        {
            //Parse Time
            DateTime date = Convert.ToDateTime(dateTime);

            if (date.Hour == 0 && date.Minute == 0 && date.Second == 0)
            {
                date = Convert.ToDateTime(date.ToShortDateString() + " " + hour);

                return date;
            }
            else
            {
                return date;
            }
        }
    }
}
