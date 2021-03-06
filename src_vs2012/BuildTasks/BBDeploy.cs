﻿//* Copyright 2010-2011 Research In Motion Limited.
//*
//* Licensed under the Apache License, Version 2.0 (the "License");
//* you may not use this file except in compliance with the License.
//* You may obtain a copy of the License at
//*
//* http://www.apache.org/licenses/LICENSE-2.0
//*
//* Unless required by applicable law or agreed to in writing, software
//* distributed under the License is distributed on an "AS IS" BASIS,
//* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//* See the License for the specific language governing permissions and
//* limitations under the License.

using System;
using System.IO;
using BlackBerry.BuildTasks.Properties;
using Microsoft.Build.CPPTasks;
using Microsoft.Build.Framework;

namespace BlackBerry.BuildTasks
{
    public sealed class BBDeploy : BBTask
    {
        #region Member Variable Declaration

        private const string RunInfoFileName = "vsndk-runinfo.flag";
        private const string LogInfo_Lanuching = "Info: Launching ";
        private const string LogInfo_ProcessID = "result::";
        private const string LogInfo_ProcessIDRunning = "result::running,";

        private const string GET_FILE = "GetFile";
        private const string GET_FILE_SAVE_AS = "GetFileSaveAs";
        private const string PUT_FILE = "PutFile";
        private const string PUT_FILE_SAVE_AS = "PutFileSaveAs";
        private const string DEVICE = "Device";
        private const string PASSWORD = "Password";
        private const string PACKAGE_ID = "PackageId";
        private const string PACKAGE_NAME = "PackageName";
        private const string LAUNCH_APP = "LaunchApp";
        private const string INSTALL_APP = "InstallApp";
        private const string LIST_INSTALLED_APPS = "ListInstalledApps";
        private const string DEBUG_NATIVE = "DebugNative";
        private const string PACKAGE = "Package";

        private string _localManifest;
        private string _flagFile;

        private StreamWriter _localLogWriter;

        private string _packageName; // actual_dname of installed package
        private uint _processID; // ID of the launched process

        #endregion

        /// <summary>
        /// BBDeploy default constructor
        /// </summary>
        public BBDeploy()
            : base(Resources.ResourceManager)
        {
            DefineSwitchOrder(INSTALL_APP, LAUNCH_APP, LIST_INSTALLED_APPS, DEBUG_NATIVE, DEVICE, PASSWORD, PACKAGE, PACKAGE_ID, PACKAGE_NAME, GET_FILE, GET_FILE_SAVE_AS, PUT_FILE, PUT_FILE_SAVE_AS);
        }

        #region Overrides

        /// <summary>
        /// Helper function to return Response File Switch
        /// </summary>
        /// <param name="responseFilePath">Path to response file.</param>
        /// <returns>Return response file switch</returns>
        protected override string GetResponseFileSwitch(string responseFilePath)
        {
            return string.Empty;
        }

        /// <summary>
        /// Helper function to generte the command line argument string.
        /// </summary>
        /// <returns>command line argument string</returns>
        protected override string GenerateCommandLineCommands()
        {
            return GenerateResponseFileCommands();
        }

        /// <summary>
        /// Creates a temporary response (.rsp) file and runs the executable file.
        /// </summary>
        /// <returns>
        /// The returned exit code of the executable file. If the task logged errors, but the executable returned an exit code of 0, this method returns -1.
        /// </returns>
        /// <param name="pathToTool">The path to the executable file.</param><param name="responseFileCommands">The command line arguments to place in the .rsp file.</param><param name="commandLineCommands">The command line arguments to pass directly to the executable file.</param>
        protected override int ExecuteTool(string pathToTool, string responseFileCommands, string commandLineCommands)
        {
            if (ListInstalledApps)
            {
                _localLogWriter = File.CreateText("installedApps.txt");
                _localLogWriter.WriteLine("Info: Generating list of applications from the device ({0})", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            // delete the run-info file:
            if (File.Exists(RunInfoFileName))
            {
                File.Delete(RunInfoFileName);
            }

            _packageName = null;
            _processID = 0;

            var result = base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);

            if (_localLogWriter != null)
            {
                _localLogWriter.Dispose();
                _localLogWriter = null;
            }

            // create run-info file, if proper info was extracted from logs:
            if (!string.IsNullOrEmpty(_packageName) && _processID != 0)
            {
                File.WriteAllText(RunInfoFileName, string.Concat("dname=", _packageName, Environment.NewLine, "pid=", _processID, Environment.NewLine));
            }

            return result;
        }

        /// <summary>
        /// Parses a single line of text to identify any errors or warnings in canonical format.
        /// </summary>
        /// <param name="singleLine">A single line of text for the method to parse.</param><param name="messageImportance">A value of <see cref="T:Microsoft.Build.Framework.MessageImportance"/> that indicates the importance level with which to log the message.</param>
        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            // dump all printouts of this tool into dedicated logger:
            if (_localLogWriter != null)
            {
                _localLogWriter.WriteLine(singleLine);
                LogToolError(singleLine);
                return;
            }

            // preprocess the 'install' part returned by the deployment tool
            // (since we install whole .bar file or only selected files, when optimized deploy is on,
            // we can't trust the 'BAR build' part of it):
            if (string.IsNullOrEmpty(_packageName) && singleLine.StartsWith(LogInfo_Lanuching))
            {
                int dots = CountAtEnd(singleLine, '.');
                _packageName = singleLine.Substring(LogInfo_Lanuching.Length, singleLine.Length - LogInfo_Lanuching.Length - dots);
            }

            // expect PID, after the package name:
            if (!string.IsNullOrEmpty(_packageName) && _processID == 0 && singleLine.StartsWith(LogInfo_ProcessID))
            {
                uint.TryParse(singleLine.Substring(LogInfo_ProcessID.Length), out _processID);
            }
            if (!string.IsNullOrEmpty(_packageName) && _processID == 0 && singleLine.StartsWith(LogInfo_ProcessIDRunning))
            {
                uint.TryParse(singleLine.Substring(LogInfo_ProcessIDRunning.Length), out _processID);
            }

            base.LogEventsFromTextOutput(singleLine, messageImportance);
        }

