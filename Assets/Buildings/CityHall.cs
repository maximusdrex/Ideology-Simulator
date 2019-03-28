using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityHall : Building

{

    public CityHall(string name, City owner) : base(name, owner)
    {
        model = getModel();
        span = .2f;
        type = "cityhall";
    }

    public GameObject getModel()
    {
        return (GameObject)Resources.Load("cityHall");
    }
}
