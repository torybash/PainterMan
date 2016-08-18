using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Entrance : TileObject {

	[SerializeField] SpriteRenderer _colorMarkerSR;

	[SerializeField] EntranceDefinition _entranceDef;

	public override TileObjectDefintion ToDef {
		get {return _entranceDef;}
		set {_entranceDef = (EntranceDefinition) value;}
	}

	public override void Set(TileObjectDefintion def) {
		base.Set(def);
		if (def.GetType() == typeof(EntranceDefinition)) {
			EntranceDefinition newDef = (EntranceDefinition)def;
			_entranceDef.color = newDef.color;
		}
		Refresh();
	}

	protected override void Refresh() {
		if (_colorMarkerSR != null) {
			if (_entranceDef.color == TileColor.None) {
				_colorMarkerSR.enabled = false;
			} else {
				_colorMarkerSR.enabled = true;
				_colorMarkerSR.color = SpriteLibrary.GetTileColor(_entranceDef.color);
			}
		}
	}
}

[System.Serializable]
public class EntranceDefinition : TileObjectDefintion{
	public TileColor color;
}