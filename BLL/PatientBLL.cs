// C:\Users\H2 SOFTWARE\Desktop\PatientRecords\BLL\PatientBLL.cs
using DAL.DAO;
using EL;
using System;
using System.Collections.Generic;
using System.Linq;
using UL;

namespace BLL
{
    public class PatientBLL
    {
        public List<PatientEntity> GetPatient() => dao.GetPatient();
        public PatientEntity GetPatientById(int id) => dao.GetPatientById(id);

        private readonly PatientDAL dao = new PatientDAL();

        public bool AddPatient(PatientEntity drug, out string message, out string errorField)
        {
            message = string.Empty;
            errorField = string.Empty;

            try
            {
                drug.Patient = Validations.CleanSpaces(drug.Patient);
                drug.DrugName = Validations.CleanSpaces(drug.DrugName);

                if (!Validations.FillRequired(drug.Patient, "Patient", out message, out errorField))
                    return false;

                if (!Validations.FillRequired(drug.DrugName, "DrugName", out message, out errorField))
                    return false;

                if (!Validations.ValidatePositiveNumber(drug.Dosage, "Dosage", out message, out errorField))
                    return false;

                drug.ModifiedDate = DateTime.Now;

                var drugsToday = dao.GetPatient()
                    .Where(x => x.Patient == drug.Patient
                             && x.DrugName == drug.DrugName
                             && x.ModifiedDate.Date == drug.ModifiedDate.Date)
                    .ToList();

                var existingfirst = drugsToday
                    .FirstOrDefault(x => x.Dosage != drug.Dosage);

                if (existingfirst != null)
                {
                    message = "Medicine is limited to one per patient per day";
                    return false;
                }

                var existing = drugsToday
                    .FirstOrDefault(x => x.Dosage == drug.Dosage);

                if (existing != null)
                {
                    message = MessageUtil.Exist;
                    return false;
                }

                dao.AddPatient(drug);
                message = MessageUtil.AddSuccess;
                return true;
            }
            catch (Exception ex)
            {
                Validations.Handle(ex, out message, out errorField);
                return false;
            }
        }

        public bool UpdatePatient(PatientEntity drug, out string message, out string errorField)
        {
            message = string.Empty;
            errorField = string.Empty;
            try
            {
                drug.Patient = Validations.CleanSpaces(drug.Patient);
                drug.DrugName = Validations.CleanSpaces(drug.DrugName);

                if (!Validations.FillRequired(drug.Patient, "Patient", out message, out errorField))
                    return false;

                if (!Validations.FillRequired(drug.DrugName, "DrugName", out message, out errorField))
                    return false;

                if (!Validations.ValidatePositiveNumber(drug.Dosage, "Dosage", out message, out errorField))
                    return false;

                drug.ModifiedDate = DateTime.Now;

                var drugsToday = dao.GetPatient()
                    .Where(x => x.Patient == drug.Patient
                             && x.DrugName == drug.DrugName
                             && x.ModifiedDate.Date == drug.ModifiedDate.Date
                             && x.ID != drug.ID)
                    .ToList();

                var existingfirst = drugsToday.FirstOrDefault(x => x.Dosage != drug.Dosage);
                if (existingfirst != null)
                {
                    message = "Medicine is limited to one per patient per day";
                    return false;
                }

                var existing = drugsToday.FirstOrDefault(x => x.Dosage == drug.Dosage);
                if (existing != null)
                {
                    message = MessageUtil.Exist;
                    return false;
                }

                dao.UpdatePatient(drug);
                message = MessageUtil.UpdateSuccess;
                return true;
            }
            catch (Exception ex)
            {
                Validations.Handle(ex, out message, out errorField);
                return false;
            }
        }

        public bool DeletePatient(int id, out string message, out string errorField)
        {
            message = string.Empty;
            errorField = string.Empty;

            try
            {
                dao.DeletePatient(id);
                message = MessageUtil.DeleteSuccess;
                return true;
            }
            catch (Exception ex)
            {
                Validations.Handle(ex, out message, out errorField);
                return false;
            }
        }
    }
}
