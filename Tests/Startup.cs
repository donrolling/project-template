using System;
using Business.Interfaces;
using Business.Security;
using Business.Service.EntityServices.Interfaces;
using Business.Services.Cookies;
using Business.Services.EntityServices;
using Business.Services.Membership;
using Common.Interfaces;
using Common.IO;
using Common.Services;
using Common.Web.Interfaces;
using Common.Web.Services;
using Data.Repository.Dapper.Interfaces;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Models.Application;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Config;
using Tests.Models;
using Tests.Utilities;

namespace Tests
{
    public class Startup
    {
        internal IServiceProvider Setup()
        {
            var pathToNLogConfig = FileUtility.GetFullPath_FromRelativePath<Startup>("nlog.config");
            var pathToAppSettingsConfig = FileUtility.GetFullPath_FromRelativePath<Startup>("appsettings.json");
            var provider = new PhysicalFileProvider(pathToNLogConfig.path);
            LogManager.Configuration = new XmlLoggingConfiguration(pathToNLogConfig.filePath);
            //loggerFactory.AddNLog();
            //env.ConfigureNLog("nlog.config");
            var config = new ConfigurationBuilder().AddJsonFile(pathToAppSettingsConfig.filePath).Build();
            var services = new ServiceCollection();
            services.AddLogging();
            //services.Configure<AppSettings>(config);
            services.AddSingleton<IFileProvider>(provider);
            //AppCache
            services.AddMemoryCache();
            //SessionCache
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
            });
            services.AddMvc(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            }).AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
            );
            services.AddCookieManager();
            services.AddSingleton<IHttpContextAccessor, FakeHttpContextAccessor>();
            services.AddScoped<SecurityFilter>();

            //our services
            services.AddTransient<IAppCacheService, AppCacheService>();
            services.AddTransient<ISessionCacheService, SessionCacheService>();
            services.AddTransient<IMembershipService, MembershipService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserRepository, UserDapperRepository>();
            services.AddTransient<IAuthorizationProcessor, WebAPI_AuthorizationProcessor>();

            //utility
            services.AddTransient<ICookieManager, CookieManager>();

            //use singleton in test world
            services.AddSingleton<IAuthenticationPersistenceService, Test_AuthenticationPersistenceService>();

            //GENERATED

            return services.BuildServiceProvider();
        }
    }
}