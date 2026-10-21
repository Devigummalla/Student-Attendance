<%@ Page Title="Sign Up" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="Student_Att.SignupPage" %>

<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center">
        <div class="col-md-8 col-lg-6">
            <div class="card shadow-sm">
                <div class="card-body p-4">
                    <h3 class="page-title">Create an account</h3>
                    <p class="text-muted">Register once, then log in with your username or email and your PIN.</p>

                    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />

                    <%-- Registration form --%>
                    <asp:Panel ID="pnlForm" runat="server" DefaultButton="btnSignup">
                        <div class="mb-3">
                            <asp:Label runat="server" AssociatedControlID="txtFullName" CssClass="form-label" Text="Full Name" />
                            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" MaxLength="100" />
                            <asp:RequiredFieldValidator ID="rfvFullName" runat="server" ControlToValidate="txtFullName"
                                ErrorMessage="Full name is required." CssClass="field-error" Display="Dynamic" />
                        </div>

                        <div class="mb-3">
                            <asp:Label runat="server" AssociatedControlID="txtEmail" CssClass="form-label" Text="Email Address" />
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="150" />
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                                ErrorMessage="Email is required." CssClass="field-error" Display="Dynamic" />
                            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                ErrorMessage="Please enter a valid email address." CssClass="field-error" Display="Dynamic" />
                        </div>

                        <div class="mb-3">
                            <asp:Label runat="server" AssociatedControlID="txtUsername" CssClass="form-label" Text="Username" />
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" MaxLength="50" />
                            <div class="form-text">3 to 50 characters: letters, numbers, dot or underscore.</div>
                            <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername"
                                ErrorMessage="Username is required." CssClass="field-error" Display="Dynamic" />
                            <asp:RegularExpressionValidator ID="revUsername" runat="server" ControlToValidate="txtUsername"
                                ValidationExpression="^[A-Za-z0-9_.]{3,50}$"
                                ErrorMessage="Username can only use letters, numbers, dot or underscore (3 to 50 characters)."
                                CssClass="field-error" Display="Dynamic" />
                        </div>

                        <div class="row">
                            <div class="col-sm-6 mb-3">
                                <asp:Label runat="server" AssociatedControlID="txtPin" CssClass="form-label" Text="PIN" />
                                <asp:TextBox ID="txtPin" runat="server" CssClass="form-control" TextMode="Password"
                                    MaxLength="6" inputmode="numeric" />
                                <asp:RequiredFieldValidator ID="rfvPin" runat="server" ControlToValidate="txtPin"
                                    ErrorMessage="PIN is required." CssClass="field-error" Display="Dynamic" />
                                <asp:RegularExpressionValidator ID="revPin" runat="server" ControlToValidate="txtPin"
                                    ValidationExpression="^[0-9]{4,6}$"
                                    ErrorMessage="PIN must be 4 to 6 digits." CssClass="field-error" Display="Dynamic" />
                            </div>

                            <div class="col-sm-6 mb-3">
                                <asp:Label runat="server" AssociatedControlID="txtConfirmPin" CssClass="form-label" Text="Confirm PIN" />
                                <asp:TextBox ID="txtConfirmPin" runat="server" CssClass="form-control" TextMode="Password"
                                    MaxLength="6" inputmode="numeric" />
                                <asp:RequiredFieldValidator ID="rfvConfirmPin" runat="server" ControlToValidate="txtConfirmPin"
                                    ErrorMessage="Please confirm your PIN." CssClass="field-error" Display="Dynamic" />
                                <asp:CompareValidator ID="cvPin" runat="server" ControlToValidate="txtConfirmPin"
                                    ControlToCompare="txtPin" Operator="Equal"
                                    ErrorMessage="PIN and Confirm PIN do not match." CssClass="field-error" Display="Dynamic" />
                            </div>
                        </div>

                        <asp:Button ID="btnSignup" runat="server" Text="Sign Up" CssClass="btn btn-primary w-100"
                            OnClick="btnSignup_Click" />
                    </asp:Panel>

                    <%-- Shown after a successful registration --%>
                    <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
                        <div class="alert alert-success">
                            <asp:Label ID="lblSuccess" runat="server" />
                        </div>
                        <a href="Login.aspx" class="btn btn-primary w-100">Go to Login</a>
                    </asp:Panel>

                    <p class="mt-3 mb-0 text-center">
                        Already registered? <a href="Login.aspx">Log in here</a>
                    </p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
