using System.Collections;
using System.Collections.Generic;
using Ionic.Zip;
using UnityEditor;
using UnityEngine;

public class Build
{
    static string buildPath = $"{Application.dataPath}/../buildfiles";
    [MenuItem( "DevOps/Perform All Builds" )]
    static void PerformAllBuilds()
    {
        PerformWebGLBuild();
        PerformMacOSBuild();
        PerformWindowsBuild();
        PerformLinuxBuild();
    }

    [MenuItem( "DevOps/Build/Perform WebGL Build" )]
    public static void PerformWebGLBuild()
    {
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "../Builds/SolarSystem.webGL", BuildTarget.WebGL, BuildOptions.None);
    }
    
    [MenuItem( "DevOps/Build/Perform macOS Build" )]
    public static void PerformMacOSBuild()
    {
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
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "../Builds/SolarSystem.win/SolarSystem.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    [MenuItem( "DevOps/Build/Perform Linux Builds" )]
    public static void PerformLinuxBuild()
    {
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "../Builds/SolarSystem.Linux64/SolarSystem.x86_64", BuildTarget.StandaloneLinux64, BuildOptions.None);
    }
    [MenuItem( "DevOps/Deploy/itch.io" )]
    public static void DeployToItchio()
    {
        Debug.Log("This will eventually deploy all the builds to itch.io");
    }
}
