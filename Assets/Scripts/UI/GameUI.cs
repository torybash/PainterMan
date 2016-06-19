using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : Controller<GameUI> {

	[Header("UI references")]
	[SerializeField] RectTransform gamePanel;
	[SerializeField] Text turnsText;
	[SerializeField] Text winText;

	void Start() {
		CloseMenu();
	}

	public void OpenMenu() {
		gamePanel.gameObject.SetActive(true);
		SetTurnsText(0);
		winText.text = "";
	}
	public void CloseMenu() {
		gamePanel.gameObject.SetActive(false);
	}

	public void SetTurnsText(int turn) {
		turnsText.text = "Turn: " + turn;
	}

	public void SetWinText(bool won) {
		winText.text = won ? "WON!" : "Lose..";
	}


	#region Button clicks
	public void ClickGoToMenu() {
		Game.I.GoTomMenu();
	}
	public void ClickRetry() {
		Game.I.RetryLevel();
	}
	#endregion Button clicks
}
