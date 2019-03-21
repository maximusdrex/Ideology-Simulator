using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityHall : Building

{
    public new float span = .5f;


    public CityHall(string name) : base(name)
    {
        model = getModel();
    }

    public GameObject getModel()
    {
        return (GameObject)Resources.Load("cityHall");
    }
}
