<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="AlgoLab.Home" %>

<%--Custom Page Title--%>
<asp:Content ID="IndexTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    AlgoLab
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="IndexHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="Assets/Stylesheets/home.css" rel="stylesheet" type="text/css" />
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

        function searchSuggestion(suggestedItem) {
            var homeSearchBox = document.querySelector('.txtHomeSearchBox');
            homeSearchBox.value = suggestedItem.innerText; // to clear the search content
            homeSearchBox.focus();
            //document.forms["home_searchForm"].submit();
        }



    </script>
</asp:Content>

<%--Custom Page Body--%>
<asp:Content ID="IndexBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <!--Section 1 : Hero Section-->
    <section id="hero">
        <div class="hero_backImg zoom"></div>
        <div class="hero_container">
            <div class="hero_title">
                <h2>Unlock your potential with expert courses.</h2>
            </div>
            <div class="hero_desc">
                <p>Learn from industry professionals and advance your skills in key areas, all at your own pace.</p>
            </div>
            <div class="hero_btnWrapper">
                <a href="Course.aspx">
                    <div class="btnToPage">
                        <p>Explore Courses</p>
                    </div>
                </a>

                <a href="SignUp.aspx">
                    <div class="btnToSection">
                        <p>Get Started for free &#129106;</p>
                    </div>
                </a>
            </div>
        </div>
    </section>

    <!--Section 2 : Featured Course-->
    <section id="featured">
        <h2 class="sectionHeading">Featured</h2>
        <p class="sectionDesc">Unlock your potential with courses curated by industry leaders to advance your skills.</p>

        <div class="sectionBody reveal">
            <div class="featured_wrapper" id="FeaturedWrapper" runat="server">
            </div>
        </div>

    </section>

    <!--Section 3 : Search Course-->
    <section id="searchCourse">
        <h2 class="sectionHeading">Find Course</h2>

        <div class="sectionBody">
            <h3 class="sectionSubheading typeText">Explore Courses That Shape Your Future</h3>
            <div class="searchFormWrapper">

                <div id="home_searchForm">
                    <asp:TextBox ID="txtHomeSearch" runat="server" placeholder="e.g. Artificial Intelligence" CssClass="txtHomeSearchBox"></asp:TextBox>
                    <asp:Button ID="btnHomeSearch" runat="server" CssClass="btnHomeSearch" Text="Search" OnClick="btnHomeSearch_Click" />
                </div>

                <div class="searchSuggest">
                    <p>Popular Topics</p>
                    <div class="searchSuggestWrapper">
                        <div class="searchSuggestBtn" onclick="searchSuggestion(this);">Machine Learning</div>
                        <div class="searchSuggestBtn" onclick="searchSuggestion(this);">Web Development</div>
                        <div class="searchSuggestBtn" onclick="searchSuggestion(this);">Data Science</div>
                        <div class="searchSuggestBtn" onclick="searchSuggestion(this);">Cybersecurity</div>
                        <div class="searchSuggestBtn" onclick="searchSuggestion(this);">Programming</div>
                        <div class="searchSuggestBtn" onclick="searchSuggestion(this);">UI/UX Design</div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!--Section 4 : Why Choose AlgoLab-->
    <section id="strength">
        <h2 class="sectionHeading">Why Choose AlgoLab</h2>
        <div class="sectionBody">
            <div class="strength_wrapper">
                <div class="strengthCard reveal">
                    <div class="strengthIconWrapper">
                        <div class="spinner"></div>
                        <img src="Assets/Images/home/home_strength_expert.png" alt="Expert Trainers" />
                    </div>
                    <h3>Expert-Led Training</h3>
                    <p>Gain insights from seasoned professionals with real-world experience in their fields.</p>
                </div>

                <div class="strengthCard reveal">
                    <div class="strengthIconWrapper">
                        <div class="spinner"></div>
                        <img src="Assets/Images/home/home_strength_careerSkills.png" alt="Career Skills" />
                    </div>
                    <h3>Career-Ready Skills</h3>
                    <p>Master practical skills that align with industry demands and career advancement.</p>
                </div>

                <div class="strengthCard reveal">
                    <div class="strengthIconWrapper">
                        <div class="spinner"></div>
                        <img src="Assets/Images/home/home_strength_flexible.png" alt="Flexible Learning Pace" />
                    </div>
                    <h3>Flexible Learning</h3>
                    <p>Learn at your own pace with schedules designed to suit your lifestyle.</p>
                </div>
            </div>
        </div>
    </section>

    <!--Section 5 : Partnership Section-->
    <section id="partner">
        <h2 class="sectionHeading">Our Partners</h2>
        <p class="sectionDesc">Benefit from partnerships with leading tech giants and educational institutions worldwide.</p>

        <div class="sectionBody">
            <div class="partnerWrapper">
                <div class="partnerSlider">
                    <div id="partnerSliderLeft" onclick="partnerSlide(this);">&#11164;</div>
                    <div id="partnerSliderRight" onclick="partnerSlide(this);">&#11166;</div>
                </div>
                <div class="partnerCardGroup">
                    <div class="partnerCard">
                        <img src="Assets/Images/home/home_partner_google.png" alt="Google" />
                    </div>
                    <div class="partnerCard">
                        <img src="Assets/Images/home/home_partner_aws.png" alt="AWS" />
                    </div>
                    <div class="partnerCard">
                        <img src="Assets/Images/home/home_partner_microsoft.png" alt="Microsoft" />
                    </div>
                    <div class="partnerCard">
                        <img src="Assets/Images/home/home_partner_ibm.png" alt="IBM" />
                    </div>
                    <div class="partnerCard">
                        <img src="Assets/Images/home/home_partner_adobe.png" alt="Adobe" />
                    </div>
                    <div class="partnerCard">
                        <img src="Assets/Images/home/home_partner_nvidia.png" alt="NVIDIA" />
                    </div>
                    <div class="partnerCard">
                        <img src="Assets/Images/home/home_partner_unity.png" alt="Unity" />
                    </div>
                    <div class="partnerCard">
                        <img src="Assets/Images/home/home_partner_github.png" alt="GitHub" />
                    </div>
                    <div class="partnerCard">
                        <img src="Assets/Images/home/home_partner_coursera.png" alt="Coursera" />
                    </div>
                    <div class="partnerCard">
                        <img src="Assets/Images/home/home_partner_cisco.png" alt="Cisco" />
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!--Section 6 : Testimonials / Success Stories-->
    <section id="testimonial">
        <h2 class="sectionHeading">Testimonials</h2>
        <div class="sectionBody">
            <div class="testimonialWrapper">
                <div class="testimonialCard">
                    <div class="testimonialBio">
                        <div class="testimonialImg"></div>
                        <div class="testimonialProfile">
                            <h3>John Doe</h3>
                            <h4>Software Engineer at Microsoft</h4>
                        </div>
                    </div>
                    <div class="testimonialComment">
                        <p>AlgoLab helped me master new programming techniques faster, boosting my productivity and confidence in every project.</p>
                    </div>
                </div>
                <div class="testimonialCard">
                    <div class="testimonialBio">
                        <div class="testimonialImg"></div>
                        <div class="testimonialProfile">
                            <h3>Sophia Martinez</h3>
                            <h4>Data Analyst at Spotify</h4>
                        </div>
                    </div>
                    <div class="testimonialComment">
                        <p>AlgoLab's online courses provided hands-on exercises that helped me grasp complex data concepts and apply them directly to real-world analytics projects.</p>
                    </div>
                </div>
                <div class="testimonialCard">
                    <div class="testimonialBio">
                        <div class="testimonialImg"></div>
                        <div class="testimonialProfile">
                            <h3>David Thompson</h3>
                            <h4>Machine Learning Engineer at NVIDIA</h4>
                        </div>
                    </div>
                    <div class="testimonialComment">
                        <p>The hands-on courses at AlgoLab gave me the confidence to tackle real-world AI challenges.</p>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!--Section 7 : Sign Up Account-->
    <section id="SignupBecomeTut" class="SignupBecomeTut" runat="server">
    </section>
</asp:Content>
