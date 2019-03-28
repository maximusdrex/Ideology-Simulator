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
}
