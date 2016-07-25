using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game : Controller<Game> {

	public enum State { Disabled, Normal, Animating}

	#region Variables
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
		HexDirection dir = InputManager.I.InputUpdate();

		if (dir != HexDirection.None) TryMovePlayer(dir);
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
		currLevelIdx = lvlIdx;
		currLvl = LevelLibrary.I.InstantiateLevel(lvlIdx);
		currLvl.LoadTiles();

		GameUI.I.OpenMenu();

		Camera.main.transform.position = (Vector3)currLvl.GetCenterPos() - Vector3.forward * 10f;

		StartLevel();
	}
	#endregion Level Managing


	private void TryMovePlayer(HexDirection dir) {
		Vec2i endPos = GameHelper.PositionFromDirection(playerPos, dir);
		Log("TryMovePlayer - dir: " + dir + ", currLevel.IsValidTile(endPos): "+ currLvl.IsValidTile(endPos));
		//if (GameRules.)
		if (currLvl.IsWalkable(endPos, turn)){
			//while (currLvl.IsWalkable(endPos + move, turn)) endPos += move;
			ExecuteMove(playerPos, dir);
		}
	}

	private void ExecuteMove(Vec2i startPos, HexDirection dir) {
		Vec2i endPos = GameHelper.PositionFromDirection(startPos, dir);
		Log("Move - endPos: " + endPos + ", endPos tile: " + currLvl.GetTileType(endPos));

		//Set Player position, interact with end tile
		//playerPos = endPos;
		//playerObj.transform.position = (Vector3)GameHelper.TileToWorldPos(endPos) + currLvl.transform.position;

		// interact with end tile
		if (GameRules.PaintBehindPlayer) {
			if (PlayerTileInteraction(startPos)) return;
		} else {
			if (PlayerTileInteraction(endPos)) return;
		}
	
		//Check for interaction between player and tileObjects
		if (PlayerTOInteraction()) return;

		//Slide
		if (GameRules.NormalTilesCausesSlide) {
			Vec2i newEndPos = GameHelper.PositionFromDirection(endPos, dir);
			if (currLvl.IsWalkable(newEndPos, turn)) {
				ExecuteMove(endPos, dir);
				return;
			}
		}

		StartCoroutine(_MovePlayer(endPos, () => {
			//Go to next turn
			currLvl.UpdateTileObjects(); //Update TileObjects
			turn++;
			GameUI.I.SetTurnsText(turn);
		}));

	}





	private bool PlayerTileInteraction(Vec2i pos) {
		TileType tileTyp = currLvl.GetTileType(pos);
		if (tileTyp == TileType.Bucket){
			currPlayerColor = currLvl.GetTileColorType(pos);
			playerObj.GetComponent<SpriteRenderer>().color = SpriteLibrary.GetTileColor(currPlayerColor);
		}

		if (currPlayerColor != TileColor.None){  //Paint tile, if not already painted OR if dry - with player color
			if (tileTyp != TileType.Bucket || GameRules.PaintBucketTiles) currLvl.PaintTile(pos, currPlayerColor, turn);
		}

		if (tileTyp == TileType.Goal && currLvl.CheckForWin()){
			WinLevel();
			return true;
		}
		return false;
	}

	private bool PlayerTOInteraction() {
		foreach (var to in currLvl.Map.GetTileObjectsAtPos(playerPos)) {
			var result = to.PlayerEntered();
			switch (result.type) {
			case TileObjectInteractionResultType.Kill:
				LoseLevel();
				return true;

			case TileObjectInteractionResultType.Teleport:
				//Set Player position, interact with end tile
				//playerPos = result.position;
				//playerObj.transform.position = (Vector3)GameHelper.TileToWorldPos(result.position) + currLvl.transform.position;

				// interact with end tile
				//if (PlayerTileInteraction()) return true;
				break;

			default: break;
			}
			 //if (result.kill)
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


	#region Coroutines
	private IEnumerator _MovePlayer(Vec2i endPos, System.Action callback) {
		float duration = 0.5f;
		float endTime = Time.time + duration;
		Vector2 startWPos = GameHelper.TileToWorldPos(playerPos) + (Vector2)currLvl.transform.position;
		Vector2 endWPos = GameHelper.TileToWorldPos(endPos) + (Vector2)currLvl.transform.position;
		while (Time.time < endTime) {
			float t = 1 - (endTime - Time.time) / duration; 
			playerObj.transform.position = startWPos +  (endWPos - startWPos) * t;
			yield return null;
		}
		playerPos = endPos;
		playerObj.transform.position = (Vector3)GameHelper.TileToWorldPos(endPos) + currLvl.transform.position;

		callback();
	}

	#endregion Coroutines
}
