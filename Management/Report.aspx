<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="AlgoLab.Report" %>


<%--Custom Page Title--%>
<asp:Content ID="ReportTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    Report
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="ReportHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/report.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" async>

</script>
</asp:Content>

<%--Custom Style Body--%>
<asp:Content ID="ReportBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server" CssClass="ReportBody">

    <section id="MyCourseReport">
        <div id="ReportMCParent">
            <h1 id="ReportMCHead">My Course</h1>
            <hr />
        </div>

        <%--My Course Report--%>
        <%--Block--%>

        <div id="CourseReport">
            <a href="ReportCourse1.aspx" id="Course1">
                <div id="BlockClass1">
                    <img id="Course1Image" src="../Images/course/courseImg/CS-0131-EN/image.jpg" alt="Sample Image" />
                    <div id="Course1Block">
                        <h2 id="CourseCode1">CS-0131-EN</h2>
                        <div id="Course1NameBlock">
                            <h4 id="Course1Name">Artificial Intelligence</h4>
                        </div>
                    </div>
                </div>
            </a>

           <%--and second class...third class--%>
            
            
        </div>
    </section>
</asp:Content>