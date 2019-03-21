using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen
{
    private int maxEducation = 2;
    public string name;
    public float health;
    private int education;
    public int gender;
    private float wealth;

    public Citizen(float health) {
        this.health = health;
        education = 0;
        wealth = 0;
    }

    public int getEducation()
    {
        return education;
    }

    public void increaseEducation()
    {
        if(education != maxEducation)
        {
            education++;
        }
    }
}
