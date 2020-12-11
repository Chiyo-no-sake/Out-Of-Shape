using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EnemySpawnController : MonoBehaviour
{
    private bool started = false;
    private List<EnemySpawner> spawners;
    private List<Enemy> spawned;

    public void Start()
    {
        spawned = new List<Enemy>();
        spawners = new List<EnemySpawner>();

        GameObject[] found = GameObject.FindGameObjectsWithTag("EnemySpawner");
        foreach(var go in found)
        {
            spawners.Add(go.GetComponentInChildren<EnemySpawner>());
        }
    }

    private void Update()
    {
        if (spawned.Count == 0) return;

        for(int i=0; i<spawned.Count; i++)
        {
            if(spawned[i] == null)
            {
                spawned.RemoveAt(i--);
            }
        }

        if (IsRoundClear())
        {
            started = false;
            RoundManager.GetInstance().AdvanceRound();
        }
    }

    public bool IsRoundClear()
    {
        return spawned.Count == 0 && started;
    }

    public void StartSpawning(int roundNumber)
    {
        started = true;
        List<Enemy> toSpawn = GenerateEnemies(roundNumber);
        StartCoroutine(SpawnRoutine(toSpawn));
    }

    private IEnumerator SpawnRoutine(List<Enemy> toSpawn)
    {
        int i = 0;
        foreach (var enemy in toSpawn)
        {
            int targetSpawnerId = i++ % spawners.Count;
            if (spawners[targetSpawnerId].IsFree())
            {
                spawned.Add(enemy);
                spawners[targetSpawnerId].Spawn(enemy);

            }

            yield return new WaitForEndOfFrame();
        }
    }

    private List<Enemy> GenerateEnemies(int round)
    {
        //TODO
        List<Enemy> toRet = new List<Enemy>();
        toRet.Add(new CubeEnemy());
        toRet.Add(new CubeEnemy());
        toRet.Add(new CubeEnemy());

        var rand = new System.Random();
        var randomList = toRet.OrderBy(x => rand.Next()).ToList();

        return toRet;
    }
}