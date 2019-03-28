using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public float span;
    public string name;
    public string type;
    public GameObject model;
    public City owner;

    public Building(string name, City owner)
    {
        this.name = name;
        this.owner = owner;
    }


}
