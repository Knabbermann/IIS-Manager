using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using IIS_Manager.Controllers.Log;
using NToastNotify;
using System.Security.Claims;

namespace IIS_Manager.Web.Areas.Admin.Pages.Users
{
    [Authorize(Roles = StaticDetails.RoleAdmin)]
    public class CreateModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<WebUser> _userStore;
        private readonly IUserEmailStore<WebUser> _emailStore;
        private readonly IToastNotification _toastNotification;
        private readonly ILogController _logController;

        public CreateModel(RoleManager<IdentityRole> roleManager,
            UserManager<WebUser> userManager,
            IUserStore<WebUser> userStore,
            IToastNotification toastNotification,
            ILogController logController)
        {
            _roleManager = roleManager;
            UserManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _toastNotification = toastNotification;
            _logController = logController;
        }

        public UserManager<WebUser> UserManager { get; }

        public List<SelectListItem> Roles { get; set; }
        [BindProperty]
        public string Role { get; set; }

        [BindProperty]
        public WebUser cUser { get; set; }

        [BindProperty]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public void OnGet()
        {
            Roles = _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync(WebUser cUser)
        {
            if (ModelState.IsValid)
            {
                var user = Activator.CreateInstance<WebUser>();
                await _userStore.SetUserNameAsync(user, cUser.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, cUser.Email, CancellationToken.None);
                var result = await UserManager.CreateAsync(user, Password);

                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user, Role);
                    _toastNotification.AddSuccessToastMessage("Successfully created user.");
                    _logController.Log($"created new user with id {user.Id}", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    return RedirectToPage("/Users/Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private IUserEmailStore<WebUser> GetEmailStore()
        {
            if (!UserManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<WebUser>)_userStore;
        }
    }
}
