﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Store : Building

{
    public Corporation corporation;
    public List<PlayerResource> neededResources;
    public List<double> sales;
    public double qouta;
    public Dictionary<PlayerResource, double> costDictionary;
    public List<Citizen> employees;
    public double money;
    public double cost;

    private int numUE;
    private int numLE;
    private int numHE;
    public int idealUE;
    public int idealLE;
    public int idealHE;

    public double priceModifier;

    public Store(string name, City owner) : base(name, owner)
    {
        costDictionary = new Dictionary<PlayerResource, double>();
        model = getModel();
        span = .2f;
        corporation = new Corporation(this);
        priceModifier = 1;
        type = "store";
        neededResources = new List<PlayerResource>();
        neededResources.Add(new PlayerResource("food"));
        neededResources.Add(new PlayerResource("iron"));
    }

    public GameObject getModel()
    {
        return (GameObject)Resources.Load("store");
    }

    public void recieveResources(string name, double amount)
    {
        if(name == null)
        {
            return;
        }
        PlayerResource pr = neededResources.Find(x => x.resourceName == name);
        //costDictionary.Add(pr, cost);
        pr.gainResource(amount);
    }

    public double giveResources(string name)
    {
        PlayerResource pr = neededResources.Find(x => x.resourceName == name);
        double amount = pr.getAmount();
        pr.setResource(0);
        return amount;

    }

    public double getResourceCount(string name) {
        return neededResources.Find(x => x.resourceName == name).getAmount();
    }

    public double generateQouta()
    {
        if (sales == null)
        {
            sales = new List<double>();
            sales.Add(0);
            return owner.citizens.Count*neededResources.Count;
        }
        else if (sales[sales.Count - 1] > sales[sales.Count - 2])
        {
            return sales[sales.Count - 1] * 1.1;
        }
        else
        {
            return sales[sales.Count - 1] * .9;
        }
    }

    public void levelEmployees()
    {
        foreach (Citizen e in employees)
        {
            e.timeAtCurrentJob++;
        }
    }

    public void hireEmployee(Citizen c)
    {
        int ed = c.getEducation();
        if (ed == 0)
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

        c.currentBuildingJob = this;
        c.timeAtCurrentJob = 0;
        employees.Add(c);
    }

    public void fireEmployee()
    {
        int edToFire = 0;
        double UEmissedPerformance = Improvement.performanceHitUE * Mathf.Abs(idealUE - numUE);
        double LEmissedPerformance = Improvement.performanceHitLE * Mathf.Abs(idealLE - numLE);
        double HEmissedPerformance = Improvement.performanceHitHE * Mathf.Abs(idealLE - numLE);

        employees.Sort(Citizen.jobTimeComparison);
        employees.Reverse();

        if (UEmissedPerformance >= LEmissedPerformance && UEmissedPerformance >= HEmissedPerformance)
        {
            edToFire = 0;
        }
        else if (LEmissedPerformance >= UEmissedPerformance && LEmissedPerformance >= HEmissedPerformance)
        {
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

    public bool payEmployees()
    {
        employees.Sort(Citizen.jobTimeComparison);
        foreach (Citizen e in employees)
        {
            double wage = 0;
            CapitalistCity location = (CapitalistCity)this.owner;
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
            if (money <= 0)
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
            priceModifier += .05;
            return money;
        }
        else
        {
            fireEmployee();
            priceModifier -= .05;
            return 0;
        }
    }

    public double getPrice()
    {
        return (cost / qouta )* priceModifier;
    }

    public static Comparison<Store> priceCompare = delegate (Store object1, Store object2)
    {
        return object1.getPrice().CompareTo(object2.getPrice());
    };
}
