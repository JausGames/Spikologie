using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //[SerializeField] private int nbPlayers = 1;
    //[SerializeField] GameObject MatchManagerPrefab;
    //[SerializeField] MapHandler mapHandler;
    //[SerializeField] private Text UICount;
    [SerializeField] private Button play;
    [SerializeField] private Button quit;
    //public Scrollbar scrollbar;


    void Start()
    {

        /*SetNbPlayer(1);
        scrollbar.onValueChanged.AddListener((float val) => SetNbPlayer(val * 3 + 1));*/
        play.onClick.AddListener(ChangeScene);
        quit.onClick.AddListener(QuitGame);
    }

    /*public void SetNbPlayer(float nb)
    {
        UICount.text = Mathf.RoundToInt(nb).ToString();
        nbPlayers = Mathf.RoundToInt(nb);
    }*/

    private void ChangeScene()
    {
        //if (MatchManager.instance == null) Instantiate(MatchManagerPrefab);
        //var map = mapHandler.GetMapByName("Map01");
        /*var spawnwPos = map.GetPositions();
        PlayerManager.instance.SetSpawnPositions(spawnwPos);
        MatchManager.instance.SetNbPlayers(nbPlayers);*/
        SceneManager.LoadScene("BaseArena");
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}
