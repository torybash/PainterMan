using UnityEngine;
using System;
using System.Collections;

public class CRManager : Manager<CRManager> {

	public static void CallAfterTime(float time, Action act){
		I.StartCoroutine(I.CallAfterTimeCR(time, act));
	}

	private IEnumerator CallAfterTimeCR(float time, Action act){
		float callTime = Time.time + time;
		while (Time.time < callTime) yield return new WaitForEndOfFrame();

		if (act != null) act();
	}
}
