using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Student_Att.Entities;
using Student_Att.Models;

namespace Student_Att.DataAccess
{
    // Database work for Departments and Students (EF Core + LINQ)
    public class StudentRepository
    {
        // Note: IsActive is "bool?" in the scaffolded entities (the column has a DEFAULT),
        // so we compare with "== true" in the queries below.
        public List<Department> GetDepartments()
        {
            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                return db.Departments
                         .Where(d => d.IsActive == true)
                         .OrderBy(d => d.DepartmentName)
                         .Select(d => new Department
                         {
                             DepartmentId = d.DepartmentId,
                             DeptCode = d.DeptCode,
                             DepartmentName = d.DepartmentName
                         })
                         .ToList();
            }
        }

        // Students of one Department + Year, with today's attendance status
        public List<Student> GetStudents(int departmentId, int yearOfStudy)
        {
            DateTime today = DateTime.Today;

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                var query = from s in db.Students
                            join d in db.Departments on s.DepartmentId equals d.DepartmentId
                            where s.DepartmentId == departmentId
                               && s.YearOfStudy == yearOfStudy
                               && s.IsActive == true
                            orderby s.RollNumber
                            select new Student
                            {
                                StudentId = s.StudentId,
                                RollNumber = s.RollNumber,
                                StudentName = s.StudentName,
                                DepartmentName = d.DepartmentName,
                                YearOfStudy = s.YearOfStudy,
                                IsActive = s.IsActive == true,
                                // "Present", "Absent", or null when not marked today
                                TodayStatus = db.Attendance
                                                .Where(a => a.StudentId == s.StudentId && a.AttendanceDate == today)
                                                .Select(a => a.AttendanceStatus)
                                                .FirstOrDefault()
                            };

                return query.ToList();     // the SQL runs here
            }
        }

        // Returns null when the student does not exist
        public Student GetStudentById(int studentId)
        {
            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                var query = from s in db.Students
                            join d in db.Departments on s.DepartmentId equals d.DepartmentId
                            where s.StudentId == studentId
                            select new Student
                            {
                                StudentId = s.StudentId,
                                RollNumber = s.RollNumber,
                                StudentName = s.StudentName,
                                DepartmentName = d.DepartmentName,
                                YearOfStudy = s.YearOfStudy,
                                IsActive = s.IsActive == true
                            };

                return query.FirstOrDefault();
            }
        }

        // Adds a student and returns the new roll number, for example CS2026004
        public string AddStudent(string studentName, int departmentId, int admissionYear, int yearOfStudy)
        {
            // Try up to 3 times: if two admins add a student at the same moment, both may pick
            // the same number. The UNIQUE rule on RollNumber rejects one, and it simply tries again.
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                using (AttendanceDbContext db = new AttendanceDbContext())
                {
                    Departments department = db.Departments
                                               .FirstOrDefault(d => d.DepartmentId == departmentId && d.IsActive == true);
                    if (department == null)
                    {
                        throw new ArgumentException("Department not found or inactive.");
                    }

                    int nextNumber = GetNextNumber(db, departmentId, admissionYear);
                    if (nextNumber > 999)
                    {
                        throw new InvalidOperationException("Roll numbers are full for this department and year.");
                    }

                    // "CS" + "2026" + "004"
                    string rollNumber = department.DeptCode + admissionYear + nextNumber.ToString("000");

                    Students student = new Students();
                    student.RollNumber = rollNumber;
                    student.StudentName = studentName;
                    student.DepartmentId = departmentId;
                    student.AdmissionYear = admissionYear;
                    student.YearOfStudy = yearOfStudy;
                    student.IsActive = true;

                    db.Students.Add(student);

                    try
                    {
                        db.SaveChanges();
                        return rollNumber;
                    }
                    catch (DbUpdateException ex)
                    {
                        if (!DbErrorHelper.IsDuplicateKey(ex))
                        {
                            throw;      // a different problem: do not hide it
                        }
                        // same roll number taken a moment ago: loop and try the next one
                    }
                }
            }

            throw new InvalidOperationException("Could not create a roll number. Please try again.");
        }

        // Highest last-3-digits used for this department + admission year, plus 1
        private int GetNextNumber(AttendanceDbContext db, int departmentId, int admissionYear)
        {
            List<string> rollNumbers = db.Students
                                         .Where(s => s.DepartmentId == departmentId && s.AdmissionYear == admissionYear)
                                         .Select(s => s.RollNumber)
                                         .ToList();

            int highest = 0;
            foreach (string roll in rollNumbers)
            {
                int number = int.Parse(roll.Substring(roll.Length - 3));   // "CS2026004" -> 4
                if (number > highest)
                {
                    highest = number;
                }
            }

            return highest + 1;
        }
    }
}
