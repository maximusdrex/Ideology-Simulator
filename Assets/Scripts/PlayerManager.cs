using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private Player player;
    private GameManager gm;
    public int playerId;
    private Canvas playerGUI;
    public List<Unit> units;
    public GameObject defaultGUI;
    public GameObject currentGUI;
    public Hex rememberedHex;
    public Unit rememberedUnit;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = gm.GetPlayer(0);
        units = new List<Unit>();
        player.units.Add(new Worker(50));
        setGUI(defaultGUI);
        spawnUnit(player.units[0], player.city.baseHex.C+1, player.city.baseHex.R+1);
        rememberedHex = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.isTurn)
        {
            if(Input.GetMouseButtonDown(0))
            {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Clicked on the UI");
                } 
                else
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Hex hex = clickHex(hit.transform);
                        if (hex == null)
                        {
                            Debug.Log("forest tile");
                        }
                        //if (rememberedHex != null && rememberedHex.tileUnits.Count > 0)
                        //{
                        //    List<Unit> unitsToMove = new List<Unit>();
                        //    foreach (Unit u in rememberedHex.tileUnits)
                        //    {
                        //        Debug.Log("iterating through units");
                        //        Debug.Log(u.type);
                        //        unitsToMove.Add(u);
                        //    }
                        //    foreach (Unit u in unitsToMove)
                        //    {
                        //        moveUnit(u, hex);
                        //    }

                        //}

                        if(rememberedUnit != null) {
                            bool moved = moveUnit(rememberedUnit, hex);
                            if (!moved) {
                                rememberedUnit = null;
                            }
                        }

                        List<IInteractableObj> hexList = hex.tileObjs;
                        if (hexList.Count == 1)
                        {
                            rememberedHex = hex;
                            setGUI(hexList[0].GetUI());
                        }
                        else if (hexList.Count > 1)
                        {
                            rememberedHex = hex;
                            UISelector(hexList, Input.mousePosition);
                        }
                        else
                        {
                            rememberedUnit = null;
                            rememberedHex = null;
                            setGUI(defaultGUI);
                        }

                        //createImprovement(hex);
                        Debug.Log(hex.ToString());
                        if (hex.resourceType != null)
                        {
                            Debug.Log(hex.resourceType.resourceName + ": " + hex.resourceType.getAmount());
                        }


                        Debug.Log(hex.ToString());

                    }
                    else
                    {
                        Debug.Log("Did not hit: ");
                        setGUI(defaultGUI);
                    }
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
            currentGUI = newObj;
        }

    }

    private void UISelector(List<IInteractableObj> objs, Vector3 screenPoint)
    {
        if(GameObject.Find("Selector") != null)
        {
            Destroy(GameObject.Find("Selector"));
        }
        GameObject selector = Instantiate((GameObject)Resources.Load("Selector"));
        selector.transform.SetParent(currentGUI.transform);
        selector.name = "Selector";
        selector.GetComponent<UISelector>().Init(objs);
        Vector2 newPos = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(currentGUI.GetComponent<RectTransform>(), new Vector2(screenPoint.x, screenPoint.y), Camera.main, out newPos);
        selector.transform.localPosition = new Vector3(newPos.x, newPos.y);

    }

    public void spawnUnit (Unit u, int q, int r)
    {
        units.Add(u);
        gm.spawnUnit(u, q, r);
    }
    public bool moveUnit(Unit u, Hex nextHex)
    {
        return gm.moveUnit(u, nextHex);
    }

    public void createImprovement(Hex hex)
    {
        if (hex.improvement == null && (hex.terrain != TerrainEnum.Terrain.Mountain &&
                   hex.terrain != TerrainEnum.Terrain.Ocean))
        {

            if (hex.getCity() != null)
            {
                GameObject obj = (GameObject)Resources.Load("farm");
                gm.placeOnHex(obj, hex.C, hex.R);
                Debug.Log("new farm");
                hex.improvement = new Farm(hex, player, true);
            }
            else
            {
                Debug.Log("Can't build on unowned land!");
            }

        }
        else
        {
            Debug.Log("exists");
        }
    }

    public void StartTurn()
    {
        if(player == null)
        {
            Start();
        }
        City capitol = getCapitol(player);
        Camera.main.transform.position = new Vector3(capitol.baseHex.GetPosition().x, 5f, capitol.baseHex.GetPosition().z -5f);
    }

    private City getCapitol(Player player)
    {
        return player.city;
    }

}

