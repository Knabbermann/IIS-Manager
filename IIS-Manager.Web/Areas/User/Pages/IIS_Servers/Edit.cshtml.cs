using IIS_Manager.Controllers.Log;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using System.Security.Claims;

namespace IIS_Manager.Web.Areas.User.Pages.IIS_Servers
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordEncrypter _passwordEncrypter;
        private readonly IToastNotification _toastNotification;
        private readonly ILogController _logController;

        public EditModel(IUnitOfWork unitOfWork,
            PasswordEncrypter passwordEncrypter,
            IToastNotification toastNotification,
            ILogController logController)
        {
            _unitOfWork = unitOfWork;
            _passwordEncrypter = passwordEncrypter;
            _toastNotification = toastNotification;
            _logController = logController;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        [BindProperty]
        public IisServer cIisServer { get; set; }

        public List<SelectListItem> BooleanOptions = StaticDetails.ServiceOptions;

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(Id))
            {
                _toastNotification.AddErrorToastMessage("Id is null");
                return RedirectToPage("/IIS_Servers/Index");
            }

            cIisServer = _unitOfWork.IisServer.GetFirstOrDefault(x => x.Id == Id);
            if (cIisServer == null)
            {
                _toastNotification.AddErrorToastMessage("Object is null");
                return RedirectToPage("/IIS_Servers/Index");
            }

            return Page();
        }

        public IActionResult OnPost(IisServer cIisServer)
        {
            if (cIisServer.Password != null)
            {
                cIisServer.PasswordHash = _passwordEncrypter.Encrypt(cIisServer.Password, cIisServer.Id);
            }

            ModelState.Remove("cIisServer.Password");
            ModelState.Remove("cIisServer.PasswordHash");
            ModelState.Remove("cIisServer.AppPools");
            ModelState.Remove("cIisServer.ErrorMessage");

            if (ModelState.IsValid)
            {
                _unitOfWork.IisServer.Update(cIisServer);
                _unitOfWork.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Successfully edited IIS-Server");
                _logController.Log($"edited Iis-Server with id {cIisServer.Id}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToPage("/IIS_Servers/Index");
            }

            return Page();
        }
    }
}
