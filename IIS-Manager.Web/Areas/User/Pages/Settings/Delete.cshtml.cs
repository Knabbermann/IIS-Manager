using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace IIS_Manager.Web.Areas.User.Pages.Settings
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<WebUser> _userManager;
        private readonly SignInManager<WebUser> _signInManager;
        private readonly IToastNotification _toastNotification;

        public DeleteModel(IUnitOfWork unitOfWork,
            UserManager<WebUser> userManager,
            SignInManager<WebUser> signInManager,
            IToastNotification toastNotification)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _toastNotification = toastNotification;
        }

        public WebUser User { get; set; }
        public string UserRole { get; set; }

        public IActionResult OnGet()
        {
            var cUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (cUserId == null)
            {
                _toastNotification.AddErrorToastMessage("can not find NameIdentifier for current user");
                RedirectToPage("/Index");
            }

            User = _unitOfWork.WebUser.GetSingleOrDefault(x => x.Id.Equals(cUserId));
            if (User == null)
            {
                _toastNotification.AddErrorToastMessage("can not find current user in database");
                RedirectToPage("/Index");
            }

            UserRole = _userManager.GetRolesAsync(User).Result.FirstOrDefault();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var cUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (cUserId == null)
            {
                _toastNotification.AddErrorToastMessage("can not find NameIdentifier for current user");
                RedirectToPage("/Index");
            }

            User = _unitOfWork.WebUser.GetSingleOrDefault(x => x.Id.Equals(cUserId));
            if (User == null)
            {
                _toastNotification.AddErrorToastMessage("can not find current user in database");
                RedirectToPage("/Index");
            }

            UserRole = _userManager.GetRolesAsync(User).Result.FirstOrDefault();
            var currentAdmins = _userManager.GetUsersInRoleAsync(StaticDetails.RoleAdmin).Result.Count;

            if (UserRole.Equals(StaticDetails.RoleAdmin) && currentAdmins < 2)
            {
                ModelState.AddModelError(string.Empty, "Can not delete last admin");
            }
            else if (ModelState.IsValid)
            {
                _unitOfWork.WebUser.Remove(User);
                _unitOfWork.SaveChanges();
                await _signInManager.SignOutAsync();
                _toastNotification.AddSuccessToastMessage("Successfully deleted user");
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}
