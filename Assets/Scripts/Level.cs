using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Level : ProBehaviour {

	#region Variables
	[SerializeField] GameObject tilePrefab;

	[SerializeField] TileMap tileMap;
	public TileMap Map { get{ return tileMap;}}
	#endregion Variables

	void Awake() {
		tileMap.PrepareForBuild(); //TODO Do this in editor before building
	}
	
	public void InitTileObjects() {
		foreach (var to in tileMap.TOList) {
			to.Init();
		}
	}

	public void UpdateTileObjects() {
		foreach (var to in tileMap.TOList) {
			to.UpdateTO();
		}
	}


	#region Loading/Unloading
	public void UnloadMap() {
		tileMap.UnloadMap();
	}

	public void LoadTiles() {
		foreach (var def in tileMap.TileDefList) {
			CreateTileAtPos(def.pos, def);
		}
		foreach (var def in tileMap.TODefList) {
			Type typ = Type.GetType(def.className);
			CreateTOAtPos(typ, def);
		}
		tileMap.MakeDict();
	}
	#endregion Loading/Unloading

	#region TileMap Wrappers
	public Vec2i GetStartPos(){
		return tileMap.GetTileOfType(TileType.Start).Pos;
	}
	public List<T> GetAllTOOfType<T>() where T : TileObject{
		return tileMap.GetAllTOOfType<T>();
	}
	public Vector2 GetCenterPos() {
		Vector4 extremes = new Vector4(float.MaxValue, float.MinValue, float.MaxValue, float.MinValue); //min x, max x, min y, max y
		foreach (var item in tileMap.TileList) {
			Vector3 pos = item.transform.position;
			if (pos.x < extremes.x) extremes.x = pos.x;
			if (pos.x > extremes.y) extremes.y = pos.x;
			if (pos.y < extremes.z) extremes.z = pos.y;
			if (pos.y > extremes.w) extremes.w = pos.y;
		}
		return new Vector2((extremes.x + extremes.y)/2f, (extremes.z + extremes.w)/2f);
	}
	public bool IsValidTile(Vec2i pos) {
		return tileMap.IsValidTile(pos);
	}

	public TileType GetTileType(Vec2i pos){
		if (tileMap.IsValidTile(pos)) {
			return tileMap.GetTile(pos).TileDef.type;
		}
		return TileType.Empty;
	}
	public TileColor GetTileColorType(Vec2i pos){
		if (tileMap.IsValidTile(pos)) {
			return tileMap.GetTile(pos).TileDef.color;
		}
		return TileColor.None;
	}
	public void PaintTile(Vec2i pos, TileColor tileColor, int turn){
		if (IsValidTile(pos)){
			Debug.Log("[Level] PaintTile - pos: "+ pos + ", tileColor: "+ tileColor + ", turn: "+ turn);
			tileMap.GetTile(pos).TileDef.color = tileColor;
			tileMap.GetTile(pos).TileDef.paintedTurn = turn;
			tileMap.GetTile(pos).Refresh();
		}
	}

	public bool IsWalkable(Vec2i pos, int turn){
		Debug.Log("[Level] IsWalkable - pos: "+ pos + ", turn: "+ turn + ", tile: "+ tileMap.GetTile(pos) + ", is color dry?: "+ IsColorDry(pos, turn));
		return tileMap.GetTile(pos).TileDef.color == TileColor.None || IsColorDry(pos, turn);
	}
	public bool IsColorDry(Vec2i pos, int turn){
		if (IsValidTile(pos)){
			TileDefinition def = tileMap.GetTile(pos).TileDef;
			if (def.type == TileType.Bucket) {
				return true;
			}
			if (def.color != TileColor.None && def.paintedTurn + GameRules.I.GetTimeToDry(def.color) < turn){
//				Debug.Log("[Level] Color IS dry!");
				return true;
			}
//			Debug.Log("[Level] Color IS NOT dry..");
			return false;
		}
		Debug.LogError("IsColorDry invalid pos!");
		return false;
	}
	public bool CheckForWin() {
		bool allTilesCorrectColor = true;
		foreach (var item in tileMap.TileList) {
			if (item.TileDef.color != item.TileDef.goalColor) {
				allTilesCorrectColor = false;
				break;
			}
		}
		return allTilesCorrectColor;
	}
	#endregion TileMap Wrappers

	#region Editor
	[HideInInspector][SerializeField] public TileType tileType; 
	[HideInInspector][SerializeField] public TileColor tileGoalColor; 
	[HideInInspector][SerializeField] public TileColor tileColor; 

	[HideInInspector][SerializeField] public TileObject tileObjectPrefab; 

	[HideInInspector][SerializeField] public Texture boxTex; 
	[HideInInspector][SerializeField] public Rect boxTexRect; 
	[HideInInspector][SerializeField] GameObject tileCont;
	private GameObject TileCont {
		get {
			if (tileCont == null) {
				tileCont = new GameObject("_Tile Container");
				tileCont.transform.SetParent(transform);
			}
			return tileCont;
		}
	}
	[HideInInspector][SerializeField] GameObject toCont;
	private GameObject TOCont {
		get {
			if (toCont == null) {
				toCont = new GameObject("_TileObject Container");
				toCont.transform.SetParent(transform);
			}
			return toCont;
		}
	}

	public void CreateTileAtPos(Vec2i pos, TileDefinition tileDef){
		Tile tileInst = PrefabLibrary.I.GetTileInstance();
		tileInst.Set(tileDef);

		tileInst.gameObject.name = "Tile " + pos;
		tileInst.transform.position = GameHelper.TileToWorldPos(pos) + (Vector2)transform.position;
		tileInst.transform.SetParent(TileCont.transform);
		tileInst.transform.localScale = Vector3.one;

		tileMap.AddTile(tileInst);
	}

	public void CreateTOAtPos(System.Type typ, TileObjectDefintion toDef) {
		TileObject toInst = PrefabLibrary.I.GetTileObject(typ);
		InitTO(toInst, toDef);
	}
	public void CreateTOAtPos<T>(T tileObject, TileObjectDefintion toDef) where T : TileObject{
		//Log("CreateTileObjectAtPos: " + to + ", " + pos + ", typeof(T): " + typeof(T) + ", totype: "+ to.GetType());
		TileObject toInst = PrefabLibrary.I.GetTileObject(tileObject);
		InitTO(toInst, toDef);
	}

	private void InitTO(TileObject toInst, TileObjectDefintion toDef) {
		toInst.Set(toDef);

		toInst.gameObject.name = "TO:" + toInst.GetType() + ", " + toDef.pos;
		toInst.transform.position = GameHelper.TileToWorldPos(toDef.pos) + (Vector2)transform.position;
		toInst.transform.SetParent(TOCont.transform);
		toInst.transform.localScale = Vector3.one;

		tileMap.AddTileObject(toInst);
	}
	#endregion Editor

}




