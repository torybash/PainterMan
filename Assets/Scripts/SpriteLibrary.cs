using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SpriteLibrary : Library<SpriteLibrary> {

	[System.Serializable]
	public class SpriteDefinition {
		public string name;
		public Sprite sprite;
	}
	[System.Serializable]
	public class TileSpriteDefinition{
		public TileType type;
		public Sprite sprite;
	}

	[System.Serializable]
	public class ColorDefinition{
		public TileColor colorType;
		public Color color;
	}

	[SerializeField] List<SpriteDefinition> allSpritesList;

	[SerializeField] List<TileSpriteDefinition> tileSpriteList;
	[SerializeField] List<ColorDefinition> tileColorList;

	public static Sprite GetSprite(string name) {
		UnityEngine.Debug.Log("GetSprite - name: " + name + ", I.allSpritesList.Exists((elem) => elem.name == name): "+ (I.allSpritesList.Exists((elem) => elem.name == name)));

		if (!I.allSpritesList.Exists((elem) => elem.name == name)) return null;
		return I.allSpritesList.First((elem) => elem.name == name).sprite;
	}

	public static Sprite GetTileSprite(TileType type){
		if (!I.tileSpriteList.Exists((elem) => elem.type == type)) return null;
		return I.tileSpriteList.First((elem) => elem.type == type).sprite;
	}


	public static Color GetTileColor(TileColor colorType){
		if (!I.tileColorList.Exists((elem) => elem.colorType == colorType)) return Color.white;
		return I.tileColorList.First((elem) => elem.colorType == colorType).color;
		//		var val = from elem in I.tileSpriteList where elem.type == type select elem;
	}



#if UNITY_EDITOR
	[SerializeField] DefaultAsset spritesDir;

	void OnValidate() {
		if (spritesDir != null) {
			if (allSpritesList == null) allSpritesList = new List<SpriteDefinition>();
			allSpritesList.Clear();

			List<Sprite> spriteList = GetDirectoryAssets<Sprite>(spritesDir);
			foreach (var spr in spriteList) {
				allSpritesList.Add(new SpriteDefinition { name = spr.name, sprite = spr });
			}
		}
	}

	 private List<T> GetDirectoryAssets<T>(DefaultAsset dir)  where T : Object {
        List<T> assetList = new List<T>();
        string path = AssetDatabase.GetAssetPath(dir);
        string[] filePaths = System.IO.Directory.GetFiles(path);
        for (int i = 0; i < filePaths.Length; i++) {
            Object[] assetObjs = AssetDatabase.LoadAllAssetsAtPath(filePaths[i]);
			for (int j = 0; j < assetObjs.Length; j++) {
				Object assetObj = assetObjs[j];
				UnityEngine.Debug.Log("assetObj: " + assetObj);
				if (assetObj != null && assetObj.GetType() == typeof(T)) {
					T asset = (T)assetObj;
					assetList.Add(asset);
				}
			}
        }
        return assetList;
    }
#endif

}


