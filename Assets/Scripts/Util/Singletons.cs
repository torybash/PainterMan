using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class Manager<T> : MonoBehaviour where T : MonoBehaviour {
	private static T _instance;

	private static object _lock = new object();

	public static T Instance {
		get {
			if (Application.isPlaying && applicationIsQuitting) {
				Debug.LogWarning("[Manager] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            lock (_lock) {
				if (_instance == null) {
					_instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1) {
						Debug.LogError("[Manager] Something went really wrong " +
							" - there should never be more than 1 singleton!" +
							" Reopening the scene might fix it.");
						return _instance;
					}

					if (_instance == null) {
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						
						singleton.name = "(Manager) " + typeof(T).ToString();

						Debug.Log("[Manager] An instance of " + typeof(T) +
							" is needed in the scene, so '" + singleton +
							"' was created with DontDestroyOnLoad.");
					}
					else {
						Debug.Log("[Manager] Using instance already created: " +
	                            _instance.gameObject.name);
					}
					if (Application.isPlaying) DontDestroyOnLoad(_instance);
				}

				return _instance;
			}
		}
	}

	 private static bool applicationIsQuitting = false;
	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	///   it will create a buggy ghost object that will stay on the Editor scene
	///   even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	public void OnDestroy() {
		Debug.Log("[Manager] OnDestroy - Application.isPlaying: "  +Application.isPlaying + " (_instance == this): " + (_instance == this));
		if (Application.isPlaying && _instance == this)  applicationIsQuitting = true;
    }

	void Awake(){
		if (_instance != null && this != _instance){
			DestroyImmediate(gameObject);
		}
	}
}




public class Library<T> : MonoBehaviour where T : MonoBehaviour {
	private static T _instance;


	public void Create(){
		if (I == null) {
			Debug.LogError("[Library] Could not Create!");
		}
	}


	private static object _lock = new object();

	public static T I {
		get {
			if (Application.isPlaying && applicationIsQuitting) {
				Debug.LogWarning("[Library] Instance '" + typeof(T) +
					"' already destroyed on application quit." +
					" Won't create again - returning null.");
				return null;
			}

			lock (_lock) {
				if (_instance == null) {
					T[] existing = (T[])FindObjectsOfType(typeof(T));
					if (existing.Length > 0) {
						Debug.Log("[Library] Existing Library(s) found: " + existing.Length + " - destroying!");
						foreach (T item in existing) {
							if (Application.isPlaying) Destroy(item.gameObject);
						}
					}

					if (_instance == null) {
						string path = "_" + typeof(T).ToString();
						T prefabObj = Resources.Load<T>(path);
						if (prefabObj == null) {
							Debug.LogWarning("[Library] No prefab for "+ typeof(T).ToString()+" found at location: Resources/"+ path);
							return _instance;
						}
						if (!Application.isPlaying){
							return prefabObj;
						}

						_instance = Instantiate(prefabObj).GetComponent<T>();
						_instance.gameObject.name = "(Library) " + typeof(T).ToString();

						DontDestroyOnLoad(_instance);

						Debug.Log("[Library] An instance of " + typeof(T) + " is needed in the scene, so '" + prefabObj.gameObject +
							"' was created with DontDestroyOnLoad.");
					}
				}

				return _instance;
			}
		}
	}
		
	private static bool applicationIsQuitting = false;
	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	///   it will create a buggy ghost object that will stay on the Editor scene
	///   even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	public void OnDestroy() {
		Debug.Log("[Library] OnDestroy - Application.isPlaying: "  +Application.isPlaying + " (_instance == this): " + (_instance == this));
		if (Application.isPlaying && _instance == this) applicationIsQuitting = true;
	}
}



public class Controller<T> : MonoBehaviour where T : MonoBehaviour {
	private static T _instance;

	private static object _lock = new object();

	public static T I {
		get {
			if (Application.isPlaying && applicationIsQuitting) {
				Debug.LogWarning("[Controller] Instance '" + typeof(T) +
					"' already destroyed on application quit." +
					" Won't create again - returning null.");
				return null;
			}

			lock (_lock) {
				if (_instance == null) {
					_instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1) {
						Debug.LogError("[Controller] Something went really wrong " +
							" - there should never be more than 1 singleton!" +
							" Reopening the scene might fix it.");
						return _instance;
					}

					if (_instance == null) {
						Debug.LogError("[Controller] Could not find an instance of controller!");
					}
					else {
						Debug.Log("[Controller] Using instance already created: " +
							_instance.gameObject.name);
					}
				}

				return _instance;
			}
		}
	}

	private static bool applicationIsQuitting = false;
	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	///   it will create a buggy ghost object that will stay on the Editor scene
	///   even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	public void OnDestroy() {
		Debug.Log("[Controller] OnDestroy - Application.isPlaying: "  +Application.isPlaying + " (_instance == this): " + (_instance == this));
		if (Application.isPlaying && _instance == this)  applicationIsQuitting = true;
	}

	void Awake(){
		if (I == null) DestroyImmediate(gameObject);
		if (_instance != null && this != _instance){
			DestroyImmediate(gameObject);
		}
	}
}
