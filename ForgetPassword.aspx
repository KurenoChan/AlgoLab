<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="ForgetPassword.aspx.cs" Inherits="AlgoLab.ForgetPassword" %>

<asp:Content ID="ForgetPasswordTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    AlgoLab
</asp:Content>

<asp:Content ID="ForgetPasswordHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="Assets/Stylesheets/ForgetPassword.css" rel="stylesheet" type="text/css" />
    <script src="Assets/Scripts/ForgetPassword.js"></script>
</asp:Content>

<asp:Content ID="ForgetPasswordBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <div class="Reset_part_page">
        <div class="reset_page">
            <asp:Label ID="resetMessageEmail" runat="server" CssClass="message-label" Text="" Visible="false"></asp:Label>
            <asp:Panel ID="resetpassForm" runat="server" Visible="true">
                <div id="forgotPasswordForm">
                    <div class="top">
                        <h1>Confirm Email</h1>
                    </div>
                    <div class="input-box">
                        <asp:TextBox
                            ID="Foremail"
                            runat="server"
                            CssClass="input-field"
                            TextMode="Email"
                            placeholder="Enter your email"
                            ></asp:TextBox>
                    </div>
                    <div class="input-box">
                        <asp:TextBox
                            ID="Forusername"
                            runat="server"
                            CssClass="input-field"
                            MaxLength="12"
                            placeholder="Username"
                           ></asp:TextBox>
                    </div>
                    <div class="input-box">
                        <asp:Button
                            ID="RegisterButton"
                            runat="server"
                            CssClass="submit"
                            Text="Submit"
                            OnClick="checkemail_Click" />
                    </div>
                </div>
            </asp:Panel>
            <asp:Label ID="resetMessage" runat="server" CssClass="message-label" Text="" Visible="false"></asp:Label>
            <asp:Panel ID="reset_pass_part" runat="server" Style="display: none;">
                <div id="passwordResetForm">
                    <div class="top">
                        <h1>Reset Password</h1>
                    </div>
                    <div class="input-box">
                        <asp:TextBox
                            ID="newPassword"
                            runat="server"
                            CssClass="input-field"
                            TextMode="Password"
                            MaxLength="12"
                            placeholder="Enter your new password"
                            
                           ></asp:TextBox>
                    </div>
                    <div class="input-box">
                        <asp:TextBox
                            ID="confirmPassword"
                            runat="server"
                            CssClass="input-field"
                            TextMode="Password"
                            MaxLength="12"
                            placeholder="Confirm your new password"
                            
                            ></asp:TextBox>
                    </div>
                    <div class="input-box">
                        <asp:Button
                            ID="resetBtn"
                            runat="server"
                            CssClass="submit"
                            Text="Reset"
                            OnClick="resetpassword_Click" />
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
