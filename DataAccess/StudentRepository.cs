using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Student_Att.Models;

namespace Student_Att.DataAccess
{
    // Database work for Departments and Students
    public class StudentRepository
    {
        public List<Department> GetDepartments()
        {
            List<Department> departments = new List<Department>();

            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_GetDepartments", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Department department = new Department();
                        department.DepartmentId = Convert.ToInt32(reader["DepartmentId"]);
                        department.DeptCode = reader["DeptCode"].ToString();
                        department.DepartmentName = reader["DepartmentName"].ToString();
                        departments.Add(department);
                    }
                }
            }

            return departments;
        }

        public List<Student> GetStudents(int departmentId, int yearOfStudy)
        {
            List<Student> students = new List<Student>();

            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_GetStudents", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = departmentId;
                command.Parameters.Add("@YearOfStudy", SqlDbType.Int).Value = yearOfStudy;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Student student = new Student();
                        student.StudentId = Convert.ToInt32(reader["StudentId"]);
                        student.RollNumber = reader["RollNumber"].ToString();
                        student.StudentName = reader["StudentName"].ToString();
                        student.DepartmentName = reader["DepartmentName"].ToString();
                        student.YearOfStudy = Convert.ToInt32(reader["YearOfStudy"]);
                        student.IsActive = true;

                        // TodayStatus is NULL in the database when attendance is not marked yet
                        if (reader["TodayStatus"] == DBNull.Value)
                        {
                            student.TodayStatus = null;
                        }
                        else
                        {
                            student.TodayStatus = reader["TodayStatus"].ToString();
                        }

                        students.Add(student);
                    }
                }
            }

            return students;
        }

        // Returns null when the student does not exist
        public Student GetStudentById(int studentId)
        {
            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_GetStudentById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    Student student = new Student();
                    student.StudentId = Convert.ToInt32(reader["StudentId"]);
                    student.RollNumber = reader["RollNumber"].ToString();
                    student.StudentName = reader["StudentName"].ToString();
                    student.DepartmentName = reader["DepartmentName"].ToString();
                    student.YearOfStudy = Convert.ToInt32(reader["YearOfStudy"]);
                    student.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    return student;
                }
            }
        }

        // Returns the new roll number, for example CS2026004
        public string AddStudent(string studentName, int departmentId, int admissionYear, int yearOfStudy)
        {
            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_AddStudent", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@StudentName", SqlDbType.NVarChar, 100).Value = studentName;
                command.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = departmentId;
                command.Parameters.Add("@AdmissionYear", SqlDbType.Int).Value = admissionYear;
                command.Parameters.Add("@YearOfStudy", SqlDbType.Int).Value = yearOfStudy;

                connection.Open();
                return Convert.ToString(command.ExecuteScalar());
            }
        }
    }
}
