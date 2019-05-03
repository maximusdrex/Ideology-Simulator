using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CityCanvasDisplayer : MonoBehaviour
{
    public Text cityName;
    public Text populationCount;
    public City ownedBy;
    public List<Text> resourceTexts;
    public List<PlayerResource> resourcesKeepTrack;
    public bool activated;

    void Start()
    {
        activated = false;
    }

    void Update()
    {
        if (activated)
        {
            Debug.Log("Currently Active!");
        }
    }

    public void setCityNameText(string name)
    {
        cityName.text = "  <b>CITY: " + name + "</b>";
    }

    public void setPopulationCountText(int amount)
    {
        populationCount.text = "  Population: " + amount.ToString();
    }

    public void displayResources(List<PlayerResource> resources)
    {
        activated = true;
        for (int i = 0; i < resources.Count; i++)
        {
            resourceTexts[i].text = " " + resources[i].resourceName + ":   " + (resources[i].getAmount());
        }
    }

}