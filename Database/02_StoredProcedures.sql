-- =============================================
-- Student Attendance Management System
-- Script 2: Stored procedures
-- =============================================
-- Procedures that "do an action" end with  SELECT ... AS Result
-- so in C# we can read the answer with ExecuteScalar().

USE StudentAttendanceDB;
GO

-- =============================================
-- USERS
-- =============================================

-- Register a new user (always gets the Faculty role)
-- Result: 0 = success, 1 = username taken, 2 = email taken
CREATE OR ALTER PROCEDURE usp_RegisterUser
    @FullName NVARCHAR(100),
    @Email    NVARCHAR(150),
    @Username NVARCHAR(50),
    @PinHash  VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
    BEGIN
        SELECT 1 AS Result;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
    BEGIN
        SELECT 2 AS Result;
        RETURN;
    END

    INSERT INTO Users (FullName, Email, Username, PinHash)
    VALUES (@FullName, @Email, @Username, @PinHash);

    SELECT 0 AS Result;
END
GO

-- Find a user by username OR email (for login)
-- The PIN is checked in C# by comparing hashes
CREATE OR ALTER PROCEDURE usp_GetUserForLogin
    @Login NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserId, FullName, Username, PinHash, Role, IsActive
    FROM Users
    WHERE Username = @Login OR Email = @Login;
END
GO

-- =============================================
-- SESSIONS (for JWT logout)
-- =============================================

-- Save a token when the user logs in
CREATE OR ALTER PROCEDURE usp_SaveSession
    @TokenId   UNIQUEIDENTIFIER,
    @UserId    INT,
    @ExpiresAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO UserSessions (TokenId, UserId, ExpiresAt)
    VALUES (@TokenId, @UserId, @ExpiresAt);
END
GO

-- Is this token still usable? Returns 1 (yes) or 0 (no)
CREATE OR ALTER PROCEDURE usp_IsSessionValid
    @TokenId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS Result
    FROM UserSessions
    WHERE TokenId = @TokenId
      AND IsLoggedOut = 0
      AND ExpiresAt > GETDATE();
END
GO

-- Logout: mark the token as finished
CREATE OR ALTER PROCEDURE usp_EndSession
    @TokenId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE UserSessions
    SET IsLoggedOut = 1
    WHERE TokenId = @TokenId;
END
GO

-- =============================================
-- DEPARTMENTS AND STUDENTS
-- =============================================

-- Department dropdown
CREATE OR ALTER PROCEDURE usp_GetDepartments
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DepartmentId, DeptCode, DepartmentName
    FROM Departments
    WHERE IsActive = 1
    ORDER BY DepartmentName;
END
GO

-- Add a student and create the roll number automatically
-- Example: CS + 2026 + 001 = CS2026001
CREATE OR ALTER PROCEDURE usp_AddStudent
    @StudentName   NVARCHAR(100),
    @DepartmentId  INT,
    @AdmissionYear INT,
    @YearOfStudy   INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;   -- if anything fails, the whole transaction is rolled back

    DECLARE @DeptCode   VARCHAR(5);
    DECLARE @NextNumber INT;
    DECLARE @RollNumber VARCHAR(12);

    SELECT @DeptCode = DeptCode
    FROM Departments
    WHERE DepartmentId = @DepartmentId AND IsActive = 1;

    IF @DeptCode IS NULL
        THROW 50001, 'Department not found or inactive.', 1;

    BEGIN TRANSACTION;

        -- Find the last number used for this department and year.
        -- UPDLOCK + HOLDLOCK: if two admins add a student at the same time,
        -- the second one waits, so both can never get the same number.
        SELECT @NextNumber = ISNULL(MAX(CAST(RIGHT(RollNumber, 3) AS INT)), 0) + 1
        FROM Students WITH (UPDLOCK, HOLDLOCK)
        WHERE DepartmentId = @DepartmentId
          AND AdmissionYear = @AdmissionYear;

        IF @NextNumber > 999
            THROW 50002, 'Roll numbers are full for this department and year.', 1;

        -- RIGHT('000' + '7', 3) gives '007'
        SET @RollNumber = @DeptCode
                        + CAST(@AdmissionYear AS VARCHAR(4))
                        + RIGHT('000' + CAST(@NextNumber AS VARCHAR(3)), 3);

        INSERT INTO Students (RollNumber, StudentName, DepartmentId, AdmissionYear, YearOfStudy)
        VALUES (@RollNumber, @StudentName, @DepartmentId, @AdmissionYear, @YearOfStudy);

    COMMIT TRANSACTION;

    SELECT @RollNumber AS RollNumber;
