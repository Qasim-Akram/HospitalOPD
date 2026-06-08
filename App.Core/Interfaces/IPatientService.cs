using System.Collections.Generic;
using App.Core.Models;

namespace App.Core.Interfaces
{
    public interface IPatientService
    {
        List<Patient> GetAll();
        List<Patient> Search(string keyword);
        Patient GetById(int patientId);
        bool Add(Patient patient);
        bool Update(Patient patient);
        bool Delete(int patientId);
        int GetTotalCount();
    }
}
