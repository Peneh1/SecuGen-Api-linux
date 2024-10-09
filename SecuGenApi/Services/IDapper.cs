using Dapper;
using System.Data;
using System.Data.Common;

namespace Dapper_ORM.Services
{
	/// <summary>
	/// Dapper Interface
	/// </summary>
	public interface IDapper : IDisposable
	{
		DbConnection GetDbconnection();
		T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
		List<T> GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
		int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
		T Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
		T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
		int Update(string sp, DynamicParameters parms);
	}
}