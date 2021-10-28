using System;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dateOfB)
        {
            DateTime today = DateTime.Now;
            var age = today.Year - dateOfB.Year;
            if(dateOfB.Date>today.AddYears(-age)) age--;
            return age;
        }
    }
}