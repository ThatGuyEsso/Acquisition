using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerialisationManager
{
    public static bool Save(string saveName,object saveData)
    {
        //Get a formatter
        BinaryFormatter formatter = GetBinaryFormatter();

        //Check if there is already a saves folder at this directory. else create new directory
        if (!Directory.Exists(Application.persistentDataPath + "/tutorialData")){
            Directory.CreateDirectory(Application.persistentDataPath + "/tutorialData");
        }

        //get path to save
        string dirPath = Application.persistentDataPath + "/tutorialData" + saveName + ".saveData";

        //Overwrite file at location
        FileStream file = File.Create(dirPath);
        formatter.Serialize(file, saveData);
        file.Close();
        
        //saving complete
        return true;
    }

    public static object Load(string path)
    {
        //No file found return null
        if (!File.Exists(path)) return null;

        //File found create new formatter and open file
        BinaryFormatter formatter = GetBinaryFormatter();
        FileStream file = File.Open(path,FileMode.Open);

        //try to deserialise file and return save
        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("Failed to load file at {0}", path);
            file.Close();
            return null;
        }

    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        return formatter;
    }
}
