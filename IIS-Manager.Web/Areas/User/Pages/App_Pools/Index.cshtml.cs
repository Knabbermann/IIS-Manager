using IIS_Manager.Controllers.Favorite;
using IIS_Manager.Controllers.Log;
using IIS_Manager.Controllers.WinRM;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Security.Claims;

namespace IIS_Manager.Web.Areas.User.Pages.App_Pools
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordEncrypter _passwordEncrypter;
        private readonly IToastNotification _toastNotification;
        private readonly IFavoriteController _favoriteController;
        private readonly ILogController _logController;

        public IndexModel(IUnitOfWork unitOfWork,
            PasswordEncrypter passwordEncrypter,
            IToastNotification toastNotification,
            IFavoriteController favoriteController,
            ILogController logController)
        {
            _unitOfWork = unitOfWork;
            _passwordEncrypter = passwordEncrypter;
            _toastNotification = toastNotification;
            _favoriteController = favoriteController;
            _logController = logController;
        }

        [BindProperty(SupportsGet = true)] 
        public IEnumerable<IisServer> IisServers { get; set; }

        public void OnGet()
        {
            IisServers = _unitOfWork.IisServer.GetAll();
            /*foreach (var iisServer in IisServers)
            {
                var cWinRmController = new WinRmController(iisServer.Id, _unitOfWork, _passwordEncrypter);
                var cHealthCheck = cWinRmController.HealthCheck();
                if (cHealthCheck[0].Equals("success"))
                {
                    iisServer.AppPools = cWinRmController.GetAllAppPools();
                    foreach (var appPool in iisServer.AppPools)
                    {
                        var isFavorite = _favoriteController.IsFavorite(StaticDetails.AssetTypeAppPool,
                            iisServer.Id + "_" + appPool.Name, HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                        var cIcon = isFavorite ? "bi-star-fill" : "bi-star";
                        TempData[$"{iisServer.Id}_{appPool.Name}_icon"] = cIcon;
                    }
                }
                else
                {
                    iisServer.AppPools = new List<AppPool>();
                }
                iisServer.ErrorMessage = cHealthCheck[1];
            }*/
            foreach (var iisServer in IisServers)
            {
                iisServer.AppPools = new List<AppPool> { new () };
            }
        }

        public JsonResult OnGetAppPools()
        {
            IisServers = _unitOfWork.IisServer.GetAll();
            foreach (var iisServer in IisServers)
            {
                var cWinRmController = new WinRmController(iisServer.Id, _unitOfWork, _passwordEncrypter);
                var cHealthCheck = cWinRmController.HealthCheck();
                if (cHealthCheck[0].Equals("success"))
                {
                    iisServer.AppPools = cWinRmController.GetAllAppPools();
                    foreach (var appPool in iisServer.AppPools)
                    {
                        appPool.IsFavorite = _favoriteController.IsFavorite(StaticDetails.AssetTypeAppPool,
                            iisServer.Id + "_" + appPool.Name, HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    }
                }
                else
                {
                    iisServer.AppPools = new List<AppPool>();
                }
                iisServer.ErrorMessage = cHealthCheck[1];
            }

            return new JsonResult(IisServers);
        }
        
        public IActionResult OnPostStart(string id, string selected, string? redirect = null)
        {
            if (selected.Contains(','))
            {
                var cAppPools = selected.Split(',');
                foreach (var appPool in cAppPools)
                {
                    var cWinRmController = new WinRmController(id, _unitOfWork, _passwordEncrypter);
                    var result = cWinRmController.ExecuteCommand("Start-WebAppPool", new List<KeyValuePair<string, object>>
                        { new("Name", $"{appPool}") });
                    if (result[0].Equals("success"))
                    {
                        _toastNotification.AddSuccessToastMessage($"{appPool} started successfully");
                        _logController.Log($"successfully started AppPools:'{appPool}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage($"error while starting {appPool}: {result[1]}"); 
                        _logController.Log($"error while starting AppPools:'{appPool}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), StaticDetails.LogTypeError);
                    }
                }
            }
            else
            {
                var cWinRmController = new WinRmController(id, _unitOfWork, _passwordEncrypter);
                var result = cWinRmController.ExecuteCommand("Start-WebAppPool", new List<KeyValuePair<string, object>>
                    { new("Name", $"{selected}") });
                if (result[0].Equals("success"))
                {
                    _toastNotification.AddSuccessToastMessage($"{selected} started successfully");
                    _logController.Log($"successfully started AppPool:'{selected}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                }
                else
                {
                    _toastNotification.AddErrorToastMessage($"error while starting {selected}: {result[1]}");
                    _logController.Log($"error while starting AppPool:'{selected}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), StaticDetails.LogTypeError);
                }
            }
            
            OnGet();
            if (redirect != null) return Redirect(redirect);
            return RedirectToPage("Index");
        }

        public IActionResult OnPostStop(string id, string selected, string? redirect = null)
        {
            if (selected.Contains(','))
            {
                var cAppPools = selected.Split(',');
                foreach (var appPool in cAppPools)
                {
                    var cWinRmController = new WinRmController(id, _unitOfWork, _passwordEncrypter);
                    var result = cWinRmController.ExecuteCommand("Stop-WebAppPool", new List<KeyValuePair<string, object>>
                        { new("Name", $"{appPool}") });
                    if (result[0].Equals("success"))
                    {
                        _toastNotification.AddSuccessToastMessage($"{appPool} stopped successfully");
                        _logController.Log($"successfully stopped AppPools:'{appPool}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage($"error while stopping {appPool}: {result[1]}");
                        _logController.Log($"error while stopping AppPools:'{appPool}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), StaticDetails.LogTypeError);
                    }
                }
            }
            else
            {
                var cWinRmController = new WinRmController(id, _unitOfWork, _passwordEncrypter);
                var result = cWinRmController.ExecuteCommand("Stop-WebAppPool", new List<KeyValuePair<string, object>>
                    { new("Name", $"{selected}") });
                if (result[0].Equals("success"))
                {
                    _toastNotification.AddSuccessToastMessage($"{selected} stopped successfully");
                    _logController.Log($"successfully stopped AppPool:'{selected}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                }
                else
                {
                    _toastNotification.AddErrorToastMessage($"error while stopping {selected}: {result[1]}");
                    _logController.Log($"error while stopping AppPool:'{selected}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), StaticDetails.LogTypeError);
                }
            }

            OnGet();
            if (redirect != null) return Redirect(redirect);
            return RedirectToPage("Index");
        }

        public IActionResult OnPostRestart(string id, string selected, string? redirect = null)
        {
            if (selected.Contains(','))
            {
                var cAppPools = selected.Split(',');
                foreach (var appPool in cAppPools)
                {
                    var cWinRmController = new WinRmController(id, _unitOfWork, _passwordEncrypter);
                    var result = cWinRmController.ExecuteCommand("Restart-WebAppPool", new List<KeyValuePair<string, object>>
                        { new("Name", $"{appPool}") });
                    if (result[0].Equals("success"))
                    {
                        _toastNotification.AddSuccessToastMessage($"{appPool} restarted successfully");
                        _logController.Log($"successfully restarted AppPools:'{appPool}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage($"error while restarting {appPool}: {result[1]}");
                        _logController.Log($"error while restarting AppPools:'{appPool}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), StaticDetails.LogTypeError);
                    }
                }
            }
            else
            {
                var cWinRmController = new WinRmController(id, _unitOfWork, _passwordEncrypter);
                var result = cWinRmController.ExecuteCommand("Restart-WebAppPool", new List<KeyValuePair<string, object>>
                    { new("Name", $"{selected}") });
                if (result[0].Equals("success"))
                {
                    _toastNotification.AddSuccessToastMessage($"{selected} restarted successfully");
                    _logController.Log($"successfully stopped AppPool:'{selected}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                }
                else
                {
                    _toastNotification.AddErrorToastMessage($"error while restarting {selected}: {result[1]}");
                    _logController.Log($"error while restarting AppPool:'{selected}'", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), StaticDetails.LogTypeError);
                }
            }

            OnGet();
            if (redirect != null) return Redirect(redirect);
            return RedirectToPage("Index");
        }

        public IActionResult OnPostSetFavorite(string id, string selected)
        {
            var cAssetType = StaticDetails.AssetTypeAppPool;
            var cAssetId = id + "_" + selected;
            var cUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            _ = _favoriteController.IsFavorite(cAssetType, cAssetId, cUserId) 
                ? _favoriteController.RemoveFavorite(cAssetType, cAssetId, cUserId) 
                : _favoriteController.AddFavorite(cAssetType, cAssetId, cUserId);

            OnGet();
            return RedirectToPage("Index");
        }
    }
}
