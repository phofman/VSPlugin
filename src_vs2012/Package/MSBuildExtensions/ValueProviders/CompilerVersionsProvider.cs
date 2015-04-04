using Microsoft.VisualStudio.ProjectSystem.Designers.Properties;
using Microsoft.VisualStudio.ProjectSystem.Utilities.PropertyPages;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Build.Framework.XamlTypes;

namespace BlackBerry.Package.MSBuildExtensions.ValueProviders
{
    [Export(typeof(IDynamicEnumValuesProvider)), DynamicEnumCategory("CompilerSelector")]
    public sealed class CompilerVersionsProvider : IDynamicEnumValuesProvider
    {
        public Task<IDynamicEnumValuesGenerator> GetProviderAsync(IList<NameValuePair> options)
        {
            return Task.FromResult<IDynamicEnumValuesGenerator>(new CompilerVersionsGenerator());
        }
    }
}
