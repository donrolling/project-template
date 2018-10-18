using Business.Services.Membership;
using Models.Application;
using Models.Entities;
using System;
using System.Threading.Tasks;

namespace Business.Interfaces {
	public interface IAuthenticationPersistenceService {

		UserCookieInfo GetUserInfo();

		Task PersistUser(UserContext user);

		UserContext RetrieveUser();

		Task<bool> SignOut();
	}
}