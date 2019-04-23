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
    public GameObject mesh;

    public Hex getHex()
    {
        return hex;
    }

    public void SetHex(Hex newHex)
    {
        if(this.hex != null)
        {
            this.hex.tileObjs.Remove(this);
        }
        this.hex = newHex;
        newHex.tileObjs.Add(this);
    }

    public GameObject GetUI()
    { 
        return null;
    }
}

