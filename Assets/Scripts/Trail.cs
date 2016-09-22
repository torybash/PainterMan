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
	private TileColor _secondTileClr;
	public TileColor SecondTileClr{ get { return _secondTileClr; } }
	private int _paintedTurn;
	public int PaintedTurn { get { return _paintedTurn; } }


	public void Init(TileColor tileClr, int paintedTurn) {
		_tileClr = tileClr;
		_secondTileClr = tileClr;
		_paintedTurn = paintedTurn;

		//Mat.color = SpriteLibrary.GetTileColor(tileClr);
		//Mat.SetColor("_Color", SpriteLibrary.GetTileColor(tileClr));
		//Mat.SetColor("_SecondColor", SpriteLibrary.GetTileColor(tileClr));
		UpdateColor();
	}

	public void UpdateColor() {
		//if (_paintedTurn + GameRules.GetTimeToDry(_tileClr) <= Game.I.Turn) {

		//} else {

		float fracDry = 1f - ((_paintedTurn + GameRules.GetTimeToDry(_tileClr) < Game.I.Turn) ? 0f : 0.5f);

		if (GameRules.PaintGradient) {
			fracDry = 0.4f + 0.6f * (1f - (Game.I.Turn - 1 - _paintedTurn) / (float) GameRules.GetTimeToDry(_tileClr));
		}
		fracDry = Mathf.Clamp01(fracDry);
		Debug.Log("Trail UpdateColor - fracDry: "+ fracDry);

		var col = SpriteLibrary.GetTileColor(_tileClr);
		var secCol = SpriteLibrary.GetTileColor(_secondTileClr);
		//Mat.SetColor("_Color", new Color(col.r * fracDry, col.g * fracDry, col.b * fracDry));
		//Mat.SetColor("_SecondColor", new Color(secCol.r * fracDry, secCol.g * fracDry, secCol.b * fracDry));
		Mat.SetColor("_Color", new Color(col.r, col.g, col.b, fracDry));
		Mat.SetColor("_SecondColor", new Color(secCol.r, secCol.g, secCol.b, fracDry));
		//Mat.color = new Color(col.r * fracDry, col.g * fracDry, col.b * fracDry);
		//}
	}


	public void SetVisible(float frac) {
		Mat.SetFloat("_Scroll", frac);
	}

	public void SetSecondaryColor(TileColor tileClr) {
		//Debug.Log("SetSecondaryColor - tileClr: " + tileClr);
		_secondTileClr = tileClr;
		UpdateColor();
	}
}
