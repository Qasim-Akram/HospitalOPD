using System.Collections.Generic;
using App.Core.Models;

namespace App.Core.Interfaces
{
    public interface IDoctorService
    {
        List<Doctor> GetAll();
        List<Doctor> Search(string keyword);
        Doctor GetById(int doctorId);
        bool Add(Doctor doctor);
        bool Update(Doctor doctor);
        bool Delete(int doctorId);
        int GetTotalCount();
    }
}
