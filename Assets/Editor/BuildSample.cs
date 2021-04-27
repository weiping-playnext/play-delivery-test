using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Google.Android.AppBundle.Editor;
using System.Linq;
using System.IO;

public class BuildSample
{
    static string[] scenesToBuild = new []
    {
        "Assets/Scenes/Scene2.unity"
    };

    static string[] prefabsToBuild = new[]
    {
        "Assets/AssetBundlePrefabs/Prefab1.prefab",
        "Assets/AssetBundlePrefabs/Prefab2.prefab"
    };

    [MenuItem("Customs/TestBuild")]
    static void BuildTest()
    {
        AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
        assetBundleBuild.assetNames = prefabsToBuild;
        assetBundleBuild.addressableNames = prefabsToBuild;
        assetBundleBuild.assetBundleName = "prefabs";

        AssetPackConfig assetPackConfig = new AssetPackConfig();
        List<string> assetBundlePaths = new List<string>();

        string outputPath = "Build/Android/TempStagingAssetBundles";//EditorUtility.SaveFolderPanel("Build Sample AssetBandle","","");
        if(!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        if (!string.IsNullOrEmpty(outputPath))
        {
            var report = BuildPipeline.BuildPlayer(scenesToBuild, outputPath + "/scenes/add_scenes.unity3d", BuildTarget.Android, BuildOptions.BuildAdditionalStreamedScenes);
            foreach (var filepath in report.files)
            {
                if (Path.GetExtension(filepath.path) == "unity3d")
                {
                    assetBundlePaths.Add(filepath.path);
                }
            }

            var manifest = BuildPipeline.BuildAssetBundles(outputPath, new AssetBundleBuild[] { assetBundleBuild }, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
            
            string[] allBundles = manifest.GetAllAssetBundles();
            foreach (string bundle in allBundles)
            {
                assetBundlePaths.Add(Path.Combine(outputPath, bundle));
            }
        }

        foreach(var bundle in assetBundlePaths)
        {
            Debug.Log(bundle);
            assetPackConfig.AddAssetBundle(bundle, AssetPackDeliveryMode.FastFollow);
        }


        string bundlePath = "Build/Android/apptest.aab";
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.locationPathName = bundlePath;
        buildPlayerOptions.options = BuildOptions.None;
        buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
        Bundletool.BuildBundle(buildPlayerOptions, assetPackConfig);
    }


}
