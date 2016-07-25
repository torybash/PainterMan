using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameRules : Library<GameRules> {

	[System.Serializable]
	public class ColorDefinitions {
		public List<ColorDef> defList;
		[System.Serializable]
		public class ColorDef {
			public TileColor clr;
			public int timeToDry;
		}
	}

	[SerializeField] bool paintBucketTiles;
	public static bool PaintBucketTiles {
		get { return I.paintBucketTiles; }
	}
	[SerializeField] bool normalTilesCausesSlide;
	public static bool NormalTilesCausesSlide {
		get { return I.normalTilesCausesSlide; }
	}
	[SerializeField] bool paintBehindPlayer;
	public static bool PaintBehindPlayer {
		get { return I.paintBehindPlayer; }
	}

	[SerializeField] private ColorDefinitions colorDefinitions;



	public static int GetTimeToDry(TileColor clr) {
		foreach (var item in I.colorDefinitions.defList) {
			if (item.clr == clr) return item.timeToDry; 
		}
		Debug.LogError("GetTimeToDry - could not find val for color: " + clr);
		return -1;
	}
}
