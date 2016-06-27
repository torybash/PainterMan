using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : Controller<MainMenu> {

	[Header("UI references")]
	[SerializeField] RectTransform mainMenuPanel;

	[SerializeField] RectTransform mainPanel;
	[SerializeField] RectTransform levelSelectPanel;
	[SerializeField] RectTransform levelSelectGroup;

	[Header("Templates")]
	[SerializeField] UI_LevelButton levelButtonTemplate;

	private List<UI_LevelButton> levelButtonList = new List<UI_LevelButton>();

	void Start() {
		OpenMenu();
	}

	public void OpenMenu() {
		mainMenuPanel.gameObject.SetActive(true);
		OpenPanel(mainPanel);
	}

	#region Button clicks
	public void ClickStartGame() {
		StartGameLevel(0);
	}

	public void ClickLevelSelect() {
		OpenPanel(levelSelectPanel);
		InitLevelSelectPanel();
	}

	public void ClickToMain() {
		OpenPanel(mainPanel);
	}

	public void ClickLevel(int lvlIdx) {
		StartGameLevel(lvlIdx);
	}
	#endregion Button clicks


	private void InitLevelSelectPanel() {
		foreach (var item in levelButtonList) if (item) Destroy(item.gameObject);
		levelButtonList.Clear();

		for (int i = 0; i < LevelLibrary.I.GetLevelCount(); i++) {
			UI_LevelButton lvlBtn = Instantiate<UI_LevelButton>(levelButtonTemplate);
			lvlBtn.transform.SetParent(levelSelectGroup);
			lvlBtn.transform.localScale = Vector3.one;
			lvlBtn.transform.localPosition = Vector3.zero;
			lvlBtn.gameObject.SetActive(true);

			int lvlIdx = i;
			lvlBtn.GetComponent<Button>().onClick.AddListener(() => ClickLevel(lvlIdx));
			lvlBtn.Init(i);
			levelButtonList.Add(lvlBtn);
		}
	}

	private void OpenPanel(RectTransform panel) {
		mainPanel.gameObject.SetActive(false);
		levelSelectPanel.gameObject.SetActive(false);

		panel.gameObject.SetActive(true);
	}

	private void StartGameLevel(int lvlIdx) {
		mainMenuPanel.gameObject.SetActive(false);

		Game.I.LoadLevel(lvlIdx);
	}
}
