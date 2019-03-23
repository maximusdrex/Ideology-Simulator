using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents the Hex in the world
/// </summary>
[Serializable]
public class Hex
{
    public static float WIDTH_MOD = Mathf.Sqrt(3)/2;
    //c + r + s = 0 (must)
    //cubic collumn
    public int C;
    //cubic row
    public int R;
    //cubic z
    public int S;

    public float x;
    public float z;

    public List<IInteractableObj> tileObjs;

    public float height;
    public float temp;
    public float moisture;
    public TerrainEnum.Terrain terrain;
    public Player owner;
    public Hex (int c, int r)
    {
        this.C = c;
        this.R = r;
        this.S = -(c + r);

        tileObjs = new List<IInteractableObj>();
    }
    /// <summary>
    /// Returns a string that represents the current Hex.
    /// </summary>
    /// <returns>A string that represents the current Hex.
    public override string ToString()
    {
        return "C: " + C + " R:" + R;
    }

    /// <summary>
    /// Determines whether the specified Hex is equal to the current Hex.
    /// </summary>
    /// <param name="h">The Hex to compare with the current Hex.</param>
    /// <returns> true if the specified Hex is equal to the current Hex; otherwise, false.</returns>
    public bool Equals(Hex h)
    {
        if(h.C == C && h.R == R && h.S == S)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Calculates the position in the world of a given hex
    /// </summary>
    /// <returns>The position.</returns>
    public Vector3 GetPosition()
    {
        float radius = 1f;
        float diameter = radius * 2;
        float width = WIDTH_MOD * diameter;
        float horizontal = width;
        float vertical = diameter * .75f;
        Vector3 pos = new Vector3(horizontal * (this.C + this.R / 2f), 0, vertical * this.R);
        this.x = pos.x;
        this.z = pos.z;
        return pos;
    }

    public Vector3 updatePosition(Vector3 cameraPosition, float numCollumns)
    {
        float mapWidth = numCollumns * WIDTH_MOD * 2f;
        Vector3 position = GetPosition();
        float widthsFromCamera = Mathf.Round((position.x - cameraPosition.x) / mapWidth);
        //should be between -.5 and .5
        if (Mathf.Abs(widthsFromCamera) < .5f)
        {
            return GetPosition();
        }
        if (widthsFromCamera > 0)
        {
            widthsFromCamera += .5f;
        }
        else
        {
            widthsFromCamera -= .5f;
        }

        int widthsToFix = (int)widthsFromCamera;
        position.x -= widthsToFix * mapWidth;
        this.x = position.x;
        this.z = position.z;
        return position;
    }

    /// <summary>
    /// Gets the minimum euclidean distance.
    /// </summary>
    /// <returns>The euclidean distance.</returns>
    /// <param name="otherHex">Other hex.</param>
    public float getEuclideanDistance (Hex otherHex) {
        return Mathf.Sqrt(Mathf.Pow(otherHex.C - C, 2f)  + Mathf.Pow(otherHex.R - R, 2f) + (otherHex.C - C)*(otherHex.R - R));
    }

    /// <summary>
    /// Gets the neighbor in a given direction.
    /// </summary>
    /// <returns>The neighbor.</returns>
    /// <param name="numRows">Number rows.</param>
    /// <param name="numCollumns">Number collumns.</param>
    /// <param name="cdir"> Magnitude of c direction.</param>
    /// <param name="rdir">Magnitude of r direction.</param>
    public Hex getNeighbor(int numRows, int numCollumns, int cdir, int rdir){
        int newC = (this.C + cdir);

        int newR = (this.R + rdir);
        if (newC < 0)
        {
            newC = numCollumns + newC;
        }
        if(newC >= numCollumns) {
            newC = newC - numCollumns;
        }

        if (newR < 0) {
            newR = numRows + newR;
        }

        if (newR >= numRows)
        {
            newR = newR - numRows;
        }
        return new Hex(newC, newR);
    }

    /// <summary>
    /// Gets each neighbor
    /// </summary>
    /// <returns>The array of neighbors.</returns>
    /// <param name="numRows">Number rows.</param>
    /// <param name="numCollumns">Number collumns.</param>
    public Hex [] getNeighbors(int numRows, int numCollumns){
        //Hex to the West 
        Hex Hex1 = getNeighbor(numRows, numCollumns, -1, 0);
        //NorthWest
        Hex Hex2 = getNeighbor(numRows, numCollumns, 0, -1);
        //NorthEast
        Hex Hex3 = getNeighbor(numRows, numCollumns, 1, -1);
        //East
        Hex Hex4 = getNeighbor(numRows, numCollumns, 1, 0);
        //SouthEast
        Hex Hex5 = getNeighbor(numRows, numCollumns, 0, 1);
        //SouthWest
        Hex Hex6 = getNeighbor(numRows, numCollumns, -1, 1);

        Hex[] hexNeighbors = new Hex[] { Hex1, Hex2, Hex3, Hex4, Hex5, Hex6 };
        return hexNeighbors;
    }

    static public int [] roundCoordinates(float x, float y)
    {
        float z = -(x + y);
        int rx = Mathf.RoundToInt(x);
        int ry = Mathf.RoundToInt(y);
        int rz = Mathf.RoundToInt(-(x + y));
        int x_diff = (int) Mathf.Abs((float)rx - (float)x);
        int y_diff = (int)Mathf.Abs((float)ry - (float)y);
        int z_diff = (int)Mathf.Abs((float)rz - (float)z);

        if(x_diff > y_diff && x_diff > z_diff)
        {
            rx = -ry - rz;
        }
        else if (y_diff > z_diff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }
        int[] coordArray = new int[] { rx,ry};
        return coordArray;
    }

     static public Hex pixelToHex(float x, float y)
    {
        float q = (Mathf.Sqrt(3f) / 3f * x - 1f/ 3f * y);
        float r = (2f/ 3f * y);
        r = Mathf.Max(0f, r);
        Debug.Log("Q" + q + " R" + r);
        int[] coordArray = roundCoordinates(q, r);
        Debug.Log("Q" + coordArray[0] + " R" + coordArray[1]);
        return new Hex(coordArray[0],coordArray[1]);
    }

    public bool addOwner(Player p)
    {
        if(owner == null)
        {
            owner = p;
            return true;
        }
        return false;
    }

}

