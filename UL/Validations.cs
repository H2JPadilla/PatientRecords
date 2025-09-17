using System;
using System.Text.RegularExpressions;

namespace UL
{
    public static class Validations
    {
        public static bool FillRequired(string text, string fieldName, out string message, out string errorField)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                message = $"{fieldName} is required.";
                errorField = fieldName;
                return false;
            }
            message = "";
            errorField = "";
            return true;
        }

        public static bool ValidatePositiveNumber(decimal? number, string fieldName, out string message, out string errorField)
        {
            if (!number.HasValue || number <= 0)
            {
                message = $"{fieldName} must be a positive number.";
                errorField = fieldName;
                return false;
            }
            message = "";
            errorField = "";
            return true;
        }

        public static bool ValidateDecimalPlaces(decimal? number, string fieldName, out string message, out string errorField)
        {
            if (number.HasValue)
            {
                string s = number.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                if (s.Contains("."))
                {
                    int decimalPlaces = s.Split('.')[1].Length;
                    if (decimalPlaces > 4)
                    {
                        message = $"{fieldName} can only have up to 4 decimal places.";
                        errorField = fieldName;
                        return false;
                    }
                }
            }
            message = "";
            errorField = "";
            return true;
        }

        public static string CleanSpaces(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }
            return Regex.Replace(text.Trim(), @"\s+", " ");
        }

        public static void Handle(Exception ex, out string errorMessage, out string errorField)
        {
            if (ex is ArgumentException)
            {
                var argEx = ex as ArgumentException;
                errorMessage = argEx.Message;
                errorField = argEx.ParamName;
            }
            else if (ex is InvalidOperationException)
            {
                errorMessage = ex.Message;
                errorField = "System";
            }
            else
            {
                errorMessage = "An unexpected error occurred. Please try again.";
                errorField = "System";
            }
        }
    }
}