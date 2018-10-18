using Business.Interfaces;
using Business.Services.Membership;
using Common.BaseClasses;
using Models.Application;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Tests.Utilities {
	public class Test_AuthenticationPersistenceService : LoggingWorker, IAuthenticationPersistenceService {
		public UserContext User { get; private set; }

		public Test_AuthenticationPersistenceService(ILoggerFactory loggerFactory) : base(loggerFactory) {
		}

		public UserCookieInfo GetUserInfo() {
			return new UserCookieInfo {
				UserSessionId = Guid.NewGuid(),
				OriginalLogin = this.User?.Name
			};
		}

		public async Task PersistUser(UserContext user) {
			await Task.Run(() => { this.User = user; });
		}

		public UserContext RetrieveUser() {
			return this.User;
		}

		public async Task<bool> SignOut() {
			await Task.Run(() => {
				this.User = null;
			});
			return true;
		}
	}
}