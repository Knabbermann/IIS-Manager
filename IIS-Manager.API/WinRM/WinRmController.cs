using IIS_Manager.DataAccess.Repository.IRepository;
using IIS_Manager.Models;
using IIS_Manager.Utility;
using System.Collections;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Security;

namespace IIS_Manager.Controllers.WinRM
{
    public class WinRmController
    {
        private readonly string _serverName;
        private readonly PSCredential _credential;

        private Runspace _runspace;
        private PowerShell _powerShell;

        public WinRmController(string configId,
            IUnitOfWork unitOfWork,
            PasswordEncrypter passwordEncrypter)
        {
            var cIisServer = unitOfWork.IisServer.GetFirstOrDefault(x => x.Id == configId);
            if (cIisServer == null) throw new ItemNotFoundException(configId);
            _serverName = cIisServer.Address;
            var password = new SecureString();
            foreach (var c in passwordEncrypter.Decrypt(cIisServer.PasswordHash, cIisServer.Id))
            {
                password.AppendChar(c);
            }

            _credential = new PSCredential(cIisServer.Username, password);
            ClearMemory(password);
        }

        public string[] HealthCheck()
        {
            var timer = new Stopwatch();

            var connectionInfo = new WSManConnectionInfo
            {
                ComputerName = _serverName,
                Credential = _credential
            };

            timer.Start();
            try
            {
                var runspace = RunspaceFactory.CreateRunspace(connectionInfo);
                runspace.Open();
                runspace.Dispose();
                timer.Stop();

                return new[] { "success", timer.ElapsedMilliseconds.ToString() };
            }
            catch (Exception ex)
            {
                return new[] {"error",ex.Message};
            }
        }

        public List<AppPool> GetAllAppPools()
        {
            var connectionInfo = new WSManConnectionInfo
            {
                ComputerName = _serverName,
                Credential = _credential
            };

            var runspace = RunspaceFactory.CreateRunspace(connectionInfo);
            runspace.Open();

            using (var powerShell = PowerShell.Create())
            {
                powerShell.Runspace = runspace;
                powerShell.AddScript(StaticDetails.ScriptGetAllApplicationPools);

                var result = powerShell.Invoke();
                runspace.Dispose();

                if (powerShell.Streams.Error.Count > 0)
                {
                    var errorMessage = string.Join(Environment.NewLine,
                        powerShell.Streams.Error.Select(err => err.Exception.Message));
                    throw new Exception(errorMessage);
                }

                var appPools = new List<AppPool>();
                foreach (var psObject in result)
                {
                    var appPool = new AppPool
                    {
                        Name = psObject.Properties["Name"]?.Value?.ToString() ?? "Unknown",
                        State = psObject.Properties["State"]?.Value?.ToString() ?? "Unknown",
                        RuntimeVersion = psObject.Properties["ManagedRuntimeVersion"]?.Value?.ToString() ?? "Unknown"
                    };
                    var applicationsList = psObject.Properties["Applications"].Value as PSObject;
                    if (applicationsList?.BaseObject is ArrayList appObjects)
                    {
                        appPool.Applications = appObjects.Cast<object>().Select(item => item.ToString()).ToList();
                    }
                    appPools.Add(appPool);
                }

                return appPools;
            }
            
        }

        public string[] ExecuteCommand(string command, List<KeyValuePair<string, object>>? parameters = null)
        {
            var connectionInfo = new WSManConnectionInfo
            {
                ComputerName = _serverName,
                Credential = _credential
            };

            try
            {
                using (var runspace = RunspaceFactory.CreateRunspace(connectionInfo))
                {
                    runspace.Open();

                    using (var powerShell = PowerShell.Create())
                    {
                        powerShell.Runspace = runspace;
                        powerShell.AddCommand(command);
                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                powerShell.AddParameter(parameter.Key, parameter.Value);
                            }
                        }

                        var result = powerShell.Invoke();
                        runspace.Dispose();

                        if (powerShell.Streams.Error.Count > 0)
                        {
                            var errorMessage = string.Join(Environment.NewLine,
                                powerShell.Streams.Error.Select(err => err.Exception.Message));
                            return new[] { "error", errorMessage };
                        }

                        var output = result.Select(p => p.ToString()).ToList();
                        return new[] { "success", string.Join(Environment.NewLine, output) };
                    }
                }
            }
            catch (Exception ex)
            {
                return new[] { "error", ex.Message };
            }
        }

        private static void ClearMemory(SecureString objectToClear)
        {
            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(objectToClear);
                var length = objectToClear.Length;

                for (var i = 0; i < length; i++)
                    Marshal.WriteInt16(unmanagedString, i * 2, 0);
                
                var randomBytes = new byte[length * 2];
                new Random().NextBytes(randomBytes);
                Marshal.Copy(randomBytes, 0, unmanagedString, randomBytes.Length);
            }
            finally
            {
                if (unmanagedString != IntPtr.Zero)
                    Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
