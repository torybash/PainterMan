using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Level : ProBehaviour {

	#region Variables
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
		//return tileMap.GetTileOfType(TileType.Start).Pos;
		return tileMap.GetTOOfType<Entrance>().ToDef.pos;
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

		return tileMap.IsValidTile(pos) && (tileMap.GetTile(pos).TileDef.color == TileColor.None || (!GameRules.PaintNeedsToBeDry || IsColorDry(pos, turn)) );
	}
	public bool IsColorDry(Vec2i pos, int turn){
		if (IsValidTile(pos)){
			TileDefinition def = tileMap.GetTile(pos).TileDef;
			if (def.type == TileType.Bucket) {
				return true;
			}
			if (def.color != TileColor.None && def.paintedTurn + GameRules.GetTimeToDry(def.color) < turn){
//				Debug.Log("[Level] Color IS dry!");
				return true;
			}
//			Debug.Log("[Level] Color IS NOT dry..");
			return false;
		}
		//Debug.LogError("IsColorDry invalid pos!");
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
	[HideInInspector][SerializeField] public LevelEditor.ControlType currControl;
	[HideInInspector][SerializeField] public int currTileObjectIdx;
	[HideInInspector][SerializeField] public int lastTileObjectIdx = -1;

	[HideInInspector][SerializeField] public TileType tileType; 
	[HideInInspector][SerializeField] public TileColor tileGoalColor; 
	[HideInInspector][SerializeField] public TileColor tileColor; 

	[HideInInspector][SerializeField] public TileObject tileObjectPrefab; 
	[HideInInspector][SerializeField] public TileObjectDefintion tileObjectDefinition; 

	[HideInInspector][SerializeField] public Texture boxTex; 
	[SerializeField] public Rect boxTexRect; 
	[HideInInspector][SerializeField] GameObject tileCont;
	[HideInInspector][SerializeField] GameObject toCont;


	private GameObject TileCont {
		get {
			if (tileCont == null) {
				tileCont = new GameObject("_Tile Container");
				tileCont.transform.SetParent(transform);
			}
			return tileCont;
		}
	}
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
	public TileObject CreateTOAtPos<T>(T tileObject, TileObjectDefintion toDef) where T : TileObject{
		//Log("CreateTileObjectAtPos: " + to + ", " + pos + ", typeof(T): " + typeof(T) + ", totype: "+ to.GetType());
		TileObject toInst = PrefabLibrary.I.GetTileObject(tileObject);
		InitTO(toInst, toDef);
		return toInst;
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

	public enum ControlType { Disabled, TilePaint, TileObject}

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

	void OnEnable()
	{
		//Debug.Log("[LevelEditor] OnEnable");
		Tools.hidden = true;
		Lvl.Map.CleanLists();
	}
 
	void OnDisable()
	{
		//Debug.Log("[LevelEditor] OnDisable");
		Tools.hidden = false;
		Lvl.Map.CleanLists();
	}





	private SerializedObject toDefintionSO;
	private SerializedProperty toDefinitionProp;
	//private void SetTODefintion(UnityEngine.Object obj) {
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
		Rect rect = new Rect(0, 0, 300, Screen.height);
		if (Lvl.boxTex != null) {
			//Debug.Log("Lvl.boxTexRect: " + Lvl.boxTexRect);
			GUI.color = new Color32(255, 255, 255, 127);
			GUI.Box(Lvl.boxTexRect, Lvl.boxTex);
			GUI.color = Color.white;
			rect = Lvl.boxTexRect;
		}
		GUILayout.BeginArea(rect);

		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("" + (Lvl.currControl == ControlType.Disabled ? ">>" : "") + "Disable", EditorStyles.boldLabel, GUILayout.Width(rect.width - 25));
		bool disabledToggle = EditorGUILayout.Toggle(Lvl.currControl == ControlType.Disabled);
		if (disabledToggle) Lvl.currControl = ControlType.Disabled; 
		GUILayout.EndHorizontal();


		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("" + (Lvl.currControl == ControlType.TilePaint ? ">>" : "") + "Tile paint", EditorStyles.boldLabel, GUILayout.Width(rect.width - 25));
		bool paintToggle = EditorGUILayout.Toggle(Lvl.currControl == ControlType.TilePaint);
		if (paintToggle) Lvl.currControl = ControlType.TilePaint; 
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

		EditorGUILayout.LabelField("" + (Lvl.currControl == ControlType.TileObject?">>":"") +"Tile objects", EditorStyles.boldLabel, GUILayout.Width(rect.width - 25));
		bool toToggle = EditorGUILayout.Toggle(Lvl.currControl == ControlType.TileObject);
		if (toToggle) Lvl.currControl = ControlType.TileObject; 
		GUILayout.EndHorizontal();

		Lvl.currTileObjectIdx = EditorGUILayout.Popup(Lvl.currTileObjectIdx, TileObjectNames);
		if (Lvl.tileObjectPrefab != null && Lvl.tileObjectDefinition != null) {
			foreach (var field in Lvl.tileObjectDefinition.GetType().GetFields()) {
				if (field.Name == "pos" || field.Name == "className") continue;
				GUILayout.Label(field.Name);

				object fieldVal = field.GetValue(Lvl.tileObjectDefinition);
				if (field.FieldType == typeof(int)) {
					var val = EditorGUILayout.IntField((int)fieldVal);
					field.SetValue(Lvl.tileObjectDefinition, val);
				} else if (field.FieldType == typeof(bool)) {
					var val = EditorGUILayout.Toggle((bool)fieldVal);
					field.SetValue(Lvl.tileObjectDefinition, val);
				}else if (field.FieldType == typeof(string)) {
					var val = EditorGUILayout.TextField((string)fieldVal);
					field.SetValue(Lvl.tileObjectDefinition, val);
				}else if (field.FieldType == typeof(float)) {
					var val = EditorGUILayout.FloatField((float)fieldVal);
					field.SetValue(Lvl.tileObjectDefinition, val);
				}else if (field.FieldType.IsEnum) {
					var val = EditorGUILayout.EnumPopup((Enum)fieldVal);
					field.SetValue(Lvl.tileObjectDefinition, val);
				}
				EditorUtility.SetDirty(Lvl);

				//Debug.Log("Lvl.tileObjectPrefab.ToDef.GetType(): " + Lvl.tileObjectPrefab.ToDef.GetType());
				//var obj = ScriptableObject.CreateInstance<TileObjectDefintionHolder>();
				//obj.Init(Lvl.tileObjectPrefab.ToDef.GetType());
				//Debug.Log("obj: " + obj);

				//ScriptableObject defObj = ScriptableObject.CreateInstance(Lvl.tileObjectDefinition.GetType());

				//if (defObj != null) {
				//	SerializedObject serDefObj = new UnityEditor.SerializedObject(defObj);
				//	//SerializedProperty defObjProp = serializedObject.FindProperty("defObj");
				//	//SerializedProperty defTypeProp = serializedObject.FindProperty("defType");
				//	//SerializedProperty numProp = serializedObject.FindProperty("num");

				//	Debug.Log("defObj: " + defObj + ", serDefObj: " + serDefObj);
				//	//Debug.Log("defTypeProp: " + defTypeProp + ", numProp: " + numProp);
				//	foreach (var item in serDefObj.targetObjects) {
				//		Debug.Log("serDefObj.item: " + item);
				//	}
				//	if (defObj != null && serDefObj.FindProperty(fieldInfo.Name) != null) {
				//		EditorGUILayout.PropertyField(serDefObj.FindProperty(fieldInfo.Name), GUIContent.none, true);						
				//	}
				//}


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
					Lvl.currControl = (ControlType)((((int)Lvl.currControl) + 1) % System.Enum.GetValues(typeof(ControlType)).Length);

				} else if (current.keyCode == KeyCode.R) {
					Lvl.tileGoalColor = TileColor.Red;
				} else if (current.keyCode == KeyCode.G) {
					Lvl.tileGoalColor = TileColor.Green;
				} else if (current.keyCode == KeyCode.B) {
					Lvl.tileGoalColor = TileColor.Blue;
				} else if (current.keyCode == KeyCode.N) {
					Lvl.tileGoalColor = TileColor.None;

				} else if (current.keyCode == KeyCode.C) {
					Lvl.tileGoalColor = TileColor.Cyan;
				} else if (current.keyCode == KeyCode.M) {
					Lvl.tileGoalColor = TileColor.Magneta;
				} else if (current.keyCode == KeyCode.Y) {
					Lvl.tileGoalColor = TileColor.Yellow;

				} else if (current.keyCode == KeyCode.Q) {
					Lvl.tileGoalColor = (TileColor)((((int)Lvl.tileGoalColor) + 1) % System.Enum.GetValues(typeof(TileColor)).Length);
				} else if (current.keyCode == KeyCode.W) {
					Lvl.tileGoalColor = (TileColor)(((int)Lvl.tileGoalColor) - 1 < 0 ? System.Enum.GetValues(typeof(TileColor)).Length - 1 : ((int)Lvl.tileGoalColor) - 1);
				}
			}
			Event.current.Use();
			GUI.changed = true;
			break;
		case EventType.mouseDown:
			if (!Lvl.boxTexRect.Contains(Event.current.mousePosition)) {
				if (current.button == 0) {
					if (Lvl.currControl == ControlType.TilePaint) PaintTile();
					else if (Lvl.currControl == ControlType.TileObject) PlaceTO();
				} else if (current.button == 1) {
					if (Lvl.currControl == ControlType.TilePaint) DeleteTile();
					else if (Lvl.currControl == ControlType.TileObject) DeleteTO();
				}
			}
			GUI.changed = true;
			break;
		case EventType.mouseDrag:
			if (!Lvl.boxTexRect.Contains(Event.current.mousePosition)) {
				if (current.button == 0) {
					if (Lvl.currControl == ControlType.TilePaint) PaintTile();
					else if (Lvl.currControl == ControlType.TileObject) PlaceTO();
					Event.current.Use();
				} else if (current.button == 1) {
					if (Lvl.currControl == ControlType.TilePaint) DeleteTile();
					else if (Lvl.currControl == ControlType.TileObject) DeleteTO();
					Event.current.Use();
				}
			}
			GUI.changed = true;
			break;
		case EventType.layout:
			if (!Lvl.boxTexRect.Contains(Event.current.mousePosition) && Lvl.currControl == ControlType.Disabled) break;
			HandleUtility.AddDefaultControl(controlID);
			break;
		}

		if (GUI.changed) {
			if (Lvl.currTileObjectIdx != Lvl.lastTileObjectIdx) {
				Lvl.tileObjectPrefab = PrefabLibrary.I.TileObjectPrefabs[Lvl.currTileObjectIdx];
				Lvl.tileObjectDefinition = (TileObjectDefintion) Activator.CreateInstance(Lvl.tileObjectPrefab.ToDef.GetType());
				Lvl.lastTileObjectIdx = Lvl.currTileObjectIdx;
			}
			Lvl.Map.CleanLists();
			EditorUtility.SetDirty(Lvl);
		}
    }


	private void NumberPressed(int number) {
		D.Log("[LevelEditor] NumberPressed: " + number + ", shiftDown: "+ altDown + ", ctrlDown: "+ ctrlDown);
		//if (altDown) {
		//	TileColor typ= TileColor.None;
		//	Lvl.tileColor = (TileColor) typ.SetToValueByNumber(number);
		//} else if (ctrlDown) {
		//	TileColor typ= TileColor.None;
		//	Lvl.tileGoalColor = (TileColor) typ.SetToValueByNumber(number);
		//} else {
			TileType typ= TileType.Empty;
			//Debug.Log("typ.SetToValueByNumber(number): " + (TileType)typ.SetToValueByNumber(number));
			Lvl.tileType = (TileType) typ.SetToValueByNumber(number);
		//}
	}

	private void PaintTile() {
		Undo.RegisterFullObjectHierarchyUndo(Lvl, "Painted tile");
		Vec2i tilePos = GameHelper.WorldToTilePos(GetMouseWorldPos() - (Vector2)Lvl.transform.position);
		//Debug.Log("[Level] PaintTile - tilePos: " + tilePos);
		if (Lvl.tileType != TileType.Empty) {
			TileDefinition tileDef = new TileDefinition { type = Lvl.tileType, goalColor = Lvl.tileGoalColor, color = (Lvl.tileType == TileType.Bucket ? Lvl.tileGoalColor : Lvl.tileColor), pos = tilePos };
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

		current.Use();
	}

	private void DeleteTile() {
		Undo.RegisterFullObjectHierarchyUndo(Lvl, "Deleted tile");
		Vec2i tilePos = GameHelper.WorldToTilePos(GetMouseWorldPos() - (Vector2)Lvl.transform.position);
		Lvl.Map.DeleteTileAt(tilePos);

		current.Use();
	}

	private void PlaceTO() {
		Debug.Log("PlaceTO - .tileObjectPrefab: " + Lvl.tileObjectPrefab + ", .typ: " + Lvl.tileObjectPrefab.GetType());

		Undo.RegisterFullObjectHierarchyUndo(Lvl, "Placed TileObject");
		Vec2i tilePos = GameHelper.WorldToTilePos(GetMouseWorldPos() - (Vector2)Lvl.transform.position);
		if (!Lvl.Map.HasTOTypeAtPos(tilePos, Lvl.tileObjectPrefab.GetType())) { //if tile not already contains type
			string className = Lvl.tileObjectPrefab.GetType().FullName + ", " + Lvl.tileObjectPrefab.GetType().Assembly.GetName().Name;
			//Lvl.tileObjectPrefab.ToDef
			//TileObjectDefintion toDef = new TileObjectDefintion { pos = tilePos, className = className };
			Debug.Log("PlacedTO - Lvl.tileObjectDefinition: " + Lvl.tileObjectDefinition + ", typ: " + Lvl.tileObjectDefinition.GetType());
			Lvl.tileObjectDefinition.pos = tilePos;
			Lvl.tileObjectDefinition.className = className;
			TileObject to = Lvl.CreateTOAtPos(Lvl.tileObjectPrefab, Lvl.tileObjectDefinition);
			Undo.RegisterCreatedObjectUndo(to.gameObject, "Create TileObject");
		}

		current.Use();
	}

	private void DeleteTO() {
		Vec2i tilePos = GameHelper.WorldToTilePos(GetMouseWorldPos() - (Vector2)Lvl.transform.position);
		Lvl.Map.DeleteAllTOAtPos(tilePos);

		current.Use();
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

//[Serializable]
//public class TileObjectDefintionHolder : ScriptableObject, ISerializationCallbackReceiver {

//	public System.Object defObj;

//	[SerializeField, HideInInspector]
//    private string defSerialized = "";
	
//	public void Init(Type defType) {
//		Debug.Log("[TileObjectDefintionHolder] Init - defType: " + defType);
//		defObj = Activator.CreateInstance(defType);
//		Debug.Log("defObj: " + defObj + ", defObj.type: "+ defObj.GetType());
		
//	}

//	public void OnBeforeSerialize() {
//		Debug.Log("OnBeforeSerialize!");
//		if (defObj == null) {
//			defSerialized = "n";
//			return;
//		}
//		defSerialized = "" + defObj.GetType() + ":";
//		for (int i = 0; i < defObj.GetType().GetFields().Length; i++) {
//			var field = defObj.GetType().GetFields()[i];

//			var fieldType = field.FieldType;
//			object fieldVal = field.GetValue(defObj);
//			if (fieldType == typeof(int))
//				defSerialized += fieldVal;
//			else if (fieldType == typeof(float))
//				defSerialized += fieldVal;
//			else if (fieldType == typeof(Color)) {
//				Color32 c = (Color)fieldVal;
//				int v = (int)c.r + (int)c.g << 8 + (int)c.b << 16 + (int)c.a << 24;
//				defSerialized += v;
//			} else if (fieldType == typeof(Vector3)) {
//				Vector3 v = (Vector3)fieldVal;
//				defSerialized += v.x + "|" + v.y + "|" + v.z;
//			}

//			defSerialized += ",";
//		}
//	}

//	public void OnAfterDeserialize() {
//		Debug.Log("OnAfterDeserialize! - defSerialized: "+ defSerialized);
//		if (defSerialized.Length == 0)
//			return;
//		char type = defSerialized[0];
//		if (type == 'n')
//			defObj = null;
//		int idx = defSerialized.IndexOf(":");
//		string typeName = defSerialized.Substring(0, idx);
//		defObj = Activator.CreateInstance(Type.GetType(typeName));
//		defSerialized = defSerialized.Split(":".ToCharArray()[0])[1];

//		string[] fieldVals = defSerialized.Split(",".ToCharArray()[0]);

//		for (int i = 0; i < defObj.GetType().GetFields().Length; i++) {
//			var field = defObj.GetType().GetFields()[i];
//			Type fieldType = field.FieldType;
//			if (fieldType == typeof(int)) {
//				int val = int.Parse(fieldVals[i]);
//				field.SetValue(defObj, val);
//				//defSerialized += "i" + fieldVal;
//			} else if (fieldType == typeof(float)) {
//				//defSerialized += "f" + fieldVal;
//			} else if (fieldType == typeof(Color)) {
//				//Color32 c = (Color)fieldVal;
//				//int v = (int)c.r + (int)c.g << 8 + (int)c.b << 16 + (int)c.a << 24;
//				//defSerialized += "c" + v;
//			} else if (fieldType == typeof(Vector3)) {
//				//Vector3 v = (Vector3)fieldVal;
//				//defSerialized += "v" + v.x + "|" + v.y + "|" + v.z;
//			}
//				//field.SetValue(defObj, );
//		}
//		//for (int i = 0; i < fieldVals; i++) {

//		//}
//	}

	
//}

#endif
