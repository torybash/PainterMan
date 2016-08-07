using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Exit : TileObject {

	[SerializeField] ExitDefinition _exitDef;

	public override TileObjectDefintion ToDef {
		get {return _exitDef;}
		set {_exitDef = (ExitDefinition) value;}
	}

	public override void Set(TileObjectDefintion def) {
		base.Set(def);
	}

	public override TileObjectInteractionResult PlayerEntered() {
		return new TileObjectInteractionResult(TileObjectInteractionResultType.Exit);
	}
}

[System.Serializable]
public class ExitDefinition : TileObjectDefintion{
	
}