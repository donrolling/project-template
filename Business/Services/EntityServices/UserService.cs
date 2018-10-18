using Business.Service.EntityServices.Interfaces;
using Common.BaseClasses;
using Common.Extensions;
using Data.Dapper.Models;
using Data.Interfaces;
using Models.Application;
using Data.Repository.Dapper.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Business.Services.EntityServices {
	public class UserService : LoggingWorker, IUserService {
		public IUserRepository UserRepository { get; }

		public UserService(IUserRepository userRepository, ILoggerFactory loggerFactory) : base(loggerFactory) {
			this.UserRepository = userRepository;
		}


		public async Task<User> Select_ById(long id) {
			if (id == 0) { return null; }
			var user = await this.UserRepository.Select_ById(id);
			return user;
		}

		public async Task<User> Select_ByLogin(string login) {
			if (string.IsNullOrEmpty(login)) {
				this.Logger.LogInformation("Select_ByLogin() login was empty.");
				return null;
			}
			try {
				var user = await this.UserRepository.Select_ByLogin(login);
				return user;
			} catch (System.Exception ex) {
				this.Logger.LogError(ex, "Select_ByLogin");
				throw;
			}
		}
	}
}