using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Student_Att.Business;

namespace Student_Att
{
    // Only Admins can open this page (see the <location path="Admin"> rule in Web.config)
    public partial class ManageStudentsPage : Page
    {
        private readonly StudentService studentService = new StudentService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            ddlDepartment.DataSource = studentService.GetDepartments();
            ddlDepartment.DataTextField = "DepartmentName";
            ddlDepartment.DataValueField = "DepartmentId";
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem("-- Select Department --", "0"));

            // This year and the 3 years before it (for 1st to 4th year students)
            int thisYear = DateTime.Today.Year;
            for (int year = thisYear; year >= thisYear - 3; year--)
            {
                ddlAdmissionYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            ServiceResult result = studentService.AddStudent(
                txtStudentName.Text,
                int.Parse(ddlDepartment.SelectedValue),
                int.Parse(ddlAdmissionYear.SelectedValue),
                int.Parse(ddlYearOfStudy.SelectedValue));

            if (result.Success)
            {
                lblMessage.CssClass = "alert alert-success d-block";
                txtStudentName.Text = "";
            }
            else
            {
                lblMessage.CssClass = "alert alert-danger d-block";
            }

            lblMessage.Text = Server.HtmlEncode(result.Message);
        }
    }
}
