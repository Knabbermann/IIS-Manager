using Microsoft.AspNetCore.Mvc.Rendering;

namespace IIS_Manager.Utility
{
    public class StaticDetails
    {
        //Roles
        public const string RoleAdmin = "Admin";
        public const string RoleUser = "User";

        //AssetTypes
        public const string AssetTypeAppPool = "AppPool";
        public const string AssetTypeIisServer = "IisServer";

        //LogTypes
        public const string LogTypeDebug = "Debug";
        public const string LogTypeInformation = "Information";
        public const string LogTypeWarning = "Warning";
        public const string LogTypeError = "Error";
        public const string LogTypeFatal = "Fatal";

        //ServiceOptions
        public static List<SelectListItem> ServiceOptions = new()
        {
            //new SelectListItem { Text = "SSH", Value = "SSH" },
            new SelectListItem { Text = "WSMan (HTTP)", Value = "WSMan (HTTP)" },
            //new SelectListItem { Text = "WSMan (HTTPS)", Value = "WSMan (HTTPS)" }
        };

        //Scripts
        public const string ScriptGetAllApplicationPools = @"
            try
            {
                Import-Module WebAdministration;
                $appPools = Get-ChildItem IIS:\AppPools;
                $appPoolsInfo = @();

                foreach ($appPool in $appPools)
                {
                    $name = $appPool.Name;
                    $state = $appPool.State;
                    $runtimeVersion = $appPool.ManagedRuntimeVersion;

                    $appPoolInfo = [PSCustomObject]@{
                        Name = $name;
                        State = $state;
                        ManagedRuntimeVersion = $runtimeVersion;
                        Applications = @()
                    };

                    $sites = Get-ChildItem IIS:\Sites;
                    foreach ($site in $sites)
                    {
                        if ($site.ApplicationPool -eq $name)
                        {
                            $appPoolInfo.Applications += $site.Name;
                        }
                    }

                    $appPoolsInfo += $appPoolInfo;
                }

                $appPoolsInfo
            }
            catch
            {
                Write-Error $_.Exception.Message
            }
        ";
    }
}
