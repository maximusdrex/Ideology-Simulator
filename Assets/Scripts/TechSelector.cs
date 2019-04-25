using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechSelector : MonoBehaviour
{
    public Tech tech;
    // Start is called before the first frame update
    void Start()
    {
        float progress;
        if (tech.GetCost() != 0)
        {
            progress = (tech.GetProgress() / tech.GetCost()) * 160;
            //GetComponentInChildren<UnityEngine.UI.Image>().GetComponent<RectTransform>().sizeDelta = new Vector2(progress, 0);
        }
        foreach (UnityEngine.UI.Text textObj in GetComponentsInChildren<UnityEngine.UI.Text>())
        {
            if(textObj.transform.gameObject.name == "ProgressText")
            {
                textObj.text = tech.GetProgress() + " / " + tech.GetCost();
            }
        }
    }

    public void OnClick()
    {
        PlayerManager pm = Camera.main.GetComponent<PlayerManager>();
        if (GameObject.Find("GameManager").GetComponent<GameManager>().GetPlayer(pm.playerId).setResearch(tech))
        {
            pm.setGUI(pm.defaultGUI);
        }
    }
}
