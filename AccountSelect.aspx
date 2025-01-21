<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="AccountSelect.aspx.cs" Inherits="AlgoLab.AccountSelect" %>

<%--Custom Page Title--%>
<asp:Content ID="AccountSelectTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    AlgoLab
</asp:Content>
<%--Custom Page Body--%>
<asp:Content ID="AccountSelectBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <section id="AccountSelector">
        <div class="Selector_page">
            <div class="Selector_Window">
                <h1 class="Welcome">Welcome to AlgoLab</h1>
                <asp:Button ID="btnCustomer" CssClass="BtnSelect" runat="server" Text="Customer Login" OnClick="btnCustomer_Click" />
                <asp:Button ID="btnStaff" CssClass="BtnSelect" runat="server" Text="Staff Login" OnClick="btnStaff_Click"  />
            </div>
        </div>
    </section>
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="AccountSelectHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="Assets/Stylesheets/AccountSelect.css" rel="stylesheet" type="text/css" />
</asp:Content>

