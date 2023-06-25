using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.DataCQS.Commands;
using myProject.DataCQS.Queries;
using myProject.Data.Entities;
using AutoMapper;
using MediatR;
using Moq;
using Microsoft.EntityFrameworkCore;
using myProject.Repositories;
using System.ServiceModel.Syndication;
using System.Net;
using System.Threading;
using System.Xml;
using HtmlAgilityPack;
using System.Reflection;
using Moq.Protected;
using myProject.Business.RateModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace myProject.Business.Tests
{
    public class ArticleServiceTest
    {
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>();
        //private readonly Mock<IConfiguration> _configMock = new Mock<IConfiguration>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ISourceService> _sourceServiceMock = new Mock<ISourceService>();
        private readonly Mock<ICategoryService> _categoryServiceMock = new Mock<ICategoryService>();
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();


        private readonly Mock<IArticleService> _articleServiceMock = new Mock<IArticleService>();

        private ArticleService CreateService()
        {

            var articleService = new ArticleService(
                _uowMock.Object,
                _mediatorMock.Object,
                _mapperMock.Object,
                _sourceServiceMock.Object,
                _categoryServiceMock.Object
                );

            return articleService;
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task UpRaitingAsync_WithValidId_SaveArticle(int id)
        {
            // Arrange
            var articleService = CreateService();
            var article = new Article() {Id = id};
            _uowMock.Setup(uow => uow.Articles.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(article);
            _uowMock.Setup(uow => uow.Articles.PatchAsync(It.IsAny<int>(), It.IsAny<List<PatchDto>>()));
            // Act
            await articleService.UpRaitingAsync(id);

            // Assert
            _uowMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(0)]
        public async Task UpRaitingAsync_WithInvalidId_ThrowExeption(int id)
        {
            // Arrange
            var articleService = CreateService();
            var article = new Article() { Id = id };
            _uowMock.Setup(uow => uow.Articles.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);
            _uowMock.Setup(uow => uow.Articles.PatchAsync(It.IsAny<int>(), It.IsAny<List<PatchDto>>()));
            // Act
            var result = async () => await articleService.UpRaitingAsync(id);

            //assert 
            await Assert.ThrowsAnyAsync<Exception>(result);
        }


        [Fact]
        public async Task EditArticleAsync_WithValidId_SaveArticle()
        {
            // Arrange
            var articleDto = new ArticleDto() { Id = 1 , Content ="edit", Title = "edit", ShortDescription = "edit" };
            var articleService = CreateService();
            var article = new Article() { Id = articleDto.Id, Content = "old", Title = "old", ShortDescription = "old" };
            _uowMock.Setup(uow => uow.Articles.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(article);
            _uowMock.Setup(uow => uow.Articles.PatchAsync(It.IsAny<int>(), It.IsAny<List<PatchDto>>()));
            // Act
            await articleService.EditArticleAsync(articleDto);

            // Assert
            _uowMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("string1")]
        [InlineData("string2")]
        [InlineData("string3")]
        public async Task GetArticlesNamesByPartNameAsync_WithValidData_ReturnArticles(string str)
        {
            // Arrange
            var articles = new List<Article> () { 
                new Article {Id = 1, Content = "string3string1", Title = "string3string1", ShortDescription = "string3string1" },
                new Article {Id = 2, Content = "string2", Title = "string2", ShortDescription = "string2" },
                new Article {Id = 3, Content = "string1string3string2", Title = "string1string3string2", ShortDescription = "string1string3string2" },
            };

            var articleService = CreateService();
            _mapperMock.Setup(mapper => mapper.Map<AutoCompleteDataDto>(It.IsAny<Article>()))
                .Returns(() => new AutoCompleteDataDto());
            _uowMock.Setup(uow => uow.Articles.GetAsQueryable()).Returns(articles.AsQueryable());
            // Act
            var data = await articleService.GetArticlesNamesByPartNameAsync(str);

            // Assert
            Assert.Equal(2, data.Count);
        }

        [Fact]
        public async Task GetIdOfArticleASync_WithValiIdArticle_ReturnArticleId()
        {
            // Arrange
            var articleDto = new ArticleDto {Title = "string1", DatePosting = DateTime.MinValue };
            var article = new List<Article>{ new Article { Id = 3, Title = "string1", DatePosting = DateTime.MinValue } };

            var articleService = CreateService();
            _uowMock.Setup(uow => uow.Articles.FindBy(a => (a.Title == articleDto.Title) && (a.DatePosting == articleDto.DatePosting))).Returns(article.AsQueryable());
            // Act
            var data = await articleService.GetIdOfArticleASync(articleDto);

            // Assert
            Assert.Equal(3, data);
        }

        [Fact]
        public async Task GetIdOfArticleASync_WithInvaliIdArticle_ThrowExeption()
        {
            // Arrange
            var articleDto = new ArticleDto { Title = "string1", DatePosting = DateTime.MinValue };

            var articleService = CreateService();
            _uowMock.Setup(uow => uow.Articles.FindBy(a => (a.Title == articleDto.Title) && (a.DatePosting == articleDto.DatePosting))).Returns(() => null);
            // Act
            var result = async () => await articleService.GetIdOfArticleASync(articleDto);

            // Assert
            await Assert.ThrowsAnyAsync<Exception>(result);
        }


        [Fact]
        public async Task EditArticleAsync_WithInvalidId_ThrowExeption()
        {
            // Arrange
            var articleDto = new ArticleDto() { Id = -1 };
            var articleService = CreateService();
            var article = new Article() { Id = articleDto.Id };
            _uowMock.Setup(uow => uow.Articles.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);
            _uowMock.Setup(uow => uow.Articles.PatchAsync(It.IsAny<int>(), It.IsAny<List<PatchDto>>()));
            // Act
            var result = async () => await articleService.EditArticleAsync(articleDto);

            //assert 
            await Assert.ThrowsAnyAsync<Exception>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task DeleteArticleByIdAsync_WithValidId_DeletesArticle(int id)
        {
            // Arrange
            var articleService = CreateService();
            _uowMock.Setup(uow => uow.Articles.Remove(It.IsAny<int>()));

            // Act
            await articleService.DeleteArticleByIdAsync(id);

            // Assert
            _uowMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(0)]
        public async Task DeleteArticleByIdAsync_WithInvalidId_ThrowExeption(int id)
        {
            // Arrange
            var articleService = CreateService();

            // Act
            var result = async () => await articleService.DeleteArticleByIdAsync(id);

            //assert 
            await Assert.ThrowsAnyAsync<ArgumentException>(result);
        }

        [Theory]
        [InlineData(0, 2, 0.15)]
        [InlineData(1, 15, -0.15)]
        [InlineData(10, 15000, 0)]
        public async Task GetArticlesByPageAsync_WithCorrectData_ReturnCorrectPage(int page, int pageSize, double positivity)
        {
            //arrange
            _mapperMock.Setup(mapper => mapper.Map<ArticleDto>(It.IsAny<Article>()))
                 .Returns(() => new ArticleDto());

            _uowMock.Setup(uow => uow.Articles.GetArticlesForPageAsync(It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<double>())).ReturnsAsync(() => new List<Article>
            {
                new Article(),
                new Article()
            });

            _uowMock.Setup(uow => uow.Categories.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => new Category());

            var articleService = CreateService();

            //act
            var data = await articleService.GetArticlesByPageAsync(page, pageSize, positivity);

            //assert 
            Assert.Equal(2, data.Count);

        }


        [Theory]
        [InlineData(0, -2, null)]
        [InlineData(-2, 15, -1)]
        [InlineData(-10, -10, 1)]
        public async Task GetArticlesByPageAsync_WithIncorrectPageAndPageSize_ReturnError(int page, int pageSize, double positivity)
        {
            //arrange
            var articleService = CreateService();

            //act
            var result = async () => await articleService.GetArticlesByPageAsync(page, pageSize, positivity);

            //assert 
            await Assert.ThrowsAnyAsync<ArgumentException>(result);
        }


        [Fact]
        public async Task GetFavArticleAsync_WithCorrectData_ReturnCorrectArticles()
        {
            //arrange
            _mapperMock.Setup(mapper => mapper.Map<ArticleDto>(It.IsAny<Article>()))
                 .Returns(() => new ArticleDto());

            var articles = new List<Article>
            {
                new Article(),
                new Article(),
                new Article(),
                new Article()
            };

            _uowMock.Setup(uow => uow.Articles.GetAsQueryable()).Returns(() => articles.AsQueryable());

            _uowMock.Setup(uow => uow.NewsResources.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => new NewsResource());

            var articleService = CreateService();

            //act
            var data = await articleService.GetFavArticleAsync();

            //assert 
            Task.FromResult(articles.ToList());
            Assert.Equal(3, data.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetArticleByIdWithSourceNameAsync_WithCorrectData_ReturnCorrectArticle(int id)
        {
            //arrange
            _mapperMock.Setup(mapper => mapper.Map<ArticleDto>(It.IsAny<Article>()))
                 .Returns(() => new ArticleDto());

            _uowMock.Setup(uow => uow.Articles.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => new Article());

            _uowMock.Setup(uow => uow.NewsResources.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => new NewsResource());

            var articleService = CreateService();

            //act
            var data = await articleService.GetArticleByIdWithSourceNameAsync(id);

            //assert 
            Assert.NotEqual(null, data);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(0)]
        public async Task GetArticleByIdWithSourceNameAsync_WithIncorrectData_ReturnArticleExeption(int id)
        {
            //arrange
            _mapperMock.Setup(mapper => mapper.Map<ArticleDto>(It.IsAny<Article>()))
                 .Returns(() => new ArticleDto());

            _uowMock.Setup(uow => uow.Articles.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            _uowMock.Setup(uow => uow.NewsResources.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => new NewsResource());

            var articleService = CreateService();

            //act
            var result = async () => await articleService.GetArticleByIdWithSourceNameAsync(id);

            //assert 
            await Assert.ThrowsAnyAsync<Exception>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetArticleByIdWithSourceNameAsync_WithCorrecttData_ReturnSourceExeption(int id)
        {
            //arrange
            _mapperMock.Setup(mapper => mapper.Map<ArticleDto>(It.IsAny<Article>()))
                 .Returns(() => new ArticleDto());

            _uowMock.Setup(uow => uow.Articles.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => new Article());

            _uowMock.Setup(uow => uow.NewsResources.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            var articleService = CreateService();

            //act
            var result = async () => await articleService.GetArticleByIdWithSourceNameAsync(id);

            //assert 
            await Assert.ThrowsAnyAsync<Exception>(result);
        }


        [Fact]
        public async Task AggregateArticlesDataFromRssAsync_WithCorrecttData_GoAddArticlesCommand()
        {

            var art = new List<Article>
            {
                new Article(),
                new Article(),
                new Article(),
                new Article()
            };
            var categories = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Category1" }
        };
            var sources = new List<NewsResourceDto>
        {
            new NewsResourceDto { Id = 1, Name = "Source1", RssFeedUrl = "http://example.com/rss" },
            new NewsResourceDto { Id = 2, Name = "Admin", RssFeedUrl = "http://example.com/rss-admin" },
            new NewsResourceDto { Id = 3, Name = "Community", RssFeedUrl = "http://example.com/rss-community" }
        };
            
            _categoryServiceMock.Setup(s => s.InitiateDefaultCategorysAsync()).Returns(Task.CompletedTask);
            _sourceServiceMock.Setup(s => s.InitDefaultSourceAsync()).Returns(Task.CompletedTask);
            _sourceServiceMock.Setup(s => s.GetSourcesAsync()).ReturnsAsync(sources);
            _categoryServiceMock.Setup(s => s.GetCategoriesAsync()).ReturnsAsync(categories);
            _uowMock.Setup(s => s.Articles.GetAsQueryable()).Returns(() => art.AsQueryable());
            var articleService = CreateService();
            // Act
            await articleService.AggregateArticlesDataFromRssAsync(new CancellationToken());

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<AddArticlesCommand>(), new CancellationToken()), Times.Once);
        }

        [Fact]
        public async Task AggregateArticlesDataFromRssAsync_WithNullCategories_ThrowExeption()
        {

            var art = new List<Article>
            {
                new Article(),
                new Article(),
                new Article(),
                new Article()
            };
            var sources = new List<NewsResourceDto>
        {
            new NewsResourceDto { Id = 1, Name = "Source1", RssFeedUrl = "http://example.com/rss" },
            new NewsResourceDto { Id = 2, Name = "Admin", RssFeedUrl = "http://example.com/rss-admin" },
            new NewsResourceDto { Id = 3, Name = "Community", RssFeedUrl = "http://example.com/rss-community" }
        };

            _categoryServiceMock.Setup(s => s.InitiateDefaultCategorysAsync()).Returns(Task.CompletedTask);
            _sourceServiceMock.Setup(s => s.InitDefaultSourceAsync()).Returns(Task.CompletedTask);
            _sourceServiceMock.Setup(s => s.GetSourcesAsync()).ReturnsAsync(sources);
            _categoryServiceMock.Setup(s => s.GetCategoriesAsync()).ReturnsAsync(() => null);
            _uowMock.Setup(s => s.Articles.GetAsQueryable()).Returns(() => art.AsQueryable());
            var articleService = CreateService();
            // Act
            var result = async () => await articleService.AggregateArticlesDataFromRssAsync(new CancellationToken());

            // Assert
            await Assert.ThrowsAnyAsync<Exception>(result);
        }


        [Fact]
        public async Task AggregateArticlesDataFromRssAsync_WithNullSources_ThrowExeption()
        {

            var art = new List<Article>
            {
                new Article(),
                new Article(),
                new Article(),
                new Article()
            };
            var categories = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Category1" }
        };
        
            _categoryServiceMock.Setup(s => s.InitiateDefaultCategorysAsync()).Returns(Task.CompletedTask);
            _sourceServiceMock.Setup(s => s.InitDefaultSourceAsync()).Returns(Task.CompletedTask);
            _sourceServiceMock.Setup(s => s.GetSourcesAsync()).ReturnsAsync(() => null);
            _categoryServiceMock.Setup(s => s.GetCategoriesAsync()).ReturnsAsync(categories);
            _uowMock.Setup(s => s.Articles.GetAsQueryable()).Returns(() => art.AsQueryable());
            var articleService = CreateService();
            // Act
            var result = async () => await articleService.AggregateArticlesDataFromRssAsync(new CancellationToken());

            // Assert
            await Assert.ThrowsAnyAsync<Exception>(result);
        }

        [Fact]
        public async Task AddFullContentForArticlesAsync_WithCorrecttData_GoAddFullTextCommand()
        {
            var cancellationToken = new CancellationToken();

            var articlesWithoutContent = new List<ArticleDto>()
            {
                new ArticleDto() { Id = 1, ArticleSourceUrl = "https://news.mail.ru/politics/56721951/" },
                new ArticleDto() { Id = 2, ArticleSourceUrl = "https://auto.onliner.by/2023/06/21/poputnye-voditeli-byli-v-shoke-legkovushka-reshila-vnezapno-razvernutsya-pryamo-posredi-trassy" }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllArticlesWithoutContentQuery>(), cancellationToken))
                        .ReturnsAsync(articlesWithoutContent);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddArticlesFullContentCommand>(), cancellationToken))
                        .Returns(Task.CompletedTask);
            var articleService = CreateService();
            // Act
            await articleService.AddFullContentForArticlesAsync(cancellationToken);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllArticlesWithoutContentQuery>(), cancellationToken), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<AddArticlesFullContentCommand>(), cancellationToken), Times.Once);
        }


        [Theory]
        [InlineData("https://news.mail.ru/politics/56721951/")]
        [InlineData("https://auto.onliner.by/2023/06/21/shkolnica-vnezapno-vybezhala-na-dorogu-na-krasnyj-svet-spasla-reakciya-voditelya")]
        public async Task GetArticleContentAsync_WithCorrecttData_HtmlDoesNotContainBadElements(string url)
        {
            // Arrange
            var articleService = CreateService();
            var expectedHtml = new List<string> { };

            if (url == "https://news.mail.ru/politics/56721951/")
            {
                expectedHtml = new List<string> { "article__title", "article__params", "news-widget", "rb-slot", "article__item_slot" };
            }
            else
            {
                expectedHtml = new List<string> { "news-reference", "news-widget", "news-promo", "news-incut", "adfox", "news-header", "script", "news-banner", "news-text-end" };
            }

            // Act
            var method = typeof(ArticleService).GetMethod("GetArticleContentAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = await (Task<string>)method.Invoke(articleService, new object[] { url });

            // Assert
            foreach (var html in expectedHtml)
            {
                Assert.DoesNotContain(html, result);
            }
        }

        [Fact]
        public async Task AddRaitingForArticlesAsync_WithCorrecttData_ShouldAddRaitingForUnratedArticles()
        {
            // Arrange
            var articleService = CreateService();
            var cancellationToken = new CancellationToken();
            var unratedArticle = new ArticleDto { Id = 1, Content = "<div class=\"article__intro\"><p>ВАШИНГТОН, 22&nbsp;июня. /ТАСС/. Госдепартамент " +
                "признал, что&nbsp;стратегия Запада при&nbsp;оказании Киеву военной помощи заключается в&nbsp;наращивании мощностей поставляемых ВСУ" +
                " вооружений. С&nbsp;таким заявлением выступил в&nbsp;среду заместитель руководителя пресс-службы Госдепартамента Ведант Пател, " +
                "отвечая на&nbsp;вопрос о&nbsp;риске эскалации конфликта из-за&nbsp;поставок Украине западной техники и&nbsp;одобрении Вашингтоном " +
                "передачи Киеву истребителей F-16.</p></div><div class=\"article__content\"><div class=\"article__item article__item_picture\"><div" +
                " class=\"article__item_image article__item_image_\"><amp-img class=\"photo\" width=\"780\" height=\"440\" layout=\"responsive\" sr" +
                "c=\"https://resizer.mail.ru/p/060dd1ad-0715-59e2-b7dc-974b3c49b8c2/AQAGmKCanGG-zp_ZinnLwPkNzq8vyCAbtsKF5wwUIidsfx4hMUMxL12kc2MxrVa" +
                "vss1NEl8hc8lWBAj1qFXcwN9l73U.jpg\"></amp-img></div><div class=\"article-item-info\" overflow=\"\"><div class=\"article-item-info__" +
                "text article-item-info__text_source\">Источник: <a class=\"article-item-info__source\" href=\"https://help.mail.ru/legal/terms/new" +
                "s/policy\" target=\"_blank\">Reuters</a></div></div></div><div class=\"article__item article__item_text\"><p>Его спросили, почему " +
                "Вашингтон продолжает снабжать ВСУ &laquo;все более и&nbsp;более мощными вооружениями&raquo;, если по&nbsp;словам президента США Дж" +
                "о Байдена, существует угроза эскалации конфликта до&nbsp;ядерной войны.</p></div><div class=\"article__item article__item_text\"><" +
                "p>&laquo;Мы&nbsp;в&nbsp;тесной координации с&nbsp;союзниками и&nbsp;партнерами давали оценку видам технологий и&nbsp;систем, котор" +
                "ые, как нам кажется, необходимы нашим украинским партнерам. В&nbsp;этом заключается наша стратегия касательно предоставления допол" +
                "нительной помощи партнерам на&nbsp;Украине&raquo;,&nbsp;&mdash; сказал он&nbsp;в&nbsp;ходе регулярного брифинга для&nbsp;журналист" +
                "ов.</p></div><div class=\"article__item article__item_keyphrase\"><div class=\"article-keyphrase article-keyphrase_side\"><span cl" +
                "ass=\"article-keyphrase__inner\">Пател добавил, что&nbsp;пока у&nbsp;него нет &laquo;новостей о&nbsp;конкретных изменениях в&nbsp;" +
                "военной помощи&raquo;.</span></div></div><div class=\"article__item article__item_text\"><p>&laquo;Мы&nbsp;видели сообщения о&nbsp;" +
                "соглашении между Россией и&nbsp;Белоруссией, и&nbsp;мы&nbsp;продолжим пристально следить за&nbsp;ситуацией, за&nbsp;ходом ее&nbsp;р" +
                "азвития и&nbsp;за&nbsp;последствиями. Мы&nbsp;не&nbsp;увидели причины для&nbsp;корректировки нашей ядерной стратегии&raquo;,&nbsp;" +
                "&mdash; отметил сотрудник Госдепартамента.</p></div><div class=\"article__item article__item_keyphrase\"><div class=\"article-keyp" +
                "hrase article-keyphrase_side\"><span class=\"article-keyphrase__inner\">При&nbsp;этом Пател напомнил о&nbsp;позиции президента США " +
                "Джо Байдена, который счел &laquo;безрассудной и&nbsp;безответственной подобную риторику о&nbsp;ядерных вооружениях&raquo; со&nbsp;" +
                "стороны Москвы.</span></div></div><div class=\"article__item article__item_text\"><p>Президент&nbsp;РФ Владимир Путин 25&nbsp;март" +
                "а заявил, что&nbsp;Россия по&nbsp;просьбе Белоруссии разместит свое тактическое ядерное оружие в&nbsp;республике, как это давно де" +
                "лают США на&nbsp;территории своих союзников. Москва&nbsp;уже передала Минску ракетный комплекс &laquo;Искандер&raquo;, который мож" +
                "ет быть носителем ядерного оружия, и&nbsp;оказала помощь в&nbsp;переоборудовании белорусских самолетов, чтобы они обладали возможн" +
                "остью применять специальные боеприпасы.</p></div><div class=\"article__item article__item_text\"><p>Белорусские ракетчики и&nbsp;ле" +
                "тчики прошли на&nbsp;территории России соответствующую подготовку. 9&nbsp;июня на&nbsp;встрече с&nbsp;белорусским президентом Алекс" +
                "андром Лукашенко Путин сообщил, что&nbsp;размещение российского ядерного оружия в&nbsp;республике начнется сразу&nbsp;же&nbsp;после" +
                " того, как <nobr>7&mdash;8&nbsp;июля</nobr> завершится подготовка сооружений для&nbsp;него.</p></div><div class=\"article__item ar" +
                "ticle__item_embed\"><amp-iframe layout=\"responsive\" width=\"16\" height=\"9\" sandbox=\"allow-scripts allow-same-origin\" allowf" +
                "ullscreen=\"allowfullscreen\" frameborder=\"0\" src=\"https://vk.com/video_ext.php?oid=-106879986&amp;id=456258689&amp;hash=08bf5e" +
                "ab15852184&amp;hd=2&amp;autoplay=1&amp;partner_ext=29\"><div placeholder=\"placeholder\"></div></amp-iframe></div></div>" };
            var unratedArticles = new List<ArticleDto> { unratedArticle };
            var expectedRaiting = 0.041522491349480967;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUnratedArticlesQuery>(), cancellationToken))
                .ReturnsAsync(unratedArticles);
            // Act
            await articleService.AddRaitingForArticlesAsync(cancellationToken);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<RateArticlesCommand>(), cancellationToken), Times.Once);
            Assert.Equal(expectedRaiting, unratedArticle.PositiveRaiting);
        }

        [Fact]
        public async Task AddRaitingForArticlesAsyncn_UnratedArticlesNotFound_ShouldThrowException()
        {
            // Arrange
            var articleService = CreateService();
            var cancellationToken = new CancellationToken();
            List<ArticleDto> unratedArticles = null;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUnratedArticlesQuery>(), cancellationToken))
                .ReturnsAsync(unratedArticles);

            // Act
            var result = async () => await articleService.AddRaitingForArticlesAsync(cancellationToken);
            // Assert
            await Assert.ThrowsAnyAsync<Exception>(result);
        }

        [Fact]
        public async Task AddRaitingForArticlesAsync_ArticleTextNotExists_ShouldThrowException()
        {
            // Arrange
            var articleService = CreateService();
            var cancellationToken = new CancellationToken();
            var unratedArticle = new ArticleDto { Id = 1, Content = "" };
            var unratedArticles = new List<ArticleDto> { unratedArticle };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUnratedArticlesQuery>(), cancellationToken))
                .ReturnsAsync(unratedArticles);

            _uowMock.Setup(u => u.Articles.GetByIdAsync(unratedArticle.Id))
                .ReturnsAsync((Article)null);

            // Act
            var result = async () => await articleService.AddRaitingForArticlesAsync(cancellationToken);
            // Assert
            await Assert.ThrowsAnyAsync<Exception>(result);
        }

    }
}