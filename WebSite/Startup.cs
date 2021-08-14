using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System;
using Advokates_CRM.DB.Models;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Threading.Tasks;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM.BL;
using Advokates_CRM.BL.Helpers;
using Avokates_CRM.RequestHandlers;
using Advokates_CRM.DB.Migrations;
using System.Data.SqlClient;
using FluentMigrator.Runner;
using System.IO;

namespace WebSite
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            BisnesLogicInitialisation.Init(configuration);            
        }

            // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // получаем строку подключения из файла конфигурации
            string connection = Configuration.GetConnectionString("DefaultConnection");
            // добавляем контекст AdvokatesContext в качестве сервиса в приложение
            services.AddDbContext<LawyerCRMContext>(options =>
                options.UseSqlServer(connection));

            RegisterDependencies(services);

                // Работаем с миграциями
            string scriptsFolder = Configuration.GetSection("ScriptsFolder").Value;
            BaseMigration.scriptsFolder = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "\\Advokates_CRM.DB\\", scriptsFolder);        // Устанавливаем начальный скрипт для БД

            services.AddFluentMigratorCore()
                   .ConfigureRunner
                   (
                       config =>
                       {
                           EnsureDBCreated();       // проверяем существование БД. Если БД нет - создаём её
                           config.AddSqlServer()
                           .WithGlobalConnectionString(connection)
                                                    // Определяем сборку, в которой находятся нужные миграции
                           //.ScanIn(Assembly.GetExecutingAssembly()).For.All()
                           .ScanIn(typeof(_1_InitDB).Assembly).For.Migrations();
                       }
                   )
                   .AddLogging(config => config.AddFluentMigratorConsole());




            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;                                              // ОБЯЗАТЕЛЬНО ПОМЕНЯТЬ на false!!!!
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // добавление кэширования
            services.AddMemoryCache();
            // services.AddDistributedMemoryCache();

            services.AddSession(opts =>
            {
                opts.Cookie.IsEssential = true; // make the session cookie Essential
            });

            services.AddAuthentication
                (options =>
                {
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {

                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    //options.Events = new JwtBearerEvents()
                    //{
                    //    OnMessageReceived = context =>
                    //    {
                    //        var token = context.HttpContext.Session.GetString("token");
                    //        context.Request.Headers.Add("Authorization", "Bearer " + token);
                    //        //context.Token = context.HttpContext.Request.Headers["X-JWT-Assertion"];
                    //        return Task.CompletedTask;
                    //    }
                    //};

                    //options.Authority = "https://localhost:44391/Auth/Authorization";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,                // валидация ключа безопасности
                        ValidateIssuer = false,                         // укзывает, будет ли валидироваться издатель при валидации токена 
                        ValidateAudience = false,                       // будет ли валидироваться потребитель токена   
                        //ValidIssuer = AuthHelper.ISSUER,               // строка, представляющая издателя
                        //ValidAudience = AuthHelper.AUDIENCE,           // установка потребителя токена
                                                                        // TODO: изменить алгоритм шифрования на асимметричный. В админке использовать публичный ключ, в апи - приватный
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("secretKey").Get<string>())),                   // установка ключа безопасности
                        ValidateLifetime = false,   // будет ли валидироваться время существования
                        ClockSkew = System.TimeSpan.FromMinutes(3000000)
                    };
                });

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("EnableCors", builder =>
            //    {
            //        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials().Build();
            //    });
            //});

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();               // конфигурировать сессии, чтоб можно было получать из них токен
                //внедряем токен в хидер поступающих запросов
            app.Use(async (context, next) =>
            {
                var token = context.Session.GetString("token");
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + token);
                }
                await next.Invoke();
            });
            app.UseAuthentication();
            app.UseCors("EnableCors");
            app.UseStatusCodePages(async context =>
            {
                //var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;

                if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    response.Redirect("/Auth/Authorization/");
                }
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    // template: "{controller=Auth}/{action=Authorization}/{id?}");
                    template: "{controller=Home}/{action=Index}");
            });

                // Делаем миграцию
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var migrator = scope.ServiceProvider.GetService<IMigrationRunner>();

                //migrator.MigrateDown(0);
                migrator.MigrateUp();
            }
        }

        /// <summary>
        /// Регистрация зависимостей
        /// </summary>
        /// <param name="services"></param>
        private void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<ISecurity, Security>();
            services.AddScoped<IDataLayer, DataLayerDB>();
            services.AddScoped<IDataLayerCabinet, DataLayerCabinet>();
            services.AddScoped<IDataLayerCase, DataLayerCase>();
            services.AddScoped<IDataLayerNote, DataLayerNote>();
            services.AddScoped<IDataLayerFigurant, DataLayerFigurant>();

            services.AddScoped<IErrorHandler, DataLayerError>();
            services.AddScoped<IDataLayerEmployee, DataLayerEmployee>();

            services.AddScoped<IBasicRequestHandler, BasicRequestHandler>();
            services.AddScoped<IViewRequestHandler, ViewRequestHandler>();
        }

        /// <summary>
        /// Удостоверяемся, что база существует. Если нет - создаём её
        /// </summary>
        private void EnsureDBCreated()
        {
            string connectionString = Configuration.GetSection("DataBaseServer").Value;
            string dataBaseName = Configuration.GetSection("DataBaseName").Value;

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand ensureCreatedComand = new SqlCommand("select count(1) from sys.databases d where d.name = '" + dataBaseName + "'", sqlConnection);
                ensureCreatedComand.CommandType = System.Data.CommandType.Text;

                bool DBExists = false;
                using (SqlDataReader reader = ensureCreatedComand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            DBExists = reader.GetInt32(0)>0;
                        }
                    }
                }
                if (!DBExists)         // БД не существует
                {
                    SqlCommand c = new SqlCommand("CREATE DATABASE " + dataBaseName, sqlConnection);
                    c.CommandType = System.Data.CommandType.Text;
                    c.ExecuteNonQuery();
                }
                sqlConnection.Close();
            }
            
        }        
    }
}
