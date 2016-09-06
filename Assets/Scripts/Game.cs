using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game : Controller<Game> {

	public enum State { Disabled, Normal, Animating}

	#region Fields
	[SerializeField] private Slug slugPrefab;

	[SerializeField] [ReadOnly] private State state;



	private List<Slug> slugList = new List<Slug>();

	private int currLevelIdx;
	private Level currLvl;
	public Level Lvl {
		get { return currLvl; }
	}

	private int turn = 0;
	public int Turn{
		get{ return turn; }
	}

	int animCount = 0;

	TrailSystem trailSys;
	#endregion Fields


	#region Lifetime	
	void Awake() {
		trailSys = new TrailSystem();
	}
	void Update(){
		if (state != State.Normal) return;
		HexDirection dir = InputManager.I.InputUpdate();
		if (dir != HexDirection.None) TryMovePlayer(dir);
	}
	#endregion Lifetime


	public bool IsAnimating() {
		return animCount > 0;
	}

	private void StartLevel() {

		//Cleanup
		foreach (var slug in slugList) {
			if (slug != null) Destroy(slug.gameObject);
		}
		slugList.Clear();

		//Spawn slug(s)
		List<Entrance> entranceList = currLvl.GetEntranceList();
		foreach (var ent in entranceList) {
			var slug = Instantiate<Slug>(slugPrefab);
			slug.SetPosition(ent.ToDef.pos);
			slug.SetColor(((EntranceDefinition)ent.ToDef).color);
			slugList.Add(slug);
		}

		currLvl.InitTileObjects();

		state = State.Normal;
		turn = 0;
		animCount = 0;
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
		bool moved = false;
		foreach (var slug in slugList) {
			Vec2i endPos = GameHelper.PositionFromDirection(slug.pos, dir);
			Log("TryMovePlayer - dir: " + dir + ", currLevel.IsValidTile(endPos): "+ currLvl.IsValidTile(endPos));
			if (IsWalkable(slug, endPos)){
				slug.isOnExit = false;
				animCount++;
				ExecuteMove(slug, dir);
			}
			moved = true;
		}

		if (moved) {
			if (GameRules.UpdateSpikesBeforeMove) currLvl.UpdateTileObjects(); //Update TileObjects
		}
	}

	private bool IsWalkable(Slug slug, Vec2i pos) {
		foreach (var item in slugList) {
			if (item != slug && item.pos == pos) return false;
		}
		if (currLvl.IsWalkable(pos, turn)) {
			if (GameRules.CantMoveOverOtherColors && currLvl.GetTileType(pos) != TileType.Bucket && currLvl.GetTileColorType(pos) != TileColor.None && slug.color != currLvl.GetTileColorType(pos)) {
				return false;
			}
			return true;
		}
		return false;
	}

	

	private void ExecuteMove(Slug slug, HexDirection dir) {
		Vec2i startPos = slug.pos;
		Vec2i endPos = GameHelper.PositionFromDirection(slug.pos, dir);
		Log("Move - endPos: " + endPos + ", endPos tile: " + currLvl.GetTileType(endPos));


		//Animate move
		StartCoroutine(_MoveSlug(slug, endPos, () => {
			// interact with end tile
			if (GameRules.PaintBehindPlayer) {
				SlugTileInteraction(slug, startPos);
			} else {
				SlugTileInteraction(slug, endPos);
			}

			//Check for sliding
			bool sliding = false;
			if (GameRules.NormalTilesCausesSlide) {
				Vec2i newEndPos = GameHelper.PositionFromDirection(slug.pos, dir);
				if (IsWalkable(slug, newEndPos)) {
					sliding = true;
				}
			}

			//Check for interaction between player and tileObjects
			if (SlugTOInteraction(slug, endPos, sliding)) return;

			//Slide!
			if (sliding) {
				if (GameRules.TurnsCountWhenSliding) turn++;
				ExecuteMove(slug, dir);
				return;
			}

			//Go to next turn
			animCount--;
			if (animCount < 0) Debug.LogError("animCount < 0! - animCount: " + animCount);
			if (animCount == 0) {
				EndTurn();
			}
		}));
	}

	private void EndTurn() {
		foreach (var slug in slugList) {
			SlugTOInteraction(slug, slug.pos, false);
		}

		if (!GameRules.UpdateSpikesBeforeMove) currLvl.UpdateTileObjects(); //Update TileObjects

		turn++;
		GameUI.I.SetTurnsText(turn);
	}

	private void SlugTileInteraction(Slug slug, Vec2i pos) {
		TileType tileTyp = currLvl.GetTileType(pos);
		//if (tileTyp == TileType.Bucket) {
		//	ColorPlayer(currLvl.GetTileColorType(pos));
		//	if (GameRules.RemoveBucketsOnEnter) {
		//		TileDefinition newTileDef = currLvl.Map.GetTile(pos).TileDef;
		//		newTileDef.type = TileType.Normal;
		//		currLvl.Map.SetTile(pos, newTileDef);
		//		currLvl.Map.GetTile(pos).Refresh();
		//	}
		//}

		if (slug.color != TileColor.None) {  //Paint tile, if not already painted OR if dry - with player color
			if (tileTyp != TileType.Bucket || GameRules.PaintBucketTiles) currLvl.PaintTile(pos, slug.color, turn);
		}

		//if (tileTyp == TileType.Goal && currLvl.CheckForWin()) {
		//	WinLevel();
		//	return true;
		//}
		//return false;
	}

	private bool SlugTOInteraction(Slug slug, Vec2i pos, bool sliding) {
		foreach (var to in currLvl.Map.GetTOAtPos(pos)) {
			var result = to.PlayerEntered();
			Debug.Log("PlayerTOInteraction - sliding: " + sliding + ", result.type: "+ result.type);

			switch (result.type) {
			case TileObjectInteractionResultType.Kill:
				LoseLevel();
				return true;

			case TileObjectInteractionResultType.Teleport:
				Debug.LogError("CURRENTLY NOT WORKING - TODO!");
				//Set Player position, interact with end tile
				//playerPos = result.position;
				//playerObj.transform.position = (Vector3)GameHelper.TileToWorldPos(result.position) + currLvl.transform.position;

				// interact with end tile
				//if (PlayerTileInteraction()) return true;
				break;
			case TileObjectInteractionResultType.PickupColor:
				slug.SetColor(result.color);
				currLvl.Map.DeleteTOAtPos(pos, to);
				break;
			case TileObjectInteractionResultType.Exit:
				if (!sliding) {
					Log("Slug on exit - exit color: " + ((ExitDefinition)to.ToDef).color + ", slug color: " + slug.color);
					if (((ExitDefinition)to.ToDef).color == TileColor.None || ((ExitDefinition)to.ToDef).color == slug.color) {
						slug.isOnExit = true;

						if (GameRules.RemoveSlugsWhenTouchExit) {
							slugList.Remove(slug);
							Destroy(slug.gameObject);
						}

						bool allSlugsOnExit = true;
						foreach (var otherSlug in slugList) {
							if (!otherSlug.isOnExit) {
								allSlugsOnExit = false;
							}
						}
						if (allSlugsOnExit) {
							WinLevel();
							return true;
						}
					}
				}
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
			//GoTomMenu();
			RetryLevel();
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
	private IEnumerator _MoveSlug(Slug slug, Vec2i endPos, System.Action callback) {
		Vector2 startWPos = GameHelper.TileToWorldPos(slug.pos) + (Vector2)currLvl.transform.position;
		Vector2 endWPos = GameHelper.TileToWorldPos(endPos) + (Vector2)currLvl.transform.position;

		slug.transform.rotation = GameHelper.GetRotationFromVector(endWPos - startWPos);

		var trail = trailSys.CreateTrail(startWPos, endWPos, slug.color);

		float duration = GameRules.AnimDurationPrTile * (Vector2.Distance(startWPos, endWPos) / Mathf.Sqrt(3f)); 
		float endTime = Time.time + duration;
		while (Time.time < endTime) {
			float t = 1 - (endTime - Time.time) / duration; 
			slug.transform.position = startWPos +  (endWPos - startWPos) * t;
			trail.SetVisible(1-t);
			yield return null;
		}
		slug.SetPosition(endPos);

		callback();
	}

	#endregion Coroutines
}