        /// <summary>
        /// Counts occurrences of specified char at the end given text.
        /// </summary>
        private static int CountAtEnd(string text, char c)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            int i = text.Length - 1;
            int result = 0;

            while (i > 0 && text[i] == c)
            {
                result++;
                i--;
            }

            return result;
        }

        /// <summary>
        /// Getter for the CommandTLogName property
        /// </summary>
        protected override string CommandTLogName
        {
            get { return "BBDeploy.command.1.tlog"; }
        }

        /// <summary>
        /// Getter for the ReadTLogNames property
        /// </summary>
        protected override string[] ReadTLogNames
        {
            get { return new[] { "BBDeploy.read.1.tlog", "BBDeploy.*.read.1.tlog" }; }
        }

        /// <summary>
        /// Getter for the WriteTLogNames property
        /// </summary>
        protected override string[] WriteTLogNames
        {
            get { return new[] { "BBDeploy.write.1.tlog", "BBDeploy.*.write.1.tlog" }; }
        }

        /// <summary>
        /// Getter for the TrackedInputFiles property
        /// </summary>
        protected override ITaskItem[] TrackedInputFiles
        {
            get
            {
                // I don't know what this method does but we need to return something other than 'null' or the task doesn't run,
                // and adding something like "localManifest.mf" to the list of items causes other problems.
                ITaskItem[] items = new ITaskItem[1];
                return items;
            }
        }

        /// <summary>
        /// Getter for the ToolName property
        /// </summary>
        protected override string ToolName
        {
            get { return ToolExe; }
        }

