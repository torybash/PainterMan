using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Object = UnityEngine.Object;

public class PrefabLibrary : Library<PrefabLibrary> {

	[SerializeField] Tile tilePrefab;
	[SerializeField] List<TileObject> tileObjectPrefabs;
	public List<TileObject> TileObjectPrefabs { get { return tileObjectPrefabs; } }

	public Tile GetTileInstance() {
		return InstantiatePrefab<Tile>(tilePrefab) ;
	}

	public TileObject GetTileObject<T>(T to) where T : TileObject {
		Debug.Log("GetTileObject<T>");
		foreach (var toPrefab in tileObjectPrefabs) {
			Debug.Log("toPrefab.GetType(): " + toPrefab.GetType() + ", typeof(T): "+ typeof(T));
			if (toPrefab.GetType() == to.GetType()) {
				return InstantiatePrefab<TileObject>(toPrefab) ;
			}
		}
		Debug.LogError("GetTileObject error! Can't find type: " + typeof(T));
		return null;
	}

	public TileObject GetTileObject(Type typ) {
		Debug.Log("GetTileObject");
		foreach (var toPrefab in tileObjectPrefabs) {
			Debug.Log("toPrefab.GetType(): " + toPrefab.GetType() + ", typ: "+ typ);
			if (toPrefab.GetType() == typ) {
				return InstantiatePrefab<TileObject>(toPrefab) ;
			}
		}
		Debug.LogError("GetTileObject error! Can't find type: " + typ);
		return null;
	}

	private T InstantiatePrefab<T>(T prefab) where T : Object {
		if (!Application.isPlaying) {
#if UNITY_EDITOR
			return (T) PrefabUtility.InstantiatePrefab(prefab);	
#endif
		}
		return Instantiate<T>(prefab);
	}


#if UNITY_EDITOR
	[SerializeField] DefaultAsset prefabDir;
	void OnValidate() {
		if (prefabDir != null) {
			//tilePrefab, tileObjectPrefabs
			if (tilePrefab == null) tilePrefab = EditorHelper.GetFirstDirectoryAsset<Tile>(prefabDir);

			tileObjectPrefabs.Clear();
			foreach (var to in EditorHelper.GetDirectoryAssets<TileObject>(prefabDir, true)) {
				//Debug.Log("to: " + to);
				tileObjectPrefabs.Add(to);
				//allSpritesList.Add(new SpriteDefinition { name = spr.name, sprite = spr });
			}
		}
	}
#endif
}
