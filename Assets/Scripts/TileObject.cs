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

}

[System.Serializable]
public class TileObjectDefintion {
	public Vec2i pos;
	public string className;
}

//[System.Serializable]
//public class TileObjectDefintionHolder : ScriptableObject { 
////public class TileObjectDefintionHolder<T> : ScriptableObject where T : TileObjectDefintion, new(){
//	public TileObjectDefintionHolder def;

//	void OnEnable() {
//        hideFlags = HideFlags.DontSave;
//        if (def == null) {
//			def = new TileObjectDefintionHolder();
//		}
//    }
//}