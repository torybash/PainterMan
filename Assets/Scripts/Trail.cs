using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Trail : MonoBehaviour {

	private Material _mat;
	private Material Mat {
		get {
			if (_mat == null) _mat = GetComponent<SpriteRenderer>().material;
			return _mat;
		}
	}

	private TileColor _tileClr;
	public TileColor TileClr{ get { return _tileClr; } }
	private int _paintedTurn;
	public int PaintedTurn { get { return _paintedTurn; } }

	public void Init(TileColor tileClr, int paintedTurn) {
		_tileClr = tileClr;
		_paintedTurn = paintedTurn;

		Mat.color = SpriteLibrary.GetTileColor(tileClr);
		//Mat.SetColor("_Color", SpriteLibrary.GetTileColor(tileClr));
	}

	public void UpdateColor() {
		//if (_paintedTurn + GameRules.GetTimeToDry(_tileClr) <= Game.I.Turn) {

		//} else {

		float fracDry = 1f - ((_paintedTurn + GameRules.GetTimeToDry(_tileClr) < Game.I.Turn) ? 0f : 0.5f);

		if (GameRules.PaintGradient) {
			fracDry = 1f - (Game.I.Turn - 1 - _paintedTurn) / (float) GameRules.GetTimeToDry(_tileClr);
		}
		fracDry = Mathf.Clamp01(fracDry);
		Debug.Log("Trail UpdateColor - fracDry: "+ fracDry);

		var col = SpriteLibrary.GetTileColor(_tileClr);
		//Mat.SetColor("_Color", new Color(col.r * fracDry, col.g * fracDry, col.b * fracDry));
		Mat.color = new Color(col.r * fracDry, col.g * fracDry, col.b * fracDry);
		//}
	}

	public void SetVisible(float frac) {
		Mat.SetFloat("_Scroll", frac);
	}
}
