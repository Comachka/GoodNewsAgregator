let hiden = document.querySelectorAll('.admin-hiden');
checkAndHide();
async function checkAndHide() {
    let indexViewResponse = await fetch(
        `https://localhost:7245/Account/GetRole`);
    let resp = await indexViewResponse.json();
    if (resp == 1) {
            hiden.forEach(e => { e.style.display = 'block' })
    }
}
