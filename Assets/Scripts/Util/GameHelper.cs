using UnityEngine;
using System.Collections;

public class GameHelper {

	#region TilePosition<-->WorldPosition
	public static Vector2 TileToWorldPos(Vec2i pos){
		float xPos = Mathf.Sqrt(3f) * pos.x + (Mathf.Abs(pos.y % 2) == 1 ? Mathf.Sqrt(3f)/2f : 0);
		float yPos = 1.5f * pos.y;
		return new Vector2(xPos, yPos);
	}

	public static Vec2i WorldToTilePos(Vector2 pos) {
		int yPos = Mathf.RoundToInt(pos.y / 1.5f);
		int xPos = Mathf.RoundToInt(pos.x / Mathf.Sqrt(3f) - (Mathf.Abs(yPos % 2) == 1 ? 1f * 0.5f : 0));
		return new Vec2i(xPos, yPos);
	}
	#endregion TilePosition<-->WorldPosition


	#region Rotation
	public static Quaternion GetRotationFromVector(Vector2 vec) {
		float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
		return Quaternion.AngleAxis(angle, Vector3.forward);
	}
	#endregion


	public static Vec2i PositionFromDirection(Vec2i pos, HexDirection dir) {
		bool evenYPos = pos.y % 2 == 0;
		Vec2i endPos = pos;
		switch (dir) {
		case HexDirection.Right:
			endPos.x += 1;
			break;
		case HexDirection.Left:
			endPos.x -= 1;
			break;
		case HexDirection.Up_Right:
			endPos.x += (evenYPos ? 0 : 1);
			endPos.y += 1;
			break;
		case HexDirection.Up_Left:
			endPos.x += (evenYPos ? -1 : 0);
			endPos.y += 1;
			break;
		case HexDirection.Down_Right:
			endPos.x += (evenYPos ? 0 : 1);
			endPos.y -= 1;
			break;
		case HexDirection.Down_Left:
			endPos.x += (evenYPos ? -1 : 0);
			endPos.y -= 1;
			break;
		}
		return endPos;
	}

}

[System.Serializable]
public class TileDefinition{
	public TileType type = TileType.Normal;
	public TileColor color = TileColor.None;
	public TileColor goalColor = TileColor.None;
	public Vec2i pos;

	public int paintedTurn = -1;

	public override string ToString() {
		return "TileDefinition - type " + type + ", color: "+ color + ", goalColor: "+ goalColor + ", paintedTurn: "+ paintedTurn;
	}
}

[System.Serializable]
public enum TileType{
	Empty		 		= 0,
	Normal				,
	Bucket				,

	Start				= 10,
	Goal				,
}

[System.Serializable]
public enum TileColor{
	None				= 0,
	Red					,
	Green				,
	Blue				,

	Cyan				,
	Magneta				,
	Yellow				,
}

[System.Serializable]
public struct Vec2i{
	public int x;
	public int y;

	public static Vec2i Zero{
		get{return new Vec2i(0,0);}
	}

	public Vec2i(Vec2i vec){
		this.x = vec.x;
		this.y = vec.y;
	}

	public Vec2i(int x, int y){
		this.x = x;
		this.y = y;
	}

	public override string ToString ()
	{
		return string.Format ("[Vec2i x: {0}, y: {1}]", x, y);
	}

	public override bool Equals(object obj)
	{
		return (obj is Vec2i) ? this == (Vec2i)obj : false;
	}

	public bool Equals(Vec2i other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		int hashCode = 1 + 17 * x + 33 * y;
		return hashCode;
	}

	#region Operators

	public static bool operator ==(Vec2i value1, Vec2i value2)
	{
		return value1.x == value2.x
			&& value1.y == value2.y;
	}

	public static bool operator !=(Vec2i value1, Vec2i value2)
	{
		return !(value1 == value2);
	}

	public static Vec2i operator +(Vec2i value1, Vec2i value2)
	{
		Vec2i result = new Vec2i();
		result.x = value1.x +value2.x;
		result.y = value1.y +value2.y;
		return result;
	}

	public static Vec2i operator -(Vec2i value)
	{
		Vec2i result = new Vec2i(-value.x, -value.y);
		return result;
	}

	public static Vec2i operator -(Vec2i value1, Vec2i value2)
	{
		Vec2i result = new Vec2i();
		result.x = value1.x - value2.x;
		result.y = value1.y - value2.y;
		return result;
	}

	public static Vec2i operator *(Vec2i value1, Vec2i value2)
	{
		Vec2i result = new Vec2i();
		result.x = value1.x * value2.x;
		result.y = value1.y * value2.y;
		return result;
	}

	public static Vec2i operator /(Vec2i value1, Vec2i value2)
	{
		Vec2i result = new Vec2i();
		result.x = value1.x / value2.x;
		result.y = value1.y / value2.y;
		return result;
	}
		
	#endregion
}
	