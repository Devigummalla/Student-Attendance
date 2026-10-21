<%@ Page Title="Access Denied" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="Student_Att.AccessDeniedPage" %>

<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center">
        <div class="col-md-7">
            <div class="card shadow-sm text-center">
                <div class="card-body p-5">
                    <h3 class="page-title text-danger">Access Denied</h3>
                    <p class="text-muted">You do not have permission to open this page. It is available to administrators only.</p>
                    <a href="Secure/Dashboard.aspx" class="btn btn-primary">Back to Dashboard</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
