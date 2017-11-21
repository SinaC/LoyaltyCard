using System;

namespace LoyaltyCard.Common.Extensions
{
    public static class DateTimeExtensions
    {
        //https://stackoverflow.com/questions/2553663/how-to-determine-if-birthday-or-anniversary-occurred-during-date-range
        public static bool IsBirthdayInRange(this DateTime birthday, DateTime min, DateTime max)
        {
            var dates = new[]
            {
                birthday, min
            };
            for (int i = 0; i < dates.Length; i++)
                if (dates[i].Month == 2 && dates[i].Day == 29)
                    dates[i] = dates[i].AddDays(-1);

            birthday = dates[0];
            min = dates[1];

            DateTime nLower = new DateTime(min.Year, birthday.Month, birthday.Day);
            DateTime nUpper = new DateTime(max.Year, birthday.Month, birthday.Day);

            if (birthday.Year <= max.Year &&
                (nLower >= min && nLower <= max || nUpper >= min && nUpper <= max))
                return true;
            return false;
        }
    }
}
