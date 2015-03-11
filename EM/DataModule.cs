using System.Diagnostics;
using System.Linq;
using Nancy;
using Nancy.ViewEngines;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace EM {
	public class DataModule : NancyModule {
		public DataModule() {
			Get["/data/bycategory/bar", true] = async (x, ct) => await GetDataByCategoryBar();
			Get["/data/bycategory/pie", true] = async (x, ct) => await GetDataByCategoryPie();
			Get["/data", true] = async (x, ct) => await GetData();
		}

		private static async Task<object> GetDataByCategoryPie() {
			var db = new Database(AppDomain.CurrentDomain.GetData("DataDirectory") + Database.DbFileName);
			var result = await db.GetAllData(Database.AllDataQuery);

			var data = result.GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
				.Select(x => new {
					Period = x.Key.ToString("yyyy-M"),
					Data = x.GroupBy(z => new {
						z.Category,
						z.ColorCode
					}).Select(z => new { label = z.Key.Category, color = z.Key.ColorCode, value = z.Sum(s => (int)s.Amount) })
				});

			return JsonConvert.SerializeObject(data);
		}

		private static async Task<string> GetDataByCategoryBar() {
			var db = new Database(AppDomain.CurrentDomain.GetData("DataDirectory") + Database.DbFileName);
			var result = await db.GetAllData(Database.AllDataQuery);

			var categories = await db.GetCategories(Database.AllCategoriesQuery);

			try {
				var data = result
					.GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
					.Select(
						x =>
							new {
								Period = x.Key.ToString("yyyy-M") + "(" + x.GroupBy(z => z.CategoryId).Sum(z => z.Sum(s => (int)s.Amount)) + ")",
								Data = x.GroupBy(z => z.CategoryId).Select(z => new { CatId = z.Key, Amount = z.Sum(s => (int)s.Amount) }).ToArray()
							})
					.ToArray();

				return JsonConvert.SerializeObject(new { Categories = categories, Data = data });
			}
			catch (Exception ex) {
				Debug.WriteLine(ex);
			}
			return "";
		}

		private static async Task<string> GetData() {
			var db = new Database(AppDomain.CurrentDomain.GetData("DataDirectory") + Database.DbFileName);

			var result = await db.GetAllData(Database.AllDataQuery);

			var data = result
				.GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
				.Select(x => new { Period = x.Key.ToString("yyyy-M"), Data = x });

			return JsonConvert.SerializeObject(data);
		}
	}
}