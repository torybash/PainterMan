using UnityEngine;
using System.Collections;

public class InputHelper {

	public static int GetNumberPressed(KeyCode key) {
		int keyVal = (int)key;
		if (keyVal >= (int) KeyCode.Alpha0 && keyVal <= (int)KeyCode.Alpha9) {
			return keyVal - (int) KeyCode.Alpha0;
		}
		//Debug.LogError("[InputHelper] GetNumberPressed invalid key: " + key);
		return -1;
	}

	public static HexDirection GetMoveFromVector(Vector2 vec) {
		float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
		if (angle > 150 || angle < -150) {
			return HexDirection.Left;
		}else if (angle < 150 && angle > 90) {
			return HexDirection.Up_Left;
		} else if (angle < 90 && angle > 30) {
			return HexDirection.Up_Right;
		}else if (angle < 30 && angle > -30) {
			return HexDirection.Right;
		}else if (angle < -30 && angle > -90) {
			return HexDirection.Down_Right;
		}else if (angle < -90 && angle > -150) {
			return HexDirection.Down_Left;
		}
		Debug.LogError("GetMoveFromVector - ERROR! Returning null!");
		return HexDirection.None;
	}
}
