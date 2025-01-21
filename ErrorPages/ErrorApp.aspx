<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="ErrorApp.aspx.cs" Inherits="AlgoLab.ErrorPages.ErrorApp" %>



<%--Custom Page Title--%>
<asp:Content ID="ErrorAppTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    Something Went Wrong
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="ErrorAppHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/error.css" rel="stylesheet" type="text/css" />
</asp:Content>

<%--Custom Page Body--%>
<asp:Content ID="ErrorAppBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <!--Section 1 : Error Page Hero-->
    <section id="errorHero">
        <div class="errorMsg_wrapper">
            <h2 class="errorMsg_title">Oops! Something Went Wrong.</h2>
            <p class="errorMsg_desc">
                <span>We're sorry, but an unexpected error occurred.</span>
                <span>Please try again later.</span>
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
        <div class="errorAppImg" style="background-image: url('../Assets/Images/icons/icon_errorApp.png');"></div>

    </section>

</asp:Content>
