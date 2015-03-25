using BlackBerry.NativeCore.Services;

namespace BlackBerry.Package.ViewModels
{
    internal sealed class MSBuildOptionViewModel
    {
        private IMSBuildPlatformService GetMSBuildService()
        {
            return Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(IMSBuildPlatformService)) as IMSBuildPlatformService;
        }

        public void InstallMSBuildPlatform()
        {
            var service = GetMSBuildService();
            if (service != null)
            {
                service.ScheduleInstallation("BlackBerry", null);
            }
        }

        public void RemoveMSBuildPlatform()
        {
            var service = GetMSBuildService();
            if (service != null)
            {
                service.ScheduleRemoval("BlackBerry", null);
            }
        }
    }
}
