using System.Collections;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;

[Serializable]
public class WrappedArray
{
    public Hex[] Hexes;
    public static WrappedArray Wrap(Hex[,] hexes)
    {
        var wrapped = new Hex[hexes.GetLength(0) * hexes.GetLength(1)];
        for (int i = 0; i < hexes.GetLength(0); i++)
        {
            for (int j = 0; j < hexes.GetLength(1); j++)
            {
                wrapped[i * hexes.GetLength(1) + j] = hexes[i, j];
            }
        }
        return new WrappedArray { Hexes = wrapped };
    }
}

public static class SaveHexMap
{
    //Takes HexData object and serializes it, which converts it to binary, feeds that into a printstream
    public static void SaveMap(Hex[,] hexes)
    {
        var xx = WrappedArray.Wrap(hexes);
        Debug.Log(hexes.ToString() + xx.Hexes.Length);
        Debug.Log(xx.Hexes[0].ToString());
        string serialized = JsonUtility.ToJson(xx, true);
        string path = Path.Combine(Application.persistentDataPath, "HexMap.xd");
        Debug.Log("SaveMap " + path);
        Debug.Log(serialized);
        File.WriteAllText(path, serialized);
    }

    public static Hex[,] LoadMap()
    {
        string path = Path.Combine(Application.persistentDataPath, "HexMap.xd");
        if (File.Exists(path))
        {
            string serialized = File.ReadAllText(path);
            var data = JsonUtility.FromJson<Hex[,]>(serialized);
            Debug.Log(data);
            return data;
        }
        else
        {
            //Debug.LogError("Save file not found.");
            return null;
        }
    }

}
