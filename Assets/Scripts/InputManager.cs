using UnityEngine;
using System.Collections;

public class InputManager : Manager<InputManager> {

	#region Variables
	private const bool DEBUG_TOUCH_INPUT = true;

	private float touchMinMove = 0.5f;
	private bool isHoldingMouseButton = false;
	private Vector2 startTouchPos;

	#endregion Variables

	public HexDirection InputUpdate() {
		HexDirection dir = HexDirection.None;
		if (Application.isMobilePlatform || DEBUG_TOUCH_INPUT) {
			dir = TouchInput();
		}
#if UNITY_EDITOR
		HexDirection keyboardDir = KeyboardInput();
		if (keyboardDir != HexDirection.None) dir= keyboardDir;
#endif
		return dir;
	}



	private HexDirection KeyboardInput() {
		if (Input.GetKeyDown(KeyCode.U)){
			return HexDirection.Up_Left;
		}else if (Input.GetKeyDown(KeyCode.I)){
			return HexDirection.Up_Right;
		}else if (Input.GetKeyDown(KeyCode.K)){
			return HexDirection.Right;
		}else if (Input.GetKeyDown(KeyCode.M)){
			return HexDirection.Down_Right;
		}else if (Input.GetKeyDown(KeyCode.N)){
			return HexDirection.Down_Left;
		}else if (Input.GetKeyDown(KeyCode.H)){
			return HexDirection.Left;
		}
		return HexDirection.None;
	}



	private HexDirection TouchInput() {
		HexDirection dir = HexDirection.None;
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
						dir = InputHelper.GetMoveFromVector(diffVec);
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
					dir = InputHelper.GetMoveFromVector(diffVec);
				}
				isHoldingMouseButton = false;
			}
		}


		return dir;
	}
}



[System.Serializable]
public enum HexDirection {
	None,
	Right,
	Left,
	Up_Right,
	Up_Left,
	Down_Right,
	Down_Left,
}