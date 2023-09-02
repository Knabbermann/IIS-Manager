using IIS_Manager.Controllers.WinRM;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace IIS_Manager.Web.Areas.User.Pages.IIS_Servers
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordEncrypter _passwordEncrypter;
        private readonly IToastNotification _toastNotification;

        public IndexModel(IUnitOfWork unitOfWork,
            PasswordEncrypter passwordEncrypter,
            IToastNotification toastNotification)
        {
            _unitOfWork = unitOfWork;
            _passwordEncrypter = passwordEncrypter;
            _toastNotification = toastNotification;
        }

        public IEnumerable<IisServer> IisServers { get; set; }

        public void OnGet()
        {
            IisServers = _unitOfWork.IisServer.GetAll();
        }

        public JsonResult OnGetHealthCheck()
        {
            IisServers = _unitOfWork.IisServer.GetAll();

            foreach (var iisServer in IisServers)
            {
                var cWinRmController = new WinRmController(iisServer.Id, _unitOfWork, _passwordEncrypter);
                var cHealthCheck = cWinRmController.HealthCheck();
                var buttonClass = cHealthCheck[0] == "success"
                    ? "btn-outline-success"
                    : "btn-outline-danger";
                ViewData[$"{iisServer.Id}_buttonClass"] = buttonClass;

                var toolTip = cHealthCheck[0].Equals("success")
                    ? $"{cHealthCheck[0]}:\nResponse time: {cHealthCheck[1]} ms"
                    : $"{cHealthCheck[0]}:\n{cHealthCheck[1]}";
                ViewData[$"{iisServer.Id}_toolTip"] = toolTip;
                iisServer.HealthCheck = cHealthCheck;
            }

            return new JsonResult(IisServers);
        }

        public IActionResult OnPost(string id)
        {
            var cWinRmController = new WinRmController(id, _unitOfWork, _passwordEncrypter);
            var cHealthCheck = cWinRmController.HealthCheck();

            if (cHealthCheck[0].Equals("success"))
                _toastNotification.AddSuccessToastMessage($"Response time: {cHealthCheck[1]} ms");
            else
                _toastNotification.AddErrorToastMessage($"{cHealthCheck[1]}");

            OnGet();
            return Page();
        }
    }
}
