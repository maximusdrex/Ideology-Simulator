using UnityEngine;
using System.Collections;

public class Fighter : Unit
{
    public Fighter(int manPower) : base("fighter")
    {
        this.manPower = manPower;
        model = getModel();
        baseStrength = 5;
    }

    public GameObject getModel()
    {
        return (GameObject)Resources.Load("Figher");
    }

}
