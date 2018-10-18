using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices.Base;
using Common.Interfaces;
using Common.Web.Interfaces;
using Models.Application;
using Models.Base;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services.Membership {
	public class MembershipService : EntityServiceBase, IMembershipService {
		public IAppCacheService AppCacheManager { get; private set; }
		public IOptions<AppSettings> AppSettings { get; set; }
		public IAuthenticationPersistenceService AuthenticationPersistenceService { get; private set; }
		public IAuthorizationProcessor AuthorizationProcessor { get; private set; }
		public IFileProvider FileProvider { get; private set; }
		public IHttpContextAccessor HttpContextAccessor { get; }
		public ISessionCacheService SessionManager { get; private set; }
		public IUserService UserService { get; private set; }
		private const string _returnUrlQSParam = "?returnUrl=";
		private static readonly Auditing _auditing = new Auditing();
		/// <summary>
		/// If you don't have one of these roles, you're outta here.
		/// </summary>
		private static readonly List<string> _requiredRoles = new List<string> { 
			RoleEnum.Guest.ToString()
		};

		public MembershipService(
			IAppCacheService appCacheService,
			ISessionCacheService sessionCacheService,
			IAuthorizationProcessor authorizationProcessor,
			IUserService userService,
			IFileProvider fileProvider,
			IAuthenticationPersistenceService authenticationPersistenceService,
			IHttpContextAccessor httpContextAccessor,
			IOptions<AppSettings> appSettings,
			ILoggerFactory loggerFactory
		) : base(_auditing, loggerFactory) {
			this.AppCacheManager = appCacheService;
			this.FileProvider = fileProvider;
			this.SessionManager = sessionCacheService;
			this.UserService = userService;
			this.AppSettings = appSettings;
			this.AuthenticationPersistenceService = authenticationPersistenceService;
			this.AuthorizationProcessor = authorizationProcessor;
			this.HttpContextAccessor = httpContextAccessor;
		}

		public async Task<UserContext> Current(bool firstTime = true) {
			//try {
			//check for persisted user
			var savedUser = this.AuthenticationPersistenceService.RetrieveUser();
			if (savedUser != null) {
				return savedUser;
			}
			//just in case the session is dead, but your cookies are alive.
			//kill the entire session and cookie ecosystem
			await this.AuthenticationPersistenceService.SignOut();
			//check http context for user
			var userInfo = this.AuthenticationPersistenceService.GetUserInfo();
			if (string.IsNullOrEmpty(userInfo.OriginalLogin)) {//give up
				return this.GuestUser();
			}
			var user = await this.UserService.Select_ByLogin(userInfo.OriginalLogin);
			return await hydrateUser(user);
			//} catch (Exception ex) {
			//	await this.AuthenticationPersistenceService.SignOut();
			//	if (firstTime) {
			//		return await Current(false);
			//	}
			//	return this.GuestUser();
			//}
		}

		public async Task<long> CurrentUserId() {
			var user = await this.Current();
			return user.Id;
		}

		public UserContext GuestUser() {
			return new UserContext {
				Login = "Guest",
				IsAuthenticated = false
			};
		}

		public bool HasClaim(System.Security.Claims.Claim claim) {
			//todo: fill this in for your application
			return true;
		}

		public async Task ReplaceRoles(List<string> roles) {
			var user = await this.Current();
			await this.SetCurrentUser(user);
		}

		public async Task<UserContext> Set_ById(long id) {
			var user = await this.UserService.Select_ById(id);
			return await hydrateUser(user);
		}

		public async Task<UserContext> Set_ByLogin(string login) {
			var user = await this.UserService.Select_ByLogin(login);
			return await hydrateUser(user);
		}

		public async Task<bool> SetCurrentUser(UserContext user) {
			if (user == null) { return true; }
			await this.AuthenticationPersistenceService.PersistUser(user);
			return true;
		}

		public async Task<bool> SignOut() {
			return await this.AuthenticationPersistenceService.SignOut();
		}

		public async Task<bool> UserHasAccess(string claimValue) {
			//fill this in for your application
			return await Task.Run(() => { return true;  });
		}

		private async Task<UserContext> getUserContext(User user) {
			var userContext = new UserContext();
			userContext.InjectFrom(user);
			var domainWithoutSuffix = user.Domain.Contains('.') ? user.Domain.Split('.')[0] : user.Domain;
			var windowsAccountName = $"{ domainWithoutSuffix.ToUpper() }\\{ user.Login }";
			userContext.Claims = new List<System.Security.Claims.Claim> {
				new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Login),
				new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.WindowsAccountName, windowsAccountName)
			};
			userContext.IsAuthenticated = true;
			userContext.AuthenticationType = CookieAuthenticationDefaults.AuthenticationScheme;
			return await Task.Run(() => { return userContext; });
		}

		private async Task<UserContext> hydrateUser(User user) {
			if (user == null) { return null; }
			var userContext = await getUserContext(user);
			await this.SetCurrentUser(userContext);
			return userContext;
		}

	}
}