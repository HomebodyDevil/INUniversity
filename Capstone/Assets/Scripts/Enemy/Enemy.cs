using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private DefaultEnemyData enemyData;
    [SerializeField] private EnemyBody enemyBody;
    [SerializeField] private EnemySprite enemyVisual;

    private int spawnersID;

    public bool isInBattle;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(enemyData.enemyName);

        isInBattle = false;
    }

    public DefaultEnemyData GetDefaultEnemyData()
    {
        if (enemyData == null)
        {
            Debug.Log("In TestEnemy. Theres no EnemyData");
            return null;
        }
        else
            return enemyData;
    }

    public void SetSpawnersID(int ID)
    {
        spawnersID = ID;
    }

    public int GetSpawnersID()
    {
        return spawnersID;
    }
}
