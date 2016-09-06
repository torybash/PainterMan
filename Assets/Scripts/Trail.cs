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

	public void Init(TileColor tileClr) {
		//Mat.color = SpriteLibrary.GetTileColor(tileClr);
		Mat.SetColor("_Color", SpriteLibrary.GetTileColor(tileClr));
	}

	public void SetVisible(float frac) {
		Mat.SetFloat("_Scroll", frac);
	}
}
