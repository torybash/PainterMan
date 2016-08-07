using UnityEngine;
using System.Collections.Generic;

public abstract class TileObject : MonoBehaviour{


	public abstract TileObjectDefintion ToDef { get; set; }

	public virtual void Set(TileObjectDefintion def) {
		ToDef.pos = def.pos;
		ToDef.className = def.className;
	}



	public virtual TileObjectInteractionResult PlayerEntered() {
		return TileObjectInteractionResult.Empty();
	}

	public virtual void Init() {}
	public virtual void UpdateTO() {}
}

[System.Serializable]
public class TileObjectDefintion {
	[ReadOnly]
	public Vec2i pos;
	[ReadOnly]
	public string className;
}



public class TileObjectInteractionResult {

	public TileObjectInteractionResultType type;
	public Vec2i pos;
	public TileColor color;

	public TileObjectInteractionResult(TileObjectInteractionResultType type, Vec2i pos = default(Vec2i), TileColor color = TileColor.None) {
		this.type = type;
		this.pos = pos;
		this.color = color;
	}

	public static TileObjectInteractionResult Empty() {
		return new TileObjectInteractionResult(TileObjectInteractionResultType.None);
	}
}

public enum TileObjectInteractionResultType {
	None,
	Kill,
	Exit,
	Teleport,
	PickupColor
}