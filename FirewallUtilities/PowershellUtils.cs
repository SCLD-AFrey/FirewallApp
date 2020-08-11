using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Threading.Tasks;

namespace FirewallUtilities
{
    internal class PowershellUtilities
    {
        internal static async Task<(PSDataCollection<PSObject> psObjs, bool IsSuccess)> RunScript(string scriptText)
        {
            try
            {
                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddScript(scriptText);
                    Debug.WriteLine(" Running Powershell: " + scriptText);
                    var pipelineObjects = await ps.InvokeAsync().ConfigureAwait(false);
                    return (pipelineObjects, true);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("RunScript FAILED");
                Debug.WriteLine("ERROR: " + e.Message);
                return (new PSDataCollection<PSObject>(), false);
            }
        }
    }
}
