using System;
using System.IO;
using System.Collections;
using UnityEngine;

public class Citizen
{
    private int maxEducation = 2;
    public City livingIn;
    public string firstName;
    public string lastName;
    public int turnsSinceFed;
    public int turnsSinceHoused;
    private double health;
    private int education;
    public int age;
    public int timeAtCurrentJob;
    public Improvement currentJob;
    public Building currentBuildingJob;
    //-1 female, 1 male, 0 nonbinary
    public int gender;
    public double wealth;
    private static string[] firstlines;
    private static string[] lastlines;
    public static double foodAmount = 2;
    public int childTimer;

    public double satisfaction;

    public Citizen(City c) {
        health = 100;
        education = 0;
        wealth = 0;
        age = 0;
        float genderRand = UnityEngine.Random.Range(0, 100);
        if (firstlines == null)
        {
            firstlines  = File.ReadAllLines(@"Assets/TextResources/firstNames.txt");
            lastlines = File.ReadAllLines(@"Assets/TextResources/lastNames.txt");

        }
        string [] names = getName();
        firstName = names[0].Trim();
        lastName = names[1].Trim();
        livingIn = c;
        childTimer = UnityEngine.Random.Range(5, 25);
        Debug.Log("Citizen born: " + firstName + " " + lastName + " living in " + livingIn.name);
    }

    public Citizen createChild()
    {
        Citizen c = new Citizen(livingIn);
        c.lastName = lastName;
        c.education = education;
        return c;
    }

    public void startTurn(bool surplusFood)
    {
        age += 1;
        checkFood();
        haveHealthEmergency();

        if(turnsSinceFed == 0 && returnSatisfaction() > .74f && childTimer <= 0)
        {
            if(surplusFood == true || wealth > 0)
            {
                Citizen child = createChild();
                livingIn.addCitizen(child);

                if(age >= 60)
                {
                    childTimer = 10000;
                }
                else
                {
                    childTimer = education+1 * 8;
                }

            }

        }
        childTimer--;
        if(age >= 100)
        {
            int deathChance = UnityEngine.Random.Range(70, age);
            if(deathChance >= 100)
            {
                die();
            }
        }
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
        string last = lastlines[UnityEngine.Random.Range(0, lastlines.Length - 1)];
        first.Replace(" ", string.Empty);
        last.Replace(" ", string.Empty);
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
        if (food >= foodAmount){
            turnsSinceFed = 0;
        }
        else
        {
            turnsSinceFed++;
        }
    }

    public void checkFood()
    {
        if(turnsSinceFed > 0)
        {
            health -= 10 * turnsSinceFed;
            if (isDead())
            {
                Debug.Log("Citizen " + firstName + " " + lastName + " has died.");

            }
            else
            {
                Debug.Log("Citizen " + firstName + " " + lastName + " is starving! Health is " + health);
            }
        }
    }

    public bool isDead()
    {
        if(health < 0)
        {
            return true;
        }
        return false;
    }

    public void die()
    {
        livingIn.citizens.Remove(this);
        if (currentJob != null)
        {
            currentJob.employeeDied(this);
        }
        else
        {
            livingIn.unemployedCitizens.Remove(this);
        }
    }



    public void takeMedicine (float healthToAdd)
    {
        //since health is a percentage, maximum is 100. 
        health = Mathf.Min(healthToAdd + (float)health, 100);
    }

    public double returnSatisfaction()
    {
        double satisfactionFromHealth = 40*(health / 100);
        double satisfactionFromBasic = 0;
        if(turnsSinceFed == 0 && turnsSinceHoused == 0)
        {
            satisfactionFromBasic = 30;
        }
        double satisfactionFromEducation = 15 * (getEducation() / maxEducation);
        double satisfactionFromLuxuries = 15 * (buyLuxuries());
        satisfaction = satisfactionFromBasic + satisfactionFromHealth + satisfactionFromLuxuries + satisfactionFromEducation;
        satisfaction -= livingIn.satisfactionHitFromWarWeariness();
        if(satisfaction >= 85)
        {
            return 1f;
        }
        if (satisfaction < 85 && satisfaction >=70)
        {
            return .75f;
        }
        if(satisfaction < 70)
        {
            return .5f;
        }
        return 1;
    }

    public int buyLuxuries()
    {
        return 1;
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
