using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tech
{
    public string name;
    private int progress;
    private int cost;
    private Tech prereq;
    private List<Tech> children;

    public Tech(string nm, int tcost, Tech req)
    {
        progress = 0;
        name = nm;
        prereq = req;
        cost = tcost;
        children = new List<Tech>();
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

    public List<Tech> GetChildren()
    {
        return children;
    }

    public Tech GetPrereq()
    {
        return prereq;
    }

    public void AddChild(Tech tech)
    {
        if(!children.Contains(tech))
        {
            children.Add(tech);
        }
    }

    public int AddProgress(int added)
    {
        if(progress + added <= cost)
        {
            progress += added;
            return 0;
        } else
        {
            int leftover = progress + added - cost;
            progress = cost;
            return leftover;
        }
    }
}
