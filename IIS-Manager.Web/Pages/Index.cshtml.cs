using IIS_Manager.Controllers.Favorite;
using IIS_Manager.Controllers.WinRM;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using NToastNotify;

namespace IIS_Manager.Web.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordEncrypter _passwordEncrypter;
        private readonly IFavoriteController _favoriteController;
        private readonly IToastNotification _toastNotification;

        public IndexModel(IUnitOfWork unitOfWork,
            PasswordEncrypter passwordEncrypter,
            IFavoriteController favoriteController,
            IToastNotification toastNotification)
        {
            _unitOfWork = unitOfWork;
            _passwordEncrypter = passwordEncrypter;
            _favoriteController = favoriteController;
            _toastNotification = toastNotification;
        }

        [BindProperty(SupportsGet = true)]
        public IEnumerable<Favorite> Favorites { get; set; }

        public void OnGet()
        {
            var cUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Favorites = _favoriteController.GetFavorites(StaticDetails.AssetTypeAppPool, cUserId).OrderBy(x => x.DisplayOrder);

            foreach (var favorite in Favorites)
            {
                if (favorite.AssetType.Equals(StaticDetails.AssetTypeAppPool))
                {
                    var cIisServerId = favorite.AssetId.Split("_")[0];
                    favorite.AssetServer = _unitOfWork.IisServer.GetSingleOrDefault(x => x.Id.Equals(cIisServerId))?.Name;
                    favorite.AssetName = favorite.AssetId.Split("_")[1];
                }
            }
        }

        public JsonResult OnGetFavorites()
        {
            var cUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Favorites = _favoriteController.GetFavorites(StaticDetails.AssetTypeAppPool, cUserId).OrderBy(x => x.DisplayOrder).ToList();

            var offlineServers = new List<string>();
            foreach (var favorite in Favorites)
            {
                if (favorite.AssetType.Equals(StaticDetails.AssetTypeAppPool))
                {
                    var cIisServerId = favorite.AssetId.Split("_")[0];
                    favorite.AssetName = favorite.AssetId.Split("_")[1];
                    try
                    {
                        if (!offlineServers.Contains(cIisServerId))
                        {
                            var appPools =
                                new WinRmController(cIisServerId, _unitOfWork, _passwordEncrypter).GetAllAppPools();
                            favorite.AssetState = appPools.SingleOrDefault(x => x.Name.Equals(favorite.AssetName))?.State;
                        }
                    }
                    catch (Exception e)
                    {
                        offlineServers.Add(cIisServerId);
                        _toastNotification.AddErrorToastMessage(e.Message);
                    }
                    favorite.AssetServer = _unitOfWork.IisServer.GetSingleOrDefault(x => x.Id.Equals(cIisServerId))?.Name;
                }
            }

            return new JsonResult(Favorites);
        }
    }
}