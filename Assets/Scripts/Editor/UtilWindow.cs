using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

public class UtilWindow : EditorWindow {

	[SerializeField] private GameObject replaceGO;
	[SerializeField] private List<GameObject> toReplaceList;
	[SerializeField] private string replaceClassName;
	[SerializeField] private bool searchInProject;

	private Vector2 scrollPos;

	[MenuItem("Window/Util Window")]
    static void Init() {
		UtilWindow window = (UtilWindow)EditorWindow.GetWindow(typeof(UtilWindow), false, "Util Window");
        window.Show();
    }

	[MenuItem("Tools/Delete PlayerPrefs %#d", priority = 10)]
	private static void CallDeletePlayerPrefs() {
		DeletePlayerPrefs();
	}

    void OnGUI () {
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos, false, false);
		GUILayout.Label("Replace GameObject(s)", EditorStyles.boldLabel);

		// "target" can be any class derrived from ScriptableObject (could be EditorWindow, MonoBehaviour, etc)
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
		SerializedProperty toReplaceListProp = so.FindProperty("toReplaceList");
		SerializedProperty replaceClassNameProp = so.FindProperty("replaceClassName");
		SerializedProperty searchInProjectProp = so.FindProperty("searchInProject");

		replaceGO = (GameObject)EditorGUILayout.ObjectField("Replacer GameObject", (Object)replaceGO, typeof(GameObject), true);

		GUILayout.Label("Replace by list", EditorStyles.miniBoldLabel);
		EditorGUILayout.PropertyField(toReplaceListProp, true);
   		if (GUILayout.Button("REPLACE")){
			if (EditorUtility.DisplayDialog("Warning!", 
				"Are you sure you want to replace all gameObjects in list?", "Yes", "No")) {
				ReplaceByList();
			}
   		}

		GUILayout.Label("Replace by class name", EditorStyles.miniBoldLabel);
		EditorGUILayout.PropertyField(replaceClassNameProp);
		EditorGUILayout.PropertyField(searchInProjectProp);
		if (GUILayout.Button("REPLACE")){
			if (EditorUtility.DisplayDialog("Warning!", 
				"Are you sure you want to replace all gameObjects with specified name?", "Yes", "No")) {
				ReplaceByClassName();
			}
   		}


		GUILayout.Space(25);
		GUILayout.Label("Player Prefs", EditorStyles.boldLabel);
		if (GUILayout.Button("Delete ALL PlayerPrefs")) {
			if (EditorUtility.DisplayDialog("Warning!", 
				"Are you sure you want to delete ALL PLAYER PREFS?", "Yes", "No")) {
				DeletePlayerPrefs();
			}
		}

		so.ApplyModifiedProperties();
		EditorGUILayout.EndScrollView();
	}


	private void ReplaceByList(){
		if (replaceGO == null) return;
		Debug.Log("ReplaceByList");
		foreach (GameObject origGO in toReplaceList) {
			ReplaceSceneObject(origGO);
		}
		toReplaceList.Clear();
	}

	private void ReplaceByClassName() {
		if (replaceGO == null) return;
		Debug.Log("ReplaceByClassName - replaceClassName: " + replaceClassName + ", searchInProject: "+ searchInProject);

		Type classTyp = FindTypeInLoadedAssemblies(replaceClassName);
		Debug.Log("classTyp: " + classTyp);
		//object classInst = Activator.CreateInstance(classTyp);

		List<GameObject> foundList = new List<GameObject>();
		//Find gameobjects in scene
		Object[] objs = FindObjectsOfType(classTyp);
		
		foreach (var item in objs) {
			GameObject itemGO = null;
			MonoBehaviour itemMB = item as MonoBehaviour;
			if (itemMB != null) itemGO = itemMB.gameObject;
			if (itemGO != null && !foundList.Contains(itemGO)) {
				foundList.Add(itemGO);
				Debug.Log("scene itemGO: " + itemGO);
			}
		}

		//Find probject gameobjects
		//if (searchInProject) {
		//	objs = Resources.FindObjectsOfTypeAll(classTyp);
		//	foreach (var item in objs) {
		//		Debug.Log("project item: "+ item);
		//		GameObject itemGO = null;
		//		MonoBehaviour itemMB = item as MonoBehaviour;
		//		if (itemMB != null) itemGO = itemMB.gameObject;
		//		if (itemGO != null && !foundList.Contains(itemGO)) {
		//			foundList.Add(itemGO);
		//			Debug.Log("project itemGO: " + itemGO + ", root: "+ itemGO.transform.root.gameObject);
		//		}
		//	}
		//}

		//Replace
		foreach (var item in foundList) {
			//TODO CHECK IF SCENE- OR ASSET OBJECT?
			ReplaceSceneObject(item);
		}
	}

	//private void ReplaceAsset(GameObject origGO) {
	//	GameObject newGO = Instantiate(replaceGO);

	//	newGO.transform.localPosition = origGO.transform.localPosition;
	//	newGO.transform.localScale = origGO.transform.localScale;
	//	newGO.transform.localRotation = origGO.transform.localRotation;
	//	newGO.name = origGO.name;

	//	PrefabUtility.ReplacePrefab(newGO, origGO);
	//}

	private void ReplaceSceneObject(GameObject origGO) {
		GameObject newGO = Instantiate(replaceGO);
//			GameObject newGO = (GameObject) PrefabUtility.InstantiatePrefab((Object)replaceGO);

		newGO.transform.SetParent(origGO.transform.parent);
		newGO.transform.localPosition = origGO.transform.localPosition;
		newGO.transform.localScale = origGO.transform.localScale;
		newGO.transform.localRotation = origGO.transform.localRotation;

		newGO.name = origGO.name;

		DestroyImmediate(origGO);
	}





	public static void DeletePlayerPrefs() {
		PlayerPrefs.DeleteAll();
	}



	#region Tools
		public static Type FindTypeInLoadedAssemblies(string typeName){
		 Type _type = null;
		 foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		 {
			 _type = assembly.GetType(typeName);
			 if (_type != null)
				 break;
		 }
		 return _type;
	 }
	#endregion
}