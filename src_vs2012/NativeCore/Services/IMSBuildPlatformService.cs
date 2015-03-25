using System;
using System.Runtime.InteropServices;

namespace BlackBerry.NativeCore.Services
{
    [Guid("796f0eaf-cbd7-4593-ab67-cb767531f5b0")]
    public interface IMSBuildPlatformService
    {
        /// <summary>
        /// Requests installation of specified platform.
        /// </summary>
        int ScheduleInstallation(string name, Action<bool> completionHandler);

        /// <summary>
        /// Schedules removal of specified platform.
        /// </summary>
        int ScheduleRemoval(string name, Action<bool> completionHandler);
    }
}
