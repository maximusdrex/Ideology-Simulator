using UnityEngine;
using System.Collections;

public class Worker : Unit
{
   public Worker(int manPower) : base("worker")
    {
        this.manPower = manPower;
        model = getModel();
        baseStrength = 0;
    }

    public GameObject getModel()
    {
        return (GameObject)Resources.Load("worker");
    }

    public void buildFarm()
    {

    }

    new public GameObject GetUI()
    {
        GameObject ui = (GameObject)Resources.Load("UnitUI");
        ui.GetComponentInChildren<UnitAction>().setText("Build Farm");
        return ui;
    }

    new public void doAction()
    {
        Camera.main.GetComponent<PlayerManager>().createImprovement(this.hex);
    }
}
