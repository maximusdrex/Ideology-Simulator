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
    public GlobalMarket g;
    public Dictionary<Unit, GameObject> unitToGameObject;

    //Called before other Starts()
    void Awake()
    {
        gameMap = GameObject.FindObjectOfType<HexMap>();
        turn = 1;
        players = new List<Player>();
        int numPlayers = 1;
        players.Add(new Player(0, true));
        for (int i = 1; i < numPlayers; i++)
        {
            players.Add(new AIPlayer(i, Random.Range(0, 1) > .5f));
        }
        unitToGameObject = new Dictionary<Unit, GameObject>();
    }

    void Start()
    {
        //Debug.Log("Game Manager started");
        foreach (Player p in players)
        {
            Hex[,] hexes = HexMap.hexes;
            City c = new City(hexes, true, true, p);
            if (p.communist)
            {
                c = new CommunistCity(hexes, true, true, p);
            }
            else if (!p.communist)
            {
                c = new CapitalistCity(hexes, true, true, p);
            }
            p.cities.Add(c);
        }
        g = new GlobalMarket();
        Camera cam = FindObjectOfType<Camera>();
        gameMap.colorHexes(cam.transform.position);
        playing = players[0];
        playing.StartTurn();
        unitToGameObject = new Dictionary<Unit, GameObject>();
    }


    void Update()
    {
        foreach (Player p in players)
        {
            foreach (City c in p.cities)
            {
                if (c.buildingChanged > 0 )
                {
                    Debug.Log("instantiating buildings");
                    instantiateBuilding(c);
                }
                else
                {

                }
            }
        }
    }

    public void spawnUnit(Unit u, int q, int r)
    {
        HexMap.hexes[q, r].tileObjs.Add(u);
        HexMap.hexes[q, r].tileUnits.Add(u);
        GameObject placedModel = placeOnHex(u.model, q, r);
        unitToGameObject.Add(u, placedModel);
        u.SetHex(HexMap.hexes[q, r]);

    }

    public void moveUnit(Unit u, Hex nextHex)
    {
        bool canMove = u.movementCheck(nextHex);
        if (canMove)
        {
            Hex oldHex = u.getHex();
            List<IInteractableObj> tileObjs = oldHex.tileObjs;
            List<Unit> tileUnits = oldHex.tileUnits;
            tileObjs.Remove(u);
            tileUnits.Remove(u);
            u.SetHex(nextHex);
            GameObject unitModel = unitToGameObject[u];
            unitModel.transform.position = new Vector3(nextHex.x, 0, nextHex.z);
            Debug.Log(unitModel.transform.position);
        }
        else
        {
            Debug.Log("Distance too far!");
        }
    }

    public void instantiateBuilding(City c)
    {
        for(int i = 0; i < c.buildingChanged; i++)
        {
            int buildingNum = c.buildings.Count - 1 - i;
            Building b = c.buildings[buildingNum];
            GameObject model = b.model;
            float span = b.span;
            if(buildingNum == 0)
            {
                placeOnHex(model, c.baseHex.C, c.baseHex.R, b.span);
            }
            else if(buildingNum % 2 == 1) {
                placeOnHex(model, c.baseHex.C, c.baseHex.R, b.span, -.4f, Quaternion.Euler(0,90,0));
            }
            else
            {
                placeOnHex(model, c.baseHex.C, c.baseHex.R, b.span, .4f, Quaternion.Euler(0,-90, 0));
            }

             
        }
        c.buildingChanged = 0;
    }

    public GameObject placeOnHex(GameObject obj, int x, int z)
    {
        return placeOnHex(obj, x, z, 0);
    }

    public GameObject placeOnHex(GameObject obj, int x, int z, float span)
    {
       return placeOnHex(obj, x, z, span, Quaternion.Euler(0, 0, 0));

    }

    public GameObject placeOnHex(GameObject obj, int x, int z, float span, Quaternion q)
    {
        return placeOnHex(obj, x, z, span, 0, q);
    }

    public GameObject placeOnHex(GameObject obj, int x, int z, float span, float horizontalDisp, Quaternion q)
    {
        GameObject placedObject = Instantiate(obj, Vector3.zero, Quaternion.identity);
        placedObject.transform.position = gameMap.getHexObj(x, z).transform.position;
        placedObject.transform.SetParent(gameMap.getHexObj(x, z).transform);
        placedObject.transform.localPosition = new Vector3(horizontalDisp, .2f, span);
        placedObject.transform.localRotation = q;
        return placedObject;
    }

    public void nextTurnPressed()
    {
        playing.EndTurn();
        playing = players[(playing.id + 1) % players.Count];
        if (playing.id == 0)
        {
            turn++;
            g.startTurn();
        }
        playing.StartTurn();
    }

    public Player GetPlayer(int id)
    {
        return players[id];
    }

    public PlayerManager getManager(int id)
    {
        if(Camera.main.GetComponent<PlayerManager>() != null)
        {
            return Camera.main.GetComponent<PlayerManager>();
        } else
        {
            return null;
        }
    }

 
}