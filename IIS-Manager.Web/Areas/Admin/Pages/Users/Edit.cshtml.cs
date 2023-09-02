using IIS_Manager.Controllers.Log;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using NToastNotify;
using System.Security.Claims;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace IIS_Manager.Web.Areas.Admin.Pages.Users
{
    [Authorize(Roles = StaticDetails.RoleAdmin)]
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<WebUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IToastNotification _toastNotification;
        private readonly ILogController _logController;

        public EditModel(IUnitOfWork unitOfWork,
            UserManager<WebUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IToastNotification toastNotification,
            ILogController logController)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _toastNotification = toastNotification;
            _logController = logController;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public WebUser User { get; set; }
        public string UserRole { get; set; }

        public List<SelectListItem> Roles { get; set; }
        [BindProperty]
        public string Role { get; set; }
        [BindProperty]
        public string Email { get; set; }

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

            Roles = _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                _toastNotification.AddErrorToastMessage("Id is null");
                return RedirectToPage("/Users/Index");
            }

            User = _unitOfWork.WebUser.GetFirstOrDefault(x => x.Id == Id);

            if (User == null)
            {
                _toastNotification.AddErrorToastMessage("Object is null.");
                return RedirectToPage("/Users/Index");
            }

            UserRole = _userManager.GetRolesAsync(User).Result.FirstOrDefault();

            if (ModelState.IsValid)
            {
                await _userManager.SetEmailAsync(User, Email);
                await _userManager.SetUserNameAsync(User, Email);

                await _userManager.RemoveFromRoleAsync(User, UserRole);
                await _userManager.AddToRoleAsync(User, Role);
                _toastNotification.AddSuccessToastMessage("Successfully edited user");
                _logController.Log($"edited user with id {User.Id}", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToPage("/Users/Index");
            }

            return Page();
        }
    }
}
