using AutoMapper;
using BeCoreApp.Application.Implementation;
using BeCoreApp.Application.Interfaces;
using BeCoreApp.Data.EF;
using BeCoreApp.Data.EF.Repositories;
using BeCoreApp.Data.IRepositories;
using BeCoreApp.Web.Configuration;
using BeCoreApp.Web.Configuration.Interfaces;
using Core.Application.Implementation;
using Core.Application.Interfaces;
using Core.Authorization;
using Core.Data.EF;
using Core.Data.EF.Repositories;
using Core.Data.Entities;
using Core.Data.IRepositories;
using Core.Extensions;
using Core.Helpers;
using Core.Infrastructure.Interfaces;
using Core.Infrastructure.Telegram;
using Core.Services;
using Core.Web.Hubs;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebMarkupMin.AspNetCore5;

namespace Core.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.Secure = CookieSecurePolicy.SameAsRequest;
                
            });
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                o => o.MigrationsAssembly("Core.Data.EF")));

            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

            services.AddMemoryCache();

            services.AddMinResponse();

            //services.AddDataProtection()
            //    .SetApplicationName("FBSDefi")
            //    .SetDefaultKeyLifetime(TimeSpan.FromDays(90))
            //    .PersistKeysToFileSystem(new DirectoryInfo("E:\\FbsDefiKey\\"));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(5);
                options.Lockout.MaxFailedAccessAttempts = 10;
                //options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(5);
                
                options.LoginPath = "/login";
                //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddRecaptcha(new RecaptchaOptions()
            {
                SiteKey = Configuration["Recaptcha:SiteKey"],
                SecretKey = Configuration["Recaptcha:SecretKey"]
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddImageResizer();
            services.AddAutoMapper(typeof(Profile));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
               {
                   options.AccessDeniedPath = new PathString("/Account/Access");
                   options.Cookie = new CookieBuilder
                   {
                       //Domain = "",
                       HttpOnly = true,
                       Name = ".aspNetCoreDemo.Security.Cookie",
                       Path = "/",
                       SameSite = SameSiteMode.Strict,
                       SecurePolicy = CookieSecurePolicy.SameAsRequest,
                       Expiration = TimeSpan.FromDays(5),
                       MaxAge = TimeSpan.FromDays(5)
                       
                      
                   };
                   options.Events = new CookieAuthenticationEvents
                   {
                       OnSignedIn = context =>
                       {
                           Console.WriteLine("{0} - {1}: {2}", DateTime.UtcNow,
                               "OnSignedIn", context.Principal.Identity.Name);
                           return Task.CompletedTask;
                       },
                       OnSigningOut = context =>
                       {
                           Console.WriteLine("{0} - {1}: {2}", DateTime.UtcNow,
                               "OnSigningOut", context.HttpContext.User.Identity.Name);
                           return Task.CompletedTask;
                       },
                       OnValidatePrincipal = context =>
                       {
                           Console.WriteLine("{0} - {1}: {2}", DateTime.UtcNow,
                               "OnValidatePrincipal", context.Principal.Identity.Name);
                           return Task.CompletedTask;
                       }
                   };

                   options.ExpireTimeSpan = TimeSpan.FromDays(10);
                   options.LoginPath = new PathString("/login");
                   options.ReturnUrlParameter = "RequestPath";
                   options.SlidingExpiration = true;
               });

            // Add application services.
            services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();

            //services.AddSingleton(Mapper.Configuration);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService));

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IViewRenderService, ViewRenderService>();

            services.AddTransient<DbInitializer>();

            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomClaimsPrincipalFactory>();
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });
            services
                .AddMvc(options =>
                {
                    options.CacheProfiles.Add("Default", new CacheProfile() { Duration = 60 });
                    options.CacheProfiles.Add("Never", new CacheProfile() { Location = ResponseCacheLocation.None, NoStore = true });
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            
            AddSignalRService(services);

            services.AddScoped<IGraphQLClient>(s => new GraphQLHttpClient(Configuration["CloudFlareApi:GraphQLURI"], new NewtonsoftJsonSerializer()));

            services.Configure<RequestLocalizationOptions>(
             opts =>
             {
                 var supportedCultures = new List<CultureInfo>
                 {
                        new CultureInfo("en-US"),
                        new CultureInfo("vi-VN")
                 };

                 opts.DefaultRequestCulture = new RequestCulture("en-US");
                 // Formatting numbers, dates, etc.
                 opts.SupportedCultures = supportedCultures;
                 // UI strings that we have localized.
                 opts.SupportedUICultures = supportedCultures;
             });

            services.AddWebMarkupMin(
            options =>
            {
                options.AllowMinificationInDevelopmentEnvironment = false;
                options.AllowCompressionInDevelopmentEnvironment = false;
            })

            .AddHtmlMinification(
                options =>
                {
                    options.MinificationSettings.RemoveRedundantAttributes = true;
                    options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
                    options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
                })
            .AddHttpCompression();



            services.AddTransient(typeof(IUnitOfWork), typeof(EFUnitOfWork));
            services.AddTransient(typeof(IRepository<,>), typeof(EFRepository<,>));
            services.AddSingleton(typeof(IModelBulkInsert<>), typeof(ModelBulkInsert<>));


            //Repository
            services.AddTransient<IFunctionRepository, FunctionRepository>();
            services.AddTransient<ITagRepository, TagRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IMenuGroupRepository, MenuGroupRepository>();
            services.AddTransient<IMenuItemRepository, MenuItemRepository>();
            services.AddTransient<IBlogCategoryRepository, BlogCategoryRepository>();
            services.AddTransient<IBlogRepository, BlogRepository>();
            services.AddTransient<IBlogTagRepository, BlogTagRepository>();
            services.AddTransient<IFeedbackRepository, FeedbackRepository>();
            services.AddTransient<ISupportRepository, SupportRepository>();
            services.AddTransient<INotifyRepository, NotifyRepository>();
            services.AddTransient<IWalletTransferRepository, WalletTransferRepository>();
            services.AddTransient<ITicketTransactionRepository, TicketTransactionRepository>();
            services.AddTransient<IConfigRepository, ConfigRepository>();
            services.AddTransient<IWalletTransactionRepository, WalletTransactionRepository>();
            services.AddTransient<IInvestBotConfigRepository, InvestBotConfigRepository>();
            services.AddTransient<IInvestProfitHistoryRepository, InvestProfitHistoryRepository>();
            services.AddTransient<IStakingRepository, StakingRepository>();
            services.AddTransient<IStakingRewardRepository, StakingRewardRepository>();
            services.AddTransient<IStakingAffiliateRepository, StakingAffiliateRepository>();
            services.AddTransient<ITokenPriceHistoryRepository, TokenPriceHistoryRepository>();
            services.AddTransient<IAirdropRepository, AirdropRepository>();
            
            //Service
            services.AddTransient<IAirdropService, AirdropService>();
            services.AddTransient<IStakingAffiliateService, StakingAffiliateService>();
            services.AddTransient<IStakingRewardService, StakingRewardService>();
            services.AddTransient<IStakingService, StakingService>();
            services.AddTransient<ITicketTransactionService, TicketTransactionService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IWalletTransferService, WalletTransferService>();
            services.AddTransient<ITRONService, TRONService>();
            services.AddTransient<IHttpService, HttpService>();
            services.AddTransient<INotifyService, NotifyService>();
            services.AddTransient<ISupportService, SupportService>();
            services.AddTransient<IBlockChainService, BlockChainService>();
            services.AddTransient<IFunctionService, FunctionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IMenuGroupService, MenuGroupService>();
            services.AddTransient<IMenuItemService, MenuItemService>();
            services.AddTransient<IBlogCategoryService, BlogCategoryService>();
            services.AddTransient<IBlogService, BlogService>();
            services.AddTransient<IFeedbackService, FeedbackService>();
            services.AddTransient<IWalletTransactionService, WalletTransactionService>();
            services.AddTransient<IImportManager, ImportManager>();
            services.AddTransient<IConfigService, ConfigService>();

            services.AddTransient<IAuthorizationHandler, BaseResourceAuthorizationHandler>();

            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<TelegramBotWrapper>();
            services.AddTransient<IImportExcelService, ImportExcelService>();

            services.AddTransient<IInvestTradingBotService, InvestTradingBotService>();
            services.AddTransient<IInvestTradingConfigService, InvestTradingConfigService>();
            services.AddTransient<ITokenPriceHistoryService, TokenPriceHistoryService>();
        }

       

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddFile("Logs/fbsbatdongsan-{Date}.txt");
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseImageResizer();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMinResponse();

            app.UseSession();

            
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseAuthentication();

           

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(routes =>
            {
                
                routes.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                
                routes.MapControllerRoute(name: "areaRoute", pattern: "{area:exists}/{controller=Login}/{action=Index}/{id?}");

                routes.MapHub<LuckyHub<IdentityUser, IdentityRole>>("/hubs/lucky");
            });


        }

        public IServiceCollection AddSignalRService(IServiceCollection services)
        {
            services.AddSignalR(option => option.EnableDetailedErrors = true)
                .AddJsonProtocol();

            return services;
        }

        protected IRootConfiguration CreateRootConfiguration()
        {
            var rootConfiguration = new RootConfiguration();
            Configuration.GetSection(ConfigurationConsts.GameConfigurationKey).Bind(rootConfiguration.GameConfiguration);
            return rootConfiguration;
        }
    }
}
