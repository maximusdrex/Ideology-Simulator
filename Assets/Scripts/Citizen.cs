using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen
{
    private int maxEducation = 2;
    public string firstName;
    public string lastName;
    public int turnsSinceFed;
    private float health;
    private int education;
    public int age;
    //-1 female, 1 male, 0 nonbinary
    public int gender;
    private float wealth;
    static string[] firstlines = System.IO.File.ReadAllLines(@"Assets/TextResource/firstNames.txt");
    static string[] lastlines = System.IO.File.ReadAllLines(@"Assets/TextResource/lastNames.txt");

    public Citizen() {
        health = 100;
        education = 0;
        wealth = 0;
        age = 18;
        float genderRand = Random.Range(0, 100);
        string [] names = getName();
        firstName = names[0];
        lastName = names[1];

    }

    public Citizen createChild()
    {
        Citizen c = new Citizen();
        c.lastName = this.lastName;
        c.education = this.education;
        return c;
    }

    public void startTurn(float pay, float tax)
    {
        age += 1;
        recievePay(pay, tax);


    }

    public string genderAsString()
    {
        if(gender == -1)
        {
            return "female";
        }
        if (gender == 1)
        {
            return "male";
        }
        return "nonbinary";
    }

    public int assignGender(float genderRand)
    {
        if(genderRand > 99.9f)
        {
            return 0;
        }
        if(genderRand < 50f)
        {
            return -1;
        }

        return 1;
        
    }
    /// <summary>
    /// Checks if citizen is due for a medical emergency, requiring expenses. 
    /// </summary>
    /// <returns> true or false based on how healthy they are. </returns>
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

    public string[] getName()
    {
        string first = firstlines[Random.Range(0, firstlines.Length-1)];
        string last = lastlines[Random.Range(0, firstlines.Length - 1)];
        return new string[] { first, last };
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
