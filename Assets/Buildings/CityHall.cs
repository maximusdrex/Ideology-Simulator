using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityHall : Building

{

    public CityHall(string name) : base(name)
    {
        model = getModel();
        span = .2f;
    }

    public GameObject getModel()
    {
        return (GameObject)Resources.Load("cityHall");
    }
}
