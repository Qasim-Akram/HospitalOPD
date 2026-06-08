using System;

namespace App.Core.Models
{
    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }

    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Notes { get; set; }

        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
    }
}
