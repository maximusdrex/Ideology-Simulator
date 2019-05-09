using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit: IInteractableObj {

    public Hex hex { get; protected set; }

    public string type;
    public string name;
    public int manPower;
    public int baseStrength;
    public int movement = 3;
    public int movementRemaining = 3;
    public GameObject model;
    public static PlayerManager pm;

    public Unit(string type)
    {
        if(pm == null) {
            pm = Camera.main.GetComponent<PlayerManager>();
        }

        this.type = type;
        name = "Worker: Manpower: " + manPower;
    }

    public void startTurn()
    {
        movementRemaining = movement;
    }

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
        newHex.tileUnits.Add(this);
    }

    public bool movementCheck(Hex nextHex)
    {
        int dist = hex.getHexDistance(nextHex);
        Debug.Log("Movement Remaining: " + movementRemaining + " Dist:" + dist);
        if(dist <= movementRemaining)
        {
            movementRemaining -= dist;
            return true;
        }
        return false; 
    }

    public GameObject GetUI()
    {
        pm.rememberedUnit = this;
        return (GameObject)Resources.Load("UnitUI");
    }

    public string GetName()
    {
        return name;
    }
}