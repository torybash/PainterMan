using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailSystem {


	List<Trail> trails;

	public TrailSystem() {

	}



	public Trail CreateTrail(Vector3 startPos, Vector3 endPos) {
		var trail = PrefabLibrary.I.GetTrailInstance();
		trail.transform.position = (endPos + startPos) / 2f;
		var diff = endPos - startPos;
		float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg + 180f;
		Debug.Log("Diff: " + diff + ", angle: " + angle);
		trail.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		return trail;
	}
}
