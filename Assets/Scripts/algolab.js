// Show the section content on scroll
window.addEventListener('scroll', reveal);

function reveal() {
    // Get all elements with the class 'reveal'
    const reveals = document.querySelectorAll('.reveal');

    // Loop through each element with the class 'reveal'
    for (let i = 0; i < reveals.length; i++) {
        // Get the height of the viewport
        const windowHeight = window.innerHeight;

        // Get the distance from the top of the element to the top of the viewport
        const revealTop = reveals[i].getBoundingClientRect().top;

        // Set a reveal point (100 pixels in this case) from the bottom of the viewport
        const revealPoint = 100;

        // Check if the element is within the reveal point from the bottom of the viewport
        if (revealTop < windowHeight - revealPoint) {
            // Add a delay to the 'active' class based on the element's index
            setTimeout(() => {
                reveals[i].classList.add('revealActive');
            }, i * 300); // 300ms delay between each card
        } else {
            // Remove the 'active' class if the element goes out of view (optional)
            reveals[i].classList.remove('revealActive');
        }
    }
}


window.addEventListener('scroll', typeText);
function typeText() {
    var typeTexts = document.querySelectorAll('.typeText');

    // Loop through each element with the class 'typeText'
    for (var j = 0; j < typeTexts.length; j++) {
        // Get the height of the viewport
        var windowHeight = window.innerHeight;

        // Get the distance from the top of the element to the top of the viewport
        var typeTop = typeTexts[j].getBoundingClientRect().top;

        // Set a typeText point (150 pixels in this case) from the bottom of the viewport
        var typePoint = 100;

        // Check if the element is within the typeText point from the bottom of the viewport
        if (typeTop < windowHeight - typePoint) {
            // If the element is within the typeText point, add the 'active' class
            typeTexts[j].classList.add('typeActive');
        }
        else {
            // If the element is outside the typeText point, remove the 'active' class
            typeTexts[j].classList.remove('typeActive');
        }
    }
}


window.addEventListener("scroll", () => {
    const scrollTop = window.scrollY;
    const zoomElements = document.querySelectorAll(".zoom"); // Select all elements with class 'zoom'

    zoomElements.forEach((element) => {
        const elementHeight = element.offsetHeight; // Get the height of the element
        const scaleFactor = 1 + Math.min(scrollTop / elementHeight, 1); // Cap the zoom effect
        element.style.transform = `scale(${scaleFactor})`;
    });
});