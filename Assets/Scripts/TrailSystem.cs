using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailSystem {


	List<Trail> trails;

	public TrailSystem() {

	}



	public Trail CreateTrail(Vector3 startPos, Vector3 endPos, TileColor tileClr) {
		var trail = PrefabLibrary.I.GetTrailInstance();
		trail.Init(tileClr);

		trail.transform.position = (endPos + startPos) / 2f;
		trail.transform.rotation = GameHelper.GetRotationFromVector(startPos - endPos ); // Quaternion.AngleAxis(angle, Vector3.forward);
		return trail;
	}


}
