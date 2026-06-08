using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using App.Core.Interfaces;
using App.Core.Models;

namespace App.Core.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly string _connStr;

        public DoctorService()
        {
            _connStr = ConfigurationManager.ConnectionStrings["HospitalDB"].ConnectionString;
        }

        public List<Doctor> GetAll()
        {
            var list = new List<Doctor>();
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand("SELECT * FROM Doctors ORDER BY FullName", con))
            {
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read()) list.Add(Map(rdr));
            }
            return list;
        }

        public List<Doctor> Search(string keyword)
        {
            var list = new List<Doctor>();
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand("SELECT * FROM Doctors WHERE FullName LIKE @kw OR Specialization LIKE @kw ORDER BY FullName", con))
            {
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read()) list.Add(Map(rdr));
            }
            return list;
        }

        public Doctor GetById(int doctorId)
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand("SELECT * FROM Doctors WHERE DoctorId=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", doctorId);
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    if (rdr.Read()) return Map(rdr);
            }
            return null;
        }

        public bool Add(Doctor d)
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(
                "INSERT INTO Doctors (FullName, Specialization, Phone, Email, IsAvailable) VALUES (@n,@s,@ph,@e,@av)", con))
            {
                cmd.Parameters.AddWithValue("@n", d.FullName);
                cmd.Parameters.AddWithValue("@s", d.Specialization);
                cmd.Parameters.AddWithValue("@ph", d.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@e", d.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@av", d.IsAvailable);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(Doctor d)
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(
                "UPDATE Doctors SET FullName=@n, Specialization=@s, Phone=@ph, Email=@e, IsAvailable=@av WHERE DoctorId=@id", con))
            {
                cmd.Parameters.AddWithValue("@n", d.FullName);
                cmd.Parameters.AddWithValue("@s", d.Specialization);
                cmd.Parameters.AddWithValue("@ph", d.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@e", d.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@av", d.IsAvailable);
                cmd.Parameters.AddWithValue("@id", d.DoctorId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int doctorId)
        {
            using (var con = new SqlConnection(_connStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("DELETE FROM Appointments WHERE DoctorId=@id", con))
                {
                    cmd.Parameters.AddWithValue("@id", doctorId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand("DELETE FROM Doctors WHERE DoctorId=@id", con))
                {
                    cmd.Parameters.AddWithValue("@id", doctorId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public int GetTotalCount()
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Doctors", con))
            {
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        private Doctor Map(IDataReader r) => new Doctor
        {
            DoctorId = (int)r["DoctorId"],
            FullName = r["FullName"].ToString(),
            Specialization = r["Specialization"].ToString(),
            Phone = r["Phone"] == DBNull.Value ? "" : r["Phone"].ToString(),
            Email = r["Email"] == DBNull.Value ? "" : r["Email"].ToString(),
            IsAvailable = (bool)r["IsAvailable"]
        };
    }
}
