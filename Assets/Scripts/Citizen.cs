using System;
using System.Collections;
using UnityEngine;

public class Citizen
{
    private int maxEducation = 2;
    public City livingIn;
    public string firstName;
    public string lastName;
    public int turnsSinceFed;
    private float health;
    private int education;
    public int age;
    public int timeAtCurrentJob;
    public Improvement currentJob;
    public Building currentBuildingJob;
    //-1 female, 1 male, 0 nonbinary
    public int gender;
    public double wealth;
    static string[] firstlines = System.IO.File.ReadAllLines(@"Assets/TextResource/firstNames.txt");
    static string[] lastlines = System.IO.File.ReadAllLines(@"Assets/TextResource/lastNames.txt");
    static double foodAmount = 2;

    public Citizen(City c) {
        health = 100;
        education = 0;
        wealth = 0;
        age = 18;
        float genderRand = UnityEngine.Random.Range(0, 100);
        string [] names = getName();
        firstName = names[0];
        lastName = names[1];
        livingIn = c;
    }

    public Citizen createChild()
    {
        Citizen c = new Citizen(livingIn);
        c.lastName = lastName;
        c.education = education;
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
            int chance = UnityEngine.Random.Range(0, 100);
            if(chance > health)
            {
                return true;
            }
        }
        return false;
    }

    public void getFired()
    {
        livingIn.unemployedCitizens.Add(this);
        timeAtCurrentJob = 0;
        currentJob = null;
        currentBuildingJob = null;
    }

    public string[] getName()
    {
        string first = firstlines[UnityEngine.Random.Range(0, firstlines.Length-1)];
        string last = lastlines[UnityEngine.Random.Range(0, firstlines.Length - 1)];
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

    public void recievePay(double pay, double tax)
    {
        wealth += pay * 1-tax;
        livingIn.money += tax * pay;
    }

    public void recieveFood(double food)
    {
        if (food >= (foodAmount)){

        }
    }


    public void takeMedicine (float healthToAdd)
    {
        //since health is a percentage, maximum is 100. 
        health = Mathf.Min(healthToAdd + health, 100);
    }

    public double returnAppeal()
    {
        double healthFromAppeal = 40*(health / 100);

    }

    public static Comparison<Citizen> educationComparison = delegate(Citizen object1, Citizen object2)
    {
        return object1.education.CompareTo(object2.education);
    };

    public static Comparison<Citizen> wealthComparison = delegate (Citizen object1, Citizen object2)
    {
        return object1.wealth.CompareTo(object2.wealth);
    };

    public static Comparison<Citizen> jobTimeComparison = delegate (Citizen object1, Citizen object2)
    {
        return object1.timeAtCurrentJob.CompareTo(object2.timeAtCurrentJob);
    };

}
