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
    public GameObject RiverModel;
    readonly int numRows = 54;
    readonly int numCollumns = 84;
    readonly float heightAdd = .33f;
    readonly float heightMinus = .35f;
    readonly float heightDropOff = .6f;
    readonly float globalSeaLevel = .5f;
    readonly float mountainHeight = .8f;
    readonly float moistureDropOff = 1.5f;
    readonly int numRivers = 4;

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
        removeFeatures(3, globalSeaLevel, -.01f);

        //removes single tile lakes
        removeFeatures(3, globalSeaLevel, .01f);

        //does it again
        removeFeatures(3, globalSeaLevel, -.01f);
        removeFeatures(3, globalSeaLevel, .01f);

        //make mountains require ranges
        removeFeatures(2, 3, mountainHeight, -.01f);
        removeFeatures(1, 2, mountainHeight, -.01f);

        setTempAndMoisture(moistureSeed);
        generateRivers(8);
        allocateTerrain();
        colorHexes();
        //InvokeRepat2ng("tempChange", 5f, 1f);
    }

    //simulate an ice age
    public void tempChange()
    {
       foreach (KeyValuePair<Hex, GameObject> h in hexToGameObject)
            {
            h.Key.moisture -= .01f;
               h.Key.temp -= .01f;
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
            h.height = hexHeight*Random.Range(.7f,1.3f);
            if(hexHeight < globalSeaLevel)
            {
                h.moisture = 1f;
            }
            else
            {
                h.moisture = 0f;
            }
            //hexGo.GetComponentInChildren<TextMesh>().text =((float)((int)(h.height * 100)) / 100).ToString();
        }
    }

    public void generateRivers(int numberOfRivers)
    {
        for (int i = 0; i < numberOfRivers; i++)
        {
            int randomCol = Random.Range(-10,10);
            int randomRow = Random.Range(-10, 10);
            Hex h = hexes[numCollumns / 2 + randomCol, numRows / 2 + randomRow];
            int numSteps = Random.Range(7, 18);
            nextRiverTile(h, numSteps);
        }
    }

    public void nextRiverTile(Hex h, int numSteps)
    {
        Debug.Log("Hex: " + h + " Elevation: " + h.height + " Num Steps: " + numSteps);
        if (numSteps <= 0)
        {
            return;
        }

        h.moisture = 1f;
        h.terrain = TerrainEnum.Terrain.River;
        Hex[] neighbors = h.getNeighbors(numRows, numCollumns);
        float moisture = 0f;
        Hex nextTile = null;
        foreach(Hex neighbor in neighbors)
        {
            if(neighbor.moisture >= moisture)
            {
                nextTile = neighbor;
                moisture = neighbor.moisture;
            }
        }
        nextRiverTile(hexes[nextTile.C,nextTile.R], numSteps-1);
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
            float distance = Mathf.Min(h.getEuclideanDistance(hexes[h.C, 0]), h.getEuclideanDistance(hexes[h.C, numRows-1]));

            distance = distance * Random.Range(.7f, 1.3f);

            //maxDistance is from middle tile to polar
            float maxPolarDistance = hexes[numCollumns/2, 0].getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            float d = Mathf.Min(distance / maxPolarDistance, 1f);
            h.temp = d;

            distance = Mathf.Min(h.getEuclideanDistance(hexes[0, h.R]), h.getEuclideanDistance(hexes[numCollumns - 1, h.R]));
            float maxCoastalDistance = hexes[0, 0].getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            d = Mathf.Min(distance / maxCoastalDistance, 1f);
            d = (1 - d) * Random.Range(.7f, 1.3f);
            float noiseMoisture = Mathf.PerlinNoise(waterSeed + x, waterSeed + z);
            float moisture = noiseMoisture * .1f + Mathf.Pow(d, moistureDropOff);
            h.moisture = moisture;

            if (h.moisture >= .9f && h.height < globalSeaLevel)
            {
                List<Hex> hexesToWater = waterHexes(h,4);
                foreach(Hex hex in hexesToWater)
                {
                    if (!moistureAdjustment.ContainsKey(hex)) {
                        moistureAdjustment.Add(hex, .05f * h.getEuclideanDistance(h));
                    }
                    else
                    {
                        moistureAdjustment[hex] += .05f * h.getEuclideanDistance(h);
                    }

                }
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

        //Raise or lowerthe hexes
        foreach (Hex hex in hexesToFix)
        {
            hex.height = requiredHeight + modifier;
        }

    }


    public List<Hex> waterHexes(Hex baseHex, int range)
    {
         
        List<Hex> hexesToWater = new List<Hex>();
        for (int x = -1*range; x <= range; x++ )
        {
            int yMin = Mathf.Max(-1*range, -1*x - range);
            int yMax = Mathf.Min(range, -1*x + range);
            for (int y = yMin; y <= yMax; y++)
            {
                int newX = x + baseHex.C;
                int newY = y + baseHex.R;

                if (newX >= numCollumns)
                {
                    newX = newX - numCollumns;
                }
                if (newX < 0)
                {
                    newX = numCollumns + newX;
                }
                if (newY >= numRows)
                {
                    newY = newY - numRows;
                }
                if (newY < 0)
                {
                    newY = numRows + newY;
                }

                if (hexes[newX, newY].moisture < .9f)
                {
                    hexesToWater.Add(hexes[newX, newY]);
                }
            }
        }
        return hexesToWater;
    }


    public float getRandomRotation()
    {
        int rotation = Random.Range(0, 360);
        rotation = rotation / 60;
        return rotation*60f;
    }

    public void allocateTerrain()
    {

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

            if ((height < globalSeaLevel || moisture > .9f) && h.terrain != TerrainEnum.Terrain.River)
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

    public GameObject instantiateTerrain(KeyValuePair<Hex,GameObject> hexGo, GameObject model, Material material)
    {
        Destroy(hexGo.Value);
        GameObject hexObject = (GameObject)Instantiate(model, hexGo.Key.GetPosition(), Quaternion.identity, this.transform);
        MeshRenderer hexMR = hexObject.GetComponentInChildren<MeshRenderer>();
        hexMR.material = material;
        // rotation = getRandomRotation();
       hexObject.transform.rotation = Quaternion.Euler(0f, -60, 0f);
        return hexObject;
    }

    public void colorHexes()
    {
        //Creates Dictionary to hold GO's which need to be added after the loop completes
        Dictionary<Hex, GameObject> islandOfMisfitTiles = new Dictionary<Hex, GameObject>();
        foreach (KeyValuePair<Hex, GameObject> hexGo in hexToGameObject)
        {
            Hex h = hexes[hexGo.Key.C, hexGo.Key.R];
            MeshRenderer mr = hexGo.Value.GetComponentInChildren<MeshRenderer>();
            if (h.terrain == TerrainEnum.Terrain.River)
            {
                islandOfMisfitTiles.Add(h, instantiateTerrain(hexGo, RiverModel, savannah));
            }
            if (h.terrain == TerrainEnum.Terrain.Forest)
            {
                islandOfMisfitTiles.Add(h, instantiateTerrain(hexGo, ForestModel, forest));
            }
            if (h.terrain == TerrainEnum.Terrain.Tundra)
            {
                islandOfMisfitTiles.Add(h, instantiateTerrain(hexGo, ForestModel, tundra));
            }
            if (h.terrain == TerrainEnum.Terrain.Mountain)
            {
                islandOfMisfitTiles.Add(h, instantiateTerrain(hexGo, MountainModel, forest));
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
