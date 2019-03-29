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

    public float populationModifier;
    public List<Citizen> citizens;
    public List<Citizen> unemployedCitizens;
    public List<Building> buildings;
    public List<Hex> ownedHexes;
    public static int maxRange = 2;
    public List<Hex> possibleHexes;
    public bool buildingChanged;
    private double importTax = -1;
    private double exportTax = -1;

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

        for(int i = 0; i < 10; i++)
        {
            citizens.Add(new Citizen(this));
        }

        if (center == true)
        {
            buildings.Add(new CityHall("City Hall", this));
            Debug.Log("City Hall added");
        }
        this.resources = initializeResources();

        baseHex.tileObjs.Add(this);
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
        return (GameObject) Resources.Load("CityCanvas");
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

    public void startTurn()
    {
        GDP = 0;
        foreach (var resource in resources)
        {
            resource.setResource(resource.getAmount() + resource.getDamount());
            //calculate GDP
            GDP += resource.harvestCost*resource.getDamount()*tax;
        }

        //calculate GDP
        GDP = (getResource("food").getDamount() * 2000000);
        GDP += (getResource("lumber").getDamount() * 500000);
        GDP += (getResource("iron").getDamount() * 1200000);
        GDP += (getResource("steel").getDamount() * 150000);
        GDP += (getResource("oal").getDamount() * 37000);
        GDP += (getResource("oil").getDamount() * 60000);
        GDP += (getResource("stone").getDamount() * 27500);
        GDP += (getResource("fuel").getDamount() * 160000);
        GDP += (getResource("luxury_metals").getDamount() * 200000);
        GDP += (getResource("plastic").getDamount() * 330000);
        GDP += (getResource("aluminum").getDamount() * 2100000);
        GDP += (getResource("eletronics").getDamount() * 200000);
        GDP += (getResource("uranium").getDamount() * 200000);
        GDP += (getResource("transport").getDamount() * 400000);
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

    public double satisfactionHitFromWarWeariness()
    {
        return 0;
    }
}
