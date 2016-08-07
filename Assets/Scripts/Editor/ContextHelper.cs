using UnityEditor;
using UnityEngine;
using System.Collections;

public static class ContextHelper {

	 //private static System.Type ProjectWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ObjectBrowser");
  //   private static EditorWindow projectWindow = null;
 
     public static void StartRenameSelectedAsset(){
		EditorUtility.FocusProjectWindow();
		var e = new Event { keyCode = KeyCode.F2, type = EventType.keyDown }; // or Event.KeyboardEvent("f2");
		EditorWindow.focusedWindow.SendEvent(e);
     }


    [MenuItem("Assets/Create/Create TileObject script", false, 90)]
    public static void Init(){
		string folderPath = AssetDatabase.GetAssetPath (Selection.activeObject);
		string copyPath = folderPath + "/New TileObject.cs";
		NamingPopup.Create(folderPath);
	}

	public static void CreateTileObjectScript(string path) {
		Debug.Log("CreateTileObjectScript - path: " + path);
		KeywordReplace.creatingTileObjectScript = true;
		bool success = AssetDatabase.CopyAsset("Assets/Resources/TileObjectTemplate.cs.txt", AssetDatabase.GenerateUniqueAssetPath(path));
		if (success) {
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	} 



	public class KeywordReplace : UnityEditor.AssetModificationProcessor {

		public static bool creatingTileObjectScript = false;

		public static void OnWillSaveAssets(string[] paths) {
			//Debug.Log("OnWillSaveAssets - paths: " + paths.Length);
			//for (int i = 0; i < paths.Length; i++) {
			//	Debug.Log("OnWillSaveAssets - path: " + i + " = " + paths[i]);
			//}
		}

		public static void OnWillCreateAsset ( string path ) {
			if (!creatingTileObjectScript) return;

			Debug.Log("OnWillCreateAsset - path: " + path);

			path = path.Replace(".meta", "");
			int index = path.LastIndexOf(".");
			string file = path.Substring(index);
			if (file != ".cs" && file != ".js" && file != ".boo") return;
			index = path.LastIndexOf("/");
			string fileName = path.Substring(index + 1); fileName = fileName.Substring(0, fileName.IndexOf("."));
			index = Application.dataPath.LastIndexOf("Assets");
			path = Application.dataPath.Substring(0, index) + path;
			Debug.Log("path: " + path);
			file = System.IO.File.ReadAllText(path);


			string lowerCaseName = fileName.Substring(0, 1).ToLowerInvariant() + fileName.Substring(1, fileName.Length - 1);
			file = file.Replace("#CLASSNAME#", fileName);
			file = file.Replace("#DEFINITION_CLASSNAME#", fileName + "Definition");
			file = file.Replace("#DEFINITION_VARNAME#", "_" + lowerCaseName + "Def");

			System.IO.File.WriteAllText(path, file);
			AssetDatabase.Refresh();

			creatingTileObjectScript = false;
		}

	}
}

public class NamingPopup : EditorWindow {

	[SerializeField] public string folderPath;
	[SerializeField] private string _fileName = "";


	public static void Create(string path) {
		NamingPopup window = ScriptableObject.CreateInstance<NamingPopup>();
		window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
		window.ShowPopup();

		window.folderPath = path;
	}


	void OnGUI(){
        EditorGUILayout.LabelField("Name of TileObject:",EditorStyles.wordWrappedLabel);
		_fileName = EditorGUILayout.TextField(_fileName);
        GUILayout.Space(70);
		if (GUILayout.Button("Create")) {
			ContextHelper.CreateTileObjectScript(folderPath + "/" + _fileName + ".cs");
			this.Close();
		}
    }
}