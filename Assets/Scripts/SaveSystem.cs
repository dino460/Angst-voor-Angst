using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveData () 
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/data.txt";
        Debug.Log(path);

        FileStream stream = new FileStream(path, FileMode.Create);

        DataGlobal data = new DataGlobal();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static DataGlobal LoadData()
    {
        string path = Application.persistentDataPath + "/data.txt";
        if(File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            DataGlobal data = formatter.Deserialize(stream) as DataGlobal;
            stream.Close();

            return data;

        } else {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}