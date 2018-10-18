using Common.Logging;
using Models.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;

namespace Tests.Models {
	[TestClass]
	public class TestBase {
		public IMembershipService MembershipService { get; private set; }
		public IUserService UserService { get; }
		public IOptions<AppSettings> AppSettings { get; }
		public ILogger Logger { get; }
		public ILoggerFactory LoggerFactory { get; }
		public TestContext TestContext { get; set; }
		public IServiceProvider ServiceProvider { get; }
		public string TestName {
			get {
				return this.TestContext.TestName;
			}
		}

		public TestBase() {
			this.ServiceProvider = new Startup().Setup();
			this.LoggerFactory = this.ServiceProvider.GetService<ILoggerFactory>();
			this.Logger = LogUtility.GetLogger(this.ServiceProvider, this.GetType());
			this.AppSettings = this.ServiceProvider.GetService<IOptions<AppSettings>>();
			this.MembershipService = this.ServiceProvider.GetService<IMembershipService>();
			//totally cheating here.
			this.UserService = this.ServiceProvider.GetService<IUserService>();
			var user = this.UserService.Select_ById(0);
			var userContext = new UserContext { Id = user.Id };
			this.MembershipService.SetCurrentUser(userContext);
		}
	}
}