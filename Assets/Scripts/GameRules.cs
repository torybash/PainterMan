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



	[Header("Game logic")]
	[SerializeField] bool removeBucketsOnEnter;
	public static bool RemoveBucketsOnEnter { get { return I.removeBucketsOnEnter; } }
	[SerializeField] bool turnsCountWhenSliding;
	public static bool TurnsCountWhenSliding { get { return I.turnsCountWhenSliding; } }
	[SerializeField] bool paintDisappearInsteadOfDrying;
	public static bool PaintDisappearInsteadOfDrying { get { return I.paintDisappearInsteadOfDrying; } }
	[SerializeField] bool paintBucketTiles;
	public static bool PaintBucketTiles { get { return I.paintBucketTiles; } }
	[SerializeField] bool normalTilesCausesSlide;
	public static bool NormalTilesCausesSlide { get { return I.normalTilesCausesSlide; } }
	[SerializeField] bool paintBehindPlayer;
	public static bool PaintBehindPlayer { get { return I.paintBehindPlayer; } }


	[Header("Defintion of colors")]
	[SerializeField] private ColorDefinitions colorDefinitions;


	[Header("Animation settings")]
	[SerializeField] float animDurationPrTile = 0.2f;
	public static float AnimDurationPrTile { get { return I.animDurationPrTile; }}


	public static int GetTimeToDry(TileColor clr) {
		foreach (var item in I.colorDefinitions.defList) {
			if (item.clr == clr) return item.timeToDry; 
		}
		Debug.LogError("GetTimeToDry - could not find val for color: " + clr);
		return -1;
	}
}
