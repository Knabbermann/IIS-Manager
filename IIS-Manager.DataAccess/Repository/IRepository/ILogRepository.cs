using IIS_Manager.Models;

namespace IIS_Manager.DataAccess.Repository.IRepository
{
    public interface ILogRepository : IRepository<Log>
    {
        void Update(Log log);
    }
}
