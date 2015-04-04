using BlackBerry.Package.ViewModels;
using Microsoft.VisualStudio.ProjectSystem.Designers.Properties;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BlackBerry.Package.MSBuildExtensions.ValueProviders
{
    sealed class CompilerVersionsGenerator : IDynamicEnumValuesGenerator
    {
        public bool AllowCustomValues
        {
            get { return false; }
        }

        public Task<ICollection<IEnumValue>> GetListedValuesAsync()
        {
            var result = new List<IEnumValue>();
            var ndk = PackageViewModel.Instance != null ? PackageViewModel.Instance.ActiveNDK : null;

            // add the default item, to let the toolset select appropriate the compiler version:
            result.Add(new DynamicEnumValue("", "default", "Default compiler version evaluated and used by toolset", true));

            if (ndk != null && Directory.Exists(ndk.HostPath))
            {
                var dirs = Directory.GetDirectories(Path.Combine(ndk.HostPath, "etc", "qcc", "gcc"));
                foreach(var d in dirs)
                {
                    var name = Path.GetFileName(d);
                    result.Add(new DynamicEnumValue(name, name, string.Concat("Forces usage of the '", name, "' version of the compiler (", d, ")"), false));
                }
            }

            return Task.FromResult<ICollection<IEnumValue>>(result.ToArray());
        }

        public Task<IEnumValue> TryCreateEnumValueAsync(string userSuppliedValue)
        {
            return Task.FromResult<IEnumValue>(null);
        }
    }
}
