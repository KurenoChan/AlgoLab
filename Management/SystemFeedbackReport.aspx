<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="SystemFeedbackReport.aspx.cs" Inherits="AlgoLab.Management.SystemFeedbackReport" %>

<%-- Custom Page Title --%>
<asp:Content ID="SystemFeedbackReportTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    System Feedback Report
</asp:Content>

<%-- Custom Style Sheet --%>
<asp:Content ID="SystemFeedbackReportHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/systemFeedbackReport.css" rel="stylesheet" type="text/css" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script type="text/javascript">
        // JavaScript to initialize charts (to be expanded in detail)
        document.addEventListener('DOMContentLoaded', function () {
            // Data received from the server-side C#
            console.log(chartData);

            const ctx = document.getElementById('ratingSummaryChart').getContext('2d');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: ['Content', 'Instructor', 'Platform', 'Performance'], // Bar labels
                    datasets: [{
                        label: 'Average Ratings',
                        data: [
                            chartData.contentRating,
                            chartData.instructorRating,
                            chartData.platformRating,
                            chartData.performanceRating
                        ],
                        backgroundColor: [
                            'rgba(75, 192, 192, 0.5)', // Content color
                            'rgba(255, 99, 132, 0.5)', // Instructor color
                            'rgba(255, 205, 86, 0.5)', // Platform color
                            'rgba(54, 162, 235, 0.5)'  // Performance color
                        ],
                        borderColor: [
                            'rgba(75, 192, 192, 1)',
                            'rgba(255, 99, 132, 1)',
                            'rgba(255, 205, 86, 1)',
                            'rgba(54, 162, 235, 1)'
                        ],
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            display: true,
                            labels: {
                                font: {
                                    family: 'Poppins',  // Apply Poppins font to legend labels
                                    weight: 'normal',    // Normal weight for legend
                                }
                            }
                        }, // Show legend
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            max: 5, // Ratings range from 0 to 5
                            ticks: {
                                stepSize: 1, // Ensure step size is 1
                                font: {
                                    family: 'Poppins',  // Apply Poppins font to y-axis ticks
                                    weight: 'normal',   // Normal weight for y-axis
                                }
                            }
                        },
                        x: {
                            ticks: {
                                font: {
                                    family: 'Poppins',  // Apply Poppins font to x-axis ticks
                                    weight: 'normal',   // Normal weight for x-axis
                                }
                            }
                        }
                    },
                    title: {
                        display: true,
                        text: 'Average Ratings for Different Categories',  // Chart title
                        font: {
                            family: 'Poppins',  // Apply Poppins font to title
                            weight: 'bold',      // Bold weight for the title
                            size: 18             // Adjust size if necessary
                        }
                    }
                }
            });
        });

    </script>
</asp:Content>

<%-- Custom Style Body --%>
<asp:Content ID="SystemFeedbackReportBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <section id="ratingSummary_customerComments">
        <!--Section 1 : Rating Summary [Charts]-->
        <section id="ratingSummary">
            <h2 class="systemFeedbackReport_title">Rating Summary</h2>
            <div class="ratingSummary_chartWrapper">
                <canvas id="ratingSummaryChart" class="ratingSummaryChart"></canvas>
            </div>
        </section>

        <!-- Section 2 : List of Customer Comments -->
        <section id="customerComments">
            <h2 class="systemFeedbackReport_title">Comments</h2>
            <div class="commentsWrapper">
                <asp:Repeater ID="RepeaterCustomerComments" runat="server">
                    <HeaderTemplate>
                        <div class="commentsList">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="commentCard">
                            <div class="commentNameDate">
                                <strong><%# Eval("CustomerName") %></strong>
                                <p class="commentTimestamp"><%# Eval("FeedbackDateTime", "{0:yyyy-MM-dd HH:mm}") %></p>
                            </div>
                            <p class="comment"><%# Eval("Comment") %></p>

                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </section>

    </section>

</asp:Content>
