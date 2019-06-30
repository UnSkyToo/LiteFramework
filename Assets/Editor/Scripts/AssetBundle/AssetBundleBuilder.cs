﻿using System.IO;
using System.Collections.Generic;
using Lite.Framework.Helper;
using UnityEngine;
using UnityEditor;
using Logger = Lite.Framework.Log.Logger;

public class AssetBundleBuilder : MonoBehaviour
{
    private static List<string> LuaFileList_ = new List<string>();

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

    public static void BuildAsset(BuildTarget Target, BuildAssetBundleOptions Options, bool IsCollectInfo)
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        if (IsCollectInfo)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();

            LuaFileList_.Clear();
            var RootPath = $"{Application.dataPath}/StandaloneAssets/";
            var AssetsList = CollectAllAssetBundlePath(RootPath, RootPath);
            AssetsList = HandleAllLuaFile(AssetsList);
            ConfigurationAssetBundle(AssetsList);
        }

        // Start Build
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, Options, Target);

        if (IsCollectInfo)
        {
            DeleteAllLuaFile();
        }

        EditorUtility.DisplayDialog("Lite", "building done", "Ok");
        AssetDatabase.Refresh();
    }

    private static void DeleteAllLuaFile()
    {
        foreach (var LuaFile in LuaFileList_)
        {
            File.Delete(LuaFile);
            File.Delete($"{LuaFile}.meta");
        }
        LuaFileList_.Clear();
    }

    private static void HandleLuaFile(string FilePath)
    {
        var OldFilePath = $"Assets/StandaloneAssets/{FilePath}";
        var NewFilePath = $"Assets/StandaloneAssets/{FilePath}.bytes";
        LuaFileList_.Add(NewFilePath);
        File.Copy(OldFilePath, NewFilePath, true);
    }

    private static List<string> HandleAllLuaFile(List<string> AssetsList)
    {
        var FileList = new List<string>();

        foreach (var Asset in AssetsList)
        {
            if (Asset.EndsWith(".lua"))
            {
                HandleLuaFile(Asset);
                FileList.Add($"{Asset}.bytes");
            }
            else
            {
                FileList.Add(Asset);
            }
        }

        AssetDatabase.Refresh();
        return FileList;
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

    public static void CleanAllBundle()
    {
        var RootPath = $"{Application.dataPath}/StandaloneAssets/";
        var AssetList = CollectAllAssetBundlePath(RootPath, RootPath);
        foreach (var Asset in AssetList)
        {
            var Importer = AssetImporter.GetAtPath($"Assets/StandaloneAssets/{Asset}");
            if (Importer != null)
            {
                Importer.assetBundleName = string.Empty;
                Importer.assetBundleVariant = string.Empty;
            }
        }

        Directory.Delete(Application.streamingAssetsPath, true);
        Directory.CreateDirectory(Application.streamingAssetsPath);
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
        Logger.DWarning("StreamingAssets Clean");
    }
}