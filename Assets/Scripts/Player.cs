using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int id;
    public bool isTurn;
    private GameObject indicator;
    public City city;
    public double GDP;
    public double money;
    public bool communist;
    public double exportTax;
    public double importTax;
    public double minimumWage;
    public double wageTax;
    public List<Unit> units;
    private GameManager gm;
    private PlayerManager playerManager;

    private List<Tech> playerTechTree;
    private Tech researching;

    public Player(int pid, bool communist)
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerTechTree = new List<Tech>();
        id = pid;
        GameObject prefab = (GameObject) Resources.Load("PlayerIndicator");
        //indicator = GameObject.Instantiate(prefab, canvas.transform);
        //indicator.transform.position = new Vector3(70, 10 + (id * 25), 0);
        //indicator.GetComponentInChildren<UnityEngine.UI.Text>().text = "Player " + id.ToString();
        //indicator.GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        this.communist = communist;
        LoadTechs();
        units = new List<Unit>();
    }

    public virtual void StartTurn()
    {
        PlayerManager pm = gm.getManager(id);
        playerManager = pm;
        if(pm != null)
        {
            pm.StartTurn();
        }

        isTurn = true;
        //indicator.GetComponent<UnityEngine.UI.Toggle>().isOn = true;
        city.startTurn();
        GDP += city.GDP;
        money += city.money;
        if(researching != null)
        {
            researching.AddProgress(TechProgress());
        }
        foreach(Unit u in units)
        {
            u.startTurn();
        }
    }

    public void EndTurn()
    {
        isTurn = false;
        //indicator.GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        city.endTurn();
    }

    void Update()
    {
        if(isTurn)
        {

        }
    }

    private void LoadTechs()
    {
        TextAsset fullTechs = (TextAsset) Resources.Load("Techs");
        string[] Techs = fullTechs.text.Split('\n');
        foreach(string tech in Techs)
        {
            string[] techDetails = tech.Split(',');
            if(techDetails.Length == 3)
            {
                try
                {
                    playerTechTree.Add(new Tech(techDetails[0].Trim(), int.Parse(techDetails[1]), getTech(techDetails[2].Trim())));
                } catch
                {
                    Debug.Log("Did not add: " + techDetails[0] + "; " + techDetails[2]);
                }
            } else if(techDetails.Length == 2)
            {
                playerTechTree.Add(new Tech(techDetails[0].Trim(), int.Parse(techDetails[1]), null));
            }
            else
            {
                Debug.Log("Did not add tech, format wrong");
            }
            
        }
        getAllTechs();
    }

    private void getAllTechs()
    {
        foreach(Tech tech in playerTechTree)
        {
            if(tech.GetPrereq() == null)
            {
                Debug.Log(tech.name + " : " + null);
            } else
            {
                Debug.Log(tech.name + " : " + tech.GetPrereq().name);
            }
        }
    }

    public Tech getTech(string techName)
    {
        foreach (Tech tech in playerTechTree)
        {
            if (tech.name == techName)
            {
                //Debug.Log(tech.name + " == " + techName);
                return tech;
            }
            //Debug.Log("--" + tech.name + " : " + tech.name.GetHashCode().ToString() + " : " + tech.name.Length.ToString() + " != " + techName + " : " + techName.GetHashCode().ToString() + " : " + techName.Length.ToString() + "--");

        }
        //Debug.Log("COULD NOT FIND: " + techName);
        return null;
    }

    public List<Tech> GetTechTree()
    {
        return playerTechTree;
    }

    private int TechProgress()
    {
        return 5;
    }

    public bool setResearch(Tech tech)
    {
        if (tech.GetPrereq().IsCompleted())
        {
            researching = tech;
            return true;
        } else
        {
            return false;
        }
        
    }
}
