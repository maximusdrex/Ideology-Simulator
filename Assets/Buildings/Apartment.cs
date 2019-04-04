using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apartment : Building
{
    public Apartment(string name, City owner) : base(name, owner)
    {
        model = getModel();
        span = .2f;
        type = "apartment";
    }

    public GameObject getModel()
    {
        return (GameObject)Resources.Load("apartment");
    }
}
