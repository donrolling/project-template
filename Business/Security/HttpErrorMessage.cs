using Business.Models;
using Models.Application;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Business.Security {
	public class HttpErrorMessage {
		public string Message { get; set; }
		public string RedirectUrl { get; set; }
		public int Status { get; set; }

		public static HttpErrorMessage Create(IHostingEnvironment env, HttpStatusCode code, string url, string msg) {
			//if (env.IsProduction()) {
			//	msg = Messages.HttpErrorMessage_ASPNETCORE_ENVIRONMENT;
			//}
			return new HttpErrorMessage {
				Status = (int)code,
				RedirectUrl = url,
				Message = msg
			};
		}

		public static HttpErrorMessage CreateError(IHostingEnvironment env, string msg) {
			return HttpErrorMessage.Create(env, HttpStatusCode.InternalServerError, Urls.Error, msg);
		}

		public static HttpErrorMessage CreateUnauthorized(IHostingEnvironment env, string msg) {
			return HttpErrorMessage.Create(env, HttpStatusCode.Forbidden, Urls.UnauthorizedUrl, msg);
		}
	}
}