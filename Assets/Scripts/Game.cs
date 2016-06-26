using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game : Controller<Game> {

	public enum State { Disabled, Normal, Animating}

	#region Variables
	[SerializeField] List<Level> levelList;

	[SerializeField] GameObject playerObj;

	[SerializeField] [ReadOnly] private State state;
	
	private Vec2i playerPos;
	private TileColor currPlayerColor;

	private int currLevelIdx;
	private Level currLvl;
	public Level Lvl {
		get { return currLvl; }
	}

	private int turn = 0;
	public int Turn{
		get{ return turn; }
	}
	#endregion Variables


	void Update(){
		if (state != State.Normal) return;
		Vec2i move = InputManager.I.InputUpdate(playerPos.y % 2 == 0);
		if (move != Vec2i.Zero) TryMovePlayer(move);
	}

	private void StartLevel() {
		Vec2i startPos = currLvl.GetStartPos();
		playerPos = startPos;
		currPlayerColor = TileColor.None;
		playerObj.transform.position = (Vector3)GameHelper.TileToWorldPos(startPos) + currLvl.transform.position;
		playerObj.GetComponent<SpriteRenderer>().color = SpriteLibrary.GetTileColor(currPlayerColor);

		currLvl.InitTileObjects();

		state = State.Normal;
		turn = 0;
	}

	#region Level Managing
	public void LoadLevel(int lvlIdx){
		if (lvlIdx >=levelList.Count) { Debug.LogError("lvlIdx invalid:" + lvlIdx); return; }
		currLevelIdx = lvlIdx;
		currLvl = levelList[lvlIdx];
		currLvl.LoadTiles();

		GameUI.I.OpenMenu();

		Camera.main.transform.position = (Vector3)currLvl.GetCenterPos() - Vector3.forward * 10f;

		StartLevel();
	}

	public int GetLevelCount() {
		return levelList.Count;
	}
	#endregion Level Managing


	private void TryMovePlayer(Vec2i move){
		Vec2i endPos = playerPos + move;
		Log("TryMovePlayer - move: " + move + ", currLevel.IsValidTile(endPos): "+ currLvl.IsValidTile(endPos));
		if (currLvl.IsValidTile(endPos) && currLvl.IsWalkable(endPos, turn)){
			ExecuteTurn(move, endPos);
		}
	}

	private void ExecuteTurn(Vec2i move, Vec2i endPos) {
		Log("Move - move: "+ move + ", endPos: "+ endPos + ", endPos tile: "+ currLvl.GetTileType(endPos)); 

		//Set Player position, interact with end tile
		playerPos = endPos;
		playerObj.transform.position = (Vector3)GameHelper.TileToWorldPos(endPos) + currLvl.transform.position;

		// interact with end tile
		if (PlayerTileInteraction()) return;

		//Update TileObjects
		currLvl.UpdateTileObjects();

		//Check for interaction between player and tileObjects
		if (PlayerTOInteraction()) return;

		//Go to next turn
		turn++;
		GameUI.I.SetTurnsText(turn);
	}

	private bool PlayerTileInteraction() {
		TileType tileTyp = currLvl.GetTileType(playerPos);
		if (tileTyp == TileType.Bucket){
			currPlayerColor = currLvl.GetTileColorType(playerPos);
			playerObj.GetComponent<SpriteRenderer>().color = SpriteLibrary.GetTileColor(currPlayerColor);
		}else{
			if (currPlayerColor != TileColor.None){  //Paint tile, if not already painted OR if dry - with player color
				currLvl.PaintTile(playerPos, currPlayerColor, turn);
			}
		}

		if (tileTyp == TileType.Goal && currLvl.CheckForWin()){
			WinLevel();
			return true;
		}
		return false;
	}

	private bool PlayerTOInteraction() {
		foreach (var to in currLvl.Map.GetTileObjectsAtPos(playerPos)) {
			to.PlayerEntered();

			//if (to.GetType() == typeof(Spikes)) {
			//	SpikesDefintion spikesDef = (SpikesDefintion)to.ToDef;
			//	if (spikesDef.isRaised) {
			//		LoseLevel();
			//		return true;
			//	}
			//}
		}
		return false;
	}

	private void LoseLevel() {
		Debug.Log("LOSE!");
		state = State.Disabled;
		GameUI.I.SetWinText(false);

		CRManager.CallAfterTime(2f, () => {
			GoTomMenu();
		});
	}
	private void WinLevel() {
		Debug.Log("WIN!");
		state = State.Disabled;
		PlayerPrefs.SetInt("Best_" + currLevelIdx, turn);
		GameUI.I.SetWinText(true);

		CRManager.CallAfterTime(2f, () => {
			GoTomMenu();
		});
	}

	public void GoTomMenu() {
		currLvl.UnloadMap();

		MainMenu.I.OpenMenu();
		GameUI.I.CloseMenu();
	}

	public void RetryLevel() {
		currLvl.UnloadMap();
		LoadLevel(currLevelIdx);
	}
}
