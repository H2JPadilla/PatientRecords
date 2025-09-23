using System;

namespace UL
{
    public class MessageUtil
    {
        //FRONT MESSAGE
        public const string AddSuccess = "Record successfully saved.";
        public const string UpdateSuccess = "Record successfully updated.";
        public const string DeleteSuccess = "Record successfully deleted.";

        public const string RequiredField = "All field/s are required.";
        public const string NonInteger = "Dosage must be a positive number.";
        public const string LimitRange = " Dosage cannot exceed 999.9999.";
        public const string NoChange = "No changes were made to the record."; 

        public const string RecordNotFound = "Record not found.";

        //BACK END MESSAGE
        public const string FailedToAdd = "Failed to add data";
        public const string FailedToUpdate = "Failed to update data";
        public const string FailedToDelete = "Failed to delete data";
        public const string ExistingRecord = "Record already exists.";
        public const string ExistingDrug = "Cannot add same drug to a patient.";
        //CATCH MESSAGES
        public const string NullException = "A required value is missing.";
        public const string CastException = "Invalid type conversion.";
        public const string ArgumentOutOfRange = "Value is out of range.";
        public const string IndexOutOfRange = "An unexpected error occurred.";
        public const string generalexception = "An unexpected error occurred.";
    }
}