using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private HexMap gameMap;
    public GameObject testObj;
    public int turn;
    private List<Player> players;
    public Player playing;
    public GameObject canvas;

    //Called before other Starts()
    void Awake()
    {
        gameMap = GameObject.FindObjectOfType<HexMap>();
        turn = 1;
        players = new List<Player>();
    }

    void Start()
    {
        int numPlayers = 5;
        players.Add(new Player(0, canvas));
        for(int i = 1; i < numPlayers; i++)
        {
            players.Add(new AIPlayer(i, canvas));
        }
        playing = players[0];
        playing.StartTurn();
        
    }


    void Update()
    {
        placeOnHex(testObj, 20, 23);
    }

    public bool placeOnHex(GameObject obj, int x, int y)
    {
        obj.transform.position = gameMap.getHexObj(x, y).transform.position;
        return true;
    }

    public void nextTurnPressed()
    {
        playing.EndTurn();
        playing = players[(playing.id+1) % players.Count];
        if(playing.id == 0)
        {
            turn++;
        }
        playing.StartTurn();
    }

    public Player GetPlayer(int id)
    {
        return players[id];
    }
}
