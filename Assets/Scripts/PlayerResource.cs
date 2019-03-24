using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResource 
{
    public string resourceName;
    private double amount;
    public double damount;
    public double harvestCost;

    public PlayerResource(string name, double startingAmount, double harvestCost)
    {
        resourceName = name;
        amount = startingAmount;
        this.harvestCost = harvestCost;
    }

    public PlayerResource(string name)
    {
        resourceName = name;
        amount = 0;
        harvestCost = 10;
    }

    public double getAmount()
    {
        return amount;
    }

    public double getDamount()
    {
        return damount;
    }

    public void setResource(double toSet)
    {
        amount = toSet;
    }

    public void gainResource(double toAdd)
    {
        amount += toAdd;
    }

    public double spendResource(double toSubtract)
    {
        if(toSubtract > amount)
        {
            double returnAmount = amount;
            amount = 0;
            return returnAmount;
        }
        amount -= toSubtract;
        return toSubtract;
    }

    public void changeDamount(double toAdd)
    {
        damount += toAdd;
    }

    public void setDResource(double toSet)
    {
        damount = toSet;
    }

    public bool Equals(PlayerResource other)
    {
        return resourceName == other.resourceName;
    }
}
