using Business.Enums;
using Business.Interfaces;
using Business.Security;
using Business.Service.EntityServices.Interfaces;
using Business.Services.Cookies;
using Business.Services.EntityServices;
using Business.Services.Membership;
using Common.Interfaces;
using Common.Services;
using Common.Web.Interfaces;
using Common.Web.Services;
using Models.Application;
using Data.Repository.Dapper;
using Data.Repository.Dapper.Interfaces;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using NLog.Web;
using Tests.Models;
using Tests.Utilities;
using System;
using NLog.Config;
using NLog;
using Common.IO;

namespace Tests {
	public class Startup {
		internal IServiceProvider Setup() {
			var pathToNLogConfig = FileUtility.GetFullPath_FromRelativePath<Startup>("nlog.config");
			var pathToAppSettingsConfig = FileUtility.GetFullPath_FromRelativePath<Startup>("appsettings.json");
			var provider = new PhysicalFileProvider(pathToNLogConfig.path);
			LogManager.Configuration = new XmlLoggingConfiguration(pathToNLogConfig.filePath);
			//loggerFactory.AddNLog();
			//env.ConfigureNLog("nlog.config");
			var services = new ServiceCollection();
			services.AddLogging();
			var config = new ConfigurationBuilder().AddJsonFile(pathToAppSettingsConfig.filePath).Build();
			services.Configure<AppSettings>(config.GetSection("AppSettings"));
			services.AddSingleton<IFileProvider>(provider);
			//AppCache
			services.AddMemoryCache();
			//SessionCache
			services.AddDistributedMemoryCache();
			services.AddSession(options => {
				options.Cookie.HttpOnly = true;
			});
			services.AddMvc(options => {
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