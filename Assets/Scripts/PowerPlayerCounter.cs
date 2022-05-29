using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PowerPlayerCounter : MonoBehaviour
{
    public Text textCounter;
    private Player player;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.powerPlayerCounter >= 0f && player.powerPlayerCounter <= 15f)
        {
            GetComponent<Canvas>().enabled = true;
            textCounter.text = String.Format("{0:0}", player.powerPlayerCounter);
        }
        else
        {
            GetComponent<Canvas>().enabled = false;
        }
    }
}
