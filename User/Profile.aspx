    <%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="AlgoLab.Profile"  %>

    <asp:Content ID="ProfileTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
        Profile
    </asp:Content>

    <%--Custom Style Sheet--%>
    <asp:Content ID="ProfileHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
        <link href="../Assets/Stylesheets/profile.css" rel="stylesheet" type="text/css" />
        <script type="text/javascript" async>

            function showCourseForm() {
                // Get the modal and overlay by their ClientIDs
                var modal = document.getElementById('<%= EditProfileModal.ClientID %>');
                var overlay = document.getElementById('modalOverlay');

                // Show the modal and overlay
                overlay.style.display = 'flex';
                modal.style.display = 'block';
            }

            function closeCourseForm() {
                // Get the modal and overlay by their ClientIDs
                var modal = document.getElementById('<%= EditProfileModal.ClientID %>');
                    var overlay = document.getElementById('modalOverlay');

                    // Hide the modal and overlay
                    overlay.style.display = 'none';
                    modal.style.display = 'none';
                }


                // JavaScript function to show the AvatarUpdated button when a file is selected
                function showUpdateButton() {
                    // Get the FileUpload control and AvatarUpdated button  

                // If a file is selected, make the button visible
                if (fileUpload.files.length > 0) {
                    updateButton.style.display = 'inline';  // Make the button visible
                }
            }

        </script>
        
    </asp:Content>

    <asp:Content ID="ProfiletBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server" CssClass="ProfiletBody">
        <section id="userProfile">
            <div id="ProfileParent">
                <h1 id="ProfilePHead">Profile</h1>
                <hr />
            </div>

            <div id="PPParent">
                <h1 class="ElementHeader">Profile Picture</h1>
                <div class="AvatarParent">
                    <!-- Avatar Image -->
                   <div id="imageParent">
                       <asp:Image runat="server" id="imgProfile"  Visible="true" CssClass="AvatarClass"/> 
                   </div>

               <%-- <div id="updateProfilePicture">                   
                    <asp:FileUpload  ID="fileUploadAvatar" runat="server" CssClass="FileUploadClass" text="Edit Your Avatar" />
                        <asp:Button ID="AvatarUpdated" runat="server" Text="Update Your Avatar" CssClass="AvatarUpdated"  OnClick="AvatarUpdated_Click" />            
                </div>--%>


                </div>
            </div>

            <div id="ProfileInfo">
                <div>
                    <div class="ElementHeader">
                        <h1>Personal Infomation</h1>
                    </div>
                    <div class="blockElement" id="Element">
                        <span class="ElementTopic">Username:</span>
                        <div>
                            <asp:Label ID="lblCustUsername" runat="server" CssClass="ElementField" />

                        </div>
                    </div>

                     <div class="blockElement" >
                         <span class="ElementTopic">Password:</span>
                         <div>
                             <asp:Label ID="lblPassword" runat="server" CssClass="ElementField" />
                         </div>
                     </div>

                    <div class="blockElement" >
                        <span class="ElementTopic">Identity</span>
                        <div>
                            <asp:Label ID="lblRole" runat="server" CssClass="ElementField"  />
                        </div>
                    </div>

                    <div class="blockElement" >
                        <span class="ElementTopic">Customer ID </span>
                        <div>
                            <asp:Label ID="lblCustID" runat="server" CssClass="ElementField" />
                        </div>
                    </div>

                <div class="blockElement" >
                    <span class="ElementTopic">Date Join:</span>
                    <div>
                        <asp:Label ID="lblDateJoined" runat="server" CssClass="ElementField"  />
                    </div>
                </div>

                <div class="blockElement" >
                    <span class="ElementTopic">First Name:</span>
                    <div>
                        <asp:Label ID="lblFirstName" runat="server" CssClass="ElementField" />
                    </div>
                </div>

                 <div class="blockElement" >
                     <span class="ElementTopic">Last Name:</span>
                     <div>
                         <asp:Label ID="lblLastName" runat="server" CssClass="ElementField" />
                     </div>
                 </div>

                  <div class="blockElement" >
                     <span class="ElementTopic">Gender</span>
                     <div>
                         <asp:Label ID="lblGender" runat="server" CssClass="ElementField" />
                     </div>
                 </div>

                 <div class="blockElement" >
                     <span class="ElementTopic">Email:</span>
                     <div>
                         <asp:Label ID="lblEmail" runat="server" CssClass="ElementField" />
                     </div>
                 </div>

                  <div class="blockElement" >
                     <span class="ElementTopic">Phone:</span>
                     <div>
                         <asp:Label ID="lblPhoneNumber" runat="server" CssClass="ElementField" />
                     </div>
                 </div>
             </div>

                    <%--TutorDesign--%>

                         <div class="EnrollmentParent">
                            <div id="BottomDesign">
                              <div id="Div1" runat="server" >                        
                                  <a href="CourseManagement.aspx" id="ViewReport">My Course </a>
                               </div>
                           </div>


                         <div id="UpdateProfile" visible="false">
                             <asp:Button runat="server" ID="EditProfileBtn" Text="Update Profile" CssClass="UpdateDesign" OnClientClick="showCourseForm(); return false;" />
                        
                         </div>

                        <%--modal--%>
                          <div id="modalOverlay" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0, 0, 0, 0.5);"></div>

                            <asp:Panel ID="EditProfileModal" runat="server" style="display: none; position: fixed; top: 20%; left: 25%; width: 50%; background: white; padding: 20px; border-radius: 8px;">
                                <h2>Edit Profile</h2>
                                    <div>
                                        <label for="txtFirstName">First Name:</label>
                                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="TextBoxClass" />
                                    </div>

                                    <div>
                                        <label for="txtLastName">Last Name:</label>
                                        <asp:TextBox ID="txtLastName" runat="server" CssClass="TextBoxClass" />
                                    </div>

                                    <div>
                                        <label for="txtCustUsername">Username:</label>
                                        <asp:TextBox ID="txtCustUsername" runat="server" CssClass="TextBoxClass" />
                                    </div>

                                    <div>
                                        <label for="txtEmail">Email:</label>
                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="TextBoxClass" />
                                    </div>

                                    <div>
                                        <label for="txtPhoneNumber">Phone Number:</label>
                                        <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="TextBoxClass" />
                                    </div>
                                    <div>
                                        <label for="rblGender">Gender:</label>
                                        
                                       <asp:RadioButtonList ID="rblGender" runat="server">
                                            <asp:ListItem Value="M">Male</asp:ListItem>
                                            <asp:ListItem Value="F">Female</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                    <div>
                                        <label for="txtPassword">Password:</label>
                                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="TextBoxClass" />
                                    </div>
        
                                    <!-- Submit button to save changes -->
                                    <asp:Button ID="SaveProfile" runat="server" Text="Save Changes" OnClick="SaveProfileChanges" />

        
                                    <!-- Close button to hide the modal -->
                                    <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="closeCourseForm(); return false;" CssClass="CloseBtn" />
                                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
                            </asp:Panel>

                          </div>
                        </div>   
                     
   
        </section>
    </asp:Content>
