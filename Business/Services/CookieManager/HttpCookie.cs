using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Business.Services.Cookies {
	public class HttpCookie : ICookie {
		public ICollection<string> Keys {
			get {
				if (_httpContext == null) {
					throw new ArgumentNullException(nameof(_httpContext));
				}

				return _httpContext.Request.Cookies.Keys;
			}
		}

		private static readonly string Purpose = "CookieManager.Token.v1";

		private readonly ChunkingHttpCookie _chunkingHttpCookie;

		private readonly CookieManagerOptions _cookieManagerOptions;

		private readonly IDataProtector _dataProtector;

		private readonly HttpContext _httpContext;

		public HttpCookie(IHttpContextAccessor httpAccessor, IDataProtectionProvider dataProtectionProvider, IOptions<CookieManagerOptions> optionAccessor) {
			_httpContext = httpAccessor.HttpContext;
			_dataProtector = dataProtectionProvider.CreateProtector(Purpose);
			_cookieManagerOptions = optionAccessor.Value;
			_chunkingHttpCookie = new ChunkingHttpCookie(optionAccessor);
		}

		public bool Contains(string key) {
			if (_httpContext == null) {
				throw new ArgumentNullException(nameof(_httpContext));
			}

			if (key == null) {
				throw new ArgumentNullException(nameof(key));
			}

			return _httpContext.Request.Cookies.ContainsKey(key);
		}

		public string Get(string key) {
			if (_httpContext == null) {
				throw new ArgumentNullException(nameof(_httpContext));
			}

			if (key == null) {
				throw new ArgumentNullException(nameof(key));
			}

			if (Contains(key)) {
				var encodedValue = _chunkingHttpCookie.GetRequestCookie(_httpContext, key);
				var protectedData = string.Empty;
				//allow encryption is optional
				//may change the allow encryption to avoid this first check if cookie value is able to decode than unprotect tha data
				if (Base64TextEncoder.TryDecode(encodedValue, out protectedData)) {
					string unprotectedData;
					if (_dataProtector.TryUnprotect(protectedData, out unprotectedData)) {
						return unprotectedData;
					}
				}
				return encodedValue;
			}

			return string.Empty;
		}

		public void Remove(string key) {
			if (_httpContext == null) {
				throw new ArgumentNullException(nameof(_httpContext));
			}

			if (key == null) {
				throw new ArgumentNullException(nameof(key));
			}

			_chunkingHttpCookie.RemoveCookie(_httpContext, key);
		}

		public void Set(string key, string value, int? expireTime) {
			//validate input TODO
			if (_httpContext == null) {
				throw new ArgumentNullException(nameof(_httpContext));
			}

			if (key == null) {
				throw new ArgumentNullException(nameof(key));
			}

			Set(key, value, null, expireTime);
		}

		public void Set(string key, string value, CookieOptions option) {
			if (_httpContext == null) {
				throw new ArgumentNullException(nameof(_httpContext));
			}

			if (key == null) {
				throw new ArgumentNullException(nameof(key));
			}

			if (option == null) {
				throw new ArgumentNullException(nameof(option));
			}

			Set(key, value, option, null);
		}

		private void Set(string key, string value, CookieOptions option, int? expireTime) {
			if (option == null) {
				option = new CookieOptions();

				if (expireTime.HasValue)
					option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
				else
					option.Expires = DateTime.Now.AddDays(_cookieManagerOptions.DefaultExpireTimeInDays);
			}

			//check for encryption
			if (_cookieManagerOptions.AllowEncryption) {
				string protecetedData = _dataProtector.Protect(value);
				var encodedValue = Base64TextEncoder.Encode(protecetedData);
				_chunkingHttpCookie.AppendResponseCookie(_httpContext, key, encodedValue, option);
			} else {
				//just append the cookie
				_chunkingHttpCookie.AppendResponseCookie(_httpContext, key, value, option);
			}
		}
	}
}