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
	[SerializeField] bool cantMoveOverOtherColors;
	public static bool CantMoveOverOtherColors { get { return I.cantMoveOverOtherColors; } }
	[SerializeField] bool removeBucketsOnEnter;
	public static bool RemoveBucketsOnEnter { get { return I.removeBucketsOnEnter; } }
	[SerializeField] bool turnsCountWhenSliding;
	public static bool TurnsCountWhenSliding { get { return I.turnsCountWhenSliding; } }
	[SerializeField] bool paintDisappearInsteadOfDrying;
	public static bool PaintDisappearInsteadOfDrying { get { return I.paintDisappearInsteadOfDrying; } }
	[SerializeField] bool paintNeedsToBeDry;
	public static bool PaintNeedsToBeDry { get { return I.paintNeedsToBeDry; } }
	[SerializeField] bool paintBucketTiles;
	public static bool PaintBucketTiles { get { return I.paintBucketTiles; } }
	[SerializeField] bool normalTilesCausesSlide;
	public static bool NormalTilesCausesSlide { get { return I.normalTilesCausesSlide; } }
	[SerializeField] bool paintBehindPlayer;
	public static bool PaintBehindPlayer { get { return I.paintBehindPlayer; } }

	[SerializeField] bool updateSpikesBeforeMove;
	public static bool UpdateSpikesBeforeMove { get { return I.updateSpikesBeforeMove; } }
	[SerializeField] bool removeSlugsWhenTouchExit;
	public static bool RemoveSlugsWhenTouchExit { get { return I.removeSlugsWhenTouchExit; } }


	[Header("Defintion of colors")]
	[SerializeField] private ColorDefinitions colorDefinitions;


	[Header("Animation settings")]
	[SerializeField] float animDurationPrTile = 0.2f;
	public static float AnimDurationPrTile { get { return I.animDurationPrTile; }}
	[SerializeField] bool paintGradient;
	public static bool PaintGradient { get { return I.paintGradient; }}

	public static int GetTimeToDry(TileColor clr) {
		foreach (var item in I.colorDefinitions.defList) {
			if (item.clr == clr) return item.timeToDry; 
		}
		Debug.LogError("GetTimeToDry - could not find val for color: " + clr);
		return -1;
	}
}
