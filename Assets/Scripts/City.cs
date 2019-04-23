using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class City : IInteractableObj
{
    public Hex baseHex;
    public float x;
    public float z;
    public int maxBuildings = 6;
    public bool center;
    public bool capitol;
    public bool capitalist;
    public string name;
    public double money;
    public double GDP; //.x, for a total cost of 1.x 
    public double tax;
    public List<PlayerResource> resources;
    private double wageTax = -1;
    private double minimumWage = -1;
    public double transport = 0;
    //Calculated by amount a car can transport in one year if driving for 8 hours per day, 300 days per year (As of 2010 this would be 24)
    public double transport_mod = 1;

    public float populationModifier;
    public List<Citizen> citizens;
    public List<Citizen> unemployedCitizens;
    public List<Building> buildings;
    public List<Improvement> nationalizedImprovements;
    public List<Hex> ownedHexes;
    public static int maxRange = 2;
    public List<Hex> possibleHexes;
    public int buildingChanged;
    private double importTax = -1;
    private double exportTax = -1;

    public Player owner;


    public City(Hex[,] hexes, bool center, bool capitol, Player owner)
    {
        baseHex = getStartingLocation(hexes);

        this.owner = owner;
        ownedHexes = new List<Hex>();
        ownedHexes.AddRange(HexMap.hexesInRange(baseHex, maxRange));
        foreach(Hex h in ownedHexes)
        {
            h.setCity(this);
        }
        buildingChanged = 0;
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
        for (int i = 0; i < 10; i++)
        {
            citizens.Add(new Citizen(this));
        }
        unemployedCitizens.AddRange(citizens);

        if (center == true)
        {
            buildings.Add(new CityHall("City Hall", this));
            Debug.Log("City Hall added");
            buildings.Add(new Store("Store", this));
            Debug.Log("Store added");
            buildings.Add(new Apartment("Apartment", this));
            Debug.Log("Apartment added");
            buildingChanged += 3;

        }
        this.resources = initializeResources();

        baseHex.tileObjs.Add(this);
        nationalizedImprovements = new List<Improvement>();
    }

    public static List<PlayerResource> initializeResources()
    {
        //resources
        List<PlayerResource> resources =  new List<PlayerResource>();
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

    public GameObject GetUI()
    {
        GameObject temp =  (GameObject) Resources.Load("CityCanvas");
        temp.GetComponent<CityCanvasDisplayer>().ownedBy = this;
        temp.GetComponent<CityCanvasDisplayer>().setCityNameText(name);
        temp.GetComponent<CityCanvasDisplayer>().displayResources(resources);
        return temp;
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
            buildingChanged++;
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

    public void startTurn()
    {
        GDP = 0;
        transport = 0;
        foreach(Improvement n in nationalizedImprovements)
        {
            n.startTurn();
            n.harvestResource();
            string improvementName = n.resource.resourceName;
            findResource(improvementName).changeDamount(n.resource.getAmount());

        }

        foreach (PlayerResource resource in resources)
        {
            resource.setResource(resource.getAmount() + resource.getDamount());
            //calculate GDP
            GDP += resource.harvestCost*resource.getDamount();
            if(resource.getAmount() > 0)
            {
                foreach (Store s in findBuilding("store"))
                {
                    s.recieveResources(resource.resourceName, resource.damount);
                }
            }
        }
        //Gather resources
        foreach (var hex in ownedHexes)
        {
            if(hex.improvement != null)
            {
                
            }
        }

        //calculate GDP
        GDP = 0;
        foreach (PlayerResource r in resources)
        {
            GDP += (r.getDamount() * r.harvestCost);
            if (r.resourceName.Equals("transport"))
            {
                transport = r.getDamount() * transport_mod;
            }
            //Debug.Log("Transportation: " + transport);
        }
        feedCitizens();
        foreach (Citizen c in citizens)
        {
            c.startTurn();
        }
        cleanUpBodies();
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
            hexes[cityX, cityY].terrain = TerrainEnum.Terrain.Blank;
        }
        return hexes[cityX, cityY];
    }

    public void endTurn()
    {
        buildingChanged = 0;
    }

    public Citizen hireCitizen(int edNeeded)
    {
        unemployedCitizens.Sort(Citizen.educationComparison);
        unemployedCitizens.Reverse();
        foreach(Citizen u in unemployedCitizens)
        {
            if(u.getEducation() >= edNeeded)
            {
                unemployedCitizens.Remove(u);
                return u;
            }
        }
        return null;
    }

    public bool isConnected(City c)
    {
        return true;
    }

    public double getImportTax()
    {
        if (importTax < 0)
        {
            return owner.importTax;
        }
        return importTax;
    }
    public double getExportTax()
    {
        if (exportTax < 0)
        {
            return owner.exportTax;
        }
        return exportTax;
    }

    //Checks if the city has set it's own minimum wage or i/e taxes
    //otherwise defaults to the players
    public double getMinimumWage()
    {
        if (minimumWage < 0)
        {
            return owner.minimumWage;
        }
        return minimumWage;

    }

    public double getWageTax()
    {
        if (wageTax < 0)
        {
            return owner.wageTax;
        }
        return wageTax;
    }

    public void setMinimumWage(double w)
    {
        this.minimumWage = w;
    }

    public void setWageTax(double t)
    {
        this.wageTax = t;
    }

    public List <Building> findBuilding(string type)
    {
        return buildings.FindAll(x => x.type == type);
    }

    public PlayerResource findResource(string name)
    {
        return resources.Find(x => x.resourceName == name);
    }
    public double satisfactionHitFromWarWeariness()
    {
        return 0;
    }

    public virtual void feedCitizens()
    {
        Debug.Log("Default feed citizens called");
    }

    public void cleanUpBodies()
    {
        for (int i = 0; i < citizens.Count; i ++)
        {
            Citizen c = citizens[i];
            if (c.isDead())
            {
                citizens.Remove(c);
            }
        }
    }

    //Changable by techs etc
    public void setTransportMod(double transport_mod)
    {
        this.transport_mod = transport_mod;
    }
}