#if UNITY_EDITOR
[CustomEditor(typeof(Level))]
public class LevelEditor : Editor {

	private enum ControlType { Disabled, TilePaint, TileObject}
	private ControlType currControl = ControlType.Disabled;

	private int hintID = -1;
	private int HintID {
		get {
			if (hintID == -1) hintID = GetHashCode();
			return hintID;
		}
	}

	private Level Lvl { get { return (Level)target; } }
	private Event current;
	private bool altDown = false;
	private bool ctrlDown = false;

	private int currTileObjectIdx;
	private string[] tileObjectNames;
	public string[] TileObjectNames {
		get {
			if (tileObjectNames == null) {
				tileObjectNames = new string[PrefabLibrary.I.TileObjectPrefabs.Count];
				for (int i = 0; i < tileObjectNames.Length; i++) {
					tileObjectNames[i] = PrefabLibrary.I.TileObjectPrefabs[i].GetType().Name;
				}
			}
			return tileObjectNames;
		}
	}

	//private SerializedObject toDefintionSO;
	//private SerializedProperty toDefinitionProp;
	//private void SetTODefintion(Object obj) {
	//	if (obj == null) return;
	//	toDefintionSO = new SerializedObject(obj);
	//	toDefinitionProp = toDefintionSO.FindProperty("def");
	//}

	//private TileObjectDefintionHolder<TileObjectDefintion> toDefHolder;
	//private TileObjectDefintionHolder<TileObjectDefintion> TODefHolder {
	//	get {
	//		if (toDefHolder == null) {
	//			toDefHolder = ScriptableObject.CreateInstance<TileObjectDefintionHolder<TileObjectDefintion>>();
	//		}
	//		return toDefHolder;
	//	}
	//}

