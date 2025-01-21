<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="CourseDetails.aspx.cs" Inherits="AlgoLab.CourseDetails" %>

<%--Custom Page Title--%>
<asp:Content ID="CourseDetailsTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    Course Details
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="CourseDetailsHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="Assets/Stylesheets/courseDetails.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" async>
    // Function to close the popup
    function closePrerequisite() {
        document.querySelector(".prerequisitePopup").style.display = "none";
    }

    // Function to close the popup if clicking outside
    document.addEventListener('click', function (event) {
        var popup = document.querySelector(".prerequisitePopup");
        var button = document.getElementById("btnCloseprerequisitePopup");

        // Check if the click was outside of the popup and the button
        if (!popup.contains(event.target) && event.target !== button) {
            closePrerequisite();
        }
    });
</script>

</asp:Content>

<%--Custom Page Body--%>
<asp:Content ID="CourseDetailsBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <!-- PREREQUISITE POPUP -->
    <section id="prerequisitePopup" class="prerequisitePopup" runat="server">
        <div id="btnCloseprerequisitePopup" onclick="closePrerequisite();">&#10006;</div>
        <h3 class="prerequisitePopupTitle">Prerequisites</h3>
        <p class="prerequisitePopupDesc">Complete the required prerequisite(s) below to unlock this course!</p>
        <div id="PrerequisitePopupCourseWrapper" class="prerequisitePopup_courseWrapper" runat="server">
        </div>
    </section>


    <!--Section 1 : Course General Details-->
    <section id="general">
        <div class="courseImg_wrapper" id="courseImgWrapper"></div>
        <div class="courseDetails_wrapper">
            <div class="courseTitleRating_wrapper">
                <asp:Label ID="lblCourseName" runat="server" Text="" CssClass="courseMainTitle"></asp:Label>
                <div class="courseRating_wrapper">
                    <i class="fa fa-star"></i>
                    <asp:Label ID="lblCourseRating" runat="server" Text=""></asp:Label>
                </div>
            </div>

            <div id="courseTag_wrapper" class="courseTag_wrapper" runat="server"></div>


            <div id="courseTutorContainer" class="courseTutor_container" runat="server">
            </div>

            <div class="courseDesc_wrapper">
                <asp:Label ID="lblCourseDesc" runat="server" Text="" CssClass="courseDesc"></asp:Label>
            </div>
            <div class="courseInfo_wrapper">
                <div class="courseLevel_wrapper">
                    <p class="courseInfo_label">Level</p>
                    <asp:Label ID="lblCourseLevel" runat="server" Text="" CssClass="courseInfo"></asp:Label>
                </div>
                <div class="courseLanguage_wrapper">
                    <p class="courseInfo_label">Language</p>
                    <asp:Label ID="lblCourseLang" runat="server" Text="" CssClass="courseInfo"></asp:Label>
                </div>
                <div class="courseEnrolCount_wrapper">
                    <p class="courseInfo_label">Enrolment</p>
                    <asp:Label ID="lblCourseEnrolCount" runat="server" Text="" CssClass="courseInfo"></asp:Label>
                </div>
                <div class="courseEnrolStatus_wrapper">
                    <p class="courseInfo_label">Status</p>
                    <asp:Label ID="lblCourseEnrolStatus" runat="server" Text="" CssClass="courseInfo"></asp:Label>
                </div>

            </div>

            <div class="courseFeeEnrolBtn_wrapper">
                <div class="courseFee_wrapper">
                    <asp:Label ID="lblCourseFeeLabel" runat="server" Text="$" CssClass="courseFee_label"></asp:Label>
                    <asp:Label ID="lblCourseFee" runat="server" Text="" CssClass="courseFee"></asp:Label>
                </div>
                <div class="courseEnrolBtn_wrapper" runat="server">
                    <asp:Button ID="btnEnrol" runat="server" CssClass="enrolViewButton" Text="Enrol" OnClick="btnEnrol_Click" />
                    <asp:Button ID="btnViewCourse" runat="server" CssClass="enrolViewButton" Text="View Course" OnClick="btnViewCourse_Click" />
                </div>
            </div>
        </div>
    </section>

    <!--Section 2 : Objective & Prerequisite & Recommendation-->
    <section id="objCommentPreRecommend">
        <div class="objComment">
            <!--LEFT SECTION 1 : Objective-->
            <div id="objective">
                <h3 class="objectiveTitle">Learning Outcome</h3>
                <asp:Label ID="lblCourseObj" runat="server" Text="" CssClass="objectiveDetails"></asp:Label>
            </div>

            <!--LEFT SECTION 2 : Comment-->
            <div id="comment">
                <h3 class="commentTitle">Comments
                    <asp:Label ID="lblCommentCount" runat="server" Text=""></asp:Label>
                </h3>

                <div class="commentInput_wrapperOuter">
                    <div class="commentInput_wrapper">
                        <div id="commentProfileImg" class="commentProfileImg" runat="server"></div>
                        <asp:TextBox ID="txtComment" runat="server" placeholder="Write a comment" CssClass="commentTextBox" OnTextChanged="txtComment_TextChanged"></asp:TextBox>
                        <asp:Button ID="btnSendComment" runat="server" CssClass="btnSendComment" OnClick="btnSendComment_Click" Text="" />
                    </div>
                    <h3>Rate This Course</h3>
                    <div class="ratingInputWrapper">
                        <asp:HiddenField ID="hfSliderValue" runat="server" Value="" />
                        <asp:TextBox ID="sliderCourseRating" runat="server" CssClass="sliderCourseRating" TextMode="Range" Min="0" Max="50"></asp:TextBox>
                        <div id="ratingSelector">
                            <div class="ratingSelectorBtn" id="ratingSelectorBtn" runat="server"></div>
                        </div>

                        <script type="text/javascript">
                            window.onload = function () {
                                var slider = document.getElementById("<%= sliderCourseRating.ClientID %>");
                                var hiddenField = document.getElementById("<%= hfSliderValue.ClientID %>");
                                var ratingSelectorWrapper = document.getElementById("ratingSelector");
                                var ratingDisplayButton = document.querySelector(".ratingSelectorBtn");

                                // Set the initial slider value and ensure the display button shows 0.0 initially
                                slider.value = 0; // Set initial value to 0
                                hiddenField.value = ""; // Clear the hidden field
                                ratingDisplayButton.innerHTML = "0.0"; // Display 0.0 initially
                                ratingSelectorWrapper.style.left = "0px"; // Reset the selector position to the start

                                // Function to update the selector's position based on the slider value
                                function updateRatingSelectorPosition() {
                                    var sliderWidth = slider.offsetWidth;
                                    var sliderValue = parseFloat(slider.value || 0); // Ensure the value is parsed to a number

                                    // Calculate the left position in pixels based on the slider value
                                    var newLeftPosition = (sliderValue - slider.min) / (slider.max - slider.min) * sliderWidth;

                                    // Apply the new position to the ratingSelector
                                    ratingSelectorWrapper.style.left = newLeftPosition + "px";

                                    // Always display the value, even if it's 0
                                    var displayValue = (sliderValue / 10).toFixed(1); // Divide by 10 to scale the value to 0.0 - 5.0
                                    ratingDisplayButton.innerHTML = displayValue; // Always display the value
                                }

                                // When the slider value changes
                                slider.addEventListener("input", function () {
                                    // Update the hidden field value with the slider value
                                    hiddenField.value = slider.value !== "" ? slider.value : ""; // Allow an empty string if no value is set

                                    updateRatingSelectorPosition(); // Update the selector position
                                });

                                // Update position when the page loads (in case a value was already set)
                                updateRatingSelectorPosition();
                            };
                        </script>




                    </div>
                </div>
                <div id="commentHistory_wrapper" class="commentHistory_wrapper" runat="server">
                </div>
            </div>
        </div>

        <div class="preRecommend">
            <!--RIGHT Section 1 : Prerequisite Courses-->
            <div id="PrerequisiteSlider" class="prerequisite" runat="server">
            </div>



            <!--RIGHT SECTION 2 : Recommendation-->
            <div id="RecommendationSlider" class="recommendation" runat="server">
            </div>
        </div>
    </section>

    <script type="text/javascript" async>
        document.addEventListener("DOMContentLoaded", function () {
            // Configurations for sliders
            const sliders = [
                {
                    groupSelector: ".recommendation .slideBar_wrapper",
                    leftBtnSelector: "#recommendSlideLeftBtn",
                    rightBtnSelector: "#recommendSlideRightBtn",
                    minCards: 4 // Require at least 4 items
                },
                {
                    groupSelector: ".prerequisite .slideBar_wrapper",
                    leftBtnSelector: "#preSlideLeftBtn",
                    rightBtnSelector: "#preSlideRightBtn",
                    minCards: 4 // Changed to 4 items
                }
            ];

            sliders.forEach(slider => {
                const cardGroup = document.querySelector(slider.groupSelector);
                const leftButton = document.querySelector(slider.leftBtnSelector);
                const rightButton = document.querySelector(slider.rightBtnSelector);

                // Check the number of items
                const itemCount = cardGroup.children.length;

                // Show or hide buttons based on the card count
                if (itemCount < slider.minCards) {
                    leftButton.style.display = "none";
                    rightButton.style.display = "none";
                } else {
                    leftButton.style.display = "block";
                    rightButton.style.display = "block";
                }
            });
        });

        // Unified slide function
        function slide(slideBtn) {
            const isRecommend = slideBtn.id.includes("recommend");
            const cardGroupSelector = isRecommend
                ? ".recommendation .slideBar_wrapper" // true: belongs to recommendation
                : ".prerequisite .slideBar_wrapper";  // false: belongs to prerequisite

            const cardGroup = document.querySelector(cardGroupSelector);
            const cardWidth = cardGroup.firstElementChild.offsetWidth; // Width of one card
            const gap = parseInt(window.getComputedStyle(cardGroup).gap) || 0; // Gap between cards
            const slideDistance = cardWidth + gap; // Total slide distance
            const maxSlide = cardGroup.scrollWidth - cardGroup.parentElement.offsetWidth; // Max scrollable distance

            // Track slider index per section
            let sliderIndex = isRecommend ? window.recommendIndex || 0 : window.prerequisiteIndex || 0;

            if (slideBtn.id.includes("Right")) {
                // Move right
                if (sliderIndex < maxSlide) {
                    sliderIndex += slideDistance;
                    if (sliderIndex > maxSlide) sliderIndex = maxSlide;
                }
            } else {
                // Move left
                if (sliderIndex > 0) {
                    sliderIndex -= slideDistance;
                    if (sliderIndex < 0) sliderIndex = 0;
                }
            }

            // Apply transform
            cardGroup.style.transform = `translateX(-${sliderIndex}px)`;

            // Update global index
            if (isRecommend) {
                window.recommendIndex = sliderIndex;
            } else {
                window.prerequisiteIndex = sliderIndex;
            }
        }
    </script>

</asp:Content>
