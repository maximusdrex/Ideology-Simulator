using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResource 
{
    public string resourceName;
    public double amount;
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

    public void spendResource(double toSubtract)
    {
        amount -= toSubtract;
    }

    public void changeDamount(double toAdd)
    {
        damount += toAdd;
    }

    public void setDResource(double toSet)
    {
        damount = toSet;
    }

}
