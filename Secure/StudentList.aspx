<%@ Page Title="Attendance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentList.aspx.cs" Inherits="Student_Att.StudentListPage" %>

<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">

    <div class="mb-3">
        <h3 class="page-title">Attendance Management</h3>
        <p class="text-muted mb-0">Today: <asp:Label ID="lblToday" runat="server" /></p>
    </div>

    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />

    <%-- Filters at the top of the page --%>
    <asp:Panel ID="pnlFilters" runat="server" CssClass="card shadow-sm mb-4" DefaultButton="btnShow">
        <div class="card-body">
            <div class="row g-3 align-items-end">
                <div class="col-md-5">
                    <asp:Label runat="server" AssociatedControlID="ddlDepartment" CssClass="form-label" Text="Department" />
                    <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-select" />
                    <asp:RequiredFieldValidator ID="rfvDepartment" runat="server" ControlToValidate="ddlDepartment"
                        InitialValue="0" ErrorMessage="Please select a department." CssClass="field-error" Display="Dynamic" />
                </div>

                <div class="col-md-4">
                    <asp:Label runat="server" AssociatedControlID="ddlYear" CssClass="form-label" Text="Year" />
                    <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-select">
                        <asp:ListItem Value="0" Text="-- Select Year --" />
                        <asp:ListItem Value="1" Text="First Year" />
                        <asp:ListItem Value="2" Text="Second Year" />
                        <asp:ListItem Value="3" Text="Third Year" />
                        <asp:ListItem Value="4" Text="Fourth Year" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvYear" runat="server" ControlToValidate="ddlYear"
                        InitialValue="0" ErrorMessage="Please select a year." CssClass="field-error" Display="Dynamic" />
                </div>

                <div class="col-md-3">
                    <asp:Button ID="btnShow" runat="server" Text="Show Students" CssClass="btn btn-primary w-100"
                        OnClick="btnShow_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <%-- Student list (shown after a Department and Year are chosen) --%>
    <asp:Panel ID="pnlResults" runat="server" Visible="false">
        <h5 class="mb-3"><asp:Label ID="lblResultTitle" runat="server" /></h5>

        <div class="table-responsive">
            <asp:GridView ID="gvStudents" runat="server" AutoGenerateColumns="false" GridLines="None"
                CssClass="table table-hover align-middle student-table shadow-sm">
                <Columns>
                    <asp:TemplateField HeaderText="Student Name">
                        <ItemTemplate>
                            <a href="<%# GetMarkAttendanceUrl(Eval("StudentId")) %>"><%# Server.HtmlEncode(Eval("StudentName").ToString()) %></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="RollNumber" HeaderText="Roll Number" />
                    <asp:BoundField DataField="DepartmentName" HeaderText="Department" />
                    <asp:BoundField DataField="YearName" HeaderText="Year" />
                    <asp:TemplateField HeaderText="Today">
                        <ItemTemplate>
                            <%# GetStatusBadge(Eval("TodayStatus")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Attendance">
                        <ItemTemplate>
                            <a class="btn btn-sm btn-outline-primary" href="<%# GetMarkAttendanceUrl(Eval("StudentId")) %>">Mark Attendance</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="alert alert-warning mb-0">No students found for the selected Department and Year.</div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </asp:Panel>

</asp:Content>
