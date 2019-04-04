
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    //FOR DEBUG REENABLE HEX TEXT ON 126 AND 80
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
    public GameObject RiverModel;
    readonly static int numRows = 54;
    readonly static int numCollumns = 84;
    readonly float heightAdd = .33f;
    readonly float heightMinus = .35f;
    readonly float heightDropOff = .6f;
    readonly float globalSeaLevel = .5f;
    readonly float mountainHeight = .8f;
    readonly float moistureDropOff = 1.5f;
    readonly int numLakes = 4;
    readonly int waterRange = 3;
    float seed = 100;
    float moistureSeed = 97;

    /// <summary>
    /// creates a 2D array of Hexes
    /// </summary>
    public static Hex[,] hexes;

    public Dictionary<Hex, GameObject> hexToGameObject;

    private void Awake()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Random.InitState(0);
        hexes = new Hex[numCollumns, numRows];
        hexToGameObject = new Dictionary<Hex, GameObject>();
        createMap(cameraPosition);
        createHeightMap(seed);
        //removes single tile islands
        removeFeatures(3, globalSeaLevel, -.01f);

        //removes single tile lakes
        removeFeatures(3, globalSeaLevel, .01f);

        //does it again
        removeFeatures(3, globalSeaLevel, -.01f);
        removeFeatures(3, globalSeaLevel, .01f);

        //make mountains require ranges
        removeFeatures(2, 3, mountainHeight, -.01f);
        removeFeatures(1, 2, mountainHeight, -.01f);

        generateLakes(numLakes);
        setTempAndMoisture(moistureSeed);
        allocateTerrain();
        colorHexes(cameraPosition);
    }

    void Start()
    {
       
        foreach(KeyValuePair<Hex, GameObject> hexGo in hexToGameObject)
        {
            hexGo.Value.GetComponentInChildren<TextMesh>().text = hexGo.Key.C + " " + hexGo.Key.R;


        }
    }

    public void save()
    {
        Debug.Log("hexmap save, hexes " + hexes.GetLength(0) + "/" + hexes.GetLength(1));
        SaveHexMap.SaveMap(hexes);
    }

    private void Update()
    {
        fixWidths();
    }

    public void fixWidths()
    {
        if (Camera.main.GetComponent<CameraMotion>().hasMoved == true)
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            foreach (KeyValuePair<Hex, GameObject> hexGo in hexToGameObject)
            {
                hexGo.Value.transform.position = hexGo.Key.updatePosition(cameraPosition, numCollumns);
            }
        }
    }

    //simulate an ice age
    //public void tempChange(Vector)
    //{
    //   foreach (Hex h in hexes)
    //        {
    //            h.moisture -= .01f;
    //           h.temp -= .01f;
    //        }
    //    colorHexes(cameraPosition);
    //}



    /// <summary>
    /// Creates a blank Ocean World.
    /// </summary>
    public void createMap(Vector3 cameraPosition)
    {
        Debug.Log("createMap " + numCollumns + "/" + numRows);
        for (int col = 0; col < numCollumns; col++)
        {
            for (int row = 0; row < numRows; row++)
            {
                hexes[col, row] = new Hex(col, row);
                Vector3 position = hexes[col, row].updatePosition(cameraPosition, numCollumns);
                GameObject hexObject = (GameObject)Instantiate(HexModel, position, Quaternion.identity, this.transform);
                hexToGameObject.Add(hexes[col, row], hexObject);
                hexObject.GetComponentInChildren<TextMesh>().text = col + " , " + row;
                hexObject.GetComponentInChildren<TextMesh>().text = "";
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

        foreach (Hex h in hexes)
        {
            GameObject hexGo = hexToGameObject[h];
            float x = hexGo.transform.position.x;
            float z = hexGo.transform.position.z;
            float hexHeight = Mathf.PerlinNoise(seed + x, seed + z);

            float distance = h.getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            float maxDistance = hexes[0, 0].getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            float d = Mathf.Min(distance / maxDistance, 1f);
            hexHeight = hexHeight * Mathf.Pow(1 - d, heightDropOff) + heightAdd * (1 - d);
            h.height = hexHeight * Random.Range(.7f, 1.3f);
            if (hexHeight < globalSeaLevel)
            {
                h.moisture = 1f;
            }
            else
            {
                h.moisture = 0f;
            }
        }
    }

    public void generateLakes(int numberOfRivers)
    {
        for (int i = 0; i < numberOfRivers; i++)
        {
            int randomCol = Random.Range(-15, 15);
            int randomRow = Random.Range(-15, 15);
            Hex h = hexes[numCollumns / 2 + randomCol, numRows / 2 + randomRow];
            int numSteps = Random.Range(3, 5);
            h.terrain = TerrainEnum.Terrain.Ocean;
            nextLakeTile(h, numSteps);
        }
    }

    public void nextLakeTile(Hex h, int numSteps)
    {
            if (numSteps <= 0)
        {
            return;
        }

        h.moisture = 1f;
        h.terrain = TerrainEnum.Terrain.Ocean;
        Hex[] neighbors = h.getNeighbors(numRows, numCollumns);
        float elevation = 1f;
        h.height = .4f;
        Hex nextTile = null;
        foreach (Hex neighbor in neighbors)
        {
            if (neighbor.height <= elevation)
            {
                nextTile = neighbor;
                elevation = neighbor.height;
            }
        }
        nextLakeTile(hexes[nextTile.C, nextTile.R], numSteps - 1);
    }

    /// <summary>
    /// Sets the temperature and moisture for each hex
    /// </summary>
    public void setTempAndMoisture(float waterSeed)
    {
        Dictionary<Hex, float> moistureAdjustment = new Dictionary<Hex, float>();
        foreach (Hex h in hexes)
        {
            GameObject hexGo = hexToGameObject[h];
            float x = hexGo.transform.position.x;
            float z = hexGo.transform.position.z;
            //Minimum distance to one of the poles
            float distance = Mathf.Min(h.getEuclideanDistance(hexes[h.C, 0]), h.getEuclideanDistance(hexes[h.C, numRows - 1]));

            distance = distance * Random.Range(.7f, 1.3f);

            //maxDistance is from middle tile to polar
            float maxPolarDistance = hexes[numCollumns / 2, 0].getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            float d = Mathf.Min(distance / maxPolarDistance, 1f);
            h.temp = d;

            //distance = Mathf.Min(h.getEuclideanDistance(hexes[0, h.R]), h.getEuclideanDistance(hexes[numColelumns - 1, h.R]));
            //float maxCoastalDistance = hexes[0, 0].getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            //d = Mathf.Miny(dexGo.Keistance / maxCoastalDistance, 1f);
            //d = (1 - d) * Random.Range(.7f, 1.3f);
            //float noiseMoisture = hhf.PerlinNoise(waterSeed + x, waterSeed + z);
            //float moisture = noiseMoisture * .1+ Mathf.Pow(d, moistureDropOff);
            //h.moiure = moisexGo.Keyture;

            if (h.moisture >= .9f && h.height < globalSeaLevel)
            {
                List<Hex> hexesToWater = hexesInRange(h, waterRange);
                foreach (Hex hex in hexesToWater)
                {
                    if (!moistureAdjustment.ContainsKey(hex))
                    {
                        moistureAdjustment.Add(hex, Random.Range(.01f, .05f) * h.getEuclideanDistance(hex));
                    }
                    else
                    {
                        moistureAdjustment[hex] += Random.Range(.01f, .05f) * h.getEuclideanDistance(hex);
                    }

                }
            }

        }

        foreach (KeyValuePair<Hex, float> fixHex in moistureAdjustment)
        {
            hexes[fixHex.Key.C, fixHex.Key.R].moisture = fixHex.Value;
        }
    }




    /// <summary>
    /// Removes features which don't have the requisite number of neighbors
    /// </summary>
    public void removeFeatures(int requiredNeighbors, float requiredHeight, float modifier)
    {
        //Uses 7 as maxneighbors since it's impossible for a hex to have more than 7 neighbors.
        removeFeatures(requiredNeighbors, 7, requiredHeight, modifier);
    }

    public GameObject getHexObj(int x, int y)
    {
        return hexToGameObject[hexes[x, y]];
    }

    public Hex getHexFromObj(GameObject obj)
    {
        if(hexToGameObject.ContainsValue(obj))
        {
            foreach(KeyValuePair<Hex, GameObject> kv in hexToGameObject)
            {
                if(kv.Value == obj)
                {
                    return kv.Key;
                }
            }
            return null;
        } else
        {
            return null;
        }
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

        //Raise or lowerthe hexes
        foreach (Hex hex in hexesToFix)
        {
            hex.height = requiredHeight + modifier * Random.Range(1, 10f);
        }

    }

    /// <summary>
    /// Helper method to get Hexes in a given range
    /// </summary>
    /// <returns>The in range.</returns>
    /// <param name="centerHex">Center hex.</param>
    /// <param name="range">Range.</param>
    public static List<Hex> hexesInRange(Hex centerHex, int range)
    {
        List<Hex> results = new List<Hex>();

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
            {
                int newX = centerHex.C + dx;
                int newY = centerHex.R + dy;
                if (newX < 0)
                {
                    newX = numCollumns + newX;
                }
                if (newX >= numCollumns)
                {
                    newX = newX - numCollumns;
                }
                if (newY < 0)
                {
                    newY = numRows + newY;
                }
                if (newY >= numRows)
                {
                    newY = newY - numRows;
                }

                try { results.Add(hexes[newX, newY]); }
                catch
                {
                    Debug.Log(newX + "," + newY);
                }

            }
        }

        return results;
    }


    /// <summary>
    /// Generates a random rotation from 0,60,120,180,240,300,
    /// </summary>
    /// <returns>The random rotation.</returns>
    public float getRandomRotation()
    {
        int rotation = Random.Range(0, 360);
        rotation = rotation / 60;
        return rotation * 60f;
    }

    public void allocateTerrain()
    {
        //base it off of moisture, set it based of naturally found resources.
        foreach (Hex h in hexes)
        {
            float height = h.height;
            float moisture = h.moisture;
            float temp = h.temp;
            if (moisture >= .66f && h.terrain != TerrainEnum.Terrain.River)
            {
                if (temp >= .75f)
                {
                    h.terrain = TerrainEnum.Terrain.Rainforest;
                }
                if (temp < .75f && temp >= .3f)
                {
                    h.terrain = TerrainEnum.Terrain.Forest;
                }
                if (temp < .3f)
                {
                    h.terrain = TerrainEnum.Terrain.Tundra;
                }

            }
            if (moisture < .66f && moisture >= .33f)
            {
                if (temp >= .66f)
                {
                    h.terrain = TerrainEnum.Terrain.Savannah;
                }
                if (temp < .66f && temp >= .5f)
                {
                    h.terrain = TerrainEnum.Terrain.Grassland;
                }
                if (temp < .5f && temp >= .25f)
                {
                    h.terrain = TerrainEnum.Terrain.Forest;
                }
                if (temp < .25f)
                {
                    h.terrain = TerrainEnum.Terrain.Steppe;
                }
            }
            if (moisture < .33f)
            {
                if (temp >= .75f)
                {
                    h.terrain = TerrainEnum.Terrain.Desert;
                }
                if (temp < .75f && temp >= .25f)
                {
                    h.terrain = TerrainEnum.Terrain.Grassland;
                }
                if (temp < .25f)
                {
                    h.terrain = TerrainEnum.Terrain.Ice;
                }
            }

            if (height < globalSeaLevel && moisture > .9f && h.terrain != TerrainEnum.Terrain.River)
            {
                h.moisture = 1f;
                h.terrain = TerrainEnum.Terrain.Ocean;
            }
            if (temp < .07f)
            {
                h.terrain = TerrainEnum.Terrain.Ice;
            }
            if (height >= mountainHeight)
            {
                h.terrain = TerrainEnum.Terrain.Mountain;
            }
        }
    }
    public GameObject instantiateTerrain(KeyValuePair<Hex, GameObject> hexGo, GameObject model, Material material, Vector3 cameraPosition)
    {

        Destroy(hexGo.Value);
        Vector3 position = hexGo.Key.updatePosition(cameraPosition, numCollumns);
        GameObject hexObject = (GameObject)Instantiate(model, position, Quaternion.identity, this.transform);
        MeshRenderer hexMR = hexObject.GetComponentInChildren<MeshRenderer>();
        hexMR.material = material;
        // rotation = getRandomRotation();
        hexObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        return hexObject;
    }



    public void colorHexes(Vector3 cameraPosition)
    {
        //Creates Dictionary to hold GO's which need to be added after the loop completes
        Dictionary<Hex, GameObject> islandOfMisfitTiles = new Dictionary<Hex, GameObject>();
        foreach (KeyValuePair<Hex, GameObject> hexGo in hexToGameObject)
        {
            Hex h = hexes[hexGo.Key.C, hexGo.Key.R];
            MeshRenderer mr = hexGo.Value.GetComponentInChildren<MeshRenderer>();
            if (h.terrain == TerrainEnum.Terrain.River)
            {
                islandOfMisfitTiles.Add(h, instantiateTerrain(hexGo, RiverModel, savannah, cameraPosition));
            }
            if (h.terrain == TerrainEnum.Terrain.Forest)
            {
                islandOfMisfitTiles.Add(h, instantiateTerrain(hexGo, ForestModel, forest, cameraPosition));
            }
            if (h.terrain == TerrainEnum.Terrain.Tundra)
            {
                islandOfMisfitTiles.Add(h, instantiateTerrain(hexGo, ForestModel, tundra, cameraPosition));
            }
            if (h.terrain == TerrainEnum.Terrain.Mountain)
            {
                islandOfMisfitTiles.Add(h, instantiateTerrain(hexGo, MountainModel, forest, cameraPosition));
            }
            else if (h.terrain == TerrainEnum.Terrain.Ocean)
            {
                mr.material = ocean;
            }
            else if (h.terrain == TerrainEnum.Terrain.Ice)
            {
                mr.material = ice;
            }
            else if (h.terrain == TerrainEnum.Terrain.Rainforest)
            {
                mr.material = rainforest;

            }
            else if (h.terrain == TerrainEnum.Terrain.Desert)
            {
                mr.material = desert;
            }
            else if (h.terrain == TerrainEnum.Terrain.Grassland)
            {
                mr.material = grassland;
            }
            else if (h.terrain == TerrainEnum.Terrain.Savannah)
            {
                mr.material = savannah;
            }
            else if (h.terrain == TerrainEnum.Terrain.Ocean)
            {
                mr.material = ocean;
            }
            else if (h.terrain == TerrainEnum.Terrain.Steppe)
            {
                mr.material = steppe;
            }

        }
        foreach (KeyValuePair<Hex, GameObject> misfit in islandOfMisfitTiles)
        {
            hexToGameObject[misfit.Key] = misfit.Value;
        }
    }
}
