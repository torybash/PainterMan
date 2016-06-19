using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Object = UnityEngine.Object;

public class PrefabLibrary : Library<PrefabLibrary> {

	[SerializeField] Tile tilePrefab;
	[SerializeField] TileObject[] tileObjectPrefabs;
	public TileObject[] TileObjectPrefabs { get { return tileObjectPrefabs; } }

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
}
