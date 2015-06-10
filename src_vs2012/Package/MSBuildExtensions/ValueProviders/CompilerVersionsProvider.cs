#if PLATFORM_VS2010
using Microsoft.VisualStudio.Project.Contracts.PropertyPages.VS2010ONLY;
using Microsoft.VisualStudio.Project.Framework;
#elif PLATFORM_VS2012
using Microsoft.VisualStudio.Project.Designers.Properties;
using Microsoft.VisualStudio.Project.Utilities.PropertyPages;
#elif PLATFORM_VS2013 || PLATFORM_VS2015
using Microsoft.VisualStudio.ProjectSystem.Designers.Properties;
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
    [Export(typeof(IDynamicEnumValuesProvider))]
#if PLATFORM_VS2015
    [ExportDynamicEnumValuesProvider("CompilerVersionSelector")]
#else
    [DynamicEnumCategory("CompilerVersionSelector")]
#endif
#if PLATFORM_VS2010
    [ProjectScope(ProjectScopeRequired.ConfiguredProject)]
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
