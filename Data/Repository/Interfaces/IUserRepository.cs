using Data.Dapper.Models;
using Data.Interfaces;
using Models.Application;
using System.Threading.Tasks;

namespace Data.Repository.Dapper.Interfaces {
	public interface IUserRepository {


		Task<User> Select_ById(long id);

		Task<User> Select_ByLogin(string login);
	}
}