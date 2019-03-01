using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private HexMap gameMap;
    public GameObject testObj;
    public int turn;
    private List<Player> players;

    //Called before other Starts()
    void Awake()
    {
        gameMap = GameObject.FindObjectOfType<HexMap>();
        turn = 0;
        players = new List<Player>();
    }

    void Start()
    {
        int numPlayers = 2;
        players.Add(new Player(0));
        for(int i = 1; i < numPlayers; i++)
        {
            players.Add(new AIPlayer(i));
        }
        while (turn < 150)
        {
            for(int i = 0; i < players.Count; i++)
            {
                players[i].PlayTurn();
                Debug.Log("Player: " + players[i].id.ToString() + " played turn " + turn.ToString());
            }
            turn++;
        }
    }

    void Update()
    {
        placeOnHex(testObj, 20, 23);
    }

    bool placeOnHex(GameObject obj, int x, int y)
    {
        obj.transform.position = gameMap.getHexObj(x, y).transform.position;
        return true;
    }
}
