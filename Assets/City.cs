using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City {
    public int x;
    public int y;

    int populationModifier;
    List<Citizen> citizens;
    List<Building> building;
    
    public City(int xcoord, int ycoord)
    {
        x = xcoord;
        y = ycoord;
    }
}
