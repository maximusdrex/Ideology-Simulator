using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen
{
    private int maxEducation = 2;
    public string name;
    public int turnsSinceFed;
    private float health;
    private int education;
    public int gender;
    private float wealth;

    public Citizen() {
        health = 100;
        education = 0;
        wealth = 0;
    }

    public void startTurn()
    {

    }

    /// <summary>
    /// Checks if citizen is due for a medical emergency, requiring expenses. 
    /// </summary>
    /// <returns> true or false based on how healthy they are </returns>
    public bool haveHealthEmergency()
    {
        if (health < 99.99f)
        {
            int chance = Random.Range(0, 100);
            if(chance > health)
            {
                return true;
            }
        }
        return false;
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

    public float recievePay(float pay, float tax)
    {
        wealth += pay * 1-tax;
        return pay * tax;
    }

    public void takeMedicine (float healthToAdd)
    {
        //since health is a percentage, maximum is 100. 
        health = Mathf.Min(healthToAdd + health, 100);
    }

}
