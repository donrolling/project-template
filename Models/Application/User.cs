using Models.Base;
using System;
using System.Collections.Generic;

namespace Models.Application {
	public class User : Entity<long> {
		public string DisplayName { get; set; }
		public string Domain { get; set; }
		public string FirstName { get; set; }
		public Guid ForeignId { get; set; }
		public string LastName { get; set; }
		public string Login { get; set; }
		public IEnumerable<string> Roles { get; set; } = new List<string>();
	}
}