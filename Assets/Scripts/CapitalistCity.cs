using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class CapitalistCity : City
{

    private double wageTax = -1;
    private double minimumWage = -1;

    public CapitalistCity(Hex[,] hexes, bool center, bool capitol, Player owner) :
    base(hexes, center, capitol,  owner)

    {

        name = getName(capitol);
        Debug.Log("City created at: " + baseHex.C + "," + baseHex.R + " named:" + name);
    }

    public string getName(bool capitol)
    {
        try
        {
            int rand = 0;
            string[] lines = File.ReadAllLines(@"Assets/TextResources/capitalistCityNames.txt");
            if (capitol == true)
            {
                rand = Random.Range(1, 9);
            }
            else
            {
                rand = Random.Range(10, lines.Length - 1);
            }
            return lines[rand];
        }
        catch (IOException e)
        {
            Debug.Log("Naming encountered an error");
            Debug.Log(e.Message);
            return "Smithville";
        }
    }

    public new void startTurn()
    {
        base.startTurn();
        feedCitizens();
        foreach(PlayerResource r in resources)
        {
            money += System.Math.Abs(r.minusAmount * r.harvestCost * tax);
        }
        Debug.Log("Money:" + money);
    }

    //Checks if the city has set it's own minimum wage or i/e taxes
    //otherwise defaults to the players
    public new double getMinimumWage()
    {
        if (minimumWage < 0)
        {
            return owner.minimumWage;
        }
        return minimumWage;

    }

    public new double getWageTax()
    {
        if (wageTax < 0)
        {
            return owner.wageTax;
        }
        return wageTax;
    }

    public new void setMinimumWage(double w)
    {
        this.minimumWage = w;
    }

    public new void setWageTax(double t)
    {
        this.wageTax = t;
    }


    public override void feedCitizens(){
        citizens.Sort(Citizen.wealthComparison);
        List<Building> stores = this.findBuilding("store");
        PlayerResource food = new PlayerResource("food");
        foreach (Citizen c in citizens)
        {
            Debug.Log(c.firstName + " " + c.wealth + " " + c.timeAtCurrentJob);
            foreach (Building b in stores)
            {
                Store s = (Store)b;
                if (s.getResourceCount(food.resourceName) < 0)
                {
                    break;
                }
                Debug.Log(s.getPrice());
                if (c.wealth >= s.getPrice())
                {
                    c.wealth -= s.getPrice();
                    c.recieveFood(Citizen.foodAmount);
                    s.recieveResources(food.resourceName, -1*Citizen.foodAmount);
                    s.money += s.getPrice();
                    break;
                }
                break;
            }
            c.recieveFood(0);
        }
        foreach (PlayerResource resource in resources)
        {
            money += resource.harvestCost * resource.getDamount() * tax;
        }
    }

}
