using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Business.Services.Cookies {
	public interface ICookie {
		ICollection<string> Keys { get; }

		bool Contains(string key);

		string Get(string key);

		void Remove(string key);

		void Set(string key, string value, int? expireTime);

		void Set(string key, string value, CookieOptions option);
	}
}