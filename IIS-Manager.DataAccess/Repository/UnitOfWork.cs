using IIS_Manager.DataAccess.DbContext;
using IIS_Manager.DataAccess.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace IIS_Manager.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WebDbContext _webDbContext;
        private readonly IConfiguration _configuration;

        public UnitOfWork(WebDbContext webDbContext, IConfiguration configuration)
        {
            _webDbContext = webDbContext;
            _configuration = configuration;
            WebUser = new WebUserRepository(_webDbContext);
            IisServer = new IisServerRepository(_webDbContext);
            Favorite = new FavoriteRepository(_webDbContext);
            Log = new LogRepository(_webDbContext);
        }

        public IWebUserRepository WebUser { get; }
        public IIisServerRepository IisServer { get; }
        public IFavoriteRepository Favorite { get; }
        public ILogRepository Log { get; }

        public void SaveChanges()
        {
            _webDbContext.SaveChanges();
        }

        public IDbConnection GetDbConnection()
        {
            var connectionString = _configuration.GetConnectionString("WebDbContextConnection");
            return new SqlConnection(connectionString);
        }
    }
}
