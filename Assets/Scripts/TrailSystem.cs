using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailSystem {


	List<Trail> trails = new List<Trail>();



	public void Init() {
		foreach (var trail in trails) {
			GameObject.Destroy(trail.gameObject);
		}
		trails.Clear();
	}

	public Trail CreateTrail(Vec2i startPos, Vec2i endPos, TileColor tileClr, int turn) {
		if (tileClr == TileColor.None) return null;

		var trail = PrefabLibrary.I.GetTrailInstance();
		trail.Init(tileClr, turn);

		Vector2 startWPos = GameHelper.TileToWorldPos(startPos) + (Vector2)Game.I.Lvl.transform.position;
		Vector2 endWPos = GameHelper.TileToWorldPos(endPos) + (Vector2)Game.I.Lvl.transform.position;
		trail.transform.position = (startWPos + endWPos) / 2f;
		trail.transform.rotation = GameHelper.GetRotationFromVector(startWPos - endWPos ); // Quaternion.AngleAxis(angle, Vector3.forward);

		trails.Add(trail);

		return trail;
	}

	public void UpdateTrails() {
		var trailsTemp = new List<Trail>(trails);
		foreach (var trail in trailsTemp) {
			if (trail.PaintedTurn + GameRules.GetTimeToDry(trail.TileClr) <= Game.I.Turn) {
				GameObject.Destroy(trail.gameObject);
				trails.Remove(trail);
			} else {
				trail.UpdateColor();
			}
		}
	}

}
