let needHiden = document.querySelectorAll('.need-hide');
let hiden = document.querySelectorAll('.hiden');

document.getElementById('btn-start-hide').addEventListener('click', () => {
    needHiden.forEach(e => { e.style.display = 'none' })
    hiden.forEach(e => { e.style.display = 'block' })
})

document.getElementById('btn-stop-hide').addEventListener('click', () => {
    needHiden.forEach(e => { e.style.display = 'flex' })
    hiden.forEach(e => { e.style.display = 'none' })
})