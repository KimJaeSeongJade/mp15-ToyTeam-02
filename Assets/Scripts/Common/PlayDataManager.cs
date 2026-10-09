using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDataManager : MonoBehaviour
{
    public void SaveData(PlayDataList playDataList)
    {
        string path = Application.persistentDataPath + "/playdata.json";
        string json = JsonUtility.ToJson(playDataList, true);
        File.WriteAllText(path, json);
    }

    public PlayDataList LoadData()
    {   
        string path = Application.persistentDataPath + "/playdata.json";

        PlayDataList playDataList = null;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            playDataList = JsonUtility.FromJson<PlayDataList>(json);
        }
        
        return playDataList;
    }
}
