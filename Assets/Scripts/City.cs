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
    public double GDP;
    public double tax;
    public List<PlayerResource> resources;

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
        initializeResources();
        Debug.Log("City created at: " + xcoord + "," + zcoord + " named:" + name + " with all resources. ");
    }

    private void initializeResources()
    {
        //resources
        resources = new List<PlayerResource>();
        resources.Add(new PlayerResource("money"));
        resources.Add(new PlayerResource("food")); //1 million meals = 1
        resources.Add(new PlayerResource("lumber")); //1000 cubic meters = 1
        resources.Add(new PlayerResource("iron")); //1000 tons = 1
        resources.Add(new PlayerResource("steel")); //1000 tons = 1
        resources.Add(new PlayerResource("coal")); //1000 tons  = 1
        resources.Add(new PlayerResource("oil")); //1000 barrels = 1
        resources.Add(new PlayerResource("uranium")); //1 ton = 1
        resources.Add(new PlayerResource("aluminum")); //1000 tons = 1
        resources.Add(new PlayerResource("stone")); //1000 tons = 1
        resources.Add(new PlayerResource("luxury_metals")); //10 pounds = 1
        resources.Add(new PlayerResource("transport")); //ability to transport 1,000,000 tons per year = 1
        resources.Add(new PlayerResource("electronics")); //1:1 electronics required per transport, somewhat arbitrary 
        resources.Add(new PlayerResource("fuel")); //1000 barrels = 1
        resources.Add(new PlayerResource("plastic")); //1000 tons = 1
    }

    public PlayerResource getResource(string name)
    {
        foreach (var resource in resources)
        {
            if (resource.resourceName.Equals(name))
            {
                return resource;
            }
        }
        //default just in case it failed
        return resources[0];
    }

    public string getName(bool capitalist, bool capitol)
    {
        try
        {
            //Pass the file path and file name to the StreamReader constructorbase
            if(capitalist == false)
            {
                int rand = 0;
                string[] lines = System.IO.File.ReadAllLines(@"Assets/communistCityNames.txt");
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
                string[] lines = System.IO.File.ReadAllLines(@"Assets/capitalistCityNames.txt");
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
        if (possibleHexes.Contains(h) && HexMap.hexes[h.C, h.R].owner == null)
        {
            HexMap.hexes[h.C, h.R].addOwner(owner);
            ownedHexes.Add(h);
            return true;
        }
        return false;
    }

    public void startTurn(City c)
    {
        //add all resources.
        foreach (var resource in resources)
        {
            resource.setResource(resource.getAmount() + resource.getDamount());
        }
        //calculate GDP
        c.GDP = (c.getResource("food").getDamount() * 2000000);
        c.GDP += (c.getResource("lumber").getDamount() * 500000);
        c.GDP += (c.getResource("iron").getDamount() * 1200000);
        c.GDP += (c.getResource("steel").getDamount() * 150000);
        c.GDP += (c.getResource("coal").getDamount() * 37000);
        c.GDP += (c.getResource("oil").getDamount() * 60000);
        c.GDP += (c.getResource("stone").getDamount() * 27500);
        c.GDP += (c.getResource("fuel").getDamount() * 160000);
        c.GDP += (c.getResource("luxury_metals").getDamount() * 200000);
        c.GDP += (c.getResource("plastic").getDamount() * 330000);
        c.GDP += (c.getResource("aluminum").getDamount() * 2100000);
        c.GDP += (c.getResource("electronics").getDamount() * 200000);
        c.GDP += (c.getResource("uranium").getDamount() * 200000);
        c.GDP += (c.getResource("transport").getDamount() * 400000);
    }

}