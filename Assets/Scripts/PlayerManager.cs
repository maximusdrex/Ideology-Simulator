using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Player player;
    private GameManager gm;
    public int playerId;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = gm.GetPlayer(0);
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
                } else
                {
                    Debug.Log("Did not hit");
                }
            }
        }
    }
}
