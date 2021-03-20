#region MIT License

// # Released under MIT License
// 
// Copyright (c) 2021 Byron Wright.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

#endregion

using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Utils;
using Debug = UnityEngine.Debug;

namespace Editor.Deploy
{
    public class Itchio
    {
        // because the executable butler is already in my path / env variables (on macOS) I can
        // just refer to the executable name
        #if UNITY_EDITOR_WIN
        public static string pathToButlerExecutable = "C:\\Users\\bslay\\Projects\\bin\\butler\\butler.exe";
        #endif
        #if UNITY_EDITOR_OSX
        
        public static string pathToButlerExecutable = "/Users/byronwright/Projects/Goodies/itchio/butler/butler";
        #endif
        // An example of these two variables can be found in the url of your project e.g.:
        //https://bslayerw.itch.io/solar-system
        // where bslayerw part is your username and "solar-system" is your game/project name
        // this is part of the url of your project page on itch.io. It's always all lower-case.
        public static string userName = "bslayerw";

        // this is part of the url of your project page on itch.io. It's always all lower-case
        public static string gameName = "solar-system";

        [MenuItem("DevOps/Deploy/itch.io/Login")]
        public static string Login()
        {
            using (var process = new Process())
            {
                var exitCode = process.Run(
                    pathToButlerExecutable,
                    "login",
                    Application.dataPath,
                    out var output,
                    out var errors
                );

                if (exitCode == 0)
                {
                    EditorUtility.DisplayDialog(
                        "Logged to itch.io",
                        $"{output}",
                        "Ok"
                    );
                    return output;
                }

                EditorUtility.DisplayDialog(
                    "failed to login to itch.io",
                    $"{errors}, {output}",
                    "Ok"
                );
                Debug.LogError($"failed to login to itch.io: {errors}, {output}");
                throw new ItchioException(exitCode, errors);
            }
        }

        public static void DeployForTargets(BuildTarget target)
        {
            var buildFolder = Build.Build.GetBuildLocationForTarget(target);
            var buildZip = $"{buildFolder}.zip";
            if (!File.Exists(buildZip))
            {
                Debug.LogError($"Failed to deploy {target}, not build file found. Be sure to create a build first");
                return;
            }

            var pushCmd =
                $"push \"{buildZip}\" {userName}/{gameName}:{ChannelForTarget(target)} --userversion-file \"{buildFolder}\"{Path.DirectorySeparatorChar}/buildnumber.txt";
            Debug.Log($"running commmand: {pushCmd}");

            using (var process = new Process())
            {
                var exitCode = process.Run(
                    pathToButlerExecutable,
                    pushCmd,
                    Application.dataPath,
                    out var output,
                    out var errors
                );
                if (exitCode == 0)
                {
                    Debug.Log($"Successfully deploy {target} itch.io");
                    return;
                }
                Debug.LogError($"failed to deploy to itch.io: {errors}, {output}");
                throw new ItchioException(exitCode, errors);
            }
        }

        [MenuItem("DevOps/Deploy/itch.io/All")]
        public static void DeployAll()
        {
            DeployWebGL();
            DeployMac();
            DeployWindows();
            DeployLinux();
        }

        [MenuItem("DevOps/Deploy/itch.io/WebGL")]
        public static void DeployWebGL()
        {
            DeployForTargets(BuildTarget.WebGL);
        }

        [MenuItem("DevOps/Deploy/itch.io/Mac")]
        public static void DeployMac()
        {
            DeployForTargets(BuildTarget.StandaloneOSX);
        }

        [MenuItem("DevOps/Deploy/itch.io/Windows")]
        public static void DeployWindows()
        {
            DeployForTargets(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("DevOps/Deploy/itch.io/Linux")]
        public static void DeployLinux()
        {
            DeployForTargets(BuildTarget.StandaloneLinux64);
        }

        [MenuItem("DevOps/Deploy Status/WebGL")]
        public static string WebGLStatus()
        {
            var status = Status(BuildTarget.StandaloneOSX);
            EditorUtility.DisplayDialog(
                "WebGL Status from itch.io",
                $"{status}",
                "Ok"
            );
            return status;
        }

        [MenuItem("DevOps/Deploy Status/macOS")]
        public static string MacStatus()
        {
            var status = Status(BuildTarget.StandaloneOSX);
            EditorUtility.DisplayDialog(
                "macOS Status from itch.io",
                $"{status}",
                "Ok"
            );
            return status;
        }

        [MenuItem("DevOps/Deploy Status/Windows")]
        public static string WindowsStatus()
        {
            var status = Status(BuildTarget.StandaloneWindows64);
            EditorUtility.DisplayDialog(
                "Windows Status from itch.io",
                $"{status}",
                "Ok"
            );
            return status;
        }

        [MenuItem("DevOps/Deploy Status/Linux")]
        public static string WindowsLinux()
        {
            var status = Status(BuildTarget.StandaloneLinux64);
            EditorUtility.DisplayDialog(
                "Linux Status from itch.io",
                $"{status}",
                "Ok"
            );
            return status;
        }

        public static string ChannelForTarget(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneOSX:
                    return "osx";
                case BuildTarget.StandaloneWindows64:
                    return "windows";
                case BuildTarget.WebGL:
                    return "html5";
                case BuildTarget.StandaloneLinux64:
                    return "linux";
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        public static string Status(BuildTarget target)
        {
            using (var process = new Process())
            {
                var exitCode = process.Run(
                    pathToButlerExecutable,
                    $"status {userName}/{gameName}:{ChannelForTarget(target)}",
                    Application.dataPath,
                    out var output,
                    out var errors
                );
                if (exitCode == 0) return output;

                EditorUtility.DisplayDialog(
                    "failed to deploy to itch.io",
                    $"{errors}, {output}",
                    "Ok"
                    );
                Debug.LogError($"failed to deploy to itch.io: {errors}, {output}");
                throw new ItchioException(exitCode, errors);
            }
        }
    }

    public class ItchioException : InvalidOperationException
    {
        /// <summary>
        ///     The exit code returned when running the Git command.
        /// </summary>
        public readonly int ExitCode;

        public ItchioException(int exitCode, string errors) : base(errors)
        {
            ExitCode = exitCode;
        }
    }
}