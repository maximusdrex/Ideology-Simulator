using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMarket { 

    public List<Corporation> corporations;
    float wealthShareToBuy = 5;


    public GlobalMarket()
    {
        corporations = new List<Corporation>();
    }


    public static Improvement searchImprovementsForUnsold(List <Improvement> improvements, PlayerResource resourceType)
    {
        improvements.Sort(Improvement.amountComparison);
        foreach (Improvement I in improvements)
        {
            if (I.resource.Equals(resourceType))
            {
                if (I.resource.getAmount() > 0)
                {
                    return I;
                }
            }
        }
        return null;
    }

    public static Producer searchProducersForUnsold(List<Producer> producers, PlayerResource resourceType)
    {
        producers.Sort(Producer.amountComparison);
        foreach (Producer p in producers)
        {
            if (p.producedResource.Equals(resourceType))
            {
                if (p.producedResource.getAmount() > 0)
                {
                    return p;
                }
            }
        }
        return null;
    }

    public void startTurn()
    {
        List<Producer> producers = new List<Producer>();
        List<Improvement> improvements = new List<Improvement>();
        List<Store> stores = new List<Store>();
        foreach (Corporation c in corporations)
        {
            c.startTurn();
            producers.AddRange(c.producersWithNeed);
            improvements.AddRange(c.improvements);
            stores.AddRange(c.stores);
        }
        foreach(Producer p in producers)
        {
            Improvement.P = p;
            improvements.Sort(Improvement.priceCompare);
            while (p.qouta > 0)
            {
                Improvement I = searchImprovementsForUnsold(improvements, p.neededResource);
                double amountSold = I.resource.spendResource(p.qouta);
                double cost = I.getHarvestCost(amountSold);
                I.recieveMoney(cost);
                p.qouta -= amountSold;
                p.totalCost += cost;
                p.recieveResources(amountSold);
            }
        }

        foreach (Store s in stores)
        {
            Producer.S = s;
            improvements.Sort(Improvement.priceCompare);
            while (s.qouta > 0)
            {
                foreach(PlayerResource p in s.neededResources)
                {
                    Producer P = searchProducersForUnsold(producers, p);
                    double amountSold = P.producedResource.spendResource(s.qouta);
                    double cost = P.getHarvestCost(amountSold);
                    P.recieveMoney(cost);
                    s.cost = cost;
                    s.qouta -= amountSold;
                    s.recieveResources(p.resourceName, amountSold);
                }
            }
        }

        foreach(Corporation c in corporations)
        {
            c.totalCosts();
        }

        corporations.Sort(Corporation.wealthComparison);
        foreach (Corporation c in corporations)
        {
            for (int i = 0; i < corporations.Count; i++)
            {
                if(corporations[i].money < c.money * wealthShareToBuy)
                {
                    Corporation sold = corporations[i];
                    corporations.Remove(sold);
                    c.improvements.AddRange(sold.improvements);
                    c.producers.AddRange(sold.producers);
                    c.stores.AddRange(sold.stores);
                    sold.sellTo(c);
                    break;
                }
            }


        }
    }
}