END
GO

-- Students for the selected Department and Year
-- TodayStatus is 'Present', 'Absent' or NULL (not marked yet)
CREATE OR ALTER PROCEDURE usp_GetStudents
    @DepartmentId INT,
    @YearOfStudy  INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT s.StudentId,
           s.RollNumber,
           s.StudentName,
           d.DepartmentName,
           s.YearOfStudy,
           a.AttendanceStatus AS TodayStatus
    FROM Students s
    INNER JOIN Departments d ON d.DepartmentId = s.DepartmentId
    LEFT JOIN Attendance a   ON a.StudentId = s.StudentId
                            AND a.AttendanceDate = CAST(GETDATE() AS DATE)
    WHERE s.DepartmentId = @DepartmentId
      AND s.YearOfStudy = @YearOfStudy
      AND s.IsActive = 1
    ORDER BY s.RollNumber;
END
GO

-- One student's details (for the Mark Attendance page)
CREATE OR ALTER PROCEDURE usp_GetStudentById
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT s.StudentId,
           s.RollNumber,
           s.StudentName,
           d.DepartmentName,
           s.YearOfStudy,
           s.IsActive
    FROM Students s
    INNER JOIN Departments d ON d.DepartmentId = s.DepartmentId
    WHERE s.StudentId = @StudentId;
END
GO

-- =============================================
-- ATTENDANCE
-- =============================================

-- Has this student's attendance already been taken today?
-- Returns 1 row if yes, 0 rows if no
CREATE OR ALTER PROCEDURE usp_GetTodayAttendance
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT a.AttendanceStatus,
           u.FullName  AS MarkedBy,
           a.CreatedAt AS MarkedAt
    FROM Attendance a
    INNER JOIN Users u ON u.UserId = a.MarkedByUserId
    WHERE a.StudentId = @StudentId
      AND a.AttendanceDate = CAST(GETDATE() AS DATE);
END
GO

-- Save today's attendance for one student
-- The date always comes from the server, never from the user
-- Result: 0 = saved, 1 = already submitted today
CREATE OR ALTER PROCEDURE usp_MarkAttendance
    @StudentId        INT,
    @AttendanceStatus VARCHAR(10),
    @MarkedByUserId   INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETDATE() AS DATE);

    IF EXISTS (SELECT 1 FROM Attendance
               WHERE StudentId = @StudentId AND AttendanceDate = @Today)
    BEGIN
        SELECT 1 AS Result;
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Attendance (StudentId, AttendanceDate, AttendanceStatus, MarkedByUserId)
        VALUES (@StudentId, @Today, @AttendanceStatus, @MarkedByUserId);

        SELECT 0 AS Result;
    END TRY
    BEGIN CATCH
        -- 2627 = the UNIQUE rule blocked it
        -- (happens if Submit is clicked twice at the same moment)
        IF ERROR_NUMBER() = 2627
        BEGIN
            SELECT 1 AS Result;
            RETURN;
        END;

        THROW;   -- any other error goes to the application
    END CATCH
END
GO

-- Dashboard numbers for today
CREATE OR ALTER PROCEDURE usp_GetTodaySummary
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS TotalStudents,
           SUM(CASE WHEN a.AttendanceStatus = 'Present' THEN 1 ELSE 0 END) AS PresentCount,
           SUM(CASE WHEN a.AttendanceStatus = 'Absent'  THEN 1 ELSE 0 END) AS AbsentCount,
           SUM(CASE WHEN a.AttendanceId IS NULL         THEN 1 ELSE 0 END) AS NotMarkedCount
    FROM Students s
    LEFT JOIN Attendance a ON a.StudentId = s.StudentId
                          AND a.AttendanceDate = CAST(GETDATE() AS DATE)
    WHERE s.IsActive = 1;
END
GO

PRINT 'Stored procedures created.';
