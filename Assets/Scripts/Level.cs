using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Security.Cryptography;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Level : MonoBehaviour {

	[System.Serializable]
	public class TileMap {
		[SerializeField] private List<Tile> tileList;
		private Dictionary<Vec2i, Tile> tileDict;

		public void MakeDict() {
			tileDict = new Dictionary<Vec2i, Tile>();
			foreach (Tile tile in tileList) {
				tileDict.Add(tile.pos, tile);
			}
		}

		public void AddTile(Tile tile) {
			tileList.Add(tile);
		}

		public bool IsValidTile(Vec2i pos) {
			return GetTile(pos) != null;
		}

		public Tile GetTile(Vec2i pos) {
			if (Application.isPlaying) {
				if (tileDict == null) {
					Debug.Log("why u not create already!");
					MakeDict();
				}
				return tileDict[pos];
			} else {
				foreach (Tile tile in tileList) {
					if (tile.pos == pos) return tile;
				}
			}
			//Debug.Log("[TileMap] Could not find tile at pos: " + pos);
			return null;
		}
		public Tile GetTileOfType(TileType tileType) {
			foreach (Tile tile in tileList) {
				if (tile.tileDef.type == tileType) return tile;
			}
			Debug.Log("[TileMap] Could not find tile of type: " + tileType);
			return null;
		}

		public void DeleteTileAt(Vec2i pos) {
			if (IsValidTile(pos)) {
				Tile tile = GetTile(pos);
				tileList.Remove(tile);
#if UNITY_EDITOR
				Undo.DestroyObjectImmediate(tile.gameObject);
#endif
				//GameObject.DestroyImmediate(tile.gameObject);
			}
		}
	}

	[SerializeField] GameObject tilePrefab;
	//[SerializeField] List<GameObject> tileList;
	[SerializeField] GameObject tileCont;

	[SerializeField] TileMap tileMap;
	public TileMap Map { get{ return tileMap;}}
	//Tile[][] tileMap;

	//[SerializeField] int width, height;


	public void Init(){
		//ReadTiles();
	}

	public Vec2i GetStartPos(){

		return tileMap.GetTileOfType(TileType.Start).pos;
		//for (int x = 0; x < tileMap.Length; x++) {
		//	for (int y = 0; y < tileMap[x].Length; y++) {
		//		if (tileMap[x][y].tileDef.type == TileType.Start) return new Vec2i(x, y);
		//	}
		//}
		Debug.LogError("Level has not start pos! - returning zero");
		return Vec2i.Zero;
	}

	public bool IsValidTile(Vec2i pos) {
		return tileMap.IsValidTile(pos);
		//if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height) {
		//	return tileMap[pos.x][pos.y].tileDef.type != TileType.Empty;
		//}
		//return false;
	}

	public TileType GetTileType(Vec2i pos){
		if (tileMap.IsValidTile(pos)) {
			return tileMap.GetTile(pos).tileDef.type;
		}
		return TileType.Empty;
	}
	public TileColor GetTileColorType(Vec2i pos){
		if (tileMap.IsValidTile(pos)) {
			return tileMap.GetTile(pos).tileDef.color;
		}
		return TileColor.None;
	}
	//public TileColor GetTileColorType(Vec2i pos){
	//	if (IsValidTile(pos)){
	//		return tileMap[pos.x][pos.y].tileDef.color;
	//	}
	//	return TileColor.None;
	//}

	public void PaintTile(Vec2i pos, TileColor tileColor, int turn){
		if (IsValidTile(pos)){
			Debug.Log("[Level] PaintTile - pos: "+ pos + ", tileColor: "+ tileColor + ", turn: "+ turn);

			tileMap.GetTile(pos).tileDef.color = tileColor;
			tileMap.GetTile(pos).tileDef.paintedTurn = turn;
			//tileMap[pos.x][pos.y].tileDef.color = tileColor;
			//tileMap[pos.x][pos.y].tileDef.paintedTurn = turn;

			tileMap.GetTile(pos).Refresh();
		}
	}

	public bool IsWalkable(Vec2i pos, int turn){
		Debug.Log("[Level] IsWalkable - pos: "+ pos + ", turn: "+ turn + ", tile: "+ tileMap.GetTile(pos) + ", is color dry?: "+ IsColorDry(pos, turn));
		return tileMap.GetTile(pos).tileDef.color == TileColor.None || IsColorDry(pos, turn);
	}
	public bool IsColorDry(Vec2i pos, int turn){
		if (IsValidTile(pos)){
			if (tileMap.GetTile(pos).tileDef.type == TileType.Bucket) {
				return true;
			}
			if (tileMap.GetTile(pos).tileDef.color != TileColor.None && tileMap.GetTile(pos).tileDef.paintedTurn + 5 <= turn){
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
	//[SerializeField] Vec2i mapSize;
	[SerializeField] public TileType tileType; 
	[SerializeField] public TileColor tileColor; 

	public void CreateTiles(int w = 10, int h = 10){
		//		int w = 10, h = 10;height
		//int w = this.width; int h = this.height;

//		tiles[x][y] = new Tile();


		//foreach (var item in tileList) {
		//	if (item) DestroyImmediate(item);
		//}
		//tileList.Clear();

		if (tileCont == null) tileCont = new GameObject("TileContainer");
		tileCont.transform.SetParent(transform);
		tileCont.transform.localScale = Vector3.one;

		for (int x = 0; x < w; x++) {
			for (int y = 0; y < h; y++) {
				CreateTileAtPos(new Vec2i(x, y));
			}
		}
	}

	public void CreateTileAtPos(Vec2i pos){
		GameObject tileInst = Instantiate(tilePrefab);
//
		tileInst.gameObject.name = "Tile " + pos;
		tileInst.transform.position = GameHelper.TileToWorldPos(pos);

		tileInst.transform.SetParent(tileCont.transform);
		tileInst.transform.localScale = Vector3.one;

		tileInst.GetComponent<Tile>().pos = pos;

		tileMap.AddTile(tileInst.GetComponent<Tile>());
	}


	//public void ReadTiles(){
	//	tileMap = new Tile[width][];
	//	for (int x = 0; x < tileMap.Length; x++) tileMap[x] = new Tile[height];

	//	foreach (Transform tileTrans in tileCont.transform) {
	//		Tile tile = tileTrans.GetComponent<Tile>();
	//		tileMap[tile.pos.x][tile.pos.y] = tile;
	//	}
	//}
	#endregion Editor

}




#if UNITY_EDITOR
[CustomEditor(typeof(Level))]
public class LevelEditor : Editor {

	int hintID = -1;
	int HintID {
		get {
			if (hintID == -1) hintID = GetHashCode();
			return hintID;
		}
	}

	Level Lvl { get { return (Level)target; } }

	private int width, height;


	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		//GUILayout.Label("Custom Inspector", EditorStyles.boldLabel);
		//if (GUILayout.Button("Create tiles")){
		//	((Level)target).CreateTiles();
		//}

		//if (GUILayout.Button("Read tiles")){
		//	((Level)target).ReadTiles();
		//}
	}

	void OnSceneGUI(){
		//Debug.Log("on scene gui- HintID: " + HintID  + ", Event.current: "+ Event.current);

		Event current = Event.current;
		int controlID = GUIUtility.GetControlID(HintID, FocusType.Passive);


		Handles.BeginGUI( );
		//if (GUI.Button(new Rect(0, 0, 100, 40), "So cool!")) {
		//	Debug.Log("Cool!");
		//}
		//EditorGUI.LabelField(new Rect(0, 50, 100, 40), "Editor label!");

		//tileType = (TileType)EditorGUI.EnumPopup(new Rect(0, 90, 100, 40), tileType);

		GUILayout.BeginArea(new Rect(0, 0, 150, Screen.height));
		Lvl.tileType = (TileType)EditorGUILayout.EnumPopup(Lvl.tileType);
		if (SpriteLibrary.GetTileSprite(Lvl.tileType) != null) {
			GUILayout.Box(SpriteLibrary.GetTileSprite(Lvl.tileType).texture, GUILayout.Width(60), GUILayout.Height(60));
		}
		Lvl.tileColor = (TileColor)EditorGUILayout.EnumPopup(Lvl.tileColor);
		EditorGUILayout.ColorField(GUIContent.none, SpriteLibrary.GetTileColor(Lvl.tileColor), false, false, false, null);
		GUILayout.EndArea();
		Handles.EndGUI();


		switch (current.type) {
		case EventType.mouseDown:
			if (current.button == 0) Painting(current);
			//Event.current.Use();
			break;
		case EventType.mouseDrag:
			if (current.button == 0) Painting(current);
			//Event.current.Use();
			break;
		//case EventType.mouseDrag:
		//	//Painting(current);
		//	//Event.current.Use();
		//	break;
		case EventType.layout:
			HandleUtility.AddDefaultControl(controlID);
			break;
		}
    }

	private void Painting(Event current) {
		//Debug.Log("current.mousePosition: " + current.mousePosition + ", Camera.current: "+ Camera.current) ;

		Vector2 mousePos = Event.current.mousePosition;
        mousePos.y = Camera.current.pixelHeight - mousePos.y;
        Vector3 position = Camera.current.ScreenPointToRay(mousePos).origin;
		//Debug.Log("World pos?: " + position) ;

		Undo.RegisterFullObjectHierarchyUndo(Lvl, "Painted tile");
		PaintTile(position);
	}

	private void PaintTile(Vector2 pos) {
		Vec2i tilePos = GameHelper.WorldToTilePos(pos);
		//Debug.Log("[Level] PaintTile - tilePos: " + tilePos);

		if (Lvl.tileType != TileType.Empty) {
			if (Lvl.Map.IsValidTile(tilePos)) {
				Lvl.Map.GetTile(tilePos).Set(Lvl.tileType, Lvl.tileColor);
			} else {
				
				Lvl.CreateTileAtPos(tilePos);
				Lvl.Map.GetTile(tilePos).Set(Lvl.tileType, Lvl.tileColor);

				Undo.RegisterCreatedObjectUndo(Lvl.Map.GetTile(tilePos).gameObject, "Painted tile");

			}
		} else {
			Lvl.Map.DeleteTileAt(tilePos);
		}

		
	}
}


#endif
