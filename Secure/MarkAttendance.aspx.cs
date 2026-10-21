using System;
using System.Web;
using System.Web.UI;
using Student_Att.Business;
using Student_Att.Models;
using Student_Att.Security;

namespace Student_Att
{
    public partial class MarkAttendancePage : Page
    {
        private readonly StudentService studentService = new StudentService();
        private readonly AttendanceService attendanceService = new AttendanceService();

        private int studentId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Only the StudentId comes from the URL. All other details are
            // loaded again from the database, so nobody can fake them in the URL.
            int.TryParse(Request.QueryString["id"], out studentId);
            lnkBack.NavigateUrl = GetListUrl();

            if (IsPostBack)
            {
                return;
            }

            if (LoadStudent())
            {
                CheckIfAlreadyMarked();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            TokenInfo user = CurrentUser.Get();
            string status = rblStatus.SelectedValue;

            MarkAttendanceResult result = attendanceService.MarkAttendance(studentId, status, user.UserId);

            if (result == MarkAttendanceResult.Saved)
            {
                // Post/Redirect/Get: going to another page means F5 cannot submit again
                Response.Redirect(GetListUrl()
                                  + "&savedRoll=" + HttpUtility.UrlEncode(lblRollNumber.Text)
                                  + "&savedStatus=" + status);
            }
            else
            {
                // Someone saved it a moment earlier (or a double click got through)
                CheckIfAlreadyMarked();
            }
        }

        // Fills the student details. Returns false if the student cannot be used.
        private bool LoadStudent()
        {
            Student student = studentService.GetStudentById(studentId);

            if (student == null || !student.IsActive)
            {
                lblMessage.CssClass = "alert alert-danger d-block";
                lblMessage.Text = "Student not found or no longer active.";
                pnlStudent.Visible = false;
                return false;
            }

            lblName.Text = Server.HtmlEncode(student.StudentName);
            lblRollNumber.Text = Server.HtmlEncode(student.RollNumber);
            lblDepartment.Text = Server.HtmlEncode(student.DepartmentName);
            lblYear.Text = student.YearName;
            lblDate.Text = DateTime.Today.ToString("dddd, dd MMMM yyyy");   // shown only; the database sets the real date
            return true;
        }

        // Duplicate check when the page opens
        private void CheckIfAlreadyMarked()
        {
            AttendanceRecord record = attendanceService.GetTodayAttendance(studentId);
            if (record == null)
            {
                return;   // not marked yet: the user can mark Present or Absent
            }

            pnlAlreadyMarked.Visible = true;
            lblExistingStatus.Text = Server.HtmlEncode(record.AttendanceStatus);
            lblMarkedBy.Text = Server.HtmlEncode(record.MarkedBy);
            lblMarkedAt.Text = record.MarkedAt.ToString("hh:mm tt");

            rblStatus.SelectedValue = record.AttendanceStatus;
            rblStatus.Enabled = false;
            btnSubmit.Enabled = false;

            ShowDuplicatePopup();
        }

        private void ShowDuplicatePopup()
        {
            // Runs in the browser once the page has loaded and opens the Bootstrap popup
            string script = "document.addEventListener('DOMContentLoaded', function () {"
                          + " new bootstrap.Modal(document.getElementById('duplicateModal')).show();"
                          + " });";

            ClientScript.RegisterStartupScript(GetType(), "DuplicatePopup", script, true);
        }

        // Link back to the student list with the same Department and Year selected
        private string GetListUrl()
        {
            return ResolveUrl("~/Secure/StudentList.aspx")
                   + "?dept=" + HttpUtility.UrlEncode(Request.QueryString["dept"])
                   + "&year=" + HttpUtility.UrlEncode(Request.QueryString["year"]);
        }
    }
}
