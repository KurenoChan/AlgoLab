<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="Lesson.aspx.cs" Inherits="AlgoLab.Lesson" %>

<%--Custom Page Title--%>
<asp:Content ID="Lesson_Title" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    Lesson
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="LessonHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/lessons.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript"> 

        var userRole = '<%= Session["UserRole"] ?? "Guest" %>';

        window.onload = function () {
            if (userRole === "Tutor") {
                // Show the upload buttons for all lessons if the user is a tutor
                var uploadButtons = document.querySelectorAll('.upload_section');
                uploadButtons.forEach(function (button) {
                    button.style.display = 'block';
                });
            }
        }

        function validateForm() {

            const materialName = document.getElementById('<%= MaterialName.ClientID %>');
            const materialPath = document.getElementById('<%= MaterialPath.ClientID %>');

            // Check if Material Name is empty
            if (!materialName.value.trim()) {
                alert("Material Name is required.");
                materialName.focus();
                return false; // Prevent the form from submitting
            }

            // Check if a file is selected
            if (!materialPath.value) {
                alert("Please select a file to upload.");
                materialPath.focus();
                return false; // Prevent the form from submitting
            }

            return true; // Allow the form to submit
        }

        function validateForm1() {
            var isValid = true;

            // Get references to the input fields
            var lessonTitle = document.getElementById('<%= LessonTitle.ClientID %>');
            var lessonDesc = document.getElementById('<%= LessonDesc.ClientID %>');
            var materialName = document.getElementById('<%= MaterialName1.ClientID %>');
            var materialPath = document.getElementById('<%= MaterialPath1.ClientID %>');

            // Check if Lesson Title is filled
            if (lessonTitle.value.trim() === "") {
                alert("Lesson Title is required.");
                isValid = false;
            }

            // Check if Lesson Description is filled
            if (lessonDesc.value.trim() === "") {
                alert("Lesson Description is required.");
                isValid = false;
            }

            // Check if Material Name is filled
            if (materialName.value.trim() === "") {
                alert("Material Name is required.");
                isValid = false;
            }

            // Check if Material file is selected
            if (materialPath.value.trim() === "") {
                alert("Please select a material file.");
                isValid = false;
            }

            // Return whether the form is valid or not
            return isValid;
        }



        function showUploadForm(lessonID) {
            var modal = document.getElementById('<%= UploadForm.ClientID %>');
            var overlay = document.getElementById('modalOverlay');

            overlay.style.display = 'flex';
            modal.style.display = 'block';

            // Set the lessonId hidden field value dynamically
            document.getElementById("lessonId").value = lessonID;

            document.getElementById("formPage1").style.display = "block";
            document.getElementById("uploadBtn").style.display = "block";
        }

        function closeUploadForm() {
            var modal = document.getElementById('<%= UploadForm.ClientID %>');
            var overlay = document.getElementById('modalOverlay');

            overlay.style.display = 'none';
            modal.style.display = 'none';
        }


        function showLessonForm() {
            // Get the modal and overlay elements by their IDs
            var modal = document.getElementById('<%= LessonForm.ClientID %>');  // Corrected the ID
            var overlay = document.getElementById('modalOverlay1');

            // Display the modal and overlay
            overlay.style.display = 'flex';
            modal.style.display = 'block';

            // Ensure the form and button are visible when the modal is shown
            document.getElementById("formPage").style.display = "block";
            document.getElementById("createLessonBtn").style.display = "block";
        }

        function closeLessonForm() {
            // Get the modal and overlay elements by their IDs
            var modal = document.getElementById('<%= LessonForm.ClientID %>');  // Corrected the ID
            var overlay = document.getElementById('modalOverlay1');

            // Hide the modal and overlay when closing the form
            overlay.style.display = 'none';
            modal.style.display = 'none';

            // Optionally, clear the form fields or reset the modal state here
            document.getElementById("MaterialName1").value = "";  // Clear the material name field
            document.getElementById("MaterialPath1").value = "";  // Clear the file upload field
        }


    </script>

</asp:Content>

