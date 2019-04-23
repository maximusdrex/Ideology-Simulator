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
}