	//private Texture boxTex;
	public Texture SetBoxTex(Rect rect) {
		if (Lvl.boxTex == null) {
			Texture2D tex2D = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false, false);
			Color32[] colors = new Color32[tex2D.width * tex2D.height];
			for (int i = 0; i < colors.Length; i++) {
				colors[i] = new Color32(1, 1, 1, 1);
			}
			tex2D.SetPixels32(colors);
			tex2D.Apply();
			Lvl.boxTex = tex2D;
		}
		return Lvl.boxTex;
	}

	public override void OnInspectorGUI (){
		base.OnInspectorGUI ();
	}

	void OnSceneGUI(){
		//Debug.Log("on scene gui- HintID: " + HintID  + ", Event.current: "+ Event.current);

		GUI.changed = false;
		current = Event.current;
		int controlID = GUIUtility.GetControlID(HintID, FocusType.Passive);


		#region Handles GUI
		Handles.BeginGUI( );
		Rect rect = new Rect(0, 0, 150, Screen.height / 2f);
		if (Lvl.boxTex != null) {
			//Debug.Log("Lvl.boxTexRect: " + Lvl.boxTexRect);
			GUI.color = new Color32(255, 255, 255, 127);
			GUI.Box(Lvl.boxTexRect, Lvl.boxTex);
			GUI.color = Color.white;
		}
		GUILayout.BeginArea(rect);

		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("" + (currControl == ControlType.Disabled ? ">>" : "") + "Disable", EditorStyles.boldLabel, GUILayout.Width(rect.width - 25));
		bool disabledToggle = EditorGUILayout.Toggle(currControl == ControlType.Disabled);
		if (disabledToggle) currControl = ControlType.Disabled; 
		GUILayout.EndHorizontal();


		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("" + (currControl == ControlType.TilePaint ? ">>" : "") + "Tile paint", EditorStyles.boldLabel, GUILayout.Width(rect.width - 25));
		bool paintToggle = EditorGUILayout.Toggle(currControl == ControlType.TilePaint);
		if (paintToggle) currControl = ControlType.TilePaint; 
		GUILayout.EndHorizontal();
		Lvl.tileType = (TileType)EditorGUILayout.EnumPopup(Lvl.tileType);
		if (SpriteLibrary.GetTileSprite(Lvl.tileType) != null) {
			GUILayout.Box(SpriteLibrary.GetTileSprite(Lvl.tileType).texture, GUILayout.Width(60), GUILayout.Height(60));
		}

		EditorGUILayout.LabelField("Start color");
		Lvl.tileColor = (TileColor)EditorGUILayout.EnumPopup(Lvl.tileColor);
		EditorGUILayout.ColorField(GUIContent.none, SpriteLibrary.GetTileColor(Lvl.tileColor), false, false, false, null);

		EditorGUILayout.LabelField("Goal color");
		Lvl.tileGoalColor = (TileColor)EditorGUILayout.EnumPopup(Lvl.tileGoalColor);
		EditorGUILayout.ColorField(GUIContent.none, SpriteLibrary.GetTileColor(Lvl.tileGoalColor), false, false, false, null);

		EditorGUILayout.Space();
		GUILayout.BeginHorizontal();

		EditorGUILayout.LabelField("" + (currControl == ControlType.TileObject?">>":"") +"Tile objects", EditorStyles.boldLabel, GUILayout.Width(rect.width - 25));
		bool toToggle = EditorGUILayout.Toggle(currControl == ControlType.TileObject);
		if (toToggle) currControl = ControlType.TileObject; 
		GUILayout.EndHorizontal();

		currTileObjectIdx = EditorGUILayout.Popup(currTileObjectIdx, TileObjectNames);
		if (Lvl.tileObjectPrefab != null) {
			foreach (var fieldInfo in Lvl.tileObjectPrefab.ToDef.GetType().GetFields()) {
				//if (fieldInfo.Name == "pos" || fieldInfo.Name == "className") continue;
				GUILayout.Label(fieldInfo.Name);
				//if (toDefinitionProp != null && toDefinitionProp.FindPropertyRelative(fieldInfo.Name) != null) {
				//	EditorGUILayout.PropertyField(toDefinitionProp.FindPropertyRelative(fieldInfo.Name), GUIContent.none);
				//}
				//EditorGUILayout.field
				//new UnityEditor.SerializedObject()
			}
		}


		GUILayout.EndVertical();
		Rect newRect = GUILayoutUtility.GetLastRect();
		if (newRect.width > 2) {
			newRect.height += 10;
			Lvl.boxTexRect = newRect;
		}
		SetBoxTex(Lvl.boxTexRect);

		GUILayout.EndArea();
		Handles.EndGUI();
#endregion Handles GUI

		

		switch (current.type) {
		case EventType.keyDown:
			Debug.Log("KeyDown: " + current.keyCode);
			//if (current.keyCode == KeyCode.LeftAlt || current.keyCode == KeyCode.RightAlt) { altDown = !altDown;}
			//else if (current.keyCode == KeyCode.LeftControl || current.keyCode == KeyCode.RightControl) ctrlDown = !ctrlDown;
			//else 
			if (InputHelper.GetNumberPressed(current.keyCode) != -1) {
				NumberPressed(InputHelper.GetNumberPressed(current.keyCode));
				current.Use();
			} else {
				if (current.keyCode == KeyCode.Tab) {
					currControl = (ControlType)((((int)currControl) + 1)%System.Enum.GetValues(typeof(ControlType)).Length);
				}
			}
			break;
		case EventType.mouseDown:
			if (current.button == 0 && !Lvl.boxTexRect.Contains(Event.current.mousePosition)) {
				if (currControl == ControlType.TilePaint) PaintTile();
				else if (currControl == ControlType.TileObject) PlaceTO();
			}
			//Event.current.Use();
			break;
		case EventType.mouseDrag:
			if (current.button == 0 && currControl == ControlType.TilePaint) PaintTile();
			//Event.current.Use();
			break;
		case EventType.layout:
			if (!Lvl.boxTexRect.Contains(Event.current.mousePosition) && currControl == ControlType.Disabled) break;
			HandleUtility.AddDefaultControl(controlID);
			break;
		}

		if (GUI.changed) {
			Lvl.tileObjectPrefab = PrefabLibrary.I.TileObjectPrefabs[currTileObjectIdx];
			//SetTODefintion((Object)Lvl.tileObjectPrefab.ToDef);
			//SetTODefintion(TODefHolder);

			Lvl.Map.CleanLists();

			EditorUtility.SetDirty(Lvl);
		}
    }


	private void NumberPressed(int number) {
		Debug.Log("NumberPressed: " + number + ", shiftDown: "+ altDown + ", ctrlDown: "+ ctrlDown);
		//if (altDown) {
		//	TileColor typ= TileColor.None;
		//	Lvl.tileColor = (TileColor) typ.SetToValueByNumber(number);
		//} else if (ctrlDown) {
		//	TileColor typ= TileColor.None;
		//	Lvl.tileGoalColor = (TileColor) typ.SetToValueByNumber(number);
		//} else {
			TileType typ= TileType.Empty;
			Debug.Log("typ.SetToValueByNumber(number): " + (TileType)typ.SetToValueByNumber(number));
			Lvl.tileType = (TileType) typ.SetToValueByNumber(number);
		//}
	}

	private void PaintTile() {
		//Debug.Log("[Level] current.mousePosition: " + current.mousePosition + ", Camera.current: " + Camera.current);
		Undo.RegisterFullObjectHierarchyUndo(Lvl, "Painted tile");
		Vec2i tilePos = GameHelper.WorldToTilePos(GetMouseWorldPos() - (Vector2)Lvl.transform.position);
		//Debug.Log("[Level] PaintTile - tilePos: " + tilePos);
		if (Lvl.tileType != TileType.Empty) {
			TileDefinition tileDef = new TileDefinition { type = Lvl.tileType, goalColor = Lvl.tileGoalColor, color = Lvl.tileColor, pos = tilePos };
			if (Lvl.Map.IsValidTile(tilePos)) {
				Undo.RecordObject(Lvl.Map.GetTile(tilePos), "Painted tile");
				Lvl.Map.SetTile(tilePos, tileDef);
			} else {
				Lvl.CreateTileAtPos(tilePos, tileDef);
				Undo.RegisterCreatedObjectUndo(Lvl.Map.GetTile(tilePos).gameObject, "Painted tile");
			}
		} else {
			Lvl.Map.DeleteTileAt(tilePos);
		}
	}

	private void PlaceTO() {
		Debug.Log("PlaceTO - .tileObjectPrefab: " + Lvl.tileObjectPrefab + ", .typ: " + Lvl.tileObjectPrefab.GetType());

		Undo.RegisterFullObjectHierarchyUndo(Lvl, "Placed tile object");
		Vec2i tilePos = GameHelper.WorldToTilePos(GetMouseWorldPos() - (Vector2)Lvl.transform.position);
		if (!Lvl.Map.HasTOTypeAtPos(tilePos, Lvl.tileObjectPrefab.GetType())) { //if tile not already contains type
			string className = Lvl.tileObjectPrefab.GetType().FullName + ", " + Lvl.tileObjectPrefab.GetType().Assembly.GetName().Name;
			//Lvl.tileObjectPrefab.ToDef
			TileObjectDefintion toDef = new TileObjectDefintion { pos = tilePos, className = className };
			Lvl.CreateTOAtPos(Lvl.tileObjectPrefab, toDef);
		}
	}


	public System.Type GetTypeByName(string assemblyQualifiedClassName) {
		System.Type typ = !string.IsNullOrEmpty(assemblyQualifiedClassName) ? System.Type.GetType(assemblyQualifiedClassName) : null;
		return typ;
	}

	private Vector2 GetMouseWorldPos() {
		Vector2 mousePos = Event.current.mousePosition;
        mousePos.y = Camera.current.pixelHeight - mousePos.y;
        return Camera.current.ScreenPointToRay(mousePos).origin;
	}

}


#endif
