using Models.Application;
using Data.Repository.Dapper;
using Data.Repository.Dapper.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Data.Repository.Interfaces {
	public class UserDapperRepository : DapperAsyncRepository, IUserRepository {

		public UserDapperRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory) {
		}

		public async Task<User> Select_ById(long id) {
			return await Task.Run(() => { return new User { Id = id }; });
		}

		public async Task<User> Select_ByLogin(string login) {
			return await Task.Run(() => { return new User { Id = 1 }; });
		}
	}
}