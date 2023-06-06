document.getElementByClass('btn-search')
    .addEventListener('click', getSearchArticle());

async function getSearchArticle() {
    const searchText = document.getElementById('article-search').value.trim();
   
    if (searchText.length) {
        let searchResponse = await fetch(
            `https://localhost:7245/Article/Search/?searchText=${searchText}`);
        let resp = await searchResponse.json();
       
    } else {
       
    }
}




async function getArticleNamesData() {
    let artcleNamesResponse = await fetch(
        'https://localhost:7245/Article/GetArticlesNames');

    let data = await artcleNamesResponse.json();


    const ac = new Autocomplete(document.getElementById('article-search'), {
        data: data,
        maximumItems: 10,
        treshold: 3,
        onSelectItem: ({ label, value }) => {
            console.log("article selected:", label, value);
            location.replace(`/article/details/${value}`);

        }
    });
}

const search = document.querySelector('#article-search');
const newsTitle = document.querySelectorAll('.article-list .card .card-title');
console.log(newsTitle);
search.addEventListener('input', (e) => {
    console.log(e.target.value);
    const searchValue = e.target.value;
    console.log(newsTitle);
    newsTitle = newsTitle.filter(item => {
        return item.includes(searchValue);
    });
    console.log(newsTitle);
})

getArticleNamesData();

