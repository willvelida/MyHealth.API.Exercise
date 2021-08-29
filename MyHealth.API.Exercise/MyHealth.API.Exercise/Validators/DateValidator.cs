using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MyHealth.API.Exercise.Validators
{
    public class DateValidator : IDateValidator
    {
        public bool IsDateValid(string date)
        {
            bool isDateValid = false;
            string pattern = "yyyy-MM-dd";
            DateTime parsedDate;

            if (DateTime.TryParseExact(date, pattern, null, DateTimeStyles.None, out parsedDate))
            {
                isDateValid = true;
            }

            return isDateValid;
        }
    }
}
