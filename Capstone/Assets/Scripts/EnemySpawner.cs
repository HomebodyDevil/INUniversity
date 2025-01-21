using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Options")]
    [SerializeField] private int spawnerID;
    [SerializeField] private int maxEnemyCount;
    [SerializeField] private List<EnemyForSpawn> spawnEnemyList;
    [SerializeField] private float spawnRangeRad;
    [SerializeField] private float collisionCheckRad;

    [Space(10), Header("Debug Options")]
    [SerializeField] private bool seeDebug = false;
    [SerializeField, Range(1f, 5f)] private float drawLinePeriod;
    [SerializeField, Range(0, 100)] private int spawnChance;

    [SerializeField] private bool isPlayerIn = false;
    [SerializeField] private bool isSpawning = false;

    private float time;
    private float ratio;

    int maxRandomValue;

    private void Awake()
    {
        SceneManagerEX.OnSwitchSceneToBattle -= DisableEnemies;
        SceneManagerEX.OnSwitchSceneToBattle += DisableEnemies;

        SceneManagerEX.OnSwitchSceneToBattle -= StopSpawning;
        SceneManagerEX.OnSwitchSceneToBattle += StopSpawning;

        SceneManagerEX.OnSwitchSceneToMap -= StartSpawning;
        SceneManagerEX.OnSwitchSceneToMap += StartSpawning;

        SceneManagerEX.OnSwitchSceneToMap -= EnableEnemies;
        SceneManagerEX.OnSwitchSceneToMap += EnableEnemies;
    }

    private void OnDestroy()
    {
        SceneManagerEX.OnSwitchSceneToBattle -= DisableEnemies;
        SceneManagerEX.OnSwitchSceneToBattle -= StopSpawning;
        SceneManagerEX.OnSwitchSceneToMap -= StartSpawning;
        SceneManagerEX.OnSwitchSceneToMap -= EnableEnemies;
    }

    void Start()
    {
        RegisterSpawnerToManager();

        maxRandomValue = 0;
        foreach (EnemyForSpawn spawn in spawnEnemyList)
            maxRandomValue += spawn.weight;
        //Debug.Log("ASDFASDFASDF " + maxRandomValue);
        
        
        StartSpawning();
    }

    // Update is called once per frame
    void Update()
    {
        VisualizeArea();
    }

    private void OnTriggerStay(Collider other)
    {
        isPlayerIn = true;
        //if (other.tag == "Player" && !isSpawning)
        //{
        //    isPlayerIn = true;
        //    StartSpawning();
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerIn = false;
        //if (other.tag == "Player" && isSpawning)
        //{
        //    isPlayerIn = false;
        //    StopSpawning();
        //}
    }

    private void VisualizeArea()
    {
        if (!seeDebug) return;

        time = (time + Time.deltaTime) % drawLinePeriod;
        ratio = time / drawLinePeriod;
        //Debug.Log("drawLinePeriod " + drawLinePeriod);
        //Debug.Log("RATIO " + ratio);

        Vector3 lineEndPoint = new Vector3(Mathf.Cos(2 * Mathf.PI * ratio), 0, Mathf.Sin(2 * Mathf.PI * ratio));
        //Debug.Log(lineEndPoint);
        lineEndPoint *= spawnRangeRad;

        Debug.DrawLine(transform.position, transform.position + lineEndPoint, Color.yellow, 0.1f);
    }

    private void RegisterSpawnerToManager()
    {
        Dictionary<int, List<Transform>> spawnersDictionary = EnemySpawnerManager.Instance().GetSpawnersDictionary();

        if (!spawnersDictionary.ContainsKey(spawnerID))
        {
            spawnersDictionary.Add(spawnerID, new List<Transform>());
        }        
    }

    private void EnableEnemies()
    {
        SetHaveEnemies(true);
    }

    private void DisableEnemies()
    {
        SetHaveEnemies(false);
    }

    private void SetHaveEnemies(bool set)
    {
        List<Transform> enemies = EnemySpawnerManager.Instance().GetEnemyList(spawnerID);

        if (enemies == null)
            return;

        foreach (Transform enemy in enemies)
        {
            enemy.gameObject.SetActive(set);
        }
    }
    
    private GameObject SelectRandomEnemy()
    {
        int randValue = UnityEngine.Random.Range(0, maxRandomValue);
        foreach(EnemyForSpawn spawn in spawnEnemyList)
        {
            if (randValue <= spawn.weight)
                return spawn.enemy;
        }

        Debug.Log("Shoud not Reach Here!!!");
        return null;
    }    

    private void SpawnEnemy()
    {
        List<Transform> enemyList = EnemySpawnerManager.Instance().GetEnemyList(spawnerID);

        if (enemyList.Count < maxEnemyCount)
        {
            GameObject enemyForSpawn = SelectRandomEnemy();
            enemyForSpawn = Instantiate(enemyForSpawn);
            enemyForSpawn.transform.SetParent(EnemySpawnerManager.Instance().transform, false);
            enemyForSpawn.GetComponent<Enemy>().SetSpawnersID(spawnerID);

            int iter = 0;
            Vector3 spawnPosition = Vector3.zero;
            do
            {
                if (iter > 5000)
                {
                    Debug.Log("Endless Loop Detected");
                    break;
                }

                int randVal = UnityEngine.Random.Range(0, 101);
                spawnPosition = transform.position + GetRandomPosition() * ((float)randVal / 100);
                //Debug.Log("IN GetRandomPosition Func, spawnPosition : " + spawnPosition);
                //Debug.Log("RandVal : " + (randVal / 100));
                iter++;
            }
            while (!ThereAreObjectInArea(spawnPosition));

            // 랜덤한 위치에 적을 생성할 수 있도록 해보자.
            enemyForSpawn.transform.position = spawnPosition;

            EnemySpawnerManager.Instance().AddEnemyToList(spawnerID, enemyForSpawn.transform);
            //BattleManager.Instance().AddEnemyToFieldEnemiesList(enemyForSpawn.transform);
        }
        //else if (enemyList.Count >= maxEnemyCount)
        //    return;
    }

    private void StartSpawning()
    {
        //Debug.Log("IN StartSpawning Func");
        isSpawning = true;
        StartCoroutine("StartSpawningCoroutine");
    }

    private void StopSpawning()
    {
        isSpawning = false;
        StopCoroutine("StartSpawningCoroutine");
    }

    IEnumerator StartSpawningCoroutine()
    {
        while(true)
        {
            //Debug.Log("IN StartSpawningCoroutine Func");
            int randVal = UnityEngine.Random.Range(0, 100);
            if (randVal < spawnChance)
                SpawnEnemy();

            yield return new WaitForSeconds(1.0f);
        }
    }

    private Vector3 GetRandomPosition()
    {
        float randVal = UnityEngine.Random.Range(0.0f, 1.0f) * (2 * Mathf.PI);
        Vector3 newPos = new Vector3(Mathf.Cos(randVal), 0, Mathf.Sin(randVal));

        return newPos * spawnRangeRad;
    }

    private bool ThereAreObjectInArea(Vector3 position)
    {
        Collider[] enemyColls = Physics.OverlapSphere(position, collisionCheckRad, LayerMask.GetMask("Enemy"));
        Collider[] playerColls = Physics.OverlapSphere(position, collisionCheckRad, LayerMask.GetMask("Player"));

        return enemyColls.Length <= 0 && playerColls.Length <= 0;
    }
}

[Serializable]
public class EnemyForSpawn
{
    public GameObject enemy;
    [Range(0, 100)]public int weight;
}
