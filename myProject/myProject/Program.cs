using Microsoft.EntityFrameworkCore;
using myProject.Data;
using myProject.Abstractions.Services;
using myProject.Business;
using myProject.Abstractions;
using myProject.Repositories;
using myProject.Repositories.Implementations;
using myProject.Abstractions.Data.Repositories;
using myProject.Data.Entities;
using myProject.DataCQS.QueriesHandlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using Hangfire;
using Serilog.Events;
using myProject.Mvc.SignalR;
using HangfireBasicAuthenticationFilter;

namespace myProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.File("F:\\434\\log.txt", LogEventLevel.Information)
                .CreateLogger();

            builder.Host.UseSerilog();

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
            builder.Services.AddScoped<IRepository<Comment>, Repository<Comment>>();
            builder.Services.AddScoped<IRepository<NewsResource>, Repository<NewsResource>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IRepository<Subscription>, Repository<Subscription>>();
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

            builder.Services.AddMediatR(
                cfg =>
                    cfg.RegisterServicesFromAssemblyContaining<GetUserByRefreshTokenQueryHandler>());


            builder.Services.AddAutoMapper(typeof(Program));


            builder.Services.AddHangfire(config =>
               config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                   .UseSimpleAssemblyNameTypeSerializer()
                   .UseRecommendedSerializerSettings()
                   .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHangfireServer();



            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
            {
                option.LoginPath = new PathString("/Account/Login");
                option.AccessDeniedPath = new PathString("/Account/Login");
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseHangfireDashboard("/Hangfire", new DashboardOptions
            {
                Authorization = new[] {new HangfireCustomBasicAuthenticationFilter
                {
                    User = builder.Configuration.GetSection("HangfireSettings:UserName").Value,
                    Pass = builder.Configuration.GetSection("HangfireSettings:Password").Value
                }
            }
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapHub<CommentsHub>("/commentsHub");

            app.Run();

            
        }
    }
}