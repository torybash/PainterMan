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

	public static Vec2i GetMoveFromVector(Vector2 vec, bool evenYPos) {
		Vec2i move = Vec2i.Zero;
		float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
		if (angle > 150 || angle < -150) {
			move.x = -1;
		}else if (angle < 150 && angle > 90) {
			move.x = (evenYPos ? -1 : 0);
			move.y = 1;
		} else if (angle < 90 && angle > 30) {
			move.x = (evenYPos ? 0 : 1);
			move.y = 1;
		}else if (angle < 30 && angle > -30) {
			move.x = 1;
		}else if (angle < -30 && angle > -90) {
			move.x = (evenYPos ? 0 : 1);
			move.y = -1;
		}else if (angle < -90 && angle > -150) {
			move.x = (evenYPos ? -1 : 0);
			move.y = -1;
		}
		//Debug.Log("GetMoveFromVector - vec: " + vec + ", angel: " + angle + " , move: " + move);
		return move;
	}
}
