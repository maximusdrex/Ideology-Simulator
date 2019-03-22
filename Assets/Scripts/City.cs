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

    //resources
    private double money;
    private double dmoney;
    private double food; //1 million meals = 1
    private double dfood;
    private double lumber; //1000 cubic meters = 1
    private double dlumber;
    private double iron; //1000 tons = 1
    private double diron;
    private double steel; //1000 tons = 1
    private double dsteel;
    private double coal; //1000 tons  = 1
    private double dcoal;
    private double oil; //1000 barrels = 1
    private double doil;
    private double uranium; //1 ton = 1
    private double duranium;
    private double aluminum; //1000 tons = 1
    private double daluminum;
    private double stone; //1000 tons = 1
    private double dstone;
    private double luxury_metals; //10 pounds = 1
    private double dluxury;
    private double transport; //ability to transport 1,000,000 tons per year = 1
    private double dtransport;
    private double electronics; //1:1 electronics required per transport, somewhat arbitrary 
    private double delectronics;
    private double fuel; //1000 barrels = 1
    private double dfuel;
    private double plastics; //1000 tons = 1
    private double dplastics;

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
        c.setMoney(c.getMoney() + c.getDmoney());
        c.setFood(c.getFood() + c.getDfood());
        c.setLumber(c.getLumber() + c.getDlumber());
        c.setIron(c.getIron() + c.getDiron());
        c.setSteel(c.getSteel() + c.getDsteel());
        c.setUranium(c.getUranium() + c.getDuranium());
        c.setStone(c.getStone() + c.getDstone());
        c.setCoal(c.getCoal() + c.getDcoal());
        c.setOil(c.getOil() + c.getDoil());
        c.setAluminum(c.getAluminum() + c.getDaluminum());
        c.setLuxury(c.getLuxury() + c.getDluxury());
        c.setFuel(c.getFuel() + c.getDfuel());
        c.setTransport(c.getTransport() + c.getDtransport());
        c.setElectronics(c.getElecronics() + c.getDelectronics());
        c.setPlastics(c.getPlastics() + c.getDplastics());
        c.GDP = (c.getDfood() * 2000000 + c.getDlumber() * 500000 + c.getDiron() * 1200000 + c.getDsteel() * 150000 + c.getDcoal() * 37000 + c.getDoil() * 60000 + c.getDstone() * 27500 + c.getDfuel() * 160000 + c.getDluxury() * 200000 + c.getDplastics() * 330000 + c.getAluminum() * 2100000 + c.getDelectronics()*200000 + c.getDuranium() * 200000 + c.getDtransport()*400000);
    }

    //getters and setters for resources
    public double getMoney()
    {
        return money;
    }

    public double getDmoney()
    {
        return dmoney;
    }

    public double getFood()
    {
        return food;
    }

    public double getDfood()
    {
        return dfood;
    }

    public double getIron()
    {
        return iron;
    }

    public double getDiron()
    {
        return diron;
    }

    public double getSteel()
    {
        return steel;
    }

    public double getDsteel()
    {
        return dsteel;
    }

    public double getLumber()
    {
        return lumber;
    }

    public double getDlumber()
    {
        return dlumber;
    }

    public double getUranium()
    {
        return uranium;
    }

    public double getDuranium()
    {
        return duranium;
    }

    public double getCoal()
    {
        return coal;
    }

    public double getDcoal()
    {
        return dcoal;
    }

    public double getOil()
    {
        return oil;
    }

    public double getDoil()
    {
        return doil;
    }

    public double getStone()
    {
        return stone;
    }

    public double getDstone()
    {
        return dstone;
    }

    public double getElecronics()
    {
        return electronics;
    }

    public double getDelectronics()
    {
        return delectronics;
    }

    public double getTransport()
    {
        return transport;
    }

    public double getDtransport()
    {
        return dtransport;
    }

    public double getAluminum()
    {
        return aluminum;
    }

    public double getDaluminum()
    {
        return daluminum;
    }

    public double getLuxury()
    {
        return luxury_metals;
    }

    public double getDluxury()
    {
        return dluxury;
    }

    public double getFuel()
    {
        return fuel;
    }

    public double getDfuel()
    {
        return dfuel;
    }

    public double getPlastics()
    {
        return plastics;
    }

    public double getDplastics()
    {
        return dplastics;
    }

    public void setMoney(double money)
    {
        this.money = money;
    }

    public void setDmoney(double dmoney)
    {
        this.dmoney = dmoney;
    } 

    public void setFood(double food)
    {
        this.food = food;
    }

    public void setDfood(double dfood)
    {
        this.dfood = dfood;
    }

    public void setLumber(double lumber)
    {
        this.lumber = lumber;
    }

    public void setDlumber(double dlumber)
    {
        this.dlumber = dlumber;
    }

    public void setIron(double iron)
    {
        this.iron = iron;
    }

    public void setDiron(double diron)
    {
        this.diron = diron;
    }

    public void setSteel(double steel)
    {
        this.steel = steel;
    }

    public void setDsteel(double dsteel)
    {
        this.dsteel = dsteel;
    }

    public void setUranium(double uranium)
    {
        this.uranium = uranium;
    }

    public void setDuranium(double duranium)
    {
        this.duranium = duranium;
    }

    public void setCoal(double coal)
    {
        this.coal = coal;
    }

    public void setDcoal(double dcoal)
    {
        this.dcoal = dcoal;
    }

    public void setOil(double oil)
    {
        this.oil = oil;
    }

    public void setDoil(double doil)
    {
        this.doil = doil;
    }

    public void setStone(double stone)
    {
        this.stone = stone;
    }

    public void setDstone(double dstone)
    {
        this.dstone = stone;
    }

    public void setFuel(double fuel)
    {
        this.fuel = fuel;
    }

    public void setDfuel(double fuel)
    {
        this.dfuel = fuel;
    }

    public void setTransport(double transport)
    {
        this.transport = transport;
    }

    public void setDtransport(double dtransport)
    {
        this.dtransport = dtransport;
    }

    public void setElectronics(double electronics)
    {
        this.electronics = electronics;
    }

    public void setDelectronics(double delectronics)
    {
        this.delectronics = delectronics;
    }

    public void setAluminum(double aluminum)
    {
        this.aluminum = aluminum;
    }

    public void setDaluminum(double daluminum)
    {
        this.daluminum = daluminum;
    }

    public void setLuxury(double luxury_metals)
    {
        this.luxury_metals = luxury_metals;
    }

    public void setDluxury(double dluxury)
    {
        this.dluxury = dluxury;
    }

    public void setPlastics(double plastics)
    {
        this.plastics = plastics;
    }

    public void setDplastics(double dplastics)
    {
        this.dplastics = dplastics;
    }

}
