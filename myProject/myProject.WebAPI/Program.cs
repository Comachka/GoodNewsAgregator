using Microsoft.EntityFrameworkCore;
using myProject.Abstractions.Data.Repositories;
using myProject.Abstractions.Services;
using myProject.Abstractions;
using myProject.Data.Entities;
using myProject.Data;
using myProject.Repositories;
using myProject.Repositories.Implementations;
using myProject.Business;
using System.Reflection;

namespace myProject.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //Add DbContext
            builder.Services.AddDbContext<MyProjectContext>(options =>
            {
                var connectionString = builder.Configuration
                .GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });


            //DI
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
            builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IRepository<NewsResource>, Repository<NewsResource>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            builder.Services.AddScoped<IRepository<UserCategory>, Repository<UserCategory>>();
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();

            builder.Services.AddTransient<ICommentService, CommentService>();
            builder.Services.AddTransient<IArticleService, ArticleService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IRoleService, RoleService>();
            builder.Services.AddTransient<ICategoryService, CategoryService>();
            builder.Services.AddTransient<ISourceService, SourceService>();
            builder.Services.AddTransient<ISubscriptionService, SubscriptionService>();
            // Add services to the container.
            builder.Services.AddAutoMapper(typeof(Program));


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                   $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}