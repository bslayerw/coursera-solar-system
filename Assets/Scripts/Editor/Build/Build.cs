using System;
using Ionic.Zip;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor.Build
{
    public class Build
    {
        static string buildPath = $"{Application.dataPath}/../buildfiles";
        [MenuItem( "DevOps/Perform All Builds" )]
        static void PerformAllBuilds()
        {
            var versionNumber = Git.BuildVersion;
            PlayerSettings.bundleVersion = versionNumber;
            Debug.Log($"Build for all platforms, version number {versionNumber}");
            /*PerformWebGLBuild();
            PerformMacOSBuild();
            PerformWindowsBuild();
            PerformLinuxBuild();*/
        }

        [MenuItem( "DevOps/Build/Perform WebGL Build" )]
        public static void PerformWebGLBuild()
        {
            PlayerSettings.bundleVersion = Git.BuildVersion;
            var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "../Builds/SolarSystem.webGL", BuildTarget.WebGL, BuildOptions.None);
            GenerateBuildReport(report);
        }
    
        [MenuItem( "DevOps/Build/Perform macOS Build" )]
        public static void PerformMacOSBuild()
        {
            PlayerSettings.bundleVersion = Git.BuildVersion;
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "../Builds/SolarSystem.osX/SolarSystem.app", BuildTarget.StandaloneOSX, BuildOptions.None);
            using(ZipFile zip= new ZipFile())
            {
                zip.AddFile("../Builds/SolarSystem.osX/SolarSystem.app");
                zip.Save("../Builds/SolarSystem.osX/SolarSystem.app.zip");
            }
        }

        [MenuItem( "DevOps/Build/Perform Windows Build" )]
        public static void PerformWindowsBuild()
        {
            PlayerSettings.bundleVersion = Git.BuildVersion;
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "../Builds/SolarSystem.win/SolarSystem.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
        }

        [MenuItem( "DevOps/Build/Perform Linux Builds" )]
        public static void PerformLinuxBuild()
        {
            PlayerSettings.bundleVersion = Git.BuildVersion;
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "../Builds/SolarSystem.Linux64/SolarSystem.x86_64", BuildTarget.StandaloneLinux64, BuildOptions.None);
        }
        [MenuItem( "DevOps/Deploy/itch.io" )]
        public static void DeployToItchio()
        {
            Debug.Log("This will eventually deploy all the builds to itch.io");
        }

        private static void GenerateBuildReport(BuildReport report)
        {
            var summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    
                    Debug.Log($"Build succeeded:{summary.totalSize} bytes for {summary.platform}");
                    break;
                case BuildResult.Failed:
                    Debug.Log($"Build failed for {summary.platform}");
                    break;
                case BuildResult.Unknown:
                    break;
                case BuildResult.Cancelled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
