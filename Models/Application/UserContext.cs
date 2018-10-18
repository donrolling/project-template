using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace Models.Application {
	public class UserContext : User, IIdentity {
		public string AuthenticationType { get; set; }
		[JsonIgnore]
		//not needed in the session cache, where we have an issue deserializing it.
		//used for claims identity name only
		public IEnumerable<Claim> Claims { get; set; }
		public bool IsAuthenticated { get; set; }
		public string Name { get { return this.Login; } }
	}
}