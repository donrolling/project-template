using Common.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Web.Interfaces {
	public interface IWebRequestService {

		Task<ServerResponse> Get(string claimValue);

		ResponseType GetResponseType(HttpStatusCode statusCode);

		Task<ServerResponse> Post(string claimValue, Dictionary<string, string> data);

		Task<ServerResponse> PostJson(string claimValue, object data);
	}
}