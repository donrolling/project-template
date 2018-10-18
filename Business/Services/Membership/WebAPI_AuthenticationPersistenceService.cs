using Business.Interfaces;
using Business.Services.Cookies;
using Common.BaseClasses;
using Common.Web.Interfaces;
using Models.Application;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Services.Membership {
	public class UserCookieInfo {
		public string OriginalLogin { get; set; }
		public Guid UserSessionId { get; set; }
	}

	public class WebAPI_AuthenticationPersistenceService : LoggingWorker, IAuthenticationPersistenceService {
		public ICookieManager CookieManager { get; set; }
		public IHttpContextAccessor HttpContextAccessor { get; }
		public ISessionCacheService SessionCacheService { get; }
		public const string AUTH_SESSION_KEY = "AuthenticationPersistenceService";
		public const string COOKIE_NAME = "AuthenticationPersistenceCookie";
		public const string COOKIE_NAME2 = ".AspNetCore.InquiryAuthCookie";

		public WebAPI_AuthenticationPersistenceService(
			ISessionCacheService sessionCacheService,
			IHttpContextAccessor httpContextAccessor,
			ICookieManager cookieManager,
			ILoggerFactory loggerFactory
		) : base(loggerFactory) {
			this.HttpContextAccessor = httpContextAccessor;
			this.SessionCacheService = sessionCacheService;
			this.CookieManager = cookieManager;
		}

		public UserCookieInfo GetUserInfo() {
			try {
				var cookie = this.CookieManager.GetOrSet<UserCookieInfo>(
					COOKIE_NAME,
					() => {
						return new UserCookieInfo {
							UserSessionId = Guid.NewGuid(),
							OriginalLogin = this.getUsernameFromHttpContext()
						};
					},
					new CookieOptions {
						Expires = DateTimeOffset.Now.AddMinutes(20),
					}
				);
				return cookie;
			} catch (Exception ex) {
				this.Logger.LogError(ex, "Cookie misread.");
				this.CookieManager.Remove(COOKIE_NAME);
				this.CookieManager.Remove(COOKIE_NAME2);
			}
			return new UserCookieInfo();
		}

		public async Task PersistUser(UserContext user) {
			var userInfo = this.GetUserInfo();
			var identity = new ClaimsIdentity(user.Claims, AuthenticationSchemes.InquiryAuthCookie);			
			var userPrincipal = new ClaimsPrincipal(identity);			
			var authenticationProperties = new AuthenticationProperties {
				ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
				IsPersistent = true,
				AllowRefresh = true
			};
			try {
				await this.HttpContextAccessor.HttpContext.SignInAsync(AuthenticationSchemes.InquiryAuthCookie, userPrincipal, authenticationProperties);
			} catch (Exception ex) {
				this.Logger.LogError(ex, "Sign In Error");
				throw;
			}
			this.SessionCacheService.Set(this.getKey(userInfo.UserSessionId), user, true);
		}

		public UserContext RetrieveUser() {
			var userInfo = this.GetUserInfo();
			if (userInfo.UserSessionId == Guid.Empty) {
				return null;
			}
			return this.SessionCacheService.Get<UserContext>(this.getKey(userInfo.UserSessionId), true);
		}

		public async Task<bool> SignOut() {
			var userInfo = this.GetUserInfo();
			try {
				await this.SessionCacheService.RemoveAsync(this.getKey(userInfo.UserSessionId));
				await this.HttpContextAccessor.HttpContext.SignOutAsync();
				this.CookieManager.Remove(COOKIE_NAME);
				this.CookieManager.Remove(COOKIE_NAME2);
				return true;
			} catch (Exception ex) {
				this.Logger.LogError(ex, "Sign Out Error");
				throw;
			}
		}

		private string getKey(Guid userSessionId) {
			return $"{ AUTH_SESSION_KEY }-{ userSessionId.ToString() }";
		}

		private string getUsernameFromHttpContext() {
			var identity = this.HttpContextAccessor?.HttpContext?.User?.Identity?.Name;
			if (string.IsNullOrEmpty(identity)) {
				throw new Exception("User Identity not found.");
			}
			if (identity.Contains("\\")) {
				return identity.Split('\\')[1];
			}
			//this.Logger.LogInformation($"getUsernameFromHttpContext() { identity }.");
			return identity;
		}
	}
}