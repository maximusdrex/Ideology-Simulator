using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : Building

{
    public Corporation corporation;
    public PlayerResource neededResource;
    public List<double> sales;
    public double qouta;
    public double cost;
    public List<Citizen> employees;
    private int numUE;
    private int numLE;
    private int numHE;

    public Store(string name, City owner) : base(name, owner)
    {
        model = getModel();
        span = .2f;
        corporation = new Corporation(this);
    }

    public GameObject getModel()
    {
        return (GameObject)Resources.Load("cityHall");
    }
    public void recieveResources(double amount)
    {
        neededResource.gainResource(amount);
    }

    public double generateQouta()
    {
        if (sales == null)
        {
            sales = new List<double>();
            sales.Add(0);
            return owner.citizens.Count;
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

        c.currentJob = this;
        c.timeAtCurrentJob = 0;
        employees.Add(c);
    }
}
