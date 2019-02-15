using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents the Hex in the world
/// </summary>
public class Hex
{
    static readonly float WIDTH_MOD = Mathf.Sqrt(3)/2;
    //c + r + s = 0 (must)
    //cubic collumn
    public int C;
    //cubic row
    public int R;
    //cubic z
    public int S;

    public float height;
    public float temp;
    public float moisture;

    public Hex (int c, int r)
    {
        this.C = c;
        this.R = r;
        this.S = -(c + r);
    }

    public override string ToString()
    {
        return "C: " + C + " R:" + R;
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
        return new Vector3(horizontal*(this.C + this.R/2f), 0, vertical * this.R);
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
}
