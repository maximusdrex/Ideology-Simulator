using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int id;
    public bool isTurn;
    private GameObject indicator;
    public List <City> cities;
    public double GDP;
    public double money;
    public bool communist;
    private List<Tech> playerTechTree;
    public double exportTax;
    public double importTax;
    public double minimumWage;
    public double wageTax;

    public Player(int pid, bool communist)
    {
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        playerTechTree = new List<Tech>();
        id = pid;
        GameObject prefab = (GameObject) Resources.Load("PlayerIndicator");
        //indicator = GameObject.Instantiate(prefab, canvas.transform);
        //indicator.transform.position = new Vector3(70, 10 + (id * 25), 0);
        //indicator.GetComponentInChildren<UnityEngine.UI.Text>().text = "Player " + id.ToString();
        //indicator.GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        cities = new List<City>();
        this.communist = communist;
        LoadTechs();
    }

    public virtual void StartTurn()
    {
        isTurn = true;
        //indicator.GetComponent<UnityEngine.UI.Toggle>().isOn = true;
        if (communist)
        {
            foreach (CommunistCity com in cities)
            {
                com.startTurn();
                GDP += com.GDP;
                money += com.money;
            }
        }
        else
        {
            foreach (CapitalistCity cap in cities)
            {
                cap.startTurn();
                GDP += cap.GDP;
                money += cap.money;
            }
        }
    }

    public void EndTurn()
    {
        isTurn = false;
        //indicator.GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        foreach(City c in cities)
        {
            c.endTurn();
        }
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
                Debug.Log("Did not add tech");
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
}
