using UnityEngine;
using System.Collections;
using System;

public class Gate : TileObject {

	[SerializeField] GateDefintion def;

	public override TileObjectDefintion ToDef {
		get {return def;}
		set {def = (GateDefintion) value;}
	}
}

[System.Serializable]
public class GateDefintion : TileObjectDefintion{
	public bool isOpen;
}
