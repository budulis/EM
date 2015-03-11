using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace EM
{
	class Database
	{
		public static readonly string DbFileName = "\\Backup_2015_03_11.expensemanager";
		public static readonly string AllDataQuery = @"select b.recorded Date,b.amount Amount,b.note Notes,c.name Category,c._id CategoryId,c.color ColorCode from bills b inner join categories c on c._id = b.category_id";
		public static readonly string AllCategoriesQuery = @"select _id Id, color ColorCode,name Name  from categories";

		readonly string _dbConnection;

		public Database(String inputFile) {
			_dbConnection = String.Format("Data Source={0}", inputFile);
		}

		public Database(Dictionary<String, String> connectionOpts) {
			var str = connectionOpts.Aggregate("", (current, row) => current + String.Format("{0}={1}; ", row.Key, row.Value));
			str = str.Trim().Substring(0, str.Length - 1);
			_dbConnection = str;
		}

		public async Task<IEnumerable<dynamic>> GetAllData(string sql) {
			var data = new List<dynamic>();

			using (var conn = new SQLiteConnection(_dbConnection))
			using (var command = new SQLiteCommand(conn)) {
				try {

					conn.Open();
					command.CommandText = sql;
					var reader = await command.ExecuteReaderAsync();
					while (reader.Read()) {
						
						var date = new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds(Convert.ToDouble(reader["Date"])));
						var amount = Convert.ToDecimal(reader["Amount"]);
						if (date.Year < 2015)
							amount = Math.Round(amount/3.4528M, MidpointRounding.AwayFromZero);
						var notes = (string)reader["Notes"];
						var category = (string)reader["Category"];
						var categoryId = (long)reader["CategoryId"];
						var colorCode = Color.FromArgb((int)reader["ColorCode"]);

						data.Add((dynamic)new
						{
							Date = date, 
							Amount = amount, 
							Notes = notes, 
							Category = category, 
							CategoryId = categoryId, 
							ColorCode = "rgba(" + colorCode.R + "," + colorCode.G + "," + colorCode.B + "," + colorCode.A + ")"
						});
					}
				}
				catch (Exception ex) {
					Debug.WriteLine(ex);
				}
			}

			return data;
		}

		public async Task<IEnumerable<dynamic>> GetCategories(string sql) {
			var data = new List<dynamic>();

			using (var conn = new SQLiteConnection(_dbConnection))
			using (var command = new SQLiteCommand(conn)) {
				try {

					conn.Open();
					command.CommandText = sql;
					var reader = await command.ExecuteReaderAsync();

					while (reader.Read()) {

						var id = reader["Id"];
						var colorCode = Color.FromArgb((int)reader["ColorCode"]);
						var name = (string)reader["Name"];

						data.Add((dynamic)new { Id = id,Name = name, ColorCode = "rgba(" + colorCode.R + "," + colorCode.G+ "," +colorCode.B + ","  + colorCode.A + ")"});
					}
				}
				catch (Exception ex) {
					Debug.WriteLine(ex);
				}
			}

			return data;
		}
		#region NotUsedSoFar
		//public int ExecuteNonQuery(string sql) {
		//	var cnn = new SQLiteConnection(_dbConnection);
		//	cnn.Open();
		//	var mycommand = new SQLiteCommand(cnn) {CommandText = sql};
		//	var rowsUpdated = mycommand.ExecuteNonQuery();
		//	cnn.Close();
		//	return rowsUpdated;
		//}

		//public string ExecuteScalar(string sql) {
		//	var cnn = new SQLiteConnection(_dbConnection);
		//	cnn.Open();
		//	var mycommand = new SQLiteCommand(cnn)
		//	{
		//		CommandText = sql
		//	};
		//	var value = mycommand.ExecuteScalar();
		//	cnn.Close();
		//	return value != null ? value.ToString() : "";
		//}

		//public bool Update(String tableName, Dictionary<String, String> data, String where) {
		//	String vals = "";
		//	Boolean returnCode = true;
		//	if (data.Count >= 1) {
		//		foreach (KeyValuePair<String, String> val in data) {
		//			vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
		//		}
		//		vals = vals.Substring(0, vals.Length - 1);
		//	}
		//	try {
		//		this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
		//	}
		//	catch {
		//		returnCode = false;
		//	}
		//	return returnCode;
		//}

		//public bool Delete(String tableName, String where)
		//{
		//	Boolean returnCode = true;
		//	try
		//	{
		//		this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
		//	}
		//	catch (Exception fail)
		//	{
		//		returnCode = false;
		//	}
		//	return returnCode;
		//}

		//public bool Insert(String tableName, Dictionary<String, String> data) {
		//	String columns = "";
		//	String values = "";
		//	Boolean returnCode = true;
		//	foreach (KeyValuePair<String, String> val in data) {
		//		columns += String.Format(" {0},", val.Key.ToString());
		//		values += String.Format(" '{0}',", val.Value);
		//	}
		//	columns = columns.Substring(0, columns.Length - 1);
		//	values = values.Substring(0, values.Length - 1);
		//	try {
		//		this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
		//	}
		//	catch (Exception fail) {
		//		returnCode = false;
		//	}
		//	return returnCode;
		//}

		//public bool ClearDB() {
		//	DataTable tables;
		//	try {
		//		tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
		//		foreach (DataRow table in tables.Rows) {
		//			this.ClearTable(table["NAME"].ToString());
		//		}
		//		return true;
		//	}
		//	catch {
		//		return false;
		//	}
		//}

		//public bool ClearTable(String table) {
		//	try {

		//		this.ExecuteNonQuery(String.Format("delete from {0};", table));
		//		return true;
		//	}
		//	catch {
		//		return false;
		//	}
		//}
		#endregion
	}
}