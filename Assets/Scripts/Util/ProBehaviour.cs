using UnityEngine;
using System.Collections;
using UnityDBG;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProBehaviour : MonoBehaviour {

	public delegate void DebugLogger(object message);
    public static DebugLogger LogDelegate;
	public static bool _initialized = false;

	private const string assetPath = "Assets/Extras/DBG/Resources/DBGTags.asset";
	private const string resourcePath = "DBGTags";

	public static void Init() {
        Debug.Log("Initializing ProBehaviour!");

		//LogDelegate += UnityDBG.DBG.Log;

        LogDelegate += UnityDBG.DBG.Log;
        //LogWarningDelegate += UnityDBG.DBG.LogWarning;
        //LogErrorDelegate += UnityDBG.DBG.LogError;

        //FallbackLogDelegate += UnityEngine.Debug.Log;

        //if (UnityDBG.DBG.DbgTags == null) {
        //    SetDBGTagReference();
        //}

        _initialized = true;
    }
	 private void Check(string tag){
        if (!_initialized) Init();
		if (DBG.DbgTags == null) SetDBGTagReference();

		#if UNITY_EDITOR
		if (DBG.DbgTags.GetTag(tag) == null) {
			DBG.DbgTags.tags.Add(new DBGTag { tag = tag, color = GetNewTagColor(), show = true });
			AssetDatabase.SaveAssets();
		}
		#endif
	}

	private Color GetNewTagColor() {
		Color newColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
		if (DBG.DbgTags != null) { //TODO clever stuff
			//foreach (var tag in DBG.DbgTags.tags) {
				//tag.color
			//}
		}
		return newColor;
	}

    private static void SetDBGTagReference() {
		
		//DBGTags dbgTags = AssetDatabase.LoadAssetAtPath<DBGTags>(dataPath);
		DBGTags dbgTags = Resources.Load<DBGTags>(resourcePath);;
#if UNITY_EDITOR
       
        if (dbgTags == null) {
            dbgTags = ScriptableObject.CreateInstance<DBGTags>();
            AssetDatabase.CreateAsset(dbgTags, assetPath);
        }
#endif
        UnityDBG.DBG.DbgTags = dbgTags;
    }


	public DebugLogger Log {
        get {
			string tag = "[" + this.GetType() +"] ";
			Check(this.GetType().ToString());
			UnityDBG.DBG.PrepareLog(tag);
            return LogDelegate;
        }
    }
}
