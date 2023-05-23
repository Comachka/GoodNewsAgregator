var hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/commentsHub")
    .build();


hubConnection.on("ReceiveMessage", function (content, avatar, user, dateCreated, raiting) {
    const commentBlock = document.getElementsByClassName('comments')[0];
    let dateTime = new Date(dateCreated);
    let date = dateTime.toLocaleDateString();
    let time = dateTime.toLocaleTimeString();
    let div = document.createElement("div");
    div.classList.add('comment-item');
    div.innerHTML = `<img class="comment-avatar" src="${avatar}" "alt="Аватар">
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
    if (contentComment) {
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
        let content = resp.content;
        console.log(content);
        let avatar = resp.avatar;
        console.log(avatar);

        let user = resp.user;
        console.log(user);

        let dateCreated = resp.dateCreated;
        let raiting = resp.raiting;
        console.log(raiting);

        hubConnection.invoke("SendMessage", content, avatar, user, dateCreated, raiting);
        document.querySelector('#content-area').style.borderColor = '#dee2e6';
    } else {
        document.querySelector('#content-area').style.borderColor = 'red';
    }
}


document.querySelector('.add-comment-form')
    .addEventListener('submit', (e) => addComment(e));
