# Architecture, Database Design and Duplicate Prevention

This document briefly explains how I structured the Student Attendance Management System, how the database is designed, and how duplicate attendance is prevented.

## 1. Application architecture

I used a layered (3-tier) structure so that the UI, the business rules and the database code are kept separate. Each layer only talks to the layer directly below it.

```
Browser
   |
   v
Presentation layer   ->  Web Forms pages (.aspx + code-behind), Site.Master
   |
   v
Business layer       ->  AuthService, StudentService, AttendanceService
   |
   v
Data access layer    ->  Repositories using ADO.NET (SqlConnection, SqlCommand)
   |
   v
SQL Server           ->  Tables, constraints, stored procedures
```

| Folder | Layer | Responsibility |
|---|---|---|
| `Account/`, `Secure/`, `Admin/`, `Site.Master` | Presentation | Show data, read user input, client-side validators |
| `Business/` | Business | Server-side validation and rules (PIN format, allowed status, filters) |
| `DataAccess/` | Data access | Call stored procedures with parameters, convert rows into objects |
| `Models/` | Shared | Plain classes such as `User`, `Student`, `Department` |
| `Security/` | Cross-cutting | PIN hashing, JWT, login cookie, request check |
| `Logging/` | Cross-cutting | Writes errors to `App_Data/Logs` |

Pages never contain SQL, and the data access classes never touch UI controls.

### Why ADO.NET and stored procedures

The assignment allows parameterized SQL or an ORM. I chose ADO.NET with stored procedures because most of the important rules in this project live in the database (unique constraints, check constraints, transactions). With ADO.NET I can see and control every query, and using parameters in every call prevents SQL injection.

### Authentication and roles (JWT)

1. On login, the PIN is checked against the stored PBKDF2 hash.
2. If it is correct, a JWT is created with the user id, name, role, a unique token id (`jti`) and an expiry of 60 minutes. It is signed with HMAC-SHA256 using a secret key from `Secrets.config`.
3. The token is stored in an HttpOnly, SameSite=Strict cookie. Web Forms pages and postbacks send cookies automatically, and JavaScript cannot read this one.
4. `JwtAuthModule` (an `IHttpModule`) runs on every request. It checks the signature, the expiry, and the `UserSessions` table to see if the user has logged out. Then it sets the current user and role.
5. `Web.config` rules protect the folders: `/Secure` needs a logged-in user, and `/Admin` needs the Admin role. Blocked users are sent to the Login page or to Access Denied.
6. On logout, the token is marked as logged out in `UserSessions`, so the same token cannot be used again.

| Role | Can do |
|---|---|
| Faculty (default on sign up) | Dashboard, filter students, mark attendance |
| Admin (created only in the database) | Everything Faculty can do, plus add students |

## 2. Database design

Database: `StudentAttendanceDB` (SQL Server)

```
Departments 1 ---- * Students 1 ---- * Attendance * ---- 1 Users 1 ---- * UserSessions
```

| Table | Key columns | Notes |
|---|---|---|
| `Users` | UserId (PK), FullName, Email, Username, PinHash, Role, IsActive, CreatedAt | Email and Username are UNIQUE. Role is `Admin` or `Faculty`. Only the PIN hash is stored. |
| `UserSessions` | SessionId (PK), TokenId (UNIQUE), UserId (FK), ExpiresAt, IsLoggedOut | One row per login. Used to revoke a JWT on logout. |
| `Departments` | DepartmentId (PK), DeptCode (UNIQUE), DepartmentName (UNIQUE), IsActive | DeptCode (CS, IT, EC, ME, CE) is used in roll numbers. |
| `Students` | StudentId (PK), RollNumber (UNIQUE), StudentName, DepartmentId (FK), AdmissionYear, YearOfStudy, IsActive | YearOfStudy must be 1 to 4. Index on (DepartmentId, YearOfStudy) for the filter. |
| `Attendance` | AttendanceId (PK), StudentId (FK), AttendanceDate, AttendanceStatus, MarkedByUserId (FK), CreatedAt | Status must be `Present` or `Absent`. UNIQUE (StudentId, AttendanceDate). |

### Roll number design

Roll numbers are generated automatically by `usp_AddStudent`:

```
DeptCode + AdmissionYear + 3-digit number      e.g.  CS2026001
```

I used the admission year and not the year of study because the year of study changes every year, but a student's ID should never change. The procedure finds the last number for that department and year inside a transaction with `UPDLOCK, HOLDLOCK`, so two students added at the same moment cannot get the same number. The UNIQUE constraint on RollNumber is the final safety check.

### Stored procedures

| Area | Procedures |
|---|---|
| Users | `usp_RegisterUser`, `usp_GetUserForLogin` |
| Sessions | `usp_SaveSession`, `usp_IsSessionValid`, `usp_EndSession` |
| Students | `usp_GetDepartments`, `usp_GetStudents`, `usp_GetStudentById`, `usp_AddStudent` |
| Attendance | `usp_GetTodayAttendance`, `usp_MarkAttendance`, `usp_GetTodaySummary` |

## 3. How duplicate attendance is prevented

The rule is: one attendance record per student per date. I did not rely on only one check. There are four layers, and the last one is in the database itself.

| # | Where | What happens |
|---|---|---|
| 1 | Page load (`MarkAttendance.aspx.cs`) | When the page opens, `usp_GetTodayAttendance` checks for today's record. If one exists, the popup **"Attendance Already Submitted for today"** is shown, and the radio buttons and Submit button are disabled. |
| 2 | Browser (JavaScript) | After the first click, the Submit button is disabled and shows "Saving...", so a double click cannot send two requests. |
| 3 | Stored procedure (`usp_MarkAttendance`) | Before inserting, `IF EXISTS` checks again and returns "already submitted" if a record is found. |
| 4 | Database constraint | `UNIQUE (StudentId, AttendanceDate)` on the Attendance table. Even if two requests arrive at the same moment, or someone inserts directly with SQL, the second insert fails. The procedure catches this error (2627) and returns "already submitted", so the user still sees the same popup. |

Two more points:

- The attendance date is never sent from the browser. The stored procedure uses the server date (`CAST(GETDATE() AS DATE)`), so the user cannot change it.
- After a successful save, the page redirects back to the student list (Post/Redirect/Get). Pressing F5 afterwards does not submit the form again.

Example (from the assignment):

| Student | Date | Status | Result |
|---|---|---|---|
| Rahul Sharma | 9 Oct 2026 | Present | Saved |
| Rahul Sharma | 9 Oct 2026 | Absent | Blocked: popup shown, no new row |
| Priya Verma | 9 Oct 2026 | Absent | Saved |
| Rahul Sharma | 10 Oct 2026 | Absent | Saved (different date) |
