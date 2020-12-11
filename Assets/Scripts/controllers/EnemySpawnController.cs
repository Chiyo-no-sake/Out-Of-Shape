using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EnemySpawnController : MonoBehaviour
{
    private bool started = false;
    private List<EnemySpawner> spawners;
    private List<GameObject> spawned;

    public void Start()
    {
        spawned = new List<GameObject>();
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
        StartCoroutine(SpawnRoutine(roundNumber*2));
    }

    private IEnumerator SpawnRoutine(int toSpawn)
    {
        for (int i = 0; i < toSpawn;)
        {
            int targetSpawnerId = i % spawners.Count;
            if (spawners[targetSpawnerId].IsFree())
            {
                spawned.Add(spawners[targetSpawnerId].Spawn());
                i++;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}