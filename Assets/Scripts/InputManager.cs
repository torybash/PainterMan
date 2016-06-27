using UnityEngine;
using System.Collections;

public class InputManager : Manager<InputManager> {

	#region Variables
	private const bool DEBUG_TOUCH_INPUT = true;

	private float touchMinMove = 0.5f;


	private bool isHoldingMouseButton = false;
	private Vector2 startTouchPos;

	#endregion Variables

	public Vec2i InputUpdate(bool evenYPos) {
		Vec2i move = Vec2i.Zero;
		if (Application.isMobilePlatform || DEBUG_TOUCH_INPUT) {
			move = TouchInput(evenYPos);
		}
#if UNITY_EDITOR
		Vec2i keyboardInput = KeyboardInput(evenYPos);
		if (keyboardInput != Vec2i.Zero) move = keyboardInput;
#endif
		return move;
	}



	private Vec2i KeyboardInput(bool evenYPos) {
		int xMove = 0, yMove = 0;
		if (Input.GetKeyDown(KeyCode.U)){
			xMove = (evenYPos ? -1 : 0);
			yMove = 1;
		}else
		if (Input.GetKeyDown(KeyCode.I)){
			xMove = (evenYPos ? 0 : 1);
			yMove = 1;
		}else
		if (Input.GetKeyDown(KeyCode.K)){
			xMove = 1;
		}else
		if (Input.GetKeyDown(KeyCode.M)){
			xMove = (evenYPos ? 0 : 1);
			yMove = -1;
		}else
		if (Input.GetKeyDown(KeyCode.N)){
			xMove = (evenYPos ? -1 : 0);
			yMove = -1;
		}else
		if (Input.GetKeyDown(KeyCode.H)){
			xMove = -1;
		}
		return new Vec2i(xMove, yMove);
	}



	private Vec2i TouchInput(bool evenYPos) {
		Vec2i move = Vec2i.Zero;
		if (Application.isMobilePlatform) {
			if (Input.touchCount > 0) {
				Touch touch = Input.touches[0];
				switch (touch.phase) {
				case TouchPhase.Began:
					startTouchPos = touch.position;
					break;
				case TouchPhase.Ended:
					Vector2 diffVec = touch.position - startTouchPos;
					if (diffVec.magnitude > touchMinMove) {
						move = InputHelper.GetMoveFromVector(diffVec, evenYPos);

					}
					break;
				default:
					break;
				}
			}
		} else {
			if (Input.GetMouseButtonDown(0)) {
				Vector2 clickPos = Input.mousePosition;
				if (Input.GetMouseButtonDown(0)) {
					startTouchPos = clickPos;
					isHoldingMouseButton = true;
				}
			}else if (Input.GetMouseButtonUp(0) && isHoldingMouseButton) {
				Vector2 clickPos = Input.mousePosition;
				Vector2 diffVec = clickPos - startTouchPos;
				if (diffVec.magnitude > touchMinMove) {
					move = InputHelper.GetMoveFromVector(diffVec, evenYPos);
				}
				isHoldingMouseButton = false;
			}
		}


		return move;
	}
}
