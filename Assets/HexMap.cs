using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{

    public Material[] HexTerrains;

    public GameObject HexModel;
    public GameObject MountainModel;
    readonly int numRows = 40;
    readonly int numCollumns = 50;
    readonly float heightAdd = .33f;
    readonly float heightMinus = .35f;
    readonly float heightDropOff = 1f;
    readonly float landHeight = .5f;
    readonly float mountainHeight = .8f;

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
        hexes = new Hex[numCollumns,numRows];
        hexToGameObject = new Dictionary<Hex, GameObject>();
        createMap();
        createHeightMap(seed);
        removeFeatures(2);
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
                mr.material = HexTerrains[3];
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
                Debug.Log(h);
                foreach(Hex neighbor in neighboringHexes)
                {
                    Hex actualNeighbor = hexes[neighbor.C, neighbor.R];
                    if (actualNeighbor.height >= landHeight)
                    {
                        numNeighbors+=1;
                        Debug.Log(numNeighbors);
                    }
                }
                if (numNeighbors < requiredNeighbors)
                {
                    atlanteanHexes.Add(h);
                }
            }

            //if the height is water
            if (h.height < landHeight) {
                Hex[] neighboringHexes = h.getNeighbors(numRows, numCollumns);
                Debug.Log(h);
                foreach (Hex neighbor in neighboringHexes)
                {
                    Hex actualNeighbor = hexes[neighbor.C, neighbor.R];
                    if (actualNeighbor.height < landHeight)
                    {
                        numNeighbors += 1;
                        Debug.Log(numNeighbors);
                    }
                }
                if (numNeighbors < requiredNeighbors)
                {
                    raptureanHexes.Add(h);
                }
            }

        }

        //Submerge the hex
        foreach(Hex hex in atlanteanHexes) {
            hex.height = landHeight - .01f;
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
            float height = hexGo.Key.height;
            if (height >= landHeight)
            {
                MeshRenderer mr = hexGo.Value.GetComponentInChildren<MeshRenderer>();
                mr.material = HexTerrains[1];
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
