using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : Controller<Game> {

	[SerializeField] List<Level> levelList;

	[SerializeField] GameObject playerObj;

	Vec2i playerPos;
	TileColor currPlayerColor;

	private Level currLevel;

	bool started = false;

	int turn = 0;
	public int Turn{
		get{ return turn; }
	}


	void Start(){
		turn = 0;
		LoadLevel(0);
	
	}

	public void LoadLevel(int lvlIdx){
		if (lvlIdx >=levelList.Count) { Debug.LogError("lvlIdx invalid:" + lvlIdx); return; }

		currLevel = levelList[lvlIdx];
		currLevel.Init();

		Vec2i startPos = currLevel.GetStartPos();

		playerPos = startPos;
		playerObj.transform.position = GameHelper.PosToVec2(startPos);

		started = true;;
	}



	void Update(){
		if (!started) return;

		int xMove = 0, yMove = 0;
		
		if (Input.GetKeyDown(KeyCode.U)){
			xMove = (playerPos.y % 2 == 0 ? -1 : 0);
			yMove = 1;
		}else
		if (Input.GetKeyDown(KeyCode.I)){
			xMove = (playerPos.y % 2 == 0 ? 0 : 1);
			yMove = 1;
		}else
		if (Input.GetKeyDown(KeyCode.K)){
			xMove = 1;
		}else
		if (Input.GetKeyDown(KeyCode.M)){
			xMove = (playerPos.y % 2 == 0 ? 0 : 1);
			yMove = -1;
		}else
		if (Input.GetKeyDown(KeyCode.N)){
			xMove = (playerPos.y % 2 == 0 ? -1 : 0);
			yMove = -1;
		}else
		if (Input.GetKeyDown(KeyCode.H)){
			xMove = -1;
		}

		if (xMove != 0 || yMove != 0){

			MovePlayer(new Vec2i(xMove, yMove));
		}
	}


	private void MovePlayer(Vec2i move){
		Vec2i endPos = playerPos + move;
		if (currLevel.IsValidTile(endPos) && currLevel.IsWalkable(endPos, turn)){
			Debug.Log("[Game] Move - move: "+ move + ", endPos: "+ endPos + ", endPos tile: "+ currLevel.GetTileType(endPos)); 

			playerPos = endPos;
			playerObj.transform.position = GameHelper.PosToVec2(playerPos);


			TileType tileTyp = currLevel.GetTileType(playerPos);
			if (tileTyp == TileType.Goal){
				Debug.Log("WIN! (TODO!)");
			}else if (tileTyp == TileType.Bucket){
				currPlayerColor = currLevel.GetTileColorType(playerPos);
				playerObj.GetComponent<SpriteRenderer>().color = SpriteLibrary.GetTileColor(currPlayerColor);
			}else{
				if (currPlayerColor != TileColor.None){  //Paint tile, if not already painted OR if dry - with player color
					currLevel.PaintTile(playerPos, currPlayerColor, turn);
				}
			}

			turn++;
		}
	}

}
