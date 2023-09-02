using IIS_Manager.DataAccess.DbContext;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;

namespace IIS_Manager.DataAccess.Repository
{
    public class WebUserRepository : Repository<WebUser>, IWebUserRepository
    {
        private readonly WebDbContext _applicationDbContext;

        public WebUserRepository(WebDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Update(WebUser webUser)
        {
            _applicationDbContext.Update(webUser);
        }
    }
}
