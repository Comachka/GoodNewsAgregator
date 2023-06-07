using AutoMapper;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Business.RateModels;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;

namespace myProject.Business
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; // Convert(article) => _mapper.Map<ArticleDto>(article);


        public ArticleService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> GetTotalArticlesCountAsync()
        {
            var count = await _unitOfWork.Articles.CountAsync();
            return count;
        }

        public async Task<IEnumerable<CategoryDto>> GetListCategoriesAsync()
        {
            return await _unitOfWork.Categories
                .GetAsQueryable()
                .Select(category => _mapper.Map<CategoryDto>(category))
                .ToListAsync();
        }

        public async Task<List<ArticleDto>> GetArticlesByPageAsync(int page, int pageSize)
        {
            try
            {
                var articles = (await _unitOfWork
                        .Articles
                        .GetArticlesForPageAsync(page, pageSize))
                    .Select(article => _mapper.Map<ArticleDto>(article))
                    .ToList();
                return articles;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public async Task<ArticleDto?> GetArticleByIdWithSourceNameAsync(int id)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            var source = await _unitOfWork.NewsResources.GetByIdAsync(article.NewsResourceId);
            article.NewsResource = source;
            if (article != null)
            {
                var dto = _mapper.Map<ArticleDto>(article);
                dto.SourceName = source.Name;
                dto.ArticleSourceUrl = source.OriginUrl;
                return dto;
            }
            return null;
        }

        public async Task<List<AutoCompleteDataDto>> GetArticlesNamesByPartNameAsync(string partName)
        {
            var articles = await _unitOfWork.Articles
                .GetAsQueryable()
                .AsNoTracking()
                .Where(article => article.Title.Contains(partName))
                .Select(article => _mapper.Map<AutoCompleteDataDto>(article))
                .ToListAsync();

            return articles;

        }

        public async Task AddAsync(ArticleDto dto)
        {
            await _unitOfWork.Articles.AddAsync(_mapper.Map<Article>(dto));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddArticlesAsync(IEnumerable<ArticleDto> articles)
        {
            var entities = articles.Select(a => _mapper.Map<Article>(a)).ToArray();

            await _unitOfWork.Articles.AddRangeAsync(entities);
            await _unitOfWork.SaveChangesAsync();

        }

        //aggregator
        public async Task<List<ArticleDto>> AggregateArticlesDataFromRssSourceAsync(NewsResourceDto source, CancellationToken cancellationToken)
        {
            var articles = new ConcurrentBag<ArticleDto>();
            var urls = await GetContainsArticleUrlsBySourceAsync(source.Id);
            using (var reader = XmlReader.Create(source.RssFeedUrl))
            {
                var feed = SyndicationFeed.Load(reader);

                await Parallel.ForEachAsync(feed.Items
                        .Where(item => !urls.Contains(item.Id)).ToArray(), cancellationToken,
                    (item, token) =>
                    {
                        articles.Add(new ArticleDto()
                        {
                            ArticleSourceUrl = item.Links[0].Uri.AbsoluteUri,
                            NewsResourceId = source.Id,
                            SourceName = source.Name,
                            Title = item.Title.Text,
                            ShortDescription = item.Summary.Text,
                            DatePosting = item.PublishDate.DateTime,
                            CategoryId = CategoryCheck(item.Categories[0].Name)
                        });
                        return ValueTask.CompletedTask;
                    });

                reader.Close();
            }

            return articles.ToList();
        }

        private int CategoryCheck(string category)
        {
            int id = 0;
            if (category.IndexOf(':') > 0)
            {
                category = category.Substring(0, category.IndexOf(':'));
            }

            switch (category.Trim().ToUpper())
            {
                case "ЗРОБЛЕНА БЕЛАРУСАМI":
                case "КУЛЬТУРА":
                    id = 1; // культура
                    break;
                case "ЛАЙФСТАЙЛ":
                case "ОБЩЕСТВО":
                    id = 1002; // общество
                    break;
                case "ПОЛИТИКА":
                    id = 3; // политика
                    break;
                case "СПОРТ":
                    id = 4; //спорт
                    break;
                case "АВТО":
                case "ТЕХНОЛОГИИ":
                case "НАУКА":
                    id = 2; // технологии и наука
                    break;
                case "КОШЕЛЕК":
                case "НЕДВИЖИМОСТЬ":
                case "ЭКОНОМИКА":
                    id = 5; // экономика
                    break;
                default:
                    id = 1003; // другое
                    break;
            }
            return id;
        }

        public async Task<List<ArticleDto>> GetFullContentArticlesAsync(List<ArticleDto> articlesDataFromRss)
        {
            var concBag = new ConcurrentBag<ArticleDto>();

            await Parallel.ForEachAsync(articlesDataFromRss, async (dto, token) =>
            {
                var content = await GetArticleContentAsync(dto.ArticleSourceUrl);
                if (content != "")
                {
                dto.Content = content;
                concBag.Add(dto);
                }
            });
            return concBag.ToList();
        }

        public async Task<double?> GetArticleRateAsync(int articleId)
        {
            var articleText = (await _unitOfWork.Articles.GetByIdAsync(articleId))?.Content;


            if (string.IsNullOrEmpty(articleText))
            {
                throw new ArgumentException("Article or article text doesn't exist",
                    nameof(articleId));
            }
            else
            {
                Dictionary<string, int>? dictionary;
                using (var jsonReader = new StreamReader($"{Environment.CurrentDirectory}\\AFINN-ru.json"))
                {
                    var jsonDict = await jsonReader.ReadToEndAsync();
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonDict);
                }

                articleText = PrepareText(articleText);

                using (var httpClient = new HttpClient())
                {
                    httpClient
                        .DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    var request = new HttpRequestMessage(HttpMethod.Post,
                        "http://api.ispras.ru/texterra/v1/nlp?targetType=lemma&apikey=e692c633ea66c56af621c5b4c7cd28dea941fa86")
                    {
                        Content = new StringContent("[{\"text\":\"" + articleText + "\"}]",
                            Encoding.UTF8, "application/json")
                    };

                    request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var lemmas = JsonConvert.DeserializeObject<Root[]>(responseString)
                            .SelectMany(root => root.Annotations.Lemma).Select(lemma => lemma.Value).ToArray();

                        if (lemmas.Any())
                        {
                            var totalRate = lemmas
                                .Where(lemma => dictionary.ContainsKey(lemma))
                                .Aggregate<string, double>(0, (current, lemma)
                                    => current + dictionary[lemma]);

                            totalRate = totalRate / lemmas.Count();
                            return totalRate;
                        }
                    }

                }
                return null;
            }
        }

        private string? PrepareText(string articleText)
        {
            articleText = articleText.Trim();

            articleText = Regex.Replace(articleText, "<.*?>", string.Empty);
            return articleText;
        }

        public async Task<List<ArticleDto>> GetUnratedArticlesAsync()
        {
            var unratedArticles = await _unitOfWork.Articles
                .GetAsQueryable()
                .AsNoTracking()
                .Where(article => article.PositiveRaiting == null)
                .Select(article => _mapper.Map<ArticleDto>(article))
                .ToListAsync();

            return unratedArticles;
        }

        private async Task<string[]> GetContainsArticleUrlsBySourceAsync(int sourceId)
        {
            var articleUrls = await _unitOfWork.Articles.GetAsQueryable()
                .Where(article => article.NewsResourceId.Equals(sourceId))
                .Select(article => article.ArticleSourceUrl)
                .ToArrayAsync();
            return articleUrls;
        }

        private async Task<string> GetArticleContentAsync(string url)
        {
            try
            { //если майл ру добваь амп
                
                var content = "";

                if (url.Contains("onliner"))
                {
                    var web = new HtmlWeb();
                    var doc = web.Load(url);
                    content = DeleteNodes(doc, "//div[@class = 'news-text']", "//div[@id = 'news-text-end']", new List<string>
                    {
                        "//div[contains(@class, 'news-reference') or contains(@class, 'news-widget') or contains(@class, 'news-incut') or starts-with(@id, 'adfox') or contains(@class, 'news-header')]",
                        "//script",
                        "//a[contains(@class, 'news-banner')]"
                    });
                }
                else
                {
                    var web = new HtmlWeb();
                    var urlMail = url.Insert((url.IndexOf("ru") + 2), "/amp");
                    var doc = web.Load(urlMail);
                    var textNode = doc.DocumentNode.SelectSingleNode("//article");
                    content = DeleteNodes(doc, "//article", "", new List<string>
                    {
                        "//h1[contains(@class, 'article__title')]",
                        "//div[contains(@class, 'article__params') or contains(@class, 'news-widget') or contains(@class, 'rb-slot')]",
                        "//div[contains(@class, 'article__content')]//div[contains(@class, 'article__item_slot')]"
                    });
                }
                return content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private string DeleteNodes(HtmlDocument? doc, string startArticle, string endArticle, List<string> nodes)
        {
            var textNode = doc.DocumentNode.SelectSingleNode(startArticle);
            if (textNode == null) return "";
            if (endArticle != "")
            {
                var endNode = doc.DocumentNode.SelectSingleNode(endArticle);
                if (endNode != null)
                {
                    var nodesToRemove = textNode.Descendants()
                        .SkipWhile(n => n != endNode)
                        .ToList();
                    nodesToRemove.ForEach(n => n.Remove());

                }
            }

            foreach (var node in nodes)
            {
                textNode.SelectNodes(node)?
                   .ToList()
                   .ForEach(n => n.Remove());
            }
            return textNode.InnerHtml;
        }


        public async Task RateArticleAsync(int id, double? rate)
        {
            await _unitOfWork.Articles.PatchAsync(id, new List<PatchDto>()
            {
                new PatchDto()
                {
                    PropertyName = nameof(ArticleDto.PositiveRaiting),
                    PropertyValue = rate
                }
            });

            await _unitOfWork.SaveChangesAsync();

        }
    }
}