using UnityEngine;
using System.Collections.Generic;

public abstract class TileObject : MonoBehaviour{

	public abstract TileObjectDefintion ToDef { get; set; }

	public virtual void Set(TileObjectDefintion def) {
		ToDef.pos = def.pos;
		ToDef.className = def.className;
	}

	public virtual void UpdateTO() {

	}

	public virtual TileObjectInteractionResult PlayerEntered() {
		return TileObjectInteractionResult.Empty();
	}

	public virtual void Init() {

	}

}

[System.Serializable]
public class TileObjectDefintion {
	[ReadOnly] public Vec2i pos;
	[ReadOnly] public string className;
}



[System.Serializable]
public class TileObjectInteractionResult {

	public TileObjectInteractionResultType type;
	public Vec2i position;

	public static TileObjectInteractionResult Empty() {
		return new TileObjectInteractionResult();
	}

	public static TileObjectInteractionResult TeleportResult(Vec2i endPos) {
		return new TileObjectInteractionResult { type = TileObjectInteractionResultType.Teleport, position = endPos };
	}

	public static TileObjectInteractionResult KillResult() {
		return new TileObjectInteractionResult { type = TileObjectInteractionResultType.Kill};
	}
}

[System.Serializable]
public enum TileObjectInteractionResultType {
	None,
	Kill,
	Teleport
}