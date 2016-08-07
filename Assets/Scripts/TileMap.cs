using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class TileMap {
	//Stores level data - values should never be changed
	[SerializeField] private List<TileDefinition> tileDefList;
	public List<TileDefinition> TileDefList { get { return tileDefList;} }
	[SerializeField] private List<TileObjectDefintion> toDefList;
	public List<TileObjectDefintion> TODefList { get { return toDefList;} }

	//Used in editor mode, and when loading level. Changes to color etc. applies to tile defintions.
	[SerializeField] private List<Tile> tileList;
	public List<Tile> TileList { get { return tileList;} }
	[SerializeField] private List<TileObject> toList;
	public List<TileObject> TOList { get { return toList;} }
	private Dictionary<Vec2i, Tile> tileDict;

	public void MakeDict() {
		tileDict = new Dictionary<Vec2i, Tile>();
		foreach (Tile tile in tileList) {
			tileDict.Add(tile.Pos, tile);
		}
	}

	public void PrepareForBuild() {
		tileDefList.Clear();
		foreach (var item in tileList) {
			tileDefList.Add(item.TileDef);
			if (Application.isPlaying) {
				GameObject.Destroy(item.gameObject);
			} else {
				GameObject.DestroyImmediate(item.gameObject);
			}
		}
		tileList.Clear();

		toDefList.Clear();
		foreach (var item in toList) {
			toDefList.Add(item.ToDef);
			if (Application.isPlaying) {
				GameObject.Destroy(item.gameObject);
			} else {
				GameObject.DestroyImmediate(item.gameObject);
			}
		}
		toList.Clear();

		//System.Type.GetType
	}
	
	public void CleanLists() {
		List<Tile> newTileList = new List<Tile>();
		foreach (var tile in tileList) if (tile != null) newTileList.Add(tile);
		tileList = newTileList;

		List<TileObject> newTOList = new List<TileObject>();
		foreach (var to in toList) if (to != null) newTOList.Add(to);
		toList = newTOList;
	}


	#region Tiles
	public void UnloadMap() { //TODO use pool instead of deleting
		foreach (var item in tileList) {
			GameObject.Destroy(item.gameObject);
		}
		tileList.Clear();
		tileDict = null;

		foreach (var item in toList) {
			GameObject.Destroy(item.gameObject);
		}
		toList.Clear();
	}

	public void AddTile(Tile tile) {
		tileList.Add(tile);
	}

	public bool IsValidTile(Vec2i pos) {
		return GetTile(pos) != null;
	}

	public Tile GetTile(Vec2i pos) {
		if (Application.isPlaying) {
			if (tileDict == null) {
				Debug.Log("why u not create already!");
				MakeDict();
			}
			return tileDict.ContainsKey(pos) ? tileDict[pos] : null;
		} else {
			foreach (Tile tile in tileList) {
				if (tile.Pos == pos) return tile;
			}
		}
		//Debug.Log("[TileMap] Could not find tile at pos: " + pos);
		return null;
	}
	public void SetTile(Vec2i pos, TileDefinition def) {
		Tile tile = GetTile(pos);
		if (tile != null) {
			tile.Set(def);
		} else {
			Debug.LogError("[TileMap] SetTile, could not find tile at pos: " + pos);
		}
	}
	public Tile GetTileOfType(TileType tileType) {
		foreach (Tile tile in tileList) {
			if (tile.TileDef.type == tileType) return tile;
		}
		Debug.Log("[TileMap] Could not find tile of type: " + tileType);
		return null;
	}

	public void DeleteTileAt(Vec2i pos) {
		if (IsValidTile(pos)) {
			Tile tile = GetTile(pos);
			tileList.Remove(tile);
#if UNITY_EDITOR
			Undo.DestroyObjectImmediate(tile.gameObject);
#endif
		}
	}
	#endregion Tiles

	#region TileObjects
	public void AddTileObject(TileObject to) {
		toList.Add(to);
	}
	public List<TileObject> GetTOAtPos(Vec2i pos) {
		List<TileObject> toAtPosList = new List<TileObject>();
		foreach (var to in toList) {
			if (to.ToDef.pos == pos) toAtPosList.Add(to);
		}
		return toAtPosList;
	}

	public bool HasTOTypeAtPos(Vec2i pos, Type typ) {
		foreach (var to in toList) {
			if (to.ToDef.pos == pos && to.GetType() == typ) return true;
		}
		return false;
	}

	public T GetTOOfType<T>() where T : TileObject {
		foreach (TileObject to in toList) if (to.GetType() == typeof(T)) return (T) to;
		return null;
	}

	public List<T> GetAllTOOfType<T>() where T : TileObject {
		List<T> list = new List<T>();
		foreach (TileObject to in toList) {
			if (to.GetType() == typeof(T)) list.Add((T)to);
		}
		return list;
	}

	public void DeleteTOAtPos(Vec2i pos, TileObject to) {
		foreach (var other in GetTOAtPos(pos)) {
			if (to == other) toList.Remove(to);
#if UNITY_EDITOR
			Undo.DestroyObjectImmediate(to.gameObject);
#else
			Destroy(other);
#endif
			return;
		}
	}

	public void DeleteAllTOAtPos(Vec2i pos) {
		foreach (var to in GetTOAtPos(pos)) {
			toList.Remove(to);
#if UNITY_EDITOR
			Undo.DestroyObjectImmediate(to.gameObject);
#endif
		}
	}



#endregion TileObjects
}