using Business.Models.Membership;
using Models.Application;
using Models.Entities;

namespace Business.Interfaces {
	public interface IAuthorizationProcessor {

		AuthorizationResult Authorize(string url, UserContext user, RoleType roleType);

		bool HasPermission(UserContext user, long businessUnitId, RoleType roleType);

		AuthorizationResult Unauthorized();
	}
}