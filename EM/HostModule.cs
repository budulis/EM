using System.Web.UI.WebControls;
using Nancy;

namespace EM
{
	public class HostModule : NancyModule {
		public HostModule()
		{
			Get["/"] = p => View["Index.html"];
		}
	}
}