using Microsoft.EntityFrameworkCore;
using myProject.Data;
using myProject.Abstractions.Services;
using myProject.Business;
using Microsoft.EntityFrameworkCore.Metadata;
using myProject.Abstractions;
using myProject.Repositories;
using myProject.Repositories.Implementations;
using myProject.Abstractions.Data.Repositories;
using myProject.Data.Entities;

namespace myProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            builder.Services.AddTransient<IArticleService, ArticleService>();
            // Add services to the container.
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}