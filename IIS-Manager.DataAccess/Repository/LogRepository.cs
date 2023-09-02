using IIS_Manager.DataAccess.DbContext;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;

namespace IIS_Manager.DataAccess.Repository
{
    public class LogRepository : Repository<Log>, ILogRepository
    {
        private readonly WebDbContext _applicationDbContext;

        public LogRepository(WebDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Update(Log log)
        {
            _applicationDbContext.Update(log);
        }
    }
}
