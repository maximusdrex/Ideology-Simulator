using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Player player;
    private GameManager gm;
    public int playerId;
    private Canvas playerGUI;

    public GameObject defaultGUI;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = gm.GetPlayer(0);

        setGUI(defaultGUI);
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
                    Hex hex = gm.gameMap.getHexFromObj(hit.transform.parent.gameObject);
                    Debug.Log(hex.ToString());
                    List<IInteractableObj> hexList = hex.tileObjs;
                    if(hexList.Count == 1)
                    {
                        setGUI(hexList[0].GetUI());
                    } else
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

    private void setGUI(GameObject gui)
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
}
