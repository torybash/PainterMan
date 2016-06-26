using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Teleport : TileObject {

	[SerializeField] TeleportDefinition def;

	private class TeleportCycle {
		public List<Teleport> teleportList;
	}

	private static TeleportCycle[] teleportCycles;
	
	private static void MakeCycles() {
		if (teleportCycles != null) return;
		teleportCycles = new TeleportCycle[1];
		foreach (var teleport in Game.I.Lvl.GetAllTOOfType<Teleport>()) {
			TeleportDefinition teleportDef = (TeleportDefinition)teleport.ToDef;
			int cycleIdx = teleportDef.teleportCycleIdx;
			if (cycleIdx >= teleportCycles.Length) {
				Array.Resize<TeleportCycle>(ref teleportCycles, cycleIdx + 1);
			}
			teleportCycles[cycleIdx].teleportList.Add(teleport);
		}
	}


	public override TileObjectDefintion ToDef {
		get {return def;}
		set {def = (TeleportDefinition)value;}
	}

	public override void Set(TileObjectDefintion def) {
		base.Set(def);
	}

	public override TileObjectInteractionResult PlayerEntered() {
		//return new TileObjectInteractionResult { kill = spikesDef.isRaised };
		return TileObjectInteractionResult.Empty;
	}

	public override void Init() {
		
	}
}


[System.Serializable]
public class TeleportDefinition : TileObjectDefintion{
	[Range(0, 100)] public int teleportCycleIdx;
}