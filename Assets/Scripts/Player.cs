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

    public Player(int pid, GameObject canvas, bool communist)
    {
        id = pid;
        GameObject prefab = (GameObject) Resources.Load("PlayerIndicator");
        indicator = GameObject.Instantiate(prefab, canvas.transform);
        indicator.transform.position = new Vector3(70, 10 + (id * 25), 0);
        indicator.GetComponentInChildren<UnityEngine.UI.Text>().text = "Player " + id.ToString();
        indicator.GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        cities = new List<City>();
        this.communist = communist;
    }

    public virtual void StartTurn()
    {
        isTurn = true;
        indicator.GetComponent<UnityEngine.UI.Toggle>().isOn = true;
        foreach(City c in cities)
        {
            c.startTurn(c);
            GDP += c.GDP;
            money += c.getResource("money").getDamount();
        }
    }

    public void EndTurn()
    {
        isTurn = false;
        indicator.GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        foreach(City c in cities)
        {
           //c.buildingChanged = false;
        }
    }

    void Update()
    {
        if(isTurn)
        {

        }
    }
}
