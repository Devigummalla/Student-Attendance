using System;
using System.Web.UI;
using Student_Att.Business;
using Student_Att.Models;
using Student_Att.Security;

namespace Student_Att
{
    public partial class DashboardPage : Page
    {
        private readonly AttendanceService attendanceService = new AttendanceService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            TokenInfo user = CurrentUser.Get();
            lblFullName.Text = Server.HtmlEncode(user.FullName);
            lblToday.Text = DateTime.Today.ToString("dddd, dd MMMM yyyy");   // e.g. Friday, 09 October 2026
            phAdminCard.Visible = CurrentUser.IsAdmin();

            TodaySummary summary = attendanceService.GetTodaySummary();
            lblTotal.Text = summary.TotalStudents.ToString();
            lblPresent.Text = summary.PresentCount.ToString();
            lblAbsent.Text = summary.AbsentCount.ToString();
            lblNotMarked.Text = summary.NotMarkedCount.ToString();
        }
    }
}
