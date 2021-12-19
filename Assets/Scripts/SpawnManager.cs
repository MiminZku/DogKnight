using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemy;
    public int curEnemy;
    public int maxEnemy = 30;
    public float spawnTime = 2f;
    public float curTime;
    // Update is called once per frame
    void Update()
    {
        if(curTime >= spawnTime && curEnemy < maxEnemy){
            Spawn();
        }
        curTime += Time.deltaTime;
    }

    void Spawn(){
        curTime = 0;
        curEnemy++;
        int randNum = Random.Range(0,spawnPoints.Length);
        Instantiate(enemy, spawnPoints[randNum]);
    }
}
