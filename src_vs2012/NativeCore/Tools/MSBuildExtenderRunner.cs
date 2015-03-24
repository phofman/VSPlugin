using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BlackBerry.NativeCore.Tools
{
    /// <summary>
    /// Class that allows running a native tool as system administrator.
    /// </summary>
    public sealed class MSBuildExtenderRunner
    {
        private const int ERROR_CANCELLED = 0x4c7;

        public event EventHandler<ToolRunnerEventArgs> Finished;

        private readonly string _fileName;
        private readonly string _defaultConfigFilePath;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public MSBuildExtenderRunner(string fileName, string vsVersion, bool install)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            _fileName = fileName;
            VsVersion = vsVersion;
            PerformInstall = install;
            WorkingDirectory = Path.GetDirectoryName(_fileName);
            ConfigFilePath = _defaultConfigFilePath = Path.Combine(WorkingDirectory, "app.cfg");
        }

        #region Properties

        public IEventDispatcher Dispatcher
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the working directory for the tool.
        /// </summary>
        public string WorkingDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the full path to the config file.
        /// </summary>
        public string ConfigFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version of Visual Studio this tool should only affect.
        /// </summary>
        public string VsVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets indication, if installation or removal action is performed.
        /// </summary>
        public bool PerformInstall
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Execute the tool and don't wait for result as it will be scheduled for admin execution.
        /// </summary>
        public void ExecuteAsync()
        {
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.FileName = _fileName;
            startInfo.WorkingDirectory = WorkingDirectory;
            startInfo.Arguments = GetArguments();
            startInfo.Verb = "runas";

            try
            {
                var process = Process.Start(startInfo);

                if (process != null)
                {
                    process.EnableRaisingEvents = true;
                    process.Exited += OnProcessExited;
                }
            }
            catch (Win32Exception ex)
            {
                // was it cancelled by user, when asking for admin permissions?
                if (ex.NativeErrorCode != ERROR_CANCELLED)
                    throw;

                NotifyFinished(false, int.MinValue);
            }
        }

        private string GetArguments()
        {
            var result = new StringBuilder();

            // indicate the action to perform:
            if (PerformInstall)
            {
                result.Append("/i");
            }
            else
            {
                result.Append("/u");
            }

            // enforce the config file location next to the executable:
            if (!string.IsNullOrEmpty(ConfigFilePath) && File.Exists(ConfigFilePath) && string.Compare(_defaultConfigFilePath, ConfigFilePath, StringComparison.OrdinalIgnoreCase) != 0)
            {
                result.Append(" /config:\"").Append(ConfigFilePath).Append("\"");
            }

            // enforce Visual Studio version:
            if (string.IsNullOrEmpty(VsVersion))
            {
                result.Append(" /all");
            }
            else
            {
                result.Append(" /").Append(VsVersion);
            }

            return result.ToString();
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            var process = sender as Process;
            if (process != null)
            {
                NotifyFinished(true, process.ExitCode);
            }
        }

        private void NotifyFinished(bool executed, int exitCode)
        {
            var finishedHandler = Finished;
            var dispatcher = Dispatcher;

            if (finishedHandler != null)
            {
                // perform a cross-thread notification (in case we want to update UI directly from the handler)
                if (dispatcher != null)
                {
                    dispatcher.Invoke(finishedHandler, this, new ToolRunnerEventArgs(exitCode, null, null, executed));
                }
                else
                {
                    finishedHandler(this, new ToolRunnerEventArgs(exitCode, null, null, executed));
                }
            }
        }
    }
}
