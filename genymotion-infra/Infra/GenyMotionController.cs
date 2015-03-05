using genymotion_infra.Infra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webdriver_infra.Infra
{
    public class GenyMotionController
    {
        private readonly string genyMotionFolder;

        private readonly string virtualBoxFolder;

        private ProcessExecutor executer;

        private Process vmProcess;


        public GenyMotionController(string genyMotionFolder = @"C:\Program Files\Genymobile\Genymotion", string virtualBoxFolder = @"C:\Program Files\Oracle\VirtualBox")
        {
            this.genyMotionFolder = genyMotionFolder;
            this.virtualBoxFolder = virtualBoxFolder;
            executer = new ProcessExecutor();
        }

        public void StartVM(string vmname)
        {
            ProcessCommand process = new ProcessCommand(genyMotionFolder + @"\player.exe", "--vm-name \"" + vmname + "\"");
            executer.handleCommand(process, false);
            vmProcess = executer.Process;

        }

        public void StopVm(string clonedVm)
        {
            if (vmProcess != null && !vmProcess.HasExited)
            {
                vmProcess.Kill();
            }

            ProcessCommand command = new ProcessCommand("wmic.exe","process where name=\"VBoxHeadless.exe\" call terminate");
            executer.handleCommand(command);

        }

        public string CloneVm(string vmname, string newName)
        {
            ProcessCommand process = null;
            if (newName == null)
            {
                newName = vmname + " Clone";
            }
            process = new ProcessCommand(virtualBoxFolder + @"\VBoxManage.exe", "clonevm \"" + vmname + "\" --name " + newName);

            process.Must = "Machine has been successfully cloned as";
            executer.handleCommand(process);

            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            process = new ProcessCommand(virtualBoxFolder + @"\VBoxManage.exe", "registervm \"" + userFolder + "\\VirtualBox VMs\\" + newName + @"\" + newName + ".vbox\"");
            process.AddError("error");
            process.AddError("Cannot unregister the machine");
            executer.handleCommand(process);
            return newName;
        }


        public string CloneVm(string vmname)
        {
            return CloneVm(vmname, null);
        }

        public bool DeleteVm(string vmName)
        {
            ProcessCommand process = new ProcessCommand(virtualBoxFolder + @"\VBoxManage.exe", "unregistervm \"" + vmName + "\" --delete");
            executer.handleCommand(process);
            return !executer.Stderr.Contains("error");

        }


    }
}
