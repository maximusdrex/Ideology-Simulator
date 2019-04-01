using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Improvement
{

    private double productivity;
    private double baseFood = 100;

    public Farm(Hex baseHex, Corporation corp, bool nationalized) : base(nationalized, baseHex)
    {
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


    public new void harvestResource()
    {

        resource.setResource(getPerformance() * baseFood * productivity);
    }




}
