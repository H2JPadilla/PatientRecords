using System;
using System.Collections.Generic;
using System.Linq;
using DAL.DAO;
using EL;
using UL;

namespace BLL
{
    public class PatientBLL
    {
        private readonly PatientDAL dal = new PatientDAL();

        public List<PatientEntity> GetPatients()
        {
            return dal.GetPatients();
        }

        public PatientEntity GetPatientById(int id)
        {
            return dal.GetPatientById(id);
        }

        public bool Exists(string patientName, string drugName, decimal? dosage, DateTime date, int? patientId = null, bool checkUniqueDrugOnly = false)
        {
            patientName = Validations.CleanSpaces(patientName);
            drugName = Validations.CleanSpaces(drugName);

            var existingRecords = dal.GetPatients()
                .Where(p => p.Patient.Equals(patientName, StringComparison.OrdinalIgnoreCase) && p.ModifiedDate.Date == date.Date) // Jabes, Alaxan, 250, 9/19/2025
                .AsQueryable();

            if (checkUniqueDrugOnly) // Jabes, Alaxan, 300, 9.19.2025 == Error. usage of same drugs within same day. // Jabes, Alaxan, 300, 9.20.2025 == Success. same patient, medicine, dosage but differnt time
            {
                existingRecords = existingRecords.Where(p => p.DrugName.Equals(drugName, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                existingRecords = existingRecords
                    .Where(p => p.DrugName.Equals(drugName, StringComparison.OrdinalIgnoreCase) && p.Dosage == dosage);
            }

            if (patientId.HasValue) // checks all
            {
                existingRecords = existingRecords.Where(p => p.ID != patientId.Value);
            }

            return existingRecords.Any();
        }

        public void AddPatient(PatientEntity patient)
        {
            string message, errorField;

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

            if (Exists(patient.Patient, patient.DrugName, patient.Dosage, DateTime.Now.Date))
            {
                throw new InvalidOperationException(MessageUtil.ExistingRecord);
            }
            if (Exists(patient.Patient, patient.DrugName, null, DateTime.Now.Date, null, true))
            {
                throw new InvalidOperationException(MessageUtil.ExistingDrug);
            }

            patient.ModifiedDate = DateTime.Now;
            dal.AddPatient(patient);
        }

        public void UpdatePatient(PatientEntity patient)
        {
            string message, errorField;

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

            if (Exists(patient.Patient, patient.DrugName, patient.Dosage, DateTime.Now.Date, patient.ID))
            {
                throw new InvalidOperationException(MessageUtil.ExistingRecord);
            }
            if (Exists(patient.Patient, patient.DrugName, null, DateTime.Now.Date, patient.ID, true))
            {
                throw new InvalidOperationException(MessageUtil.ExistingDrug);
            }

            patient.ModifiedDate = DateTime.Now;
            dal.UpdatePatient(patient);
        }

        public void DeletePatient(int id)
        {
            dal.DeletePatient(id);
        }
    }
}