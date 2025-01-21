<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="AdminIncomeReport.aspx.cs" Inherits="AlgoLab.AdminIncomeReport" %>


<%--Custom Page Title--%>
<asp:Content ID="IncomeReportTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    Income Report
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="IncomeReportHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/AdminIncomeReport.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" async>

</script>
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

</asp:Content>

<%--Custom Style Body--%>
<asp:Content ID="IncomeReportBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server" CssClass="IncomeReportBody">

    <section id="IncomeReport">
        <div id="IncomeReportParent">
            <h1 id="IncomeReportHeading">Income Report</h1>
            <hr />
        </div>

        <div class="DataFieldParent">


            <div class="DataParent">
                <div class="imageStored">
                    <img class="IconImage" src="../Assets/Images/IncomePage/course.png" alt="Course"/>
                </div>
                <div class="DataClass">
                    <span class="Data">Total of Courses</span>
                    <div class="DatabaseNumber" id="NumberCourses">
                        <%: NumberOfCourses %>
                    </div>
                </div>
            </div>

            <div class="DataParent">
                <div class="imageStored">
                    <img class="IconImage" src="../Assets/Images/IncomePage/commission.png"  alt="Course"/>
                </div>
                 <div class="DataClass">
                     <span class="Data">Net Income</span>
                     <%--read course fee--%>
                     <div class="DatabaseNumber" id="NetIncome">
                        <%: TotalNetIncome.ToString("C") %>
                     </div>
                  </div>
            </div>

        </div>

        <!-- Dropdowns for Month and Year Selection -->
        <div class="SelectionDiv">
            <div class="AdminSelection">
                <label for="ddlMonth">Select Month:</label>
                <asp:DropDownList ID="ddlMonth" runat="server" CssClass="Seleted">
                    <asp:ListItem Text="January" Value="1" />
                    <asp:ListItem Text="February" Value="2" />
                    <asp:ListItem Text="March" Value="3" />
                    <asp:ListItem Text="April" Value="4" />
                    <asp:ListItem Text="May" Value="5" />
                    <asp:ListItem Text="June" Value="6" />
                    <asp:ListItem Text="July" Value="7" />
                    <asp:ListItem Text="August" Value="8" />
                    <asp:ListItem Text="September" Value="9" />
                    <asp:ListItem Text="October" Value="10" />
                    <asp:ListItem Text="November" Value="11" />
                    <asp:ListItem Text="December" Value="12" />
                </asp:DropDownList>

            </div>

            <div class="AdminSelection">
                <label for="ddlYear">Select Year:</label>
                <asp:DropDownList ID="ddlYear" runat="server" CasClass="Selected">
                    <asp:ListItem Text="2023" Value="2023" />
                    <asp:ListItem Text="2024" Value="2024" />
                    <asp:ListItem Text="2025" Value="2025" />
                </asp:DropDownList>
            </div>

            <div id="GenerateParent">
                <asp:Button ID="btnFilter" runat="server" Text="Generate Report" OnClick="btnFilter_Click" CssClass="Generated"/>
            </div>
        </div>

    <div>
        <asp:GridView ID="GridViewReports" runat="server" AutoGenerateColumns="False" CssClass="table" BorderWidth="1" CellPadding="4" CellSpacing="0">
            <Columns>
                <asp:BoundField DataField="TutorID" HeaderText="Tutor ID" SortExpression="TutorID" />
                <asp:BoundField DataField="CourseID" HeaderText="Course ID" SortExpression="CourseID" />
                <asp:BoundField DataField="CourseName" HeaderText="Course Name" SortExpression="CourseName" />
                <asp:BoundField DataField="CourseFee" HeaderText="Course Fee" SortExpression="CourseFee" />
                <asp:BoundField DataField="NumberOfStudents" HeaderText="Number of Students" SortExpression="NumberOfStudents" />
                <asp:BoundField DataField="TotalIncome" HeaderText="Total Income" SortExpression="TotalIncome" />
                <asp:BoundField DataField="NetIncome" HeaderText="Net Income" SortExpression="NetIncome" />
                <asp:BoundField DataField="Year" HeaderText="Year" SortExpression="Year" />
                <asp:BoundField DataField="Month" HeaderText="Month" SortExpression="Month" />
            </Columns>
        </asp:GridView>
    </div>

     </section>
    </asp:Content>
