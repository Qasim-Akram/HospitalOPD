using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using App.Core.Interfaces;
using App.Core.Models;

namespace App.Core.Services
{
    public class PatientService : IPatientService
    {
        private readonly string _connStr;

        public PatientService()
        {
            _connStr = ConfigurationManager.ConnectionStrings["HospitalDB"].ConnectionString;
        }

        public List<Patient> GetAll()
        {
            var list = new List<Patient>();
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand("SELECT * FROM Patients ORDER BY FullName", con))
            {
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read()) list.Add(Map(rdr));
            }
            return list;
        }

        public List<Patient> Search(string keyword)
        {
            var list = new List<Patient>();
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand("SELECT * FROM Patients WHERE FullName LIKE @kw OR Phone LIKE @kw ORDER BY FullName", con))
            {
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read()) list.Add(Map(rdr));
            }
            return list;
        }

        public Patient GetById(int patientId)
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand("SELECT * FROM Patients WHERE PatientId = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", patientId);
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    if (rdr.Read()) return Map(rdr);
            }
            return null;
        }

        public bool Add(Patient p)
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(
                "INSERT INTO Patients (FullName, Gender, DateOfBirth, Phone, Address, RegisteredOn) VALUES (@n,@g,@d,@ph,@a,@r)", con))
            {
                cmd.Parameters.AddWithValue("@n", p.FullName);
                cmd.Parameters.AddWithValue("@g", p.Gender);
                cmd.Parameters.AddWithValue("@d", p.DateOfBirth);
                cmd.Parameters.AddWithValue("@ph", p.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@a", p.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@r", DateTime.Now);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(Patient p)
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(
                "UPDATE Patients SET FullName=@n, Gender=@g, DateOfBirth=@d, Phone=@ph, Address=@a WHERE PatientId=@id", con))
            {
                cmd.Parameters.AddWithValue("@n", p.FullName);
                cmd.Parameters.AddWithValue("@g", p.Gender);
                cmd.Parameters.AddWithValue("@d", p.DateOfBirth);
                cmd.Parameters.AddWithValue("@ph", p.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@a", p.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@id", p.PatientId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int patientId)
        {
            using (var con = new SqlConnection(_connStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("DELETE FROM Appointments WHERE PatientId=@id", con))
                {
                    cmd.Parameters.AddWithValue("@id", patientId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand("DELETE FROM Patients WHERE PatientId=@id", con))
                {
                    cmd.Parameters.AddWithValue("@id", patientId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public int GetTotalCount()
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Patients", con))
            {
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        private Patient Map(IDataReader r) => new Patient
        {
            PatientId = (int)r["PatientId"],
            FullName = r["FullName"].ToString(),
            Gender = r["Gender"].ToString(),
            DateOfBirth = (DateTime)r["DateOfBirth"],
            Phone = r["Phone"] == DBNull.Value ? "" : r["Phone"].ToString(),
            Address = r["Address"] == DBNull.Value ? "" : r["Address"].ToString(),
            RegisteredOn = (DateTime)r["RegisteredOn"]
        };
    }
}
