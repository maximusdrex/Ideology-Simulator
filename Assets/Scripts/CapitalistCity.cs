using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class CapitalistCity : City
{
    public CapitalistCity(Hex[,] hexes, bool center, bool capitol, Player owner) :
    base(hexes, center, capitol,  owner)

    {

        name = getName(capitol);
        Debug.Log("City created at: " + x + "," + z + " named:" + name);
    }

    public string getName(bool capitol)
    {
        try
        {
            int rand = 0;
            string[] lines = File.ReadAllLines(@"Assets/TextResource/capitalistCityNames.txt");
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
    }

    public void feedCitizens()
    {
        citizens.Sort(Citizen.wealthComparison);
        List<Building> stores = this.findBuilding("store");
        PlayerResource food = new PlayerResource("food");
        foreach (Citizen c in citizens)
        {
            foreach (Building b in stores)
            {
                Store s = (Store)b;
                if (s.getResourceCount(food.resourceName) < 0)
                {
                    break;
                }
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
        }
    }

}
