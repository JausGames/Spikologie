using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{
	public Button no;
	public Button yes;
	public bool playAgain = false;

	void Start()
	{
		no.onClick.AddListener(QuitGame);
		yes.onClick.AddListener(Rematch);
	}

	void QuitGame()
	{
		Destroy(MatchManager.instance.gameObject);
		SceneManager.LoadScene("MainMenu");
	}
	void Rematch()
	{
		Debug.Log("REMATCH");
		gameObject.SetActive(false);
		MatchManager.instance.ResetGame();
	}
}
