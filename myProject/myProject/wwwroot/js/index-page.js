let hiden = document.querySelectorAll('.admin-hiden');
let hidenSM = document.querySelectorAll('.supModer-hiden');
let hidenM = document.querySelectorAll('.moder-hiden');
checkAndHide();
async function checkAndHide() {
    let indexViewResponse = await fetch(
        `https://localhost:7245/Account/GetRole`);
    let resp = await indexViewResponse.json();
    if (resp == 1) {
        hiden.forEach(e => { e.style.display = 'block' })
    }
    else if (resp == 4) {
        hidenSM.forEach(e => { e.style.display = 'block' })
    }
    else if (resp == 2) {
        hidenM.forEach(e => { e.style.display = 'block' })
    }
}
