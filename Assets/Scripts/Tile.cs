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
	private SpriteRenderer TileSR {
		get {
			if (sr == null) sr = GetComponent<SpriteRenderer>();
			return sr;
		}
	}

	//[SerializeField] SpriteRenderer paintSR;
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
			//paintSR.enabled = false;
			Debug.LogError("Empty tileType?!");
		}else {
			//paintSR.enabled = true;
			//paintSR.color = SpriteLibrary.GetTileColor(tileDef.color);
			TileSR.sprite = SpriteLibrary.GetTileSprite(tileDef.type);
			TileSR.color = SpriteLibrary.GetTileColor(tileDef.color);
			indicatorSR.color = SpriteLibrary.GetTileColor(tileDef.goalColor);
		}

		RefreshPaintColor();
	}


	void Update(){ //TODO DO SOMEWHERE ELSE (NOT IN UPDATE!!!)
		RefreshPaintColor();
	}

	private void RefreshPaintColor() {
		if (tileDef.color != TileColor.None && tileDef.paintedTurn >= 0) { //&& tileDef.type != TileType.Bucket){
			if (tileDef.paintedTurn + GameRules.GetTimeToDry(tileDef.color) <= Game.I.Turn){
				if (GameRules.PaintDisappearInsteadOfDrying && tileDef.type != TileType.Bucket) {
					tileDef.color = TileColor.None;
					Refresh();
				} else {
					TileSR.color = SpriteLibrary.GetTileColor(tileDef.color);
					//paintSR.color = SpriteLibrary.GetTileColor(tileDef.color);
				}
			}else{
				float fracDry = 1f - ((tileDef.paintedTurn + GameRules.GetTimeToDry(tileDef.color) < Game.I.Turn) ? 0f : 0.5f);

				if (GameRules.PaintGradient) {
					fracDry = 1f - (Game.I.Turn - 1 - tileDef.paintedTurn) / (float) GameRules.GetTimeToDry(tileDef.color);
				}
				Debug.Log("fracDry: "+ fracDry + ", tileDef.paintedTurn: " + tileDef.paintedTurn + ", Game.I.Turn: " + Game.I.Turn + ", GameRules.GetTimeToDry(tileDef.color): " + GameRules.GetTimeToDry(tileDef.color));

				TileSR.color = SpriteLibrary.GetTileColor(tileDef.color);
				TileSR.color = new Color(TileSR.color.r * fracDry, TileSR.color.g * fracDry, TileSR.color.b * fracDry);
				//paintSR.color = SpriteLibrary.GetTileColor(tileDef.color);
				//paintSR.color = new Color(paintSR.color.r*fracDry, paintSR.color.g*fracDry, paintSR.color.b*fracDry);
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
