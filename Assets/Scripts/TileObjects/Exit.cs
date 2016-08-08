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
		if (def.GetType() == typeof(ExitDefinition)) {
			ExitDefinition newDef = (ExitDefinition)def;
			_exitDef.color = newDef.color;
		}
		Refresh();
	}

	public override TileObjectInteractionResult PlayerEntered() {
		return new TileObjectInteractionResult(TileObjectInteractionResultType.Exit);
	}

	protected override void Refresh() {
		if (_mainSR != null) _mainSR.color = SpriteLibrary.GetTileColor(_exitDef.color);
	}
}

[System.Serializable]
public class ExitDefinition : TileObjectDefintion{
	public TileColor color;
}