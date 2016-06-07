using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {

	public TileDefinition tileDef;

	public Vec2i pos;

	private SpriteRenderer sr;
	private SpriteRenderer Sr{
		get{ 
			if (sr == null) sr = GetComponent<SpriteRenderer>();
			return sr;
		}
	}

	public void Set(TileType tileType, TileColor color) {
		tileDef.type = tileType;
		tileDef.color = color;
		Refresh();
	}

	public void Refresh(){
		if (tileDef.type == TileType.Empty){
			//Sr.enabled = false;	
			Debug.LogError("Empty tileType?!");
		}else {
			Sr.enabled = true;	
			Sr.sprite = SpriteLibrary.GetTileSprite(tileDef.type);
			Sr.color = SpriteLibrary.GetTileColor(tileDef.color);
		}
	}


	void Update(){ //TODO DO SOMEWHERE ELSE (NOT IN UPDATE!!!)
		if (tileDef.color != TileColor.None && tileDef.type != TileType.Bucket){
			if (tileDef.paintedTurn + 5 <= Game.I.Turn){
				Sr.color = SpriteLibrary.GetTileColor(tileDef.color);

			}else{
				float fracDry = 1f - (tileDef.paintedTurn + 5 - Game.I.Turn) / 5f;
				Sr.color = SpriteLibrary.GetTileColor(tileDef.color);
				Sr.color = new Color(Sr.color.r*fracDry, Sr.color.g*fracDry, Sr.color.b*fracDry);
			}

		}
	}

	public override string ToString ()
	{
		return string.Format ("[Tile] tileDef, color: " + tileDef.color + ", paintedTurn: " + tileDef.paintedTurn + ", type: " + tileDef.type);
	}

	#if UNITY_EDITOR
	void OnValidate(){
		Refresh();
	}

	#endif

}
