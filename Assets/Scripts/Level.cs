using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Security.Cryptography;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Level : MonoBehaviour {

	[SerializeField] GameObject tilePrefab;
	[SerializeField] List<GameObject> tileList;
	[SerializeField] GameObject tileCont;

//	TileDefinition[][] tileMap;
	Tile[][] tileMap;

	[SerializeField] int width, height;


	public void Init(){
		ReadTiles();
	}

	public Vec2i GetStartPos(){

		for (int x = 0; x < tileMap.Length; x++) {
			for (int y = 0; y < tileMap[x].Length; y++) {
				if (tileMap[x][y].tileDef.type == TileType.Start) return new Vec2i(x, y);
			}
		}
		Debug.LogError("Level has not start pos! - returning zero");
		return Vec2i.Zero;
	}

	public bool IsValidTile(Vec2i pos){
		if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height){
			return tileMap[pos.x][pos.y].tileDef.type != TileType.Empty;
		}
		return false;
	}

	public TileType GetTileType(Vec2i pos){
		if (IsValidTile(pos)){
			return tileMap[pos.x][pos.y].tileDef.type;
		}
		return TileType.Empty;
	}
	public TileColor GetTileColorType(Vec2i pos){
		if (IsValidTile(pos)){
			return tileMap[pos.x][pos.y].tileDef.color;
		}
		return TileColor.None;
	}

	public void PaintTile(Vec2i pos, TileColor tileColor, int turn){
		if (IsValidTile(pos)){
			Debug.Log("[Level] PaintTile - pos: "+ pos + ", tileColor: "+ tileColor + ", turn: "+ turn);

			tileMap[pos.x][pos.y].tileDef.color = tileColor;
			tileMap[pos.x][pos.y].tileDef.paintedTurn = turn;

			tileMap[pos.x][pos.y].Refresh();
		}
	}

	public bool IsWalkable(Vec2i pos, int turn){
		Debug.Log("[Level] IsWalkable - pos: "+ pos + ", turn: "+ turn + ", tile: "+ tileMap[pos.x][pos.y] + ", is color dry?: "+ IsColorDry(pos, turn));
		return tileMap[pos.x][pos.y].tileDef.color == TileColor.None || IsColorDry(pos, turn);
	}
	public bool IsColorDry(Vec2i pos, int turn){
		if (IsValidTile(pos)){
			if (tileMap[pos.x][pos.y].tileDef.type == TileType.Bucket) {
				return true;
			}
			if (tileMap[pos.x][pos.y].tileDef.color != TileColor.None && tileMap[pos.x][pos.y].tileDef.paintedTurn + 5 <= turn){
//				Debug.Log("[Level] Color IS dry!");
				return true;
			}
//			Debug.Log("[Level] Color IS NOT dry..");
			return false;
		}
		Debug.LogError("IsColorDry invalid pos!");
		return false;
	}
	
	#region Editor
	public void CreateTiles(){
		//		int w = 10, h = 10;height
		int w = this.width; int h = this.height;

//		tiles[x][y] = new Tile();


		foreach (var item in tileList) {
			if (item) DestroyImmediate(item);
		}
		tileList.Clear();

		if (tileCont == null) tileCont = new GameObject("TileContainer");
		tileCont.transform.SetParent(transform);
		tileCont.transform.localScale = Vector3.one;

		for (int x = 0; x < w; x++) {
			for (int y = 0; y < h; y++) {
				CreateTileAtPos(x, y);
			}
		}
	}

	private void CreateTileAtPos(int x, int y){
		GameObject tileInst = Instantiate(tilePrefab);
//
		tileInst.gameObject.name = "Tile " + x + "-" + y;
		tileInst.transform.position = GameHelper.PosToVec2(new Vec2i(x,y));

		tileInst.transform.SetParent(tileCont.transform);
		tileInst.transform.localScale = Vector3.one;

		tileInst.GetComponent<Tile>().pos = new Vec2i(x,y);

		tileList.Add(tileInst);
	}


	public void ReadTiles(){
		tileMap = new Tile[width][];
		for (int x = 0; x < tileMap.Length; x++) tileMap[x] = new Tile[height];

		foreach (Transform tileTrans in tileCont.transform) {
			Tile tile = tileTrans.GetComponent<Tile>();
			tileMap[tile.pos.x][tile.pos.y] = tile;
		}
	}
	#endregion Editor

}

#if UNITY_EDITOR
[CustomEditor(typeof(Level))]
public class LevelInspector : Editor{

	private int width, height;

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		GUILayout.Label("Custom Inspector", EditorStyles.boldLabel);
		if (GUILayout.Button("Create tiles")){
			((Level)target).CreateTiles();
		}

		if (GUILayout.Button("Read tiles")){
			((Level)target).ReadTiles();
		}
	}
}
#endif
