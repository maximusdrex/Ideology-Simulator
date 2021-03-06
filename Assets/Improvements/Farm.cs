﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Improvement
{

    private double productivity;
    private double baseFood = 50;
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
        resource.setResource(getPerformance() * baseFood * productivity);
        payEmployees();
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
