using IIS_Manager.Controllers.Favorite;
using IIS_Manager.Controllers.WinRM;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Security.Claims;

namespace IIS_Manager.Web.Areas.User.Pages.Favorites
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
                    var appPools = new WinRmController(cIisServerId, _unitOfWork, _passwordEncrypter).GetAllAppPools();
                    favorite.AssetServer = _unitOfWork.IisServer.GetSingleOrDefault(x => x.Id.Equals(cIisServerId))?.Name;
                    favorite.AssetName = favorite.AssetId.Split("_")[1];
                    favorite.AssetState = appPools.SingleOrDefault(x => x.Name.Equals(favorite.AssetName))?.State;
                }
            }
        }

        public IActionResult OnPostUp(int displayOrder)
        {
            if (displayOrder == 1) _toastNotification.AddErrorToastMessage("Object ist already on top.");
            else
            {
                var cFavorite = _unitOfWork.Favorite.GetFirstOrDefault(x => x.DisplayOrder == displayOrder);
                var cFavoriteUp = _unitOfWork.Favorite.GetFirstOrDefault(x => x.DisplayOrder == displayOrder - 1);
                if (cFavorite == null || cFavoriteUp == null)
                    _toastNotification.AddErrorToastMessage("Object is null.");
                else
                {
                    cFavorite.DisplayOrder = displayOrder - 1;
                    cFavoriteUp.DisplayOrder = displayOrder;
                    _unitOfWork.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Successfully changed order.");
                }

            }
            return RedirectToPage("Index");
        }

        public IActionResult OnPostDown(int displayOrder)
        {
            var lastDisplayOrder = _unitOfWork.Favorite.GetAll().MaxBy(x => x.DisplayOrder)?.DisplayOrder;
            if (displayOrder == lastDisplayOrder) _toastNotification.AddErrorToastMessage("Object ist already on bottom.");
            else
            {
                var cFavorite = _unitOfWork.Favorite.GetFirstOrDefault(x => x.DisplayOrder == displayOrder);
                var cFavoriteDown = _unitOfWork.Favorite.GetFirstOrDefault(x => x.DisplayOrder == displayOrder + 1);
                if (cFavorite == null || cFavoriteDown == null)
                    _toastNotification.AddErrorToastMessage("Object is null.");
                else
                {
                    cFavorite.DisplayOrder = displayOrder + 1;
                    cFavoriteDown.DisplayOrder = displayOrder;
                    _unitOfWork.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Successfully changed order.");
                }

            }
            return RedirectToPage("Index");
        }

        public IActionResult OnPostRemove(int id)
        {
            var cFavorite = _unitOfWork.Favorite.GetFirstOrDefault(x => x.Id == id);

            if (cFavorite == null) _toastNotification.AddErrorToastMessage("Object is null");
            else
            {
                _unitOfWork.Favorite.Remove(cFavorite);
                _unitOfWork.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Successfully removed favorite.");
            }

            return RedirectToPage("Index");
        }
    }
}
