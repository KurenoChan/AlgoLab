<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AlgoLab.Login" %>

<%--Custom Page Title--%>
<asp:Content ID="LoginTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    AlgoLab
</asp:Content>
<%--Custom Page Body--%>
<asp:Content ID="LoginBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <section id="LoginPart">
        <div class="account_menu_button" id="Selection" runat="server" > 
            <button class="account_button white-btn" id="loginBtn" onclick="login()">Login</button>
            <button class="account_button" id="registerBtn" onclick="register()" postbackurl="~/SignUp.aspx">Sign Up</button>
        </div>
        <div class="account_page">
            <div class="Login_Part">
                <div class="slider">
                </div>
                <%--<!---------------------------------------Formbox-->--%>
                <div class="form-box">
                    <!------------------- login form -------------------------->
                    <div class="login-container" id="login">
                        <div class="top">
                            <span  id="SignUpSuggest" runat="server" >Don't have an account? <a href="#" onclick="register()">Sign Up</a></span>
                            <h1 id="LoginContainerTitle" runat="server">Login</h1>
                        </div>
                        <div id="loginForm" runat="server">
                            <!-- Your input fields for login -->
                            <div class="input-box">
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="input-field"  placeholder="Username" required="true" OnTextChanged="txtUsername_TextChanged"></asp:TextBox>
                            </div>
                            <div class="input-box">
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input-field" placeholder="Password" required="true" OnTextChanged="txtPassword_TextChanged"></asp:TextBox>
                            </div>
                            <!-- Your submit button -->
                            <div class="input-box">
                                <asp:Button ID="btnLogin" runat="server" CssClass="submit" Text="Login" OnClick="btnLogin_Click" />
                            </div>
                        </div>
                        <div class="two-col">
                            <div class="two">
                                <label>
                                    <asp:HyperLink ID="ForgotPasswordLink" runat="server" NavigateUrl="ForgetPassword.aspx">Forgot password?</asp:HyperLink>
                                </label>
                            </div>
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
<asp:Content ID="LoginHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="Assets/Stylesheets/account.css" rel="stylesheet" type="text/css" />
    <script src="Assets/Scripts/account.js"></script>
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

