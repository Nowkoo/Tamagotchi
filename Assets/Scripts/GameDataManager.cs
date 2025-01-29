using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public CatStats catStats;

    void Awake()
    {
        LoadGame();
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame();
        }
        else
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        catStats.SaveData();
    }

    public void LoadGame()
    {
        catStats.LoadData();
    }
}
