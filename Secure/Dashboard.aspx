<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Student_Att.DashboardPage" %>

<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">

    <div class="mb-4">
        <h2 class="page-title">Student Attendance Management System</h2>
        <p class="text-muted mb-0">
            Welcome, <strong><asp:Label ID="lblFullName" runat="server" /></strong>
            &middot; <asp:Label ID="lblToday" runat="server" />
        </p>
    </div>

    <%-- Today's numbers --%>
    <div class="row g-3 mb-4">
        <div class="col-6 col-lg-3">
            <div class="stat-card stat-total">
                <div class="stat-label">Active Students</div>
                <div class="stat-value"><asp:Label ID="lblTotal" runat="server" Text="0" /></div>
            </div>
        </div>
        <div class="col-6 col-lg-3">
            <div class="stat-card stat-present">
                <div class="stat-label">Present Today</div>
                <div class="stat-value"><asp:Label ID="lblPresent" runat="server" Text="0" /></div>
            </div>
        </div>
        <div class="col-6 col-lg-3">
            <div class="stat-card stat-absent">
                <div class="stat-label">Absent Today</div>
                <div class="stat-value"><asp:Label ID="lblAbsent" runat="server" Text="0" /></div>
            </div>
        </div>
        <div class="col-6 col-lg-3">
            <div class="stat-card stat-pending">
                <div class="stat-label">Not Marked Yet</div>
                <div class="stat-value"><asp:Label ID="lblNotMarked" runat="server" Text="0" /></div>
            </div>
        </div>
    </div>

    <%-- Navigation --%>
    <div class="row g-3">
        <div class="col-md-6">
            <div class="card shadow-sm h-100">
                <div class="card-body">
                    <h5>Attendance Management</h5>
                    <p class="text-muted">Choose a department and year, then mark each student Present or Absent.</p>
                    <a href="StudentList.aspx" class="btn btn-primary">Open Attendance</a>
                </div>
            </div>
        </div>

        <asp:PlaceHolder ID="phAdminCard" runat="server" Visible="false">
            <div class="col-md-6">
                <div class="card shadow-sm h-100">
                    <div class="card-body">
                        <h5>Manage Students</h5>
                        <p class="text-muted">Add new students. Roll numbers are created automatically.</p>
                        <a href="../Admin/ManageStudents.aspx" class="btn btn-outline-primary">Manage Students</a>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>

</asp:Content>
