using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Security.Claims;
using IIS_Manager.Controllers.Log;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace IIS_Manager.Web.Areas.Admin.Pages.Users
{
    [Authorize(Roles = StaticDetails.RoleAdmin)]
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<WebUser> _userManager;
        private readonly IToastNotification _toastNotification;
        private readonly ILogController _logController;

        public DeleteModel(IUnitOfWork unitOfWork,
            UserManager<WebUser> userManager,
            IToastNotification toastNotification,
            ILogController logController)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _toastNotification = toastNotification;
            _logController = logController;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public WebUser User { get; set; }
        public string UserRole { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(Id))
            {
                _toastNotification.AddErrorToastMessage("Id is null");
                return RedirectToPage("/Users/Index");
            }

            User = _unitOfWork.WebUser.GetFirstOrDefault(x => x.Id == Id);

            if (User == null)
            {
                _toastNotification.AddErrorToastMessage("Object is null");
                return RedirectToPage("/Users/Index");
            }

            UserRole = _userManager.GetRolesAsync(User).Result.FirstOrDefault();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Id))
            {
                _toastNotification.AddErrorToastMessage("Id is null");
                return RedirectToPage("/Users/Index");
            }

            User = _unitOfWork.WebUser.GetFirstOrDefault(x => x.Id == Id);

            if (User == null)
            {
                _toastNotification.AddErrorToastMessage("Object is null");
                return RedirectToPage("/Users/Index");
            }

            UserRole = _userManager.GetRolesAsync(User).Result.FirstOrDefault();
            var currentAdmins = _userManager.GetUsersInRoleAsync(StaticDetails.RoleAdmin).Result.Count;

            if (User.Id.Equals(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                ModelState.AddModelError(string.Empty, "Please delete your account via user settings");
            }
            else if (UserRole.Equals(StaticDetails.RoleAdmin) && currentAdmins < 2)
            {
                ModelState.AddModelError(string.Empty, "Can not delete last admin");
            }
            else if (ModelState.IsValid)
            {
                _unitOfWork.WebUser.Remove(User);
                _unitOfWork.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Successfully deleted user");
                _logController.Log($"deleted user with id {User.Id}", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToPage("/Users/Index");
            }

            return Page();
        }
    }
}
