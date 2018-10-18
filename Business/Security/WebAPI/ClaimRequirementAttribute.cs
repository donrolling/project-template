using Business.Services.Membership;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.Web.Security.WebAPI {
	public class ClaimRequirementAttribute : Attribute, IFilterMetadata {
		public Claim Claim { get; }

		public ClaimRequirementAttribute(Business.Security.ClaimTypes claimType, NavigationSections claimValue) {
			this.Claim = new Claim(claimType.ToString(), claimValue.ToString());
		}

		public ClaimRequirementAttribute(string claimType, string claimValue) {
			this.Claim = new Claim(claimType, claimValue);
		}
	}
}