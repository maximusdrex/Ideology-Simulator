using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Producer : Improvement
{
    public List <PlayerResource> neededResource;
    public Producer (City location, Corporation corp) :  base(location, corp)
    {

    }
}
