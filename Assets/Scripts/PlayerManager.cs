﻿using System.Collections;
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

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = gm.GetPlayer(0);
        units = new List<Unit>();
        setGUI(defaultGUI);
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
                } else
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
                        Debug.Log(hex.ToString());
                        List<IInteractableObj> hexList = hex.tileObjs;
                        if (hexList.Count == 1)
                        {
                            setGUI(hexList[0].GetUI());
                        }
                        else if (hexList.Count > 1)
                        {
                            UISelector(hexList, Input.mousePosition);
                        }
                        else
                        {
                            setGUI(defaultGUI);
                        }
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
    public void moveUnit(Unit u, Hex nextHex)
    {
        u.SetHex(nextHex);
        gm.moveUnit(u);
    }
}

