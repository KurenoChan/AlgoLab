<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="ApproveTutor.aspx.cs" Inherits="AlgoLab.ApproveTutor" %>

<asp:Content ID="ApproveTutorTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    AlgoLab
</asp:Content>

<asp:Content ID="ApproveTutorBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <section id="ApproveTutorPart" runat="server">
        <h1 class="title">Tutor Request</h1>
        <div id="Approve_Container" class="Approve_part" runat="server">
        </div>
    </section>
</asp:Content>

<asp:Content ID="ApproveTutorHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/ApproveTutor.css" rel="stylesheet" type="text/css" />
    <script async src="../Assets/Scripts/ApproveTutor.js"></script>
    
</asp:Content>
