using IIS_Manager.Models;

namespace IIS_Manager.DataAccess.Repository.IRepository
{
    public interface IFavoriteRepository : IRepository<Favorite>
    {
        void Update(Favorite favorite);
    }
}
