using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tech
{
    public string name;
    private int progress;
    private int cost;
    private Tech prereq;

    public Tech(string nm, int tcost, Tech req)
    {
        progress = 0;
        name = nm;
        prereq = req;
        cost = tcost;
    }

    public int GetProgress()
    {
        return progress;
    }

    public bool IsCompleted()
    {
        if(progress >= cost)
        {
            return true;
        }
        return false;
    }

    public int GetCost()
    {
        return cost;
    }
}
