<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="AlgoLab.SignUp" %>

<%--Custom Page Title--%>
<asp:Content ID="SignUpTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    AlgoLab
</asp:Content>
<%--Custom Page Body--%>
<asp:Content ID="SignUpBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <section id="SignUpPart">
        <div class="account_menu_button">
            <button class="account_button " id="loginBtn" onclick="login()">Login</button>
            <button class="account_button white-btn" id="registerBtn" onclick="register()">Sign Up</button>
        </div>
        <div class="account_page">

            <!---------------------------------------Formbox-->
            <div class="form-box">
                <!------------------- registration form -------------------------->
                <div class="register-container" id="register" runat="server">
                    <div class="top">
                        <span>Have an account? 
                           
                            <asp:HyperLink ID="LoginLink" runat="server" NavigateUrl="AccountSelect.aspx">Login</asp:HyperLink>
                        </span>
                        <h1>Sign Up</h1>
                    </div>
                    <div class="input-box OneRow">
                        <asp:TextBox
                            ID="FirstName"
                            runat="server"
                            CssClass="input-field OneCol"
                            MaxLength="12"
                            placeholder="FirstName"
                            required="true"></asp:TextBox>
                        <asp:TextBox
                            ID="LastName"
                            runat="server"
                            CssClass="input-field OneCol"
                            MaxLength="12"
                            placeholder="LastName"
                            required="true"></asp:TextBox>
                    </div>
                    <div class="input-box">
                        <asp:TextBox
                            ID="Username"
                            runat="server"
                            CssClass="input-field"
                            MaxLength="12"
                            placeholder="Username"
                            required="true"></asp:TextBox>
                    </div>
                    <div class="input-box">
                        <asp:TextBox
                            ID="UserEmail"
                            runat="server"
                            CssClass="input-field"
                            TextMode="Email"
                            placeholder="Email"
                            required="true"></asp:TextBox>
                    </div>
                    <div class="input-box">
                        <asp:TextBox
                            ID="PhoneNumber"
                            runat="server"
                            CssClass="input-field"
                            TextMode="Phone"
                            MaxLength="11"
                            placeholder="Phone number"
                            required="true"></asp:TextBox>
                    </div>
                    <div class="GenderBox">
                        <!-- Male Option -->
                        <label class="gender-option Male">
                            <asp:RadioButton
                                ID="MaleGender"
                                runat="server"
                                GroupName="Gender"
                                Text="" />
                            <i></i>
                            <img class="GenderIcon" src="Assets/Images/signup/male.png" alt="Male" />

                            <span>Male</span>

                        </label>

                        <!-- Female Option -->
                        <label class="gender-option">
                            <asp:RadioButton
                                ID="FemaleGender"
                                runat="server"
                                GroupName="Gender"
                                Text="" />
                            <i></i>
                            <img class="GenderIcon" src="Assets/Images/signup/female.png" alt="Female" />
                            <span>Female</span>

                        </label>
                    </div>
                    <div class="input-box">
                        <asp:TextBox
                            ID="Password"
                            runat="server"
                            CssClass="input-field"
                            TextMode="Password"
                            MaxLength="12"
                            placeholder="Password"
                            required="true"></asp:TextBox>
                    </div>
                    <div class="input-box">
                        <asp:TextBox
                            ID="ConfirmPassword"
                            runat="server"
                            CssClass="input-field"
                            TextMode="Password"
                            MaxLength="12"
                            placeholder="Confirm Password"
                            required="true"></asp:TextBox>
                    </div>
                    <div class="input-box">
                        <asp:Button
                            ID="RegisterButton"
                            runat="server"
                            CssClass="submit"
                            Text="Register"
                            OnClick="RegisterButton_Click"/>
                    </div>
                    <div class="two-col">
                        <div class="two">
                            <label>
                                <asp:HyperLink
                                    ID="TermsConditions"
                                    runat="server"
                                    NavigateUrl="https://www.example.com">Terms & conditions</asp:HyperLink>
                            </label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <p class="output" id="output1"></p>
    <script src="Assets/Scripts/account.js"></script>


</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="SignUpHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="Assets/Stylesheets/SignUp.css" rel="stylesheet" type="text/css" />
    <script src="Assets/Scripts/account.js"></script>
    <link href="Assets/Stylesheets/index.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" async>
        var partnerIndex = 0; // Tracks the current scroll position of the slider
        function partnerSlide(slideBtn) {
            const partnerCardGroup = document.querySelector('.partnerCardGroup');
            const cardWidth = partnerCardGroup.firstElementChild.offsetWidth; // Width of one card
            const gap = parseInt(window.getComputedStyle(partnerCardGroup).gap) || 0; // Gap between cards (|| 0 = or zero if the gap value is undefined or invalid)
            const slideDistance = cardWidth + 5 * gap; // Total slide distance
            const maxSlide = partnerCardGroup.scrollWidth - partnerCardGroup.parentElement.offsetWidth; // Max scrollable distance

            if (slideBtn.id === 'partnerSliderRight') {
                // Move right (if not at the max scroll position)
                if (partnerIndex < maxSlide) {
                    partnerIndex += slideDistance;
                    if (partnerIndex > maxSlide) partnerIndex = maxSlide; // Cap at max scroll
                }
            } else if (slideBtn.id === 'partnerSliderLeft') {
                // Move left (if not at the start)
                if (partnerIndex > 0) {
                    partnerIndex -= slideDistance;
                    if (partnerIndex < 0) partnerIndex = 0; // Cap at min scroll
                }
            }

            // Apply the slide effect
            partnerCardGroup.style.transform = `translateX(-${partnerIndex}px)`;
        }
    </script>
</asp:Content>

