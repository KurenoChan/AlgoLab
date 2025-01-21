<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="Support.aspx.cs" Inherits="AlgoLab.Support" %>

<%--Custom Page Title--%>
<asp:Content ID="SupportTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    Help & Support
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="SupportHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="Assets/Stylesheets/support.css" rel="stylesheet" type="text/css" />
</asp:Content>

<%--Custom Page Body--%>
<asp:Content ID="SupportBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <!--Section 1 : Hero Section-->
    <section id="supportHead">
        <h3 class="supportHeadTitle">Help & Support</h3>
        <div class="supportCard">
            <div id="supportContact" class="supportInnerCard">
                <img src="Assets/Images/icons/icon_support.png" alt="AlgoLab Support" />
                <h3>Contact Us</h3>
                <div class="supportOptions_wrapper">
                    <div class="supportOption">
                        <i class="fa fa-phone"></i>
                        <a href="tel:+18005551234">+1 (800) 555-1234</a>
                    </div>
                    <div class="supportOption">
                        <i class="fa fa-envelope"></i>
                        <a href="mailto:support@algolab.com">support@algolab.com</a>
                    </div>
                </div>
            </div>
            <div id="supportFeedback" class="supportInnerCard">
                <img src="Assets/Images/icons/icon_feedback.png" alt="AlgoLab Feedback" />
                <h3>Help Us Improve!</h3>
                <p class="supportCardDesc">Share your thoughts with us! Help us improve your learning experience.</p>
                <asp:HyperLink ID="lnkToFeedback" runat="server" NavigateUrl="~/User/Feedback.aspx" CssClass="toFeedbackLink">Rate Us</asp:HyperLink>
            </div>
        </div>
    </section>


    <!--Section 2 : FAQs-->
    <section id="faqs">
        <h3 class="sectionHeading">FAQs</h3>
        <div class="sectionBody">
            <ul class="faqs_accordion">
                <li class="faqsCard">
                    <input type="radio" name="faq" id="faq1" />
                    <label for="faq1">What is AlgoLab?</label>
                    <div class="faqAns">
                        <p>
                           AlgoLab is an online platform offering courses in coding, software development, and IT fields to help you enhance your technical skills.
                        </p>
                    </div>
                </li>

                <li class="faqsCard">
                    <input type="radio" name="faq" id="faq2" />
                    <label for="faq2">How can I enroll in a course?</label>
                    <div class="faqAns">
                        <p>
                           Browse the Course Catalog, select a course, register or log in, and complete the payment process if required. You’ll receive a confirmation email once enrolled.
                        </p>
                    </div>
                </li>

                <li class="faqsCard">
                    <input type="radio" name="faq" id="faq3" />
                    <label for="faq3">Are there any prerequisites?</label>
                    <div class="faqAns">
                        <p>
                           Each course may have prerequisites. Beginner courses require no prior knowledge, but advanced courses may require basic programming knowledge. Check the course details for more info.
                        </p>
                    </div>
                </li>

                <li class="faqsCard">
                    <input type="radio" name="faq" id="faq4" />
                    <label for="faq4">Can I access materials after completing the course?</label>
                    <div class="faqAns">
                        <p>
                           Yes, you will have lifetime access to all course materials, even after completion.
                        </p>
                    </div>
                </li>

                <li class="faqsCard">
                    <input type="radio" name="faq" id="faq5" />
                    <label for="faq5">How can I contact customer support?</label>
                    <div class="faqAns">
                        <p>
                           Visit our Contact page to find our email, phone number, or use the contact form for support.
                        </p>
                    </div>
                </li>

                <li class="faqsCard">
                    <input type="radio" name="faq" id="faq6" />
                    <label for="faq6">Can I become a tutor at AlgoLab?</label>
                    <div class="faqAns">
                        <p>
                           Yes, visit our Become a Tutor page for more information on applying to become a tutor.
                        </p>
                    </div>
                </li>

                <li class="faqsCard">
                    <input type="radio" name="faq" id="faq7" />
                    <label for="faq7">How do I reset my password?</label>
                    <div class="faqAns">
                        <p>
                           Click "Forgot Password" on the Login page, enter your email, and follow the instructions to reset your password.
                        </p>
                    </div>
                </li>
            </ul>
        </div>
    </section>
</asp:Content>
