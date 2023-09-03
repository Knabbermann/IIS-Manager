using IIS_Manager.Controllers.Log;
using IIS_Manager.DataAccess.Repository;
using IIS_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IIS_Manager.Web.Areas.User.Pages.Logs
{
    public class IndexModel : PageModel
    {
        private readonly ILogController _logController;

        public IndexModel(ILogController logController)
        {
            _logController = logController;
        }

        [BindProperty(SupportsGet = true)]
        public IEnumerable<Log> Logs { get; set; }

        public void OnGet()
        {
            Logs = _logController.GetAllLogs().OrderByDescending(x => x.CreatedDate);
        }
    }
}
