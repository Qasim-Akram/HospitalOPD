# Hospital OPD Management System

A desktop application for managing outpatient department (OPD) operations — patients, doctors, and appointments — built with C# WinForms and SQL Server LocalDB.

---

## What it does

- Register and manage patients and doctors
- Schedule, complete, or cancel appointments
- Track appointment status (Scheduled / Completed / Cancelled)
- Clean sidebar navigation with swappable views

---

## Tech stack

- **C# / .NET Framework 4.8**
- **WinForms** — two-project solution (`App.Core` + `App.WindowsApp`)
- **SQL Server LocalDB** — `(localdb)\MSSQLLocalDB`
- **ADO.NET** — raw `System.Data.SqlClient`, no Entity Framework
- **App.config** for connection string

---

## Project structure

```
HospitalOPD/
├── App.Core/
│   ├── Models/          # Patient, Doctor, Appointment, AppointmentStatus
│   ├── Interfaces/      # Service contracts
│   └── Services/        # ADO.NET implementations
└── App.WindowsApp/
    ├── Views/           # PatientsView, DoctorsView, AppointmentsView, Dashboard
    ├── Forms/           # PatientForm, DoctorForm, AppointmentForm
    ├── Helpers/         # Theme, UIHelper
    └── MainForm.cs      # Sidebar + UserControl swapping
```

---

## Getting started

**Prerequisites**
- Visual Studio 2019 or later
- .NET Framework 4.8
- SQL Server LocalDB (ships with Visual Studio)

**Setup**

1. Clone the repo
   ```bash
   git clone https://github.com/your-username/HospitalOPD.git
   ```

2. Open `HospitalOPD.sln` in Visual Studio

3. Run the database setup script against `(localdb)\MSSQLLocalDB` to create the `HospitalOPD` database and tables (Patients, Doctors, Appointments)

4. Build and run — set `App.WindowsApp` as the startup project

---

## Database

- **Server:** `(localdb)\MSSQLLocalDB`
- **Database:** `HospitalOPD`
- **Tables:** `Patients`, `Doctors`, `Appointments`

The connection string lives in `App.WindowsApp/App.config`.

---

## Notes

No NuGet packages, no ORM — just straight ADO.NET. Kept intentionally simple since this was built as a semester project for learning purposes.
