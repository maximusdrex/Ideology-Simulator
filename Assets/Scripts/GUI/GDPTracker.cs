﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDPTracker : MonoBehaviour
{
    private Player p;
    private GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        p = gm.GetPlayer(Camera.main.GetComponent<PlayerManager>().playerId);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<UnityEngine.UI.Text>().text = "GDP: " + p.GDP.ToString();
    }
}