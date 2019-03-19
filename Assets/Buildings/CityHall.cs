using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityHall : Building

{
    public new float span = .5f;
    public GameObject CityHallModel;


    public CityHall(string name) : base(name)
    {
        this.CityHallModel = getModel();
    }

    public GameObject getModel()
    {
        return (GameObject) Resources.Load("Assets/Resources/cityHall.fbx");
    }
}
