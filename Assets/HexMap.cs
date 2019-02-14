using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        createMap();
    }

    public Material[] HexTerrains;

    public GameObject HexPrefab;

    readonly int numRows = 20;
    readonly int numCollummns = 40;

    public void createMap()
    {
        for(int col = 0; col < numCollummns; col++)
        {
            for(int row = 0; row< numRows; row++)
            {
                Hex h = new Hex(col, row);
                //Add Hex
                GameObject hexObject = (GameObject) Instantiate(HexPrefab, h.GetPosition(), Quaternion.identity, this.transform);

                MeshRenderer mr = hexObject.GetComponentInChildren<MeshRenderer>();
                mr.material = HexTerrains[Random.Range(0, HexTerrains.Length)];
            }
        }
        //While does cut down on number of batches to render, it produces a gross ripple
        //And doesn't let you move camer
        //StaticBatchingUtility.Combine(this.gameObject);
    }
}
