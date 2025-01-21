<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="AdminInterface.aspx.cs" Inherits="AlgoLab.AdminInterface" %>

<%--Custom Page Title--%>
<asp:Content ID="AccountSelectTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    AlgoLab
</asp:Content>
<%--Custom Page Body--%>
<asp:Content ID="AccountSelectBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <section id="AdminInterfacePart">
        <div class="Selector_page">
            <img src="/Assets/Images/AdminInterface/Computer-Science-Backgrounds-HD.jpg">
            <div class="Selector_Window">
                <h1 class="Welcome">Welcome Back<span id="Administrator" cssClass="AdminName" runat="server"></span></h1>

                <asp:Button ID="ApproveTutor" CssClass="BtnSelect" runat="server" Text="Tutor Request" PostBackUrl="~/Management/ApproveTutor.aspx" />
                <asp:Button ID="SalesReport" CssClass="BtnSelect" runat="server" Text="Income Report" PostBackUrl="~/Management/AdminIncomeReport.aspx" />
                <asp:Button ID="btnSysFeedbackReport" CssClass="BtnSelect" runat="server" Text="System Feedback Report" PostBackUrl="~/Management/SystemFeedbackReport.aspx" />


            </div>
        </div>
    </section>
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="AccountSelectHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/AdminInterface.css" rel="stylesheet" type="text/css" />
</asp:Content>

