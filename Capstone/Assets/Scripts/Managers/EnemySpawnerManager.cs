using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    private static EnemySpawnerManager instance;

    private Dictionary<int, List<Transform>> spawnersDictionary;

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Awake()
    {
        Initialize();
        spawnersDictionary = new Dictionary<int, List<Transform>>();
    }

    public static EnemySpawnerManager Instance()
    {
        return instance;
    }

    public Dictionary<int, List<Transform>> GetSpawnersDictionary()
    {
        return spawnersDictionary;
    }

    public void AddEnemyToList(int spawnerID, Transform enemy)
    {
        if (spawnersDictionary.ContainsKey(spawnerID))
        {
            spawnersDictionary[spawnerID].Add(enemy);
        }
        else
        {
            Debug.Log("There is no valid Spanwer. check ID or enemy");
        }
    }

    public List<Transform> GetEnemyList(int spawnerID)
    {
        if (spawnersDictionary.ContainsKey(spawnerID))
        {
            return spawnersDictionary[spawnerID];
        }
        else
        {
            Debug.Log("There is no valid Spanwer. check ID");
            return null;
        }
    }
}
