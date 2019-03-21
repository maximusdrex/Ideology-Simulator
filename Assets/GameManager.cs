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
    }

    void Start()
    {
        Debug.Log("Game Manager started");
        foreach (Player p in players)
        {
            int cityX = Random.Range(0, 40);
            int cityY = Random.Range(0, 40);
            Hex h = HexMap.hexes[cityX, cityY];
            City c = new City(h, cityX, cityY, true, true, true,p);
            p.cities.Add(c);
        }
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
                    instantiateBuilding(c);
                }
                else
                {

                }
            }
        }
    }

    public void instantiateBuilding(City c)
    {
        Building b = c.buildings[c.buildings.Count - 1];
        GameObject model = b.model;
        float span = b.span;
        placeOnHex(model, c.baseHex.C, c.baseHex.R, b.span);
        c.buildingChanged = false;
    }

    public bool placeOnHex(GameObject obj, int x, int z)
    {
        placeOnHex(obj, x, z, 0);
        return true;
    }

    public bool placeOnHex(GameObject obj, int x, int z, float span)
    {
        GameObject placedObject = Instantiate(obj);
        placedObject.transform.position = gameMap.getHexObj(x, z).transform.position;
        placedObject.transform.SetParent(gameMap.getHexObj(x, z).transform);
        placedObject.transform.localPosition = new Vector3(0, 0, span);
        Debug.Log(span);
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
