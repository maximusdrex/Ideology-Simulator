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
        float seed = 10;
        float moistureSeed = 25;
        hexes = new Hex[numCollumns,numRows];
        hexToGameObject = new Dictionary<Hex, GameObject>();
        createMap();
        createHeightMap(seed);
        removeFeatures(3);
        setTempAndMoisture(moistureSeed);
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
            
            distance = Mathf.Min(hexGo.Key.getEuclideanDistance(hexes[0, hexGo.Key.R]), hexGo.Key.getEuclideanDistance(hexes[numCollumns-1, hexGo.Key.R]));
            //Debug.Log(hexGo.Key + " Distance:" + distance);
            float maxCoastalDistance = hexes[0, 0].getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            d = Mathf.Min(distance / maxCoastalDistance, 1f);
            //Debug.Log(hexGo.Key + " Pure D:" + d);
            d = (1-d) * Random.Range(.7f, 1.3f);
            //Debug.Log(hexGo.Key + " D:" + d);
            float noiseMoisture = Mathf.PerlinNoise(waterSeed + x, waterSeed + z);
           //Debug.Log(hexGo.Key + "noise: " + noiseMoisture);
            float moisture = noiseMoisture*.1f + Mathf.Pow(d,moistureDropOff);
            //Debug.Log(hexGo.Key + "moisture" + moisture);
            hexGo.Key.moisture = moisture;

        }
    }

    /// <summary>
    /// Removes features which don't have the requisite number of neighbors
    /// </summary>
    public void removeFeatures(int requiredNeighbors)
    {
        List <Hex> atlanteanHexes = new List<Hex>();
        List<Hex> raptureanHexes = new List<Hex>();

        foreach (Hex h in hexes){
            int numNeighbors = 0;

            //if the height is land
            if(h.height >= landHeight)
            {
                Hex[] neighboringHexes = h.getNeighbors(numRows, numCollumns);
                foreach(Hex neighbor in neighboringHexes)
                {
                    Hex actualNeighbor = hexes[neighbor.C, neighbor.R];
                    if (actualNeighbor.height >= landHeight)
                    {
                        numNeighbors+=1;
                    }
                }
                if (numNeighbors < requiredNeighbors)
                {
                    atlanteanHexes.Add(h);
                }
            }

            //Submerge the hex
            foreach (Hex hex in atlanteanHexes)
            {
                hex.height = landHeight - .01f;
            }

            //if the height is water
            if (h.height < landHeight) {
                Hex[] neighboringHexes = h.getNeighbors(numRows, numCollumns);
                foreach (Hex neighbor in neighboringHexes)
                {
                    Hex actualNeighbor = hexes[neighbor.C, neighbor.R];
                    if (actualNeighbor.height < landHeight)
                    {
                        numNeighbors += 1;
                    }
                }
                if (numNeighbors < requiredNeighbors)
                {
                    raptureanHexes.Add(h);
                }
            }

        }

        //Raise each hex
        foreach (Hex hex in raptureanHexes)
        {
            hex.height = landHeight + .01f;
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

            if (height >= landHeight)
            {
                if(moisture >= .66f)
                {
                    if(temp >= .75f) {
                        mr.material = rainforest;
                    }
                    if(temp < .75f && temp >= .25f)
                    {
                        mr.material = marsh;
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
                        mr.material = forest;
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
