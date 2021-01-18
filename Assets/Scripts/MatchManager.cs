using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MatchManager : MonoBehaviour
{
    [SerializeField] GameObject UI = null;
    [SerializeField] GameObject canvas = null;
    [SerializeField] GameObject playAgainUI = null;
    [SerializeField] List<Player> players = null;
    [SerializeField] List<Player> alives = null;
    [SerializeField] Map map;
    [SerializeField] GameObject timerUI = null;
    [SerializeField] PlayAgain playAgain = null;
    [SerializeField] CameraController camera = null;

    Vector3[] spawns = new Vector3[]
    {   new Vector3(-1.5f, 2.4f, 0),
        new Vector3(1.5f, 2.4f, 0)
    };
    //[SerializeField] int nbPlayers = 0;

    #region Singleton
    public static MatchManager instance;
    //public static PlayerManager playerManager;

    private void Awake()
    {
        instance = this;
        //playerManager = GetComponentInChildren<PlayerManager>();
        UI = transform.Find("UI").gameObject;
        canvas = UI.transform.Find("Canvas").gameObject;
        timerUI = canvas.transform.Find("Timer").gameObject;
        Debug.Log("BUG : " + timerUI);
        timerUI.GetComponent<TimerUI>().SetManager(this);
        playAgainUI = canvas.transform.Find("PlayAgain").gameObject;
        playAgain = playAgainUI.GetComponent<PlayAgain>();
        players.AddRange(FindObjectsOfType<Player>());
        alives.AddRange(FindObjectsOfType<Player>());

    }

    #endregion


    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        foreach (Player player in players)
        {
            foreach (Player opponent in players)
            {
                if (opponent != player) Physics2D.IgnoreCollision(player.GetCollider(), opponent.GetCollider(), true);
            }
        }

        //playerManager.SetMatchUp();
        timerUI.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        alives.Clear();
        foreach(Player player in players)
        {
            if (player.IsAlive())
            {
                alives.Add(player);
            }
        }
        if (alives.Count <= 1 && !playAgain.playAgain)
        {
            camera.SetEnd(true);
            SetInGame(false);
            playAgainUI.SetActive(true);
        }
    }
    public void SetInputBlock(bool value)
    {
        foreach(Player player in players)
        {
            player.SetInputBlock(value);
        }
    }
    public void SetInGame(bool value)
    {
        foreach (Player player in players)
        {
            player.SetInGame(value);
        }
    }

    public void StartGame()
    {
        timerUI.SetActive(false);
        SetInputBlock(false);
        SetInGame(true);


        //playerManager.SetCanMove();
    }
    public void ResetGame()
    {
        Debug.Log("Touça");
        map.ReplaceTiles();
        alives.AddRange(FindObjectsOfType<Player>());
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log("Matchmanager, ResetGame : " + players[i]);
            players[i].StopMotion();
            players[i].ResetPercent();
            players[i].SetAlive(true);
            players[i].transform.position = spawns[i];

        }
        playAgainUI.SetActive(false);
        playAgain.playAgain = false;
        camera.Resetgame(players.ToArray());
        camera.SetEnd(false);
        timerUI.SetActive(true);
        SetInputBlock(true);
    }


}
