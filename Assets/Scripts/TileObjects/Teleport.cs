using UnityEngine;
using System.Collections;
using System;

public class Teleport : TileObject {

	[SerializeField] TeleportDefinition def;

	public override TileObjectDefintion ToDef {
		get {return def;}
		set {def = (TeleportDefinition)value;}
	}

	public override void Set(TileObjectDefintion def) {
		base.Set(def);
	}
}


[System.Serializable]
public class TeleportDefinition : TileObjectDefintion{
	public int teleportCycleIdx;
}