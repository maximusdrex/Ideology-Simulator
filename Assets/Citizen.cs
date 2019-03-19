using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen
{
    public float health;
    private int education;
    private int maxEducation = 2;
    public int gender;

    public Citizen(float health) {
        this.health = health;
        this.education = 0;
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
