using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit: IInteractableObj {

    public Hex hex { get; protected set; }

    public string name;
    public int manPower;
    public int baseStrength;
    public int movement = 2;
    public int movementRemaining = 2;

    public void SetHex(Hex hex)
    {
        if(hex != null)
        {
            hex.tileObjs.Remove(this);
        }
        this.hex = hex;
        hex.tileObjs.Add(this);
    }

    public GameObject GetUI()
    { 
        return null;
    }
}

