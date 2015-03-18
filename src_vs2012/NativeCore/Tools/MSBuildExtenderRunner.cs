using System;
using System.Diagnostics;
using System.IO;

namespace BlackBerry.NativeCore.Tools
{
    /// <summary>
    /// Class that allows running a native tool as system administrator.
    /// </summary>
    public sealed class MSBuildExtenderRunner
    {
        private readonly string _fileName;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public MSBuildExtenderRunner(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            _fileName = fileName;
        }

        public void Execute()
        {
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.FileName = _fileName;
            startInfo.WorkingDirectory = Path.GetDirectoryName(_fileName);
            startInfo.Verb = "runas";

            Process.Start(startInfo);
        }
    }
}
