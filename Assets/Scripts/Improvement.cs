using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Improvement
{
    public Player owner;
    public City location;
    public Corporation corporation;
    public List<Citizen> employees;
    public double harvestCost;
    public Hex baseHex;
    public int idealUE;
    public int idealLE;
    public int idealHE;

    public int minEmployees;

    private int numUE;
    private int numLE;
    private int numHE;
    private double performanceHitUE = .1;
    private double performanceHitLE = .05;
    private double performanceHitHE = .02;



    public Improvement(City location, Corporation corp) {
        owner = location.owner;
        corporation = corp;
        employees = new List<Citizen>();
        harvestCost = baseHex.resourceType.harvestCost;

    }


    public void levelEmployees()
    {
        foreach(Citizen e in employees)
        {
            e.timeAtCurrentJob++;
        }
    }

    public void hireEmployee(Citizen c)
    {
        int ed = c.getEducation();
        if(ed == 0)
        {
            numUE++;
        }
        else if (ed == 0)
        {
            numLE++;
        }
        else
        {
            numHE++;
        }

        c.currentJob = this;
        c.timeAtCurrentJob = 0;
        employees.Add(c);  
    }

    public void fireEmployee()
    {
        int edToFire = 0;
        double UEmissedPerformance = performanceHitUE * Mathf.Abs(idealUE - numUE);
        double LEmissedPerformance = performanceHitLE * Mathf.Abs(idealLE - numLE);
        double HEmissedPerformance = performanceHitHE * Mathf.Abs(idealLE - numLE);

        employees.Sort(Citizen.jobTimeComparison);
        employees.Reverse();

        if(UEmissedPerformance >= LEmissedPerformance && UEmissedPerformance>= HEmissedPerformance)
        {
            edToFire = 0;
        }
        else if(LEmissedPerformance >= UEmissedPerformance && LEmissedPerformance >= HEmissedPerformance){
            edToFire = 1;
        }
        else
        {
            edToFire = 2;
        }

        foreach (Citizen e in employees)
        {
            if (e.getEducation() == edToFire)
            {
                e.getFired();
                employees.Remove(e);
                break;
            }
            break;
        }
    }


    public double getPerformance()
    {
        if((numUE + numLE + numHE) < minEmployees)
        {
            return 0;
        }
        return 1f - performanceHitUE * Mathf.Abs(idealUE - numUE) - performanceHitLE * Mathf.Abs(idealLE - numLE) - performanceHitHE * Mathf.Abs(idealHE - numHE);
    }

    public double harvestResource()
    {
        return getPerformance() * baseHex.resourceType.amount;
    }

    public double getHarvestCost()
    {
        return harvestResource() * harvestCost;
    }
}
