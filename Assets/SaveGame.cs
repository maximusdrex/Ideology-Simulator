using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveGame
{
    public static void SaveGame()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistantDataPath + "/HexMap";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(HexMap);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadMap() 
    {
        string path = Application.persistantDataPath + "/HexMap";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found.");
            return null;
        }
    }

}
