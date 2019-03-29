using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{


    public AIPlayer(int pid, bool communist) : base(pid,  communist)
    {

    }

    public override void StartTurn()
    {
        isTurn = true;
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.nextTurnPressed();
    }
}