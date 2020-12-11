using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : IManager
{
    private int _currentRound;
    private EnemySpawnController _spawnController;

    private static RoundManager instance;

    public static RoundManager GetInstance()
    {
        if (instance == null) instance = new RoundManager();
        return instance;
    }

    public void Init()
    {
        _currentRound = 0;
        _spawnController = UnityEngine.GameObject.FindGameObjectWithTag("SpawnController").GetComponent<EnemySpawnController>();    
    }

    public bool IsReady()
    {
        return true;
    }

    public void OnSetupComplete()
    {
        _spawnController.StartSpawning(_currentRound);

    }

    public void AdvanceRound()
    {
        _spawnController.StartSpawning(++_currentRound);
    }

    public int GetRoundNum()
    {
        return _currentRound;
    }
}
