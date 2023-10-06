using IIS_Manager.Controllers.Log;
using IIS_Manager.Controllers.WinRM;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace IIS_Manager.Web.Areas.User.Pages.Performance
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordEncrypter _passwordEncrypter;
        private readonly IToastNotification _toastNotification;
        private readonly ILogController _logController;

        public IndexModel(IUnitOfWork unitOfWork,
            PasswordEncrypter passwordEncrypter,
            IToastNotification toastNotification,
            ILogController logController)
        {
            _unitOfWork = unitOfWork;
            _passwordEncrypter = passwordEncrypter;
            _toastNotification = toastNotification;
            _logController = logController;
        }

        public IEnumerable<IisServer> IisServers { get; set; }

        public void OnGet()
        {
            IisServers = _unitOfWork.IisServer.GetAll();

            foreach (var iisServer in IisServers)
            {
                var cWinRmController = new WinRmController(iisServer.Id, _unitOfWork, _passwordEncrypter);
                iisServer.HealthCheck = cWinRmController.HealthCheck();
                if (iisServer.HealthCheck[0].Equals("success"))
                    iisServer.ServerInfo = SetServerInfo(cWinRmController, iisServer);
                else iisServer.ErrorMessage = iisServer.HealthCheck[1];
            }
        }

        private ServerInfo? SetServerInfo(WinRmController cWinRmController, IisServer cIisServer)
        {
            string[] propertiesToQuery = { "Name", "NumberOfLogicalProcessors" };
            var serverInfo = new ServerInfo();

            foreach (var property in propertiesToQuery)
            {
                var result = cWinRmController.ExecuteScript($"(Get-WmiObject -Class Win32_Processor).{property}");
                if (result[0].Equals("error"))
                {
                    _toastNotification.AddErrorToastMessage($"Error while getting {property} for {cIisServer.Name}");
                    _logController.Log($"Error while getting {property} for {cIisServer.Name}", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), StaticDetails.LogTypeError);
                    return null;
                }

                if (property == "Name")
                {
                    serverInfo.ProcessorName = result[1].Trim();
                }
                else if (property == "NumberOfLogicalProcessors")
                {
                    serverInfo.ProcessorCores = int.Parse(result[1]);
                }
            }

            var memoryResult = cWinRmController.ExecuteScript("(Get-WmiObject -Class Win32_PhysicalMemory | Measure-Object -Property Capacity -Sum).Sum / 1GB");
            if (memoryResult[0].Equals("error"))
            {
                _toastNotification.AddErrorToastMessage($"Error while getting memory information for {cIisServer.Name}");
                _logController.Log($"Error while getting memory information for {cIisServer.Name}", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), StaticDetails.LogTypeError);
                return null;
            }

            serverInfo.MemorySize = int.Parse(memoryResult[1]);
            serverInfo.ServerId = cIisServer.Id;

            return serverInfo;
        }
    }
}
