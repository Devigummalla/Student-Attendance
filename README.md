# Student Attendance Management System

This is my mini project for the .NET training. It is an ASP.NET Web Forms application where a logged-in user (Admin or Faculty) selects a department and year, sees the students, and marks each student as Present or Absent for the day. Attendance can be submitted only once per student per day.

## Technologies used

- ASP.NET Web Forms (.NET Framework 4.7.2)
- C#
- Entity Framework Core 3.1.32 (database first) with LINQ
- SQL Server (LocalDB / SQL Server Express)
- Bootstrap 5 (the copy that comes with the Visual Studio template, so it works offline)
- JWT for login, stored in an HttpOnly cookie

EF Core 3.1 is used because it is the last EF Core version that supports .NET Framework.

## Prerequisites

You need these installed before running the project:

1. Windows 10 or 11
2. **Visual Studio 2022 version 17.13 or later** (Community is fine) with the **ASP.NET and web development** workload.
   The solution file is `.slnx`, which older versions cannot open. To check your version: Help > About Microsoft Visual Studio.
3. .NET Framework 4.7.2 targeting pack (installed with the workload above)
4. SQL Server LocalDB (installed with Visual Studio) or SQL Server Express
5. SQL Server Management Studio (SSMS) to run the database scripts
6. A browser (I tested with Microsoft Edge)

## Folder structure

```
Student-Att/                          <- repository root
├── Student-Att.slnx                  <- open this in Visual Studio
├── README.md
├── .gitignore
├── Database/                         <- SQL scripts (run these first)
├── Docs/                             <- technical documentation
├── Screenshots/                      <- screenshots of every page and test case
└── Student-Att/                      <- the web project
    ├── Account/                      Login and Sign Up pages
    ├── Secure/                       Dashboard, Student List, Mark Attendance (login required)
    ├── Admin/                        Manage Students (Admin role only)
    ├── Models/                       Classes like User, Student, Department
    ├── Entities/                     EF Core classes generated from the database + AttendanceDbContext
    ├── DataAccess/                   Repositories that query the database with EF Core (LINQ)
    ├── Business/                     Validation and rules
    ├── Security/                     PIN hashing, JWT, login cookie
    ├── Logging/                      Writes errors to App_Data/Logs
    ├── Content/, Scripts/            CSS and Bootstrap
    ├── Site.Master                   Common layout (left menu, logout)
    ├── Web.config
    ├── ConnectionStrings.sample.config
    └── Secrets.sample.config
```

## Setup steps

### Step 1: Get the code

Clone the repository (or unzip it) into any folder, for example `C:\Projects\Student-Att`.

### Step 2: Create the database

1. Open SSMS and connect to the server:
   - LocalDB: `(localdb)\MSSQLLocalDB`
   - SQL Express: `.\SQLEXPRESS`
2. Open each script from the `Database` folder and run it (F5) in this order:

| Script | What it does |
|---|---|
| `01_CreateDatabase.sql` | Creates the `StudentAttendanceDB` database and the five tables |
| `02_StoredProcedures.sql` | Creates the stored procedures. The app now uses EF Core, but `03_SeedData.sql` still uses `usp_AddStudent` to create roll numbers, so run this script |
| `03_SeedData.sql` | Adds 5 departments, 39 students and one admin user |
| `04_TestQueries.sql` | Optional. Tests the database rules and rolls everything back |

3. Check that it worked:

```sql
USE StudentAttendanceDB;
SELECT COUNT(*) AS Departments FROM Departments;   -- should be 5
SELECT COUNT(*) AS Students FROM Students;          -- should be 39
SELECT Username, Role FROM Users;                   -- should show admin / Admin
```

If you need to start the database again from zero, run this first and then repeat the scripts:

```sql
USE master;
ALTER DATABASE StudentAttendanceDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE StudentAttendanceDB;
```

### Step 3: Create the two config files