<%--Custom Page Body--%>
<asp:Content ID="LessonBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <section>
        <div class="header">
            <h1>Lesson</h1>
        </div>


    </section>
    <div class="space"></div>
    <hr style="width: 80%" />



    <asp:Panel ID="TutorLayout" runat="server" Visible="false">
        <section class="lesson_content">
            <div class="create_button">
                <a href="#" onclick="showLessonForm()"><b>+</b>&nbsp;&nbsp;&nbsp;Create Lesson</a>
            </div>
        </section>
    </asp:Panel>
    <br />


    <asp:Repeater ID="RepeaterLessons" runat="server" OnItemDataBound="RepeaterLessons_ItemDataBound">
        <ItemTemplate>
            <div class="lesson-container">
                <!-- Lesson Header -->
                <div class="lesson-header">
                    <h3 class="lesson-title"><%# Eval("LessonTitle") %></h3>

                </div>
                <div class="lesson-desc">
                    <p class="lesson-desc"><%# Eval("LessonDesc") %></p>
                </div>


                <div class="material-container">
                    <asp:Repeater ID="RepeaterMaterials" runat="server" DataSource='<%# Eval("Materials") %>'>
                        <ItemTemplate>
                            <a href='<%# Eval("MaterialPath") != null ? Eval("MaterialPath") : "#" %>' target="_blank" class="material-box">
                                <img src="\assets\images\course\defaultthumbnail.png" alt="Material" class="material-thumbnail" />
                                <p class="material-name"><%# Eval("MaterialName") != null ? Eval("MaterialName") : "No Material Available" %></p>
                            </a>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Panel runat="server" ID="uploadmaterial" Style="display: none">
                        <section class="upload_section">
                            <a href="#" onclick="showUploadForm('<%# Eval("LessonID") %>')" class="upload-button">
                                <p>UPLOAD MATERIAL</p>
                            </a>
                        </section>
                    </asp:Panel>
                </div>

            </div>
        </ItemTemplate>
    </asp:Repeater>






    <asp:Panel ID="UploadForm" runat="server" CssClass="modal-panel" Style="display: none">
        <div id="modalOverlay" class="modal-overlay" onclick="closeUploadForm()">
            <div id="UploadMaterial" class="modal-content" onclick="event.stopPropagation()">
                <button type="button" class="close-btn" onclick="closeUploadForm()">&times;</button>
                <h2 class="modal-header">Upload Material</h2>

                <div class="modal-body" id="formPage1">
                    <div class="form-group">

                        <input type="hidden" id="lessonId" name="lessonId" value="" />

                        <label for="MaterialName">Material Name <span class="required">*</span>:</label>
                        <asp:TextBox ID="MaterialName" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator
                            ID="rfvMaterialName"
                            runat="server"
                            ControlToValidate="MaterialName"
                            ErrorMessage="Material Name is required."
                            CssClass="text-danger"
                            Display="Dynamic" />
                    </div>
                    <div class="form-group">
                        <label for="MaterialPath">Material:</label>
                        <asp:FileUpload ID="MaterialPath" runat="server" CssClass="form-control" />
                    </div>

                </div>

                <div class="modal-footer" id="uploadBtn" style="display: none">
                    <asp:Button
                        ID="upload_Btn"
                        runat="server"
                        Text="Upload"
                        CssClass="btn btn-primary"
                        OnClick="upload_Btn_Click"
                        OnClientClick="return validateForm();" />
                </div>
            </div>
        </div>
    </asp:Panel>


































    <asp:Panel ID="LessonForm" runat="server" CssClass="modal-panel" Style="display: none">
        <div id="modalOverlay1" class="modal-overlay" onclick="closeLessonForm()">
            <div id="UploadMaterial1" class="modal-content" onclick="event.stopPropagation()">
                <button type="button" class="close-btn" onclick="closeLessonForm()">&times;</button>
                <h2 class="modal-header">Create Lesson</h2>

                <div class="modal-body" id="formPage">
                    <!-- Lesson Title -->
                    <div class="form-group">
                        <label for="LessonTitle">Lesson Title : <span class="required">*</span>:</label>
                        <asp:TextBox ID="LessonTitle" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator
                            ID="rfvLessonTitle"
                            runat="server"
                            ControlToValidate="LessonTitle"
                            ErrorMessage="Lesson Title is required."
                            CssClass="text-danger"
                            Display="Dynamic" />
                    </div>

                    <!-- Lesson Description -->
                    <div class="form-group">
                        <label for="LessonDesc">Lesson Description : <span class="required">*</span>:</label>
                        <asp:TextBox ID="LessonDesc" runat="server" CssClass="form-control" TextMode="MultiLine" />
                        <asp:RequiredFieldValidator
                            ID="rfvLessonDesc"
                            runat="server"
                            ControlToValidate="LessonDesc"
                            ErrorMessage="Lesson Description is required."
                            CssClass="text-danger"
                            Display="Dynamic" />
                    </div>

                    <!-- Material Name -->
                    <div class="form-group">
                        <label for="MaterialName1">Material Name <span class="required">*</span>:</label>
                        <asp:TextBox ID="MaterialName1" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator
                            ID="rfvMaterialName1"
                            runat="server"
                            ControlToValidate="MaterialName1"
                            ErrorMessage="Material Name is required."
                            CssClass="text-danger"
                            Display="Dynamic" />
                    </div>

                    <!-- Material Upload -->
                    <div class="form-group">
                        <label for="MaterialPath1">Material:</label>
                        <asp:FileUpload ID="MaterialPath1" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <!-- Footer and Submit Button -->
                <div class="modal-footer" id="createLessonBtn" style="display: none">
                    <asp:Button
                        ID="createLesson_Btn"
                        runat="server"
                        Text="Create Lesson"
                        CssClass="btn btn-primary"
                        OnClick="createLesson_Btn_Click"
                        OnClientClick="return validateForm1();" />
                </div>
            </div>
        </div>
    </asp:Panel>




    <asp:Panel ID="no_content" runat="server" Visible="false">
        <div class="no_content">
            <h1>No Lesson Available</h1>
        </div>
    </asp:Panel>

    <asp:Panel runat="server" ID="student_Layout" CssClass="center-panel" Visible="false">
        <asp:Button ID="CompleteCourseBtn" runat="server" Text="Complete Course"
            CssClass="create_button" OnClick="CompleteCourse_Click"
            CommandArgument='<%# Eval("CourseID") %>' />
    </asp:Panel>







</asp:Content>
