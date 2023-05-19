window.addEventListener("DOMContentLoaded", (event) => {
    let switcherThemeBtn = document.getElementById('theme-switcher-btn');

    switcherThemeBtn.addEventListener('change', function () {
        let isDarkTheme = switcherThemeBtn.checked;

        if (isDarkTheme) {
            setHomePageDark();
        } else {
            setHomePageLight();
        }
    });

});

function setHomePageDark() {
    let cards = document.getElementsByClassName('card');
    for (let card of cards) {
        card.classList.add('text-bg-warning');
    }

    let body = document.getElementsByTagName('body')[0];
    body.style.backgroundColor = 'slategrey';
}

function setHomePageLight() {
    let cards = document.getElementsByClassName('card');
    for (let card of cards) {
        card.classList.remove('text-bg-warning');
    }

    let body = document.getElementsByTagName('body')[0];
    body.style.backgroundColor = '';
}