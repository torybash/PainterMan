using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class SpriteLibrary : Library<SpriteLibrary> {

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

	[SerializeField] List<TileSpriteDefinition> tileSpriteList;
	[SerializeField] List<ColorDefinition> tileColorList;


	public static Sprite GetTileSprite(TileType type){
		if (!I.tileSpriteList.Exists((elem) => elem.type == type)) return null;
		return I.tileSpriteList.First((elem) => elem.type == type).sprite;
		//		var val = from elem in I.tileSpriteList where elem.type == type select elem;
	}

	public static Color GetTileColor(TileColor colorType){
		if (!I.tileColorList.Exists((elem) => elem.colorType == colorType)) return Color.white;
		return I.tileColorList.First((elem) => elem.colorType == colorType).color;
		//		var val = from elem in I.tileSpriteList where elem.type == type select elem;
	}



}


