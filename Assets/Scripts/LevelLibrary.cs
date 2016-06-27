using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelLibrary : Library<LevelLibrary> {

	[SerializeField] List<Level> levelList;

	
	public Level InstantiateLevel(int lvlIdx){
		if (lvlIdx >= levelList.Count) { Debug.LogError("lvlIdx invalid:" + lvlIdx); return null; }
		string name = levelList[lvlIdx].gameObject.name;
		Level lvl = Instantiate(levelList[lvlIdx]);
		lvl.gameObject.name = name;
		return lvl;

		//currLevelIdx = lvlIdx;
		//currLvl = levelList[lvlIdx];
		//currLvl.LoadTiles();

		//GameUI.I.OpenMenu();

		//Camera.main.transform.position = (Vector3)currLvl.GetCenterPos() - Vector3.forward * 10f;

		//StartLevel();
	}

	public string GetLevelTitle(int lvlIdx) {
		if (lvlIdx >= levelList.Count) { Debug.LogError("lvlIdx invalid:" + lvlIdx); return null; }
		return levelList[lvlIdx].gameObject.name;
	}

	public int GetLevelCount() {
		return levelList.Count;
	}



	#if UNITY_EDITOR
	[SerializeField] DefaultAsset levelDir;
	void OnValidate() {
		if (levelDir != null) {
			levelList.Clear();
			foreach (var to in EditorHelper.GetDirectoryAssets<Level>(levelDir, true)) {
				levelList.Add(to);
			}
		}
	}
#endif
}
