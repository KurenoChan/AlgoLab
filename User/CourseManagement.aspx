<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="CourseManagement.aspx.cs" Inherits="AlgoLab.CourseManagement" %>

<%--Custom Page Title--%>
<asp:Content ID="CManagementTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    Course Management
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="CmanagementHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/cmanagement.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function validateCourseForm() {
            var isValid = true;

            // Get the form fields
            var courseFee = document.getElementById('<%= CourseFee.ClientID %>').value;
            var courseName = document.getElementById('<%= CourseName.ClientID %>').value;
            var courseDesc = document.getElementById('<%= CourseDesc.ClientID %>').value;

            // Check if required fields are empty
            if (courseName.trim() === "") {
                alert("Course Name is required.");
                isValid = false;
            }
            if (courseDesc.trim() === "") {
                alert("Course Description is required.");
                isValid = false;
            }
            if (courseFee.trim() === "") {
                alert("Course Fee is required.");
                isValid = false;
            } else if (isNaN(courseFee) || parseFloat(courseFee) <= 0) {
                alert("Course Fee must be a valid positive number.");
                isValid = false;
            }

            return isValid;
        }



        function showCourseForm() {
            // Get the modal and overlay by their ClientIDs
            var modal = document.getElementById('<%= CreateCourseForm.ClientID %>');
            var overlay = document.getElementById('modalOverlay');

            // Show the modal and overlay
            overlay.style.display = 'flex';
            modal.style.display = 'block';

            document.getElementById("formPage1").style.display = "block";
            document.getElementById("nextBtn").style.display = "block";

            // Show the second page of the form (next_page and createBtn)
            document.getElementById("next_page").style.display = "none";
            document.getElementById("createBtn").style.display = "none";
        }

        function closeCourseForm() {
            // Get the modal and overlay by their ClientIDs
            var modal = document.getElementById('<%= CreateCourseForm.ClientID %>');
            var overlay = document.getElementById('modalOverlay');

            // Hide the modal and overlay
            overlay.style.display = 'none';
            modal.style.display = 'none';
        }

        function nextPageForm() {
            var isValid = true;

            // Get the form fields
            var courseFee = document.getElementById('<%= CourseFee.ClientID %>').value;
            var courseName = document.getElementById('<%= CourseName.ClientID %>').value;
            var courseDesc = document.getElementById('<%= CourseDesc.ClientID %>').value;

            // Check if required fields are empty
            if (courseName.trim() === "") {
                alert("Course Name is required.");
                isValid = false;
            }
            if (courseDesc.trim() === "") {
                alert("Course Description is required.");
                isValid = false;
            }
            if (courseFee.trim() === "") {
                alert("Course Fee is required.");
                isValid = false;
            } else if (isNaN(courseFee) || parseFloat(courseFee) <= 0) {
                alert("Course Fee must be a valid positive number.");
                isValid = false;
            }

            if (isValid) {
                // Hide the first page of the form (formPage1 and nextBtn)
                document.getElementById("formPage1").style.display = "none";
                document.getElementById("nextBtn").style.display = "none";

                // Show the second page of the form (next_page and createBtn)
                document.getElementById("next_page").style.display = "block";
                document.getElementById("createBtn").style.display = "block";
            }

            return isValid;  // If validation fails, the form won't proceed to the next page
        }

    </script>
</asp:Content>

