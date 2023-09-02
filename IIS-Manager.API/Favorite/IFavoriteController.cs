namespace IIS_Manager.Controllers.Favorite
{
    public interface IFavoriteController
    {
        public bool IsFavorite(string assetType, string assetId, string? userId = null);

        public List<Models.Favorite> GetFavorites(string assetType, string? userId = null);

        public string[] AddFavorite(string assetType, string assetId, string? userId = null);

        public string[] RemoveFavorite(string assetType, string assetId, string? userId = null);
    }
}
