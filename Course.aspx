<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="Course.aspx.cs" Inherits="AlgoLab.Course" EnableViewState="true" %>

<%--Custom Page Title--%>
<asp:Content ID="CourseTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    Course
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="CourseHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="Assets/Stylesheets/course.css" rel="stylesheet" type="text/css" />
</asp:Content>

<%--Custom Page Body--%>
<asp:Content ID="CourseBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <!--Section 1 : Hero Section-->
    <section id="hero">
        <div class="hero_backImg zoom"></div>
        <div class="hero_container">
            <div class="hero_title">
                <h2>Discover AlgoLab Courses</h2>
            </div>
            <div class="hero_desc">
                <p>Empower your Levels with expert-led training in programming, design, and more.</p>
            </div>
            <div class="hero_btnWrapper">
                <a href="#courseCatalog">
                    <div class="btnToSection">
                        <p>View Courses <i class="fa fa-angle-double-down"></i></p>
                    </div>
                </a>
            </div>
        </div>
    </section>

    <!--Section 2 : Course Catalog-->
    <section id="courseCatalog">
        <div class="siteMapPath_wrapper">
            <asp:SiteMapPath ID="SiteMapPath_AlgoLab" runat="server" PathSeparator=" > " CssClass="siteMapPath" />
        </div>
        <div class="courseCatalog_title">
            <h2>Course Catalog</h2>
        </div>

        <div class="course_wrapper">
            <!--Left: Type of subjects available & Filter Options-->
            <div class="course_wrapper_left">
                <div class="course_category_wrapper">
                    <asp:SqlDataSource ID="SqlDataSourceCategory" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT DISTINCT Category.CategoryID, Category.CategoryName
FROM Course
INNER JOIN CourseCategory ON CourseCategory.CourseID = Course.CourseID
INNER JOIN Category ON CourseCategory.CategoryID = Category.CategoryID"></asp:SqlDataSource>

                    <h2 class="course_category_title">Category</h2>
                    <asp:CheckBoxList
                        ID="cblCategory"
                        runat="server"
                        DataSourceID="SqlDataSourceCategory"
                        DataTextField="CategoryName"
                        DataValueField="CategoryID"
                        CssClass="cbl-category">
                    </asp:CheckBoxList>
                </div>

                <div class="course_filter_wrapper">
                    <!--Level Filter Data Source-->
                    <asp:SqlDataSource ID="SqlDataSourceLevel" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT DISTINCT Course.CourseLevel,
                            CASE 
                                WHEN Course.CourseLevel = 'BEG' THEN 1
                                WHEN Course.CourseLevel = 'ITM' THEN 2
                                WHEN Course.CourseLevel = 'ADV' THEN 3
                            END AS SortOrder
                        FROM Course
                        ORDER BY SortOrder;"></asp:SqlDataSource>
                    <!--Tag Filter Data Source-->
                    <asp:SqlDataSource ID="SqlDataSourceTag" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT DISTINCT Tag.TagID, Tag.TagName
                            FROM Tag
                            INNER JOIN CourseTag ON CourseTag.TagID = Tag.TagID
                            INNER JOIN Course ON CourseTag.CourseID = Course.CourseID
                            ORDER BY Tag.TagName;"></asp:SqlDataSource>

                    <h2 class="course_filter_title">Filters</h2>
                    <!--Filter 1: Course Level (Beginner/Intermediate/Advanced)-->
                    <div class="filter_wrapper">
                        <h3 class="filter_title">Level</h3>
                        <asp:CheckBoxList
                            ID="cblLevels"
                            runat="server"
                            DataSourceID="SqlDataSourceLevel"
                            DataTextField="CourseLevel"
                            DataValueField="CourseLevel"
                            CssClass="cbl-filterLevels filter">
                        </asp:CheckBoxList>
                    </div>
                    <!--Filter 2: Course Tags-->
                    <div class="filter_wrapper">
                        <h3 class="filter_title">Tags</h3>
                        <asp:CheckBoxList
                            ID="cblTags"
                            runat="server"
                            DataSourceID="SqlDataSourceTag"
                            DataTextField="TagName"
                            DataValueField="TagID"
                            CssClass="cbl-filterTags filter">
                        </asp:CheckBoxList>
                    </div>
                </div>
            </div>




            <!--Right: Search bar & Content (Featured is shown by default)-->
            <div class="course_wrapper_right">
                <div class="course_search_wrapper">
                    <div id="course_searchForm">
                        <asp:TextBox ID="txtCourseSearch" runat="server" placeholder="e.g. Artificial Intelligence" CssClass="txtCourseSearchBox"></asp:TextBox>
                        <button id="btnCourseSearch" type="submit" runat="server" class="btnCourseSearch">
                            <i class="fa fa-search"></i>
                        </button>
                    </div>

                </div>
                <div class="course_content_wrapper">
                    <!--Course Content : DEFAULT-->
                    <div class="course_content_default_wrapper" runat="server" id="DefaultCourseWrapper">

                        <div class="course_content_default_featured_wrapper">
                            <h2 class="course_default_sectionTitle">Featured</h2>
                            <div class="course_content_featuredContent_wrapper">
                                <asp:PlaceHolder ID="PlaceHolder_DefaultFeaturedCard" runat="server"></asp:PlaceHolder>
                            </div>
                        </div>

                        <div class="course_content_default_free_wrapper">
                            <h2 class="course_default_sectionTitle">Free</h2>
                            <div class="course_content_freeContent_wrapper">
                                <asp:PlaceHolder ID="PlaceHolder_DefaultFreeCard" runat="server"></asp:PlaceHolder>
                            </div>
                        </div>

                        <div class="course_content_default_new_wrapper">
                            <h2 class="course_default_sectionTitle">New</h2>
                            <div class="course_content_newContent_wrapper">
                                <asp:PlaceHolder ID="PlaceHolder_DefaultNewCard" runat="server"></asp:PlaceHolder>
                            </div>
                        </div>

                    </div>


                    <!--Course Content : CUSTOM-->
                    <div class="course_content_wrapper">
                        <!-- Course Content: CUSTOM -->
                        <asp:Label ID="lblCourseCount" runat="server" Text="" CssClass="courseCountLabel"></asp:Label>
                        <div class="course_content_custom_wrapper" runat="server" id="CustomCourseWrapper">
                            <div class="noCourse_wrapper" id="NoCourseWrapper" runat="server">
                                <div class="no_courseImg"></div>
                                <h3>Results not found</h3>
                                <p>We couldn't find any courses matching your search. But don't worry, try these tips:</p>
                                <ul>
                                    <li>Double-check your spelling.</li>
                                    <li>Try using different keywords.</li>
                                </ul>
                                <div class="no_course_btnWrapper">
                                    <asp:Button ID="btnTopPicks" runat="server" Text="Discover Top Picks" OnClick="btnTopPicks_Click" CssClass="noCourseBtn" />
                                    <asp:Button ID="btnAllCourses" runat="server" Text="Show All Courses" OnClick="btnAllCourses_Click" CssClass="noCourseBtn" />
                                </div>
                            </div>

                            <asp:PlaceHolder ID="PlaceHolder_CustomCourseCard" runat="server"></asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
