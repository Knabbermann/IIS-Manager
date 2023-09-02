using System.Data;

namespace IIS_Manager.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IWebUserRepository WebUser { get; }
        IIisServerRepository IisServer { get; }
        IFavoriteRepository Favorite { get; }
        ILogRepository Log { get; }
        void SaveChanges();
        IDbConnection GetDbConnection();
    }
}
