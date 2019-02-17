using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{

    public Material rainforest;
    public Material savannah;
    public Material desert;
    public Material marsh;
    public Material forest;
    public Material grassland;
    public Material tundra;
    public Material steppe;
    public Material ice;
    public Material ocean;

    public GameObject HexModel;
    public GameObject MountainModel; 
    public GameObject ForestModel;
    readonly int numRows = 54;
    readonly int numCollumns = 84;
    readonly float heightAdd = .33f;
    readonly float heightMinus = .35f;
    readonly float heightDropOff = .6f;
    readonly float landHeight = .5f;
    readonly float mountainHeight = .8f;
    readonly float moistureDropOff = 1.5f;

    /// <summary>
    /// creates a 2D array of Hexes
    /// </summary>
    public Hex[,] hexes;

    public Dictionary<Hex, GameObject> hexToGameObject;

    // Start is called before the first frame update
    void Start()
    {
        //float seed = Random.Range(1, 100000);
        float seed = 100;
        float moistureSeed = 97;
        hexes = new Hex[numCollumns,numRows];
        hexToGameObject = new Dictionary<Hex, GameObject>();
        createMap();
        createHeightMap(seed);

        //removes single tile islands
        removeFeatures(3, landHeight, -.01f);

        //removes single tile lakes
        removeFeatures(3, landHeight, .01f);

        //does it again
        removeFeatures(3, landHeight, -.01f);
        removeFeatures(3, landHeight, .01f);

        //make mountains require ranges
        removeFeatures(2, 3, mountainHeight, -.01f);
        removeFeatures(1, 2, mountainHeight, -.01f);

        setTempAndMoisture(moistureSeed);
        colorHexes();
        //InvokeRepeating("tempChange", 0f, 1f);
    }

    //simulate an ice age
    public void tempChange()
    {
       foreach (KeyValuePair<Hex, GameObject> hexGo in hexToGameObject)
            {
                hexGo.Key.height -= .1f;
                hexGo.Key.temp += .1f;
            }
        colorHexes();
    }


    /// <summary>
    /// Creates a blank Ocean World.
    /// </summary>
    public void createMap(){
        for (int col = 0; col < numCollumns; col++){
            for (int row = 0; row < numRows; row++){
                hexes[col, row] = new Hex(col, row);
                GameObject hexObject = (GameObject)Instantiate(HexModel, hexes[col,row].GetPosition(), Quaternion.identity, this.transform);
                hexToGameObject.Add(hexes[col, row], hexObject);
                hexObject.GetComponentInChildren<TextMesh>().text = col + " , " + row;
                MeshRenderer mr = hexObject.GetComponentInChildren<MeshRenderer>();
                mr.material = ocean;
            }
        }

    }

    /// <summary>
    /// Creates the height map
    /// </summary>
    public void createHeightMap(float seed)
    {

        foreach (KeyValuePair<Hex, GameObject> hexGo in hexToGameObject)
        {
            float x = hexGo.Value.transform.position.x;
            float z = hexGo.Value.transform.position.z;
            float hexHeight = Mathf.PerlinNoise(seed + x, seed + z);

            float distance = hexGo.Key.getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            float maxDistance = hexes[0, 0].getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            float d = Mathf.Min(distance / maxDistance, 1f);
            hexHeight = hexHeight * Mathf.Pow(1 - d, heightDropOff) + heightAdd * (1 - d);
            hexGo.Key.height = hexHeight;
            if(hexHeight < landHeight)
            {
                hexGo.Key.moisture = 1f;
            }
            else
            {
                hexGo.Key.moisture = 0f;
            }
            //hexGo.Value.GetComponentInChildren<TextMesh>().text =((float)((int)(height * 100)) / 100).ToString();
        }
    }

    /// <summary>
    /// Sets the temperature and moisture for each hex
    /// </summary>
    public void setTempAndMoisture(float waterSeed)
    {
        foreach (KeyValuePair<Hex, GameObject> hexGo in hexToGameObject)
        {
            float x = hexGo.Value.transform.position.x;
            float z = hexGo.Value.transform.position.z;
            //Minimum distance to one of the poles
            float distance = Mathf.Min(hexGo.Key.getEuclideanDistance(hexes[hexGo.Key.C, 0]), hexGo.Key.getEuclideanDistance(hexes[hexGo.Key.C, numRows-1]));

            distance = distance * Random.Range(.7f, 1.3f);

            //maxDistance is from middle tile to polar
            float maxPolarDistance = hexes[numCollumns/2, 0].getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            float d = Mathf.Min(distance / maxPolarDistance, 1f);
            hexGo.Key.temp = d;

            distance = Mathf.Min(hexGo.Key.getEuclideanDistance(hexes[0, hexGo.Key.R]), hexGo.Key.getEuclideanDistance(hexes[numCollumns - 1, hexGo.Key.R]));
            float maxCoastalDistance = hexes[0, 0].getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            d = Mathf.Min(distance / maxCoastalDistance, 1f);
            d = (1 - d) * Random.Range(.7f, 1.3f);
            float noiseMoisture = Mathf.PerlinNoise(waterSeed + x, waterSeed + z);
            float moisture = noiseMoisture * .1f + Mathf.Pow(d, moistureDropOff);
            hexGo.Key.moisture = moisture;

            if (hexGo.Key.moisture >= .9f && hexGo.Key.height < landHeight)
            {
                List<Hex> alreadyMoistened = new List<Hex>();
                waterHexes(alreadyMoistened, hexGo.Key, 3);
            }

        }
    }

    /// <summary>
    /// Removes features which don't have the requisite number of neighbors
    /// </summary>
    public void removeFeatures(int requiredNeighbors, float requiredHeight, float modifier) {
        //Uses 7 as maxneighbors since it's impossible for a hex to have more than 7 neighbors.
        removeFeatures(requiredNeighbors, 7, requiredHeight, modifier);
    }

    /// <summary>
    /// Removes features which don't have the requisite number of neighbors
    /// </summary>
    public void removeFeatures(int requiredNeighbors, int maxNeighbors, float requiredHeight, float modifier)
    {
        List<Hex> hexesToFix = new List<Hex>();
        foreach (Hex h in hexes)
        {

            int numNeighbors = 0;
            //if we're raising the height
            if (modifier > 0)
            {
                //check if it's lower than required
                if (h.height < requiredHeight)
                {
                    Hex[] neighboringHexes = h.getNeighbors(numRows, numCollumns);
                    foreach (Hex neighbor in neighboringHexes)
                    {
                        Hex actualNeighbor = hexes[neighbor.C, neighbor.R];
                        //Needs neighbors of the same height
                        if (actualNeighbor.height < requiredHeight)
                        {
                            numNeighbors += 1;
                        }
                    }
                    if (numNeighbors < requiredNeighbors || numNeighbors > maxNeighbors)
                    {
                        hexesToFix.Add(h);
                    }
                }
            }

            //if we're submerging the height
            if (modifier < 0)
            {
                //check if it's taller than required
                if (h.height >= requiredHeight)
                {
                    Hex[] neighboringHexes = h.getNeighbors(numRows, numCollumns);
                    foreach (Hex neighbor in neighboringHexes)
                    {
                        Hex actualNeighbor = hexes[neighbor.C, neighbor.R];
                        //needs neighbors who are also tall to survive
                        if (actualNeighbor.height >= requiredHeight)
                        {
                            numNeighbors += 1;
                        }
                    }
                    if (numNeighbors < requiredNeighbors || numNeighbors > maxNeighbors)
                    {
                        hexesToFix.Add(h);
                    }
                }
            }
        }

        //Raise or lower the hexes
        foreach (Hex hex in hexesToFix)
        {
            hex.height = requiredHeight + modifier;
        }

    }


    public void waterHexes(List <Hex> alreadyMoistened, Hex targetHex, int charges)
    {
        if(targetHex.height > mountainHeight)
        {
            return;
        }
        if(targetHex.moisture > 1f)
        {
            return;
        }
        if (charges == 0)
        {
            return;
        }
        if (alreadyMoistened.Contains(targetHex))
        {
            return;
        }

        charges--;
        alreadyMoistened.Add(targetHex);
        foreach (Hex h in targetHex.getNeighbors(numRows, numCollumns))
        {
            hexes[h.C,h.R].moisture += charges * .1f;
            if(charges >= 3)
            {
                Debug.Log("Hex: " + h + " Moisture: " + h.moisture);
            }
            waterHexes(alreadyMoistened, h, charges);
        }
    }

    public void colorHexes()
    {
        //Creates Dictionary to hold GO's which need to be added after the loop completes
        Dictionary<Hex, GameObject> islandOfMisfitTiles = new Dictionary<Hex, GameObject>();
        foreach (KeyValuePair<Hex, GameObject> hexGo in hexToGameObject)
        {
            MeshRenderer mr = hexGo.Value.GetComponentInChildren<MeshRenderer>();
            float height = hexGo.Key.height;
            float moisture = hexGo.Key.moisture;
            float temp = hexGo.Key.temp;

            if (height >= landHeight && height < mountainHeight)
            {
                if(moisture >= .66f)
                {
                    if(temp >= .75f) {
                        mr.material = rainforest;
                    }
                    if(temp < .75f && temp >= .25f)
                    {
                        mr.material = forest;
                    }
                    if(temp < .25f) {
                        mr.material = tundra;
                    }
                }
                if (moisture < .66f && moisture >= .33f)
                {
                    if (temp >= .75f)
                    {
                        mr.material = savannah;
                    }
                    if (temp < .75f && temp >= .25f)
                    {
                        Destroy(hexGo.Value);
                        GameObject hexObject = (GameObject)Instantiate(ForestModel, hexGo.Key.GetPosition(), Quaternion.identity, this.transform);
                        islandOfMisfitTiles.Add(hexGo.Key, hexObject);
                        MeshRenderer forestMR = hexObject.GetComponentInChildren<MeshRenderer>();
                        forestMR.material = forest;
                    }
                    if (temp < .25f)
                    {
                        mr.material = steppe;
                    }
                }
                if (moisture < .33f)
                {
                    if (temp >= .75f)
                    {
                        mr.material = desert;
                    }
                    if (temp < .75f && temp >= .25f)
                    {
                        mr.material = grassland;
                    }
                    if (temp < .25f)
                    {
                        mr.material = ice;
                    }
                }
            }

            if(height < landHeight && moisture > .9f)
            {
                mr.material = ocean;
            }
            if (temp < .07f)
            {
                mr.material = ice;
            }

            if (height >= mountainHeight)
            {
                Destroy(hexGo.Value);
                GameObject hexObject = (GameObject)Instantiate(MountainModel, hexGo.Key.GetPosition(), Quaternion.identity, this.transform);
                islandOfMisfitTiles.Add(hexGo.Key, hexObject);
            }
        }
        foreach (KeyValuePair<Hex, GameObject> misfit in islandOfMisfitTiles)
        {
            hexToGameObject[misfit.Key] = misfit.Value;
        }
    }
}
