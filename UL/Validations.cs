using System;
using System.Text.RegularExpressions;
namespace UL
{
    public static class Validations
    {
        public static string RemoveAllWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return Regex.Replace(input, @"\s+", "");
        }

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
            if (ex is ArgumentNullException argNullEx)
            {
                errorMessage = "A required argument was null: " + argNullEx.Message;
                errorField = argNullEx.ParamName ?? "Unknown";
            }
            else if (ex is ArgumentOutOfRangeException argRangeEx)
            {
                errorMessage = "Argument out of range: " + argRangeEx.Message;
                errorField = argRangeEx.ParamName ?? "Unknown";
            }
            else if (ex is ArgumentException argEx)
            {
                errorMessage = "Invalid argument: " + argEx.Message;
                errorField = argEx.ParamName ?? "Unknown";
            }
            else if (ex is FormatException)
            {
                errorMessage = "Invalid format: " + ex.Message;
                errorField = "Input";
            }
            else if (ex is InvalidCastException)
            {
                errorMessage = "Invalid type conversion: " + ex.Message;
                errorField = "System";
            }
            else if (ex is InvalidOperationException)
            {
                errorMessage = "Invalid operation: " + ex.Message;
                errorField = "System";
            }
            else if (ex is NullReferenceException)
            {
                errorMessage = "Object reference not set to an instance of an object.";
                errorField = "System";
            }
            else if (ex is IndexOutOfRangeException)
            {
                errorMessage = "Index out of range: " + ex.Message;
                errorField = "Collection";
            }
            
            else if (ex is OverflowException)
            {
                errorMessage = "Numeric overflow occurred.";
                errorField = "Math";
            }
            else
            {
                errorMessage = "An unexpected error occurred. Please try again.";
                errorField = "System";
            }
        }
    }
}