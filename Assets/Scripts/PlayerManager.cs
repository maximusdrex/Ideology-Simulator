using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Player player;
    private GameManager gm;
    public int playerId;
    private Canvas playerGUI;
    public List<Unit> units;
    public GameObject defaultGUI;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = gm.GetPlayer(0);
        units = new List<Unit>();
        setGUI(defaultGUI);
        City c = player.cities[0];
        Hex[] neighbors = c.baseHex.getNeighbors(HexMap.numCollumns, HexMap.numRows);
        Hex neighbor = neighbors[Random.Range(0, 5)];
        Worker w = new Worker(100);
        spawnUnit(w, neighbor.C, neighbor.R);
        Debug.Log("Unit spawned " + neighbor.C + " " + neighbor.R);
    }

    // Update is called once per frame
    void Update()
    {
        if(player.isTurn)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Hex hex = clickHex(hit.transform);
                    if(hex == null)
                    {
                        Debug.Log("forest tile");
                    }
                    Debug.Log(hex.ToString());
                    if (hex.resourceType != null)
                    {
                        Debug.Log(hex.resourceType.resourceName + ": " + hex.resourceType.getAmount());
                    }
                    List<IInteractableObj> hexList = hex.tileObjs;
                    if(hexList.Count == 1)
                    {
                        setGUI(hexList[0].GetUI());
                    } 
                    else
                    {
                        setGUI(defaultGUI);
                    }
                } else
                {
                    Debug.Log("Did not hit");
                    setGUI(defaultGUI);
                }
            }
        }
    }

    private Hex clickHex(Transform hitT)
    {
        GameObject hexMap = gm.gameMap.gameObject;
        if(hitT.parent != null)
        {
            Debug.Log(hitT.gameObject.name);
            if (hitT.parent.gameObject == hexMap)
            {
                return (gm.gameMap.getHexFromObj(hitT.gameObject));
            } else
            {
                return clickHex(hitT.parent.transform);
            }
        } else
        {
            return null;
        }
    }

    public void setGUI(GameObject gui)
    {
        bool exists = false;
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("GUI"))
        {
            if(gui.name != obj.name)
            {
                GameObject.Destroy(obj);
            } else
            {
                exists = true;
            }
        }

        if(gui != null && !exists)
        {
            GameObject newObj = Instantiate(gui);
            newObj.tag = "GUI";
            newObj.name = gui.name;
        }

    }

    public void spawnUnit (Unit u, int q, int r)
    {
        units.Add(u);
        player.units.Add(u);
        gm.spawnUnit(u, q, r);
    }
    public void moveUnit(Unit u, Hex nextHex)
    {
        u.SetHex(nextHex);
        gm.moveUnit(u);
    }
}

