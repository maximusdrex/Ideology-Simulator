using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit: IInteractableObj {

    public Hex hex { get; protected set; }

    public string type;
    public string name;
    public int manPower;
    public int baseStrength;
    public int movement = 3;
    public int movementRemaining = 3;
    public int health = 10;
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
        if((this.hex).tileUnits[1] == null){

        }
        else
        {
            fight();
        }
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

    public virtual GameObject GetUI()
    {
        pm.rememberedUnit = this;
        return (GameObject)Resources.Load("UnitUI");
    }

    public string GetName()
    {
        return name;
    }

    public void fight()
    {
        System.Random rnd = new System.Random();
        foreach (Unit u in getHex().tileUnits)
        {
            health -= (int)(u.baseStrength * (.75 + (.5 *(rnd.NextDouble()))));
        }
    }
    public virtual void doAction()
    {

    }
}