        /// <summary>
        /// Returns the full path to the tool.
        /// </summary>
        protected override string GenerateFullPathToTool()
        {
            return ToolName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Getter/Setter for the GetFile property
        /// Retrieve specified file from default application directory
        /// </summary>
        public string GetFile
        {
            get { return GetSwitchAsString(GET_FILE); }
            set
            {
                // Value passed in is the file to get, relative to our app's directory.
                // We determine the app's directory from the PackageName and PackageId.
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.String)
                {
                    Name = GET_FILE,
                    DisplayName = "GetFile",
                    Description = "Get the given file from the application's directory.",
                    SwitchValue = "-getFile ",
                    Value = string.Concat("../../../../apps/", PackageName, ".", PackageId, "/", value)
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for the GetFileSaveAs property
        /// Retrieve specified file from location.
        /// </summary>
        public string GetFileSaveAs
        {
            get { return GetSwitchAsString(GET_FILE_SAVE_AS); }
            set
            {
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.String)
                {
                    Name = GET_FILE_SAVE_AS,
                    DisplayName = "GetFileSaveAs",
                    Description = "Save a file retrieved using GetFile to the given location.",
                    Value = value
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for the PutFile property
        /// Save given file to default application directory.
        /// </summary>
        public string PutFile
        {
            get { return GetSwitchAsString(PUT_FILE); }
            set
            {
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.String)
                {
                    Name = PUT_FILE,
                    DisplayName = "PutFile",
                    Description = "Put the given file into the application's directory.",
                    SwitchValue = "-putFile ",
                    Value = value
                    //Value = value.Replace(".exe", "");
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for the PutFileSaveAs property
        /// Save given file to specified location
        /// </summary>
        public string PutFileSaveAs
        {
            get { return GetSwitchAsString(PUT_FILE_SAVE_AS); }
            set
            {
                // We determine the app's directory from the PackageName and PackageId.
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.String)
                {
                    Name = PUT_FILE_SAVE_AS,
                    DisplayName = "PutFileSaveAs",
                    Description = "Save a file retrieved using PutFile to the given location.",
                    Value = string.Concat("../../../../apps/", PackageName, ".", PackageId + "/", value)
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for the InstallApp property - Switch to cause deploy to install a given application
        /// </summary>
        public bool InstallApp
        {
            get { return GetSwitchAsBool(INSTALL_APP); }
            set
            {
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.Boolean)
                {
                    Name = INSTALL_APP,
                    DisplayName = "Install App",
                    Description = "The -installApp option installs an app from a given package.",
                    SwitchValue = "-installApp",
                    BooleanValue = value
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for the LaunchApp Property - Switch to launch given application.
        /// </summary>
        public bool LaunchApp
        {
            get { return GetSwitchAsBool(LAUNCH_APP); }
            set
            {
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.Boolean)
                {
                    Name = LAUNCH_APP,
                    DisplayName = "Launch App",
                    Description = "The -launchApp option allows an app to be launched with or without being installed.",
                    SwitchValue = "-launchApp ",
                    BooleanValue = value
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for ListInstalledApps Property.
        /// Switch for command to retrieve the list of installed apps.
        /// </summary>
        public bool ListInstalledApps
        {
            get { return GetSwitchAsBool(LIST_INSTALLED_APPS); }
            set
            {
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.Boolean)
                {
                    Name = LIST_INSTALLED_APPS,
                    DisplayName = "List Installed Apps",
                    Description = "The -listInstalledApps option does just that.",
                    SwitchValue = "-listInstalledApps ",
                    BooleanValue = value
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Setter for FlagFile property
        /// </summary>
        public string FlagFile
        {
            set
            {
                _flagFile = value;
            }
        }

        /// <summary>
        /// Getter/Setter for DebugNative property
        /// Switch to cause task to install application in debug configuration.
        /// </summary>
        public bool DebugNative
        {
            get { return GetSwitchAsBool(DEBUG_NATIVE); }

            set
            {
                if (File.Exists(_flagFile))
                {
                    value = true;
                }

                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.Boolean)
                {
                    Name = DEBUG_NATIVE,
                    DisplayName = "Launch for debugging",
                    Description = "The -debugNative option launches the app in suspended, ready-to-debug state.",
                    SwitchValue = "-debugNative ",
                    BooleanValue = value
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for Device property.
        /// Device property designates the IP of the target device.
        /// </summary>
        public string Device
        {
            get { return GetSwitchAsString(DEVICE); }
            set
            {
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.String)
                {
                    Name = DEVICE,
                    DisplayName = "Device IP",
                    Description = "The -device option specifies the target device's IP address.",
                    SwitchValue = "-device ",
                    Value = value
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for Password property.
        /// Password property specifies the target device password.
        /// </summary>
        public string Password
        {
            get { return GetSwitchAsString(PASSWORD); }
            set
            {
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.String)
                {
                    Name = PASSWORD,
                    DisplayName = "Device Password",
                    Description = "The -password option specifies the target device's password.",
                    SwitchValue = "-password ",
                    Value = DecryptPassword(value)
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for Package property.
        /// Package property specified package to be installed/launched on task execution.
        /// </summary>
        public string Package
        {
            get { return GetSwitchAsString(PACKAGE); }
            set
            {
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.String)
                {
                    Name = PACKAGE,
                    DisplayName = "Package",
                    Description = "The -package option specifies a .bar package.",
                    SwitchValue = "-package ",
                    Value = value
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for PackageId property
        /// </summary>
        [Output]
        public string PackageId
        {
            get { return GetSwitchAsString(PACKAGE_ID); }
            set
            {
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.String)
                {
                    Name = PACKAGE_ID,
                    DisplayName = "Package ID",
                    Description = "The -package-id option specifies the application's package id.",
                    SwitchValue = "-package-id ",
                    Value = value
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for PackageName property
        /// </summary>
        [Output]
        public string PackageName
        {
            get { return GetSwitchAsString(PACKAGE_NAME); }
            set
            {
                ToolSwitch toolSwitch = new ToolSwitch(ToolSwitchType.String)
                {
                    Name = PACKAGE_NAME,
                    DisplayName = "Package Name",
                    Description = "The -package-name option specifies the application's package name.",
                    SwitchValue = "-package-name ",
                    Value = value
                };
                SetSwitch(toolSwitch);
            }
        }

        /// <summary>
        /// Getter/Setter for LocalManifestFile property
        /// We need the local manifest in order to determine PackageId and PackageName.
        /// </summary>
        public string LocalManifestFile
        {
            get { return _localManifest; }
            set
            {
                _localManifest = value;

                string[] lines = File.ReadAllLines(_localManifest);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("Package-Name: "))
                    {
                        string name = lines[i].Substring(14);
                        PackageName = name;
                    }
                    else if (lines[i].StartsWith("Package-Id: "))
                    {
                        string id = lines[i].Substring(12);
                        PackageId = id;
                    }
                }
            }
        }

        /// <summary>
        /// Getter/Setter for the TargetManifestFile
        /// </summary>
        [Output]
        public string TargetManifestFile
        {
            get { return null; }
        }

        #endregion
    }
}
