using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levy : MonoBehaviour
{
    public Player owner;
    private GameManager gm;
    private PlayerManager playerManager;

    public void levyExecute()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        owner = gm.playing;
        if (owner.money > 50000)
        {
            owner.money -= 50000;
        }
        else
        {
            Debug.Log("not enough bread");
        }
    }
}
