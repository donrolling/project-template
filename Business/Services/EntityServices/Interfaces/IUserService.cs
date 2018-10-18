using Data.Dapper.Models;
using Data.Interfaces;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Service.EntityServices.Interfaces {
	public interface IUserService {

		Task<User> Select_ById(long id);

		Task<User> Select_ByLogin(string login);
	}
}