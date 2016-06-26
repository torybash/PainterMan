#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class EditorHelper {

	public static T GetFirstDirectoryAsset<T>(DefaultAsset dir, string subFolder = "")  where T : Object {
        string path = AssetDatabase.GetAssetPath(dir);
		string[] filePaths = System.IO.Directory.GetFiles(path + subFolder);
		for (int i = 0; i < filePaths.Length; i++) {
			T asset = GetAssetAtPath<T>(filePaths[i]);
			return asset;
        }
		Debug.LogError("[EditorHelper] GetFirstDirectoryAsset - type: "+ typeof(T) + " not existing at path: " + path);
        return null;
    }

	 public static List<T> GetDirectoryAssets<T>(DefaultAsset dir, bool includeSubFolders = false)  where T : Object {
        List<T> assetList = new List<T>();
        string path = AssetDatabase.GetAssetPath(dir);
		string[] filePaths = System.IO.Directory.GetFiles(path);
		for (int i = 0; i < filePaths.Length; i++) {
			T asset = GetAssetAtPath<T>(filePaths[i]);
			if (asset != null) assetList.Add(asset);
        }

		if (includeSubFolders) assetList.AddRange(GetSubFolderAssets<T>(path));

        return assetList;
    }

	private static List<T> GetSubFolderAssets<T>(string path) where T : Object {
		List<T> assetList = new List<T>();
		string[] dirPaths = System.IO.Directory.GetDirectories(path);
		for (int i = 0; i < dirPaths.Length; i++) {
			string dirPath = dirPaths[i];
			string[] filePaths = System.IO.Directory.GetFiles(dirPath);
			for (int j = 0; j < filePaths.Length; j++) {
				T asset = GetAssetAtPath<T>(filePaths[j]);
				if (asset != null) assetList.Add(asset);
			}

			string[] subDirPaths = System.IO.Directory.GetDirectories(dirPath);
			for (int j = 0; j < subDirPaths.Length; j++) {
				assetList.AddRange(GetSubFolderAssets<T>(subDirPaths[j]));
			}
		}
		return assetList;
	}

	private static T GetAssetAtPath<T>(string path) where T : Object {
        Object[] assetObjs = AssetDatabase.LoadAllAssetsAtPath(path);
		for (int j = 0; j < assetObjs.Length; j++) {
			Object assetObj = assetObjs[j];
			//Debug.Log("[EditorHelper] GetDirectoryAssets - assetObj: " + assetObj);
			//if (assetObj != null && assetObj.GetType() == typeof(T)) {
			if (assetObj != null && assetObj is T) {
				T asset = (T)assetObj;
				return asset;
			}
		}
		return null;
	}

	
}
#endif