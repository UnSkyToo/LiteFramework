using System.IO;
using System.Collections.Generic;
using Lite.Framework.Helper;
using UnityEngine;
using UnityEditor;
using Logger = Lite.Framework.Log.Logger;

public class AssetBundleBuilder : MonoBehaviour
{
    [MenuItem("Lite/AssetBundle Builder")]
    private static void BuildAssets()
    {
        if (BuildPipeline.isBuildingPlayer)
        {
            EditorUtility.DisplayDialog("Lite", "building is running", "Ok");
            return;
        }

        AssetBundleOptionWindow.ShowWindow();
    }

    public static void BuildAsset(BuildTarget Target, BuildAssetBundleOptions Options)
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();

        var RootPath = $"{Application.dataPath}/StandaloneAssets/";
        var AssetsList = CollectAllAssetBundlePath(RootPath, RootPath);
        ConfigurationAssetBundle(AssetsList);

        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        // Start Build
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, Options, Target);
        EditorUtility.DisplayDialog("Lite", "building done", "Ok");

        AssetDatabase.Refresh();
    }

    private static List<string> CollectAllAssetBundlePath(string RootPath, string CurrentPath)
    {
        CurrentPath = PathHelper.UnifyPath(CurrentPath);
        var Result = new List<string>();

        var Files = Directory.GetFiles(CurrentPath);
        foreach (var FilePath in Files)
        {
            if (FilePath.EndsWith(".meta"))
            {
                continue;
            }
            Result.Add(FilePath.Substring(RootPath.Length));
        }

        var Dirs = Directory.GetDirectories(CurrentPath);
        foreach (var DirPath in Dirs)
        {
            var FileList = CollectAllAssetBundlePath(RootPath, DirPath);
            Result.AddRange(FileList);
        }

        return Result;
    }

    private static void ConfigurationAssetBundle(List<string> AssetsList)
    {
        foreach (var AssetPath in AssetsList)
        {
            SetAssetBundleName(AssetPath);
        }
    }

    private static void SetAssetBundleName(string ResPath)
    {
        var Importer = AssetImporter.GetAtPath($"Assets/StandaloneAssets/{ResPath}");
        if (Importer == null)
        {
            Logger.DError($"unexpected asset path : {ResPath}");
            return;
        }

        /*var Index = ResPath.LastIndexOf('.');
        if (Index == -1)
        {
            Logger.DError($"unexpected asset path : {ResPath}");
            return;
        }

        var BundleName = ResPath.Substring(0, Index);*/
        var BundleName = ResPath;

        if (Importer is TextureImporter TexImporter)
        {
            var Index = ResPath.LastIndexOf('.');
            BundleName = ResPath.Substring(0, Index);

            if (TexImporter.textureType == TextureImporterType.Sprite)
            {
                BundleName = $"{BundleName}.sprite";
            }
            else
            {
                BundleName = $"{BundleName}.texture";
            }
        }

        Importer.assetBundleName = BundleName;
        Importer.assetBundleVariant = string.Empty;
    }
}