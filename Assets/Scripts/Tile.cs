using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : ProBehaviour {

	[SerializeField] private TileDefinition tileDef;
	public TileDefinition TileDef {
		get { return tileDef; }
	}

	public Vec2i Pos {
		get { return tileDef.pos; }
	}

	private SpriteRenderer sr;
	private SpriteRenderer Sr{
		get{ 
			if (sr == null) sr = GetComponent<SpriteRenderer>();
			return sr;
		}
	}

	[SerializeField] SpriteRenderer indicatorSR;


	public void Set(TileDefinition def) {
		tileDef.color = def.color;
		tileDef.goalColor = def.goalColor;
		tileDef.type = def.type;
		tileDef.pos = def.pos;
		tileDef.paintedTurn = def.paintedTurn;
		Refresh();
	}

	public void Refresh(){
		//Log("Hello!");
		if (tileDef == null) return;

		if (tileDef.type == TileType.Empty){
			//Sr.enabled = false;	
			Debug.LogError("Empty tileType?!");
		}else {
			Sr.enabled = true;	
			Sr.sprite = SpriteLibrary.GetTileSprite(tileDef.type);
			Sr.color = SpriteLibrary.GetTileColor(tileDef.color);
			indicatorSR.color = SpriteLibrary.GetTileColor(tileDef.goalColor);
		}

		RefreshPaintColor();
	}


	void Update(){ //TODO DO SOMEWHERE ELSE (NOT IN UPDATE!!!)
		RefreshPaintColor();
	}

	private void RefreshPaintColor() {
		if (tileDef.color != TileColor.None && tileDef.paintedTurn >= 0) { //&& tileDef.type != TileType.Bucket){
			if (tileDef.paintedTurn + 5 <= Game.I.Turn){
				Sr.color = SpriteLibrary.GetTileColor(tileDef.color);
			}else{
				float fracDry = 1f - ((tileDef.paintedTurn + GameRules.I.GetTimeToDry(tileDef.color) < Game.I.Turn) ? 0f : 0.5f);
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
