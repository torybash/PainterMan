using UnityEngine;
using System.Collections;
using System;

public class Spikes : TileObject {
	
	[SerializeField] SpikesDefintion spikesDef;

	[SerializeField] SpriteRenderer spikesRend;

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

	private void Refresh() {
		if (spikesDef == null || spikesRend == null) return;
		if (spikesDef.isRaised) spikesRend.sprite = SpriteLibrary.GetSprite(raisedSpriteName);
		else spikesRend.sprite = SpriteLibrary.GetSprite(loweredSpriteName);
	}


	
	#if UNITY_EDITOR
	void OnValidate(){
		//Refresh();
	}
	#endif
}


[System.Serializable]
public class SpikesDefintion : TileObjectDefintion{
	public bool isRaised;
}