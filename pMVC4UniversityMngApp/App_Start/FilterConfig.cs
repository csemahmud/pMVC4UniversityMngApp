using System.Web;
using System.Web.Mvc;

namespace pMVC4UniversityMngApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}