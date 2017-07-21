using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApiTest
{
    public class FilterConfig
    {

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new System.Web.Http.AuthorizeAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
