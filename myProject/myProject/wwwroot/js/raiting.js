const emoji = document.querySelectorAll('.emoji');

emoji.forEach(e => {
    let starBar = e.querySelector('.star-bar');
    const svgArray = e.querySelectorAll('.emoji svg');
    let count = starBar.getAttribute('value');
    if (count <= 25) {
        svgArray[0].style.display = 'block';
    }
    else if (count > 25 && count <= 50) {
        svgArray[1].style.display = 'block';
    }
    else if (count > 50 && count <= 75) {
        svgArray[2].style.display = 'block';
    }
    else if (count > 75 && count <= 100) {
        svgArray[3].style.display = 'block';
    }
})