using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Producer : Improvement
{
    public PlayerResource neededResource;
    public List<double> sales;
    public double qouta;
    public PlayerResource producedResource;
    public double totalCost;
    public int turnsUnsold;
    public static Store S;

    public Producer (bool nationalized, Hex baseHex, Player player) :  base(nationalized, baseHex, player)
    {

    }

    public void updateCost(double cost)
    {
        totalCost += cost;
    }

    public double setSalePrice(City C, double amount)
    {
        double harvestedCost = totalCost / producedResource.getAmount();
        harvestedCost *= 1 + baseHex.owner.exportTax;
        harvestedCost *= 1 + C.owner.importTax;
        return harvestedCost;
    }

    public double generateQouta()
    {
        if (sales == null)
        {
            sales = new List<double>();
            sales.Add(0);
            return baseHex.getCity().citizens.Count;
        }
        else if(sales[sales.Count-1] > sales[sales.Count - 2])
        {
            return sales[sales.Count - 1] * 1.1;
        }
        else
        {
            return sales[sales.Count - 1] * .9;
        }
    }

    public void recieveResources(double amount)
    {
        producedResource.setResource(amount*getPerformance());
    }

    public static Comparison<Producer> qoutaComparison = delegate (Producer object1, Producer object2)
    {
        return object1.generateQouta().CompareTo(object2.generateQouta());
    };

    public static new  Comparison<Producer> amountComparison = delegate (Producer object1, Producer object2)
    {
        return object1.producedResource.getAmount().CompareTo(object2.producedResource.getAmount());
    };

    public new static Comparison<Producer> priceCompare = delegate (Producer object1, Producer object2)
    {
        return object1.getSalePrice(S.owner, 1).CompareTo(object2.getSalePrice(S.owner, 1));
    };

}
