using System.Collections.Generic;
using App.Core.Models;

namespace App.Core.Interfaces
{
    public interface IAppointmentService
    {
        List<Appointment> GetAll();
        List<Appointment> Search(string keyword);
        List<Appointment> GetByStatus(AppointmentStatus status);
        List<Appointment> GetByDoctor(int doctorId);
        Appointment GetById(int appointmentId);
        bool Add(Appointment appointment);
        bool Update(Appointment appointment);
        bool Delete(int appointmentId);
        int GetTotalCount();
        int GetCountByStatus(AppointmentStatus status);
    }
}
