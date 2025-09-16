using EL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DAO
{
    public class PatientDAL : DatabaseContext
    {
        public PatientEntity GetPatientById(int id)
        {
            PatientEntity drug = null;
            using (var con = GetConnection())
            {
                string query = "SELECT ID, Patient, DrugName, Dosage, ModifiedDate FROM TablePrescription WHERE ID=@ID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            drug = new PatientEntity
                            {
                                ID = Convert.ToInt32(reader["ID"].ToString()),
                                Patient = reader["Patient"].ToString(),
                                DrugName = reader["DrugName"].ToString(),
                                Dosage= Convert.ToDecimal(reader["Dosage"].ToString()),
                                ModifiedDate= Convert.ToDateTime(reader["ModifiedDate"])
                            };
                        }
                    }
                }
                return drug;
            }
        }

        public void AddPatient(PatientEntity drug)
        {
            using (var con = GetConnection())
            {
                string query = "INSERT INTO TablePrescription (Patient, DrugName, Dosage, ModifiedDate) " +
                               "VALUES (@Patient, @DrugName, @Dosage, @ModifiedDate)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Patient", drug.Patient);
                cmd.Parameters.AddWithValue("@DrugName", drug.DrugName);
                cmd.Parameters.AddWithValue("@Dosage", drug.Dosage);
                cmd.Parameters.AddWithValue("@ModifiedDate", drug.ModifiedDate);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<PatientEntity> GetPatient()
        {
            var drugs = new List<PatientEntity>();
            using (var con = GetConnection())
            {
                string query = "SELECT ID, Patient, DrugName, Dosage, ModifiedDate FROM TablePrescription";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    drugs.Add(new PatientEntity
                    {
                        ID = Convert.ToInt32(dr["ID"].ToString()),
                        Patient = dr["Patient"].ToString(),
                        DrugName = dr["DrugName"].ToString(),
                        Dosage = Convert.ToDecimal(dr["Dosage"].ToString()),
                        ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"])
                    });
                }
                return drugs;
            }
        }

        public bool UpdatePatient(PatientEntity drug)
        {
            bool isUpdated = false;
            using (var con = GetConnection())
            {
                string query = @"UPDATE TablePrescription SET Patient=@Patient, DrugName=@DrugName, Dosage=@Dosage, ModifiedDate=@ModifiedDate WHERE ID=@ID";
                using(SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ID", drug.ID);
                    cmd.Parameters.AddWithValue("@Patient", drug.Patient);
                    cmd.Parameters.AddWithValue("@DrugName", drug.DrugName);
                    cmd.Parameters.AddWithValue("@Dosage", drug.Dosage);
                    cmd.Parameters.AddWithValue("@ModifiedDate", drug.ModifiedDate);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    isUpdated = rows > 0;
                }
            }
            return isUpdated;
        }

        public bool DeletePatient(int id)
        {
            bool isDeleted = false;
            using (var con = GetConnection())
            {
                string query = @"DELETE FROM TablePrescription WHERE ID=@id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                   
                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    isDeleted = rows > 0;
                }
            }
            return isDeleted;
        }
    }
}
