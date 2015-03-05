using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace genymotion_infra.Infra
{
    public class ProcessExecutor
    {
        private string stdout;
        private string stderror;
        private Process process;

        public void handleCommand(ProcessCommand command, bool blocking = true)
        {
            if (command == null)
            {
                return;
            }
            process = new System.Diagnostics.Process();
            process.StartInfo.FileName = command.FileName;
            process.StartInfo.Arguments = command.Arguments;
            process.EnableRaisingEvents = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            Thread.Sleep(500);
            if (blocking)
            {
                stdout = process.StandardOutput.ReadToEnd();
                StreamReader s = process.StandardError;
                stderror = s.ReadToEnd();
            }
            
            foreach (string error in command.Errors)
            {
                if (stdout.Contains(error) || stderror.Contains(error))
                {
                    throw new Exception("Found error "+error+" in process output");
                }
            }
            if (command.Must != null && stdout != null && !stdout.Contains(command.Must))
            {
                throw new Exception("Must " + command.Must + " was not found in process output");
            }

            

            if (blocking)
            {
                process.WaitForExit(20000);
            }
            
            
        }

        public Process Process
        {
            get { return this.process; }
        }

        public string Stdout
        {
            get { return this.stdout; }
        }

        public string Stderr
        {
            get { return this.stderror;  }
        }

    }

    public class ProcessCommand
    {
        private readonly string fileName;
        private readonly string arguments;
        private string must;
        private List<string> errors = new List<string>();

        public ProcessCommand(string fileName,string arguments){
            this.fileName = fileName;
            this.arguments = arguments;
        }

        public string FileName
        {
            get { return this.fileName; }

        }

        public string Arguments
        {
            get { return this.arguments; }
        }

        public List<string> Errors
        {
            get { return this.errors; }
        }

        public void AddError(string error)
        {
            errors.Add(error);
        }

        public string Must
        {
            set { this.must = value;  }
            get { return this.must; }
        }


    }
}
