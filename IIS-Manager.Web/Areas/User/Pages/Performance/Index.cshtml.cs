using IIS_Manager.Controllers.WinRM;
using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IIS_Manager.Web.Areas.User.Pages.Performance
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordEncrypter _passwordEncrypter;

        public IndexModel(IUnitOfWork unitOfWork,
            PasswordEncrypter passwordEncrypter)
        {
            _unitOfWork = unitOfWork;
            _passwordEncrypter = passwordEncrypter;
        }

        public IEnumerable<IisServer> IisServers { get; set; }

        public void OnGet()
        {
            IisServers = _unitOfWork.IisServer.GetAll();

            foreach (var iisServer in IisServers)
            {
                var cWinRmController = new WinRmController(iisServer.Id, _unitOfWork, _passwordEncrypter);
                var result = cWinRmController.ExecuteCommand("Get-WmiObject", new List<KeyValuePair<string, object>>
                { new("Class", "Win32_Processor"), new("Select-Object", "") });
            }
        }
    }
}
