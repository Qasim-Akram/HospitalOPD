using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using App.Core.Interfaces;
using App.Core.Models;

namespace App.Core.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly string _connStr;

        private const string SelectJoin = @"
            SELECT a.*, p.FullName AS PatientName, d.FullName AS DoctorName, d.Specialization AS DoctorSpec
            FROM Appointments a
            INNER JOIN Patients p ON a.PatientId = p.PatientId
            INNER JOIN Doctors d ON a.DoctorId = d.DoctorId";

        public AppointmentService()
        {
            _connStr = ConfigurationManager.ConnectionStrings["HospitalDB"].ConnectionString;
        }

        public List<Appointment> GetAll()
        {
            var list = new List<Appointment>();
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(SelectJoin + " ORDER BY a.AppointmentDate DESC", con))
            {
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read()) list.Add(Map(rdr));
            }
            return list;
        }

        public List<Appointment> Search(string keyword)
        {
            var list = new List<Appointment>();
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(SelectJoin +
                " WHERE p.FullName LIKE @kw OR d.FullName LIKE @kw OR a.Reason LIKE @kw ORDER BY a.AppointmentDate DESC", con))
            {
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read()) list.Add(Map(rdr));
            }
            return list;
        }

        public List<Appointment> GetByStatus(AppointmentStatus status)
        {
            var list = new List<Appointment>();
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(SelectJoin + " WHERE a.Status=@s ORDER BY a.AppointmentDate DESC", con))
            {
                cmd.Parameters.AddWithValue("@s", status.ToString());
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read()) list.Add(Map(rdr));
            }
            return list;
        }

        public List<Appointment> GetByDoctor(int doctorId)
        {
            var list = new List<Appointment>();
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(SelectJoin + " WHERE a.DoctorId=@id ORDER BY a.AppointmentDate DESC", con))
            {
                cmd.Parameters.AddWithValue("@id", doctorId);
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read()) list.Add(Map(rdr));
            }
            return list;
        }

        public Appointment GetById(int appointmentId)
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(SelectJoin + " WHERE a.AppointmentId=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", appointmentId);
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                    if (rdr.Read()) return Map(rdr);
            }
            return null;
        }

        public bool Add(Appointment a)
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(
                "INSERT INTO Appointments (PatientId, DoctorId, AppointmentDate, Reason, Status, Notes) VALUES (@pid,@did,@dt,@r,@s,@n)", con))
            {
                cmd.Parameters.AddWithValue("@pid", a.PatientId);
                cmd.Parameters.AddWithValue("@did", a.DoctorId);
                cmd.Parameters.AddWithValue("@dt", a.AppointmentDate);
                cmd.Parameters.AddWithValue("@r", a.Reason ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@s", a.Status.ToString());
                cmd.Parameters.AddWithValue("@n", a.Notes ?? (object)DBNull.Value);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(Appointment a)
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(
                "UPDATE Appointments SET PatientId=@pid, DoctorId=@did, AppointmentDate=@dt, Reason=@r, Status=@s, Notes=@n WHERE AppointmentId=@id", con))
            {
                cmd.Parameters.AddWithValue("@pid", a.PatientId);
                cmd.Parameters.AddWithValue("@did", a.DoctorId);
                cmd.Parameters.AddWithValue("@dt", a.AppointmentDate);
                cmd.Parameters.AddWithValue("@r", a.Reason ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@s", a.Status.ToString());
                cmd.Parameters.AddWithValue("@n", a.Notes ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@id", a.AppointmentId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int appointmentId)
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand("DELETE FROM Appointments WHERE AppointmentId=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", appointmentId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public int GetTotalCount()
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Appointments", con))
            {
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public int GetCountByStatus(AppointmentStatus status)
        {
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Appointments WHERE Status=@s", con))
            {
                cmd.Parameters.AddWithValue("@s", status.ToString());
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        private Appointment Map(IDataReader r) => new Appointment
        {
            AppointmentId = (int)r["AppointmentId"],
            PatientId = (int)r["PatientId"],
            DoctorId = (int)r["DoctorId"],
            AppointmentDate = (DateTime)r["AppointmentDate"],
            Reason = r["Reason"] == DBNull.Value ? "" : r["Reason"].ToString(),
            Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), r["Status"].ToString()),
            Notes = r["Notes"] == DBNull.Value ? "" : r["Notes"].ToString(),
            PatientName = r["PatientName"].ToString(),
            DoctorName = r["DoctorName"].ToString(),
            DoctorSpecialization = r["DoctorSpec"].ToString()
        };
    }
}
