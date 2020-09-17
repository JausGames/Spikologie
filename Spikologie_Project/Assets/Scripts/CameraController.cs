using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Player[] players;
    [SerializeField] private List<Player> alives;
    [SerializeField] private float smoothness = 3f;
    [SerializeField] private GameObject display;
    [SerializeField] private bool end = false;

    private void Awake()
    {
        alives.AddRange(players);
    }
    public void Resetgame(Player[] players)
    {
        alives.Clear();
        alives.AddRange(players);
    }
    public void SetEnd(bool value)
    {
        end = value;
    }
    // Update is called once per frame
    void Update()
    {
        if (end)
        {
            transform.position = alives[0].transform.position + 10 * Vector3.back;
            GetComponent<Camera>().orthographicSize = 3f;
        }
        else
        {

            var dist = Mathf.Abs(players[0].transform.position.y - players[1].transform.position.y);
            foreach (Player player in players)
            {
                if (player.transform.position.y < -7f)
                {
                    alives.Remove(player);
                    end = true;
                }
            }
            GetComponent<Camera>().orthographicSize = Mathf.Clamp(dist + 1f, 6f, dist + 2f);
            transform.position = ((players[0].transform.position.y + players[1].transform.position.y) / 2f ) * Vector3.up + 10 * Vector3.back + 1.5f * Vector3.up;
        }
    }
}
