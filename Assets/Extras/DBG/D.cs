using UnityEngine;
using System.Collections;
using UnityDBG;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class D {

    public delegate void DebugLogger(object message);
    public static DebugLogger LogDelegate;
    public static DebugLogger LogWarningDelegate;
    public static DebugLogger LogErrorDelegate;

    public static DebugLogger FallbackLogDelegate;

    public static bool _initialized = false;

    private const string dataPath = "Assets/DBG/DBGTags.asset";

    public static void Init() {
        Debug.Log("Initializing D!");

        LogDelegate += UnityDBG.DBG.Log;
        LogWarningDelegate += UnityDBG.DBG.LogWarning;
        LogErrorDelegate += UnityDBG.DBG.LogError;

        FallbackLogDelegate += UnityEngine.Debug.Log;

        if (UnityDBG.DBG.DbgTags == null) {
            SetDBGTagReference();
        }

        _initialized = true;
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


    private static void Check(){
        if (!_initialized) Init();
        if (UnityDBG.DBG.DbgTags == null) SetDBGTagReference();
    }

    public static DebugLogger Log {
        get {
            Check();
            return LogDelegate;
        }
    }

    public static DebugLogger LogWarning {
        get {
            Check();
            return LogWarningDelegate;
        }
    }

    public static DebugLogger LogError {
        get {
            Check();
            return LogErrorDelegate;
        }
    }
}
