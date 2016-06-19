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

    private const string dataPath = "Assets/Extras/DBG/DBGTags.asset";

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
		if (DBG.DbgTags.GetTag(tag) == null) {
			DBG.DbgTags.tags.Add(new DBGTag { tag = tag });
#if UNITY_EDITOR
			AssetDatabase.SaveAssets();
#endif
		}
	}

    private static void SetDBGTagReference() {
#if UNITY_EDITOR
        ScriptableObject dbgTags = AssetDatabase.LoadAssetAtPath<DBGTags>(dataPath);
        if (dbgTags == null) {
            dbgTags = ScriptableObject.CreateInstance<DBGTags>();
            AssetDatabase.CreateAsset(dbgTags, dataPath);
        }
        UnityDBG.DBG.DbgTags = (UnityDBG.DBGTags)dbgTags;
#endif
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
