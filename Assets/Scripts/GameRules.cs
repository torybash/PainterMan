using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameRules : Library<GameRules> {

	[SerializeField] private ColorDefinitions colorDefinitions;

	[System.Serializable]
	public class ColorDefinitions {
		public List<ColorDef> defList;
		[System.Serializable]
		public class ColorDef {
			public TileColor clr;
			public int timeToDry;
		}
	}


	public int GetTimeToDry(TileColor clr) {
		foreach (var item in colorDefinitions.defList) {
			if (item.clr == clr) return item.timeToDry; 
		}
		Debug.LogError("GetTimeToDry - could not find val for color: " + clr);
		return -1;
	}
}
