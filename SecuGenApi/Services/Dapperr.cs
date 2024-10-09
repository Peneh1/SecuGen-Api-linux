using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace Dapper_ORM.Services {
    /// <summary>
    /// Dapper Implementation
    /// </summary>
    public class Dapperr : IDapper
	{
		private readonly IConfiguration _config;
		private string Connectionstring = "DefaultConnection";

		/// <inheritdoc/>
		public Dapperr(IConfiguration config)
		{
			_config = config;
		}
		/// <inheritdoc/>
		public void Dispose()
		{

		}

		/// <inheritdoc/>
		public DbConnection GetDbconnection()
		{
			return new SqlConnection(_config.GetConnectionString(Connectionstring));
		}

		/// <inheritdoc/>
		public int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public T Insert<T>(string sp, DynamicParameters? parms, CommandType commandType = CommandType.StoredProcedure)
		{
			T result;
			using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
			try
			{
				if (db.State == ConnectionState.Closed)
					db.Open();

				using var tran = db.BeginTransaction();
				try
				{
					result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault()!;
					tran.Commit();
				}
				catch (Exception)
				{
					tran.Rollback();
					throw;
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				if (db.State == ConnectionState.Open)
					db.Close();
			}

			return result;
		}

		/// <inheritdoc/>
		public T Get<T>(string sp, DynamicParameters? parms, CommandType commandType = CommandType.Text)
		{
			using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
			return db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault()!;
		}

		/// <inheritdoc/>
		public List<T> GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
		{
			using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
			return db.Query<T>(sp, parms, commandType: commandType).ToList();
		}


		/// <inheritdoc/>
		public T Update<T>(string sp, DynamicParameters? parms, CommandType commandType = CommandType.StoredProcedure)
		{
			T result;
			using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
			try
			{
				if (db.State == ConnectionState.Closed)
					db.Open();

				using var tran = db.BeginTransaction();
				try
				{
					result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault()!;
					tran.Commit();
				}
				catch (Exception)
				{
					tran.Rollback();
					throw;
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				if (db.State == ConnectionState.Open)
					db.Close();
			}

			return result;
		}
		/// <inheritdoc/>

		public int Update(string sp, DynamicParameters? parms)
		{
			int result;
			using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
			try
			{
				if (db.State == ConnectionState.Closed)
					db.Open();

				using var tran = db.BeginTransaction();
				try
				{
					result = db.Execute(sp, parms, tran);
					tran.Commit();
				}
				catch (Exception)
				{
					tran.Rollback();
					throw;
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				if (db.State == ConnectionState.Open)
					db.Close();
			}

			return result;
		}

	}
}