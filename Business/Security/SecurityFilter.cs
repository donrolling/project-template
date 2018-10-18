using Business.Interfaces;
using Common.Logging;
using Common.Web.Security.WebAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Business.Security {
	public class SecurityFilter : IActionFilter {
		public IHostingEnvironment HostingEnvironment { get; }
		public ILogger Logger { get; }
		private IMembershipService MembershipService { get; }

		public SecurityFilter(IMembershipService membershipService, IHostingEnvironment environment, ILoggerFactory loggerFactory) {
			this.MembershipService = membershipService ?? throw new ArgumentNullException(nameof(membershipService));
			this.HostingEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
			this.Logger = LogUtility.GetLogger(loggerFactory, this.GetType());
		}

		public void OnActionExecuted(ActionExecutedContext context) {
		}

		public void OnActionExecuting(ActionExecutingContext actionContext) {
			var claimRequirementAttributes = actionContext.ActionDescriptor.FilterDescriptors.Select(x => x.Filter).OfType<ClaimRequirementAttribute>();
			//not equals null just because...not sure that would ever happen
			foreach (var claimRequirementAttribute in claimRequirementAttributes.Where(a => a != null)) {
				var hasClaim = false;
				try {
					hasClaim = this.MembershipService.HasClaim(claimRequirementAttribute.Claim);
				} catch (Exception ex) {
					return;
				}
				if (hasClaim) { continue; }
				var errorMessage2 = HttpErrorMessage.CreateUnauthorized(this.HostingEnvironment, Messages.SecurityFilter_AccessDenied);
				actionContext.Result = new JsonResult(errorMessage2);
				break;
			}
		}
	}
}