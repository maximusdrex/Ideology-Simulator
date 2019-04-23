using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Improvement
{

    private double productivity;
    private double baseFood = 100;
    static public GameObject model;


    public Farm(Hex baseHex, Player player, bool nationalized) : base(nationalized, baseHex, player)
    {
        idealUE = 1;
        if(baseHex.moisture  >= .66f)
        {
            productivity = 1.25;
        }
        else if (baseHex.moisture > .5f)
        {
            productivity = 1;
        }
        else
        {
            productivity = .75;
        }
    }


    public override void harvestResource()
    {
        Debug.Log("Performace:" + getPerformance());
        Debug.Log("Productivity:" + productivity);
        resource.setResource(getPerformance() * baseFood * productivity);
        Debug.Log("Farm produced: " + getPerformance() * baseFood * productivity);
    }

    public GameObject getModel()
    {
        if (model == null)
        {
            model = (GameObject)Resources.Load("farm");
        }
        return model;
    }



}
