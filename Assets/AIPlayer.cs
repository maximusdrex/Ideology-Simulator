using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{


    public AIPlayer(int pid, GameObject canvas) : base(pid, canvas)
    {

    }

    public override void StartTurn()
    {
        isTurn = true;
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.nextTurnPressed();
    }
}