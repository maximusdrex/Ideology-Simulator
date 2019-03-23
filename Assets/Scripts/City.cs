﻿using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class City
{
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
    public List<Citizen> unemployedCitizens;
    public List<Building> buildings;
    public List<Hex> ownedHexes;
    public static int maxRange = 2;
    public List<Hex> possibleHexes;
    public bool buildingChanged;

    public Player owner;


    public City(Hex[,] hexes, bool center, bool capitol, Player owner)
    {
        baseHex = this.getStartingLocation(hexes);

        this.owner = owner;
        ownedHexes = new List<Hex>();
        ownedHexes.Add(baseHex);
        possibleHexes = HexMap.hexesInRange(baseHex, maxRange);

        buildingChanged = true;
        x = baseHex.x;
        z = baseHex.z;
        this.center = center;
        if (center == true)
        {
            maxBuildings++;
        }
        buildings = new List<Building>();
        citizens = new List<Citizen>();
        unemployedCitizens = new List<Citizen>();

        if (center == true)
        {
            buildings.Add(new CityHall("City Hall"));
            Debug.Log("City Hall added");
        }
        this.resources = initializeResources();
    }

    public static List<PlayerResource> initializeResources()
    {
        //resources
        List<PlayerResource> resources =  new List<PlayerResource>();
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
        return resources;
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



    public bool addBuilding(Building b)
    {
        if (buildings.Count == maxBuildings)
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
            HexMap.hexes[h.C, h.R].setCity(this);
            ownedHexes.Add(h);
            return true;
        }
        return false;
    }

    public void startTurn(City c)
    {
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


    public Hex getStartingLocation(Hex[,] hexes)
    {
        bool goodLocation = false;
        int cityX = 0;
        int cityY = 0;
        while (goodLocation == false)
        {
            cityX = Random.Range(0, hexes.GetLength(0) - 1);
            cityY = Random.Range(0, hexes.GetLength(1) - 1);
            TerrainEnum.Terrain terrain = hexes[cityX, cityY].terrain;
            if (terrain != TerrainEnum.Terrain.Ocean && terrain != TerrainEnum.Terrain.Mountain)
            {
                goodLocation = true;
            }
        }
        return hexes[cityX, cityY];
    }

    public Citizen hireCitizen(int edNeeded)
    {
        unemployedCitizens.Sort(Citizen.educationComparison);
        foreach(Citizen u in unemployedCitizens)
        {
            if(u.getEducation() >= edNeeded)
            {
                return u;
            }
        }
        return null;
    }

}
