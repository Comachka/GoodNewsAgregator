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
using System.Web;
using myProject.DataCQS.Commands;
using myProject.DataCQS.Queries;
using MediatR;
using Serilog;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace myProject.Business
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ISourceService _sourceService;
        private readonly ICategoryService _categoryService;


        public ArticleService(IUnitOfWork unitOfWork,
            IMediator mediator,
            IMapper mapper,
            ISourceService sourceService,
            ICategoryService categoryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
            _sourceService = sourceService;
            _categoryService = categoryService;
        }

        public async Task EditArticleAsync(ArticleDto article)
        {
            var art = await _unitOfWork.Articles.GetByIdAsync(article.Id);
            if (art != null)
            {
                bool changed = false;
                if (article.Title != art.Title)
                {
                    await _unitOfWork.Articles.PatchAsync(art.Id, new List<PatchDto>()
                    {
                        new PatchDto()
                        {
                            PropertyName = nameof(ArticleDto.Title),
                            PropertyValue = article.Title
                        }
                    });
                    changed = true;
                }
                if (article.Content != art.Content)
                {
                    await _unitOfWork.Articles.PatchAsync(art.Id, new List<PatchDto>()
                    {
                        new PatchDto()
                        {
                            PropertyName = nameof(ArticleDto.Content),
                            PropertyValue = article.Content
                        }
                    });
                    changed = true;
                }
                if (article.ShortDescription != art.ShortDescription)
                {

                    await _unitOfWork.Articles.PatchAsync(art.Id, new List<PatchDto>()
                    {
                        new PatchDto()
                        {
                            PropertyName = nameof(ArticleDto.ShortDescription),
                            PropertyValue = article.ShortDescription
                        }
                    });
                    changed = true;
                }
                if (changed)
                {
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            else
            {
                throw new Exception($"Cant find article with id: {article.Id}");
            }
        }

        public async Task UpRaitingAsync(int id)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            if (article != null)
            {
                var rate = article.PositiveRaiting + 0.005;
                await _unitOfWork.Articles.PatchAsync(id, new List<PatchDto>()
            {
                new PatchDto()
                {
                    PropertyName = nameof(ArticleDto.PositiveRaiting),
                    PropertyValue = rate
                }
            });

                await _unitOfWork.SaveChangesAsync();
                return;
            }
            else
            {
                throw new Exception($"Cant find article with id: {id}");
            }
        }

        public async Task DeleteArticleByIdAsync(int id)
        {
            if (id >0)
            {
                await _unitOfWork.Articles.Remove(id);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Invalid page or pageSize");
            }
        }

        public async Task<int> GetTotalArticlesCountAsync(double raiting)
        {
            var articles = await _unitOfWork.Articles.FindBy(a => a.PositiveRaiting > raiting).ToListAsync();
            return articles.Count;
        }

        public async Task<List<ArticleDto>> GetArticlesByPageAsync(int page, int pageSize, double positivity)
        {
            try
            {
                if (page >= 0 && pageSize >= 1)
                {
                    var articles = (await _unitOfWork
                                            .Articles
                                            .GetArticlesForPageAsync(page, pageSize, positivity))
                                        .Select(article => _mapper.Map<ArticleDto>(article))
                                        .ToList();
                    foreach (var article in articles)
                    {
                        var category = await _unitOfWork.Categories.GetByIdAsync(article.CategoryId);
                        article.Category = category.Name;
                    }
                    return articles;
                }
                else
                {
                    throw new ArgumentException("Invalid page or pageSize");
                }
            }
            catch (ArgumentException ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }

        }

        public async Task<List<ArticleDto>> GetFavArticleAsync()
        {
            var articles = _unitOfWork
                    .Articles
                    .GetAsQueryable()
                    .OrderByDescending(a => a.PositiveRaiting)
                    .Take(3)
                    .Select(article => _mapper.Map<ArticleDto>(article))
                    .ToList();
            foreach(var article in articles)
            {
                var source = await _unitOfWork.NewsResources.GetByIdAsync(article.NewsResourceId);
                article.SourceName = source.Name;
            }
            return articles;
        }

        public async Task<ArticleDto?> GetArticleByIdWithSourceNameAsync(int id)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            var source = await _unitOfWork.NewsResources.GetByIdAsync(article.NewsResourceId);
            article.NewsResource = source;
            if (article != null)
            {
                if (source != null)
                {
                    var dto = _mapper.Map<ArticleDto>(article);
                    dto.SourceName = source.Name;
                    dto.ArticleSourceUrl = source.OriginUrl;
                    return dto;
                }
                throw new Exception($"This source [{article.NewsResourceId}] is not exist");
            }
            throw new Exception($"This article [{id}] is not exist");
        }

        public async Task<List<AutoCompleteDataDto>> GetArticlesNamesByPartNameAsync(string partName)
        {
            var articles = _unitOfWork.Articles
                .GetAsQueryable()
                .AsNoTracking()
                .Where(article => article.Title.Contains(partName))
                .Select(article => _mapper.Map<AutoCompleteDataDto>(article))
                .ToList();

            return articles;
        }

        public async Task<int> GetIdOfArticleASync(ArticleDto article)
        {
            var ent = _unitOfWork.Articles.FindBy(a => ((a.Title == article.Title) && (a.DatePosting == article.DatePosting))).FirstOrDefault();
            if (ent != null)
            {
                return ent.Id;
            }
            else
            {
                throw new Exception("This article is not exist");
            }
        }

        public async Task AddAsync(ArticleDto dto)
        { 
            await _unitOfWork.Articles.AddAsync(_mapper.Map<Article>(dto));
            await _unitOfWork.SaveChangesAsync();
        }

        //aggregator
        public async Task AggregateArticlesDataFromRssAsync(CancellationToken cancellationToken)
        {
            await _categoryService.InitiateDefaultCategorysAsync();
            await _sourceService.InitDefaultSourceAsync();
            var sources = await _sourceService.GetSourcesAsync();
            if (sources == null)
            {
                throw new Exception("Cant find and init any sources");
            }
            else
            {
                sources.Remove(sources.FirstOrDefault(s => s.Name == "Admin"));
                sources.Remove(sources.FirstOrDefault(s => s.Name == "Community"));
            }

            var articles = new ConcurrentBag<ArticleDto>();
            var urls = await GetContainsArticleUrlsBySourceAsync();
            var categorys = await _categoryService.GetCategoriesAsync();
            if (categorys == null)
            {
                throw new Exception("Cant find and init any categorys");
            }
            Parallel.ForEach(sources, async source =>
            {
                using (var reader = XmlReader.Create(source.RssFeedUrl))
                {
                    var feed = SyndicationFeed.Load(reader);

                    await Parallel.ForEachAsync(feed.Items
                            .Where(item => !urls.Contains(item.Links[0].Uri.AbsoluteUri)).ToArray(), cancellationToken,
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
                                CategoryId = CategoryCheck(item.Categories[0].Name, categorys)
                            });
                            return ValueTask.CompletedTask;
                        });
                    reader.Close();
                }
            });
            await _mediator.Send(new AddArticlesCommand() { Articles = articles }, cancellationToken);
        }

        public async Task AddFullContentForArticlesAsync(CancellationToken cancellationToken)
        {
            var articlesWithoutContent = await _mediator.Send(new GetAllArticlesWithoutContentQuery());

            var concBag = new ConcurrentBag<ArticleDto>();
            var onDelete = new List<int>();
            await Parallel.ForEachAsync(articlesWithoutContent, async (dto, token) =>
            {
                var content = await GetArticleContentAsync(dto.ArticleSourceUrl);
                if (content != "")
                {
                    dto.Content = content;
                    concBag.Add(dto);
                }
                else
                {
                    onDelete.Add(dto.Id);
                }
            });

            if (onDelete.Count > 0)
            {
                await _mediator.Send(new RemoveRangeArticlesCommand() { ArticlesId = onDelete }, cancellationToken);
            }

            await _mediator.Send(new AddArticlesFullContentCommand() { Articles = concBag }, cancellationToken);
        }


        public async Task AddRaitingForArticlesAsync(CancellationToken cancellationToken)
        {
            var unratedArticles = await _mediator.Send(new GetUnratedArticlesQuery(), cancellationToken);

            if (unratedArticles == null)
            {
                throw new Exception("Cant find any unrated article after their creation");
            }

            foreach (var unratedArticle in unratedArticles)
            {
                unratedArticle.PositiveRaiting = await GetArticleRateAsync(unratedArticle);
            }
            await _mediator.Send(new RateArticlesCommand() { Articles = unratedArticles }, cancellationToken);
        }


        private int CategoryCheck(string category, List<CategoryDto> categorys)
        {

            string categoryName = "";
            if (category.IndexOf(':') > 0)
            {
                category = category.Substring(0, category.IndexOf(':'));
            }

            switch (category.Trim().ToUpper())
            {
                case "ЗРОБЛЕНА БЕЛАРУСАМI":
                case "КУЛЬТУРА":
                    categoryName = "Культура"; 
                    break;
                case "ЛАЙФСТАЙЛ":
                case "ОБЩЕСТВО":
                    categoryName = "Общество"; 
                    break;
                case "ПОЛИТИКА":
                    categoryName = "Политика"; 
                    break;
                case "СПОРТ":
                    categoryName = "Спорт"; 
                    break;
                case "АВТО":
                case "ТЕХНОЛОГИИ":
                case "НАУКА":
                    categoryName = "Наука и технологии";
                    break;
                case "КОШЕЛЕК":
                case "НЕДВИЖИМОСТЬ":
                case "ЭКОНОМИКА":
                    categoryName = "Экономика"; 
                    break;
                default:
                    categoryName = "Разное"; 
                    break;
            }
            var cat = categorys.FirstOrDefault(c => c.Name == categoryName);
            return cat.Id ;
        }

        

        private async Task<double?> GetArticleRateAsync(ArticleDto unratedArticle)
        {
            var articleText = unratedArticle.Content;


            if (string.IsNullOrEmpty(articleText))
            {
                throw new ArgumentException("Article or article text doesn't exist",
                    nameof(unratedArticle.Id));
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
                    else
                    {
                        throw new Exception("Bad response status");
                    }
                }
                return null;
            }
        }

        private string? PrepareText(string articleText)
        {
            articleText = articleText.Trim();

            articleText = HttpUtility.HtmlDecode(articleText);
            articleText = Regex.Replace(articleText, "<.*?>|\n|\r|\"|\\d|\\W", " ");

            return articleText.Trim();
        }

        private async Task<string[]> GetContainsArticleUrlsBySourceAsync()
        {
            var articleUrls = _unitOfWork.Articles.GetAsQueryable()
                        .Select(article => article.ArticleSourceUrl)
                        .ToArray();//////////////////////////////
            return articleUrls;
        }

        private async Task<string> GetArticleContentAsync(string url)
        {
            try
            { 
                
                var content = "";

                if (url.Contains("onliner"))
                {
                    var web = new HtmlWeb();
                    var doc = web.Load(url);
                    content = DeleteNodes(doc, "//div[@class = 'news-text']", "//div[@id = 'news-text-end']", new List<string>
                    {
                        "//div[contains(@class, 'news-reference') or contains(@class, 'news-widget') or contains(@class, 'news-promo') or contains(@class, 'news-incut') or starts-with(@id, 'adfox') or contains(@class, 'news-header')]",
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
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }

        }

        private string DeleteNodes(HtmlDocument? doc, string startArticle, string endArticle, List<string> nodes)
        {
            var textNode = doc.DocumentNode.SelectSingleNode(startArticle);
            if (textNode == null) return "";
            if (endArticle != "")
            {
                var endNode = doc.DocumentNode.SelectNodes(endArticle)?.ToList().Last();
                if (endNode != null)
                {
                    var nodesToRemove = textNode.Descendants()
                        .SkipWhile(n => n != endNode)
                        .ToList();
                    nodesToRemove.ForEach(n => n.Remove());

                }
                else
                {
                    textNode.SelectNodes("//div[contains(@class, 'news-media_condensed')] | //p[contains(@style, 'text-align: right;')]")?
                   .ToList()
                   .ForEach(n => n.Remove());
                }
            }

            string selNodes = string.Join(" | ", nodes);

            textNode.SelectNodes(selNodes)?
                   .ToList()
                   .ForEach(n => n.Remove());
            return textNode.InnerHtml;
        }
    }
}