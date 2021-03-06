﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Teleport : TileObject {

	[SerializeField] TeleportDefinition _def;

	private class TeleportCycle {
		public List<Teleport> teleportList;
	}

	private static TeleportCycle[] teleportCycles;
	
	private static void MakeCycles() {
		if (teleportCycles != null) return;
		teleportCycles = new TeleportCycle[1];
		foreach (var teleport in Game.I.Lvl.Map.GetAllTOOfType<Teleport>()) {
			TeleportDefinition teleportDef = (TeleportDefinition)teleport.ToDef;
			int cycleIdx = teleportDef.teleportCycleIdx;
			if (cycleIdx + 1 >= teleportCycles.Length) {
				Array.Resize<TeleportCycle>(ref teleportCycles, cycleIdx + 1);
			}
			if (teleportCycles[cycleIdx] == null) teleportCycles[cycleIdx] = new TeleportCycle();
			if (teleportCycles[cycleIdx].teleportList == null) teleportCycles[cycleIdx].teleportList = new List<Teleport>();
			teleportCycles[cycleIdx].teleportList.Add(teleport);
		}
	}


	public override TileObjectDefintion ToDef {
		get {return _def;}
		set {_def = (TeleportDefinition)value;}
	}

	public override void Set(TileObjectDefintion def) {
		base.Set(def);
		if (def.GetType() == typeof(TeleportDefinition)) {
			TeleportDefinition newDef = (TeleportDefinition)def;
			_def.teleportCycleIdx = newDef.teleportCycleIdx;
		}
		Refresh();
	}

	public override TileObjectInteractionResult PlayerEntered() {
		if (teleportCycles == null || teleportCycles.Length <= _def.teleportCycleIdx) {
			Debug.LogError("teleportCycles not created??");
			return TileObjectInteractionResult.Empty();
		}
		Vec2i teleportPos = Vec2i.Zero;
		int thisTPIdx = teleportCycles[_def.teleportCycleIdx].teleportList.IndexOf(this);
		if (thisTPIdx + 1 == teleportCycles[_def.teleportCycleIdx].teleportList.Count) teleportPos = teleportCycles[_def.teleportCycleIdx].teleportList[0]._def.pos;
		else teleportPos = teleportCycles[_def.teleportCycleIdx].teleportList[thisTPIdx + 1]._def.pos;
		return new TileObjectInteractionResult(TileObjectInteractionResultType.Teleport, teleportPos);
	}

	public override void Init() {
		MakeCycles();
	}



	void OnDrawGizmos() {
		Teleport[] teleports = transform.root.GetComponentsInChildren<Teleport>();

		foreach (var tp in teleports) {
			Gizmos.color = new Color(1 - _def.teleportCycleIdx % 2, 1 - _def.teleportCycleIdx % 3, 0 + _def.teleportCycleIdx % 4);
			if (tp != this && tp._def.teleportCycleIdx == _def.teleportCycleIdx) {
				Gizmos.DrawLine(transform.position, tp.transform.position);
			}
		}
	}
}


[System.Serializable]
public class TeleportDefinition : TileObjectDefintion{
	[Range(0, 10)] public int teleportCycleIdx;
}