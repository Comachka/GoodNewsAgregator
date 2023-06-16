document.getElementById('content-area')
    .addEventListener('keydown', (e) => {
        if (e.keyCode == 13) {
            document.getElementById('add-comment-btn').click();
            document.getElementById('content-area').blur();
            document.getElementById('content-area').value = '';
        }
    });



document.getElementById('commments-toggle-btn')
    .addEventListener('click', (e) => collapseCommentsWithJsonData(e));


function addCommentsHtml(resp, commentBlock) {
    for (let comment of resp) {
        let dateTime = new Date(comment.dateCreated);
        let date = dateTime.toLocaleDateString();
        let time = dateTime.toLocaleTimeString();
        let div = document.createElement("div");
        div.classList.add('comment-item');
        div.innerHTML = `<form action="https://localhost:7245/Account/Profile/${comment.userId}"><button style="border:none; height: 64px; background: none;" сlass="btn-profile" type="submit"><img class="comment-avatar" src="${comment.avatar}" alt="Аватар"/></button></form>
                <div class="comment-content">
                    <h5 class="comment-title">${comment.user}</h5> <p style="word-break: break-word;">${comment.content}</p> <p class="comment-date">${date}  ${time}</p>
                </div>`
        commentBlock.appendChild(div);
    }
}

document.querySelector('#content-area').oninput = (e => {
    e.target.value.trim().length ? document.querySelector('#content-area').style.borderColor = '#dee2e6' : document.querySelector('#content-area').style.borderColor = 'red';
})


async function collapseCommentsWithJsonData(e) {
    const btn = e.currentTarget;
    const commentBlock = document.getElementsByClassName('comments')[0];
    if (btn.classList.contains('commments-toggle-btn-close')) {
        const url = window.location.href;
        const articleId = url.substring(url.lastIndexOf('/') + 1);
        let commentsViewResponse = await fetch(
            `https://localhost:7245/Comment/GetFakeComments/?articleId=${articleId}`);
        let resp = await commentsViewResponse.json();
        addCommentsHtml(resp, commentBlock);
        btn.classList.remove('commments-toggle-btn-close');
    } else {
        btn.classList.add('commments-toggle-btn-close');
        commentBlock.innerHTML = '';
    }
    const commentsCollapse = new bootstrap.Collapse('#collapsisbleComments', {
        toggle: true
    })
}


let html = document.querySelector('.row').innerHTML;
html = html.replaceAll('amp-img', 'img');
document.querySelector('.row').innerHTML = html;
