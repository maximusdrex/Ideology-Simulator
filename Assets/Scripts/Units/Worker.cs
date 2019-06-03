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

    public override GameObject GetUI()
    {
        pm.rememberedUnit = this;
        GameObject ui = (GameObject)Resources.Load("UnitUI");
        ui.GetComponentInChildren<UnitAction>().setText("Build Farm");
        return ui;
    }

    public override void doAction()
    {
        Debug.Log("working");
        Camera.main.GetComponent<PlayerManager>().createImprovement(this.hex);
    }
}
