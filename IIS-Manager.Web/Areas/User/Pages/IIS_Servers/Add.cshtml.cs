using IIS_Manager.Controllers.Log;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Security.Claims;

namespace IIS_Manager.Web.Areas.User.Pages.IIS_Servers
{
    [Authorize]
    public class AddModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordEncrypter _passwordEncrypter;
        private readonly IToastNotification _toastNotification;
        private readonly ILogController _logController;

        public AddModel(IUnitOfWork unitOfWork, 
            PasswordEncrypter passwordEncrypter,
            IToastNotification toastNotification,
            ILogController logController)
        {
            _unitOfWork = unitOfWork;
            _passwordEncrypter = passwordEncrypter;
            _toastNotification = toastNotification;
            _logController = logController;
        }

        [BindProperty]
        public IisServer cIisServer { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost(IisServer cIisServer)
        {
            cIisServer.Id = Guid.NewGuid().ToString();
            cIisServer.PasswordHash = _passwordEncrypter.Encrypt(cIisServer.Password, cIisServer.Id);
            ModelState.Remove("cIisServer.PasswordHash");
            ModelState.Remove("cIisServer.ErrorMessage");
            ModelState.Remove("cIisServer.AppPools");
            ModelState.Remove("cIisServer.HealthCheck");
            ModelState.Remove("cIisServer.Id");
            if (ModelState.IsValid)
            {
                _unitOfWork.IisServer.Add(cIisServer);
                _unitOfWork.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Successfully added IIS-Server.");
                _logController.Log($"added Iis-Server with id {cIisServer.Id}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToPage("/IIS_Servers/Index");
            }

            return Page();
        }
    }
}
