-- =============================================
-- Student Attendance Management System
-- Script 3: Sample data
-- Run only once, on a fresh database.
-- =============================================

USE StudentAttendanceDB;
GO

-- ---------------------------------------------
-- Departments (IDs will be 1 to 5)
-- ---------------------------------------------
INSERT INTO Departments (DeptCode, DepartmentName) VALUES
('CS', 'Computer Science'),        -- 1
('IT', 'Information Technology'),  -- 2
('EC', 'Electronics'),             -- 3
('ME', 'Mechanical'),              -- 4
('CE', 'Civil');                   -- 5
GO

-- ---------------------------------------------
-- Admin user
-- Username: admin    PIN: 2026
-- PinHash format: iterations.salt.hash  (PBKDF2 with SHA-256)
-- ---------------------------------------------
INSERT INTO Users (FullName, Email, Username, PinHash, Role)
VALUES ('System Administrator', 'admin@college.com', 'admin',
        '100000.XaIHRJO75nMJROZzYRE1GA==.NaP8KBI7P5OLZAgIWG+tzpy1mkPk+y66knhPUMm2SiA=',
        'Admin');
GO

-- ---------------------------------------------
-- Students (added through usp_AddStudent so roll numbers are generated)
-- Parameters: Name, DepartmentId, AdmissionYear, YearOfStudy
-- 1st year joined 2026, 2nd year 2025, 3rd year 2024, 4th year 2023
-- Each line shows the new roll number in the Results tab.
-- ---------------------------------------------

-- Computer Science (DepartmentId 1)
EXEC usp_AddStudent 'Rahul Sharma',     1, 2026, 1;   -- CS2026001
EXEC usp_AddStudent 'Priya Verma',      1, 2026, 1;   -- CS2026002
EXEC usp_AddStudent 'Kiran Patil',      1, 2026, 1;   -- CS2026003 (made inactive below)
EXEC usp_AddStudent 'Karthik Reddy',    1, 2025, 2;
EXEC usp_AddStudent 'Sneha Iyer',       1, 2025, 2;
EXEC usp_AddStudent 'Vikram Singh',     1, 2024, 3;
EXEC usp_AddStudent 'Ananya Rao',       1, 2024, 3;
EXEC usp_AddStudent 'Rohit Kumar',      1, 2023, 4;
EXEC usp_AddStudent 'Divya Nair',       1, 2023, 4;

-- Information Technology (DepartmentId 2)
EXEC usp_AddStudent 'Aditya Mehta',     2, 2026, 1;
EXEC usp_AddStudent 'Pooja Patel',      2, 2026, 1;
EXEC usp_AddStudent 'Sai Teja',         2, 2025, 2;
EXEC usp_AddStudent 'Lakshmi Prasanna', 2, 2025, 2;
EXEC usp_AddStudent 'Nikhil Jain',      2, 2024, 3;
EXEC usp_AddStudent 'Kavya Menon',      2, 2024, 3;
EXEC usp_AddStudent 'Harsha Vardhan',   2, 2023, 4;
EXEC usp_AddStudent 'Meghana Reddy',    2, 2023, 4;

-- Electronics (DepartmentId 3)
EXEC usp_AddStudent 'Varun Gupta',      3, 2026, 1;
EXEC usp_AddStudent 'Shreya Das',       3, 2026, 1;
EXEC usp_AddStudent 'Manoj Naidu',      3, 2025, 2;
EXEC usp_AddStudent 'Swathi Kulkarni',  3, 2025, 2;
EXEC usp_AddStudent 'Ravi Teja',        3, 2024, 3;
EXEC usp_AddStudent 'Bhavana Joshi',    3, 2024, 3;
EXEC usp_AddStudent 'Suresh Babu',      3, 2023, 4;
EXEC usp_AddStudent 'Nandini Pillai',   3, 2023, 4;

-- Mechanical (DepartmentId 4)
EXEC usp_AddStudent 'Akhil Chowdary',   4, 2026, 1;
EXEC usp_AddStudent 'Ritika Saxena',    4, 2026, 1;
EXEC usp_AddStudent 'Pranav Desai',     4, 2025, 2;
EXEC usp_AddStudent 'Keerthi Shetty',   4, 2025, 2;
EXEC usp_AddStudent 'Abhishek Yadav',   4, 2024, 3;
EXEC usp_AddStudent 'Deepika Mishra',   4, 2024, 3;
EXEC usp_AddStudent 'Ganesh Murthy',    4, 2023, 4;
EXEC usp_AddStudent 'Harini Krishnan',  4, 2023, 4;

-- Civil (DepartmentId 5) - no 4th year students, to test the "No students found" message
EXEC usp_AddStudent 'Siddharth Bose',   5, 2026, 1;
EXEC usp_AddStudent 'Tanvi Agarwal',    5, 2026, 1;
EXEC usp_AddStudent 'Rakesh Goud',      5, 2025, 2;
EXEC usp_AddStudent 'Sravani Kumari',   5, 2025, 2;
EXEC usp_AddStudent 'Mahesh Varma',     5, 2024, 3;
EXEC usp_AddStudent 'Aishwarya Hegde',  5, 2024, 3;
GO

-- An inactive student should never appear in the attendance list
UPDATE Students SET IsActive = 0 WHERE RollNumber = 'CS2026003';
GO

-- ---------------------------------------------
-- Check the data
-- ---------------------------------------------
SELECT * FROM Departments;
SELECT UserId, FullName, Username, Role FROM Users;
SELECT RollNumber, StudentName, DepartmentId, AdmissionYear, YearOfStudy, IsActive
FROM Students
ORDER BY RollNumber;
