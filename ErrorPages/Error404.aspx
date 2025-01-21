<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="Error404.aspx.cs" Inherits="AlgoLab.ErrorPages.Error404" %>

<%--Custom Page Title--%>
<asp:Content ID="Error404Title" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    Page Not Found
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="Error404Head" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/error.css" rel="stylesheet" type="text/css" />
</asp:Content>

<%--Custom Page Body--%>
<asp:Content ID="Error404Body" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <!--Section 1 : Error Page Hero-->
    <section id="errorHero">
        <div class="errorMsg_wrapper">
            <h2 class="errorMsg_title">Oops! Page Not Found</h2>
            <p class="errorMsg_desc">
                <span>The page you&rsquo;re looking for doesn’t exist or has been moved.</span>
                <span>Don’t worry, we’ll help you find your way back.</span>
            </p>

            <div class="errorActions">
                <a href="<%= ResolveUrl("~/Home.aspx") %>">
                    <div class="btnToPage">
                        <p>Back To Home</p>
                    </div>
                </a>
                <a href="<%= ResolveUrl("~/Support.aspx") %>">
                    <div class="btnToPage">
                        <p>Contact Support</p>
                    </div>
                </a>
            </div>
        </div>
        <div class="error404Img" style="background-image: url('../Assets/Images/icons/icon_error404.png');"></div>

    </section>

</asp:Content>
