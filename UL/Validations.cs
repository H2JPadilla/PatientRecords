using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UL
{
    public class Validations
    {
        public static void Handle(Exception ex, out string message, out string errorField)
        {
            message = "Unexpected error.";
            errorField = "System";

            if (ex is InvalidCastException)
            {
                message = MessageUtil.CastException; // e.g. "Invalid type conversion."
            }
            else if (ex is ArgumentOutOfRangeException)
            {
                message = MessageUtil.ArgumentOutOfRange; // e.g. "Value is out of range."
            }
            else if (ex is IndexOutOfRangeException)
            {
                message = MessageUtil.IndexOutOfRange; // e.g. "Index is out of range."
            }
            else if (ex is ArgumentNullException)
            {
                message = MessageUtil.NullException; // e.g. "null value detected."
            }
            else
            {
                message = MessageUtil.generalexception; // e.g. "An unexpected error occurred."
            }
        }

        // Method to clean up extra spaces in a string
        public static string CleanSpaces(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return string.Join(" ",
                input.Trim()
                     .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }


        //Check if a required field is filled
        public static bool FillRequired(string value, string fieldName, out string message, out string errorField)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                message = MessageUtil.RequiredField; // or $"The {fieldName} field is required."
                errorField = fieldName;
                return false;
            }

            message = string.Empty;
            errorField = string.Empty;
            return true;
        }

        // ✅ Positive integer check
        public static bool ValidatePositiveNumber(decimal value, string fieldName, out string message, out string errorField)
        {
            if (value <= 0)
            {
                message = MessageUtil.NonInteger; // or $"The {fieldName} must be greater than zero."
                errorField = fieldName;
                return false;
            }

            message = string.Empty;
            errorField = string.Empty;
            return true;
        }
    }
}
