using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResource 
{
    public string resourceName;
    private double amount;
    public double plusAmount;
    public double minusAmount;
    public double harvestCost;
    public double damount;

    public PlayerResource(string name, double startingAmount, double harvestCost)
    {
        resourceName = name;
        amount = startingAmount;
        damount = 0;
        this.harvestCost = harvestCost;
    }

    public PlayerResource(string name, double startingAmount)
    {
        resourceName = name;
        amount = startingAmount;
        damount = 0;
        setHarvestCost(name);
    }

    public PlayerResource(string name)
    {
        resourceName = name;
        amount = 0;
        damount = 0;
        setHarvestCost(name);
    }

    public void setHarvestCost(string name)
    {
        if (name.Equals("food"))
            this.harvestCost = 2000000;
        else if (name.Equals("lumber"))
            this.harvestCost = 500000;
        else if (name.Equals("iron"))
            this.harvestCost = 1200000;
        else if (name.Equals("steel"))
            this.harvestCost = 1500000;
        else if (name.Equals("coal"))
            this.harvestCost = 37000;
        else if (name.Equals("oil"))
            this.harvestCost = 60000;
        else if (name.Equals("stone"))
            this.harvestCost = 27500;
        else if (name.Equals("fuel"))
            this.harvestCost = 160000;
        else if (name.Equals("luxury_metals"))
            this.harvestCost = 200000;
        else if (name.Equals("plastic"))
            this.harvestCost = 330000;
        else if (name.Equals("aluminum"))
            this.harvestCost = 210000;
        else if (name.Equals("electronics"))
            this.harvestCost = 200000;
        else if (name.Equals("uranium"))
            this.harvestCost = 200000;
        else if (name.Equals("transport"))
            this.harvestCost = 400000;
    }

    public double getAmount()
    {
        return amount;
    }

    public double getDamount()
    {
        return plusAmount - minusAmount;
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
        plusAmount += toAdd;
    }

    public void setDResource(double toSet)
    {
        plusAmount = toSet;
        damount = toSet;
    }

    public bool Equals(PlayerResource other)
    {
        return resourceName == other.resourceName;
    }

    public override string ToString()
    {
        return resourceName + " :" + amount;
    }
}
