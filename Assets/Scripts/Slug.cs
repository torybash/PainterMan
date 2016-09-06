using UnityEngine;
using System.Collections;

public class Slug : MonoBehaviour {

	public Vec2i pos;
	public TileColor color;
	public bool isOnExit;


	[SerializeField] SpriteRenderer sr;

	

	public void SetPosition(Vec2i pos) {
		this.pos = pos;
		transform.position = (Vector3)GameHelper.TileToWorldPos(pos) + Game.I.Lvl.transform.position;
	}

	public void SetColor(TileColor color) {
		this.color = color;
		sr.color = SpriteLibrary.GetTileColor(color);
	}
}
