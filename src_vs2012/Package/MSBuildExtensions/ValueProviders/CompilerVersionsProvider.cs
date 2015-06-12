#if PLATFORM_VS2010
using Microsoft.VisualStudio.Project.Contracts.PropertyPages.VS2010ONLY;
using Microsoft.VisualStudio.Project.Framework;
#elif PLATFORM_VS2012
using Microsoft.VisualStudio.Project.Designers.Properties;
using Microsoft.VisualStudio.Project.Utilities.PropertyPages;
#elif PLATFORM_VS2013 || PLATFORM_VS2015
using Microsoft.VisualStudio.ProjectSystem.Designers.Properties;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using Microsoft.VisualStudio.ProjectSystem.Utilities.PropertyPages;
using System.Threading.Tasks;
#endif
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Build.Framework.XamlTypes;

namespace BlackBerry.Package.MSBuildExtensions.ValueProviders
{
    /// <summary>
    /// Dynamic-enum provider class, that can be used inside MSBuild rules files (by category) and is automatically
    /// picked-up by Visual Studio to suggest values presented for developer in a combo-box.
    /// </summary>
    [Export(ExportContractNames.Scopes.UnconfiguredProject, typeof(IDynamicEnumValuesProvider))]
#if PLATFORM_VS2010
    [ProjectScope(ProjectScopeRequired.ConfiguredProject)]
#elif PLATFORM_VS2012 || PLATFORM_VS2013
    [DynamicEnumCategory("CompilerVersionSelector")]
#elif PLATFORM_VS2015
    [AppliesTo(ProjectCapabilities.AlwaysApplicable)]
    [ExportDynamicEnumValuesProvider("CompilerVersionSelector")]
#else
#error Define how to export the provider
#endif
    public sealed class CompilerVersionsProvider : IDynamicEnumValuesProvider
    {
#if PLATFORM_VS2010 || PLATFORM_VS2012
        public IDynamicEnumValuesGenerator GetProvider(IList<NameValuePair> options)
        {
            return new CompilerVersionsGenerator();
        }
#endif

#if PLATFORM_VS2013 || PLATFORM_VS2015
        public Task<IDynamicEnumValuesGenerator> GetProviderAsync(IList<NameValuePair> options)
        {
            return Task.FromResult<IDynamicEnumValuesGenerator>(new CompilerVersionsGenerator());
        }
#endif
    }
}
