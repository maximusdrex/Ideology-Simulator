using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class defines and does the math to determine the world location for 
/// a given hex tile.
/// </summary>
public class Hex
{
    static readonly float WIDTH_MOD = Mathf.Sqrt(3)/2;
    //c + r + s = 0 (must)
    //cubic collumn
    public readonly int C;
    //cubic row
    public readonly int R;
    //cubic z
    public readonly int S;

    public Hex (int c, int r)
    {
        this.C = c;
        this.R = r;
        this.S = -(c + r);
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
}