These two files are not in Git because they contain machine-specific settings and a secret key. Sample versions are included. Both go in the **web project folder** (`Student-Att\Student-Att\`, the same folder as `Web.config`).

**a) Connection string**

Copy `ConnectionStrings.sample.config` and rename the copy to `ConnectionStrings.config`. The connection string is named `AttendanceDb`, and `AttendanceDbContext` reads it from here.

If you use LocalDB, nothing needs to change. If you use SQL Express, change the `Data Source`:

```xml
<!-- LocalDB (default) -->
connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=StudentAttendanceDB;Integrated Security=True"

<!-- SQL Express -->
connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=StudentAttendanceDB;Integrated Security=True"
```

**b) JWT secret key**

Copy `Secrets.sample.config` and rename the copy to `Secrets.config`. Replace the value with any random text of **at least 32 characters**. This key signs the login token.

```xml
<add key="JwtSecret" value="any-long-random-text-of-at-least-32-characters" />
```

`Web.config` already points to both files, so it does not need any change.

### Step 4: Build

1. Double-click `Student-Att.slnx` to open it in Visual Studio.
2. Press **Ctrl + Shift + B**. On the first build Visual Studio downloads the NuGet packages (including EF Core 3.1.32), which can take a minute.
   - If it says packages are missing: right-click the solution > **Restore NuGet Packages**, then build again.
3. The Output window should end with `Build: 1 succeeded`.

You may see 4 yellow **NU1902** warnings. They come from older libraries that EF Core 3.1 depends on. They are warnings, not errors, and the app still runs.

The EF Core classes in `Entities/` are already in the project, so you do not need to scaffold again. If the database changes later, regenerate them in **Tools > NuGet Package Manager > Package Manager Console**:

```
Scaffold-DbContext "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=StudentAttendanceDB;Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities -Context AttendanceDbContext -Tables Users,UserSessions,Departments,Students,Attendance -Force
```

After scaffolding, change `OnConfiguring` in `AttendanceDbContext` back to read the connection string from `ConfigurationManager.ConnectionStrings["AttendanceDb"]`, because the generated version hard-codes it.

### Step 5: Run

1. In Solution Explorer, right-click `Default.aspx` > **Set As Start Page**.
2. Press **F5**.
3. The first time, Visual Studio may ask to trust the IIS Express SSL certificate. Click **Yes** both times.
4. The browser opens at `https://localhost:44338/` and shows the Login page.

## Login details

A default admin account is created by `03_SeedData.sql`:

- Username: `admin`
- PIN: `2026`

You can also create a new account from the Sign Up page. New accounts get the **Faculty** role.

| Role | What they can do |
|---|---|
| Faculty | Login, view dashboard, filter students, mark attendance |
| Admin | Everything Faculty can do, plus add new students (Manage Students page) |

To make any account an Admin, run this in SSMS and then log in again:

```sql
UPDATE Users SET Role = 'Admin' WHERE Username = 'your-username';
```

## How to use

1. Log in with your username or email and PIN.
2. The dashboard shows today's date and today's counts (present, absent, not marked).
3. Click **Mark Attendance** in the left menu.
4. Select a Department and a Year, then click **Show Students**.
5. Click a student's name or the **Mark Attendance** button.
6. The student details and today's date are filled in automatically. Select Present or Absent and click **Submit Attendance**.
7. If attendance was already submitted for that student today, a popup shows "Attendance Already Submitted for today" and the form is disabled.
8. Click **Logout** in the left menu when you are done.

## How duplicate attendance is prevented

I handled this in four places so it cannot be bypassed:

1. When the Mark Attendance page opens, it checks if a record already exists for today. If yes, it shows the popup and disables the Submit button.
2. The Submit button disables itself after the first click, so double clicking does not send two requests.
3. `AttendanceRepository` checks again just before saving the new record.
4. The `UNIQUE (StudentId, AttendanceDate)` constraint in the database blocks a second record even if two requests arrive together or someone inserts directly with SQL. EF Core raises a `DbUpdateException` (SQL error 2627), which the app catches and shows as the same popup.

The attendance date is always taken from the server (`DateTime.Today`), not from the user.

## Security notes

- PINs are never stored as plain text. They are hashed with PBKDF2 (SHA-256, 100,000 iterations, random salt).
- After login a JWT is created and saved in an HttpOnly cookie. Every request checks the token signature, expiry, and whether the user has logged out.
- On logout the token is marked as logged out in the `UserSessions` table, so the same token cannot be reused.
- Pages under `/Secure` need login. Pages under `/Admin` need the Admin role. Both are set in `Web.config`.
- All database access goes through EF Core LINQ queries, which are sent as parameterised SQL (no SQL injection).
- The connection string and JWT secret are kept out of Git (`.gitignore`).
- Errors are written to `App_Data/Logs` and the user only sees a simple error page.

## Testing

I tested all the scenarios from the assignment (TC01 to TC16). Screenshots are in the `Screenshots` folder, and each test case is described in the technical documentation in `Docs`.

A few quick ones to try:

- Mark the same student twice on the same day: the popup appears and no second record is saved.
- Select **Civil** and **Fourth Year**: shows "No students found for the selected Department and Year."
- Log out, then open `https://localhost:44338/Secure/Dashboard.aspx` directly: you are sent back to the Login page.
- Log in as a Faculty user and open `https://localhost:44338/Admin/ManageStudents.aspx`: shows Access Denied.



## Common problems

| Problem | Fix |
|---|---|
| Visual Studio cannot open `Student-Att.slnx` | Update Visual Studio 2022 to 17.13 or later |
| Build error about missing NuGet packages | Right-click the solution > Restore NuGet Packages, then build again |
| Yellow NU1902 warnings after build | Expected with EF Core 3.1; they do not stop the app |
| "Unable to load DLL Microsoft.Data.SqlClient.SNI" | Add `<hostingEnvironment shadowCopyBinAssemblies="false" />` inside `<system.web>` in `Web.config`, then rebuild |
| "Unable to open configSource file 'ConnectionStrings.config'" | Step 3a was skipped, or the file is not in the same folder as `Web.config` |
| "JwtSecret is missing or shorter than 32 characters" | Step 3b was skipped, or the secret is too short |
| "A network-related or instance-specific error" / cannot connect | The server name in `ConnectionStrings.config` does not match your SQL Server, or the database scripts were not run |
| "Cannot open database StudentAttendanceDB" | Run the scripts in Step 2 |
| `CREATE DATABASE` fails because the database already exists | Drop it first (see the end of Step 2) |
| Page looks unstyled | Press Ctrl + F5 in the browser to clear the cached CSS |
| Still logged in after restarting the app | This is expected. The login cookie stays until Logout or until it expires after 60 minutes |
