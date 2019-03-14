using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class City {
    public int x;
    public int y;
    public int maxBuildings;
    bool center;
    bool capitol;
    bool capitalist;
    string name;

    int populationModifier;
    List<Citizen> citizens;
    List<Building> buildings;
    
    public City(int xcoord, int ycoord, bool center, bool capitol, bool capitalist)
    {
        x = xcoord;
        name = getName(capitalist, capitol);
        Debug.Log(name);
        y = ycoord;
        maxBuildings = 6;
        this.center = center;
        buildings = new List<Building>();
        if (center == true)
        {
            buildings.Add(new CityHall ("City Hall"));
        }
    }

    public string getName(bool capitalist, bool capitol)
    {
        try
        {
            //Pass the file path and file name to the StreamReader constructorbase
            if(capitalist == false)
            {
                int rand = 0;
                string[] lines = System.IO.File.ReadAllLines(@"/Users/cayden/Documents/unityRepo/communistCityNames");
                if(capitol == true)
                {   
                    rand = Random.Range(1, 5);
                }
                else
                {   
                    rand = Random.Range(6, lines.Length - 1);
                }
                return lines[rand];
            }
            else
            {
                int rand = 0;
                string[] lines = System.IO.File.ReadAllLines(@"/Users/cayden/Documents/unityRepo/capitalistCityNames");
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
        }
        catch(IOException e)
        {
            Debug.Log("Naming encountered an error");
            Debug.Log(e.Message);
            return "Sandersgrad";
        }
    }

    public bool addBuilding(Building b)
    {
        if(buildings.Count == maxBuildings)
        {
            return false;
        }
        else
        {
            buildings.Add(b);
            return true;
        }
    }
}
