﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechGUI : MonoBehaviour
{
    Player player;
    GameManager gm;
    List<Tech> ttree;
    private int nexty;
    private int layerSpace = 180;
    private int ySpace = 50;
    private int maxLayer = 1;
    private int maxY = 100;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = gm.GetPlayer(Camera.main.GetComponent<PlayerManager>().playerId);
        ttree = player.GetTechTree();
        //Debug.Log(player.id);
        setupTree();
        nexty = 30;
        placeChildren(player.getTech("Industrialization"), 0);
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2((maxLayer + 1) * layerSpace, maxY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setupTree()
    {
        if (player.getTech("Industrialization") != null)
        {
            setChildren(player.getTech("Industrialization"));
        } else
        {
            Debug.Log("error tech");
        }
    }

    private void setChildren(Tech parent)
    {
        Debug.Log("setting Children");
        foreach (Tech tech in ttree)
        {
            //Debug.Log(tech.name + " testing if is child of " + parent.name);
            if(tech.GetPrereq() == parent)
            {
                //Debug.Log("set " + tech.name + " as child of " + parent.name);
                parent.AddChild(tech);
                setChildren(tech);
            }
        }
    }

    int placeChildren(Tech tech, int layer)
    {
        //Debug.Log("Placing " + tech.name);
        if(tech.GetChildren().Count < 1)
        {
            //Debug.Log("This was called");
            if(layer > maxLayer)
            {
                maxLayer = layer;
            }
            placeTech(tech, layer, nexty);
            int lasty = nexty;
            nexty += ySpace;
            return lasty;
        } else
        {
            List<int> y = new List<int>();
            int numchild = 0;
            foreach(Tech child in tech.GetChildren())
            {
                int lasty = placeChildren(child, layer + 1);
                y.Add(lasty);
                numchild++;
            }
            int min = Mathf.Min(y.ToArray());
            int max = Mathf.Max(y.ToArray());
            if(max+30 > maxY)
            {
                maxY = max + 30;
            }
            //Debug.Log("Min/Max of " + tech.name + " " + min.ToString() + "/" + max.ToString());
            int newPos = ((min + (max + 30)) / 2) - 15;
            placeTech(tech, layer, newPos);
            return newPos;
        }
    }

    void placeTech(Tech tech, int layer, int y)
    {
        //Debug.Log(tech.name + " placed at y: " + y.ToString());
        int x = layer * layerSpace;
        GameObject Button = Instantiate((GameObject)Resources.Load("TechButtons"));
        Button.transform.SetParent(transform, false);
        Button.transform.position = new Vector3(x, -y);
        //Debug.Log("Set " + tech.name + " to " + x.ToString() + ", " + y.ToString());
        Button.name = tech.name;
        Button.GetComponentInChildren<UnityEngine.UI.Text>().text = tech.name;
        Button.GetComponent<TechSelector>().tech = tech;
    }
}
