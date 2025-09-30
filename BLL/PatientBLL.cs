using DAL.DAO;
using EL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UL;

namespace BLL
{
    public class PatientBLL
    {
        private readonly PatientDAL dal = new PatientDAL();

        //Get all patients ID, DrugName, Dosage, ModifiedDate.
        public List<PatientEntity> GetPatients()
        {
            try
            {
                return dal.GetPatients();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get patient ID
        public PatientEntity GetPatientById(int id)
        {
            try
            {
                return dal.GetPatientById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Exist validation
        public bool Exists(string patientName, string drugName, decimal? dosage, DateTime date, int? patientId = null, bool checkUniqueDrugOnly = false)
        {
            try
            {

                return dal.Exists(patientName, drugName, dosage, date, patientId, checkUniqueDrugOnly);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    //CREATE OR UPDATE
    public void CreateUpdate(PatientEntity patient)
    {
      string message = null;
      string errorField = null;

      //VALIDATE
      try
      {
        if (!Validations.FillRequired(patient.Patient, "Patient", out message, out errorField))
        {
          throw new ArgumentException(message, errorField);
        }
        if (!Validations.FillRequired(patient.DrugName, "DrugName", out message, out errorField))
        {
          throw new ArgumentException(message, errorField);
        }
        if (!Validations.ValidatePositiveNumber(patient.Dosage, "Dosage", out message, out errorField))
        {
          throw new ArgumentException(message, errorField);
        }
        if (!Validations.ValidateDecimalPlaces(patient.Dosage, "Dosage", out message, out errorField))
        {
          throw new ArgumentException(message, errorField);
        }

        patient.Patient = Validations.CleanSpaces(patient.Patient);
        patient.DrugName = Validations.CleanSpaces(patient.DrugName);

        //CHECK WHETHER TO CREATE OR UPDATE. IF ID = 0 , THEN CREATE. ELSE, UPDATE.
        if (patient.ID <= 0)
        {
          if (Exists(patient.Patient, patient.DrugName, patient.Dosage, DateTime.Now.Date))
          {
            throw new InvalidOperationException(MessageUtil.ExistingRecord);
          }
          if (Exists(patient.Patient, patient.DrugName, null, DateTime.Now.Date, null, true))
          {
            throw new InvalidOperationException(MessageUtil.ExistingDrug);
          }
          patient.ModifiedDate = DateTime.Now;
          dal.CreateUpdate(patient);
        }

        //UPDATE FUNCTION
        else
        {
          //Check if a record with the same data already exists(excluding the current record)
          var existingPatient = GetPatientById(patient.ID);
          if (existingPatient == null)
          {
            throw new InvalidOperationException(MessageUtil.RecordNotFound);
          }


          //Check if there are any changes to the data
          bool hasChanges =
              !existingPatient.Patient.Equals(patient.Patient, StringComparison.OrdinalIgnoreCase) ||
              !existingPatient.DrugName.Equals(patient.DrugName, StringComparison.OrdinalIgnoreCase) ||
              existingPatient.Dosage != patient.Dosage;

          if (!hasChanges)
          {
            throw new InvalidOperationException(MessageUtil.NoChange);
          }

          //New check for duplicate record including the modified fields
          if (Exists(patient.Patient, patient.DrugName, patient.Dosage, DateTime.Now.Date, patient.ID))
          {
            throw new InvalidOperationException(MessageUtil.ExistingRecord);
          }

          // New check for duplicate drug on the same day
          if (Exists(patient.Patient, patient.DrugName, null, DateTime.Now.Date, patient.ID, true))
          {
            throw new InvalidOperationException(MessageUtil.ExistingDrug);
          }

          //Setting the date(NOW)
          patient.ModifiedDate = DateTime.Now;

          dal.CreateUpdate(patient);
        }  
      }
      catch (SqlException ex)
      {
        throw new InvalidOperationException(ex.Message, ex);
      }
    }

        //Delete Patient by ID
        public void DeletePatient(int id)
        {
            try
            {
                dal.DeletePatient(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}