using IIS_Manager.DataAccess.Repository.IRepository;

namespace IIS_Manager.Controllers.Favorite
{
    public class FavoriteController : IFavoriteController
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavoriteController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool IsFavorite(string assetType, string assetId, string? userId = null)
        {
            Models.Favorite? cFavorite;

            if (userId != null)
            {
                cFavorite = _unitOfWork.Favorite.GetSingleOrDefault(x =>
                    x.AssetType.Equals(assetType) && x.AssetId.Equals(assetId) && x.WebUserId.Equals(userId));
            }
            else
            {
                cFavorite = _unitOfWork.Favorite.GetSingleOrDefault(x =>
                    x.AssetType.Equals(assetType) && x.AssetId.Equals(assetId) && x.IsPrivate == false);
            }

            return cFavorite is not null;
        }

        public List<Models.Favorite> GetFavorites(string assetType, string? userId = null)
        {
            return userId != null 
                ? _unitOfWork.Favorite.GetAll(x => x.WebUserId.Equals(userId)).ToList() 
                : _unitOfWork.Favorite.GetAll(x => x.IsPrivate == false).ToList();
        }

        public string[] AddFavorite(string assetType, string assetId, string? userId = null)
        {
            var lastDisplayOrder = _unitOfWork.Favorite.GetAll().MaxBy(x => x.DisplayOrder)?.DisplayOrder;
            var cFavorite = new Models.Favorite
            {
                AssetType = assetType,
                AssetId = assetId,
                DisplayOrder = lastDisplayOrder + 1 ?? 1
            };
            if (userId != null)
            {
                cFavorite.IsPrivate = true;
                var cUser = _unitOfWork.WebUser.GetSingleOrDefault(x => x.Id  == userId);
                cFavorite.User = cUser;
                cFavorite.WebUserId = cUser.Id;
            }
            else
            {
                cFavorite.IsPrivate = false;
            }

            try
            {
                _unitOfWork.Favorite.Add(cFavorite);
                _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                return new[] { "error", e.Message };
            }

            return new[] { "success" };
        }

        public string[] RemoveFavorite(string assetType, string assetId, string? userId = null)
        {
            if (!IsFavorite(assetType, assetId, userId)) 
                return new[] { "info", "asset is not favorized." };

            Models.Favorite? cFavorite;
            if (userId != null)
            {
                cFavorite = _unitOfWork.Favorite.GetSingleOrDefault(x =>
                    x.AssetType.Equals(assetType) && x.AssetId.Equals(assetId) && x.WebUserId.Equals(userId));
            }
            else
            {
                cFavorite = _unitOfWork.Favorite.GetSingleOrDefault(x =>
                    x.AssetType.Equals(assetType) && x.AssetId.Equals(assetId) && x.IsPrivate == false);
            }

            if(cFavorite == null) 
                return new[] { "info", "asset is not favorized." };

            try
            {
                _unitOfWork.Favorite.Remove(cFavorite);
                _unitOfWork.SaveChanges();
                return new[] { "success" };
            }
            catch (Exception e)
            {
                return new[] { "error", e.Message };
            }
        }
    }
}