<%--Custom Page Body--%>
<asp:Content ID="LessonBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <div class="title">
        <h1>My Course</h1>
        <div class="space"></div>
        <hr />
    </div>

    <!-- Create Course Button -->
    <section class="create_content">
        <asp:Panel ID="create" runat="server" Visible="false">
            <div class="create_button">
                <a href="#" onclick="showCourseForm()"><b>+</b>&nbsp;&nbsp;&nbsp;Create Course</a>
            </div>
        </asp:Panel>
    </section>

    <asp:Panel ID="CreateCourseForm" runat="server" CssClass="modal-panel" Style="display: none">
        <!-- Overlay to dim the background -->
        <div id="modalOverlay" class="modal-overlay" onclick="closeCourseForm()">
            <!-- Modal content -->
            <div id="CreateCourse" class="modal-content" onclick="event.stopPropagation()">
                <!-- Close button -->
                <button type="button" class="close-btn" onclick="closeCourseForm()">&times;</button>
                <h2 class="modal-header">Create New Course</h2>

                <!-- Modal Body -->
                <div class="modal-body" id="formPage1">
                    <div class="two-column-grid">
                        <!-- Column 1 -->
                        <div>
                            <div class="form-group">
                                <label for="CourseName">Course Name <span class="required">*</span>:</label>
                                <asp:TextBox ID="CourseName" runat="server" CssClass="form-control" required="true" />
                            </div>
                            <div class="form-group">
                                <label for="CourseShortDesc">Short Description <span class="required">*</span>:</label>
                                <asp:TextBox ID="CourseShortDesc" runat="server" TextMode="MultiLine" CssClass="form-control" required="true" />
                            </div>
                            <div class="form-group">
                                <label for="CourseLevel">Course Level:</label>
                                <asp:DropDownList ID="CourseLevel" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="BEG">Beginner</asp:ListItem>
                                    <asp:ListItem Value="ITM">Intermediate</asp:ListItem>
                                    <asp:ListItem Value="ADV">Advanced</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label for="CourseFee">Course Fee <span class="required">*</span>:</label>
                                <asp:TextBox ID="CourseFee" runat="server" CssClass="form-control" required="true" />
                                <asp:Label ID="CourseFeeError" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                            </div>
                        </div>

                        <!-- Column 2 -->
                        <div>
                            <div class="form-group">
                                <label for="CourseDesc">Description <span class="required">*</span>:</label>
                                <asp:TextBox ID="CourseDesc" runat="server" TextMode="MultiLine" CssClass="form-control" required="true" />
                            </div>
                            <div class="form-group">
                                <label for="CourseObj">Objective <span class="required">*</span>:</label>
                                <asp:TextBox ID="CourseObj" runat="server" TextMode="MultiLine" CssClass="form-control" required="true" />
                            </div>
                            <div class="form-group">
                                <label for="CourseLang">Language:</label>
                                <asp:DropDownList ID="CourseLang" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="EN">English</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label for="CourseImgPath">Course Image (Optional):</label>
                                <asp:FileUpload ID="CourseImgUpload" runat="server" CssClass="form-control" />
                            </div>
                            <div class="form-group">
                                <label for="CourseIconPath">Course Icon (Optional):</label>
                                <asp:FileUpload ID="CourseIcon" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Modal Footer -->
                <div class="modal-footer" id="nextBtn">
                    <button type="button" class="btn btn-primary" onclick="nextPageForm()">Next --></button>
                </div>

                <div class="modal-body two-column-grid" id="next_page" style="display: none;">
                    <!-- Column 1 -->
                    <div>
                        <div class="modal-body">


                            <!-- Your form structure -->
                            <div class="form-group">
                                <label for="CourseCategory">Select Category:</label>
                                <asp:DropDownList
                                    ID="CourseCategory"
                                    runat="server"
                                    CssClass="form-control">
                                </asp:DropDownList>
                            </div>



                            <div class="form-group">
                                <label for="CourseTags" class="form-label">Select Tags:</label>
                                <asp:CheckBoxList
                                    ID="CourseTags"
                                    runat="server"
                                    CssClass="checkbox-list"
                                    RepeatColumns="4"
                                    RepeatDirection="Horizontal">
                                </asp:CheckBoxList>
                            </div>


                        </div>




                        <!-- Modal Footer -->
                        <div class="modal-footer" id="createBtn" style="display: none">
                            <asp:Button ID="Button1" runat="server" Text="Create Course" OnClick="CreateCourse" CssClass="btn btn-primary" AutoPostBack="false" />
                        </div>
                    </div>
                </div>

            </div>

        </div>
    </asp:Panel>



    <!-- No Content Message -->
    <asp:Panel ID="no_content" runat="server" Visible="false">
        <div class="no_content">
            <h1>No Course Available</h1>
        </div>
    </asp:Panel>

    <div class="course_wrapper">
        <div id="TutorMyCourseCard" class="myCourseCard_PlaceHolder tutorMyCourseCard" runat="server">
            <asp:PlaceHolder ID="PlaceHolder_MyCourseCard_Tutor" runat="server"></asp:PlaceHolder>
        </div>

        <div id="StudentMyCourseCard" class="myCourseCard_PlaceHolder" runat="server">
            <div class="studentActiveWrapper">
                <h2>Active Courses</h2>
                <div class="placeholderContent">
                    <asp:PlaceHolder ID="PlaceHolder_MyCourseCard_Active" runat="server"></asp:PlaceHolder>
                </div>
            </div>
            <div class="studentCompletedWrapper">
                <h2>Completed Courses</h2>
                <div class="placeholderContent">
                    <asp:PlaceHolder ID="PlaceHolder_MyCourseCard_Completed" runat="server"></asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
