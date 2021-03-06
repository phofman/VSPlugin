﻿using System;
using System.Collections.Generic;
using System.IO;
using BlackBerry.NativeCore.Diagnostics;

namespace BlackBerry.NativeCore.Model
{
    /// <summary>
    /// Description of locally installed runtime libraries.
    /// </summary>
    public sealed class RuntimeInfo : ApiInfo
    {
        public RuntimeInfo(string folder, string name, Version version)
            : base(name, version, DeviceFamilyType.Phone)
        {
            Folder = folder;
            Details = Version.ToString();
        }

        #region Properties

        /// <summary>
        /// Gets the local path, where the runtime libraries are installed.
        /// </summary>
        public string Folder
        {
            get;
            private set;
        }

        public override bool IsInstalled
        {
            get { return !string.IsNullOrEmpty(Folder) && Directory.Exists(Folder); }
        }

        #endregion

        /// <summary>
        /// Gets an indication, if current runtime points to the same location.
        /// </summary>
        public bool Matches(string folder)
        {
            return string.CompareOrdinal(folder, Folder) == 0;
        }

        /// <summary>
        /// Gets an indication, if current runtime points to the same location.
        /// </summary>
        public bool Matches(RuntimeInfo info)
        {
            if (info == null)
                return false;

            return string.CompareOrdinal(info.Folder, Folder) == 0
                && info.Version == Version;
        }

        /// <summary>
        /// Creates the shimmed definition instance.
        /// </summary>
        public RuntimeDefinition ToDefinition()
        {
            return new RuntimeDefinition(Folder);
        }

        /// <summary>
        /// Loads descriptions of installed runtimes from specified folders.
        /// </summary>
        public static RuntimeInfo[] Load(params string[] globalRuntimeConfigFolders)
        {
            if (globalRuntimeConfigFolders == null)
                throw new ArgumentNullException("globalRuntimeConfigFolders");

            var result = new List<RuntimeInfo>();

            foreach (var folder in globalRuntimeConfigFolders)
            {
                if (Directory.Exists(folder))
                {
                    try
                    {
                        string[] runtimeDirectories = Directory.GetDirectories(folder);

                        foreach (string runtimeDirectory in runtimeDirectories)
                        {
                            if (runtimeDirectory.Contains("runtime_"))
                            {
                                var name = Path.GetFileName(runtimeDirectory); // 'runtime_xx_yy_zz'...
                                var version = GetVersionFromFolderName(name);

                                if (version != null)
                                {
                                    var runtimeInfo = new RuntimeInfo(runtimeDirectory, string.Concat("Runtime Libraries for ", version), version);

                                    result.Add(runtimeInfo);
                                }
                                else
                                {
                                    TraceLog.WarnLine("Unable to find runtime libraries version number in folder: \"{0}\"", runtimeDirectory);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteException(ex, "Unable to load info about runtime libraries from folder: \"{0}\"", folder);
                    }
                }
            }

            result.Sort();
            return result.ToArray();
        }
    }
}
