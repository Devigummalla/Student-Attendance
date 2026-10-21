<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Student_Att.LoginPage" %>

<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center">
        <div class="col-md-6 col-lg-5">
            <div class="card shadow-sm">
                <div class="card-body p-4">
                    <h3 class="page-title">Login</h3>
                    <p class="text-muted">Use your username or email and your PIN.</p>

                    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />

                    <asp:Panel ID="pnlLogin" runat="server" DefaultButton="btnLogin">
                        <div class="mb-3">
                            <asp:Label runat="server" AssociatedControlID="txtLogin" CssClass="form-label" Text="Username or Email" />
                            <asp:TextBox ID="txtLogin" runat="server" CssClass="form-control" MaxLength="150" />
                            <asp:RequiredFieldValidator ID="rfvLogin" runat="server" ControlToValidate="txtLogin"
                                ErrorMessage="Username or email is required." CssClass="field-error" Display="Dynamic" />
                        </div>

                        <div class="mb-3">
                            <asp:Label runat="server" AssociatedControlID="txtPin" CssClass="form-label" Text="PIN" />
                            <asp:TextBox ID="txtPin" runat="server" CssClass="form-control" TextMode="Password"
                                MaxLength="6" inputmode="numeric" />
                            <asp:RequiredFieldValidator ID="rfvPin" runat="server" ControlToValidate="txtPin"
                                ErrorMessage="PIN is required." CssClass="field-error" Display="Dynamic" />
                        </div>

                        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary w-100"
                            OnClick="btnLogin_Click" />
                    </asp:Panel>

                    <p class="mt-3 mb-0 text-center">
                        New user? <a href="Signup.aspx">Create an account</a>
                    </p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
