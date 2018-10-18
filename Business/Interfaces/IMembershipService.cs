using Business.Models;
using Common.Interfaces;
using Models.Application;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Interfaces {
	public interface IMembershipService {
		IAppCacheService AppCacheManager { get; }

		Task<UserContext> Current(bool firstTime = true);
				
		Task<long> CurrentUserId();

		UserContext GuestUser();

		bool HasClaim(Claim claim);

		Task ReplaceRoles(List<string> roles);

		Task<UserContext> Set_ById(long id);

		Task<UserContext> Set_ByLogin(string login);

		Task<bool> SetCurrentUser(UserContext user);

		Task<bool> SignOut();

		Task<bool> UserHasAccess(string claimValue);
	}
}