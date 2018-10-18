using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services.Cookies {
	public class CookieManager : ICookieManager {
		private readonly ICookie _cookie;
		private List<string> deletedCookies = new List<string>();

		public CookieManager(ICookie cookie) {
			this._cookie = cookie;
		}

		public bool Contains(string key) {
			return _cookie.Contains(key);
		}

		public T Get<T>(string key) {
			if (this.deletedCookies.Any(a => a == key)) {
				return default(T);
			}
			return GetExisting<T>(key);
		}

		public T GetOrSet<T>(string key, Func<T> acquirer, int? expireTime = default(int?)) {
			if (_cookie.Contains(key) && !this.deletedCookies.Any(a => a == key)) {
				//get the existing value
				return GetExisting<T>(key);
			} else {
				var value = acquirer();
				this.Set(key, value, expireTime);

				return value;
			}
		}

		public T GetOrSet<T>(string key, Func<T> acquirer, CookieOptions option) {
			if (_cookie.Contains(key) && !this.deletedCookies.Any(a => a == key)) {
				//get the existing value
				return GetExisting<T>(key);
			} else {
				var value = acquirer();
				this.Set(key, value, option);
				return value;
			}
		}

		public void Remove(string key) {
			if (!this.deletedCookies.Any(a => a == key)) {
				this.deletedCookies.Add(key);
				_cookie.Remove(key);
				//actually removes the cookie
				_cookie.Set(key, "", new CookieOptions { Expires = DateTime.Now.AddMinutes(-20) });
			}
		}

		public void Set(string key, object value, int? expireTime = default(int?)) {
			_cookie.Set(key, JsonConvert.SerializeObject(value), expireTime);
		}

		public void Set(string key, object value, CookieOptions option) {
			_cookie.Set(key, JsonConvert.SerializeObject(value), option);
		}

		private T GetExisting<T>(string key) {
			var value = _cookie.Get(key);

			if (string.IsNullOrEmpty(value))
				return default(T);

			return JsonConvert.DeserializeObject<T>(value);
		}
	}
}