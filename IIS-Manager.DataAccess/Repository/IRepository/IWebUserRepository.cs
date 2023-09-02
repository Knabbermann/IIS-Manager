using IIS_Manager.Models;

namespace IIS_Manager.DataAccess.Repository.IRepository
{
    public interface IWebUserRepository : IRepository<WebUser>
    {
        void Update(WebUser applicationUser);
    }
}
