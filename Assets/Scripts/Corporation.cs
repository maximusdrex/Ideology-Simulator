using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corporation
{
    List<Improvement> improvements;
    List<PlayerResource> resources;
    int turnsUnsold;

    public Corporation(Improvement improvement)
    {
        improvements.Add(improvement);
        resources = City.initializeResources();
    }

    public void harvestAll()
    {
        foreach(Improvement I in improvements)
        {
            double harvestedAmount = I.harvestResource();
            string name = I.baseHex.resourceType.resourceName;
            foreach (PlayerResource r in resources)
            {
                if (name == r.resourceName)
                {
                    r.amount += harvestedAmount;
                }
            }

        }
    }

    public double setSalePrice(Improvement I, Producer P)
    {
        double harvestedCost = I.getHarvestCost();
        harvestedCost *= 1 + I.baseHex.owner.exportTax;
        harvestedCost *= 1 + P.baseHex.owner.importTax;
        return harvestedCost;
    }

}
