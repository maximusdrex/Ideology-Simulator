using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CommunistCity : City
{
    public CommunistCity(Hex[,] hexes, bool center, bool capitol, Player owner) :
    base(hexes, center, capitol, owner)

    {

        name = getName(capitol);
        Debug.Log("City created at: " + x + "," + z + " named:" + name);
    }


    public string getName(bool capitol)
    {
        try
        {
            int rand = 0;
            string[] lines = File.ReadAllLines(@"Assets/TextResources/communistCityNames.txt");
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
            return "Sandersgrad";
        }
    }

    public new void startTurn()
    {
        base.startTurn();
    }
}
