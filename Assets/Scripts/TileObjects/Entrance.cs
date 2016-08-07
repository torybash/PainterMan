using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Entrance : TileObject {

	[SerializeField] EntranceDefinition _entranceDef;

	public override TileObjectDefintion ToDef {
		get {return _entranceDef;}
		set {_entranceDef = (EntranceDefinition) value;}
	}
}

[System.Serializable]
public class EntranceDefinition : TileObjectDefintion{
	
}