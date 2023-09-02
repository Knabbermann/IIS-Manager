using IIS_Manager.DataAccess.DbContext;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;

namespace IIS_Manager.DataAccess.Repository
{
    public class FavoriteRepository : Repository<Favorite>, IFavoriteRepository
    {
        private readonly WebDbContext _applicationDbContext;

        public FavoriteRepository(WebDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Update(Favorite favorite)
        {
            _applicationDbContext.Update(favorite);
        }
    }
}
