using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Student_Att.DataAccess;
using Student_Att.Models;

namespace Student_Att.Business
{
    // Rules for departments and students
    public class StudentService
    {
        private readonly StudentRepository studentRepository = new StudentRepository();

        public List<Department> GetDepartments()
        {
            return studentRepository.GetDepartments();
        }

        public List<Student> GetStudents(int departmentId, int yearOfStudy)
        {
            if (departmentId <= 0)
            {
                throw new ArgumentException("Please select a department.");
            }
            if (yearOfStudy < 1 || yearOfStudy > 4)
            {
                throw new ArgumentException("Year must be between 1 and 4.");
            }

            return studentRepository.GetStudents(departmentId, yearOfStudy);
        }

        // Returns null when the student does not exist
        public Student GetStudentById(int studentId)
        {
            if (studentId <= 0)
            {
                return null;
            }
            return studentRepository.GetStudentById(studentId);
        }

        public ServiceResult AddStudent(string studentName, int departmentId, int admissionYear, int yearOfStudy)
        {
            studentName = (studentName ?? "").Trim();

            if (studentName.Length < 2 || studentName.Length > 100)
            {
                return ServiceResult.Fail("Please enter the student's full name (2 to 100 characters).");
            }
            if (departmentId <= 0)
            {
                return ServiceResult.Fail("Please select a department.");
            }
            if (yearOfStudy < 1 || yearOfStudy > 4)
            {
                return ServiceResult.Fail("Please select the year of study.");
            }
            if (admissionYear < 2000 || admissionYear > DateTime.Today.Year)
            {
                return ServiceResult.Fail("Please select a valid admission year.");
            }

            try
            {
                string rollNumber = studentRepository.AddStudent(studentName, departmentId, admissionYear, yearOfStudy);
                return ServiceResult.Ok("Student added successfully. Roll number: " + rollNumber);
            }
            catch (SqlException ex)
            {
                // 50001 and 50002 are our own errors thrown by usp_AddStudent
                if (ex.Number == 50001 || ex.Number == 50002)
                {
                    return ServiceResult.Fail(ex.Message);
                }
                throw;
            }
        }
    }
}
