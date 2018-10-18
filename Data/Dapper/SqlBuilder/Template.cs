using Dapper;
using System.Text.RegularExpressions;

namespace Data.Repository.Dapper.Base {
	public class Template {
		private readonly string sql;
		private readonly SqlBuilder builder;
		private readonly object initParams;
		private int dataSeq = -1; // Unresolved

		public Template(SqlBuilder builder, string sql, dynamic parameters) {
			this.initParams = parameters;
			this.sql = sql;
			this.builder = builder;
		}

		private static Regex regex = new Regex(@"\/\*\*.+\*\*\/", RegexOptions.Compiled | RegexOptions.Multiline);

		private void ResolveSql() {
			if (dataSeq != builder.seq) {
				DynamicParameters p = new DynamicParameters(initParams);

				rawSql = sql;

				foreach (var pair in builder.Data) {
					rawSql = rawSql.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
				}
				parameters = p;

				// replace all that is left with empty
				rawSql = regex.Replace(rawSql, "");

				dataSeq = builder.seq;
			}
		}

		private string rawSql;
		private object parameters;

		public string RawSql { get { ResolveSql(); return rawSql; } }
		public object Parameters { get { ResolveSql(); return parameters; } }
	}
}