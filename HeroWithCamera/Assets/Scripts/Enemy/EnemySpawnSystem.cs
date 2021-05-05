using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnSystem
{
    public GameManager theManager = GameObject.FindObjectOfType<GameManager>();

    private const int kMaxEnemy = 10;

    public int mTotalEnemy = 0;
    private GameObject mEnemyTemplate = null;
    private Vector2 mSpawnRegionMin, mSpawnRegionMax;

    private int mEnemyDestroyed = 0;

    public EnemySpawnSystem(Vector2 min, Vector2 max)
    {
        mEnemyTemplate = Resources.Load<GameObject>("Prefabs/Enemy") as GameObject;
        mSpawnRegionMin = min;
        mSpawnRegionMax = max;
        // GenerateEnemy(); Cannot call from here as WayPoint system is not initialized in EnemyBehavior!
    }

    public void GenerateEnemy()
    {
        for (int i = mTotalEnemy; i < kMaxEnemy; i++)
        {
            GameObject p = GameObject.Instantiate(mEnemyTemplate) as GameObject;
            float x = Random.Range(mSpawnRegionMin.x, mSpawnRegionMax.x);
            float y = Random.Range(mSpawnRegionMin.y, mSpawnRegionMax.y);
            p.transform.position = new Vector3(x, y, 0f);
            mTotalEnemy++;
            theManager.updateEnemyCount(mTotalEnemy);
        }
    }

    public void OneEnemyDestroyed() 
    { 
        mEnemyDestroyed++;  
        ReplaceOneEnemy(); 
    }

    public void ReplaceOneEnemy() 
    { 
        mTotalEnemy--;
        theManager.updateEnemyCount(mTotalEnemy);
        GenerateEnemy(); 
    }

    public string GetEnemyState() 
    { 
        return "  ENEMY: Count(" + mTotalEnemy + ") Destroyed(" + mEnemyDestroyed + ")"; 
    }
}