using System.IO;
using UnityEditor;
using UnityEngine;

public class WebGlBuild 
{
    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGL()
    {
        string buildPath = "Builds/WebGL";
        if (Directory.Exists(buildPath))
            Directory.Delete(buildPath, true);

        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, buildPath, BuildTarget.WebGL, BuildOptions.None);
    }
}
