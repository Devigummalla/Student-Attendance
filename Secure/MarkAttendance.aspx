<%@ Page Title="Mark Attendance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MarkAttendance.aspx.cs" Inherits="Student_Att.MarkAttendancePage" %>

<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center">
        <div class="col-lg-7">
            <div class="card shadow-sm">
                <div class="card-body p-4">
                    <h3 class="page-title mb-3">Mark Attendance</h3>

                    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />

                    <asp:Panel ID="pnlStudent" runat="server">
                        <%-- Student details: filled automatically, not editable --%>
                        <table class="table detail-table mb-4">
                            <tr>
                                <th>Student Name</th>
                                <td><asp:Label ID="lblName" runat="server" /></td>
                            </tr>
                            <tr>
                                <th>Roll Number</th>
                                <td><asp:Label ID="lblRollNumber" runat="server" /></td>
                            </tr>
                            <tr>
                                <th>Department</th>
                                <td><asp:Label ID="lblDepartment" runat="server" /></td>
                            </tr>
                            <tr>
                                <th>Year</th>
                                <td><asp:Label ID="lblYear" runat="server" /></td>
                            </tr>
                            <tr>
                                <th>Attendance Date</th>
                                <td><asp:Label ID="lblDate" runat="server" CssClass="fw-semibold" /></td>
                            </tr>
                        </table>

                        <%-- Shown when attendance was already taken today --%>
                        <asp:Panel ID="pnlAlreadyMarked" runat="server" Visible="false" CssClass="alert alert-info">
                            Already marked <strong><asp:Label ID="lblExistingStatus" runat="server" /></strong>
                            by <asp:Label ID="lblMarkedBy" runat="server" />
                            at <asp:Label ID="lblMarkedAt" runat="server" />.
                        </asp:Panel>

                        <div class="mb-4">
                            <div class="form-label fw-semibold">Attendance Status</div>
                            <asp:RadioButtonList ID="rblStatus" runat="server" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" CssClass="status-options">
                                <asp:ListItem Value="Present" Text="Present" />
                                <asp:ListItem Value="Absent" Text="Absent" />
                            </asp:RadioButtonList>
                            <asp:RequiredFieldValidator ID="rfvStatus" runat="server" ControlToValidate="rblStatus"
                                ErrorMessage="Please select Present or Absent." CssClass="field-error d-block" Display="Dynamic" />
                        </div>

                        <div class="d-flex gap-2">
                            <%-- UseSubmitBehavior=false + disableButton(): the button is disabled after the
                                 first click, so the form cannot be submitted twice --%>
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit Attendance" CssClass="btn btn-success"
                                UseSubmitBehavior="false" OnClientClick="if (!disableButton(this)) return false;"
                                OnClick="btnSubmit_Click" />
                            <asp:HyperLink ID="lnkBack" runat="server" CssClass="btn btn-outline-secondary" Text="Back to List" />
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>

    <%-- Popup for duplicate attendance (Bootstrap modal) --%>
    <div class="modal fade" id="duplicateModal" tabindex="-1" aria-labelledby="duplicateTitle" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="duplicateTitle">Duplicate Attendance</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p class="mb-0 fs-5"><%: Student_Att.Business.AttendanceService.AlreadySubmittedMessage %></p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" data-bs-dismiss="modal">OK</button>
                </div>
            </div>
        </div>
    </div>

    <script>
        // Called when Submit is clicked. Checks the validators first,
        // then disables the button so a second click does nothing.
        function disableButton(button) {
            if (typeof Page_ClientValidate === 'function' && !Page_ClientValidate()) {
                return false;
            }
            button.disabled = true;
            button.value = 'Saving...';
            return true;
        }
    </script>
</asp:Content>
