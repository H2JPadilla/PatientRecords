using DAL.DAO;
using EL;
using System;
using System.Collections.Generic;

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

        public bool Exists(string patient, string drug, int? dosage, DateTime? date, int? excludeId = null, bool isUniqueDrugCheck = false)
        {
            return dal.Exists(patient, drug, dosage, date, excludeId, isUniqueDrugCheck);
        }

        public void AddPatient(PatientEntity patient)
        {
            patient.ModifiedDate = DateTime.Now;
            dal.AddPatient(patient);
        }

        public void UpdatePatient(PatientEntity patient)
        {
            patient.ModifiedDate = DateTime.Now;
            dal.UpdatePatient(patient);
        }

        public void DeletePatient(int id)
        {
            dal.DeletePatient(id);
        }
    }
}