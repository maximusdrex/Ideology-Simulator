using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    /// <summary>
    /// Testing commit.
    /// </summary>

    public Material[] HexTerrains;

    public GameObject HexModel;
    public GameObject MountainModel;
    readonly int numRows = 40;
    readonly int numCollumns = 50;
    readonly float heightAdd = .33f;
    readonly float heightMinus = .35f;
    readonly float heightDropOff = 1f;

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
    }
    /// <summary>
    /// Creates a blank Ocean World.
    /// </summary>
    public void createMap(){
        for (int col = 0; col < numCollumns; col++){
            for (int row = 0; row < numRows; row++){
                hexes[col, row] = new Hex(col, row);
                GameObject hexObject = (GameObject)Instantiate(HexModel, hexes[col,row].GetPosition(0), Quaternion.identity, this.transform);
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
    public void createHeightMap(float seed){
        //Creates Dictionary to hold GO's which need to be added after the loop completes
        Dictionary < Hex, GameObject> islandOfMisfitTiles = new Dictionary<Hex, GameObject>(); 

        foreach (KeyValuePair<Hex, GameObject> hexGo in hexToGameObject)
        {
            float x = hexGo.Value.transform.position.x;
            float z = hexGo.Value.transform.position.z;
            float height = Mathf.PerlinNoise(seed+x,seed+z);

            float distance = hexGo.Key.getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            float maxDistance = hexes[0,0].getEuclideanDistance(hexes[numCollumns / 2, numRows / 2]);
            float d = Mathf.Min( distance / maxDistance, 1f);
            height = height * Mathf.Pow(1-d,heightDropOff) + heightAdd * (1-d);
            //hexGo.Value.GetComponentInChildren<TextMesh>().text =((float)((int)(height * 100)) / 100).ToString();

            if (height > .5)
            {
                MeshRenderer mr = hexGo.Value.GetComponentInChildren<MeshRenderer>();
                mr.material = HexTerrains[1];
            }
            if(height > .8) {
                Destroy(hexGo.Value);
                GameObject hexObject = (GameObject)Instantiate(MountainModel, hexGo.Key.GetPosition(0), Quaternion.identity, this.transform);
                islandOfMisfitTiles.Add(hexGo.Key, hexObject);
            }

        }
        foreach(KeyValuePair<Hex,GameObject> misfit in islandOfMisfitTiles)
        {
            hexToGameObject[misfit.Key] = misfit.Value;
        }
    }


    //public GameObject createHex(float arrayPos){
    //    float floatnumRows = numRows;
    //    int Col = (int)Mathf.Floor(arrayPos / floatnumRows);
    //    int Row = (int)(arrayPos - Col * numCollumns);
    //    Hex h = new Hex(Col, Row);
    //    hexes[(int)arrayPos] = h;
    //    GameObject hexObject = (GameObject)Instantiate(HexModel, h.GetPosition(0), Quaternion.identity, this.transform);
    //    return hexObject;
    //}
    //
    //        float terrain = Random.Range(1, 4);
    //       
    //        //Add Hex

    //        if (terrain > 2 )
    //        {
    //            //Make a Mountain
    //            GameObject hexObject = (GameObject)Instantiate(MountainModel, h.GetPosition(0), Quaternion.identity, this.transform);
    //        }
    //        else
    //        {
    //            
    //        }
    //    }
    //}
    //While does cut down on number of batches to render, it produces a gross ripple
    //And doesn't let you move camer
    //StaticBatchingUtility.Combine(this.gameObject);
}
