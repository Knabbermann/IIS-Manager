using IIS_Manager.DataAccess.DbContext;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;

namespace IIS_Manager.DataAccess.Repository
{
    public class IisServerRepository : Repository<IisServer>, IIisServerRepository
    {
        private readonly WebDbContext _applicationDbContext;

        public IisServerRepository(WebDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Update(IisServer iisServer)
        {
            _applicationDbContext.Update(iisServer);
        }
    }
}
