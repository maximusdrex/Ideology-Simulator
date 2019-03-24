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
        //write a sell function
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
}
