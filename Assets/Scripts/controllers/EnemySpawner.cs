using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab = null;
    [SerializeField] private GameObject enemyTarget = null;

    public GameObject Spawn()
    {
        Debug.Log("Spawning enemy");
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.transform.position = transform.position;
        enemy.GetComponent<AIEnemy>().SetTarget(enemyTarget);

        return enemy;
    }

    public bool IsFree()
    {
        LayerMask layerMask = LayerMask.GetMask("Entities");
        Debug.DrawLine(transform.position * 0.7f, transform.position * 0.7f + transform.position.normalized * 50, Color.red, 10);
        return !Physics.SphereCast(new Ray(transform.position*0.7f, transform.position.normalized), 10f, 50.0f, layerMask);
    }
}