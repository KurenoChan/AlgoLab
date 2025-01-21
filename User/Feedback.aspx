<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="AlgoLab.Feedback" EnableViewState="true" %>

<%--Custom Page Title--%>
<asp:Content ID="FeedbackTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    Feedback
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="FeedbackHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/feedback.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" async>
    document.addEventListener('DOMContentLoaded', function () {
        // Get all the tooltips
        const tooltips = document.querySelectorAll('.errorTooltip');

        // Function to show the tooltip with fade-in and slide-in effect
        function showTooltip(index) {
            const tooltip = tooltips[index];

            // Reset height and margin to default values for smooth transition
            tooltip.style.height = 'auto'; // Ensure height is auto for smooth animation
            tooltip.style.margin = '2vh auto'; // Reset margin for smooth animation

            // Fade in and slide to normal position
            tooltip.style.opacity = 1;
            tooltip.style.transform = 'translateY(0)';
            tooltip.style.display = 'flex'; // Ensure the tooltip is visible during animation

            // After 3 seconds, start the fade-out and slide-out
            setTimeout(() => {
                tooltip.style.opacity = 0; // Fade out
                tooltip.style.transform = 'translateY(-5vh)'; // Slide up

                // After the animation completes, hide the tooltip
                setTimeout(() => {
                    tooltip.style.height = '0'; // Hide the tooltip after animation
                    tooltip.style.margin = '0'; // Hide the tooltip after animation
                    tooltip.style.padding = '0'; // Hide the tooltip after animation
                }, 500); // Match this duration to the animation duration (or adjust as needed)
            }, 5000); // Tooltip stays visible for 3 seconds before starting fade-out
        }

        // Show each tooltip one after the other with a delay
        tooltips.forEach((tooltip, index) => {
            setTimeout(() => {
                showTooltip(index); // Show each tooltip sequentially
            }, index * 500); // Delay each tooltip by 2 seconds (adjust as needed)
        });
    });
</script>


</asp:Content>

<%--Custom Page Body--%>
<asp:Content ID="FeedbackBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <section id="ErrorTooltipWrapper" class="errorTooltipWrapper" runat="server">
        <div class="errorTooltip">
            <div class="errorTooltipImg"></div>
            <p class="errorTooltipMsg">Please select a rating for your learning performance.</p>
        </div>
    </section>

    <!--Section 1 : Feedback Form-->
    <section id="feedbackForm">
        <h3>Feedback</h3>
        <div class="feedbackForm_wrapper">

            <!--1. Content Rating-->
            <div class="feedbackForm_card">
                <h4 class="feedbackQuestion">How would you rate the quality and relevance of the course content?</h4>
                <asp:RadioButtonList ID="rblContentRating" runat="server" CssClass="feedbackRating">
                    <asp:ListItem>1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem>3</asp:ListItem>
                    <asp:ListItem>4</asp:ListItem>
                    <asp:ListItem>5</asp:ListItem>
                </asp:RadioButtonList>
            </div>

            <!--2. Instructor Rating-->
            <div class="feedbackForm_card reveal">
                <h4 class="feedbackQuestion">How satisfied were you with the instructor's teaching style and expertise?</h4>
                <asp:RadioButtonList ID="rblInstructorRating" runat="server" CssClass="feedbackRating">
                    <asp:ListItem>1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem>3</asp:ListItem>
                    <asp:ListItem>4</asp:ListItem>
                    <asp:ListItem>5</asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <!--3. Platform Rating-->
            <div class="feedbackForm_card reveal">
                <h4 class="feedbackQuestion">How would you rate your overall experience using our platform (navigation, usability, etc.)?</h4>
                <asp:RadioButtonList ID="rblPlatformRating" runat="server" CssClass="feedbackRating">
                    <asp:ListItem>1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem>3</asp:ListItem>
                    <asp:ListItem>4</asp:ListItem>
                    <asp:ListItem>5</asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <!--4. Performance Rating-->
            <div class="feedbackForm_card reveal">
                <h4 class="feedbackQuestion">How do you feel about your learning performance and progress in this course?</h4>
                <asp:RadioButtonList ID="rblPerformanceRating" runat="server" CssClass="feedbackRating">
                    <asp:ListItem>1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem>3</asp:ListItem>
                    <asp:ListItem>4</asp:ListItem>
                    <asp:ListItem>5</asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <!--5. Feedback Details-->
            <div class="feedbackForm_card reveal">
                <h4 class="feedbackQuestion">Do you have any additional feedback or suggestions for improvement?</h4>
                <asp:TextBox ID="txtFeedbackDetails" runat="server" TextMode="MultiLine" CssClass="feedbackDetails" Rows="3"></asp:TextBox>
            </div>

            <asp:Button ID="btnSubmitFeedback" runat="server" CssClass="submitFeedbackBtn" Text="Submit" OnClick="btnSubmitFeedback_Click" />
        </div>
    </section>

</asp:Content>
