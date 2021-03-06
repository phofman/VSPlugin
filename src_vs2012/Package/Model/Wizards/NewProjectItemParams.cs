﻿using System;
using System.IO;
using EnvDTE;

namespace BlackBerry.Package.Model.Wizards
{
    /// <summary>
    /// Context parameters passed by Visual Studio, when creating new project item.
    /// </summary>
    sealed class NewProjectItemParams : ContextParams
    {
        internal const string TypeGuid = "{0f90e1d1-4999-11d1-b6d1-00a0c90f2744}";

        /// <summary>
        /// Init constructor.
        /// </summary>
        public NewProjectItemParams(object[] contextParams)
            : base(TypeGuid, contextParams != null && contextParams.Length > 1 ? contextParams[1] : null)
        {
            if (contextParams == null)
                throw new ArgumentNullException("contextParams");
            if (contextParams.Length < 6)
                throw new ArgumentOutOfRangeException("contextParams");

            ProjectItems = (ProjectItems) contextParams[2];
            LocalDirectory = contextParams[3] != null ? contextParams[3].ToString() : null;
            ItemName = contextParams[4] != null ? contextParams[4].ToString() : null;
            InstallationDirectory = contextParams[5] != null ? contextParams[5].ToString() : null;
            Silent = contextParams.Length < 7 || GetBoolValue(contextParams[6]); // optional in VS2010, set to 'true' then

            // if item is created via 'File->New->File...' it will have no folder at name start, so let's try to add it to keep the convention:
            if (!string.IsNullOrEmpty(LocalDirectory) && !string.IsNullOrEmpty(ItemName) && !Path.IsPathRooted(ItemName))
            {
                ItemName = Path.Combine(LocalDirectory, ItemName);
            }
        }

        #region Properties

        public ProjectItems ProjectItems
        {
            get;
            private set;
        }

        public string LocalDirectory
        {
            get;
            private set;
        }

        public string ItemName
        {
            get;
            private set;
        }

        public string InstallationDirectory
        {
            get;
            private set;
        }

        public bool Silent
        {
            get;
            private set;
        }

        #endregion
    }
}
