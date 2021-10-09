using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveData (string profile) 
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/data" + profile + ".txt";
        Debug.Log(path);

        FileStream stream = new FileStream(path, FileMode.Create);

        DataGlobal data = new DataGlobal();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static DataGlobal LoadData(string profile)
    {
        string path = Application.persistentDataPath + "/data" + profile + ".txt";
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

/*
    public static void CreateFileToLoad(Profile loadProfile)
    {
        string path = Application.persistentDataPath + "/loadProfile.txt";
        Debug.Log("criou");
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, loadProfile);
        stream.Close();
    }

    public static Profile LoadFileToLoad()
    {
        string path = Application.persistentDataPath + "/loadProfile.txt";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        Profile profileToLoad = formatter.Deserialize(stream) as Profile;
        
        stream.Close();
        File.Delete(path);
        
        return profileToLoad;
    }
*/
    public static void DeleteSaveData(string path)
    {
        File.Delete(path);
    }
}