using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResource 
{
    public string resourceName;
    public double amount;
    public double damount;

    public PlayerResource(string name, double startingAmount)
    {
        resourceName = name;
        amount = startingAmount;
    }

    public PlayerResource(string name)
    {
        resourceName = name;
        amount = 0;
    }

    public double getAmount()
    {
        return amount;
    }

    public double getDamount()
    {
        return damount;
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
}
