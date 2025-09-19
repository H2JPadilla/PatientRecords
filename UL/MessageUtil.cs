using System;

namespace UL
{
    public class MessageUtil
    {
        //FRONT MESSAGE
        public const string AddSuccess = "Record has been added successfully!";
        public const string UpdateSuccess = "Record has been updated successfully!";
        public const string DeleteSuccess = "Record has been deleted successfully!";
        public const string RequiredField = "This field is required.";
        public const string NonInteger = "Dosage must be a positive number";
        public const string LimitRange = "Dosage must be between 1 - 999.9999";

        public const string RecordNotFound = "Record not found.";

        //BACK END MESSAGE
        public const string FailedToAdd = "Failed to add data";
        public const string FailedToUpdate = "Failed to update data";
        public const string FailedToDelete = "Failed to delete data";
        public const string ExistingRecord = "Record already exists!";
        public const string ExistingDrug = "This patient can already take this medicine once per day";

        //CATCH MESSAGES
        public const string NullException = "A required value is missing.";
        public const string CastException = "Invalid type conversion.";
        public const string ArgumentOutOfRange = "Value is out of range.";
        public const string IndexOutOfRange = "An unexpected error occurred.";
        public const string generalexception = "An unexpected error occurred.";
    }
}
