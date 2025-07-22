using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasUis : MonoBehaviour
{
    public TextMeshProUGUI healthdisplay;
    PlayerControl player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        healthdisplay.text = "Health: "+ player.health.ToString();
    }
}
