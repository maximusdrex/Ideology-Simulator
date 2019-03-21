using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class City {
    public Hex baseHex;
    public float x;
    public float z;
    public int maxBuildings = 6;
    public bool center;
    public bool capitol;
    public bool capitalist;
    public string name;

    public float populationModifier;
    public List<Citizen> citizens;
    public List<Building> buildings;
    public List<Hex> ownedHexes;
    public static int maxRange = 2;
    public List<Hex> possibleHexes;
    public bool buildingChanged;

    public Player owner;


    public City(Hex hex, float xcoord, float zcoord, bool center, bool capitol, bool capitalist, Player owner)
    {
        this.owner = owner;
        ownedHexes = new List<Hex>();
        baseHex = hex;
        ownedHexes.Add(baseHex);
        possibleHexes = HexMap.hexesInRange(baseHex, maxRange);

        buildingChanged = true;
        x = xcoord;
        z = zcoord;
        name = getName(capitalist, capitol);
        this.center = center;
        if(center == true)
        {
            maxBuildings++;
        }
        buildings = new List<Building>();
        citizens = new List<Citizen>();
        if (center == true)
        {
            buildings.Add(new CityHall ("City Hall"));
            Debug.Log("City Hall added");
        }
        Debug.Log("City created at: " + xcoord + "," + zcoord + " named:" + name);
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
            Debug.Log("Max buildings reached");
            return false;
        }
        else
        {
            buildings.Add(b);
            buildingChanged = true;
            return true;
        }
    }

    public bool addHex(Hex h)
    {
        if (possibleHexes.Contains(h) && HexMap.hexes[h.C,h.R].owner == null)
        {
            HexMap.hexes[h.C, h.R].addOwner(owner);
            ownedHexes.Add(h);
            return true;
        }
        return false;
    }
}
