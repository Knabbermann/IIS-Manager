using IIS_Manager.Models;

namespace IIS_Manager.DataAccess.Repository.IRepository
{
    public interface IIisServerRepository : IRepository<IisServer>
    {
        void Update(IisServer iisServer);
    }
}
