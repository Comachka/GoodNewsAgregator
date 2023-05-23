document.getElementById('commments-toggle-btn')
    .addEventListener('click', (e) => collapseCommentsWithJsonData(e));

//document.querySelector('.add-comment-form')
//    .addEventListener('submit', (e) => addComment(e));


function addCommentsHtml(resp, commentBlock) {
    for (let comment of resp) {
        let dateTime = new Date(comment.dateCreated);
        let date = dateTime.toLocaleDateString();
        let time = dateTime.toLocaleTimeString();
        let div = document.createElement("div");
        div.classList.add('comment-item');
        div.innerHTML = `<img class="comment-avatar" src="${comment.avatar}" "alt="Аватар"> 
                <div class="comment-content">
                    <h5 class="comment-title">${comment.user}</h5> <p>${comment.content}</p> <p class="comment-date">${date}  ${time}</p>
                </div>`
        commentBlock.appendChild(div);
    }
}

//async function addComment(e) {
//    e.preventDefault();
//    let contentComment = document.querySelector('#content-area').value;
//    let articleIdJson = parseInt(document.querySelector('#article-id').value);
//    document.querySelector('#content-area').value = "";
//    if (contentComment) {
//        let commentsViewResponse = await fetch('https://localhost:7245/Comment/Create', {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/json',
//                'Accept': 'application/json'
//            },
//            body: JSON.stringify({
//                UserId: null,
//                User: null,
//                Avatar: null,
//                Content: contentComment,
//                Raiting: 0,
//                DateCreated: null,
//                ArticleId: articleIdJson
//            })
//        }).catch(error => {
//            console.error('Error:', error);
//        });
//        let resp = await commentsViewResponse.json();
//        if (!resp) {
//            console.error('Error: Something goes wrong');
//            return;
//        }
//        const commentBlock = document.getElementsByClassName('comments')[0];
//        commentBlock.innerHTML = '';
//        addCommentsHtml(resp, commentBlock);
//        document.querySelector('#content-area').style.borderColor = '#dee2e6';
//    } else {
//        document.querySelector('#content-area').style.borderColor = 'red';
//    }
//}

document.querySelector('#content-area').oninput = (e => {
    e.target.value.length ? document.querySelector('#content-area').style.borderColor = '#dee2e6' : document.querySelector('#content-area').style.borderColor = 'red';
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