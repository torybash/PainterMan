using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PaintBucket : TileObject {


	[SerializeField] private PaintBucketDefinition _paintBucketDef;
	[SerializeField] private SpriteRenderer _bucketRend;

	public override TileObjectDefintion ToDef {
		get {
			return _paintBucketDef;
		}
		set {_paintBucketDef = (PaintBucketDefinition) value;}
	}

	public override void Set(TileObjectDefintion def) {
		base.Set(def);
		if (def.GetType() == typeof(PaintBucketDefinition)) {
			PaintBucketDefinition newDef = (PaintBucketDefinition)def;
			_paintBucketDef.color = newDef.color;
		}
		Refresh();
	}

	public override TileObjectInteractionResult PlayerEntered() {
		return new TileObjectInteractionResult(TileObjectInteractionResultType.PickupColor, color:_paintBucketDef.color);
	}

	private void Refresh() {
		if (_bucketRend != null) _bucketRend.color = SpriteLibrary.GetTileColor(_paintBucketDef.color);
	}

	

	void OnValidate() {
#if UNITY_EDITOR

		//if (_soDef == null) _soDef = new TileObjectDefintion() ;
		//Debug.Log("Paintbucket OnValidate - _soDef: " + _soDef);
		//Refresh();
#endif
	}
}

[System.Serializable]
public class PaintBucketDefinition : TileObjectDefintion{
	public TileColor color;
}