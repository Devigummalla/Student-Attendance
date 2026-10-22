-- =============================================
-- Student Attendance Management System
-- Script 1: Create database and tables
-- =============================================

-- Optional: remove the old SAMS database first
-- USE master;
-- ALTER DATABASE SAMS SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
-- DROP DATABASE SAMS;

USE master;
GO

CREATE DATABASE StudentAttendanceDB;
GO

USE StudentAttendanceDB;
GO

-- ---------------------------------------------
-- Users: people who log in (Admin or Faculty)
-- ---------------------------------------------
CREATE TABLE Users
(
    UserId     INT IDENTITY(1,1) PRIMARY KEY,
    FullName   NVARCHAR(100) NOT NULL,
    Email      NVARCHAR(150) NOT NULL UNIQUE,
    Username   NVARCHAR(50)  NOT NULL UNIQUE,
    PinHash    VARCHAR(200)  NOT NULL,               -- hashed PIN, never the real PIN
    Role       VARCHAR(10)   NOT NULL DEFAULT 'Faculty',
    IsActive   BIT           NOT NULL DEFAULT 1,
    CreatedAt  DATETIME      NOT NULL DEFAULT GETDATE(),

    CONSTRAINT CK_Users_Role     CHECK (Role IN ('Admin', 'Faculty')),
    CONSTRAINT CK_Users_Username CHECK (Username NOT LIKE '%@%')   -- so login by username or email is never confused
);
GO

-- ---------------------------------------------
-- UserSessions: one row per login (JWT token)
-- Lets us really log out a token
-- ---------------------------------------------
CREATE TABLE UserSessions
(
    SessionId   INT IDENTITY(1,1) PRIMARY KEY,
    TokenId     UNIQUEIDENTIFIER NOT NULL UNIQUE,     -- same value as the JWT "jti" claim
    UserId      INT      NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    ExpiresAt   DATETIME NOT NULL,
    IsLoggedOut BIT      NOT NULL DEFAULT 0,
    CreatedAt   DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- ---------------------------------------------
-- Departments
-- ---------------------------------------------
CREATE TABLE Departments
(
    DepartmentId   INT IDENTITY(1,1) PRIMARY KEY,
    DeptCode       VARCHAR(5)    NOT NULL UNIQUE,     -- CS, IT, EC... used in roll numbers
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE,
    IsActive       BIT           NOT NULL DEFAULT 1
);
GO

-- ---------------------------------------------
-- Students
-- Roll number = DeptCode + AdmissionYear + 3-digit number
-- Example: CS2026001
-- ---------------------------------------------
CREATE TABLE Students
(
    StudentId     INT IDENTITY(1,1) PRIMARY KEY,
    RollNumber    VARCHAR(12)   NOT NULL UNIQUE,
    StudentName   NVARCHAR(100) NOT NULL,
    DepartmentId  INT NOT NULL FOREIGN KEY REFERENCES Departments(DepartmentId),
    AdmissionYear INT NOT NULL,                       -- year the student joined (never changes)
    YearOfStudy   INT NOT NULL,                       -- 1, 2, 3 or 4 (changes every year)
    IsActive      BIT NOT NULL DEFAULT 1,

    CONSTRAINT CK_Students_YearOfStudy CHECK (YearOfStudy BETWEEN 1 AND 4)
);
GO

-- Makes the "Department + Year" filter fast
CREATE INDEX IX_Students_Department_Year ON Students (DepartmentId, YearOfStudy);
GO

-- ---------------------------------------------
-- Attendance
-- ---------------------------------------------
CREATE TABLE Attendance
(
    AttendanceId     INT IDENTITY(1,1) PRIMARY KEY,
    StudentId        INT         NOT NULL FOREIGN KEY REFERENCES Students(StudentId),
    AttendanceDate   DATE        NOT NULL,
    AttendanceStatus VARCHAR(10) NOT NULL,
    MarkedByUserId   INT         NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    CreatedAt        DATETIME    NOT NULL DEFAULT GETDATE(),

    CONSTRAINT CK_Attendance_Status CHECK (AttendanceStatus IN ('Present', 'Absent')),

    -- The most important rule: one record per student per day
    CONSTRAINT UQ_Attendance_Student_Date UNIQUE (StudentId, AttendanceDate)
);
GO

PRINT 'Database and tables created.';
