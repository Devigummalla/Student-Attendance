-- =============================================
-- Student Attendance Management System
-- Script 4: Test the database rules
-- Everything runs inside a transaction and is ROLLED BACK at the end,
-- so your data does not change. Read each result and compare with
-- the "Expected" comment. Screenshot for your test results.
-- =============================================

USE StudentAttendanceDB;
GO

BEGIN TRANSACTION;

-- TC01: register a new user          Expected: Result = 0
EXEC usp_RegisterUser 'Test User', 'test@college.com', 'testuser', 'dummyhash';

-- TC02: same username again           Expected: Result = 1
EXEC usp_RegisterUser 'Another User', 'another@college.com', 'testuser', 'dummyhash';

-- Same email again                    Expected: Result = 2
EXEC usp_RegisterUser 'Another User', 'test@college.com', 'anotheruser', 'dummyhash';

-- Login lookup by username, then by email   Expected: 1 row each, Role = Faculty
EXEC usp_GetUserForLogin 'testuser';
EXEC usp_GetUserForLogin 'test@college.com';

-- TC06: CS (1), 1st year              Expected: Rahul and Priya only (Kiran is inactive)
EXEC usp_GetStudents 1, 1;

-- TC16: Civil (5), 4th year           Expected: no rows
EXEC usp_GetStudents 5, 4;

-- TC09: Rahul (StudentId 1) Present   Expected: Result = 0
EXEC usp_MarkAttendance 1, 'Present', 1;

-- TC11: Rahul again, same day         Expected: Result = 1 (already submitted)
EXEC usp_MarkAttendance 1, 'Absent', 1;

-- TC12: Priya (StudentId 2), same day Expected: Result = 0
EXEC usp_MarkAttendance 2, 'Absent', 1;

-- Page-open check for Rahul           Expected: 1 row (Present, marked by System Administrator)
EXEC usp_GetTodayAttendance 1;

-- TC13: Rahul, a different date       Expected: inserted (1 row affected)
INSERT INTO Attendance (StudentId, AttendanceDate, AttendanceStatus, MarkedByUserId)
VALUES (1, DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 'Absent', 1);

-- Database guarantee: bypass the procedure and insert a duplicate directly
-- Expected: RED error 2627 "Violation of UNIQUE KEY constraint 'UQ_Attendance_Student_Date'"
INSERT INTO Attendance (StudentId, AttendanceDate, AttendanceStatus, MarkedByUserId)
VALUES (1, CAST(GETDATE() AS DATE), 'Absent', 1);

-- Wrong status                        Expected: RED error 547 (CHECK constraint)
INSERT INTO Attendance (StudentId, AttendanceDate, AttendanceStatus, MarkedByUserId)
VALUES (2, DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 'Late', 1);

-- New CS 2026 student                 Expected: RollNumber = CS2026004
EXEC usp_AddStudent 'Test Student', 1, 2026, 1;

-- Session: save, check, logout, check again
DECLARE @Token UNIQUEIDENTIFIER = NEWID();
EXEC usp_SaveSession @Token, 1, '2099-12-31';
EXEC usp_IsSessionValid @Token;      -- Expected: Result = 1
EXEC usp_EndSession @Token;
EXEC usp_IsSessionValid @Token;      -- Expected: Result = 0 (TC15: logged out)

-- Dashboard numbers                   Expected: Present 1, Absent 1
EXEC usp_GetTodaySummary;

ROLLBACK TRANSACTION;   -- undo all test changes
PRINT 'Tests finished. All changes rolled back.';
