using System;
using System.IO;
using System.Text;
using Editor.Deploy;
using Ionic.Zip;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Utils;

namespace Editor.Build
{
    public class Build
    {
        private static readonly string BuildPath = $"{Application.dataPath}/../Builds";
        private static readonly string ProductName = "SolarSystem";

        private static readonly string[] suffixes =
            {"Bytes", "KB", "MB", "GB", "TB", "PB"};


        [MenuItem("DevOps/Build & Deploy All")]
        public static void BuildAndDeployAll()
        {
            PerformAllBuilds();
            Itchio.DeployAll();
        }
        
        [MenuItem("DevOps/Build/All")]
        private static void PerformAllBuilds()
        {
            CleanAll();
            var versionNumber = Git.BuildVersion;
            PlayerSettings.bundleVersion = versionNumber;
            Debug.Log($"Build for all platforms, version number {versionNumber}...");
            PerformWebGLBuild();
            PerformMacOSBuild();
            PerformWindowsBuild();
            PerformLinuxBuild();
            Debug.Log($"Build for all platforms complete. version number {versionNumber}");
        }
        
        private static void PerformBuild(BuildTarget target, BuildOptions options)
        {
            var versionNumber = Git.BuildVersion;
            PlayerSettings.bundleVersion = versionNumber;
            var buildLocation = $"{BuildPath}/{ProductName}.{target}.{versionNumber}";
            var buildName = $"{ProductName}{GetAppExtension(target)}";
            Debug.Log($"buildLocation {buildLocation}");
            Debug.Log($"path: {buildLocation}/{buildName}");
            // edge case for WebGL, as it's a single folder with all the files in it 
            if (target == BuildTarget.WebGL)
            {
                buildName = "";
            }
            var report = BuildPipeline.BuildPlayer(
                EditorBuildSettings.scenes,
                $"{buildLocation}/{buildName}",
                target,
                options
            );
            if (!GenerateBuildReportAndCheckIfSuccess(report)) return;
            CreateVersionFile(target);
            ArchiveBuild(buildLocation);
        }

        public static string GetBuildLocationForTarget(BuildTarget target)
        {
            var versionNumber = Git.BuildVersion;
            return $"{BuildPath}/{ProductName}.{target}.{versionNumber}";
        }

        private static string GetAppExtension(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneOSX:
                    return ".app";
                case BuildTarget.StandaloneWindows64:
                    return ".exe";
                case BuildTarget.WebGL:
                    return ".webGL";
                case BuildTarget.StandaloneLinux64:
                    return ".x86_64";
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        [MenuItem("DevOps/Build/Build For WebGL")]
        public static void PerformWebGLBuild()
        {
            PerformBuild(BuildTarget.WebGL, BuildOptions.None);
        }

        [MenuItem("DevOps/Build/Build For macOS")]
        public static void PerformMacOSBuild()
        {
            PerformBuild(BuildTarget.StandaloneOSX, BuildOptions.None);
        }

        [MenuItem("DevOps/Build/Build For Windows 64")]
        public static void PerformWindowsBuild()
        {
            PerformBuild(BuildTarget.StandaloneWindows64, BuildOptions.None);
        }

        [MenuItem("DevOps/Build/Build For Linux 64")]
        public static void PerformLinuxBuild()
        {
            PerformBuild(BuildTarget.StandaloneLinux64, BuildOptions.None);
        }
        private static bool GenerateBuildReportAndCheckIfSuccess(BuildReport report)
        {
            var summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log($"Build succeeded for {summary.platform}. Size: {FormatSize(summary.totalSize)} in [{summary.totalTime}]");
                    return true;
                case BuildResult.Failed:
                    Debug.Log($"Build failed for {summary.platform}. Size: {FormatSize(summary.totalSize)} in [{summary.totalTime}]");
                    return false;
                case BuildResult.Unknown:
                    return false;
                case BuildResult.Cancelled:
                    Debug.Log($"Build cancelled for {summary.platform}. Size: {FormatSize(summary.totalSize)} in [{summary.totalTime}]");
                    return false;
                default:
                    Debug.Log(
                        $"Build failed, unknown result:{summary.result} for {summary.platform}. Size: {FormatSize(summary.totalSize)} in [{summary.totalTime}]");
                    return false;
            }
        }
        
        [MenuItem("DevOps/Git Status")]
        public static void GitStatus()
        {
            var status = Git.Status;
            Debug.Log($"git status {status}");
            if (Git.Status == "")
            {
                Debug.Log($"Local files are all comitted.");
            }
            else
            {
                Debug.Log($"You have local files that have not been comitted: {status}");
            }
        }
        
        private static void CreateVersionFile(BuildTarget target)
        {
            var versionNumber = Git.BuildVersion;
            var buildPathForTarget = GetBuildLocationForTarget(target);
            Encoding utf8WithoutBom = new UTF8Encoding(false);
            using (TextWriter tw = new StreamWriter($"{buildPathForTarget}/buildnumber.txt", false, utf8WithoutBom))
            {
                tw.WriteLine(versionNumber);
            }
        }

        private static void ArchiveBuild(string fileOrFolderToArchive)
        {
            using (var zip = new ZipFile())
            {
                zip.AddDirectory($"{fileOrFolderToArchive}");
                zip.Save($"{fileOrFolderToArchive}.zip");
            }
        }
        
        [MenuItem("DevOps/Clean/All")]
        public static void CleanAll()
        {
            CleanMac();
            CleanWindows();
            CleanLinux();
            CleanWebGL();
        }
        
        [MenuItem("DevOps/Clean/Clean macOS")]
        public static void CleanMac()
        {
            RemoveBuildTargetFiles(BuildTarget.StandaloneOSX);
        }
        [MenuItem("DevOps/Clean/Clean Windows")]
        public static void CleanWindows()
        {
            RemoveBuildTargetFiles(BuildTarget.StandaloneWindows64);
        }
        [MenuItem("DevOps/Clean/Clean Linux")]
        public static void CleanLinux()
        {
            RemoveBuildTargetFiles(BuildTarget.StandaloneLinux64);
        }
        [MenuItem("DevOps/Clean/Clean WebGL")]
        public static void CleanWebGL()
        {
            RemoveBuildTargetFiles(BuildTarget.WebGL);
        }
        
        private static void RemoveBuildTargetFiles(BuildTarget target)
        {
            var versionNumber = Git.BuildVersion;
            PlayerSettings.bundleVersion = versionNumber;
            var buildLocation = $"{BuildPath}/{ProductName}.{target}.{versionNumber}";
            try
            {
                Debug.Log($"deleting build, {buildLocation}");
                Directory.Delete($"{buildLocation}", true);
                Debug.Log("done.");
            }
            catch (IOException e)
            {
                Debug.Log($"couldn't delete target folder {buildLocation}. {e.Message}");
            }

            try
            {
                Debug.Log($"deleting build archive, {buildLocation}.zip");
                File.Delete($"{buildLocation}.zip");
                Debug.Log("done.");
            }
            catch (IOException e)
            {
                Debug.Log($"couldn't delete target zip {buildLocation}.zip {e.Message}");
            }
            
        }

        public static string FormatSize(ulong bytes)
        {
            var counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }

            return $"{number:n1}{suffixes[counter]}";
        }
    }
}