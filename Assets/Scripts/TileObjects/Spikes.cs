using UnityEngine;
using System.Collections;
using System;

public class Spikes : TileObject {
	
	[SerializeField] SpikesDefintion spikesDef;

	//[SerializeField] SpriteRenderer spikesRend;

	private const string raisedSpriteName = "to_SpikesUp";
	private const string loweredSpriteName = "to_SpikesDown";

	public override TileObjectDefintion ToDef {
		get {return spikesDef;}
		set {spikesDef = (SpikesDefintion) value;}
	}

	public override void Set(TileObjectDefintion def) {
		base.Set(def);
		if (def.GetType() == typeof(SpikesDefintion)) {
			SpikesDefintion newSpikesDef = (SpikesDefintion)def;
			spikesDef.isRaised = newSpikesDef.isRaised;
		}
		Refresh();
	}

	public override void UpdateTO() {
		spikesDef.isRaised = !spikesDef.isRaised;
		Refresh();
	}

	public override TileObjectInteractionResult PlayerEntered() {
		if (spikesDef.isRaised) {
			return new TileObjectInteractionResult(TileObjectInteractionResultType.Kill);
		}
		return TileObjectInteractionResult.Empty();
	}

	protected override void Refresh() {
		if (spikesDef == null || _mainSR == null) return;
		if (spikesDef.isRaised) _mainSR.sprite = SpriteLibrary.GetSprite(raisedSpriteName);
		else _mainSR.sprite = SpriteLibrary.GetSprite(loweredSpriteName);
	}

}


[System.Serializable]
public class SpikesDefintion : TileObjectDefintion{
	public bool isRaised;
}