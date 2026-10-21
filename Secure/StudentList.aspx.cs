using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Student_Att.Business;
using Student_Att.Models;

namespace Student_Att
{
    public partial class StudentListPage : Page
    {
        private readonly StudentService studentService = new StudentService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            lblToday.Text = DateTime.Today.ToString("dddd, dd MMMM yyyy");
            LoadDepartments();
            ShowSavedMessage();

            // The filters live in the URL (?dept=1&year=2), so the
            // Back/Cancel button on the next page brings us back to the same list
            int departmentId;
            int year;
            bool hasDepartment = int.TryParse(Request.QueryString["dept"], out departmentId);
            bool hasYear = int.TryParse(Request.QueryString["year"], out year);

            if (hasDepartment && hasYear)
            {
                SelectValue(ddlDepartment, departmentId.ToString());
                SelectValue(ddlYear, year.ToString());
                LoadStudents(departmentId, year);
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            // Reload the page with the filters in the URL
            Response.Redirect("StudentList.aspx?dept=" + ddlDepartment.SelectedValue + "&year=" + ddlYear.SelectedValue);
        }

        private void LoadDepartments()
        {
            ddlDepartment.DataSource = studentService.GetDepartments();
            ddlDepartment.DataTextField = "DepartmentName";
            ddlDepartment.DataValueField = "DepartmentId";
            ddlDepartment.DataBind();

            // First item asks the user to choose
            ddlDepartment.Items.Insert(0, new ListItem("-- Select Department --", "0"));
        }

        private void LoadStudents(int departmentId, int year)
        {
            if (year < 1 || year > 4 || departmentId <= 0)
            {
                return;
            }

            List<Student> students = studentService.GetStudents(departmentId, year);

            lblResultTitle.Text = Server.HtmlEncode(ddlDepartment.SelectedItem.Text) + " - "
                                  + YearOfStudyHelper.GetName(year)
                                  + " (" + students.Count + " students)";

            gvStudents.DataSource = students;
            gvStudents.DataBind();     // an empty list shows the EmptyDataTemplate message
            pnlResults.Visible = true;
        }

        // Message after coming back from the Mark Attendance page
        private void ShowSavedMessage()
        {
            string roll = Request.QueryString["savedRoll"];
            string status = Request.QueryString["savedStatus"];

            if (string.IsNullOrEmpty(roll) || (status != "Present" && status != "Absent"))
            {
                return;
            }

            lblMessage.CssClass = "alert alert-success d-block";
            lblMessage.Text = "Attendance saved successfully: " + Server.HtmlEncode(roll) + " marked " + status + ".";
        }

        // Selects an item only if it exists (a wrong value in the URL must not crash the page)
        private void SelectValue(DropDownList list, string value)
        {
            if (list.Items.FindByValue(value) != null)
            {
                list.SelectedValue = value;
            }
        }

        // ----- Used by the GridView in the .aspx -----

        protected string GetMarkAttendanceUrl(object studentId)
        {
            return ResolveUrl("~/Secure/MarkAttendance.aspx")
                   + "?id=" + studentId
                   + "&dept=" + HttpUtility.UrlEncode(ddlDepartment.SelectedValue)
                   + "&year=" + HttpUtility.UrlEncode(ddlYear.SelectedValue);
        }

        protected string GetStatusBadge(object status)
        {
            string value = status as string;

            if (value == "Present")
            {
                return "<span class=\"badge bg-success\">Present</span>";
            }
            if (value == "Absent")
            {
                return "<span class=\"badge bg-danger\">Absent</span>";
            }
            return "<span class=\"badge bg-secondary\">Not marked</span>";
        }
    }
}
