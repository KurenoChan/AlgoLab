<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="BecomeTutor.aspx.cs" Inherits="AlgoLab.BecomeTutor" %>

<%--Custom Page Title--%>
<asp:Content ID="BecomeTutorTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    AlgoLab
</asp:Content>
<%--Custom Page Body--%>
<asp:Content ID="BecomeTutorBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <section id="BecomeTutorPart">
        <div class="PageBody">

            <div class="container1">

                <div class="Benefit_Part">
                    <h2>&nbsp  Benefit of become tutor</h2>
                    <div class="Row">
                        <div class="box">
                            <img src="../Assets/Images/BecomeTutor/commission.png" alt="overtime" style="padding: 10px 10px;">
                            <div class="flipper">
                                <div class="back">
                                    Join Us Now!<br>
                                    High Commission Rate
                                </div>
                            </div>
                        </div>
                        <div class="box">
                            <img src="../Assets/Images/BecomeTutor/support.png" alt="overtime" style="padding: 10px 10px;">
                            <div class="flipper">
                                <div class="back">
                                    Join Us Now!<br>
                                    Provide Support Help
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="Row">
                        <div class="box">
                            <img src="../Assets/Images/BecomeTutor/environment.png" alt="overtime" style="padding: 10px 10px;">
                            <div class="flipper">
                                <div class="back">
                                    Join Us Now!<br>
                                    Relax Environment
                                </div>
                            </div>
                        </div>
                        <div class="box">
                            <img src="../Assets/Images/BecomeTutor/group.png" alt="overtime" style="padding: 10px 10px;">
                            <div class="flipper">
                                <div class="back">
                                    Join Us Now!<br>
                                    High amount of student
                                </div>
                            </div>
                        </div>
                    </div>


                </div>
                <div class="BecomeTutorFrom">
                    <h1>Tutor Form</h1>
                    <div class="input_wrapper">
                        <div class="input-box">
                            <label for="Expertise_Area">Expertise Area</label>
                            <asp:TextBox ID="Expertise_Area" CssClass="input-field" runat="server" placeholder="Expertise Area"></asp:TextBox>
                        </div>
                        <div class="input-box">
                            <label for="Education_Level">Education Level </label>
                            <asp:TextBox ID="Education_Level" CssClass="input-field" runat="server" placeholder="Education Level"></asp:TextBox>
                        </div>

                        <div class="input-box bioInput">
                            <label for="Bio">Bio</label>
                            <asp:TextBox ID="Bio" CssClass="input-field" runat="server" placeholder="Bio" TextMode="MultiLine"></asp:TextBox>
                        </div>
                        <div class="input-box">
                            <label for="Job_Role">Job Role</label>
                            <asp:TextBox ID="Job_Role" CssClass="input-field" runat="server" placeholder="Job Role"></asp:TextBox>
                        </div>
                        <div class="input-box">
                            <label for="Company">Company</label>
                            <asp:TextBox ID="Company" CssClass="input-field" runat="server" placeholder="Company"></asp:TextBox>
                        </div>
                        <div class="input-box Upload">
                            <label for="Certification">Certification</label>
                            <br />
                            <asp:FileUpload ID="Certification" CssClass="Upload_btn" runat="server" AllowMultiple="False" />
                            <asp:Label ID="UploadStatusLabel" CssClass="upload-status" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="input-box">
                            <asp:Button ID="Submit" CssClass="submit" runat="server" Text="Submit" OnClick="Submit_Click" />

                        </div>
                    </div>
                </div>

            </div>
        </div>
    </section>
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="BecomeTutorHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/BecomeTutor.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">
</asp:Content>


