using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public HexMap gameMap;
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
        int numPlayers = 5;
        players.Add(new Player(0, canvas));
        for (int i = 1; i < numPlayers; i++)
        {
            players.Add(new AIPlayer(i, canvas));

        }
        foreach (Player p in players)
        {
            City c = new City(Random.Range(0, 40), Random.Range(0, 40), true, true, true);
            p.cities.Add(c);
        }
    }

    void Start()
    {
        Debug.Log("Game Manager started");
        
        playing = players[0];
        playing.StartTurn();
    }


    void Update()
    {
        foreach (Player p in players)
        {
            foreach(City c in p.cities)
            {
                if(c.buildingChanged == true)
                {
                    Debug.Log("Instantiating new building");
                    GameObject model = c.buildings[c.buildings.Count-1].model;
                    placeOnHex(model, c.x, c.y);
                    c.buildingChanged = false;
                }
                else
                {
                    
                }
            }
        }
        placeOnHex(testObj, 2, 1);
    }

    public bool placeOnHex(GameObject obj, int x, int y)
    {
        obj.transform.SetParent(gameMap.getHexObj(x, y).transform);
        obj.transform.localPosition = new Vector3(0, 0, 0);
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
