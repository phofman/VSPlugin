#if PLATFORM_VS2012
using Microsoft.VisualStudio.Project.Designers.Properties;
using Microsoft.VisualStudio.Project.Utilities.PropertyPages;
#elif PLATFORM_VS2013
using Microsoft.VisualStudio.ProjectSystem.Designers.Properties;
using Microsoft.VisualStudio.ProjectSystem.Utilities.PropertyPages;
#endif
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Build.Framework.XamlTypes;

namespace BlackBerry.Package.MSBuildExtensions.ValueProviders
{
    [Export(typeof(IDynamicEnumValuesProvider)), DynamicEnumCategory("CompilerVersionSelector")]
    public sealed class CompilerVersionsProvider : IDynamicEnumValuesProvider
    {
#if PLATFORM_VS2012
        public IDynamicEnumValuesGenerator GetProvider(IList<NameValuePair> options)
        {
            return new CompilerVersionsGenerator();
        }
#endif

#if PLATFORM_VS2013
        public Task<IDynamicEnumValuesGenerator> GetProviderAsync(IList<NameValuePair> options)
        {
            return Task.FromResult<IDynamicEnumValuesGenerator>(new CompilerVersionsGenerator());
        }
#endif
    }
}
