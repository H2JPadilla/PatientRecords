
using EL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.DAO
{
    public class PatientDAL : DatabaseContext
    {
        public List<PatientEntity> GetPatients()
        {
            var list = new List<PatientEntity>();
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, Patient, DrugName, Dosage, ModifiedDate FROM TablePrescription", con);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new PatientEntity
                    {
                        ID = (int)rdr["ID"],
                        Patient = rdr["Patient"].ToString(),
                        DrugName = rdr["DrugName"].ToString(),
                        Dosage = Convert.ToInt32(rdr["Dosage"]),
                        ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"])
                    });
                }
            }
            return list;
        }

        public PatientEntity GetPatientById(int id)
        {
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, Patient, DrugName, Dosage, ModifiedDate FROM TablePrescription WHERE ID=@ID", con);
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
                        Dosage = Convert.ToInt32(rdr["Dosage"]),
                        ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"])
                    };
                }
            }
            return null;
        }

        public void AddPatient(PatientEntity patient)
        {
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand(
                    "INSERT INTO TablePrescription (Patient, DrugName, Dosage, ModifiedDate) VALUES (@Patient, @DrugName, @Dosage, @ModifiedDate)", con);

                cmd.Parameters.AddWithValue("@Patient", patient.Patient);
                cmd.Parameters.AddWithValue("@DrugName", patient.DrugName);
                cmd.Parameters.AddWithValue("@Dosage", patient.Dosage);
                cmd.Parameters.AddWithValue("@ModifiedDate", patient.ModifiedDate);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdatePatient(PatientEntity patient)
        {
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand(
                    "UPDATE TablePrescription SET Patient=@Patient, DrugName=@DrugName, Dosage=@Dosage, ModifiedDate=@ModifiedDate WHERE ID=@ID", con);

                cmd.Parameters.AddWithValue("@Patient", patient.Patient);
                cmd.Parameters.AddWithValue("@DrugName", patient.DrugName);
                cmd.Parameters.AddWithValue("@Dosage", patient.Dosage);
                cmd.Parameters.AddWithValue("@ModifiedDate", patient.ModifiedDate);
                cmd.Parameters.AddWithValue("@ID", patient.ID);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeletePatient(int id)
        {
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand("DELETE FROM TablePrescription WHERE ID=@ID", con);
                cmd.Parameters.AddWithValue("@ID", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public bool Exists(string patient, string drug, int? dosage, DateTime? date = null, int? excludeId = null, bool isUniqueDrugCheck = false)
        {
            using (var con = GetConnection())
            {
                string sql;
                if (isUniqueDrugCheck)
                {
                    sql = "SELECT COUNT(1) FROM TablePrescription WHERE Patient = @Patient AND CAST(ModifiedDate AS DATE) = CAST(@Date AS DATE)";
                }
                else
                {
                    sql = "SELECT COUNT(1) FROM TablePrescription WHERE Patient = @Patient AND DrugName = @DrugName AND Dosage = @Dosage AND CAST(ModifiedDate AS DATE) = CAST(@Date AS DATE)";
                }

                if (excludeId.HasValue)
                {
                    sql += " AND ID <> @ID";
                }

                var cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Patient", patient);

                if (!isUniqueDrugCheck)
                {
                    cmd.Parameters.AddWithValue("@DrugName", drug);
                    cmd.Parameters.AddWithValue("@Dosage", dosage);
                }
                cmd.Parameters.AddWithValue("@Date", date.Value.Date);

                if (excludeId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ID", excludeId.Value);
                }

                con.Open();
                object result = cmd.ExecuteScalar();
                int count = Convert.ToInt32(result);

                return count > 0;
            }
        }
    }
}