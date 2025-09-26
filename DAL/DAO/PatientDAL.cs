using EL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace DAL.DAO
{
    public class PatientDAL : DatabaseContext // Extend DbContext for connection.
    {
        public List<PatientEntity> GetPatients() // This is the list for displaying data
        {
            var list = new List<PatientEntity>();
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand("spSelectPatient", con); //SP
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new PatientEntity
                    {
                        ID = (int)rdr["ID"],
                        Patient = rdr["Patient"].ToString(),
                        DrugName = rdr["DrugName"].ToString(),
                        Dosage = Convert.ToDecimal(rdr["Dosage"]),
                        ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"])

                    });
                }
            }
            return list;
        }

        //Getting the ID for update
        public PatientEntity GetPatientById(int id) // Get ID
        {
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand("spSelectPatient", con); // SP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);
                con.Open();
                var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    return new PatientEntity
                    {
                        ID = (int)rdr["ID"],

                        Patient = rdr["Patient"].ToString(),
                        DrugName = rdr["DrugName"].ToString(),
                        Dosage = Convert.ToDecimal(rdr["Dosage"]),
                        ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"])
                    };
                }
            }
            return null;
        }

        //Add Patient
        public void AddPatient(PatientEntity patient)
        {
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand("spCreatePatient", con); // SP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Patient", patient.Patient);
                cmd.Parameters.AddWithValue("@DrugName", patient.DrugName);
                cmd.Parameters.AddWithValue("@Dosage", patient.Dosage);
                cmd.Parameters.AddWithValue("@ModifiedDate", patient.ModifiedDate);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //Update Patient
        public void UpdatePatient(PatientEntity patient)
        {
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand("spUpdatePatient", con); // SP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Patient", patient.Patient);
                cmd.Parameters.AddWithValue("@DrugName", patient.DrugName);
                cmd.Parameters.AddWithValue("@Dosage", patient.Dosage);
                cmd.Parameters.AddWithValue("@ModifiedDate", patient.ModifiedDate);
                cmd.Parameters.AddWithValue("@ID", patient.ID);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //Delete Patient
        public void DeletePatient(int id)
        {
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand("spDeletePatient", con); //SP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //Check existing record
        public bool Exists(string patient, string drug, decimal? dosage, DateTime? date = null, int? excludeId = null, bool isUniqueDrugCheck = false)
        {
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand("spCheckPatientExists", con); // SP
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Patient", patient);
                cmd.Parameters.AddWithValue("@DrugName", drug);
                cmd.Parameters.AddWithValue("@Dosage", dosage.HasValue ? (object)dosage.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Date", date.Value.Date);
                cmd.Parameters.AddWithValue("@ExcludeId", excludeId.HasValue ? (object)excludeId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@IsUniqueDrugCheck", isUniqueDrugCheck);

                con.Open();
                object result = cmd.ExecuteScalar();
                int count = Convert.ToInt32(result);

                return count > 0;
            }
        }
    }
}