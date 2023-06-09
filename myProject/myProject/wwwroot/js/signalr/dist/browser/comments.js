var hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/commentsHub")
    .build();


hubConnection.on("ReceiveMessage", function (content, avatar, user, dateCreated, raiting, userId) {
    const commentBlock = document.getElementsByClassName('comments')[0];
    let dateTime = new Date(dateCreated);
    let date = dateTime.toLocaleDateString();
    let time = dateTime.toLocaleTimeString();
    let div = document.createElement("div");
    div.classList.add('comment-item');
    div.innerHTML = `<form action="https://localhost:7245/Account/Profile/${userId}"><button сlass="btn_profile" type="submit"><img class="comment-avatar" src="${avatar}" alt="Аватар"/></button></form>
                <div class="comment-content">
                    <h5 class="comment-title">${user}</h5> <p>${content}</p> <p class="comment-date">${date}  ${time}</p>
                </div>`
    commentBlock.appendChild(div);
});

hubConnection.start();

async function addComment(e) {
    e.preventDefault();
    let contentComment = document.querySelector('#content-area').value;
    let articleIdJson = parseInt(document.querySelector('#article-id').value);
    document.querySelector('#content-area').value = "";
    if (contentComment.trim().length) {
        let commentsViewResponse = await fetch('https://localhost:7245/Comment/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({
                UserId: null,
                User: null,
                Avatar: null,
                Content: contentComment,
                Raiting: 0,
                DateCreated: null,
                ArticleId: articleIdJson
            })
        }).catch(error => {
            console.error('Error:', error);
        });
        let resp = await commentsViewResponse.json();
        if (!resp) {
            console.error('Error: Something goes wrong');
            return;
        }

        console.log(resp);
        let userId = resp.userId;
        let content = resp.content;
        let avatar = resp.avatar;
        let user = resp.user;
        let dateCreated = resp.dateCreated;
        let raiting = resp.raiting;
        hubConnection.invoke("SendMessage", content, avatar, user, dateCreated, raiting, userId);
        document.querySelector('#content-area').style.borderColor = '#dee2e6';
    } else {
        document.querySelector('#content-area').style.borderColor = 'red';
    }
}


document.querySelector('.add-comment-form')
    .addEventListener('submit', (e) => addComment(e));
