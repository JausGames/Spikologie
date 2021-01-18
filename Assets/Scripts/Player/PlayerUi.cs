using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUi : MonoBehaviour
{
    //[SerializeField] private Player player;
    [SerializeField] private Text percent;
    [SerializeField] private Image background;

    public void SetPercent(float per)
    {
        var nb = Mathf.Round(per * 100)/100;
        percent.text = nb.ToString() + " %";
        var r = Mathf.Clamp(1f - 0.4f * nb / 300, 0.6f, 1f);
        var gb = Mathf.Clamp(1f - nb / 300f, 0f, 1f);
        background.color = new Color(r, gb, gb, 1f);
    }
}
