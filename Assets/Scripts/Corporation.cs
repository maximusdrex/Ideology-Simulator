using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corporation
{
    public List<Improvement> improvements;
    public List<Producer> producers;
    public List<Store> stores;
    public List<Producer> producersWithNeed;
    public List<Store> storesWithNeed;
    public List<PlayerResource> resources;
    public List<PlayerResource> finalProducts;
    public double money;
    int turnsUnsold;

    public Corporation(Improvement improvement)
    {
        improvements = new List<Improvement>();
        producers = new List<Producer>();
        producersWithNeed = new List<Producer>();
        storesWithNeed = new List<Store>(); 
        stores = new List<Store>();
        improvements.Add(improvement);
        resources = City.initializeResources();
    }

    public Corporation(Producer producer)
    {
        improvements = new List<Improvement>();
        producers = new List<Producer>();
        producersWithNeed = new List<Producer>();
        storesWithNeed = new List<Store>();
        stores = new List<Store>();
        producers.Add(producer);
        resources = City.initializeResources();
    }

    public Corporation(Store store)
    {
        improvements = new List<Improvement>();
        producers = new List<Producer>();
        producersWithNeed = new List<Producer>();
        storesWithNeed = new List<Store>();
        stores = new List<Store>();
        stores.Add(store);
        resources = City.initializeResources();
    }

    public void harvestAll()
    {
        foreach(Improvement I in improvements)
        {
            I.harvestResource();
        }
    }




    public void startTurn()
    {
        producersWithNeed.Clear();
        harvestAll();
        producers.Sort(Producer.qoutaComparison);
        foreach (Producer p in producers)
        {
            Improvement I = GlobalMarket.searchImprovementsForUnsold(improvements, p.neededResource);
            p.qouta = p.generateQouta();
            double amountSold = I.resource.spendResource(p.qouta);
            double cost = I.getHarvestCost(amountSold);
            I.recieveMoney(cost);
            p.qouta -= amountSold;
            p.recieveResources(amountSold);
            p.updateCost(cost);
            if (p.qouta > 0)
            {
                producersWithNeed.Add(p);
            }
        }
        foreach(Store s in stores)
        {
            Producer p = GlobalMarket.searchProducersForUnsold(producers, s.neededResource);
            s.generateQouta();
            double amountSold = p.resource.spendResource(s.qouta);
            double cost = p.setSalePrice(s.owner, amountSold);
            p.recieveMoney(cost);
            s.qouta -= amountSold;
            s.recieveResources(amountSold);
            s.cost += cost;
            if (s.qouta > 0)
            {
                storesWithNeed.Add(s);
            }
        }
        foreach(Improvement I in improvements) {
            money += I.cleanUp();
        }


    }


}
