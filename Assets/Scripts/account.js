







var a = document.getElementById("loginBtn");
var b = document.getElementById("registerBtn");
var x = document.getElementById("login");
var y = document.getElementById("register");
function login() {
    window.location.href = "AccountSelect.aspx";
}
function register() {
    window.location.href = "SignUp.aspx";
}
document.addEventListener("DOMContentLoaded", () => {
    const images = [
        "../Assets/Images/account/image1.jpg",
        "../Assets/Images/account/image2.jpg",
        "../Assets/Images/account/image3.jpg"
    ];

    const slider = document.querySelector('.slider');
    let currentIndex = 0;

    function changeBackgroundImage() {
        slider.style.backgroundImage = `url('${images[currentIndex]}')`;
        currentIndex = (currentIndex + 1) % images.length; // Loop back to the start
    }

    // Start the image change interval
    setInterval(changeBackgroundImage, 1000); // Change image every 1 second

    // Initialize with the first image
    changeBackgroundImage();
});