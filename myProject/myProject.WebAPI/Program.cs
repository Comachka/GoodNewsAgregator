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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using myProject.DataCQS.QueriesHandlers;
using Serilog;
using Serilog.Events;
using Hangfire;

namespace myProject.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.File("F:\\434\\logWebApi.txt", LogEventLevel.Information)
                .CreateLogger();

            builder.Host.UseSerilog();
            // Add services to the container.
            //Add DbContext
            builder.Services.AddDbContext<MyProjectContext>(options =>
            {
                var connectionString = builder.Configuration
                .GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidAudience = builder.Configuration["Jwt:Audience"],
                       ValidIssuer = builder.Configuration["Jwt:Issuer"],
                       IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecurityKey"])),
                   };
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
            builder.Services.AddTransient<IJwtService, JwtService>();
            builder.Services.AddTransient<ITokenService, TokenService>();

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


            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseHangfireDashboard();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}