using System;
using System.Collections.Generic;
using System.Text;

namespace MyHealth.API.Exercise.Validators
{
    public interface IDateValidator
    {
        bool IsDateValid(string date);
    }
}
