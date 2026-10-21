<%@ Page Title="Manage Students" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageStudents.aspx.cs" Inherits="Student_Att.ManageStudentsPage" %>

<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center">
        <div class="col-lg-7">
            <div class="card shadow-sm">
                <div class="card-body p-4">
                    <h3 class="page-title">Add Student</h3>
                    <p class="text-muted">The roll number is created automatically: Department code + Admission year + number (e.g. CS2026001).</p>

                    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />

                    <asp:Panel ID="pnlForm" runat="server" DefaultButton="btnAdd">
                        <div class="mb-3">
                            <asp:Label runat="server" AssociatedControlID="txtStudentName" CssClass="form-label" Text="Student Name" />
                            <asp:TextBox ID="txtStudentName" runat="server" CssClass="form-control" MaxLength="100" />
                            <asp:RequiredFieldValidator ID="rfvStudentName" runat="server" ControlToValidate="txtStudentName"
                                ErrorMessage="Student name is required." CssClass="field-error" Display="Dynamic" />
                        </div>

                        <div class="mb-3">
                            <asp:Label runat="server" AssociatedControlID="ddlDepartment" CssClass="form-label" Text="Department" />
                            <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-select" />
                            <asp:RequiredFieldValidator ID="rfvDepartment" runat="server" ControlToValidate="ddlDepartment"
                                InitialValue="0" ErrorMessage="Please select a department." CssClass="field-error" Display="Dynamic" />
                        </div>

                        <div class="row">
                            <div class="col-sm-6 mb-3">
                                <asp:Label runat="server" AssociatedControlID="ddlAdmissionYear" CssClass="form-label" Text="Admission Year" />
                                <asp:DropDownList ID="ddlAdmissionYear" runat="server" CssClass="form-select" />
                            </div>
                            <div class="col-sm-6 mb-3">
                                <asp:Label runat="server" AssociatedControlID="ddlYearOfStudy" CssClass="form-label" Text="Year of Study" />
                                <asp:DropDownList ID="ddlYearOfStudy" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="1" Text="First Year" />
                                    <asp:ListItem Value="2" Text="Second Year" />
                                    <asp:ListItem Value="3" Text="Third Year" />
                                    <asp:ListItem Value="4" Text="Fourth Year" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <asp:Button ID="btnAdd" runat="server" Text="Add Student" CssClass="btn btn-primary"
                            OnClick="btnAdd_Click" />
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
