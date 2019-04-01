using System.Collections;
using System.Collections.Generic;
using System;
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
    public static double performanceHitUE = .1;
    public static double performanceHitLE = .05;
    public static double performanceHitHE = .02;


    private double money;
    public PlayerResource resource;
    static public Producer P;

    bool nationalized;



    public Improvement(bool nationalized, Hex baseHex) {
        if (!nationalized)
        {
            corporation = new Corporation(this);
        }
        location = baseHex.getCity();
        owner = location.owner;
        employees = new List<Citizen>();
        harvestCost = baseHex.resourceType.harvestCost;
        resource = baseHex.resourceType;
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
        double performance = 0;
        foreach(Citizen e in employees) {
            performance += e.returnSatisfaction();
        }
        performance = performance / employees.Count;
        if ((numUE + numLE + numHE) < minEmployees)
        {
            return 0;
        }
        return performance - performanceHitUE * Mathf.Abs(idealUE - numUE) - performanceHitLE * Mathf.Abs(idealLE - numLE) - performanceHitHE * Mathf.Abs(idealHE - numHE);
    }

    public virtual void harvestResource()
    {

        resource.setResource(getPerformance() * baseHex.resourceType.getAmount());
    }

    public double getHarvestCost(double amount)
    {
        return amount * harvestCost;
    }

    public void recieveMoney(double amount)
    {

    }

    public double getSalePrice(Producer P, double amount)
    {
        double harvestedCost = getHarvestCost(amount);
        harvestedCost *= 1 + baseHex.owner.exportTax;
        harvestedCost *= 1 + P.baseHex.owner.importTax;
        return harvestedCost;
    }

    public double getSalePrice(City C, double amount)
    {
        double harvestedCost = getHarvestCost(amount);
        harvestedCost *= 1 + baseHex.owner.exportTax;
        return harvestedCost;
    }

    public static Comparison<Improvement> amountComparison = delegate (Improvement object1, Improvement object2)
    {
        return object1.resource.getAmount().CompareTo(object2.resource.getAmount());
    };


    public static Comparison<Improvement> priceCompare  = delegate (Improvement object1, Improvement object2)
    {
        return object1.getSalePrice(P, 1).CompareTo(object2.getSalePrice(P, 1));
    };


    public bool payEmployees()
    {
        employees.Sort(Citizen.jobTimeComparison);
        foreach(Citizen e in employees)
        {
            CapitalistCity location = (CapitalistCity)this.location;
            double wage = 0;
            if (e.getEducation() == 0)
            {
                wage = location.getMinimumWage();
            }
            if (e.getEducation() == 1)
            {
                wage = location.getMinimumWage() * 1.5;
            }
            if (e.getEducation() == 2)
            {
                wage = location.getMinimumWage() * 2;
            }
            e.recievePay(wage, location.getWageTax());
            money -= wage;
            if(money <= 0)
            {
                return false;
            }
        }

        return true;
    }

    public double cleanUp()
    {
       bool solvent = payEmployees();
       if (solvent)
        {
            return money;
        }
        else
        {
            fireEmployee();
            return 0;
        }
    }
}
