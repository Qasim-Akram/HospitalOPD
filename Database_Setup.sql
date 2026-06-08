IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'HospitalOPD')
    CREATE DATABASE HospitalOPD;
GO

USE HospitalOPD;
GO

IF OBJECT_ID('Appointments', 'U') IS NOT NULL DROP TABLE Appointments;
IF OBJECT_ID('Patients', 'U') IS NOT NULL DROP TABLE Patients;
IF OBJECT_ID('Doctors', 'U') IS NOT NULL DROP TABLE Doctors;
GO

CREATE TABLE Patients (
    PatientId    INT IDENTITY(1,1) PRIMARY KEY,
    FullName     NVARCHAR(150)   NOT NULL,
    Gender       NVARCHAR(10)    NOT NULL,
    DateOfBirth  DATE            NOT NULL,
    Phone        NVARCHAR(20)    NULL,
    Address      NVARCHAR(300)   NULL,
    RegisteredOn DATETIME        NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Doctors (
    DoctorId       INT IDENTITY(1,1) PRIMARY KEY,
    FullName       NVARCHAR(150) NOT NULL,
    Specialization NVARCHAR(100) NOT NULL,
    Phone          NVARCHAR(20)  NULL,
    Email          NVARCHAR(150) NULL,
    IsAvailable    BIT           NOT NULL DEFAULT 1
);

CREATE TABLE Appointments (
    AppointmentId   INT IDENTITY(1,1) PRIMARY KEY,
    PatientId       INT           NOT NULL REFERENCES Patients(PatientId),
    DoctorId        INT           NOT NULL REFERENCES Doctors(DoctorId),
    AppointmentDate DATETIME      NOT NULL,
    Reason          NVARCHAR(300) NULL,
    Status          NVARCHAR(20)  NOT NULL DEFAULT 'Scheduled',
    Notes           NVARCHAR(500) NULL
);
GO

INSERT INTO Patients (FullName, Gender, DateOfBirth, Phone, Address) VALUES
('Ahmed Raza', 'Male', '1990-03-15', '0300-1234567', 'House 12, Block B, Bahawalpur'),
('Sana Malik', 'Female', '1985-07-22', '0321-9876543', 'Street 5, Model Town, Lahore'),
('Usman Khan', 'Male', '1978-11-05', '0333-4567890', 'Flat 3, City Centre, Karachi');

INSERT INTO Doctors (FullName, Specialization, Phone, Email, IsAvailable) VALUES
('Dr. Tariq Mahmood', 'Cardiology', '0300-1111111', 'tariq@hospital.com', 1),
('Dr. Ayesha Siddiqui', 'Neurology', '0321-2222222', 'ayesha@hospital.com', 1),
('Dr. Bilal Ahmed', 'Orthopedics', '0333-3333333', 'bilal@hospital.com', 0);

INSERT INTO Appointments (PatientId, DoctorId, AppointmentDate, Reason, Status, Notes) VALUES
(1, 1, GETDATE(), 'Chest pain checkup', 'Scheduled', 'First visit'),
(2, 2, DATEADD(DAY, -2, GETDATE()), 'Headache and dizziness', 'Completed', 'Prescribed medication'),
(3, 3, DATEADD(DAY, -5, GETDATE()), 'Knee pain', 'Cancelled', 'Patient rescheduled');
GO
