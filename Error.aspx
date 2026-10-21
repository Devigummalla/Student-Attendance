<%@ Page Title="Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="Student_Att.ErrorPage" %>

<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center">
        <div class="col-md-7">
            <div class="card shadow-sm text-center">
                <div class="card-body p-5">
                    <h3 class="page-title">Something went wrong</h3>
                    <p class="text-muted">An unexpected error occurred. It has been logged. Please try again in a moment.</p>
                    <a href="Default.aspx" class="btn btn-primary">Go to Home</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
