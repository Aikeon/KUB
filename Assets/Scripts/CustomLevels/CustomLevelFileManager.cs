using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class CustomLevelFileManager : MonoBehaviour
{
    static private LevelFile CreateLevelFile(Transform KUB)
    {
        LevelFile file = new LevelFile();

        return file;
    }

    static public void SaveLevel(Transform KUB)
    {
        LevelFile levelfile = CreateLevelFile(KUB);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, levelfile);
        file.Close();

        Debug.Log("Game Saved");
    }

    static public void LoadLevel(Transform KUB)
    { 
        // 1
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            LevelFile levelfile = (LevelFile)bf.Deserialize(file);
            file.Close();

            // Resets menu display

            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }
}
