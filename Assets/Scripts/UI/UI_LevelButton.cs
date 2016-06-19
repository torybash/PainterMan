using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_LevelButton : MonoBehaviour {

	[SerializeField] Text lvlNumberText;
	[SerializeField] Text bestText;

	public void Init(int lvlIdx) {
		lvlNumberText.text = "" + (lvlIdx + 1);

		if (PlayerPrefs.HasKey("Best_" + lvlIdx)) {
			bestText.gameObject.SetActive(true);
			bestText.text = "Best: " + PlayerPrefs.GetInt("Best_" + lvlIdx);
		} else {
			bestText.gameObject.SetActive(false);
		}
	}
}
