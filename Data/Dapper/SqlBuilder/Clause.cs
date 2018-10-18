namespace Data.Repository.Dapper.Base {
	public class Clause {
		public string Sql { get; set; }
		public object Parameters { get; set; }
		public bool IsInclusive { get; set; }
	}
}