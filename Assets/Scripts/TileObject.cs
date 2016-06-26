using UnityEngine;
using System.Collections;

public abstract class TileObject : MonoBehaviour{

	public abstract TileObjectDefintion ToDef { get; set; }

	public virtual void Set(TileObjectDefintion def) {
		ToDef.pos = def.pos;
		ToDef.className = def.className;
	}

	public virtual void UpdateTO() {

	}

	public virtual TileObjectInteractionResult PlayerEntered() {
		return TileObjectInteractionResult.Empty;
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
	public bool kill;

	public static TileObjectInteractionResult Empty {
		get {
			var result = new TileObjectInteractionResult();
			return result;
		}
	}
}