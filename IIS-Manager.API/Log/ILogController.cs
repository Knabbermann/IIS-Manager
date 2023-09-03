using IIS_Manager.Utility;

namespace IIS_Manager.Controllers.Log
{
    public interface ILogController
    {
        public List<Models.Log> GetAllLogs();
        public void Log(string message, string userId, string? type = StaticDetails.LogTypeInformation);
    }
}
